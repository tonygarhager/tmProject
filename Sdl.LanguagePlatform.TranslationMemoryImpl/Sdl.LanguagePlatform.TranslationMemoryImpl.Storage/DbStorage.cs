using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal abstract class DbStorage : DbStorageBase
	{
		public class BooleanSettingsWrapper
		{
			public BuiltinRecognizers BuiltinRecognizers;

			public TokenizerFlags TokenizerFlags;

			public WordCountFlags WordCountFlags;

			public int DbSettingsValue
			{
				get
				{
					return PackDbSettingsValue();
				}
				set
				{
					UnpackDbSettingsValue(value);
				}
			}

			public BooleanSettingsWrapper(BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags)
			{
				BuiltinRecognizers = recognizers;
				TokenizerFlags = tokenizerFlags;
				WordCountFlags = wordCountFlags;
			}

			public BooleanSettingsWrapper(int dbsettingsValue)
			{
				UnpackDbSettingsValue(dbsettingsValue);
			}

			private void UnpackDbSettingsValue(int dbsettingsValue)
			{
				BuiltinRecognizers = (BuiltinRecognizers)(dbsettingsValue & 0x7F);
				int num = (dbsettingsValue & 0xFF0000) >> 16;
				num ^= 1;
				num ^= 2;
				num ^= 4;
				TokenizerFlags = (TokenizerFlags)(num & 7);
				num >>= 4;
				num ^= 4;
				WordCountFlags = (WordCountFlags)(num & 0xF);
			}

			private int PackDbSettingsValue()
			{
				return (((((int)(WordCountFlags ^ WordCountFlags.BreakOnTag) << 4) | (int)TokenizerFlags) ^ 1 ^ 2 ^ 4) << 16) | (int)BuiltinRecognizers;
			}
		}

		public const string AlignerDefinitionKey = "AlignerDefinition";

		public const string AlignmentDataVersionKey = "AlignmentDataVersion";

		public const int AlignmentDataVersion = 1;

		internal const int CurrentDataVersion = 1;

		public const string TokenDataVersionKey = "TokenDataVersion";

		public const int TokenDataVersion = 1;

		protected XmlSegmentSerializer XmlSegmentSerializer = new XmlSegmentSerializer();

		public bool IsInMemoryStorage => false;

		protected abstract bool CanReportReindexRequired
		{
			get;
		}

		public static string BinarySerializeObjectToString(object o)
		{
			return Convert.ToBase64String(BinarySerializeObjectToByteArray(o));
		}

		public static byte[] BinarySerializeObjectToByteArray(object o)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(memoryStream, o);
				memoryStream.Flush();
				memoryStream.Position = 0L;
				return memoryStream.ToArray();
			}
		}

		public static object BinarySerializeStringToObject(string s)
		{
			return BinarySerializeByteArrayToObject(Convert.FromBase64String(s));
		}

		public static object BinarySerializeByteArrayToObject(byte[] b)
		{
			using (MemoryStream memoryStream = new MemoryStream(b))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return binaryFormatter.Deserialize(memoryStream);
			}
		}

		public abstract string GetVersion();

		public abstract bool HasGuids();

		public abstract bool HasFlags();

		protected byte[] GetBlob(DbDataReader rdr, int column)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] buffer = new byte[4096];
				long num = 0L;
				long bytes;
				do
				{
					bytes = rdr.GetBytes(column, num, buffer, 0, 4096);
					num += bytes;
					if (bytes > 0)
					{
						memoryStream.Write(buffer, 0, (int)bytes);
					}
				}
				while (bytes == 4096);
				return memoryStream.ToArray();
			}
		}

		protected byte[] Compress(byte[] input)
		{
			using (MemoryStream memoryStream2 = new MemoryStream(input))
			{
				byte[] result;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
					{
						byte[] buffer = new byte[16384];
						int num;
						do
						{
							num = memoryStream2.Read(buffer, 0, 16384);
							if (num > 0)
							{
								gZipStream.Write(buffer, 0, num);
							}
						}
						while (num == 16384);
						gZipStream.Close();
					}
					memoryStream.Flush();
					memoryStream.Close();
					result = memoryStream.ToArray();
				}
				memoryStream2.Close();
				return result;
			}
		}

		protected byte[] Uncompress(byte[] input)
		{
			using (MemoryStream memoryStream = new MemoryStream(input))
			{
				byte[] result;
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
					{
						byte[] buffer = new byte[16384];
						int num;
						do
						{
							num = gZipStream.Read(buffer, 0, 16384);
							if (num > 0)
							{
								memoryStream2.Write(buffer, 0, num);
							}
						}
						while (num == 16384);
						gZipStream.Close();
					}
					memoryStream2.Flush();
					result = memoryStream2.ToArray();
					memoryStream2.Close();
				}
				memoryStream.Close();
				return result;
			}
		}

		protected virtual int GetInt32(DbDataReader rdr, int column)
		{
			return rdr.GetInt32(column);
		}

		protected virtual long GetInt64(DbDataReader rdr, int column)
		{
			return rdr.GetInt64(column);
		}

		protected virtual string GetString(DbDataReader rdr, int column)
		{
			return rdr.GetString(column);
		}

		protected virtual DateTime GetDateTime(DbDataReader rdr, int column)
		{
			return rdr.GetDateTime(column);
		}

		protected List<Resource> GetResources(DbCommand cmd, bool includeData)
		{
			List<Resource> list = new List<Resource>();
			DbDataReader dbDataReader = null;
			try
			{
				dbDataReader = cmd.ExecuteReader();
				while (dbDataReader.Read())
				{
					int @int = GetInt32(dbDataReader, 0);
					Guid guid = HasGuids() ? dbDataReader.GetGuid(1) : new Guid(dbDataReader.GetString(1));
					LanguageResourceType int2 = (LanguageResourceType)GetInt32(dbDataReader, 2);
					string language = dbDataReader.IsDBNull(3) ? null : dbDataReader.GetString(3);
					byte[] data = includeData ? GetBlob(dbDataReader, 4) : null;
					list.Add(new Resource(@int, guid, int2, language, data));
				}
				return list;
			}
			finally
			{
				if (dbDataReader != null)
				{
					dbDataReader.Close();
					dbDataReader.Dispose();
				}
			}
		}

		protected Resource GetResource(DbCommand cmd, bool includeData)
		{
			List<Resource> resources = GetResources(cmd, includeData);
			if (resources.Count <= 0)
			{
				return null;
			}
			return resources[0];
		}

		protected List<TranslationMemory> GetTms(DbCommand cmd)
		{
			List<TranslationMemory> list = new List<TranslationMemory>();
			DbDataReader dbDataReader = null;
			try
			{
				dbDataReader = cmd.ExecuteReader();
				while (dbDataReader.Read())
				{
					BooleanSettingsWrapper booleanSettingsWrapper = new BooleanSettingsWrapper(GetInt32(dbDataReader, 7));
					DateTime? expirationDate = DateTimeUtilities.Normalize(DateTime.MaxValue);
					if (!dbDataReader.IsDBNull(10))
					{
						expirationDate = DateTime.SpecifyKind(GetDateTime(dbDataReader, 10), DateTimeKind.Utc);
					}
					TranslationMemory translationMemory = new TranslationMemory(GetInt32(dbDataReader, 0), HasGuids() ? dbDataReader.GetGuid(1) : new Guid(dbDataReader.GetString(1)), dbDataReader.GetString(2), dbDataReader.GetString(3), dbDataReader.GetString(4), booleanSettingsWrapper.BuiltinRecognizers, dbDataReader.GetString(8), DateTime.SpecifyKind(GetDateTime(dbDataReader, 9), DateTimeKind.Utc), dbDataReader.IsDBNull(5) ? null : dbDataReader.GetString(5), dbDataReader.IsDBNull(6) ? null : dbDataReader.GetString(6), expirationDate, booleanSettingsWrapper.TokenizerFlags, booleanSettingsWrapper.WordCountFlags);
					translationMemory.FuzzyIndexes = (FuzzyIndexes)GetInt32(dbDataReader, 11);
					translationMemory.LastRecomputeDate = (dbDataReader.IsDBNull(12) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(dbDataReader, 12), DateTimeKind.Utc)));
					translationMemory.LastRecomputeSize = (dbDataReader.IsDBNull(13) ? null : new int?(GetInt32(dbDataReader, 13)));
					translationMemory.FGASupport = (FGASupport)GetInt32(dbDataReader, 14);
					translationMemory.DataVersion = GetInt32(dbDataReader, 15);
					translationMemory.TextContextMatchType = (TextContextMatchType)GetInt32(dbDataReader, 16);
					translationMemory.IdContextMatch = (!dbDataReader.IsDBNull(17) && dbDataReader.GetBoolean(17));
					translationMemory.CanReportReindexRequired = CanReportReindexRequired;
					list.Add(translationMemory);
				}
				return list;
			}
			finally
			{
				if (dbDataReader != null)
				{
					dbDataReader.Close();
					dbDataReader.Dispose();
				}
			}
		}

		protected TranslationMemory GetTm(DbCommand cmd)
		{
			List<TranslationMemory> tms = GetTms(cmd);
			if (tms.Count <= 0)
			{
				return null;
			}
			return tms[0];
		}

		protected List<AttributeDeclaration> GetAttributes(DbCommand cmd1, DbCommand cmd2)
		{
			List<AttributeDeclaration> list = new List<AttributeDeclaration>();
			DbDataReader dbDataReader = null;
			try
			{
				dbDataReader = cmd1.ExecuteReader();
				while (dbDataReader.Read())
				{
					list.Add(new AttributeDeclaration(GetInt32(dbDataReader, 0), HasGuids() ? dbDataReader.GetGuid(1) : new Guid(dbDataReader.GetString(1)), dbDataReader.GetString(2), (FieldValueType)GetInt32(dbDataReader, 3), GetInt32(dbDataReader, 4)));
				}
			}
			finally
			{
				if (dbDataReader != null)
				{
					dbDataReader.Close();
					dbDataReader.Dispose();
				}
			}
			cmd2.Prepare();
			foreach (AttributeDeclaration item in list)
			{
				if (item.Type == FieldValueType.MultiplePicklist || item.Type == FieldValueType.SinglePicklist)
				{
					cmd2.Parameters[0].Value = item.Id;
					List<PickValue> picklistValues = GetPicklistValues(cmd2);
					picklistValues.Sort();
					item.Picklist.AddRange(picklistValues);
				}
			}
			return list;
		}

		protected AttributeDeclaration GetAttribute(DbCommand cmd1, DbCommand cmd2)
		{
			List<AttributeDeclaration> attributes = GetAttributes(cmd1, cmd2);
			if (attributes.Count <= 0)
			{
				return null;
			}
			return attributes[0];
		}

		protected List<PickValue> GetPicklistValues(DbCommand cmd)
		{
			List<PickValue> list = new List<PickValue>();
			DbDataReader dbDataReader = null;
			try
			{
				dbDataReader = cmd.ExecuteReader();
				while (dbDataReader.Read())
				{
					list.Add(new PickValue(GetInt32(dbDataReader, 0), HasGuids() ? dbDataReader.GetGuid(1) : new Guid(dbDataReader.GetString(1)), dbDataReader.GetString(2)));
				}
				return list;
			}
			finally
			{
				if (dbDataReader != null)
				{
					dbDataReader.Close();
					dbDataReader.Dispose();
				}
			}
		}

		protected PickValue GetPicklistValue(DbCommand cmd)
		{
			List<PickValue> picklistValues = GetPicklistValues(cmd);
			if (picklistValues.Count <= 0)
			{
				return null;
			}
			return picklistValues[0];
		}

		protected void AddAttributeValues(List<AttributeValue> attributes, DbCommand cmd)
		{
			cmd.Prepare();
			foreach (AttributeValue attribute in attributes)
			{
				bool flag = false;
				int num;
				switch (attribute.Type)
				{
				case FieldValueType.SingleString:
					num = 3;
					break;
				case FieldValueType.MultipleString:
					num = 3;
					flag = true;
					break;
				case FieldValueType.DateTime:
					num = 2;
					break;
				case FieldValueType.Integer:
					num = 4;
					break;
				case FieldValueType.SinglePicklist:
					num = 5;
					break;
				case FieldValueType.MultiplePicklist:
					num = 5;
					flag = true;
					break;
				default:
					throw new ArgumentOutOfRangeException("attributes", attribute.Type, "unknown value.Type");
				}
				cmd.Parameters[0].Value = attribute.DeclarationId;
				if (flag)
				{
					if (num == 3)
					{
						string[] array = (string[])attribute.Value;
						foreach (string value in array)
						{
							cmd.Parameters[num].Value = value;
							cmd.ExecuteNonQuery();
						}
					}
					else
					{
						int[] array2 = (int[])attribute.Value;
						foreach (int num2 in array2)
						{
							cmd.Parameters[num].Value = num2;
							cmd.ExecuteNonQuery();
						}
					}
				}
				else
				{
					cmd.Parameters[num].Value = attribute.Value;
					cmd.ExecuteNonQuery();
				}
				cmd.Parameters[num].Value = DBNull.Value;
			}
		}

		protected void AddContexts(TuContexts contexts, DbCommand cmd)
		{
			cmd.Prepare();
			foreach (TuContext value in contexts.Values)
			{
				cmd.Parameters[1].Value = value.Context1;
				cmd.Parameters[2].Value = value.Context2;
				cmd.ExecuteNonQuery();
			}
		}

		protected List<TranslationUnit> GetTuRange(DbCommand getTusCmd, DbCommand getTextContextsCmd, DbCommand getIdContextsCmd, DbCommand getAttributesCmd, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			int num = -1;
			int num2 = -1;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			using (DbDataReader dbDataReader = getTusCmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					TranslationUnit translationUnit = ReadTu(dbDataReader);
					if (!dictionary.ContainsKey(translationUnit.Id))
					{
						translationUnit.Contexts = new TuContexts();
						list.Add(translationUnit);
						if (num < 0 || translationUnit.Id < num)
						{
							num = translationUnit.Id;
						}
						if (num2 < 0 || translationUnit.Id > num2)
						{
							num2 = translationUnit.Id;
						}
						dictionary.Add(translationUnit.Id, list.Count - 1);
					}
				}
			}
			getTextContextsCmd.Parameters["@from_tu_id"].Value = num;
			getTextContextsCmd.Parameters["@into_tu_id"].Value = num2;
			using (DbDataReader contextReader = getTextContextsCmd.ExecuteReader())
			{
				ReadTextContexts(contextReader, list, dictionary, sourceCulture, targetCulture, textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource);
			}
			if (getIdContextsCmd != null)
			{
				getIdContextsCmd.Parameters["@from_tu_id"].Value = num;
				getIdContextsCmd.Parameters["@into_tu_id"].Value = num2;
				using (DbDataReader contextReader2 = getIdContextsCmd.ExecuteReader())
				{
					ReadIdContexts(contextReader2, list, dictionary);
				}
			}
			getAttributesCmd.Parameters["@from_tu_id"].Value = num;
			getAttributesCmd.Parameters["@into_tu_id"].Value = num2;
			using (DbDataReader reader = getAttributesCmd.ExecuteReader())
			{
				ReadAttributeValues(reader, list, dictionary, autoAdvanceResultSet: true);
				return list;
			}
		}

		protected List<TranslationUnit> GetTusWithAttributes(DbCommand getTusCmd, bool idContextMatch)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			using (DbDataReader dbDataReader = getTusCmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					TranslationUnit translationUnit = ReadTu(dbDataReader);
					translationUnit.Contexts = new TuContexts();
					list.Add(translationUnit);
					dictionary.Add(translationUnit.Id, list.Count - 1);
				}
				dbDataReader.NextResult();
				ReadTextContexts(dbDataReader, list, dictionary);
				if (idContextMatch)
				{
					dbDataReader.NextResult();
					ReadIdContexts(dbDataReader, list, dictionary);
				}
				ReadAttributeValues(dbDataReader, list, dictionary, autoAdvanceResultSet: true);
				return list;
			}
		}

		protected List<TranslationUnit> GetTuRange(DbCommand getTusCmd)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			int num = -1;
			int num2 = -1;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			using (DbDataReader dbDataReader = getTusCmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					TranslationUnit translationUnit = ReadTu(dbDataReader);
					if (!dictionary.ContainsKey(translationUnit.Id))
					{
						translationUnit.Contexts = new TuContexts();
						list.Add(translationUnit);
						if (num < 0 || translationUnit.Id < num)
						{
							num = translationUnit.Id;
						}
						if (num2 < 0 || translationUnit.Id > num2)
						{
							num2 = translationUnit.Id;
						}
						dictionary.Add(translationUnit.Id, list.Count - 1);
					}
				}
				return list;
			}
		}

		protected TranslationUnit ReadTu(DbDataReader reader)
		{
			if (!HasFlags())
			{
				return ReadTuNoFlags(reader);
			}
			return ReadTuWithFlags(reader);
		}

		protected TranslationUnit ReadTuWithFlags(DbDataReader reader)
		{
			switch (reader.FieldCount)
			{
			case 27:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), GetInt32(reader, 2), new Segment(GetInt64(reader, 3), 0L, reader.GetString(4), null, reader.IsDBNull(25) ? null : reader.GetFieldValue<byte[]>(25)), new Segment(GetInt64(reader, 7), 0L, reader.GetString(8), null, reader.IsDBNull(26) ? null : reader.GetFieldValue<byte[]>(26)), DateTime.SpecifyKind(GetDateTime(reader, 11), DateTimeKind.Utc), reader.GetString(12), DateTime.SpecifyKind(GetDateTime(reader, 13), DateTimeKind.Utc), reader.GetString(14), DateTime.SpecifyKind(GetDateTime(reader, 15), DateTimeKind.Utc), reader.GetString(16), GetInt32(reader, 17), (!reader.IsDBNull(18)) ? GetInt32(reader, 18) : 0, reader.IsDBNull(19) ? null : reader.GetFieldValue<byte[]>(19), reader.IsDBNull(20) ? null : reader.GetFieldValue<byte[]>(20), reader.IsDBNull(21) ? null : reader.GetFieldValue<byte[]>(21), reader.IsDBNull(22) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 22), DateTimeKind.Utc)), reader.IsDBNull(23) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 23), DateTimeKind.Utc)), reader.GetInt32(24));
			case 23:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), GetInt32(reader, 2), new Segment(GetInt64(reader, 3), 0L, reader.GetString(4), null, reader.IsDBNull(21) ? null : reader.GetFieldValue<byte[]>(21)), new Segment(GetInt64(reader, 5), 0L, reader.GetString(6), null, reader.IsDBNull(22) ? null : reader.GetFieldValue<byte[]>(22)), DateTime.SpecifyKind(GetDateTime(reader, 7), DateTimeKind.Utc), reader.GetString(8), DateTime.SpecifyKind(GetDateTime(reader, 9), DateTimeKind.Utc), reader.GetString(10), DateTime.SpecifyKind(GetDateTime(reader, 11), DateTimeKind.Utc), reader.GetString(12), GetInt32(reader, 13), (!reader.IsDBNull(14)) ? GetInt32(reader, 14) : 0, reader.IsDBNull(15) ? null : reader.GetFieldValue<byte[]>(15), reader.IsDBNull(16) ? null : reader.GetFieldValue<byte[]>(16), reader.IsDBNull(17) ? null : reader.GetFieldValue<byte[]>(17), reader.IsDBNull(18) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 18), DateTimeKind.Utc)), reader.IsDBNull(19) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 19), DateTimeKind.Utc)), reader.GetInt32(20));
			case 25:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), GetInt32(reader, 2), new Segment(GetInt64(reader, 3), 0L, reader.IsDBNull(4) ? reader.GetString(5) : reader.GetString(4), null, reader.IsDBNull(23) ? null : reader.GetFieldValue<byte[]>(23)), new Segment(GetInt64(reader, 6), 0L, reader.IsDBNull(7) ? reader.GetString(8) : reader.GetString(7), null, reader.IsDBNull(24) ? null : reader.GetFieldValue<byte[]>(24)), DateTime.SpecifyKind(GetDateTime(reader, 9), DateTimeKind.Utc), reader.GetString(10), DateTime.SpecifyKind(GetDateTime(reader, 11), DateTimeKind.Utc), reader.GetString(12), DateTime.SpecifyKind(GetDateTime(reader, 13), DateTimeKind.Utc), reader.GetString(14), GetInt32(reader, 15), (!reader.IsDBNull(16)) ? GetInt32(reader, 16) : 0, reader.IsDBNull(17) ? null : reader.GetFieldValue<byte[]>(17), reader.IsDBNull(18) ? null : reader.GetFieldValue<byte[]>(18), reader.IsDBNull(19) ? null : reader.GetFieldValue<byte[]>(19), reader.IsDBNull(20) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 20), DateTimeKind.Utc)), reader.IsDBNull(21) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 21), DateTimeKind.Utc)), reader.GetInt32(22));
			case 14:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), 0, new Segment(GetInt64(reader, 2), 0L, reader.GetString(3), null, reader.IsDBNull(12) ? null : reader.GetFieldValue<byte[]>(12)), new Segment(GetInt64(reader, 4), 0L, reader.GetString(5), null, reader.IsDBNull(13) ? null : reader.GetFieldValue<byte[]>(13)), DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", 0, 0, reader.IsDBNull(6) ? null : reader.GetFieldValue<byte[]>(6), reader.IsDBNull(7) ? null : reader.GetFieldValue<byte[]>(7), reader.IsDBNull(8) ? null : reader.GetFieldValue<byte[]>(8), reader.IsDBNull(9) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 9), DateTimeKind.Utc)), reader.IsDBNull(10) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 10), DateTimeKind.Utc)), reader.GetInt32(11));
			case 10:
			case 11:
				return new TranslationUnit(GetInt32(reader, 0), default(Guid), 0, new Segment(GetInt64(reader, 1), 0L, reader.GetString(2), null, reader.IsDBNull(8) ? null : reader.GetFieldValue<byte[]>(8)), new Segment(0L, 0L, reader.GetString(3), null, reader.IsDBNull(9) ? null : reader.GetFieldValue<byte[]>(9)), DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", 0, (!reader.IsDBNull(6)) ? reader.GetInt32(6) : 0, reader.IsDBNull(4) ? null : reader.GetFieldValue<byte[]>(4), reader.IsDBNull(5) ? null : reader.GetFieldValue<byte[]>(5), null, null, null, reader.GetInt32(7));
			default:
				throw new Exception("Invalid data retrieved from storage");
			}
		}

		protected TranslationUnit ReadTuNoFlags(DbDataReader reader)
		{
			switch (reader.FieldCount)
			{
			case 29:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), GetInt32(reader, 2), new Segment(GetInt64(reader, 3), 0L, reader.GetString(4), null, reader.IsDBNull(24) ? null : reader.GetFieldValue<byte[]>(24)), new Segment(GetInt64(reader, 7), 0L, reader.GetString(8), null, reader.IsDBNull(25) ? null : reader.GetFieldValue<byte[]>(25)), DateTime.SpecifyKind(GetDateTime(reader, 11), DateTimeKind.Utc), reader.GetString(12), DateTime.SpecifyKind(GetDateTime(reader, 13), DateTimeKind.Utc), reader.GetString(14), DateTime.SpecifyKind(GetDateTime(reader, 15), DateTimeKind.Utc), reader.GetString(16), GetInt32(reader, 17), -1, reader.IsDBNull(18) ? null : reader.GetFieldValue<byte[]>(18), reader.IsDBNull(19) ? null : reader.GetFieldValue<byte[]>(19), reader.IsDBNull(20) ? null : reader.GetFieldValue<byte[]>(20), reader.IsDBNull(21) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 21), DateTimeKind.Utc)), reader.IsDBNull(22) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 22), DateTimeKind.Utc)), reader.GetInt32(23), (TranslationUnitFormat)reader.GetByte(26), (TranslationUnitOrigin)reader.GetByte(27), (ConfirmationLevel)reader.GetByte(28));
			case 25:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), GetInt32(reader, 2), new Segment(GetInt64(reader, 3), 0L, reader.GetString(4), null, reader.IsDBNull(20) ? null : reader.GetFieldValue<byte[]>(20)), new Segment(GetInt64(reader, 5), 0L, reader.GetString(6), null, reader.IsDBNull(21) ? null : reader.GetFieldValue<byte[]>(21)), DateTime.SpecifyKind(GetDateTime(reader, 7), DateTimeKind.Utc), reader.GetString(8), DateTime.SpecifyKind(GetDateTime(reader, 9), DateTimeKind.Utc), reader.GetString(10), DateTime.SpecifyKind(GetDateTime(reader, 11), DateTimeKind.Utc), reader.GetString(12), GetInt32(reader, 13), -1, reader.IsDBNull(14) ? null : reader.GetFieldValue<byte[]>(14), reader.IsDBNull(15) ? null : reader.GetFieldValue<byte[]>(15), reader.IsDBNull(16) ? null : reader.GetFieldValue<byte[]>(16), reader.IsDBNull(17) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 17), DateTimeKind.Utc)), reader.IsDBNull(18) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 18), DateTimeKind.Utc)), reader.GetInt32(19), (TranslationUnitFormat)reader.GetByte(22), (TranslationUnitOrigin)reader.GetByte(23), (ConfirmationLevel)reader.GetByte(24));
			case 27:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), GetInt32(reader, 2), new Segment(GetInt64(reader, 3), 0L, reader.IsDBNull(4) ? reader.GetString(5) : reader.GetString(4), null, reader.IsDBNull(22) ? null : reader.GetFieldValue<byte[]>(22)), new Segment(GetInt64(reader, 6), 0L, reader.IsDBNull(7) ? reader.GetString(8) : reader.GetString(7), null, reader.IsDBNull(23) ? null : reader.GetFieldValue<byte[]>(23)), DateTime.SpecifyKind(GetDateTime(reader, 9), DateTimeKind.Utc), reader.GetString(10), DateTime.SpecifyKind(GetDateTime(reader, 11), DateTimeKind.Utc), reader.GetString(12), DateTime.SpecifyKind(GetDateTime(reader, 13), DateTimeKind.Utc), reader.GetString(14), GetInt32(reader, 15), -1, reader.IsDBNull(16) ? null : reader.GetFieldValue<byte[]>(16), reader.IsDBNull(17) ? null : reader.GetFieldValue<byte[]>(17), reader.IsDBNull(18) ? null : reader.GetFieldValue<byte[]>(18), reader.IsDBNull(19) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 19), DateTimeKind.Utc)), reader.IsDBNull(20) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 20), DateTimeKind.Utc)), reader.GetInt32(21), (TranslationUnitFormat)reader.GetByte(24), (TranslationUnitOrigin)reader.GetByte(25), (ConfirmationLevel)reader.GetByte(26));
			case 14:
				return new TranslationUnit(GetInt32(reader, 0), HasGuids() ? reader.GetGuid(1) : new Guid(reader.GetString(1)), 0, new Segment(GetInt64(reader, 2), 0L, reader.GetString(3), null, reader.IsDBNull(12) ? null : reader.GetFieldValue<byte[]>(12)), new Segment(GetInt64(reader, 4), 0L, reader.GetString(5), null, reader.IsDBNull(13) ? null : reader.GetFieldValue<byte[]>(13)), DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", 0, -1, reader.IsDBNull(6) ? null : reader.GetFieldValue<byte[]>(6), reader.IsDBNull(7) ? null : reader.GetFieldValue<byte[]>(7), reader.IsDBNull(8) ? null : reader.GetFieldValue<byte[]>(8), reader.IsDBNull(9) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 9), DateTimeKind.Utc)), reader.IsDBNull(10) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(reader, 10), DateTimeKind.Utc)), reader.GetInt32(11));
			case 12:
			case 13:
				return new TranslationUnit(GetInt32(reader, 0), default(Guid), 0, new Segment(GetInt64(reader, 1), 0L, reader.GetString(2), null, reader.IsDBNull(7) ? null : reader.GetFieldValue<byte[]>(7)), new Segment(0L, 0L, reader.GetString(3), null, reader.IsDBNull(8) ? null : reader.GetFieldValue<byte[]>(8)), DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", 0, -1, reader.IsDBNull(4) ? null : reader.GetFieldValue<byte[]>(4), reader.IsDBNull(5) ? null : reader.GetFieldValue<byte[]>(5), null, null, null, reader.GetInt32(6), (TranslationUnitFormat)reader.GetByte(9), (TranslationUnitOrigin)reader.GetByte(10), (ConfirmationLevel)reader.GetByte(11));
			default:
				throw new Exception("Invalid data retrieved from storage");
			}
		}

		protected void ReadAttributeValues(DbDataReader reader, List<TranslationUnit> tus, Dictionary<int, int> tuIndex, bool autoAdvanceResultSet, Dictionary<int, AttributeDeclaration> attributes = null)
		{
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			do
			{
				object obj = null;
				int num = 0;
				int num2 = 0;
				string name = null;
				int num3 = 0;
				TranslationUnit translationUnit;
				while (reader.Read())
				{
					int num4 = 0;
					bool num5 = reader.FieldCount == 5;
					int @int = GetInt32(reader, num4++);
					int int2 = GetInt32(reader, num4++);
					string text;
					int num6;
					if (num5)
					{
						text = reader.GetString(num4++);
						num6 = GetInt32(reader, num4++);
					}
					else
					{
						if (!attributes.TryGetValue(int2, out AttributeDeclaration value))
						{
							throw new Exception("Invalid attribute ID");
						}
						text = value.Name;
						num6 = (int)value.Type;
					}
					if ((@int != num && num != 0) || (int2 != num2 && num2 != 0))
					{
						if (!tuIndex.ContainsKey(num))
						{
							translationUnit = null;
						}
						else
						{
							translationUnit = tus[tuIndex[num]];
							switch (num3)
							{
							case 5:
								translationUnit.Attributes.Add(new AttributeValue(num2, name, (FieldValueType)num3, list2.ToArray()));
								break;
							case 2:
								list.Sort();
								translationUnit.Attributes.Add(new AttributeValue(num2, name, (FieldValueType)num3, list.ToArray()));
								break;
							default:
								translationUnit.Attributes.Add(new AttributeValue(num2, name, (FieldValueType)num3, obj));
								break;
							}
						}
						list2.Clear();
						list.Clear();
					}
					switch (num6)
					{
					case 5:
						list2.Add(GetInt32(reader, num4));
						break;
					case 2:
						list.Add(reader.GetString(num4));
						break;
					case 3:
						obj = GetDateTime(reader, num4);
						break;
					default:
						obj = reader.GetValue(num4);
						if (obj is decimal || obj is long)
						{
							obj = GetInt32(reader, num4);
						}
						break;
					}
					num4++;
					num2 = int2;
					name = text;
					num3 = num6;
					num = @int;
				}
				if (num2 == 0)
				{
					continue;
				}
				if (!tuIndex.ContainsKey(num))
				{
					translationUnit = null;
					continue;
				}
				translationUnit = tus[tuIndex[num]];
				switch (num3)
				{
				case 5:
					translationUnit.Attributes.Add(new AttributeValue(num2, name, (FieldValueType)num3, list2.ToArray()));
					break;
				case 2:
					list.Sort();
					translationUnit.Attributes.Add(new AttributeValue(num2, name, (FieldValueType)num3, list.ToArray()));
					break;
				default:
					translationUnit.Attributes.Add(new AttributeValue(num2, name, (FieldValueType)num3, obj));
					break;
				}
			}
			while (autoAdvanceResultSet && reader.NextResult());
		}

		protected void ReadTextContexts(DbDataReader contextReader, List<TranslationUnit> tus, Dictionary<int, int> tuIndex, CultureInfo sourceCulture = null, CultureInfo targetCulture = null, bool cmIsPrecedingFollowing = false)
		{
			while (contextReader.Read())
			{
				int @int = GetInt32(contextReader, 0);
				long int2 = GetInt64(contextReader, 1);
				long int3 = GetInt64(contextReader, 2);
				if (!tuIndex.ContainsKey(@int) || tus[tuIndex[@int]].Id != @int)
				{
					continue;
				}
				TuContext tuContext = new TuContext(int2, int3);
				if (contextReader.FieldCount == 10 || contextReader.FieldCount == 11)
				{
					if (cmIsPrecedingFollowing && contextReader.FieldCount != 11)
					{
						throw new Exception("Unexpected number of fields reading context content");
					}
					int num = 0;
					if (sourceCulture == null || targetCulture == null)
					{
						throw new Exception("Cultures must be supplied to retrieve context content");
					}
					if (int2 != 0L && !contextReader.IsDBNull(3))
					{
						num = contextReader.GetInt32(9);
						Segment segment = new Segment(0L, 0L, contextReader.GetString(3), null, contextReader.IsDBNull(4) ? null : contextReader.GetFieldValue<byte[]>(4));
						tuContext.Segment1 = ((num == 0) ? DeserializeXmlSegment(segment.Text) : SegmentSerialization.Load(segment, sourceCulture));
						if (!contextReader.IsDBNull(5))
						{
							byte[] fieldValue = contextReader.GetFieldValue<byte[]>(5);
							tuContext.Segment1.Tokens = TokenSerialization.LoadTokens(fieldValue, tuContext.Segment1);
						}
					}
					if (int3 != 0L && !contextReader.IsDBNull(6))
					{
						if (cmIsPrecedingFollowing)
						{
							num = contextReader.GetInt32(10);
						}
						Segment segment2 = new Segment(0L, 0L, contextReader.GetString(6), null, contextReader.IsDBNull(7) ? null : contextReader.GetFieldValue<byte[]>(7));
						tuContext.Segment2 = ((num == 0) ? DeserializeXmlSegment(segment2.Text) : SegmentSerialization.Load(segment2, cmIsPrecedingFollowing ? sourceCulture : targetCulture));
						if (!contextReader.IsDBNull(8))
						{
							byte[] fieldValue2 = contextReader.GetFieldValue<byte[]>(8);
							tuContext.Segment2.Tokens = TokenSerialization.LoadTokens(fieldValue2, tuContext.Segment2);
						}
					}
				}
				tus[tuIndex[@int]].AddContext(tuContext);
			}
			foreach (TranslationUnit tu in tus)
			{
				if (tu.Contexts == null)
				{
					tu.Contexts = new TuContexts();
				}
			}
		}

		protected void ReadAlignmentData(DbDataReader alignmentDataReader, List<TranslationUnit> tus, Dictionary<int, int> tuIndex)
		{
			while (alignmentDataReader.Read())
			{
				int @int = GetInt32(alignmentDataReader, 0);
				byte[] alignmentData = alignmentDataReader.IsDBNull(1) ? null : alignmentDataReader.GetFieldValue<byte[]>(1);
				DateTime? alignModelDate = alignmentDataReader.IsDBNull(2) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(alignmentDataReader, 2), DateTimeKind.Utc));
				DateTime? insertDate = alignmentDataReader.IsDBNull(3) ? null : new DateTime?(DateTime.SpecifyKind(GetDateTime(alignmentDataReader, 3), DateTimeKind.Utc));
				if (tuIndex.ContainsKey(@int) && tus[tuIndex[@int]].Id == @int)
				{
					tus[tuIndex[@int]].AlignmentData = alignmentData;
					tus[tuIndex[@int]].AlignModelDate = alignModelDate;
					tus[tuIndex[@int]].InsertDate = insertDate;
				}
			}
		}

		protected void ReadIdContexts(DbDataReader contextReader, List<TranslationUnit> tus, Dictionary<int, int> tuIndex)
		{
			while (contextReader.Read())
			{
				int @int = GetInt32(contextReader, 0);
				string @string = contextReader.GetString(1);
				if (tuIndex.ContainsKey(@int) && tus[tuIndex[@int]].Id == @int)
				{
					tus[tuIndex[@int]].AddIdContext(@string);
				}
			}
			foreach (TranslationUnit tu in tus)
			{
				if (tu.IdContexts == null)
				{
					tu.IdContexts = new TuIdContexts();
				}
			}
		}

		protected List<TranslationUnit> GetTus(DbCommand searchCmd, DbCommand getAttributesPerTuCmd, DbCommand getTextContextsPerTuCmd, DbCommand getIdContextsPerTuCmd, int cap)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			using (DbDataReader dbDataReader = searchCmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					TranslationUnit item = ReadTu(dbDataReader);
					list.Add(item);
					if (cap > 0 && list.Count >= cap)
					{
						break;
					}
				}
			}
			if (list.Count <= 0)
			{
				return list;
			}
			if (getAttributesPerTuCmd != null)
			{
				getAttributesPerTuCmd.Prepare();
				foreach (TranslationUnit item2 in list)
				{
					getAttributesPerTuCmd.Parameters[0].Value = item2.Id;
					using (DbDataReader dbDataReader2 = getAttributesPerTuCmd.ExecuteReader())
					{
						do
						{
							GetAttributeValuesForTu(item2, dbDataReader2);
						}
						while (dbDataReader2.NextResult());
					}
				}
			}
			if (getTextContextsPerTuCmd != null)
			{
				getTextContextsPerTuCmd.Prepare();
				foreach (TranslationUnit item3 in list)
				{
					getTextContextsPerTuCmd.Parameters[0].Value = item3.Id;
					item3.Contexts = GetTextContexts(getTextContextsPerTuCmd);
				}
			}
			if (getIdContextsPerTuCmd == null)
			{
				return list;
			}
			getIdContextsPerTuCmd.Prepare();
			foreach (TranslationUnit item4 in list)
			{
				getIdContextsPerTuCmd.Parameters[0].Value = item4.Id;
				item4.IdContexts = GetIdContexts(getIdContextsPerTuCmd);
			}
			return list;
		}

		protected List<PersistentObjectToken> GetTuIds(DbCommand cmd)
		{
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			DbDataReader dbDataReader = null;
			try
			{
				dbDataReader = cmd.ExecuteReader();
				while (dbDataReader.Read())
				{
					int @int = GetInt32(dbDataReader, 0);
					Guid guid = dbDataReader.GetGuid(1);
					list.Add(new PersistentObjectToken(@int, guid));
				}
				return list;
			}
			finally
			{
				if (dbDataReader != null)
				{
					dbDataReader.Close();
					dbDataReader.Dispose();
				}
			}
		}

		protected List<int> GetTuSimpleIds(DbCommand cmd)
		{
			List<int> list = new List<int>();
			DbDataReader dbDataReader = null;
			try
			{
				dbDataReader = cmd.ExecuteReader();
				while (dbDataReader.Read())
				{
					int @int = GetInt32(dbDataReader, 0);
					list.Add(@int);
				}
				return list;
			}
			finally
			{
				if (dbDataReader != null)
				{
					dbDataReader.Close();
					dbDataReader.Dispose();
				}
			}
		}

		protected void GetAttributeValuesForTu(TranslationUnit tu, DbDataReader reader)
		{
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			object obj = null;
			int num = 0;
			string name = null;
			int num2 = 0;
			while (reader.Read())
			{
				int @int = GetInt32(reader, 0);
				string @string = reader.GetString(1);
				int int2 = GetInt32(reader, 2);
				if (@int != num && num != 0)
				{
					switch (num2)
					{
					case 5:
						tu.Attributes.Add(new AttributeValue(num, name, (FieldValueType)num2, list2.ToArray()));
						break;
					case 2:
						tu.Attributes.Add(new AttributeValue(num, name, (FieldValueType)num2, list.ToArray()));
						break;
					default:
						tu.Attributes.Add(new AttributeValue(num, name, (FieldValueType)num2, obj));
						break;
					}
					list2.Clear();
					list.Clear();
				}
				switch (int2)
				{
				case 5:
					list2.Add(GetInt32(reader, 3));
					break;
				case 2:
					list.Add(reader.GetString(3));
					break;
				default:
					obj = reader.GetValue(3);
					if (obj is decimal || obj is long)
					{
						obj = GetInt32(reader, 3);
					}
					break;
				}
				num = @int;
				name = @string;
				num2 = int2;
			}
			if (num != 0)
			{
				switch (num2)
				{
				case 5:
					tu.Attributes.Add(new AttributeValue(num, name, (FieldValueType)num2, list2.ToArray()));
					break;
				case 2:
					tu.Attributes.Add(new AttributeValue(num, name, (FieldValueType)num2, list.ToArray()));
					break;
				default:
					tu.Attributes.Add(new AttributeValue(num, name, (FieldValueType)num2, obj));
					break;
				}
			}
		}

		protected TranslationUnit GetSingleTu(DbCommand getTUsCmd, DbCommand getAttributesPerTuCmd, DbCommand getTextContextsPerTuCmd, DbCommand getIdContextsPerTuCmd)
		{
			List<TranslationUnit> tus = GetTus(getTUsCmd, getAttributesPerTuCmd, getTextContextsPerTuCmd, getIdContextsPerTuCmd, 1);
			if (tus.Count <= 0)
			{
				return null;
			}
			return tus[0];
		}

		protected TuContexts GetTextContexts(DbCommand cmd)
		{
			TuContexts tuContexts = new TuContexts();
			using (DbDataReader dbDataReader = cmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					tuContexts.Add(new TuContext(GetInt64(dbDataReader, 0), GetInt64(dbDataReader, 1)));
				}
				return tuContexts;
			}
		}

		protected TuIdContexts GetIdContexts(DbCommand cmd)
		{
			TuIdContexts tuIdContexts = new TuIdContexts();
			using (DbDataReader dbDataReader = cmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					tuIdContexts.Add(GetString(dbDataReader, 0));
				}
				return tuIdContexts;
			}
		}

		protected string ComputeFeatureString(List<int> features, bool withParens, int maxLength)
		{
			if (features == null || features.Count == 0)
			{
				throw new ArgumentNullException("features", "Features not initialized.");
			}
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder(withParens ? "(" : string.Empty);
			foreach (int feature in features)
			{
				string text = feature.ToString(CultureInfo.InvariantCulture);
				if (maxLength > 0 && stringBuilder.Length + text.Length + 2 >= maxLength)
				{
					break;
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(text);
			}
			if (withParens)
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		protected List<TranslationUnit> GetTUsOnly(DbCommand retrievalCmd)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			using (DbDataReader dbDataReader = retrievalCmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					TranslationUnit item = ReadTu(dbDataReader);
					list.Add(item);
				}
				return list;
			}
		}

		protected List<TranslationUnit> GetExactBatchMatches(DbCommand retrievalCmd)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			using (DbDataReader dbDataReader = retrievalCmd.ExecuteReader())
			{
				while (dbDataReader.HasRows && dbDataReader.FieldCount == 1)
				{
					dbDataReader.NextResult();
				}
				if (!dbDataReader.HasRows)
				{
					return list;
				}
				while (dbDataReader.Read())
				{
					TranslationUnit item = ReadTu(dbDataReader);
					list.Add(item);
				}
				return list;
			}
		}

		protected List<TranslationUnit> GetDuplicateBatchMatches(DbCommand retrievalCmd)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			using (DbDataReader dbDataReader = retrievalCmd.ExecuteReader())
			{
				if (!dbDataReader.HasRows)
				{
					return list;
				}
				while (dbDataReader.Read())
				{
					TranslationUnit item = (!HasFlags()) ? new TranslationUnit(GetInt32(dbDataReader, 0), default(Guid), 0, new Segment(GetInt64(dbDataReader, 1), 0L, dbDataReader.GetString(3), null, dbDataReader.IsDBNull(8) ? null : dbDataReader.GetFieldValue<byte[]>(8)), new Segment(GetInt64(dbDataReader, 2), 0L, dbDataReader.GetString(4), null, dbDataReader.IsDBNull(9) ? null : dbDataReader.GetFieldValue<byte[]>(9)), DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", 0, -1, dbDataReader.IsDBNull(5) ? null : dbDataReader.GetFieldValue<byte[]>(5), dbDataReader.IsDBNull(6) ? null : dbDataReader.GetFieldValue<byte[]>(6), null, null, null, dbDataReader.GetInt32(7), (TranslationUnitFormat)dbDataReader.GetByte(10), (TranslationUnitOrigin)dbDataReader.GetByte(11), (ConfirmationLevel)dbDataReader.GetByte(12)) : new TranslationUnit(GetInt32(dbDataReader, 0), default(Guid), 0, new Segment(GetInt64(dbDataReader, 1), 0L, dbDataReader.GetString(3), null, dbDataReader.IsDBNull(9) ? null : dbDataReader.GetFieldValue<byte[]>(9)), new Segment(GetInt64(dbDataReader, 2), 0L, dbDataReader.GetString(4), null, dbDataReader.IsDBNull(10) ? null : dbDataReader.GetFieldValue<byte[]>(10)), DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", DateTime.SpecifyKind(default(DateTime), DateTimeKind.Utc), "", 0, (!dbDataReader.IsDBNull(7)) ? dbDataReader.GetInt32(7) : 0, dbDataReader.IsDBNull(5) ? null : dbDataReader.GetFieldValue<byte[]>(5), dbDataReader.IsDBNull(6) ? null : dbDataReader.GetFieldValue<byte[]>(6), null, null, null, dbDataReader.GetInt32(8));
					list.Add(item);
				}
				return list;
			}
		}

		protected Dictionary<int, List<TranslationUnit>> GetFuzzyBatchMatches(DbCommand retrievalCmd)
		{
			Dictionary<int, List<TranslationUnit>> dictionary = new Dictionary<int, List<TranslationUnit>>();
			using (DbDataReader dbDataReader = retrievalCmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					TranslationUnit item = ReadTu(dbDataReader);
					int @int = GetInt32(dbDataReader, 12);
					if (dictionary.ContainsKey(@int))
					{
						dictionary[@int].Add(item);
					}
					else
					{
						List<TranslationUnit> list = new List<TranslationUnit>();
						list.Add(item);
						dictionary.Add(@int, list);
					}
				}
				return dictionary;
			}
		}

		protected List<TranslationUnit> GetTUsWithAttributesAndContexts(DbCommand retrievalCmd, bool returnIdContext, bool returnContext = true, Dictionary<int, AttributeDeclaration> attributes = null, CultureInfo sourceCulture = null, CultureInfo targetCulture = null, bool cmIsPrecedingFollowing = false)
		{
			using (DbDataReader reader = retrievalCmd.ExecuteReader())
			{
				return GetTUsWithAttributesAndContexts(reader, returnIdContext, returnContext, attributes, sourceCulture, targetCulture, cmIsPrecedingFollowing);
			}
		}

		protected List<TranslationUnit> GetTUsWithAttributesAndContexts(DbDataReader reader, bool returnIdContext, bool returnContext = true, Dictionary<int, AttributeDeclaration> attributes = null, CultureInfo sourceCulture = null, CultureInfo targetCulture = null, bool cmIsPrecedingFollowing = false)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			while (reader.Read())
			{
				TranslationUnit translationUnit = ReadTu(reader);
				dictionary.Add(translationUnit.Id, list.Count);
				list.Add(translationUnit);
			}
			if (list.Count <= 0)
			{
				return list;
			}
			if (!reader.NextResult())
			{
				throw new Exception("Next result set expected");
			}
			ReadAttributeValues(reader, list, dictionary, autoAdvanceResultSet: false, attributes);
			if (!reader.NextResult())
			{
				throw new Exception("Next result set expected");
			}
			ReadAttributeValues(reader, list, dictionary, autoAdvanceResultSet: false, attributes);
			if (!reader.NextResult())
			{
				throw new Exception("Next result set expected");
			}
			ReadAttributeValues(reader, list, dictionary, autoAdvanceResultSet: false, attributes);
			if (!reader.NextResult())
			{
				throw new Exception("Next result set expected");
			}
			ReadAttributeValues(reader, list, dictionary, autoAdvanceResultSet: false, attributes);
			if (returnContext)
			{
				if (!reader.NextResult())
				{
					throw new Exception("Next result set expected");
				}
				ReadTextContexts(reader, list, dictionary, sourceCulture, targetCulture, cmIsPrecedingFollowing);
			}
			if (returnIdContext)
			{
				if (!reader.NextResult())
				{
					throw new Exception("Next result set expected");
				}
				ReadIdContexts(reader, list, dictionary);
			}
			if (!reader.NextResult())
			{
				throw new Exception("Next result set expected");
			}
			ReadAlignmentData(reader, list, dictionary);
			return list;
		}

		protected List<(int, DateTime)> GetAlignmentTimestamps(DbCommand retrievalCmd)
		{
			List<(int, DateTime)> list = new List<(int, DateTime)>();
			using (DbDataReader dbDataReader = retrievalCmd.ExecuteReader())
			{
				while (dbDataReader.Read())
				{
					int @int = GetInt32(dbDataReader, 0);
					DateTime dateTime = GetDateTime(dbDataReader, 1);
					list.Add((@int, dateTime));
				}
				return list;
			}
		}

		protected bool GetReindexRequired(DbCommand cmd)
		{
			using (DbDataReader dbDataReader = cmd.ExecuteReader())
			{
				return dbDataReader.Read();
			}
		}

		protected int GetTuCountForReindex(DbCommand cmd)
		{
			object obj = cmd.ExecuteScalar();
			if (obj != null)
			{
				return Convert.ToInt32(obj);
			}
			return -1;
		}

		public void DeserializeTuSegments(TranslationUnit storageTu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (storageTu.SerializationVersion == 0)
			{
				tu.SourceSegment = XmlSegmentSerializer.DeserializeSegment(storageTu.Source.Text);
				tu.TargetSegment = XmlSegmentSerializer.DeserializeSegment(storageTu.Target.Text);
			}
			else
			{
				tu.SourceSegment = SegmentSerialization.Load(storageTu.Source, sourceCulture);
				tu.TargetSegment = SegmentSerialization.Load(storageTu.Target, targetCulture);
			}
		}

		public virtual void SerializeTuSegments(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, TranslationUnit storageTu)
		{
			storageTu.Source.Text = XmlSegmentSerializer.SerializeSegment(tu.SourceSegment);
			storageTu.Target.Text = XmlSegmentSerializer.SerializeSegment(tu.TargetSegment);
		}

		private Sdl.LanguagePlatform.Core.Segment DeserializeXmlSegment(string xml)
		{
			return XmlSegmentSerializer.DeserializeSegment(xml);
		}
	}
}
