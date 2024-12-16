using Sdl.Core.FineGrainedAlignment;
using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using Sdl.LanguagePlatform.TranslationMemoryImpl.FGA;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class ResourceManager
	{
		private enum PersistentObjectType
		{
			TM,
			TU,
			Resource,
			Attribute,
			PicklistValue
		}

		private class TMDupHashInfo
		{
			public List<int[]> FileBasedTmTuIdsWithDupSourceHashes;

			public List<int[]> FileBasedTmTuIdsWithDupTargetHashes;
		}

		private readonly CallContext _Context;

		private static readonly Dictionary<Guid, TMDupHashInfo> TMDupHashInfoMap = new Dictionary<Guid, TMDupHashInfo>();

		private static readonly object TMDupHashInfoMapLocker = new object();

		private static readonly Dictionary<Guid, bool> _ReindexRequiredCache = new Dictionary<Guid, bool>();

		public ResourceManager(CallContext context)
		{
			_Context = context;
		}

		public void CreateSchema()
		{
			_Context.Storage.CreateSchema();
		}

		public void DropSchema()
		{
			_Context.Storage.DropSchema();
		}

		public void CleanSchema()
		{
			_Context.Storage.CleanupSchema();
		}

		public bool SchemaExists()
		{
			return _Context.Storage.SchemaExists();
		}

		public void RecomputeStatistics(int tmId)
		{
			_Context.Storage.RecomputeFrequencies(tmId);
		}

		public FuzzyIndexTuningSettings GetFuzzyIndexTuningSettings(int tmId)
		{
			FuzzyIndexTuningSettings fuzzyIndexTuningSettings = new FuzzyIndexTuningSettings();
			fuzzyIndexTuningSettings.MinScoreIncrease = GetIntParameter(tmId, "ADDTOMINSCORE");
			fuzzyIndexTuningSettings.MinSearchVectorLengthSourceWordIndex = GetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
			{
				1
			}));
			fuzzyIndexTuningSettings.MinSearchVectorLengthSourceCharIndex = GetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
			{
				2
			}));
			fuzzyIndexTuningSettings.MinSearchVectorLengthTargetCharIndex = GetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
			{
				4
			}));
			fuzzyIndexTuningSettings.MinSearchVectorLengthTargetWordIndex = GetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
			{
				8
			}));
			return fuzzyIndexTuningSettings;
		}

		private int GetIntParameter(int tmId, string key)
		{
			string parameter = _Context.Storage.GetParameter(tmId, key);
			if (string.IsNullOrEmpty(key) || !int.TryParse(parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
			{
				return 0;
			}
			return result;
		}

		private void SetIntParameter(int tmId, string key, int v)
		{
			_Context.Storage.SetParameter(tmId, key, v.ToString(CultureInfo.InvariantCulture));
		}

		private void SetIntParameter(int tmId, string key, int v, int min, int max)
		{
			if (v < min)
			{
				SetIntParameter(tmId, key, min);
			}
			else if (v > max)
			{
				SetIntParameter(tmId, key, max);
			}
			else
			{
				SetIntParameter(tmId, key, v);
			}
		}

		public void SetFuzzyIndexTuningSettings(int tmId, FuzzyIndexTuningSettings settings)
		{
			if (settings != null)
			{
				SetIntParameter(tmId, "ADDTOMINSCORE", settings.MinScoreIncrease, 0, 100);
				SetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
				{
					1
				}), settings.MinSearchVectorLengthSourceWordIndex, 3, 50);
				SetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
				{
					2
				}), settings.MinSearchVectorLengthSourceCharIndex, 5, 50);
				SetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
				{
					4
				}), settings.MinSearchVectorLengthTargetCharIndex, 5, 50);
				SetIntParameter(tmId, string.Format(CultureInfo.InvariantCulture, "MINHAVING{0}", new object[1]
				{
					8
				}), settings.MinSearchVectorLengthTargetWordIndex, 3, 50);
			}
		}

		private static bool IsSystemField(string attributeName)
		{
			return Field.IsReservedName(attributeName);
		}

		private void AddDefaultFields(PersistentObjectToken tmId)
		{
			Field field = new Field(Field.StructureContextFieldName, FieldValueType.MultipleString);
			AddField(tmId, field);
		}

		public FieldDefinitions GetFields(PersistentObjectToken tmId)
		{
			List<AttributeDeclaration> attributes = _Context.Storage.GetAttributes(tmId.Id);
			FieldDefinitions fieldDefinitions = new FieldDefinitions();
			foreach (AttributeDeclaration item in attributes)
			{
				Field field;
				if (item.Type == FieldValueType.SinglePicklist || item.Type == FieldValueType.MultiplePicklist)
				{
					PicklistField picklistField = new PicklistField(item.Name, item.Type);
					foreach (PickValue item2 in item.Picklist)
					{
						PicklistItem pli = new PicklistItem(item2.Value)
						{
							ID = item2.Id
						};
						try
						{
							picklistField.Picklist.Add(pli, ignoreDups: true);
						}
						catch (LanguagePlatformException)
						{
						}
					}
					field = picklistField;
				}
				else
				{
					field = new Field(item.Name, item.Type);
				}
				field.ResourceId = new PersistentObjectToken(item.Id, item.Guid);
				fieldDefinitions.Add(field);
			}
			return fieldDefinitions;
		}

		public PersistentObjectToken AddField(PersistentObjectToken tmId, Field field)
		{
			if (!Field.IsValidName(field.Name))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName, field.Name);
			}
			if (field.Name.Length > TranslationMemorySetup.MaximumFieldNameLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededFieldNameLimit);
			}
			AttributeDeclaration attributeDeclaration = new AttributeDeclaration(field.Name, Guid.NewGuid(), field.ValueType, tmId.Id);
			if (field.ValueType != FieldValueType.SinglePicklist && field.ValueType != FieldValueType.MultiplePicklist)
			{
				if (!_Context.Storage.AddAttribute(attributeDeclaration))
				{
					return null;
				}
				return new PersistentObjectToken(attributeDeclaration.Id, attributeDeclaration.Guid);
			}
			foreach (PicklistItem item in ((field as PicklistField) ?? throw new LanguagePlatformException(ErrorCode.DAFieldTypesInconsistent)).Picklist)
			{
				if (!Field.IsValidName(item.Name))
				{
					throw new LanguagePlatformException(ErrorCode.TMInvalidPicklistValueName, item.Name);
				}
				if (item.Name.Length > TranslationMemorySetup.MaximumFieldNameLength)
				{
					throw new LanguagePlatformException(ErrorCode.TMExceededPicklistValueNameLimit);
				}
				attributeDeclaration.AddPickListValue(new PickValue(item.Name));
			}
			if (!_Context.Storage.AddAttribute(attributeDeclaration))
			{
				return null;
			}
			return new PersistentObjectToken(attributeDeclaration.Id, attributeDeclaration.Guid);
		}

		public PersistentObjectToken[] AddFields(PersistentObjectToken tmId, FieldDefinitions fields)
		{
			List<PersistentObjectToken> list = fields.Select((Field field) => AddField(tmId, field)).ToList();
			if (list.Count <= 0)
			{
				return null;
			}
			return list.ToArray();
		}

		public bool RemoveField(PersistentObjectToken tmId, string fieldName)
		{
			if (!IsSystemField(fieldName))
			{
				return _Context.Storage.DeleteAttribute(tmId.Id, fieldName);
			}
			return false;
		}

		public bool RenameField(PersistentObjectToken tmId, PersistentObjectToken fieldId, string newName)
		{
			if (!Field.IsValidName(newName))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName, newName);
			}
			if (newName.Length > TranslationMemorySetup.MaximumFieldNameLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededFieldNameLimit);
			}
			AttributeDeclaration attribute = _Context.Storage.GetAttribute(tmId.Id, fieldId.Id);
			if (attribute != null && attribute.TMId == tmId.Id && !IsSystemField(attribute.Name) && !IsSystemField(newName))
			{
				return _Context.Storage.RenameAttribute(tmId.Id, attribute.Id, newName);
			}
			return false;
		}

		public bool RemoveField(PersistentObjectToken tmId, PersistentObjectToken fieldId)
		{
			AttributeDeclaration attribute = _Context.Storage.GetAttribute(tmId.Id, fieldId.Id);
			if (attribute != null && attribute.TMId == tmId.Id && !IsSystemField(attribute.Name))
			{
				return _Context.Storage.DeleteAttribute(tmId.Id, fieldId.Id);
			}
			return false;
		}

		public PersistentObjectToken AddPicklistValue(PersistentObjectToken tmId, PersistentObjectToken fieldId, string value)
		{
			if (!Field.IsValidName(value))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidPicklistValueName);
			}
			if (value.Length > TranslationMemorySetup.MaximumFieldNameLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededPicklistValueNameLimit);
			}
			AttributeDeclaration attribute = _Context.Storage.GetAttribute(tmId.Id, fieldId.Id);
			if (attribute == null || attribute.TMId != tmId.Id || IsSystemField(attribute.Name) || !attribute.IsPicklistField)
			{
				return null;
			}
			PickValue pickValue = new PickValue(value);
			return new PersistentObjectToken(_Context.Storage.AddPicklistValue(tmId.Id, attribute.Id, pickValue), pickValue.Guid);
		}

		public PersistentObjectToken[] AddPicklistValues(PersistentObjectToken tmId, PersistentObjectToken fieldId, string[] values)
		{
			AttributeDeclaration attribute = _Context.Storage.GetAttribute(tmId.Id, fieldId.Id);
			if (attribute == null || attribute.TMId != tmId.Id || IsSystemField(attribute.Name) || !attribute.IsPicklistField)
			{
				return null;
			}
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			foreach (string obj in values)
			{
				if (!Field.IsValidName(obj))
				{
					throw new LanguagePlatformException(ErrorCode.TMInvalidPicklistValueName);
				}
				if (obj.Length > TranslationMemorySetup.MaximumFieldNameLength)
				{
					throw new LanguagePlatformException(ErrorCode.TMExceededPicklistValueNameLimit);
				}
				PickValue pickValue = new PickValue(obj);
				int num = _Context.Storage.AddPicklistValue(tmId.Id, attribute.Id, pickValue);
				list.Add((num >= 0) ? new PersistentObjectToken(num, pickValue.Guid) : null);
			}
			if (list.Count != 0)
			{
				return list.ToArray();
			}
			return null;
		}

		public bool RemovePicklistValue(PersistentObjectToken tmId, PersistentObjectToken fieldId, string value)
		{
			AttributeDeclaration attribute = _Context.Storage.GetAttribute(tmId.Id, fieldId.Id);
			if (attribute != null && attribute.TMId == tmId.Id && !IsSystemField(attribute.Name) && attribute.IsPicklistField)
			{
				return _Context.Storage.DeletePicklistValue(tmId.Id, attribute.Id, value);
			}
			return false;
		}

		public bool RenamePicklistValue(PersistentObjectToken tmId, PersistentObjectToken fieldId, string previousName, string newName)
		{
			if (!Field.IsValidName(newName))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidPicklistValueName);
			}
			if (newName.Length > TranslationMemorySetup.MaximumFieldNameLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededPicklistValueNameLimit);
			}
			AttributeDeclaration attribute = _Context.Storage.GetAttribute(tmId.Id, fieldId.Id);
			if (attribute == null || attribute.TMId != tmId.Id || IsSystemField(attribute.Name) || !attribute.IsPicklistField)
			{
				return false;
			}
			if (attribute.FindPicklistValueId(newName) < 0)
			{
				return _Context.Storage.RenamePicklistValue(tmId.Id, attribute.Id, previousName, newName);
			}
			return false;
		}

		public bool RemovePicklistValues(PersistentObjectToken tmId, PersistentObjectToken fieldId, string[] values)
		{
			AttributeDeclaration decl = _Context.Storage.GetAttribute(tmId.Id, fieldId.Id);
			if (decl == null || decl.TMId != tmId.Id || IsSystemField(decl.Name) || !decl.IsPicklistField)
			{
				return false;
			}
			return values.Aggregate(seed: true, (bool current, string value) => current & _Context.Storage.DeletePicklistValue(tmId.Id, decl.Id, value));
		}

		public bool CreateLanguageResource(LanguageResource resource)
		{
			Resource resource2 = new Resource(resource);
			resource2.Guid = Guid.NewGuid();
			bool result = _Context.Storage.AddResource(resource2);
			resource.ResourceId = new PersistentObjectToken(resource2.Id, resource2.Guid);
			return result;
		}

		public bool DeleteLanguageResource(LanguageResource resource)
		{
			return DeleteLanguageResource(resource.ResourceId);
		}

		private static bool VerifyGuid(PersistentObjectToken resourceId, int tmId, PersistentObjectType t)
		{
			if (resourceId == null)
			{
				return false;
			}
			return !object.Equals(resourceId.Guid, Guid.Empty);
		}

		public bool DeleteLanguageResource(PersistentObjectToken resourceId)
		{
			if (!VerifyGuid(resourceId, 0, PersistentObjectType.Resource))
			{
				return false;
			}
			bool result = _Context.Storage.DeleteResource(resourceId.Id);
			resourceId.Id = 0;
			resourceId.Guid = Guid.Empty;
			return result;
		}

		public bool UpdateLanguageResource(LanguageResource resource)
		{
			if (!VerifyGuid(resource.ResourceId, 0, PersistentObjectType.Resource))
			{
				return false;
			}
			Resource resource2 = new Resource(resource);
			return _Context.Storage.UpdateResource(resource2);
		}

		public List<LanguageResource> GetLanguageResources(bool includeData)
		{
			return (from sr in _Context.Storage.GetResources(includeData)
				select sr.ToLanguageResource()).ToList();
		}

		public List<LanguageResource> GetLanguageResources(PersistentObjectToken tmId, bool includeData)
		{
			if (VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return (from sr in _Context.Storage.GetResources(tmId.Id, includeData)
					select sr.ToLanguageResource()).ToList();
			}
			return null;
		}

		internal int GetResourcesWriteCount()
		{
			return _Context.Storage.GetResourcesWriteCount();
		}

		public LanguageResource GetLanguageResource(PersistentObjectToken resourceId, bool includeData)
		{
			if (!VerifyGuid(resourceId, 0, PersistentObjectType.Resource))
			{
				return null;
			}
			return _Context.Storage.GetResource(resourceId.Id, includeData)?.ToLanguageResource();
		}

		public List<PersistentObjectToken> GetResourceTMs(PersistentObjectToken resourceId)
		{
			if (!VerifyGuid(resourceId, 0, PersistentObjectType.Resource))
			{
				return null;
			}
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory> tMsByResourceId = _Context.Storage.GetTMsByResourceId(resourceId.Id);
			if (tMsByResourceId == null)
			{
				return list;
			}
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory item in tMsByResourceId)
			{
				list.Add(new PersistentObjectToken(item.Id, item.Guid));
			}
			return list;
		}

		public bool AttachTMResource(PersistentObjectToken tmId, PersistentObjectToken resourceId)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return false;
			}
			if (VerifyGuid(resourceId, tmId.Id, PersistentObjectType.Resource))
			{
				return _Context.Storage.AttachTmResource(tmId.Id, resourceId.Id);
			}
			return false;
		}

		public bool DetachTmResource(PersistentObjectToken tmId, PersistentObjectToken resourceId)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return false;
			}
			if (VerifyGuid(resourceId, tmId.Id, PersistentObjectType.Resource))
			{
				return _Context.Storage.DetachTmResource(tmId.Id, resourceId.Id);
			}
			return false;
		}

		public bool CreateTranslationMemory(TranslationMemorySetup setup)
		{
			return CreateTranslationMemory(setup, null);
		}

		public bool CreateTranslationMemory(TranslationMemorySetup setup, List<AlignableCorpusId> modelCorpusIdList)
		{
			if (setup == null)
			{
				throw new ArgumentNullException("setup");
			}
			if (setup.LanguageDirection == null)
			{
				throw new ArgumentNullException("LanguageDirection");
			}
			if (setup.LanguageDirection.SourceCulture == null)
			{
				throw new ArgumentNullException("SourceCulture");
			}
			if (setup.LanguageDirection.TargetCulture == null)
			{
				throw new ArgumentNullException("TargetCulture");
			}
			if (setup.LanguageDirection.SourceCulture.IsNeutralCulture)
			{
				throw new LanguagePlatformException(ErrorCode.InvalidTMSourceLanguage);
			}
			if (setup.LanguageDirection.TargetCulture.IsNeutralCulture)
			{
				throw new LanguagePlatformException(ErrorCode.InvalidTMTargetLanguage);
			}
			if (string.IsNullOrEmpty(setup.Name) || !Field.IsValidName(setup.Name))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidTMName);
			}
			if (setup.Name.Length > TranslationMemorySetup.MaximumTranslationMemoryNameLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededTMNameLimit);
			}
			if (!string.IsNullOrEmpty(setup.Copyright) && setup.Copyright.Length > TranslationMemorySetup.MaximumCopyrightFieldLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededCopyrightFieldLimit);
			}
			if (!string.IsNullOrEmpty(setup.Description) && setup.Description.Length > TranslationMemorySetup.MaximumDescriptionFieldLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededDescriptionFieldLimit);
			}
			setup.CreationDate = DateTimeUtilities.Normalize(DateTime.Now);
			setup.CreationUser = _Context.UserName;
			if (setup.ExactSearchOnly)
			{
				setup.FuzzyIndexes = (FuzzyIndexes)0;
			}
			else
			{
				setup.FuzzyIndexes |= FuzzyIndexes.SourceWordBased;
				setup.FuzzyIndexes |= FuzzyIndexes.TargetWordBased;
			}
			if (setup.EnableFullTextSearch)
			{
				throw new NotImplementedException("Full text search is not yet available.");
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory translationMemory = new Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory(setup)
			{
				Guid = Guid.NewGuid()
			};
			bool result = _Context.Storage.AddTm(translationMemory);
			setup.ResourceId = new PersistentObjectToken(translationMemory.Id, translationMemory.Guid);
			if (setup.FGASupport != FGASupport.Off && setup.FGASupport != 0 && modelCorpusIdList != null && _Context.IsFilebasedTm && SqliteStorage.UseFileBasedFga())
			{
				modelCorpusIdList.Add(new StorageBasedAlignableCorpusId(setup.ResourceId));
			}
			AddDefaultFields(setup.ResourceId);
			if (setup.FieldDeclarations != null)
			{
				AddFields(setup.ResourceId, setup.FieldDeclarations);
			}
			return result;
		}

		public bool RecoverJAZHCMInfo(PersistentObjectToken tmId, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken)
		{
			return RecoverJAZHCMInfo(tmId, progress, cancellationToken, null);
		}

		public bool RecoverJAZHCMInfo(PersistentObjectToken tmId, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken, bool? skipConversionChecks)
		{
			List<LanguageResource> languageResources = GetLanguageResources(tmId, includeData: true);
			AnnotatedTranslationMemory annotatedTranslationMemory = _Context.GetAnnotatedTranslationMemory(tmId);
			return new LegacyCMConverter().ConvertCMInfo(annotatedTranslationMemory, languageResources, this, _Context, progress, cancellationToken, skipConversionChecks);
		}

		public bool ChangeTranslationMemory(TranslationMemorySetup setup)
		{
			bool deletedTranslationModel;
			return ChangeTranslationMemory(setup, null, out deletedTranslationModel);
		}

		public bool ChangeTranslationMemory(TranslationMemorySetup setup, List<AlignableCorpusId> modelCorpusIdList, out bool deletedTranslationModel)
		{
			return ChangeTranslationMemory(setup, modelCorpusIdList, out deletedTranslationModel, null, CancellationToken.None);
		}

		public bool ChangeTranslationMemory(TranslationMemorySetup setup, List<AlignableCorpusId> modelCorpusIdList, out bool deletedTranslationModel, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken)
		{
			deletedTranslationModel = false;
			if (setup == null)
			{
				throw new ArgumentNullException("setup");
			}
			if (setup.ResourceId == null)
			{
				throw new ArgumentNullException("ResourceId");
			}
			if (!VerifyGuid(setup.ResourceId, 0, PersistentObjectType.TM))
			{
				return false;
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(setup.ResourceId.Id);
			if (tm == null)
			{
				throw new LanguagePlatformException(ErrorCode.TMNotFound);
			}
			if (tm.ExactSearchOnly != setup.ExactSearchOnly)
			{
				throw new LanguagePlatformException(ErrorCode.TMCannotModifyExactSearchOnlyFlag);
			}
			if (tm.EnableFullTextSearch != setup.EnableFullTextSearch)
			{
				throw new LanguagePlatformException(ErrorCode.TMCannotModifyFullTextSearchFlag);
			}
			if (!string.IsNullOrEmpty(setup.Copyright) && setup.Copyright.Length > TranslationMemorySetup.MaximumCopyrightFieldLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededCopyrightFieldLimit);
			}
			if (!string.IsNullOrEmpty(setup.Description) && setup.Description.Length > TranslationMemorySetup.MaximumDescriptionFieldLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededDescriptionFieldLimit);
			}
			tm.Copyright = setup.Copyright;
			tm.Description = setup.Description;
			tm.ExpirationDate = setup.ExpirationDate;
			if (string.IsNullOrEmpty(setup.Name) || !Field.IsValidName(setup.Name))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidTMName);
			}
			if (setup.Name.Length > TranslationMemorySetup.MaximumTranslationMemoryNameLength)
			{
				throw new LanguagePlatformException(ErrorCode.TMExceededTMNameLimit);
			}
			tm.Name = setup.Name;
			tm.Recognizers = setup.Recognizers;
			tm.WordCountFlags = setup.WordCountFlags;
			tm.TokenizerFlags = setup.TokenizerFlags;
			if (tm.FuzzyIndexes != setup.FuzzyIndexes)
			{
				throw new LanguagePlatformException(ErrorCode.TMCannotModifyFuzzyIndices);
			}
			bool flag = false;
			if (setup.FGASupport != tm.FGASupport)
			{
				if (setup.FGASupport == FGASupport.Legacy)
				{
					throw new Exception("FGA support cannot be reverted to legacy status");
				}
				if (tm.FGASupport == FGASupport.Legacy)
				{
					flag = true;
				}
			}
			bool flag2 = false;
			if (setup.CanReportReindexRequired && !tm.CanReportReindexRequired && _Context.IsFilebasedTm)
			{
				flag = true;
			}
			if (!setup.UsesLegacyHashes && tm.DataVersion == 0 && _Context.IsFilebasedTm)
			{
				flag = true;
				SqliteStorage sqliteStorage = _Context.Storage as SqliteStorage;
				if (sqliteStorage != null && tm.CanReportReindexRequired && !sqliteStorage.CanChooseTextContextMatchType)
				{
					flag2 = true;
				}
			}
			if (setup.UsesLegacyHashes && tm.DataVersion == 1)
			{
				throw new Exception("A strict-hashing TM cannot be downgraded to use legacy hashes");
			}
			if (!setup.UsesLegacyHashes && tm.DataVersion == 0 && !_Context.IsFilebasedTm)
			{
				tm.DataVersion = -1;
			}
			if (flag)
			{
				SqliteStorage sqliteStorage2 = _Context.Storage as SqliteStorage;
				if (sqliteStorage2 == null)
				{
					throw new Exception("FGA support cannot be enabled for a legacy TM without conversion");
				}
				sqliteStorage2.InPlaceUpgrade();
				if (tm.FGASupport == FGASupport.Legacy)
				{
					tm.FGASupport = FGASupport.Off;
				}
			}
			bool flag3 = false;
			if (setup.FGASupport != tm.FGASupport && (setup.FGASupport == FGASupport.Off || tm.FGASupport == FGASupport.Off))
			{
				if (setup.FGASupport == FGASupport.Off)
				{
					_Context.AlignableStorage.ClearAlignmentData(setup.ResourceId.Id);
					_Context.AlignableStorage.SetAlignerDefinition(setup.ResourceId.Id, null);
					if (_Context.IsFilebasedTm)
					{
						deletedTranslationModel = true;
					}
				}
				else
				{
					if (!_Context.IsFilebasedTm)
					{
						throw new Exception("For server-based TM, enable FGA by setting the aligner definition");
					}
					if (SqliteStorage.UseFileBasedFga() && (setup.FGASupport == FGASupport.Automatic || setup.FGASupport == FGASupport.NonAutomatic))
					{
						flag3 = true;
					}
				}
			}
			tm.FGASupport = setup.FGASupport;
			bool num = _Context.Storage.UpdateTm(tm);
			if (flag2)
			{
				List<LanguageResource> languageResources = GetLanguageResources(setup.ResourceId, includeData: true);
				AnnotatedTranslationMemory annotatedTranslationMemory = _Context.GetAnnotatedTranslationMemory(setup.ResourceId);
				new LegacyCMConverter().ConvertCMInfo(annotatedTranslationMemory, languageResources, this, _Context, progress, cancellationToken, null);
			}
			if (num & flag3)
			{
				modelCorpusIdList.Add(new StorageBasedAlignableCorpusId(setup.ResourceId));
			}
			InvalidateReindexRequiredCache(setup.ResourceId.Guid);
			return num;
		}

		public bool DeleteTranslationMemory(PersistentObjectToken tmId)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return false;
			}
			if (_Context.Storage.DeleteTmRequiresEmptyTm)
			{
				DeleteAllTranslationUnits(tmId);
			}
			bool result = _Context.Storage.DeleteTm(tmId.Id);
			tmId.Id = 0;
			tmId.Guid = Guid.Empty;
			return result;
		}

		public bool DeleteTranslationMemorySchema(PersistentObjectToken tmId)
		{
			bool result = _Context.Storage.DeleteTmSchema(tmId.Id);
			tmId.Id = 0;
			tmId.Guid = Guid.Empty;
			return result;
		}

		public TranslationMemorySetup GetTranslationMemory(PersistentObjectToken tmId)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			if (tm != null)
			{
				return ObjectFactory.CreateTranslationMemory(tm, _Context.Storage.IsReadOnly, this);
			}
			return null;
		}

		public TranslationMemorySetup[] GetTranslationMemories(bool checkPermissions)
		{
			List<TranslationMemorySetup> list = new List<TranslationMemorySetup>();
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory> tms = _Context.Storage.GetTms();
			bool isReadOnly = _Context.Storage.IsReadOnly;
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory item2 in tms)
			{
				if (!checkPermissions || (_Context.GetPermissions(new PersistentObjectToken(item2.Id, item2.Guid)) & Permission.ReadTM) != 0)
				{
					TranslationMemorySetup item = ObjectFactory.CreateTranslationMemory(item2, isReadOnly, this);
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public TranslationMemorySetup GetTranslationMemory(string name)
		{
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(name);
			if (tm != null)
			{
				return ObjectFactory.CreateTranslationMemory(tm, _Context.Storage.IsReadOnly, this);
			}
			return null;
		}

		public PersistentObjectToken GetTranslationMemoryId(string name)
		{
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(name);
			if (tm == null)
			{
				return null;
			}
			return new PersistentObjectToken(tm.Id, tm.Guid);
		}

		public int GetTuCount(PersistentObjectToken tmId)
		{
			if (VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return _Context.Storage.GetTuCount(tmId.Id);
			}
			return 0;
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit GetTranslationUnit(PersistentObjectToken tmId, PersistentObjectToken tuId)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			if (!VerifyGuid(tuId, tmId.Id, PersistentObjectType.TU))
			{
				return null;
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu = _Context.Storage.GetTu(tmId.Id, tuId.Id, tm.IdContextMatch);
			if (tu != null)
			{
				return GetTranslationUnit(tu, null, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture);
			}
			return null;
		}

		public bool DeleteTranslationUnit(PersistentObjectToken tmId, PersistentObjectToken tuId)
		{
			return DeleteTranslationUnitInternal(tmId, tuId, skipOrphanContextDeletion: false);
		}

		private bool DeleteTranslationUnitInternal(PersistentObjectToken tmId, PersistentObjectToken tuId, bool skipOrphanContextDeletion)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return false;
			}
			if (!VerifyGuid(tuId, tmId.Id, PersistentObjectType.TU))
			{
				return false;
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			bool deleteOrphanContexts = tm.DataVersion > 0 && !skipOrphanContextDeletion;
			return _Context.Storage.DeleteTu(tmId.Id, tuId, tm.TextContextMatchType, deleteOrphanContexts);
		}

		public List<PersistentObjectToken> DeleteTranslationUnits(PersistentObjectToken tmId, PersistentObjectToken[] ids)
		{
			return DeleteTranslationUnitsInternal(tmId, (IReadOnlyCollection<PersistentObjectToken>)(object)ids, skipOrphanContextDeletion: false);
		}

		private List<PersistentObjectToken> DeleteTranslationUnitsInternal(PersistentObjectToken tmId, IReadOnlyCollection<PersistentObjectToken> ids, bool skipOrphanContextDeletion)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return new List<PersistentObjectToken>();
			}
			if (ids == null || ids.Count <= 0)
			{
				return new List<PersistentObjectToken>();
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			bool deleteOrphanContexts = tm.DataVersion > 0 && !skipOrphanContextDeletion;
			return _Context.Storage.DeleteTus(tmId.Id, ids.ToList(), tm.TextContextMatchType, deleteOrphanContexts);
		}

		public List<PersistentObjectToken> DeleteTranslationUnits(PersistentObjectToken tmId, ref RegularIterator iterator)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return new List<PersistentObjectToken>();
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			List<PersistentObjectToken> list = TranslationUnitFilteringStrategyFactory.GetBestStrategyForFilter(iterator.Filter, _Context, tm.DataVersion).DeleteTusFiltered(tmId, iterator);
			iterator.ScannedTranslationUnits = list.Count;
			iterator.ProcessedTranslationUnits = list.Count;
			if (list.Count > 0)
			{
				iterator.PositionFrom = list[list.Count - 1].Id;
			}
			return list;
		}

		public List<PersistentObjectToken> DeleteAllTranslationUnits(PersistentObjectToken tmId)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return new List<PersistentObjectToken>();
			}
			int startAfter = 0;
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			List<PersistentObjectToken> tuIds;
			do
			{
				tuIds = _Context.Storage.GetTuIds(tmId.Id, startAfter, 1000, forward: true);
				Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
				if (tuIds.Count > 0)
				{
					list.AddRange(_Context.Storage.DeleteTus(tmId.Id, tuIds, tm.TextContextMatchType, tm.DataVersion > 0));
					_Context.Storage.CommitTransaction();
					startAfter = tuIds[tuIds.Count - 1].Id;
				}
			}
			while (tuIds.Count > 0);
			return list;
		}

		public int EditTranslationUnits(PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] tuIds)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return 0;
			}
			if (tuIds == null || tuIds.Length == 0)
			{
				return 0;
			}
			FieldDefinitions fields = GetFields(tmId);
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			foreach (PersistentObjectToken persistentObjectToken in tuIds)
			{
				if (VerifyGuid(persistentObjectToken, tmId.Id, PersistentObjectType.TU))
				{
					Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu = _Context.Storage.GetTu(tmId.Id, persistentObjectToken.Id, tm.IdContextMatch);
					if (tu != null)
					{
						list.Add(GetTranslationUnit(tu, fields, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture));
					}
				}
			}
			if (list.Count <= 0)
			{
				return 0;
			}
			return EditTranslationUnitsInternal(tmId, editScript, updateMode, list);
		}

		public int EditTranslationUnits(PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return 0;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> translationUnits = GetTranslationUnits(tmId, iterator);
			if (translationUnits != null && translationUnits.Count > 0)
			{
				return EditTranslationUnitsInternal(tmId, editScript, updateMode, translationUnits);
			}
			return 0;
		}

		public List<PersistentObjectToken> EditTranslationUnitsWithFilter(PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			List<PersistentObjectToken> result = new List<PersistentObjectToken>();
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return result;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> translationUnits = GetTranslationUnits(tmId, iterator);
			if (translationUnits != null && translationUnits.Count > 0)
			{
				EditTranslationUnitsInternal(tmId, editScript, updateMode, translationUnits);
				return translationUnits.Select((Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu) => tu.ResourceId).ToList();
			}
			return result;
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] PreviewEditTranslationUnits(PersistentObjectToken tmId, EditScript editScript, ref RegularIterator iterator)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> translationUnits = GetTranslationUnits(tmId, iterator);
			if (translationUnits == null || translationUnits.Count <= 0)
			{
				return null;
			}
			List<bool> list = EditScriptApplier.Apply(editScript, translationUnits);
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list2 = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i])
				{
					list2.Add(translationUnits[i]);
				}
			}
			return list2.ToArray();
		}

		private int EditTranslationUnitsInternal(PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, IList<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tus)
		{
			int result = 0;
			Importer importer = null;
			if (updateMode == EditUpdateMode.AddTranslationUnit)
			{
				importer = _Context.GetImporter(tmId);
				importer.SkipSynchronousFga = true;
			}
			if (tus == null || tus.Count <= 0)
			{
				return result;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			bool flag = editScript.Actions.Any((EditAction x) => x is EditActionDeleteTags || x is EditActionSearchReplace) && importer == null;
			if (flag)
			{
				foreach (Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu in tus)
				{
					list.Add(tu.SourceSegment.ToString());
					list2.Add(tu.TargetSegment.ToString());
				}
			}
			List<bool> list3 = EditScriptApplier.Apply(editScript, tus);
			bool[] array = new bool[list3.Count];
			if (flag)
			{
				for (int i = 0; i < list3.Count; i++)
				{
					if (tus[i].AlignModelDate.HasValue)
					{
						array[i] = true;
						if (list3[i] && (string.CompareOrdinal(tus[i].SourceSegment.ToString(), list[i]) != 0 || string.CompareOrdinal(tus[i].TargetSegment.ToString(), list2[i]) != 0))
						{
							array[i] = false;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < list3.Count; j++)
				{
					if (tus[j].AlignModelDate.HasValue)
					{
						array[j] = true;
					}
				}
			}
			int num = list3.Sum((bool b) => b ? 1 : 0);
			if (num <= 0)
			{
				return result;
			}
			switch (updateMode)
			{
			case EditUpdateMode.UpdateTranslationUnit:
				UpdateTranslationUnitsInternal(tmId, tus, list3, array, setMetadata: true);
				break;
			case EditUpdateMode.AddTranslationUnit:
			{
				List<AnnotatedTranslationUnit> list4 = new List<AnnotatedTranslationUnit>();
				for (int k = 0; k < list3.Count; k++)
				{
					if (list3[k])
					{
						list4.Add(new AnnotatedTranslationUnit(importer.Tm, tus[k], keepTokens: false, keepPeripheralWhitespace: false));
					}
				}
				importer.Import(list4, null, null, null, isUpdate: false, array);
				break;
			}
			default:
				throw new Exception("unexpected case constant");
			}
			return num;
		}

		public List<PersistentObjectToken> DeleteMatchingTranslationUnits(PersistentObjectToken tmId, FilterExpression filter)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return new List<PersistentObjectToken>();
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			FieldDefinitions fields = GetFields(tmId);
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			PersistentObjectToken[] array = new PersistentObjectToken[100];
			int num = 0;
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tus;
			do
			{
				tus = _Context.Storage.GetTus(tmId.Id, 0, 100, forward: true, idContextMatch: false, includeContextContent: false, TextContextMatchType.PrecedingAndFollowingSource, null, null);
				foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in tus)
				{
					if (filter.Evaluate(GetTranslationUnit(item, fields, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture)))
					{
						array[num++] = new PersistentObjectToken(item.Id, Guid.Empty);
						if (num == 100)
						{
							list.AddRange(_Context.Storage.DeleteTus(tmId.Id, array.ToList(), tm.TextContextMatchType, tm.DataVersion > 0));
							_Context.Storage.CommitTransaction();
							num = 0;
						}
					}
				}
			}
			while (tus.Count == 100);
			if (num <= 0)
			{
				return new List<PersistentObjectToken>();
			}
			list.AddRange(_Context.Storage.DeleteTus(tmId.Id, array.ToList(), tm.TextContextMatchType, tm.DataVersion > 0));
			_Context.Storage.CommitTransaction();
			return list;
		}

		private FieldValues GetFieldValuesFromStorageAttributes(int tmId, List<AttributeValue> attributes, FieldDefinitions fields)
		{
			FieldValues fieldValues = new FieldValues();
			if (attributes == null)
			{
				return fieldValues;
			}
			foreach (AttributeValue attribute in attributes)
			{
				Field field = null;
				if (fields != null)
				{
					field = fields[attribute.AttributeName];
					if (field != null && field.ValueType != attribute.Type)
					{
						field = null;
					}
				}
				switch (attribute.Type)
				{
				case FieldValueType.SingleString:
					fieldValues.Add(new SingleStringFieldValue(attribute.AttributeName, attribute.Value as string));
					break;
				case FieldValueType.MultipleString:
				{
					MultipleStringFieldValue multipleStringFieldValue = new MultipleStringFieldValue(attribute.AttributeName);
					string[] array2 = attribute.Value as string[];
					foreach (string s in array2)
					{
						multipleStringFieldValue.Add(s);
					}
					fieldValues.Add(multipleStringFieldValue);
					break;
				}
				case FieldValueType.DateTime:
					fieldValues.Add(new DateTimeFieldValue(attribute.AttributeName, (DateTime)attribute.Value));
					break;
				case FieldValueType.SinglePicklist:
					if (field != null)
					{
						PicklistItem picklistItem2 = (field as PicklistField).Picklist.Lookup((int)attribute.Value);
						if (picklistItem2 != null)
						{
							fieldValues.Add(new SinglePicklistFieldValue(attribute.AttributeName, new PicklistItem(picklistItem2)));
						}
					}
					else
					{
						fieldValues.Add(new SinglePicklistFieldValue(attribute.AttributeName, new PicklistItem(_Context.Storage.GetPicklistValue(tmId, (int)attribute.Value).Value)));
					}
					break;
				case FieldValueType.MultiplePicklist:
				{
					MultiplePicklistFieldValue multiplePicklistFieldValue = new MultiplePicklistFieldValue(attribute.AttributeName);
					if (field != null)
					{
						PicklistField picklistField = field as PicklistField;
						int[] array = attribute.Value as int[];
						foreach (int id in array)
						{
							PicklistItem picklistItem = picklistField.Picklist.Lookup(id);
							if (picklistItem != null)
							{
								multiplePicklistFieldValue.Add(new PicklistItem(picklistItem));
							}
						}
					}
					else
					{
						int[] array = attribute.Value as int[];
						foreach (int key in array)
						{
							multiplePicklistFieldValue.Add(new PicklistItem(_Context.Storage.GetPicklistValue(tmId, key).Value));
						}
					}
					fieldValues.Add(multiplePicklistFieldValue);
					break;
				}
				case FieldValueType.Integer:
					fieldValues.Add(new IntFieldValue(attribute.AttributeName, (int)attribute.Value));
					break;
				}
			}
			return fieldValues;
		}

		public static int GetTUFlags(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			return (int)(tu.Origin + ((int)tu.Format << 8) + ((int)tu.ConfirmationLevel << 16));
		}

		private static void SetTuFlagsLegacyColumn(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu)
		{
			int flags = storageTu.Flags;
			try
			{
				int num = (int)(tu.Origin = (TranslationUnitOrigin)(flags & 0xFF));
				flags >>= 8;
				num = (int)(tu.Format = (TranslationUnitFormat)(flags & 0xFF));
				flags >>= 8;
				num = (int)(tu.ConfirmationLevel = (ConfirmationLevel)(flags & 0xFF));
			}
			catch
			{
			}
		}

		private static void SetTuFlagsColumns(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu)
		{
			tu.Origin = storageTu.Origin;
			tu.Format = storageTu.Format;
			tu.ConfirmationLevel = storageTu.ConfirmationLevel;
		}

		public void SetTuFlags(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu)
		{
			if (!(_Context.Storage is SqlStorage))
			{
				SetTuFlagsLegacyColumn(tu, storageTu);
			}
			else
			{
				SetTuFlagsColumns(tu, storageTu);
			}
			if (Importer._OriginsToNormalize.Contains(tu.Origin))
			{
				tu.Origin = TranslationUnitOrigin.TM;
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit GetTranslationUnit(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu, FieldDefinitions fields, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (storageTu == null)
			{
				throw new ArgumentNullException("storageTu");
			}
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = new Sdl.LanguagePlatform.TranslationMemory.TranslationUnit();
			_Context.Storage.DeserializeTuSegments(storageTu, translationUnit, sourceCulture, targetCulture);
			translationUnit.SourceSegment.Tokens = TokenSerialization.LoadTokens(storageTu.SourceTokenData, translationUnit.SourceSegment);
			translationUnit.TargetSegment.Tokens = TokenSerialization.LoadTokens(storageTu.TargetTokenData, translationUnit.TargetSegment);
			FillRemainingTranslationUnitDetails(translationUnit, storageTu, fields);
			translationUnit.ResourceId = new PersistentObjectToken(storageTu.Id, storageTu.Guid);
			return translationUnit;
		}

		internal Sdl.LanguagePlatform.TranslationMemory.TranslationUnit GetReducedTranslationUnit(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (storageTu == null)
			{
				throw new ArgumentNullException("storageTu");
			}
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = new Sdl.LanguagePlatform.TranslationMemory.TranslationUnit
			{
				ResourceId = new PersistentObjectToken(storageTu.Id, storageTu.Guid)
			};
			_Context.Storage.DeserializeTuSegments(storageTu, translationUnit, sourceCulture, targetCulture);
			SetTuFlags(translationUnit, storageTu);
			translationUnit.SourceSegment.Tokens = TokenSerialization.LoadTokens(storageTu.SourceTokenData, translationUnit.SourceSegment);
			translationUnit.TargetSegment.Tokens = TokenSerialization.LoadTokens(storageTu.TargetTokenData, translationUnit.TargetSegment);
			return translationUnit;
		}

		internal void FillRemainingTranslationUnitDetails(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu, FieldDefinitions fields)
		{
			if (storageTu == null)
			{
				throw new ArgumentNullException("storageTu");
			}
			tu.Origin = TranslationUnitOrigin.TM;
			tu.SystemFields.UseCount = storageTu.UsageCounter;
			tu.SystemFields.UseDate = storageTu.LastUsedDate;
			tu.SystemFields.UseUser = storageTu.LastUsedUser;
			tu.SystemFields.CreationUser = storageTu.CreationUser;
			tu.SystemFields.CreationDate = storageTu.CreationDate;
			tu.SystemFields.ChangeUser = storageTu.ChangeUser;
			tu.SystemFields.ChangeDate = storageTu.ChangeDate;
			tu.Contexts = storageTu.Contexts;
			tu.IdContexts = storageTu.IdContexts;
			tu.AlignmentData = ((storageTu.AlignmentData == null) ? null : new LiftAlignedSpanPairSet(storageTu.AlignmentData));
			tu.AlignModelDate = storageTu.AlignModelDate;
			tu.InsertDate = storageTu.InsertDate;
			tu.FieldValues = GetFieldValuesFromStorageAttributes(storageTu.TranslationMemoryId, storageTu.Attributes, fields);
			SetTuFlags(tu, storageTu);
			if ((tu.AlignmentData == null || tu.AlignmentData.IsEmpty) && tu.SourceSegment.Tokens != null && tu.TargetSegment.Tokens != null)
			{
				tu.AlignmentData = new LiftAlignedSpanPairSet((short)tu.SourceSegment.Tokens.Count, (short)tu.TargetSegment.Tokens.Count);
				PlaceableComputer.ConvertPlaceablesToAlignments(PlaceableComputer.ComputePlaceables(tu.SourceSegment, tu.TargetSegment), tu.AlignmentData, tu.SourceSegment.Tokens, tu.TargetSegment.Tokens);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit GetAlignableTu(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (storageTu == null)
			{
				throw new ArgumentNullException("storageTu");
			}
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = new Sdl.LanguagePlatform.TranslationMemory.TranslationUnit();
			_Context.Storage.DeserializeTuSegments(storageTu, translationUnit, sourceCulture, targetCulture);
			translationUnit.ResourceId = new PersistentObjectToken(storageTu.Id, storageTu.Guid);
			translationUnit.SourceSegment.Tokens = TokenSerialization.LoadTokens(storageTu.SourceTokenData, translationUnit.SourceSegment);
			translationUnit.TargetSegment.Tokens = TokenSerialization.LoadTokens(storageTu.TargetTokenData, translationUnit.TargetSegment);
			translationUnit.AlignmentData = ((storageTu.AlignmentData == null) ? null : new LiftAlignedSpanPairSet(storageTu.AlignmentData));
			translationUnit.AlignModelDate = storageTu.AlignModelDate;
			translationUnit.InsertDate = storageTu.InsertDate;
			translationUnit.SystemFields.UseCount = storageTu.UsageCounter;
			translationUnit.SystemFields.UseDate = storageTu.LastUsedDate;
			translationUnit.SystemFields.UseUser = storageTu.LastUsedUser;
			translationUnit.SystemFields.CreationUser = storageTu.CreationUser;
			translationUnit.SystemFields.CreationDate = storageTu.CreationDate;
			translationUnit.SystemFields.ChangeUser = storageTu.ChangeUser;
			translationUnit.SystemFields.ChangeDate = storageTu.ChangeDate;
			SetTuFlags(translationUnit, storageTu);
			return translationUnit;
		}

		private void OptimizeStorageForExport()
		{
			(_Context.Storage as SqliteStorage)?.Optimize();
		}

		private bool VerifySort(IList<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tus, bool ascending)
		{
			if (tus == null || tus.Count < 2)
			{
				return true;
			}
			if (ascending)
			{
				for (int i = 1; i < tus.Count; i++)
				{
					if (tus[i - 1].Id >= tus[i].Id)
					{
						return false;
					}
				}
			}
			else
			{
				for (int j = 1; j < tus.Count; j++)
				{
					if (tus[j - 1].Id <= tus[j].Id)
					{
						return false;
					}
				}
			}
			return true;
		}

		public List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetTranslationUnits(PersistentObjectToken tmId, RegularIterator iter)
		{
			return GetTranslationUnits(tmId, iter, skipOptimize: false);
		}

		internal List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetTranslationUnitsWithContextContent(PersistentObjectToken tmId, RegularIterator iter, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture, bool usesIdContextMatch)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			FieldDefinitions fields = GetFields(tmId);
			return TranslationUnitFilteringStrategyFactory.GetBestStrategyForFilter(iter.Filter, _Context, tm.DataVersion).GetTusFiltered(tmId, iter, fields, includeContextContent: true, textContextMatchType, sourceCulture, targetCulture, usesIdContextMatch);
		}

		public List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetTranslationUnits(PersistentObjectToken tmId, RegularIterator iter, bool skipOptimize)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			if (iter.PositionFrom == 0 && _Context.IsFilebasedTm && !skipOptimize)
			{
				OptimizeStorageForExport();
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			FieldDefinitions fields = GetFields(tmId);
			return TranslationUnitFilteringStrategyFactory.GetBestStrategyForFilter(iter.Filter, _Context, tm.DataVersion).GetTusFiltered(tmId, iter, fields, includeContextContent: false, TextContextMatchType.PrecedingAndFollowingSource, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture, tm.IdContextMatch);
		}

		public List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetAlignableTus(PersistentObjectToken tmId, RegularIterator iter, bool unalignedOnly, bool unalignedOrPostdated)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			int num = iter.PositionFrom = iter.PositionTo;
			iter.ScannedTranslationUnits = 0;
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> alignableTus;
			do
			{
				alignableTus = _Context.AlignableStorage.GetAlignableTus(tmId.Id, num, iter.MaxCount, unalignedOnly, unalignedOrPostdated);
				if (alignableTus.Count > 0)
				{
					num = alignableTus[alignableTus.Count - 1].Id;
				}
				foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in alignableTus)
				{
					int num2 = ++iter.ProcessedTranslationUnits;
					num2 = ++iter.ScannedTranslationUnits;
					iter.PositionTo = item.Id;
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit alignableTu = GetAlignableTu(item, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture);
					list.Add(alignableTu);
					if (list.Count == iter.MaxCount)
					{
						break;
					}
				}
			}
			while ((iter.MaxScan <= 0 || iter.ScannedTranslationUnits < iter.MaxScan) && alignableTus.Count > 0 && list.Count < iter.MaxCount);
			if (iter.ScannedTranslationUnits == 0)
			{
				int num2 = iter.PositionFrom = (iter.PositionTo = num);
			}
			return list;
		}

		private void PrepareAlignableTus(IReadOnlyCollection<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tus, PersistentObjectToken tmId)
		{
			if (tus != null && tus.Count != 0)
			{
				AnnotatedTranslationMemory annotatedTranslationMemory = _Context.GetAnnotatedTranslationMemory(tmId);
				Importer importer = _Context.GetImporter(tmId);
				bool flag = false;
				foreach (Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu in tus)
				{
					if (tu.SourceSegment.Tokens == null || tu.TargetSegment.Tokens == null)
					{
						AnnotatedTranslationUnit atu = new AnnotatedTranslationUnit(importer.Tm, tu, keepTokens: false, keepPeripheralWhitespace: false);
						importer.Reindex(atu);
						InvalidateReindexRequiredCache(tmId.Guid);
						flag = true;
					}
					if (!tu.InsertDate.HasValue)
					{
						if (!_Context.IsFilebasedTm)
						{
							throw new Exception("InsertDate should never be null");
						}
						DateTime value = DateTimeUtilities.Normalize(DateTime.Now);
						tu.InsertDate = value;
					}
					annotatedTranslationMemory.SourceTools.Stem(tu.SourceSegment);
					annotatedTranslationMemory.TargetTools.Stem(tu.TargetSegment);
					annotatedTranslationMemory.SourceTools.EnsureTokenizedSegment(tu.SourceSegment);
					annotatedTranslationMemory.TargetTools.EnsureTokenizedSegment(tu.TargetSegment);
					tu.AlignmentData = new LiftAlignedSpanPairSet((short)tu.SourceSegment.Tokens.Count, (short)tu.TargetSegment.Tokens.Count);
					PlaceableComputer.ConvertPlaceablesToAlignments(PlaceableComputer.ComputePlaceables(tu.SourceSegment, tu.TargetSegment), tu.AlignmentData, tu.SourceSegment.Tokens, tu.TargetSegment.Tokens);
				}
				if (flag)
				{
					_Context.Storage.CommitTransaction();
				}
			}
		}

		public List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetAlignableTranslationUnits(PersistentObjectToken tmId, List<int> TUIDs)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> alignableTus = _Context.AlignableStorage.GetAlignableTus(tmId.Id, TUIDs);
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in alignableTus)
			{
				Sdl.LanguagePlatform.TranslationMemory.TranslationUnit alignableTu = GetAlignableTu(item, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture);
				list.Add(alignableTu);
			}
			PrepareAlignableTus(list, tmId);
			return list;
		}

		public List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetFullTranslationUnits(PersistentObjectToken tmId, List<int> TUIDs)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> fullTusByIds = _Context.Storage.GetFullTusByIds(tmId.Id, TUIDs);
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			FieldDefinitions fields = GetFields(tmId);
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in fullTusByIds)
			{
				Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = GetTranslationUnit(item, fields, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture);
				list.Add(translationUnit);
			}
			return list;
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(PersistentObjectToken tmId, List<int> tuIds)
		{
			if (VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return _Context.AlignableStorage.GetAlignmentTimestamps(tmId.Id, tuIds);
			}
			return null;
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(PersistentObjectToken tmId, RegularIterator iter, DateTime modelDate)
		{
			if (VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return _Context.AlignableStorage.GetAlignmentTimestamps(tmId.Id, iter.PositionFrom, iter.MaxCount, modelDate);
			}
			return null;
		}

		public List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> GetTusForAlignment(PersistentObjectToken tmId, RegularIterator iter, bool unalignedOnly, bool postdated)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> alignableTus = GetAlignableTus(tmId, iter, unalignedOnly, postdated);
			if (alignableTus == null || alignableTus.Count == 0)
			{
				return null;
			}
			PrepareAlignableTus(alignableTus, tmId);
			return alignableTus;
		}

		public void AlignTranslationUnits(PersistentObjectToken tmId, CancellationToken token, IProgress<int> progress, bool unalignedOnly, bool unalignedOrPostdatedOnly)
		{
			AlignerDefinition alignerDefinition = _Context.AlignableCorpusManager.GetAlignableCorpus(_Context.GetAlignableCorpusId(tmId)).GetAlignerDefinition();
			if (API.UseExternalAligner())
			{
				throw new NotImplementedException();
			}
			ITranslationModelDataService translationModelDataService;
			if (!_Context.IsFilebasedTm)
			{
				translationModelDataService = API.GetTranslationModelDataService();
			}
			else
			{
				ITranslationModelDataService translationModelDataService2 = new ContainerBasedTranslationModelDataService(_Context.Container);
				translationModelDataService = translationModelDataService2;
			}
			ITranslationModelDataService translationModelDataService3 = translationModelDataService;
			using (translationModelDataService3)
			{
				IFineGrainedAligner aligner = new SimpleAlignerBroker(translationModelDataService3).GetAligner(alignerDefinition);
				aligner.BulkMode = true;
				if (unalignedOnly && unalignedOrPostdatedOnly)
				{
					throw new Exception("AlignTranslationUnits - unalignedOnly && unalignedOrPostdatedOnly");
				}
				RegularIterator regularIterator = new RegularIterator();
				progress.Report(0);
				int num = 0;
				while (true)
				{
					List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tusForAlignment = GetTusForAlignment(tmId, regularIterator, unalignedOnly, unalignedOrPostdatedOnly);
					if (tusForAlignment == null)
					{
						break;
					}
					List<IAlignableContentPair> list = new List<IAlignableContentPair>();
					foreach (Sdl.LanguagePlatform.TranslationMemory.TranslationUnit item in tusForAlignment)
					{
						list.Add(new AlignableTu(item));
					}
					if (token.IsCancellationRequested)
					{
						break;
					}
					if (aligner.Align(list, token, null))
					{
						AnnotatedTranslationMemory annotatedTranslationMemory = _Context.GetAnnotatedTranslationMemory(tmId);
						_Context.AlignableStorage.UpdateTuAlignmentData(GetTuAlignmentData(tusForAlignment, annotatedTranslationMemory), tmId.Id);
					}
					progress.Report(regularIterator.ProcessedTranslationUnits);
					num += regularIterator.ScannedTranslationUnits;
					if (num > 10000)
					{
						num = 0;
						GC.Collect();
						Console.WriteLine(GC.GetTotalMemory(forceFullCollection: true));
					}
				}
			}
		}

		private static IEnumerable<TuAlignmentDataInternal> GetTuAlignmentData(List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tus, AnnotatedTranslationMemory tm)
		{
			foreach (Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu2 in tus)
			{
				AnnotatedTranslationUnit tu = new AnnotatedTranslationUnit(tm, tu2, keepTokens: true, keepPeripheralWhitespace: false);
				yield return GetAlignmentDataInternal(tm.Tm, tu);
			}
		}

		internal static TuAlignmentDataInternal GetAlignmentDataInternal(TranslationMemorySetup tm, AnnotatedTranslationUnit tu)
		{
			TuAlignmentDataInternal tuAlignmentDataInternal = new TuAlignmentDataInternal();
			tuAlignmentDataInternal.tuId = tu.TranslationUnit.ResourceId;
			tuAlignmentDataInternal.alignModelDate = tu.TranslationUnit.AlignModelDate;
			tuAlignmentDataInternal.insertDate = tu.TranslationUnit.InsertDate.Value;
			tuAlignmentDataInternal.alignmentData = tu.TranslationUnit.AlignmentData?.Save();
			tuAlignmentDataInternal.hashes = new HashSet<long>();
			tuAlignmentDataInternal.lengths = new List<byte>();
			tuAlignmentDataInternal.sigwordCounts = new List<byte>();
			tuAlignmentDataInternal.hashes.Add(tu.Source.Hash);
			StorageBasedAlignableCorpus.ExtractIndexData(tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture, tu.TranslationUnit.SourceSegment.Tokens, tu.TranslationUnit.TargetSegment.Tokens, tu.TranslationUnit.AlignmentData, tuAlignmentDataInternal.hashes, tuAlignmentDataInternal.lengths, tuAlignmentDataInternal.sigwordCounts);
			return tuAlignmentDataInternal;
		}

		[Obsolete]
		public bool AlignTranslationUnits(PersistentObjectToken tmId, RegularIterator iter, bool unalignedOnly, bool unalignedOrPostdatedOnly)
		{
			throw new NotSupportedException();
		}

		public bool ReindexTranslationUnits(PersistentObjectToken tmId, RegularIterator iter)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return false;
			}
			Importer importer = _Context.GetImporter(tmId);
			if (_Context.IsFilebasedTm && !TMDupHashInfoMap.ContainsKey(tmId.Guid))
			{
				lock (TMDupHashInfoMapLocker)
				{
					TMDupHashInfo tMDupHashInfo = new TMDupHashInfo
					{
						FileBasedTmTuIdsWithDupSourceHashes = _Context.Storage.GetDuplicateSegmentHashes(tmId.Id, target: false, null)
					};
					if (importer.Tm.Tm.TextContextMatchType == TextContextMatchType.PrecedingSourceAndTarget)
					{
						tMDupHashInfo.FileBasedTmTuIdsWithDupTargetHashes = _Context.Storage.GetDuplicateSegmentHashes(tmId.Id, target: true, null);
					}
					TMDupHashInfoMap.Add(tmId.Guid, tMDupHashInfo);
				}
			}
			IList<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> translationUnits = GetTranslationUnits(tmId, iter);
			if (translationUnits == null || translationUnits.Count == 0)
			{
				if (_Context.IsFilebasedTm)
				{
					lock (TMDupHashInfoMapLocker)
					{
						if (TMDupHashInfoMap.TryGetValue(tmId.Guid, out TMDupHashInfo value))
						{
							_Context.Storage.AddDeduplicatedContextHashes(tmId.Id, ref value.FileBasedTmTuIdsWithDupSourceHashes, ref value.FileBasedTmTuIdsWithDupTargetHashes);
							TMDupHashInfoMap.Remove(tmId.Guid);
						}
					}
				}
				if (!importer.Tm.Tm.UsesLegacyHashes)
				{
					_Context.Storage.DeleteOrphanContexts(tmId.Id, importer.Tm.Tm.TextContextMatchType);
				}
				return false;
			}
			importer.Reindex(translationUnits.Select((Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu) => new AnnotatedTranslationUnit(importer.Tm, tu, keepTokens: false, keepPeripheralWhitespace: false)).ToArray());
			InvalidateReindexRequiredCache(tmId.Guid);
			return true;
		}

		internal long GetCurrentTokenizationSignatureHash(AnnotatedTranslationMemory tm)
		{
			string tokenizerSignature = tm.SourceTools.TokenizerSignature;
			string tokenizerSignature2 = tm.TargetTools.TokenizerSignature;
			string stemmerSignature = tm.SourceTools.StemmerSignature;
			string stemmerSignature2 = tm.TargetTools.StemmerSignature;
			string str = tokenizerSignature + "-" + stemmerSignature;
			string text = string.Concat(str1: tokenizerSignature2 + "-" + stemmerSignature2, str0: str + "-" + tm.SourceTools.StoplistSignature);
			if (!tm.Tm.UsesLegacyHashes)
			{
				text += "strict";
			}
			return Hash.GetHashCodeLong(text);
		}

		private void InvalidateReindexRequiredCache(Guid tmGuid)
		{
			if (_Context.IsFilebasedTm)
			{
				lock (_ReindexRequiredCache)
				{
					_ReindexRequiredCache.Remove(tmGuid);
				}
			}
		}

		private bool? TryGetReindexRequired(Guid tmGuid)
		{
			if (!_Context.IsFilebasedTm)
			{
				return null;
			}
			lock (_ReindexRequiredCache)
			{
				if (_ReindexRequiredCache.TryGetValue(tmGuid, out bool value))
				{
					return value;
				}
			}
			return null;
		}

		private void SetGetReindexRequired(Guid tmGuid, bool required)
		{
			if (_Context.IsFilebasedTm)
			{
				lock (_ReindexRequiredCache)
				{
					_ReindexRequiredCache.Remove(tmGuid);
					_ReindexRequiredCache.Add(tmGuid, required);
				}
			}
		}

		public bool? GetReindexRequired(PersistentObjectToken tmId)
		{
			bool? result = TryGetReindexRequired(tmId.Guid);
			if (result.HasValue)
			{
				return result;
			}
			AnnotatedTranslationMemory annotatedTranslationMemory = _Context.GetAnnotatedTranslationMemory(tmId);
			long currentTokenizationSignatureHash = GetCurrentTokenizationSignatureHash(annotatedTranslationMemory);
			result = _Context.Storage.GetReindexRequired(tmId.Id, currentTokenizationSignatureHash);
			if (!result.HasValue || result.Value || !_Context.IsFilebasedTm)
			{
				if (result.HasValue)
				{
					SetGetReindexRequired(tmId.Guid, result.Value);
				}
				return result;
			}
			result = HasLegacyFileBasedHashChanges(annotatedTranslationMemory, currentTokenizationSignatureHash);
			SetGetReindexRequired(tmId.Guid, result.Value);
			return result;
		}

		private bool HasLegacyFileBasedHashChanges(AnnotatedTranslationMemory tm, long currentHash)
		{
			if (!_Context.IsFilebasedTm)
			{
				return false;
			}
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tus = _Context.Storage.GetTus(tm.Tm.ResourceId.Id, -1, 100, forward: true, tm.Tm.IdContextMatch, includeContextContent: false, tm.Tm.TextContextMatchType, tm.Tm.LanguageDirection.SourceCulture, tm.Tm.LanguageDirection.TargetCulture);
			FieldDefinitions fields = GetFields(tm.Tm.ResourceId);
			List<int> list = new List<int>();
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in tus)
			{
				Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = GetTranslationUnit(item, fields, tm.Tm.LanguageDirection.SourceCulture, tm.Tm.LanguageDirection.TargetCulture);
				AnnotatedTranslationUnit annotatedTranslationUnit = new AnnotatedTranslationUnit(tm, translationUnit, keepTokens: false, keepPeripheralWhitespace: false);
				long num = tm.Tm.UsesLegacyHashes ? annotatedTranslationUnit.Source.Hash : annotatedTranslationUnit.Source.StrictHash;
				long num2 = tm.Tm.UsesLegacyHashes ? annotatedTranslationUnit.Target.Hash : annotatedTranslationUnit.Target.StrictHash;
				if (num != item.Source.Hash || num2 != item.Target.Hash)
				{
					list.Add(item.Id);
				}
			}
			RegularIterator regularIterator = new RegularIterator();
			int startAfter = regularIterator.PositionFrom = regularIterator.PositionTo;
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tusForReindex = _Context.AlignableStorage.GetTusForReindex(tm.Tm.ResourceId.Id, startAfter, regularIterator.MaxCount, currentHash);
			HashSet<int> idsNeedingReindex = new HashSet<int>();
			tusForReindex.ForEach(delegate(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit x)
			{
				idsNeedingReindex.Add(x.Id);
			});
			foreach (int item2 in list)
			{
				if (!idsNeedingReindex.Contains(item2))
				{
					return true;
				}
			}
			return false;
		}

		public int GetTuCountForReindex(PersistentObjectToken tmId)
		{
			AnnotatedTranslationMemory annotatedTranslationMemory = _Context.GetAnnotatedTranslationMemory(tmId);
			long currentTokenizationSignatureHash = GetCurrentTokenizationSignatureHash(annotatedTranslationMemory);
			return _Context.Storage.GetTuCountForReindex(tmId.Id, currentTokenizationSignatureHash);
		}

		public void SelectiveReindexTranslationUnits(PersistentObjectToken tmId, CancellationToken token, IProgress<int> progress)
		{
			ReindexTranslationUnits(tmId, token, progress, selective: true);
		}

		internal void ReindexTranslationUnits(PersistentObjectToken tmId, CancellationToken token, IProgress<int> progress, bool selective)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return;
			}
			RegularIterator regularIterator = new RegularIterator();
			Importer importer = _Context.GetImporter(tmId);
			int num = 0;
			AnnotatedTranslationMemory annotatedTranslationMemory = _Context.GetAnnotatedTranslationMemory(tmId);
			long tokenizationSignatureHash = importer.TokenizationSignatureHash;
			if (HasLegacyFileBasedHashChanges(annotatedTranslationMemory, tokenizationSignatureHash))
			{
				selective = false;
			}
			if (selective)
			{
				int tuCount = GetTuCount(tmId);
				if (_Context.Storage.GetTuCountForReindex(tmId.Id, tokenizationSignatureHash) > tuCount / 3)
				{
					selective = false;
				}
			}
			List<int[]> tuIdsWithDupSourceHashes = _Context.Storage.GetDuplicateSegmentHashes(tmId.Id, target: false, tokenizationSignatureHash);
			List<int[]> tuIdsWithDupTargetHashes = null;
			if (annotatedTranslationMemory.Tm.TextContextMatchType == TextContextMatchType.PrecedingSourceAndTarget)
			{
				tuIdsWithDupTargetHashes = _Context.Storage.GetDuplicateSegmentHashes(tmId.Id, target: true, tokenizationSignatureHash);
			}
			while (!token.IsCancellationRequested)
			{
				List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
				int num2 = regularIterator.PositionFrom = regularIterator.PositionTo;
				regularIterator.ScannedTranslationUnits = 0;
				if (selective)
				{
					List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tusForReindex;
					do
					{
						tusForReindex = _Context.AlignableStorage.GetTusForReindex(tmId.Id, num2, regularIterator.MaxCount, tokenizationSignatureHash);
						if (tusForReindex.Count > 0)
						{
							num2 = tusForReindex[tusForReindex.Count - 1].Id;
						}
						foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in tusForReindex)
						{
							int num3 = ++regularIterator.ProcessedTranslationUnits;
							num3 = ++regularIterator.ScannedTranslationUnits;
							regularIterator.PositionTo = item.Id;
							Sdl.LanguagePlatform.TranslationMemory.TranslationUnit reducedTranslationUnit = GetReducedTranslationUnit(item, annotatedTranslationMemory.Tm.LanguageDirection.SourceCulture, annotatedTranslationMemory.Tm.LanguageDirection.TargetCulture);
							list.Add(reducedTranslationUnit);
							if (list.Count == regularIterator.MaxCount)
							{
								break;
							}
						}
					}
					while ((regularIterator.MaxScan <= 0 || regularIterator.ScannedTranslationUnits < regularIterator.MaxScan) && tusForReindex.Count > 0 && list.Count < regularIterator.MaxCount);
				}
				else
				{
					list.AddRange(GetTranslationUnits(tmId, regularIterator));
				}
				if (regularIterator.ScannedTranslationUnits == 0)
				{
					int num3 = regularIterator.PositionFrom = (regularIterator.PositionTo = num2);
				}
				if (list.Count == 0)
				{
					break;
				}
				List<AnnotatedTranslationUnit> list2 = new List<AnnotatedTranslationUnit>();
				foreach (Sdl.LanguagePlatform.TranslationMemory.TranslationUnit item2 in list)
				{
					list2.Add(new AnnotatedTranslationUnit(importer.Tm, item2, keepTokens: false, keepPeripheralWhitespace: false));
				}
				importer.Reindex(list2.ToArray());
				InvalidateReindexRequiredCache(tmId.Guid);
				num += list.Count;
				progress?.Report(num);
				_Context.Complete();
			}
			_Context.Storage.AddDeduplicatedContextHashes(tmId.Id, ref tuIdsWithDupSourceHashes, ref tuIdsWithDupTargetHashes);
			if (!annotatedTranslationMemory.Tm.UsesLegacyHashes)
			{
				_Context.Storage.DeleteOrphanContexts(tmId.Id, annotatedTranslationMemory.Tm.TextContextMatchType);
			}
			_Context.Complete();
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] SearchDuplicateTranslationUnits(PersistentObjectToken tmId, DuplicateIterator iter)
		{
			return SearchDuplicateTranslationUnits(tmId, iter, targetSegments: false);
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] SearchDuplicateTranslationUnits(PersistentObjectToken tmId, DuplicateIterator iter, bool targetSegments)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list = new List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>();
			DuplicateIterator.DuplicateIteratorPosition duplicateIteratorPosition = iter.Forward ? (iter.PositionFrom = iter.PositionTo) : (iter.PositionTo = iter.PositionFrom);
			FieldDefinitions fields = GetFields(tmId);
			iter.ScannedTranslationUnits = 0;
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list2;
			do
			{
				list2 = _Context.Storage.DuplicateSearch(tmId.Id, duplicateIteratorPosition.Hash, duplicateIteratorPosition.TUId, iter.MaxCount, iter.Forward, targetSegments);
				if (list2.Count == 0 && duplicateIteratorPosition.TUId != 0)
				{
					duplicateIteratorPosition.TUId = 0;
					list2 = _Context.Storage.DuplicateSearch(tmId.Id, duplicateIteratorPosition.Hash, duplicateIteratorPosition.TUId, iter.MaxCount, iter.Forward, targetSegments);
				}
				iter.ProcessedTranslationUnits += list2.Count;
				if (list2.Count > 0)
				{
					Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit translationUnit = list2[list2.Count - 1];
					long num = targetSegments ? translationUnit.Target.Hash : translationUnit.Source.Hash;
					duplicateIteratorPosition = ((list2.Count != iter.MaxCount) ? (iter.Forward ? new DuplicateIterator.DuplicateIteratorPosition(num, 0) : new DuplicateIterator.DuplicateIteratorPosition(num - 1, 0)) : (iter.Forward ? new DuplicateIterator.DuplicateIteratorPosition(num, list2[list2.Count - 1].Id) : new DuplicateIterator.DuplicateIteratorPosition(num, list2[list2.Count - 1].Id - 1)));
				}
				foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in list2)
				{
					if (iter.Filter == null || iter.Filter.Evaluate(GetTranslationUnit(item, fields, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture)))
					{
						if (iter.Forward)
						{
							list.Add(item);
						}
						else
						{
							list.Insert(0, item);
						}
						if (list.Count == iter.MaxCount)
						{
							break;
						}
					}
				}
				iter.ScannedTranslationUnits += list2.Count;
			}
			while ((iter.MaxScan <= 0 || iter.ScannedTranslationUnits < iter.MaxScan) && list2.Count > 0 && list.Count == 0);
			List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list3 = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			if (list.Count <= 0)
			{
				DuplicateIterator.DuplicateIteratorPosition duplicateIteratorPosition4 = iter.PositionFrom = (iter.PositionTo = duplicateIteratorPosition);
			}
			else
			{
				long num2 = targetSegments ? list[0].Target.Hash : list[0].Source.Hash;
				long hash = targetSegments ? list[list.Count - 1].Target.Hash : list[list.Count - 1].Source.Hash;
				iter.PositionFrom = new DuplicateIterator.DuplicateIteratorPosition(num2 - 1, 0);
				iter.PositionTo = new DuplicateIterator.DuplicateIteratorPosition(hash, (list.Count == iter.MaxCount) ? list[list.Count - 1].Id : 0);
				foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item2 in list)
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit2 = GetTranslationUnit(item2, fields, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture);
					list3.Add(translationUnit2);
				}
			}
			return list3.ToArray();
		}

		public ImportResult UpdateTranslationUnit(PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			DeleteTranslationUnitInternal(tmId, tu.ResourceId, skipOrphanContextDeletion: true);
			tu.SystemFields.ChangeDate = DateTimeUtilities.Normalize(DateTime.Now);
			tu.SystemFields.ChangeUser = null;
			Importer importer = _Context.GetImporter(tmId);
			return importer.ImportInternal(importParameters: new ImportParameters
			{
				Type = ImportType.Update,
				RetainFgaInfo = false,
				IsBatch = false,
				TuContext = null,
				PreviousTranslationHash = 0
			}, atu: new AnnotatedTranslationUnit(importer.Tm, tu, keepTokens: false, keepPeripheralWhitespace: false), settings: null);
		}

		public ImportResult[] UpdateTranslationUnits(PersistentObjectToken tmId, IList<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tus, IList<bool> mask)
		{
			return UpdateTranslationUnitsInternal(tmId, tus, mask, null, setMetadata: true);
		}

		internal ImportResult[] UpdateTranslationUnitsInternal(PersistentObjectToken tmId, IList<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tus, IList<bool> mask, IList<bool> retainFGAInfo, bool setMetadata)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return null;
			}
			if (tus == null)
			{
				return null;
			}
			if (tus.Count == 0)
			{
				return new ImportResult[0];
			}
			PersistentObjectToken[] array = new PersistentObjectToken[tus.Count];
			for (int i = 0; i < tus.Count; i++)
			{
				array[i] = tus[i].ResourceId;
			}
			if (mask == null)
			{
				DeleteTranslationUnitsInternal(tmId, (IReadOnlyCollection<PersistentObjectToken>)(object)array, skipOrphanContextDeletion: true);
			}
			else
			{
				for (int j = 0; j < tus.Count; j++)
				{
					if (mask[j])
					{
						DeleteTranslationUnitInternal(tmId, tus[j].ResourceId, skipOrphanContextDeletion: true);
					}
				}
			}
			Importer importer = _Context.GetImporter(tmId);
			AnnotatedTranslationUnit[] array2 = new AnnotatedTranslationUnit[tus.Count];
			for (int k = 0; k < tus.Count; k++)
			{
				if (tus[k] == null)
				{
					array2[k] = null;
					continue;
				}
				if ((mask == null || mask[k]) && setMetadata)
				{
					tus[k].SystemFields.ChangeDate = DateTimeUtilities.Normalize(DateTime.Now);
					tus[k].SystemFields.ChangeUser = _Context.UserName;
				}
				bool keepTokens = retainFGAInfo != null && retainFGAInfo[k];
				array2[k] = new AnnotatedTranslationUnit(importer.Tm, tus[k], keepTokens, keepPeripheralWhitespace: false);
			}
			importer.SkipSynchronousFga = true;
			return importer.Import(array2, null, null, mask, isUpdate: true, retainFGAInfo);
		}

		public static AttributeValue GetStorageAttributeValue(AttributeDeclaration decl, FieldValue fv, bool ignoreNewFields)
		{
			AttributeValue result = null;
			switch (decl.Type)
			{
			case FieldValueType.SingleString:
			{
				SingleStringFieldValue singleStringFieldValue = fv as SingleStringFieldValue;
				if (!string.IsNullOrEmpty(singleStringFieldValue.Value) && singleStringFieldValue.Value.Length > TranslationMemorySetup.MaximumTextFieldValueLength)
				{
					throw new LanguagePlatformException(ErrorCode.TMExceededTextFieldValueLimit);
				}
				result = new AttributeValue(decl, singleStringFieldValue.Value);
				break;
			}
			case FieldValueType.MultipleString:
				if ((fv as MultipleStringFieldValue).Values.Any((string v) => !string.IsNullOrEmpty(v) && v.Length > TranslationMemorySetup.MaximumTextFieldValueLength))
				{
					throw new LanguagePlatformException(ErrorCode.TMExceededTextFieldValueLimit);
				}
				result = new AttributeValue(decl, ((MultipleStringFieldValue)fv).Values.ToArray());
				break;
			case FieldValueType.DateTime:
				result = new AttributeValue(decl, ((DateTimeFieldValue)fv).Value);
				break;
			case FieldValueType.SinglePicklist:
			{
				string text = ((SinglePicklistFieldValue)fv)?.Value.Name;
				int num2 = decl.FindPicklistValueId(text);
				if (num2 != -1)
				{
					result = new AttributeValue(decl, num2);
				}
				else if (!ignoreNewFields)
				{
					throw new ArgumentException("Invalid picklist value for this attribute.", text);
				}
				break;
			}
			case FieldValueType.MultiplePicklist:
			{
				List<int> list = new List<int>();
				foreach (PicklistItem value in ((MultiplePicklistFieldValue)fv).Values)
				{
					int num = decl.FindPicklistValueId(value.Name);
					if (num != -1)
					{
						if (!list.Contains(num))
						{
							list.Add(num);
						}
					}
					else if (!ignoreNewFields)
					{
						throw new ArgumentException("Invalid picklist value for this attribute.", value.Name);
					}
				}
				result = new AttributeValue(decl, list.ToArray());
				break;
			}
			case FieldValueType.Integer:
				result = new AttributeValue(decl, ((IntFieldValue)fv).Value);
				break;
			default:
				throw new ArgumentException("Invalid attribute type.");
			}
			return result;
		}

		public static List<AttributeValue> GetAttributeValuesFromFieldValues(List<AttributeDeclaration> attributeDeclarations, FieldValues fieldValues)
		{
			List<AttributeValue> list = new List<AttributeValue>();
			if (fieldValues == null || fieldValues.Count <= 0)
			{
				return list;
			}
			foreach (FieldValue fv in fieldValues)
			{
				AttributeDeclaration attributeDeclaration = attributeDeclarations.Find((AttributeDeclaration d) => d.Name.Equals(fv.Name, StringComparison.Ordinal));
				if (attributeDeclaration != null)
				{
					AttributeValue storageAttributeValue = GetStorageAttributeValue(attributeDeclaration, fv, ignoreNewFields: true);
					if (storageAttributeValue != null)
					{
						list.Add(storageAttributeValue);
					}
				}
			}
			return list;
		}

		private bool ApplyFieldsToStorageTu(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit sTu, List<AttributeDeclaration> attributeDeclarations, FieldValues newValues, bool overwrite, FieldDefinitions fields)
		{
			FieldValues fieldValuesFromStorageAttributes = GetFieldValuesFromStorageAttributes(sTu.TranslationMemoryId, sTu.Attributes, fields);
			bool result = false;
			if (overwrite)
			{
				fieldValuesFromStorageAttributes.Clear();
				if (sTu.Attributes.Count > 0)
				{
					sTu.Attributes.Clear();
					result = true;
				}
			}
			if (!fieldValuesFromStorageAttributes.Merge(newValues))
			{
				return result;
			}
			sTu.Attributes = GetAttributeValuesFromFieldValues(attributeDeclarations, fieldValuesFromStorageAttributes);
			return true;
		}

		public int ApplyFieldsToTus(PersistentObjectToken tmId, FieldValues newValues, bool overwrite, PersistentObjectToken[] tuIds)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return 0;
			}
			if (tuIds == null || tuIds.Length == 0)
			{
				return 0;
			}
			int num = 0;
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(tmId.Id);
			List<AttributeDeclaration> attributes = _Context.Storage.GetAttributes(tm.Id);
			FieldDefinitions fields = GetFields(tmId);
			foreach (PersistentObjectToken persistentObjectToken in tuIds)
			{
				Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu = _Context.Storage.GetTu(tmId.Id, persistentObjectToken.Id, tm.IdContextMatch);
				if (tu.TranslationMemoryId != tmId.Id)
				{
					throw new LanguagePlatformException(ErrorCode.DATUNotInTM);
				}
				if (ApplyFieldsToStorageTu(tu, attributes, newValues, overwrite, fields))
				{
					tu.ChangeDate = DateTimeUtilities.Normalize(DateTime.Now);
					tu.ChangeUser = _Context.UserName;
					_Context.Storage.UpdateTuHeader(tu, rewriteAttributes: true);
					num++;
				}
			}
			return num;
		}

		public void SetPassword(PersistentObjectToken tmId, Permission permission, string pwd)
		{
			if (tmId == null || VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				SqliteStorage sqliteStorage = _Context.Storage as SqliteStorage;
				if (sqliteStorage == null)
				{
					throw new NotSupportedException();
				}
				bool num = IsPasswordProtected(tmId);
				sqliteStorage.SetPassword(tmId, permission, pwd);
				if (!num || permission == Permission.Administrator)
				{
					_Context.UpdatePassword(pwd);
				}
			}
		}

		public bool HasPassword(PersistentObjectToken tmId, Permission permission)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return false;
			}
			SqliteStorage sqliteStorage = _Context.Storage as SqliteStorage;
			if (sqliteStorage != null)
			{
				return sqliteStorage.HasPassword(tmId, permission);
			}
			throw new NotSupportedException();
		}

		public Permission IsPasswordSet(PersistentObjectToken tmId, string pwd)
		{
			if (!VerifyGuid(tmId, 0, PersistentObjectType.TM))
			{
				return Permission.None;
			}
			SqliteStorage sqliteStorage = _Context.Storage as SqliteStorage;
			if (sqliteStorage != null)
			{
				if (IsPasswordProtected(tmId))
				{
					return sqliteStorage.GetPasswordPermissionLevel(tmId, pwd);
				}
				return Permission.None;
			}
			throw new NotSupportedException();
		}

		private bool IsPasswordProtected(PersistentObjectToken tmId)
		{
			if (!HasPassword(tmId, Permission.ReadOnly) && !HasPassword(tmId, Permission.ReadWrite) && !HasPassword(tmId, Permission.Maintenance))
			{
				return HasPassword(tmId, Permission.Administrator);
			}
			return true;
		}

		internal void ClearFuzzyCache(Container container)
		{
			_Context.Storage.ClearFuzzyCache(container as FileContainer);
		}

		internal bool HasFuzzyCacheNonEmpty(Container container)
		{
			SqliteStorage sqliteStorage;
			if (!(container is FileContainer) || (sqliteStorage = (_Context.Storage as SqliteStorage)) == null)
			{
				return false;
			}
			return sqliteStorage.HasFuzzyCacheNonEmpty(container);
		}
	}
}
