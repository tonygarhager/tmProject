using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.FGA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class SqlStorage : DbStorage, IAlignableStorage, IStorage, IDisposable
	{
		internal enum SchemaVersionCompatibility
		{
			Unknown,
			Compatible,
			Older,
			Newer
		}

		internal const string CurrentVersion = "11.0.11";

		private const long TuContextUnassigned = -1L;

		private bool _versionChecked;

		public bool DeleteTmRequiresEmptyTm => false;

		public bool IsReadOnly => false;

		protected override bool CanReportReindexRequired => true;

		public SqlStorage(string connectionString)
		{
			_conn = new SqlConnection(connectionString);
			_versionChecked = false;
		}

		public void Flush()
		{
		}

		public void CreateSchema()
		{
			ExecuteSchemaCommand("Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.SQL.CreateSchemaSQL.sql", ignoreErrors: false);
			SetParameter("VERSION", "11.0.11");
		}

		private bool ProcedureExists(string procedureName)
		{
			string cmdText = "SELECT COUNT(*) FROM information_schema.routines WHERE routine_name = '" + procedureName + "' AND routine_type = 'PROCEDURE'";
			using (SqlCommand sqlCommand = CreateCommand(cmdText, requiresTransaction: false))
			{
				return int.Parse(sqlCommand.ExecuteScalar().ToString()) == 1;
			}
		}

		public void DropSchema()
		{
			if (ProcedureExists("get_tms"))
			{
				foreach (int tmId in GetTmIds())
				{
					DeleteTm(tmId, ignoreErrors: true, checkVersion: false);
					DeleteTmSchema(tmId);
				}
			}
			ExecuteSchemaCommand("Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.SQL.DropSchemaSQL.sql", ignoreErrors: true);
		}

		public bool SchemaExists()
		{
			using (SqlCommand sqlCommand = CreateCommand("SELECT COUNT(*) FROM sysobjects WHERE name = 'translation_memories' AND type = 'U'", requiresTransaction: false))
			{
				return (int)sqlCommand.ExecuteScalar() > 0;
			}
		}

		public bool AddTm(TranslationMemory tm)
		{
			CheckVersion();
			if (tm.TextContextMatchType == (TextContextMatchType)0)
			{
				throw new Exception("TextContextMatchType must be specified");
			}
			if (tm.DataVersion != 1)
			{
				throw new Exception("A server-based TM must always be created using the latest DataVersion");
			}
			BooleanSettingsWrapper booleanSettingsWrapper = new BooleanSettingsWrapper(tm.Recognizers, tm.TokenizerFlags, tm.WordCountFlags);
			using (SqlCommand sqlCommand = CreateCommand("dbo.add_tm", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@guid", SqlDbType.UniqueIdentifier).Value = tm.Guid;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = tm.Name;
				sqlCommand.Parameters.Add("@src_lang", SqlDbType.NVarChar).Value = tm.LanguageDirection.SourceCultureName;
				sqlCommand.Parameters.Add("@trg_lang", SqlDbType.NVarChar).Value = tm.LanguageDirection.TargetCultureName;
				sqlCommand.Parameters.Add("@copyright", SqlDbType.NVarChar).Value = tm.Copyright;
				sqlCommand.Parameters.Add("@description", SqlDbType.NVarChar).Value = tm.Description;
				sqlCommand.Parameters.Add("@settings", SqlDbType.Int).Value = booleanSettingsWrapper.DbSettingsValue;
				sqlCommand.Parameters.Add("@cru", SqlDbType.NVarChar).Value = tm.CreationUser;
				sqlCommand.Parameters.Add("@crd", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tm.CreationDate);
				if (tm.ExpirationDate.HasValue)
				{
					sqlCommand.Parameters.Add("@expd", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tm.ExpirationDate.Value);
				}
				sqlCommand.Parameters.Add("@fuzzy_indexes", SqlDbType.Int).Value = (int)tm.FuzzyIndexes;
				sqlCommand.Parameters.Add("@fga_support", SqlDbType.Int).Value = (int)tm.FGASupport;
				sqlCommand.Parameters.Add("@data_version", SqlDbType.Int).Value = 1;
				sqlCommand.Parameters.Add("@text_context_match_type", SqlDbType.Int).Value = (int)tm.TextContextMatchType;
				sqlCommand.Parameters.Add("@id_context_match", SqlDbType.Bit).Value = tm.IdContextMatch;
				try
				{
					tm.Id = (int)sqlCommand.ExecuteScalar();
					CreateTmSchema(tm.Id);
					return true;
				}
				catch (SqlException ex)
				{
					if (ex.Number != 2627)
					{
						throw;
					}
					return false;
				}
			}
		}

		public TranslationMemory GetTm(int id)
		{
			CheckVersion();
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tms", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
				return GetTm(sqlCommand);
			}
		}

		public TranslationMemory GetTm(string name)
		{
			CheckVersion();
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tms", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
				return GetTm(sqlCommand);
			}
		}

		public List<TranslationMemory> GetTms()
		{
			CheckVersion();
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tms", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				return GetTms(sqlCommand);
			}
		}

		public bool UpdateTm(TranslationMemory tm)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.update_tm", requiresTransaction: true))
			{
				BooleanSettingsWrapper booleanSettingsWrapper = new BooleanSettingsWrapper(tm.Recognizers, tm.TokenizerFlags, tm.WordCountFlags);
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = tm.Id;
				sqlCommand.Parameters.Add("@guid", SqlDbType.UniqueIdentifier).Value = tm.Guid;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = tm.Name;
				sqlCommand.Parameters.Add("@src_lang", SqlDbType.NVarChar).Value = tm.LanguageDirection.SourceCultureName;
				sqlCommand.Parameters.Add("@trg_lang", SqlDbType.NVarChar).Value = tm.LanguageDirection.TargetCultureName;
				sqlCommand.Parameters.Add("@copyright", SqlDbType.NVarChar).Value = tm.Copyright;
				sqlCommand.Parameters.Add("@description", SqlDbType.NVarChar).Value = tm.Description;
				sqlCommand.Parameters.Add("@settings", SqlDbType.Int).Value = booleanSettingsWrapper.DbSettingsValue;
				sqlCommand.Parameters.Add("@cru", SqlDbType.NVarChar).Value = tm.CreationUser;
				sqlCommand.Parameters.Add("@crd", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tm.CreationDate);
				if (tm.ExpirationDate.HasValue)
				{
					sqlCommand.Parameters.Add("@expd", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tm.ExpirationDate.Value);
				}
				sqlCommand.Parameters.Add("@fuzzy_indexes", SqlDbType.Int).Value = tm.FuzzyIndexes;
				sqlCommand.Parameters.Add("@fga_support", SqlDbType.Int).Value = tm.FGASupport;
				if (tm.DataVersion < 0)
				{
					tm.DataVersion = 1;
				}
				sqlCommand.Parameters.Add("@data_version", SqlDbType.Int).Value = tm.DataVersion;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public bool DeleteTm(int key)
		{
			return DeleteTm(key, ignoreErrors: true);
		}

		public bool DeleteTmSchema(int key)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.tm_dropschema", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@tmid", SqlDbType.Int).Value = key;
				sqlCommand.ExecuteNonQuery();
				return true;
			}
		}

		public bool AddAttribute(AttributeDeclaration attribute)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.add_attribute_%%", attribute.TMId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = attribute.Name;
				sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = (int)attribute.Type;
				sqlCommand.Parameters.Add("@guid", SqlDbType.UniqueIdentifier).Value = attribute.Guid;
				try
				{
					attribute.Id = (int)sqlCommand.ExecuteScalar();
				}
				catch (SqlException ex)
				{
					if (ex.Number != 2627)
					{
						throw;
					}
					return false;
				}
			}
			if (attribute.Type != FieldValueType.MultiplePicklist && attribute.Type != FieldValueType.SinglePicklist)
			{
				return true;
			}
			using (SqlCommand sqlCommand2 = CreateCommand("dbo.add_picklist_value_%%", attribute.TMId, requiresTransaction: true))
			{
				sqlCommand2.CommandType = CommandType.StoredProcedure;
				sqlCommand2.Parameters.Add("@attribute_id", SqlDbType.Int).Value = attribute.Id;
				SqlParameter sqlParameter = sqlCommand2.Parameters.Add("@value", SqlDbType.NVarChar);
				SqlParameter sqlParameter2 = sqlCommand2.Parameters.Add("@guid", SqlDbType.UniqueIdentifier);
				sqlCommand2.Prepare();
				foreach (PickValue item in attribute.Picklist)
				{
					sqlParameter.Value = item.Value;
					sqlParameter2.Value = item.Guid;
					try
					{
						item.Id = (int)sqlCommand2.ExecuteScalar();
					}
					catch (SqlException ex2)
					{
						if (ex2.Number != 2627)
						{
							throw;
						}
						return false;
					}
				}
			}
			return true;
		}

		public List<AttributeDeclaration> GetAttributes(int tmId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_attributes_%%", tmId, requiresTransaction: false))
			{
				using (SqlCommand sqlCommand2 = CreateCommand("dbo.get_attribute_picklist_%%", tmId, requiresTransaction: false))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand2.CommandType = CommandType.StoredProcedure;
					sqlCommand2.Parameters.Add("@attribute_id", SqlDbType.Int);
					return GetAttributes(sqlCommand, sqlCommand2);
				}
			}
		}

		public AttributeDeclaration GetAttribute(int tmId, int id)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_attributes_%%", tmId, requiresTransaction: false))
			{
				using (SqlCommand sqlCommand2 = CreateCommand("dbo.get_attribute_picklist_%%", tmId, requiresTransaction: false))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
					sqlCommand2.CommandType = CommandType.StoredProcedure;
					sqlCommand2.Parameters.Add("@attribute_id", SqlDbType.Int);
					return GetAttribute(sqlCommand, sqlCommand2);
				}
			}
		}

		public AttributeDeclaration GetAttribute(int tmId, string name)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_attributes_%%", tmId, requiresTransaction: false))
			{
				using (SqlCommand sqlCommand2 = CreateCommand("dbo.get_attribute_picklist_%%", tmId, requiresTransaction: false))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
					sqlCommand2.CommandType = CommandType.StoredProcedure;
					sqlCommand2.Parameters.Add("@attribute_id", SqlDbType.Int);
					return GetAttribute(sqlCommand, sqlCommand2);
				}
			}
		}

		public bool RenameAttribute(int tmId, int attributeKey, string newName)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.rename_attribute_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = attributeKey;
				sqlCommand.Parameters.Add("@newname", SqlDbType.NVarChar).Value = newName;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public bool DeleteAttribute(int tmId, int key)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_attribute_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = key;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public bool DeleteAttribute(int tmId, string name)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_attribute_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public int AddPicklistValue(int tmId, int attributeId, PickValue value)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.add_picklist_value_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@attribute_id", SqlDbType.Int).Value = attributeId;
				sqlCommand.Parameters.Add("@value", SqlDbType.NVarChar).Value = value.Value;
				sqlCommand.Parameters.Add("@guid", SqlDbType.UniqueIdentifier).Value = value.Guid;
				value.Id = (int)sqlCommand.ExecuteScalar();
				return value.Id;
			}
		}

		public PickValue GetPicklistValue(int tmId, int key)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_attribute_picklist_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = key;
				return GetPicklistValue(sqlCommand);
			}
		}

		public bool RenamePicklistValue(int tmId, int attributeId, string oldName, string newName)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.rename_picklist_value_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@attribute_id", SqlDbType.Int).Value = attributeId;
				sqlCommand.Parameters.Add("@oldname", SqlDbType.NVarChar).Value = oldName;
				sqlCommand.Parameters.Add("@newname", SqlDbType.NVarChar).Value = newName;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public bool DeletePicklistValue(int tmId, int attributeId, string value)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_picklist_value_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@attribute_id", SqlDbType.Int).Value = attributeId;
				sqlCommand.Parameters.Add("@value", SqlDbType.NVarChar).Value = value;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public int DeleteOrphanContexts(int tmId, TextContextMatchType textContextMatchType)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_orphan_contexts_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@cm_is_preceding_following", SqlDbType.Bit).Value = (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
				return sqlCommand.ExecuteNonQuery();
			}
		}

		public virtual string GetParameter(string name)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_parameter", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
				return (string)sqlCommand.ExecuteScalar();
			}
		}

		public string GetParameter(int tmId, string name)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_parameter", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
				sqlCommand.Parameters.Add("@tm_id", SqlDbType.Int).Value = tmId;
				return (string)sqlCommand.ExecuteScalar();
			}
		}

		public void SetParameter(string name, string value)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.set_parameter", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
				sqlCommand.Parameters.Add("@value", SqlDbType.NVarChar).Value = (value ?? string.Empty);
				sqlCommand.ExecuteNonQuery();
			}
		}

		public void SetParameter(int tmId, string name, string value)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.set_parameter", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
				sqlCommand.Parameters.Add("@value", SqlDbType.NVarChar).Value = (value ?? string.Empty);
				sqlCommand.Parameters.Add("@tm_id", SqlDbType.Int).Value = tmId;
				sqlCommand.ExecuteNonQuery();
			}
		}

		public override string GetVersion()
		{
			return GetParameter("VERSION");
		}

		public int ResolveResourceGuid(Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveTranslationMemoryGuid(Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveTranslationUnitGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveAttributeGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolvePicklistValueGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public bool AddResource(Resource resource)
		{
			CheckVersion();
			using (SqlCommand sqlCommand = CreateCommand("dbo.add_resource", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = (int)resource.Type;
				sqlCommand.Parameters.Add("@data", SqlDbType.VarBinary).Value = resource.Data;
				sqlCommand.Parameters.Add("@guid", SqlDbType.UniqueIdentifier).Value = resource.Guid;
				if (resource.Language != null)
				{
					sqlCommand.Parameters.Add("@language", SqlDbType.NVarChar).Value = resource.Language;
				}
				try
				{
					resource.Id = (int)sqlCommand.ExecuteScalar();
					return true;
				}
				catch (SqlException ex)
				{
					if (ex.Number != 2627)
					{
						throw;
					}
					return false;
				}
			}
		}

		public List<Resource> GetResources(bool includeData)
		{
			CheckVersion();
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_resources", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				return GetResources(sqlCommand, includeData);
			}
		}

		public List<Resource> GetResources(int tmId, bool includeData)
		{
			CheckVersion();
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_resources", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@tm_id", SqlDbType.Int).Value = tmId;
				return GetResources(sqlCommand, includeData);
			}
		}

		public Resource GetResource(int key, bool includeData)
		{
			CheckVersion();
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_resources", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = key;
				return GetResource(sqlCommand, includeData);
			}
		}

		public int GetResourcesWriteCount()
		{
			return -1;
		}

		public bool UpdateResource(Resource resource)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.update_resource", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = resource.Id;
				sqlCommand.Parameters.Add("@data", SqlDbType.VarBinary).Value = resource.Data;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public bool DeleteResource(int key)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_resource", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = key;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public List<TranslationMemory> GetTMsByResourceId(int resourceId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tms_by_resource", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@resource_id", SqlDbType.Int).Value = resourceId;
				return GetTms(sqlCommand);
			}
		}

		public bool AttachTmResource(int tmId, int resourceId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.add_tm_resource", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@tm_id", SqlDbType.Int).Value = tmId;
				sqlCommand.Parameters.Add("@resource_id", SqlDbType.Int).Value = resourceId;
				try
				{
					return sqlCommand.ExecuteNonQuery() > 0;
				}
				catch (SqlException ex)
				{
					if (ex.Number != 2627)
					{
						throw;
					}
					return false;
				}
			}
		}

		public bool DetachTmResource(int tmId, int resourceId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_tm_resource", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@tm_id", SqlDbType.Int).Value = tmId;
				sqlCommand.Parameters.Add("@resource_id", SqlDbType.Int).Value = resourceId;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public void AddTu(TranslationUnit tu, FuzzyIndexes indexes, bool keepId, long tokenizationSignatureHash)
		{
			DateTime value = DateTimeUtilities.Normalize(DateTime.Now);
			if (!tu.InsertDate.HasValue)
			{
				tu.InsertDate = value;
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.add_tu_%%", tu.TranslationMemoryId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@guid", SqlDbType.UniqueIdentifier).Value = tu.Guid;
				sqlCommand.Parameters.Add("@source_hash", SqlDbType.BigInt).Value = tu.Source.Hash;
				sqlCommand.Parameters.Add("@source_text", SqlDbType.NText).Value = tu.Source.Text;
				sqlCommand.Parameters.Add("@target_hash", SqlDbType.BigInt).Value = tu.Target.Hash;
				sqlCommand.Parameters.Add("@target_text", SqlDbType.NText).Value = tu.Target.Text;
				sqlCommand.Parameters.Add("@crd", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tu.CreationDate);
				sqlCommand.Parameters.Add("@cru", SqlDbType.NVarChar).Value = tu.CreationUser;
				sqlCommand.Parameters.Add("@chd", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tu.ChangeDate);
				sqlCommand.Parameters.Add("@chu", SqlDbType.NVarChar).Value = tu.ChangeUser;
				sqlCommand.Parameters.Add("@lud", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tu.LastUsedDate);
				sqlCommand.Parameters.Add("@luu", SqlDbType.NVarChar).Value = tu.LastUsedUser;
				sqlCommand.Parameters.Add("@usc", SqlDbType.Int).Value = tu.UsageCounter;
				sqlCommand.Parameters.Add("@format", SqlDbType.Int).Value = tu.Format;
				sqlCommand.Parameters.Add("@origin", SqlDbType.Int).Value = tu.Origin;
				sqlCommand.Parameters.Add("@confirmationLevel", SqlDbType.Int).Value = tu.ConfirmationLevel;
				sqlCommand.Parameters.Add("@sig_hash", SqlDbType.BigInt).Value = tokenizationSignatureHash;
				sqlCommand.Parameters.Add("@alignment_data", SqlDbType.VarBinary).Value = tu.AlignmentData;
				SqlParameter sqlParameter = sqlCommand.Parameters.Add("@align_model_date", SqlDbType.DateTime);
				sqlParameter.Value = DBNull.Value;
				if (tu.AlignModelDate.HasValue)
				{
					sqlParameter.Value = DbStorageBase.Normalize(tu.AlignModelDate.Value);
				}
				sqlCommand.Parameters.Add("@insert_date", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tu.InsertDate.GetValueOrDefault());
				if (keepId && tu.Id > 0)
				{
					sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = tu.Id;
				}
				if ((indexes & FuzzyIndexes.SourceWordBased) == FuzzyIndexes.SourceWordBased)
				{
					byte[] featuresData = GetFeaturesData(tu.Source.Features);
					if (featuresData != null)
					{
						sqlCommand.Parameters.Add("@data1", SqlDbType.VarBinary, featuresData.Length).Value = featuresData;
					}
				}
				if ((indexes & FuzzyIndexes.SourceCharacterBased) == FuzzyIndexes.SourceCharacterBased)
				{
					byte[] featuresData = GetFeaturesData(tu.Source.ConcordanceFeatures);
					if (featuresData != null)
					{
						sqlCommand.Parameters.Add("@data2", SqlDbType.VarBinary, featuresData.Length).Value = featuresData;
					}
				}
				if ((indexes & FuzzyIndexes.TargetCharacterBased) == FuzzyIndexes.TargetCharacterBased)
				{
					byte[] featuresData = GetFeaturesData(tu.Target.ConcordanceFeatures);
					if (featuresData != null)
					{
						sqlCommand.Parameters.Add("@data4", SqlDbType.VarBinary, featuresData.Length).Value = featuresData;
					}
				}
				if ((indexes & FuzzyIndexes.TargetWordBased) == FuzzyIndexes.TargetWordBased)
				{
					byte[] featuresData = GetFeaturesData(tu.Target.Features);
					if (featuresData != null)
					{
						sqlCommand.Parameters.Add("@data8", SqlDbType.VarBinary, featuresData.Length).Value = featuresData;
					}
				}
				if (tu.SourceTokenData != null)
				{
					sqlCommand.Parameters.Add("@source_token_data", SqlDbType.VarBinary, tu.SourceTokenData.Length).Value = tu.SourceTokenData;
				}
				if (tu.TargetTokenData != null)
				{
					sqlCommand.Parameters.Add("@target_token_data", SqlDbType.VarBinary, tu.TargetTokenData.Length).Value = tu.TargetTokenData;
				}
				sqlCommand.Parameters.Add("@relaxed_hash", SqlDbType.BigInt).Value = tu.Source.RelaxedHash;
				sqlCommand.Parameters.Add("@serialization_version", SqlDbType.Int).Value = tu.SerializationVersion;
				if (tu.Source.SerializedTags != null)
				{
					sqlCommand.Parameters.Add("@source_tags", SqlDbType.VarBinary, tu.Source.SerializedTags.Length).Value = tu.Source.SerializedTags;
				}
				if (tu.Target.SerializedTags != null)
				{
					sqlCommand.Parameters.Add("@target_tags", SqlDbType.VarBinary, tu.Target.SerializedTags.Length).Value = tu.Target.SerializedTags;
				}
				tu.Id = (int)sqlCommand.ExecuteScalar();
				AddAttributeValues(tu.TranslationMemoryId, tu);
			}
		}

		public List<Tuple<Guid, int>> AddTus(Tuple<TranslationUnit, ImportType>[] batchTUs, FuzzyIndexes indexes, long tokenizationSignatureHash, int tmid)
		{
			DataTable dataTable = new DataTable();
			DataTable dataTable2 = new DataTable();
			DataTable dataTable3 = new DataTable();
			DataTable dataTable4 = new DataTable();
			DataTable dataTable5 = new DataTable();
			DataTable dataTable6 = new DataTable();
			DataTable dataTable7 = new DataTable();
			DataTable dataTable8 = new DataTable();
			PrepareBatchTables(dataTable, dataTable3, dataTable4, dataTable5, dataTable6, dataTable2, dataTable7, dataTable8);
			for (int i = 0; i < batchTUs.Length; i++)
			{
				UpdateImportBatch(batchTUs[i], dataTable, dataTable3, dataTable4, dataTable6, dataTable5, dataTable2, dataTable7, dataTable8, indexes, tokenizationSignatureHash);
			}
			List<Tuple<Guid, int>> list = new List<Tuple<Guid, int>>();
			using (SqlCommand sqlCommand = CreateCommand("dbo.add_tu_batch_%%", tmid, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@tuBatch", dataTable);
				sqlCommand.Parameters.AddWithValue("@tuFeatures", dataTable2);
				sqlCommand.Parameters.AddWithValue("@tuStringAttributes", dataTable3);
				sqlCommand.Parameters.AddWithValue("@tuNumericAttributes", dataTable4);
				sqlCommand.Parameters.AddWithValue("@tuPickListAttributes", dataTable5);
				sqlCommand.Parameters.AddWithValue("@tuDateAttributes", dataTable6);
				sqlCommand.Parameters.AddWithValue("@tuContexts", dataTable7);
				sqlCommand.Parameters.AddWithValue("@tuIdContexts", dataTable8);
				using (DbDataReader dbDataReader = sqlCommand.ExecuteReader())
				{
					while (dbDataReader.Read())
					{
						Guid guid = dbDataReader.GetGuid(0);
						int @int = GetInt32(dbDataReader, 1);
						list.Add(new Tuple<Guid, int>(guid, @int));
					}
					return list;
				}
			}
		}

		private static void PrepareBatchTables(DataTable tusDataTable, DataTable stringAttributes, DataTable numericAttributes, DataTable picklistAttributes, DataTable dateAttributes, DataTable featuresDataTable, DataTable contextBatchTable, DataTable contextIdBatchTable)
		{
			tusDataTable.Columns.Add("guid", typeof(Guid));
			tusDataTable.Columns.Add("id", typeof(int));
			tusDataTable.Columns.Add("source_hash", typeof(long));
			tusDataTable.Columns.Add("target_hash", typeof(long));
			tusDataTable.Columns.Add("source_segment", typeof(string));
			tusDataTable.Columns.Add("target_segment", typeof(string));
			tusDataTable.Columns.Add("creation_date", typeof(DateTime));
			tusDataTable.Columns.Add("creation_user", typeof(string));
			tusDataTable.Columns.Add("change_date", typeof(DateTime));
			tusDataTable.Columns.Add("change_user", typeof(string));
			tusDataTable.Columns.Add("last_used_date", typeof(DateTime));
			tusDataTable.Columns.Add("last_used_user", typeof(string));
			tusDataTable.Columns.Add("usage_counter", typeof(int));
			tusDataTable.Columns.Add("source_token_data", typeof(byte[]));
			tusDataTable.Columns.Add("target_token_data", typeof(byte[]));
			tusDataTable.Columns.Add("alignment_data", typeof(byte[]));
			tusDataTable.Columns.Add("align_model_date", typeof(DateTime));
			tusDataTable.Columns.Add("insert_date", typeof(DateTime));
			tusDataTable.Columns.Add("tokenization_sig_hash", typeof(long));
			tusDataTable.Columns.Add("relaxed_hash", typeof(long));
			tusDataTable.Columns.Add("serialization_version", typeof(int));
			tusDataTable.Columns.Add("source_tags", typeof(byte[]));
			tusDataTable.Columns.Add("target_tags", typeof(byte[]));
			tusDataTable.Columns.Add("importType", typeof(int));
			tusDataTable.Columns.Add("format", typeof(int));
			tusDataTable.Columns.Add("origin", typeof(int));
			tusDataTable.Columns.Add("confirmationLevel", typeof(int));
			stringAttributes.Columns.Add("guid", typeof(Guid));
			stringAttributes.Columns.Add("attribute_id", typeof(int));
			stringAttributes.Columns.Add("value", typeof(string));
			numericAttributes.Columns.Add("guid", typeof(Guid));
			numericAttributes.Columns.Add("attribute_id", typeof(int));
			numericAttributes.Columns.Add("value", typeof(int));
			picklistAttributes.Columns.Add("guid", typeof(Guid));
			picklistAttributes.Columns.Add("attribute_id", typeof(int));
			picklistAttributes.Columns.Add("value", typeof(int));
			dateAttributes.Columns.Add("guid", typeof(Guid));
			dateAttributes.Columns.Add("attribute_id", typeof(int));
			dateAttributes.Columns.Add("value", typeof(DateTime));
			featuresDataTable.Columns.Add("guid", typeof(Guid));
			featuresDataTable.Columns.Add("feature", typeof(long));
			featuresDataTable.Columns.Add("length", typeof(int));
			featuresDataTable.Columns.Add("type", typeof(int));
			contextBatchTable.Columns.Add("guid", typeof(Guid));
			contextBatchTable.Columns.Add("context1", typeof(long));
			contextBatchTable.Columns.Add("context2", typeof(long));
			contextIdBatchTable.Columns.Add("guid", typeof(Guid));
			contextIdBatchTable.Columns.Add("idcontext", typeof(string));
		}

		private static void UpdateImportBatch(Tuple<TranslationUnit, ImportType> tuple, DataTable tus, DataTable stringAttributes, DataTable numericAttributes, DataTable dateAttributes, DataTable picklistAttributes, DataTable features, DataTable textContexts, DataTable idContexts, FuzzyIndexes indexes, long tokenizationSignatureHash)
		{
			if (((tuple != null) ? tuple.Item1 : null) != null)
			{
				TranslationUnit item = tuple.Item1;
				AddTuToBatch(tus, tuple, tokenizationSignatureHash);
				AddContextsToBatch(textContexts, item);
				AddDateTimeAttributesToBatch(dateAttributes, item);
				AddNumericAttributesToBatch(numericAttributes, item);
				AddPickListAttributesToBatch(picklistAttributes, item);
				AddStringAttributesToBatch(stringAttributes, item);
				AddIdContextsToBatch(idContexts, item);
				if (tuple.Item2 != ImportType.PartialUpdate)
				{
					AddFeaturesToBatch(features, item, indexes);
				}
			}
		}

		private static void AddIdContextsToBatch(DataTable contextIdBatch, TranslationUnit tu)
		{
			if (tu.IdContexts != null)
			{
				foreach (string value in tu.IdContexts.Values)
				{
					DataRow dataRow = contextIdBatch.NewRow();
					dataRow[0] = tu.Guid;
					dataRow[1] = value;
					contextIdBatch.Rows.Add(dataRow);
				}
			}
		}

		private static void AddContextsToBatch(DataTable contextBatch, TranslationUnit tu)
		{
			if (tu.Contexts != null)
			{
				foreach (TuContext value in tu.Contexts.Values)
				{
					DataRow dataRow = contextBatch.NewRow();
					dataRow[0] = tu.Guid;
					dataRow[1] = value.Context1;
					dataRow[2] = value.Context2;
					contextBatch.Rows.Add(dataRow);
				}
			}
		}

		private static void AddDateTimeAttributesToBatch(DataTable attributesDataTable, TranslationUnit tu)
		{
			foreach (AttributeValue attribute in tu.Attributes)
			{
				_ = attribute.Type;
				_ = 3;
				if (attribute.Type == FieldValueType.DateTime)
				{
					DataRow dataRow = attributesDataTable.NewRow();
					dataRow[0] = tu.Guid;
					dataRow[1] = attribute.DeclarationId;
					dataRow[2] = Convert.ToDateTime(attribute.Value);
					attributesDataTable.Rows.Add(dataRow);
				}
			}
		}

		private static void AddPickListAttributesToBatch(DataTable attributesDataTable, TranslationUnit tu)
		{
			foreach (AttributeValue attribute in tu.Attributes)
			{
				_ = attribute.Type;
				_ = 3;
				switch (attribute.Type)
				{
				case FieldValueType.MultiplePicklist:
				{
					int[] array = (int[])attribute.Value;
					foreach (int num in array)
					{
						DataRow dataRow = attributesDataTable.NewRow();
						dataRow[0] = tu.Guid;
						dataRow[1] = attribute.DeclarationId;
						dataRow[2] = num;
						attributesDataTable.Rows.Add(dataRow);
					}
					break;
				}
				case FieldValueType.SinglePicklist:
				{
					DataRow dataRow = attributesDataTable.NewRow();
					dataRow[0] = tu.Guid;
					dataRow[1] = attribute.DeclarationId;
					dataRow[2] = Convert.ToInt32(attribute.Value);
					attributesDataTable.Rows.Add(dataRow);
					break;
				}
				}
			}
		}

		private static void AddNumericAttributesToBatch(DataTable attributesDataTable, TranslationUnit tu)
		{
			foreach (AttributeValue attribute in tu.Attributes)
			{
				_ = attribute.Type;
				_ = 3;
				if (attribute.Type == FieldValueType.Integer)
				{
					DataRow dataRow = attributesDataTable.NewRow();
					dataRow[0] = tu.Guid;
					dataRow[1] = attribute.DeclarationId;
					dataRow[2] = Convert.ToInt32(attribute.Value);
					attributesDataTable.Rows.Add(dataRow);
				}
			}
		}

		private static void AddStringAttributesToBatch(DataTable attributesDataTable, TranslationUnit tu)
		{
			foreach (AttributeValue attribute in tu.Attributes)
			{
				_ = attribute.Type;
				_ = 3;
				switch (attribute.Type)
				{
				case FieldValueType.MultipleString:
				{
					string[] array = (string[])attribute.Value;
					foreach (string value in array)
					{
						DataRow dataRow = attributesDataTable.NewRow();
						dataRow[0] = tu.Guid;
						dataRow[1] = attribute.DeclarationId;
						dataRow[2] = value;
						attributesDataTable.Rows.Add(dataRow);
					}
					break;
				}
				case FieldValueType.SingleString:
				{
					DataRow dataRow = attributesDataTable.NewRow();
					dataRow[0] = tu.Guid;
					dataRow[1] = attribute.DeclarationId;
					dataRow[2] = ((attribute.Type == FieldValueType.DateTime) ? DbStorageBase.Normalize((DateTime)attribute.Value).ToString(CultureInfo.InvariantCulture) : Convert.ToString(attribute.Value));
					attributesDataTable.Rows.Add(dataRow);
					break;
				}
				}
			}
		}

		private static void AddTuToBatch(DataTable tusDataTable, Tuple<TranslationUnit, ImportType> tuple, long tokenizationSignatureHash)
		{
			TranslationUnit item = tuple.Item1;
			DataRow dataRow = tusDataTable.NewRow();
			dataRow["guid"] = item.Guid;
			dataRow["id"] = item.Id;
			if (tuple.Item2 != ImportType.PartialUpdate)
			{
				dataRow["source_hash"] = item.Source.Hash;
				dataRow["target_hash"] = item.Target.Hash;
				dataRow["source_segment"] = item.Source.Text;
				dataRow["target_segment"] = item.Target.Text;
				dataRow["relaxed_hash"] = item.Source.RelaxedHash;
				if (item.Source.SerializedTags != null)
				{
					dataRow["source_tags"] = item.Source.SerializedTags;
				}
				if (item.Target.SerializedTags != null)
				{
					dataRow["target_tags"] = item.Target.SerializedTags;
				}
			}
			dataRow["serialization_version"] = item.SerializationVersion;
			dataRow["creation_date"] = DbStorageBase.Normalize(item.CreationDate);
			dataRow["creation_user"] = item.CreationUser;
			dataRow["change_date"] = DbStorageBase.Normalize(item.ChangeDate);
			dataRow["change_user"] = item.ChangeUser;
			dataRow["last_used_date"] = DbStorageBase.Normalize(item.LastUsedDate);
			dataRow["last_used_user"] = item.LastUsedUser;
			dataRow["usage_counter"] = item.UsageCounter;
			if (item.SourceTokenData != null)
			{
				dataRow["source_token_data"] = item.SourceTokenData;
			}
			if (item.TargetTokenData != null)
			{
				dataRow["target_token_data"] = item.TargetTokenData;
			}
			dataRow["alignment_data"] = item.AlignmentData;
			if (item.AlignModelDate.HasValue)
			{
				dataRow["align_model_date"] = DbStorageBase.Normalize(item.AlignModelDate.Value);
			}
			DateTime value = DateTimeUtilities.Normalize(DateTime.Now);
			if (!item.InsertDate.HasValue)
			{
				item.InsertDate = value;
			}
			dataRow["insert_date"] = DbStorageBase.Normalize(item.InsertDate.GetValueOrDefault());
			dataRow["tokenization_sig_hash"] = tokenizationSignatureHash;
			dataRow["importType"] = tuple.Item2;
			dataRow["format"] = item.Format;
			dataRow["origin"] = item.Origin;
			dataRow["confirmationLevel"] = item.ConfirmationLevel;
			tusDataTable.Rows.Add(dataRow);
		}

		private static void AddFeaturesToBatch(DataTable featuresDataTable, TranslationUnit tu, FuzzyIndexes indexes)
		{
			if ((indexes & FuzzyIndexes.SourceWordBased) == FuzzyIndexes.SourceWordBased)
			{
				UpdateFeaturesTables(featuresDataTable, tu.Guid, tu.Source.Features, FuzzyIndexes.SourceWordBased);
			}
			if ((indexes & FuzzyIndexes.SourceCharacterBased) == FuzzyIndexes.SourceCharacterBased)
			{
				UpdateFeaturesTables(featuresDataTable, tu.Guid, tu.Source.ConcordanceFeatures, FuzzyIndexes.SourceCharacterBased);
			}
			if ((indexes & FuzzyIndexes.TargetCharacterBased) == FuzzyIndexes.TargetCharacterBased)
			{
				UpdateFeaturesTables(featuresDataTable, tu.Guid, tu.Target.ConcordanceFeatures, FuzzyIndexes.TargetCharacterBased);
			}
			if ((indexes & FuzzyIndexes.TargetWordBased) == FuzzyIndexes.TargetWordBased)
			{
				UpdateFeaturesTables(featuresDataTable, tu.Guid, tu.Target.Features, FuzzyIndexes.TargetWordBased);
			}
		}

		private static void UpdateFeaturesTables(DataTable featuresDataTable, Guid guid, List<int> features, FuzzyIndexes indexes)
		{
			foreach (int feature in features)
			{
				DataRow dataRow = featuresDataTable.NewRow();
				dataRow[0] = guid;
				dataRow[1] = feature;
				dataRow[2] = features.Count;
				dataRow[3] = indexes;
				featuresDataTable.Rows.Add(dataRow);
			}
		}

		public TranslationUnit GetTu(int tmId, int key, bool idContextMatch)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tu_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = key;
				sqlCommand.Parameters.Add("@returnIdContext", SqlDbType.Int).Value = (idContextMatch ? 1 : 0);
				List<TranslationUnit> tUsWithAttributesAndContexts = GetTUsWithAttributesAndContexts(sqlCommand, idContextMatch);
				if (tUsWithAttributesAndContexts == null || tUsWithAttributesAndContexts.Count == 0)
				{
					return null;
				}
				return tUsWithAttributesAndContexts[0];
			}
		}

		public int GetTuCount(int tmId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tu_count_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				return (int)sqlCommand.ExecuteScalar();
			}
		}

		public List<TranslationUnit> GetTus(int tmId, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			return GetTus_SingleRoundtrip(tmId, startAfter, count, forward, idContextMatch, includeContextContent, textContextMatchType, sourceCulture, targetCulture);
		}

		public List<TranslationUnit> GetTusForReindex(int tmId, int startAfter, int count, long currentSigHash)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tus_for_reindex_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@sig_hash", SqlDbType.BigInt).Value = currentSigHash;
				sqlCommand.Parameters.Add("@start_after", SqlDbType.Int).Value = startAfter;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				return GetTuRange(sqlCommand);
			}
		}

		public List<TranslationUnit> GetTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tus_ex_f_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@start_after", SqlDbType.Int).Value = startAfter;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@forward", SqlDbType.Int).Value = (forward ? 1 : 0);
				UpdateCommandWithFilterParameters(filter, sqlCommand);
				sqlCommand.Parameters.Add("@returnIdContext", SqlDbType.Int).Value = (idContextMatch ? 1 : 0);
				sqlCommand.Parameters.Add("@includeContextContent", SqlDbType.Int).Value = (includeContextContent ? 1 : 0);
				sqlCommand.Parameters.Add("@cm_is_preceding_following", SqlDbType.Bit).Value = (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
				List<TranslationUnit> tUsWithAttributesAndContexts = GetTUsWithAttributesAndContexts(sqlCommand, idContextMatch, returnContext: true, null, sourceCulture, targetCulture, textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
				tUsWithAttributesAndContexts.Sort((TranslationUnit x, TranslationUnit y) => x.Id - y.Id);
				return tUsWithAttributesAndContexts;
			}
		}

		private void UpdateCommandWithFilterParameters(FilterExpression filter, SqlCommand cmd)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("value", typeof(string));
			dataTable.Columns.Add("type", typeof(string));
			cmd.Parameters.AddWithValue("@filterParam", dataTable);
			if (filter != null)
			{
				SqlStorageFilterParameterBuilder sqlStorageFilterParameterBuilder = new SqlStorageFilterParameterBuilder();
				sqlStorageFilterParameterBuilder.ProcessFilterExpression(filter);
				IEnumerable<string> systemFields = sqlStorageFilterParameterBuilder.GetSystemFields();
				IEnumerable<string> fieldValueTypeAttributes = sqlStorageFilterParameterBuilder.GetFieldValueTypeAttributes(FieldValueType.Integer);
				IEnumerable<string> fieldValueTypeAttributes2 = sqlStorageFilterParameterBuilder.GetFieldValueTypeAttributes(FieldValueType.DateTime);
				IEnumerable<string> fieldValueTypeAttributes3 = sqlStorageFilterParameterBuilder.GetFieldValueTypeAttributes(FieldValueType.SinglePicklist);
				IEnumerable<string> fieldValueTypeAttributes4 = sqlStorageFilterParameterBuilder.GetFieldValueTypeAttributes(FieldValueType.MultiplePicklist);
				IEnumerable<string> fieldValueTypeAttributes5 = sqlStorageFilterParameterBuilder.GetFieldValueTypeAttributes(FieldValueType.SingleString);
				IEnumerable<string> fieldValueTypeAttributes6 = sqlStorageFilterParameterBuilder.GetFieldValueTypeAttributes(FieldValueType.MultipleString);
				string filterExpression = sqlStorageFilterParameterBuilder.FilterExpression;
				string havingExpression = sqlStorageFilterParameterBuilder.HavingExpression;
				AddSearchFilter(dataTable, systemFields, "systemFields");
				AddSearchFilter(dataTable, fieldValueTypeAttributes2, "dateAttributes");
				AddSearchFilter(dataTable, fieldValueTypeAttributes, "numericAttributes");
				AddSearchFilter(dataTable, fieldValueTypeAttributes3, "singlePickListAttributes");
				AddSearchFilter(dataTable, fieldValueTypeAttributes4, "multiPickListAttributes");
				AddSearchFilter(dataTable, fieldValueTypeAttributes5, "stringAttributes");
				AddSearchFilter(dataTable, fieldValueTypeAttributes6, "multiStringAttributes");
				AddSearchFilter(dataTable, filterExpression, "filterExpression");
				AddSearchFilter(dataTable, havingExpression, "havings");
			}
		}

		private static void AddSearchFilter(DataTable filterParams, string filterParam, string filterName)
		{
			DataRow dataRow = filterParams.NewRow();
			dataRow[0] = filterParam;
			dataRow[1] = filterName;
			filterParams.Rows.Add(dataRow);
		}

		private void AddSearchFilter(DataTable filterParams, IEnumerable<string> filterCollection, string collectionName)
		{
			foreach (string item in filterCollection)
			{
				AddSearchFilter(filterParams, item, collectionName);
			}
		}

		public List<TranslationUnit> GetTus(int tmId, List<Tuple<int, long, long>> tuAndHash, bool returnIdContext)
		{
			using (DataTable dataTable = new DataTable())
			{
				dataTable.Columns.Add("id", typeof(int));
				dataTable.Columns.Add("sourceHash", typeof(long));
				dataTable.Columns.Add("targetHash", typeof(long));
				foreach (Tuple<int, long, long> item in tuAndHash)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = item.Item1;
					dataRow[1] = item.Item2;
					dataRow[2] = item.Item3;
					dataTable.Rows.Add(dataRow);
				}
				List<TranslationUnit> tusWithAttributes;
				using (SqlCommand sqlCommand = CreateCommand("dbo.get_tus_WithHashes_%%", tmId, requiresTransaction: false))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.AddWithValue("@TUID_hashes", dataTable);
					sqlCommand.Parameters.AddWithValue("@returnIdContext", returnIdContext ? 1 : 0);
					tusWithAttributes = GetTusWithAttributes(sqlCommand, returnIdContext);
				}
				dataTable.Rows.Clear();
				return tusWithAttributes;
			}
		}

		public List<PersistentObjectToken> GetTuIds(int tmId, int startAfter, int count, bool forward)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tu_ids_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@start_after", SqlDbType.Int).Value = startAfter;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@forward", SqlDbType.Int).Value = (forward ? 1 : 0);
				return GetTuIds(sqlCommand);
			}
		}

		public bool UpdateTuHeader(TranslationUnit tu, bool rewriteAttributes)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.update_tu_%%", tu.TranslationMemoryId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = tu.Id;
				sqlCommand.Parameters.Add("@chd", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tu.ChangeDate);
				sqlCommand.Parameters.Add("@chu", SqlDbType.NVarChar).Value = tu.ChangeUser;
				sqlCommand.Parameters.Add("@lud", SqlDbType.DateTime).Value = DbStorageBase.Normalize(tu.LastUsedDate);
				sqlCommand.Parameters.Add("@luu", SqlDbType.NVarChar).Value = tu.LastUsedUser;
				sqlCommand.Parameters.Add("@usc", SqlDbType.Int).Value = tu.UsageCounter;
				sqlCommand.Parameters.Add("@format", SqlDbType.Int).Value = tu.Format;
				sqlCommand.Parameters.Add("@origin", SqlDbType.Int).Value = tu.Origin;
				sqlCommand.Parameters.Add("@confirmationLevel", SqlDbType.Int).Value = tu.ConfirmationLevel;
				if (sqlCommand.ExecuteNonQuery() <= 0)
				{
					return false;
				}
			}
			if (!rewriteAttributes)
			{
				return true;
			}
			DeleteAttributeValues(tu.TranslationMemoryId, tu);
			AddAttributeValues(tu.TranslationMemoryId, tu);
			return true;
		}

		private static DataTable CreateFuzzyIndexTable()
		{
			return new DataTable
			{
				Columns = 
				{
					new DataColumn("feature", typeof(int)),
					new DataColumn("translation_unit_id", typeof(int))
				}
			};
		}

		private static void AddFuzzyIndexData(DataTable fuzzyIndexTable, List<int> features, int tuId)
		{
			foreach (int feature in features)
			{
				DataRow dataRow = fuzzyIndexTable.NewRow();
				dataRow["translation_unit_id"] = tuId;
				dataRow["feature"] = feature;
				fuzzyIndexTable.Rows.Add(dataRow);
			}
		}

		public void UpdateTuIndices(List<TranslationUnit> tus, FuzzyIndexes indexes, long tokenizationSignatureHash, TextContextMatchType textContextMatchType)
		{
			if (tus != null && tus.Count != 0)
			{
				if (tus.Any((TranslationUnit x) => x.SerializationVersion != tus[0].SerializationVersion))
				{
					throw new Exception("All TUs must have the same serialization version!");
				}
				if (tus.Any((TranslationUnit x) => x.TranslationMemoryId != tus[0].TranslationMemoryId))
				{
					throw new Exception("All TUs must have the same TM id!");
				}
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("id", typeof(int)));
				dataTable.Columns.Add(new DataColumn("source_hash", typeof(long)));
				dataTable.Columns.Add(new DataColumn("source_text", typeof(string)));
				dataTable.Columns.Add(new DataColumn("target_hash", typeof(long)));
				dataTable.Columns.Add(new DataColumn("target_text", typeof(string)));
				dataTable.Columns.Add(new DataColumn("source_token_data", typeof(byte[])));
				dataTable.Columns.Add(new DataColumn("target_token_data", typeof(byte[])));
				dataTable.Columns.Add(new DataColumn("relaxed_hash", typeof(long)));
				dataTable.Columns.Add(new DataColumn("source_tags", typeof(byte[])));
				dataTable.Columns.Add(new DataColumn("target_tags", typeof(byte[])));
				DataTable dataTable2 = CreateFuzzyIndexTable();
				DataTable dataTable3 = CreateFuzzyIndexTable();
				DataTable dataTable4 = CreateFuzzyIndexTable();
				DataTable dataTable5 = CreateFuzzyIndexTable();
				foreach (TranslationUnit tu in tus)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["id"] = tu.Id;
					dataRow["source_hash"] = tu.Source.Hash;
					dataRow["source_text"] = tu.Source.Text;
					dataRow["target_hash"] = tu.Target.Hash;
					dataRow["target_text"] = tu.Target.Text;
					dataRow["relaxed_hash"] = tu.Source.RelaxedHash;
					if (tu.Source.Features != null && tu.Source.Features.Count > 0)
					{
						AddFuzzyIndexData(dataTable2, tu.Source.Features, tu.Id);
					}
					if ((indexes & FuzzyIndexes.SourceCharacterBased) == FuzzyIndexes.SourceCharacterBased)
					{
						AddFuzzyIndexData(dataTable3, tu.Source.ConcordanceFeatures, tu.Id);
					}
					if ((indexes & FuzzyIndexes.TargetCharacterBased) == FuzzyIndexes.TargetCharacterBased)
					{
						AddFuzzyIndexData(dataTable4, tu.Target.ConcordanceFeatures, tu.Id);
					}
					if ((indexes & FuzzyIndexes.TargetWordBased) == FuzzyIndexes.TargetWordBased)
					{
						AddFuzzyIndexData(dataTable5, tu.Target.Features, tu.Id);
					}
					if (tu.SourceTokenData != null)
					{
						dataRow["source_token_data"] = tu.SourceTokenData;
					}
					if (tu.TargetTokenData != null)
					{
						dataRow["target_token_data"] = tu.TargetTokenData;
					}
					if (tu.Source.SerializedTags != null)
					{
						dataRow["source_tags"] = tu.Source.SerializedTags;
					}
					if (tu.Target.SerializedTags != null)
					{
						dataRow["target_tags"] = tu.Target.SerializedTags;
					}
					dataTable.Rows.Add(dataRow);
				}
				using (SqlCommand sqlCommand = CreateCommand("dbo.update_tu_data_batch_%%", tus[0].TranslationMemoryId, requiresTransaction: true))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@sig_hash", SqlDbType.BigInt).Value = tokenizationSignatureHash;
					sqlCommand.Parameters.Add("@cm_is_preceding_following", SqlDbType.Bit).Value = (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
					DateTime val = DateTimeUtilities.Normalize(DateTime.Now);
					sqlCommand.Parameters.Add("@insert_date", SqlDbType.DateTime).Value = DbStorageBase.Normalize(val);
					sqlCommand.Parameters.Add("@serialization_version", SqlDbType.Int).Value = tus[0].SerializationVersion;
					sqlCommand.Parameters.AddWithValue("@updateTuData", dataTable);
					sqlCommand.Parameters.AddWithValue("@fuzzyData1", dataTable2);
					sqlCommand.Parameters.AddWithValue("@fuzzyData2", dataTable3);
					sqlCommand.Parameters.AddWithValue("@fuzzyData4", dataTable4);
					sqlCommand.Parameters.AddWithValue("@fuzzyData8", dataTable5);
					sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public bool DeleteTu(int tmId, PersistentObjectToken key, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_tu_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = key.Id;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		public List<PersistentObjectToken> DeleteTus(int tmId, List<PersistentObjectToken> keys, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			List<int> list = new List<int>();
			foreach (PersistentObjectToken key in keys)
			{
				list.Add(key.Id);
			}
			DataTable dataTable = ConvertListToSqlDataTable(list);
			if (dataTable.Rows.Count == 0)
			{
				return new List<PersistentObjectToken>();
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_tus_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@tu_ids", dataTable);
				sqlCommand.Parameters.Add("@cm_is_preceding_following", SqlDbType.Bit).Value = (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
				sqlCommand.Parameters.Add("@delete_orphan_contexts", SqlDbType.Bit).Value = deleteOrphanContexts;
				return GetTuIds(sqlCommand);
			}
		}

		public List<PersistentObjectToken> DeleteTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_tus_filtered_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@start_after", SqlDbType.Int).Value = startAfter;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@forward", SqlDbType.Int).Value = (forward ? 1 : 0);
				UpdateCommandWithFilterParameters(filter, sqlCommand);
				sqlCommand.Parameters.Add("@cm_is_preceding_following", SqlDbType.Bit).Value = (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
				sqlCommand.Parameters.Add("@delete_orphan_contexts", SqlDbType.Bit).Value = deleteOrphanContexts;
				return GetTuIds(sqlCommand);
			}
		}

		public int DeleteAllTus(int tmId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_tu_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				return sqlCommand.ExecuteNonQuery();
			}
		}

		public TuContexts GetTextContexts(int tmId, int tuId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tu_contexts_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@tu_id", SqlDbType.Int).Value = tuId;
				return GetTextContexts(sqlCommand);
			}
		}

		public TuIdContexts GetTuIdContexts(int tmId, int tuId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tu_idcontexts_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@tu_id", SqlDbType.Int).Value = tuId;
				return GetIdContexts(sqlCommand);
			}
		}

		public void AddContexts(int tmId, int tuId, TuContexts contexts)
		{
			if (contexts == null || contexts.Length == 0)
			{
				return;
			}
			if (contexts.Length <= 1)
			{
				using (SqlCommand sqlCommand = CreateCommand("dbo.add_tu_context_%%", tmId, requiresTransaction: true))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@tu_id", SqlDbType.Int).Value = tuId;
					sqlCommand.Parameters.Add("@context1", SqlDbType.BigInt);
					sqlCommand.Parameters.Add("@context2", SqlDbType.BigInt);
					AddContexts(contexts, sqlCommand);
				}
				return;
			}
			byte[] contextsData = GetContextsData(contexts);
			if (contextsData != null)
			{
				using (SqlCommand sqlCommand2 = CreateCommand("dbo.add_tu_contexts_%%", tmId, requiresTransaction: true))
				{
					sqlCommand2.CommandType = CommandType.StoredProcedure;
					sqlCommand2.Parameters.Add("@tu_id", SqlDbType.Int).Value = tuId;
					sqlCommand2.Parameters.Add("@data", SqlDbType.VarBinary, contextsData.Length).Value = contextsData;
					sqlCommand2.ExecuteNonQuery();
				}
			}
		}

		public void AddIdContexts(int tmId, int tuId, TuIdContexts contexts)
		{
			if (contexts != null && contexts.Length != 0)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("idcontext", typeof(string));
				foreach (string value in contexts.Values)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = value;
					dataTable.Rows.Add(dataRow);
				}
				using (SqlCommand sqlCommand = CreateCommand("dbo.add_tu_idcontexts_%%", tmId, requiresTransaction: true))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@tu_id", SqlDbType.Int).Value = tuId;
					sqlCommand.Parameters.AddWithValue("@idcontexts", dataTable);
					sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public bool[] UpdateTuAlignmentData(IEnumerable<TuAlignmentDataInternal> tuAlignmentDatas, int tmId)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("translation_unit_id", typeof(int));
			dataTable.Columns.Add("FragmentHash", typeof(long));
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("TuId", typeof(int));
			List<bool> list = new List<bool>();
			using (SqlCommand sqlCommand = CreateCommand("dbo.update_tu_alignmentdata_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				SqlParameter sqlParameter = sqlCommand.Parameters.Add("@tu_id", SqlDbType.Int);
				SqlParameter sqlParameter2 = sqlCommand.Parameters.Add("@data", SqlDbType.VarBinary);
				SqlParameter sqlParameter3 = sqlCommand.Parameters.Add("@align_model_date", SqlDbType.DateTime);
				SqlParameter sqlParameter4 = sqlCommand.Parameters.Add("@insert_date", SqlDbType.DateTime);
				foreach (TuAlignmentDataInternal tuAlignmentData in tuAlignmentDatas)
				{
					sqlParameter.Value = tuAlignmentData.tuId.Id;
					sqlParameter2.Value = DBNull.Value;
					sqlParameter3.Value = DBNull.Value;
					if (tuAlignmentData.alignModelDate.HasValue)
					{
						sqlParameter3.Value = DbStorageBase.Normalize(tuAlignmentData.alignModelDate.Value);
					}
					if (tuAlignmentData.alignmentData != null)
					{
						sqlParameter2.Value = tuAlignmentData.alignmentData;
					}
					sqlParameter4.Value = DbStorageBase.Normalize(tuAlignmentData.insertDate);
					int num = 0;
					try
					{
						num = sqlCommand.ExecuteNonQuery();
					}
					catch (Exception)
					{
					}
					if (num == 0)
					{
						list.Add(item: false);
					}
					else
					{
						DataRow dataRow = dataTable2.NewRow();
						dataRow["TuId"] = tuAlignmentData.tuId.Id;
						dataTable2.Rows.Add(dataRow);
						foreach (long hash in tuAlignmentData.hashes)
						{
							DataRow dataRow2 = dataTable.NewRow();
							dataRow2["translation_unit_id"] = tuAlignmentData.tuId.Id;
							dataRow2["FragmentHash"] = hash;
							dataTable.Rows.Add(dataRow2);
						}
						list.Add(item: true);
					}
				}
			}
			using (SqlCommand sqlCommand2 = CreateCommand("dbo.delete_tu_fragments_%%", tmId, requiresTransaction: true))
			{
				sqlCommand2.CommandType = CommandType.StoredProcedure;
				sqlCommand2.Parameters.AddWithValue("@tu_ids", dataTable2);
				sqlCommand2.ExecuteNonQuery();
			}
			using (SqlCommand sqlCommand3 = CreateCommand("dbo.insert_tu_fragments_%%", tmId, requiresTransaction: true))
			{
				sqlCommand3.CommandType = CommandType.StoredProcedure;
				sqlCommand3.Parameters.AddWithValue("@index_entries", dataTable);
				sqlCommand3.ExecuteNonQuery();
			}
			return list.ToArray();
		}

		public List<TranslationUnit> SubsegmentSearch(int tmId, List<long> fragmentHashes, byte minFragmentLength, byte minSigWords, int maxHits, Dictionary<int, HashSet<long>> hashesPerTu)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Hash", typeof(long));
			dataTable.PrimaryKey = new DataColumn[1]
			{
				dataTable.Columns["Hash"]
			};
			foreach (long fragmentHash in fragmentHashes)
			{
				if (!dataTable.Rows.Contains(fragmentHash))
				{
					dataTable.Rows.Add(fragmentHash);
				}
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.dta_search_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@source_hashes", dataTable);
				sqlCommand.Parameters.Add("@min_length", SqlDbType.TinyInt).Value = minFragmentLength;
				sqlCommand.Parameters.Add("@min_sigwords", SqlDbType.TinyInt).Value = minSigWords;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = maxHits;
				hashesPerTu.Clear();
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("TuId", typeof(int));
				using (DbDataReader dbDataReader = sqlCommand.ExecuteReader())
				{
					while (dbDataReader.Read())
					{
						int @int = dbDataReader.GetInt32(0);
						long int2 = dbDataReader.GetInt64(1);
						if (hashesPerTu.TryGetValue(@int, out HashSet<long> value))
						{
							value.Add(int2);
						}
						else
						{
							hashesPerTu.Add(@int, new HashSet<long>
							{
								int2
							});
							DataRow dataRow = dataTable2.NewRow();
							dataRow[0] = @int;
							dataTable2.Rows.Add(dataRow);
						}
					}
					if (!dbDataReader.NextResult())
					{
						throw new Exception("Next result set expected");
					}
					return GetTUsWithAttributesAndContexts(dbDataReader, returnIdContext: false, returnContext: false);
				}
			}
		}

		public List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, int maxHits)
		{
			return null;
		}

		public List<TranslationUnit> DuplicateSearch(int tmId, List<long> sourceHashes, List<long> targetHashes)
		{
			using (DataTable dataTable = new DataTable())
			{
				dataTable.Columns.Add("id", typeof(int));
				dataTable.Columns.Add("sourceHash", typeof(long));
				dataTable.Columns.Add("targetHash", typeof(long));
				for (int i = 0; i < sourceHashes.Count; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["id"] = i;
					dataRow["sourceHash"] = sourceHashes[i];
					dataRow["targetHash"] = targetHashes[i];
					dataTable.Rows.Add(dataRow);
				}
				using (SqlCommand sqlCommand = CreateCommand("dbo.batch_duplicate_search_%%", tmId, requiresTransaction: false))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.AddWithValue("@tuid_hashes", dataTable);
					return GetDuplicateBatchMatches(sqlCommand);
				}
			}
		}

		public List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, FilterExpression hardFilter)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Hashes", typeof(long));
			foreach (long sourceHash in sourceHashes)
			{
				dataTable.Rows.Add(sourceHash);
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.batch_exact_search_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@source_hashes", dataTable);
				UpdateCommandWithFilterParameters(hardFilter, sqlCommand);
				return GetExactBatchMatches(sqlCommand);
			}
		}

		public List<TranslationUnit> ExactSearch(int tmId, long sourceHash, long targetHash, int maxHits, DateTime lastChangeDate, int skipRows, TuContextData tuContextData, bool descendingOrder, List<int> tuIdsToSkip)
		{
			DataTable value = ConvertListToSqlDataTable(tuIdsToSkip);
			TuContext textContext = tuContextData.TextContext;
			if (!string.IsNullOrEmpty(tuContextData.IdContext) && tuIdsToSkip != null)
			{
				throw new Exception("tuContextData and tuIdsToSkip should not both be set");
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.exact_search_ex_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@source_hash", SqlDbType.BigInt).Value = sourceHash;
				if (targetHash != 0L)
				{
					sqlCommand.Parameters.Add("@target_hash", SqlDbType.BigInt).Value = targetHash;
				}
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = maxHits;
				sqlCommand.Parameters.Add("@skiprows", SqlDbType.Int).Value = skipRows;
				sqlCommand.Parameters.Add("@last_change_date", SqlDbType.DateTime).Value = lastChangeDate;
				sqlCommand.Parameters.Add("@LeftTuContextSource", SqlDbType.BigInt).Value = (textContext?.Context1 ?? (-1));
				sqlCommand.Parameters.Add("@LeftTuContextTarget", SqlDbType.BigInt).Value = (textContext?.Context2 ?? (-1));
				sqlCommand.Parameters.Add("@idcontext", SqlDbType.NVarChar).Value = tuContextData.IdContext;
				sqlCommand.Parameters.Add("@DescendingOrder", SqlDbType.Bit).Value = descendingOrder;
				sqlCommand.Parameters.AddWithValue("@tuIdsToSkip", value);
				bool returnContext = textContext != null && textContext.Context1 != -1;
				bool returnIdContext = !string.IsNullOrEmpty(tuContextData.IdContext);
				return GetTUsWithAttributesAndContexts(sqlCommand, returnIdContext, returnContext);
			}
		}

		public Dictionary<int, List<TranslationUnit>> FuzzySearch(int tmId, Dictionary<int, List<int>> features, int minScore, int maxHits, FilterExpression hardFilter)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("TuId", typeof(int));
			dataTable.Columns.Add("Feature", typeof(int));
			foreach (KeyValuePair<int, List<int>> feature in features)
			{
				foreach (int item in feature.Value)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = feature.Key;
					dataRow[1] = item;
					dataTable.Rows.Add(dataRow);
				}
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.batch_fuzzy_search_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@BatchFeatures", dataTable);
				sqlCommand.Parameters.Add("@minScore", SqlDbType.Int).Value = minScore;
				sqlCommand.Parameters.Add("@pageSize", SqlDbType.Int).Value = maxHits;
				UpdateCommandWithFilterParameters(hardFilter, sqlCommand);
				return GetFuzzyBatchMatches(sqlCommand);
			}
		}

		public List<TranslationUnit> FuzzySearch(int tmId, List<int> features, FuzzyIndexes index, int minScore, int maxHits, bool concordance, int lastTuId, TuContextData tuContextData, bool descendingOrder)
		{
			TuContext tuContext = tuContextData.TextContext;
			if (concordance)
			{
				tuContext = null;
			}
			string cmdText = "dbo.fuzzy_search_%%";
			int count = features.Count;
			string value = ComputeFeatureString(features, withParens: true, 0);
			if (concordance)
			{
				cmdText = "fuzzy_search_concordance_%%";
			}
			using (SqlCommand sqlCommand = CreateCommand(cmdText, tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@features", SqlDbType.NVarChar).Value = value;
				sqlCommand.Parameters.Add("@length", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@minScore", SqlDbType.Int).Value = minScore;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = maxHits;
				sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = (int)index;
				sqlCommand.Parameters.Add("@last_id", SqlDbType.Int).Value = lastTuId;
				if (!concordance)
				{
					sqlCommand.Parameters.Add("@context1", SqlDbType.BigInt).Value = (tuContext?.Context1 ?? (-1));
					sqlCommand.Parameters.Add("@context2", SqlDbType.BigInt).Value = (tuContext?.Context2 ?? (-1));
				}
				sqlCommand.Parameters.Add("@DescendingOrder", SqlDbType.Bit).Value = descendingOrder;
				bool returnContext = tuContext != null;
				return GetTUsWithAttributesAndContexts(sqlCommand, returnIdContext: false, returnContext);
			}
		}

		public List<TranslationUnit> DuplicateSearch(int tmId, long lastHash, int lastTuId, int count, bool forward, bool targetSegments)
		{
			string cmdText = "dbo.duplicate_search_%%";
			string parameterName = "@last_source_hash";
			if (targetSegments)
			{
				cmdText = "dbo.duplicate_search_target_%%";
				parameterName = "@last_target_hash";
			}
			using (SqlCommand sqlCommand = CreateCommand(cmdText, tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add(parameterName, SqlDbType.BigInt).Value = lastHash;
				sqlCommand.Parameters.Add("@last_tu_id", SqlDbType.Int).Value = lastTuId;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@forward", SqlDbType.Int).Value = (forward ? 1 : 0);
				return GetTUsWithAttributesAndContexts(sqlCommand, returnIdContext: false, returnContext: false);
			}
		}

		public void RecomputeFrequencies(int tmId)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.recompute_frequencies_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.ExecuteNonQuery();
			}
		}

		public void ClearFuzzyIndex(FuzzyIndexes index)
		{
			throw new NotImplementedException();
		}

		public bool? GetReindexRequired(int tmId, long currentSignatureHash)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_reindex_required_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@sig_hash", SqlDbType.BigInt).Value = currentSignatureHash;
				return GetReindexRequired(sqlCommand);
			}
		}

		public int GetTuCountForReindex(int tmId, long currentSignatureHash)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tu_count_for_reindex_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@sig_hash", SqlDbType.BigInt).Value = currentSignatureHash;
				return GetTuCountForReindex(sqlCommand);
			}
		}

		public List<int[]> GetDuplicateSegmentHashes(int tmId, bool target, long? currentSigHash)
		{
			CheckVersion();
			List<int[]> list = new List<int[]>();
			Dictionary<int, long?> dictionary = null;
			long num = -1L;
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_dup_seg_hashes_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@sig_hash", SqlDbType.BigInt).Value = currentSigHash;
				sqlCommand.Parameters.Add("@target", SqlDbType.Bit).Value = target;
				using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
				{
					while (sqlDataReader.Read())
					{
						int @int = sqlDataReader.GetInt32(0);
						long int2 = sqlDataReader.GetInt64(1);
						long? value = null;
						if (!sqlDataReader.IsDBNull(2))
						{
							value = sqlDataReader.GetInt64(2);
						}
						if (dictionary == null || int2 != num)
						{
							if (dictionary != null && dictionary.Values.Any((long? x) => !x.HasValue || x.Value != currentSigHash))
							{
								list.Add(dictionary.Keys.ToArray());
							}
							dictionary = new Dictionary<int, long?>();
							num = int2;
						}
						dictionary.Add(@int, value);
					}
				}
			}
			if (dictionary != null && dictionary.Values.Any((long? x) => !x.HasValue || x.Value != currentSigHash))
			{
				list.Add(dictionary.Keys.ToArray());
			}
			return list;
		}

		private static DataTable GroupedTuidDataTable()
		{
			return new DataTable
			{
				Columns = 
				{
					{
						"TuId",
						typeof(int)
					},
					{
						"GroupId",
						typeof(int)
					}
				}
			};
		}

		private static void GroupedTuidDataTableBatch(DataTable dt, List<int[]> groupedTuIds, int minTUsPerBatch)
		{
			int num = 0;
			for (int i = 0; i < groupedTuIds.Count; i++)
			{
				int[] array = groupedTuIds[i];
				foreach (int num2 in array)
				{
					DataRow dataRow = dt.NewRow();
					dataRow["TuId"] = num2;
					dataRow["GroupId"] = i + 1;
					dt.Rows.Add(dataRow);
					num++;
				}
				if (num >= minTUsPerBatch)
				{
					groupedTuIds.RemoveRange(0, i + 1);
					return;
				}
			}
			groupedTuIds.Clear();
		}

		public void AddDeduplicatedContextHashes(int tmId, ref List<int[]> tuIdsWithDupSourceHashes, ref List<int[]> tuIdsWithDupTargetHashes)
		{
			if (tuIdsWithDupSourceHashes.Count != 0 || (tuIdsWithDupTargetHashes != null && tuIdsWithDupTargetHashes.Count != 0))
			{
				using (SqlCommand sqlCommand = CreateCommand("CREATE TABLE #SourceHashMap (newHash BIGINT, althash BIGINT)", tmId, requiresTransaction: true))
				{
					sqlCommand.ExecuteNonQuery();
					sqlCommand.CommandText = "CREATE TABLE #TargetHashMap (newHash BIGINT, althash BIGINT)";
					sqlCommand.ExecuteNonQuery();
				}
				DataTable dataTable = GroupedTuidDataTable();
				using (SqlCommand sqlCommand2 = CreateCommand("dbo.create_hash_expansion_map_%%", tmId, requiresTransaction: true))
				{
					sqlCommand2.CommandType = CommandType.StoredProcedure;
					SqlParameter sqlParameter = sqlCommand2.Parameters.AddWithValue("@target", false);
					SqlParameter sqlParameter2 = sqlCommand2.Parameters.Add("@tuIdsWithDupHashes", SqlDbType.Structured);
					while (tuIdsWithDupSourceHashes.Count > 0)
					{
						dataTable.Rows.Clear();
						GroupedTuidDataTableBatch(dataTable, tuIdsWithDupSourceHashes, 100);
						sqlParameter2.Value = dataTable;
						sqlCommand2.ExecuteNonQuery();
					}
					if (tuIdsWithDupTargetHashes != null)
					{
						sqlParameter.Value = true;
						while (tuIdsWithDupTargetHashes.Count > 0)
						{
							dataTable.Rows.Clear();
							GroupedTuidDataTableBatch(dataTable, tuIdsWithDupTargetHashes, 100);
							sqlParameter2.Value = dataTable;
							sqlCommand2.ExecuteNonQuery();
						}
					}
				}
				int num = 0;
				using (SqlCommand sqlCommand3 = CreateCommand("dbo.process_additional_hashes_for_tu_%%", tmId, requiresTransaction: true))
				{
					sqlCommand3.CommandType = CommandType.StoredProcedure;
					SqlParameter sqlParameter3 = sqlCommand3.Parameters.AddWithValue("@start_after", num);
					sqlCommand3.Parameters.AddWithValue("@count", 100);
					sqlCommand3.Parameters.AddWithValue("@cm_is_preceding_following", (tuIdsWithDupTargetHashes == null) ? 1 : 0);
					while (true)
					{
						sqlParameter3.Value = num;
						object obj = sqlCommand3.ExecuteScalar();
						if (obj == DBNull.Value)
						{
							break;
						}
						int num2 = Convert.ToInt32(obj);
						if (num2 < num + 100)
						{
							break;
						}
						num = num2;
					}
				}
			}
		}

		public void ClearFuzzyCache(Container container)
		{
			throw new NotImplementedException();
		}

		public override bool HasGuids()
		{
			return true;
		}

		public override bool HasFlags()
		{
			return false;
		}

		public static SqlStorage Create(DatabaseContainer container)
		{
			return new SqlStorage(SqlStorageUtils.BuildConnectionString(container));
		}

		public List<int> GetTmIds()
		{
			List<int> list = new List<int>();
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tms", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
				{
					while (sqlDataReader.Read())
					{
						int @int = sqlDataReader.GetInt32(0);
						list.Add(@int);
					}
					return list;
				}
			}
		}

		public bool DeleteTm(int key, bool ignoreErrors)
		{
			return DeleteTm(key, ignoreErrors, checkVersion: true);
		}

		private bool DeleteTm(int key, bool ignoreErrors, bool checkVersion)
		{
			if (checkVersion)
			{
				CheckVersion();
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_tm", requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = key;
				return sqlCommand.ExecuteNonQuery() > 0;
			}
		}

		internal SqlCommand CreateCommand(string cmdText, bool requiresTransaction)
		{
			SqlCommand sqlCommand = string.IsNullOrEmpty(cmdText) ? new SqlCommand() : new SqlCommand(cmdText);
			InitializeCommand(sqlCommand, requiresTransaction);
			return sqlCommand;
		}

		internal SqlCommand CreateCommand(string cmdText, int tmId, bool requiresTransaction)
		{
			if (!string.IsNullOrEmpty(cmdText))
			{
				cmdText = cmdText.Replace("%%", tmId.ToString(CultureInfo.InvariantCulture));
			}
			return CreateCommand(cmdText, requiresTransaction);
		}

		internal void CreateTmSchema(int tmId)
		{
			ExecuteSchemaCommand("Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.SQL.CreateTMSchemaSQL.sql", tmId, ignoreErrors: false);
		}

		private void ExecuteSchemaCommand(string schemaName, bool ignoreErrors)
		{
			ExecuteSchemaCommand(schemaName, 0, ignoreErrors);
		}

		private void ExecuteSchemaCommand(string schemaName, int key, bool ignoreErrors)
		{
			string[] array = ReadSchemaCommands(schemaName, "\r\nGO", this);
			using (SqlCommand sqlCommand = CreateCommand(null, requiresTransaction: true))
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					sqlCommand.CommandText = ((key > 0) ? text.Replace("%%", key.ToString(CultureInfo.InvariantCulture)) : text);
					try
					{
						sqlCommand.ExecuteNonQuery();
					}
					catch (SqlException ex)
					{
						if (!ignoreErrors)
						{
							throw ex;
						}
					}
				}
			}
		}

		private void CheckVersion()
		{
			if (_versionChecked)
			{
				return;
			}
			_versionChecked = true;
			string version = GetVersion();
			SchemaVersionCompatibility schemaVersionCompatibility = GetSchemaVersionCompatibility("11.0.11", version);
			if (schemaVersionCompatibility != SchemaVersionCompatibility.Compatible)
			{
				string parameter = GetParameter("CompatibleVersion");
				if (string.IsNullOrEmpty(parameter))
				{
					throw HandleIncompatibleVersion(schemaVersionCompatibility, "11.0.11", version);
				}
				schemaVersionCompatibility = GetSchemaVersionCompatibility("11.0.11", parameter);
				if (schemaVersionCompatibility != SchemaVersionCompatibility.Compatible)
				{
					throw HandleIncompatibleVersion(schemaVersionCompatibility, "11.0.11", version);
				}
			}
		}

		private LanguagePlatformException HandleIncompatibleVersion(SchemaVersionCompatibility compatibility, string softwareVersion, string databaseVersion)
		{
			switch (compatibility)
			{
			case SchemaVersionCompatibility.Older:
				return new LanguagePlatformException(new FaultDescription(ErrorCode.StorageVersionDataOutdated, FaultStatus.Error)
				{
					Message = string.Format(FaultDescription.GetDescriptionFromErrorCode(ErrorCode.StorageVersionDataOutdated), databaseVersion, softwareVersion)
				});
			case SchemaVersionCompatibility.Newer:
				return new LanguagePlatformException(ErrorCode.StorageVersionDataNewer);
			default:
				throw new ArgumentOutOfRangeException("compatibility", compatibility, null);
			}
		}

		private SchemaVersionCompatibility GetSchemaVersionCompatibility(string expectedVersion, string actualVersion)
		{
			if (!TryParseVersion(expectedVersion, out Version version))
			{
				throw new ArgumentException("Invalid version", "expectedVersion");
			}
			if (!TryParseVersion(actualVersion, out Version version2))
			{
				throw new ArgumentException("Invalid version", "actualVersion");
			}
			Version v = new Version(version.Major + 1, 0, 0);
			if (version2 >= v)
			{
				return SchemaVersionCompatibility.Newer;
			}
			if (version2 >= version && version2 < v)
			{
				return SchemaVersionCompatibility.Compatible;
			}
			if (!(version2 < version))
			{
				return SchemaVersionCompatibility.Unknown;
			}
			return SchemaVersionCompatibility.Older;
		}

		internal static bool TryParseVersion(string semanticVersion, out Version version)
		{
			version = null;
			Match match = new Regex("^(?<major>[0]|[1-9]+[0-9]*)((\\.(?<minor>[0]|[1-9]+[0-9]*))(\\.(?<patch>[0]|[1-9]+[0-9]*))?)?(\\-(?<pre>[0-9A-Za-z\\-\\.]+))?(\\+(?<build>[0-9A-Za-z\\-\\.]+))?$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant).Match(semanticVersion);
			if (!match.Success)
			{
				return false;
			}
			Group group = match.Groups["major"];
			Group group2 = match.Groups["minor"];
			Group group3 = match.Groups["patch"];
			if (!group.Success)
			{
				return false;
			}
			int major = int.Parse(group.Value, CultureInfo.InvariantCulture);
			if (!group2.Success)
			{
				return false;
			}
			int minor = int.Parse(group2.Value, CultureInfo.InvariantCulture);
			int build = 0;
			if (group3.Success)
			{
				build = int.Parse(group3.Value, CultureInfo.InvariantCulture);
			}
			version = new Version(major, minor, build);
			return true;
		}

		private static byte[] GetFeaturesData(List<int> features)
		{
			if (features == null || features.Count == 0)
			{
				return null;
			}
			byte[] array = new byte[features.Count * 4];
			for (int i = 0; i < features.Count; i++)
			{
				int num = features[i];
				int num2 = i * 4;
				array[num2] = (byte)((num & 4278190080u) / 16777216);
				array[num2 + 1] = (byte)((num & 0xFF0000) / 65536);
				array[num2 + 2] = (byte)((num & 0xFF00) / 256);
				array[num2 + 3] = (byte)(num & 0xFF);
			}
			return array;
		}

		private static byte[] GetContextsData(TuContexts contexts)
		{
			if (contexts == null || contexts.Length == 0)
			{
				return null;
			}
			int num = 0;
			byte[] array = new byte[contexts.Length * 2 * 8];
			foreach (TuContext value in contexts.Values)
			{
				long context = value.Context1;
				long context2 = value.Context2;
				byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(context));
				byte[] bytes2 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(context2));
				Array.Copy(bytes, 0, array, num, 8);
				num += 8;
				Array.Copy(bytes2, 0, array, num, 8);
				num += 8;
			}
			return array;
		}

		private void DeleteAttributeValues(int tmId, TranslationUnit tu)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.delete_attribute_values_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@tu_id", SqlDbType.Int).Value = tu.Id;
				sqlCommand.ExecuteNonQuery();
			}
		}

		private void AddAttributeValues(int tmId, TranslationUnit tu)
		{
			if (tu.Attributes != null && tu.Attributes.Count > 0)
			{
				using (SqlCommand sqlCommand = CreateCommand("dbo.add_attribute_value_%%", tmId, requiresTransaction: true))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.Parameters.Add("@attribute_id", SqlDbType.Int);
					sqlCommand.Parameters.Add("@tu_id", SqlDbType.Int).Value = tu.Id;
					sqlCommand.Parameters.Add("@date_value", SqlDbType.DateTime).Value = DBNull.Value;
					sqlCommand.Parameters.Add("@string_value", SqlDbType.NVarChar).Value = DBNull.Value;
					sqlCommand.Parameters.Add("@numeric_value", SqlDbType.Int).Value = DBNull.Value;
					sqlCommand.Parameters.Add("@picklist_value_id", SqlDbType.Int).Value = DBNull.Value;
					AddAttributeValues(tu.Attributes, sqlCommand);
				}
			}
		}

		private List<TranslationUnit> GetTus_SingleRoundtrip(int tmId, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tus_ex_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@start_after", SqlDbType.Int).Value = startAfter;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@forward", SqlDbType.Int).Value = (forward ? 1 : 0);
				sqlCommand.Parameters.Add("@returnIdContext", SqlDbType.Int).Value = (idContextMatch ? 1 : 0);
				sqlCommand.Parameters.Add("@includeContextContent", SqlDbType.Int).Value = (includeContextContent ? 1 : 0);
				sqlCommand.Parameters.Add("@cm_is_preceding_following", SqlDbType.Bit).Value = (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
				List<TranslationUnit> tUsWithAttributesAndContexts = GetTUsWithAttributesAndContexts(sqlCommand, idContextMatch, returnContext: true, null, sourceCulture, targetCulture, textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
				if (forward)
				{
					tUsWithAttributesAndContexts.Sort((TranslationUnit x, TranslationUnit y) => x.Id - y.Id);
				}
				else
				{
					tUsWithAttributesAndContexts.Sort((TranslationUnit x, TranslationUnit y) => y.Id - x.Id);
				}
				return tUsWithAttributesAndContexts;
			}
		}

		private bool HasFGASupport(int id)
		{
			TranslationMemory tm = GetTm(id);
			if (tm.FGASupport != FGASupport.NonAutomatic)
			{
				return tm.FGASupport == FGASupport.Automatic;
			}
			return true;
		}

		public void ClearAlignmentData(int tmId)
		{
			if (HasFGASupport(tmId))
			{
				using (SqlCommand sqlCommand = CreateCommand("clear_alignment_data_%%", tmId, requiresTransaction: false))
				{
					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public int GetPostdatedTranslationUnitCount(int tmid, DateTime modelDate)
		{
			if (!HasFGASupport(tmid))
			{
				return 0;
			}
			using (SqlCommand sqlCommand = CreateCommand("get_postdated_tu_count_%%", tmid, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("dateVal", DbStorageBase.Normalize(modelDate));
				return Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
		}

		public int GetAlignedPredatedTranslationUnitCount(int tmid, DateTime modelDate)
		{
			if (!HasFGASupport(tmid))
			{
				return 0;
			}
			using (SqlCommand sqlCommand = CreateCommand("get_aligned_predated_tu_count_%%", tmid, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("model_date", DbStorageBase.Normalize(modelDate));
				return Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
		}

		public int GetUnalignedCount(int tmid, DateTime? modelDate)
		{
			if (!HasFGASupport(tmid))
			{
				return 0;
			}
			using (SqlCommand sqlCommand = CreateCommand("sp_get_unaligned_tu_count_%%", tmid, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				if (modelDate.HasValue)
				{
					sqlCommand.Parameters.AddWithValue("model_date", DateTimeUtilities.Normalize(modelDate.Value));
				}
				return Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
		}

		public int GetPairCount(int tmid)
		{
			if (HasFGASupport(tmid))
			{
				return GetTuCount(tmid);
			}
			return 0;
		}

		public AlignerDefinition GetAlignerDefinition(int tmId)
		{
			if (!HasFGASupport(tmId))
			{
				return null;
			}
			using (SqlCommand sqlCommand = CreateCommand("select aligner_definition from translation_memories where id = " + tmId.ToString(), requiresTransaction: false))
			{
				using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
				{
					if (!sqlDataReader.Read())
					{
						throw new Exception("TM not found: " + tmId.ToString());
					}
					if (sqlDataReader.IsDBNull(0))
					{
						return null;
					}
					return AlignerDefinition.FromSerialization(sqlDataReader.GetFieldValue<byte[]>(0));
				}
			}
		}

		public void SetAlignerDefinition(int tmId, AlignerDefinition definition)
		{
			TranslationMemory tm = GetTm(tmId);
			byte[] value = null;
			if (definition != null)
			{
				value = definition.ToSerialization();
			}
			using (SqlCommand sqlCommand = CreateCommand("update translation_memories set aligner_definition = @aligner_definition where id = " + tmId.ToString(), requiresTransaction: true))
			{
				sqlCommand.Parameters.Add("@aligner_definition", SqlDbType.VarBinary).Value = value;
				sqlCommand.ExecuteNonQuery();
				if (tm.FGASupport == FGASupport.Off)
				{
					sqlCommand.CommandText = "update translation_memories set FGASupport = " + 2.ToString() + " where id = " + tmId.ToString();
				}
			}
		}

		public void SetIsAlignmentEnabled(int tmId, bool enabled)
		{
			if (!HasFGASupport(tmId))
			{
				throw new Exception("The TM does not support FGA");
			}
			string cmdText = "update translation_memories set fga_support = " + (enabled ? 3 : 2).ToString() + " where id = " + tmId.ToString();
			using (SqlCommand sqlCommand = CreateCommand(cmdText, tmId, requiresTransaction: false))
			{
				sqlCommand.ExecuteNonQuery();
			}
		}

		private static DataTable ConvertListToSqlDataTable(List<int> tuIds)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("TuId", typeof(int));
			if (tuIds == null)
			{
				return dataTable;
			}
			foreach (int tuId in tuIds)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = tuId;
				dataTable.Rows.Add(dataRow);
			}
			return dataTable;
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(int tmId, List<int> tuIds)
		{
			DataTable value = ConvertListToSqlDataTable(tuIds);
			using (SqlCommand sqlCommand = CreateCommand("dbo.sp_GetAlignmentTimestampsByIds_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@tu_ids", value);
				return GetAlignmentTimestamps(sqlCommand);
			}
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(int tmId, int startAfter, int count, DateTime modelDate)
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.sp_GetAlignmentTimestampsPaginated_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@start_after", SqlDbType.Int).Value = startAfter;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@model_date", SqlDbType.DateTime).Value = DateTimeUtilities.Normalize(modelDate);
				return GetAlignmentTimestamps(sqlCommand);
			}
		}

		public List<TranslationUnit> GetAlignableTus(int tmId, List<int> tuIds)
		{
			DataTable value = ConvertListToSqlDataTable(tuIds);
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tus_by_ids_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@tu_ids", value);
				return GetTUsOnly(sqlCommand);
			}
		}

		public List<TranslationUnit> GetFullTusByIds(int tmId, List<int> tuIds)
		{
			DataTable value = ConvertListToSqlDataTable(tuIds);
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_full_tus_by_ids_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@tu_ids", value);
				return GetTUsWithAttributesAndContexts(sqlCommand, returnIdContext: false);
			}
		}

		public List<TranslationUnit> GetAlignableTus(int tmId, int startAfter, int count, bool unalignedOnly, bool unalignedOrPostdated)
		{
			if (unalignedOnly && unalignedOrPostdated)
			{
				throw new Exception("GetAlignableTus - unalignedOnly && unalignedOrPostdated");
			}
			using (SqlCommand sqlCommand = CreateCommand("dbo.get_tus_ToAlign_%%", tmId, requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.Add("@start_after", SqlDbType.Int).Value = startAfter;
				sqlCommand.Parameters.Add("@count", SqlDbType.Int).Value = count;
				sqlCommand.Parameters.Add("@unalignedorpostdated", SqlDbType.Bit).Value = unalignedOrPostdated;
				sqlCommand.Parameters.Add("@unaligned", SqlDbType.Bit).Value = unalignedOnly;
				List<TranslationUnit> tUsOnly = GetTUsOnly(sqlCommand);
				tUsOnly.Sort((TranslationUnit x, TranslationUnit y) => x.Id - y.Id);
				return tUsOnly;
			}
		}

		public void PrepareForModelBuild(int tmId)
		{
		}

		public bool CleanupSchema()
		{
			using (SqlCommand sqlCommand = CreateCommand("dbo.tm_cleanup", requiresTransaction: false))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.ExecuteNonQuery();
			}
			return true;
		}

		public override void SerializeTuSegments(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, TranslationUnit storageTu)
		{
			SegmentSerialization.Save(tu.SourceSegment, storageTu.Source);
			SegmentSerialization.Save(tu.TargetSegment, storageTu.Target);
		}

		public void AddorUpdateLastSearch(int tmId, List<int> tuIds, DateTime lastSearch)
		{
			DataTable value = ConvertListToSqlDataTable(tuIds);
			using (SqlCommand sqlCommand = CreateCommand("dbo.sp_addUpdate_tu_lastSearch_%%", tmId, requiresTransaction: true))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue("@tu_ids", value);
				sqlCommand.Parameters.Add("@lastSearch", SqlDbType.DateTime).Value = lastSearch;
				sqlCommand.ExecuteNonQuery();
			}
		}
	}
}
