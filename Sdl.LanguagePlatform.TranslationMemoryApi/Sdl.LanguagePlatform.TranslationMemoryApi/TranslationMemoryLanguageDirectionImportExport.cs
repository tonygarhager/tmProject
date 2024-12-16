using Sdl.Core.Api.DataAccess;
using Sdl.Core.TM.ImportExport;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class TranslationMemoryLanguageDirectionImportExport : IImportDestination, IExportOrigin
	{
		private ITranslationMemoryLanguageDirection _ld;

		public CultureInfo SourceLanguage => _ld.SourceLanguage;

		public CultureInfo TargetLanguage => _ld.TargetLanguage;

		public FieldDefinitions FieldDefinitions => new FieldDefinitions(_ld.TranslationProvider.FieldDefinitions.AsFields());

		public bool UsesLegacyHashes => (_ld.TranslationProvider as IAdvancedContextTranslationMemory)?.UsesLegacyHashes ?? true;

		public LanguageResource[] LanguageResources
		{
			get
			{
				List<LanguageResource> list = new List<LanguageResource>();
				foreach (LanguageResourceEntity entity in _ld.TranslationProvider.LanguageResourceBundles.Entities)
				{
					LanguageResource languageResource = new LanguageResource();
					languageResource.CultureName = entity.CultureName;
					languageResource.ResourceId = new PersistentObjectToken(entity.Id.IntValue, entity.UniqueId.Value);
					languageResource.Type = entity.Type.Value;
					languageResource.Data = entity.Data;
					list.Add(languageResource);
				}
				return list.ToArray();
			}
		}

		public string Name => _ld.TranslationProvider.Name;

		public TokenizerFlags TokenizerFlags => (_ld.TranslationProvider as ITranslationMemory2015)?.TokenizerFlags ?? TokenizerFlags.DefaultFlags;

		public WordCountFlags WordCountFlags => (_ld.TranslationProvider as ITranslationMemory2015)?.WordCountFlags ?? WordCountFlags.BreakOnTag;

		public TextContextMatchType TextContextMatchType => (_ld.TranslationProvider as IAdvancedContextTranslationMemory)?.TextContextMatchType ?? TextContextMatchType.PrecedingSourceAndTarget;

		public bool UsesIdContextMatch => (_ld.TranslationProvider as IAdvancedContextTranslationMemory)?.UsesIdContextMatching ?? false;

		public bool IncludesContextContent => _ld is IAdvancedContextTranslationMemoryLanguageDirection;

		public DateTime CreationDate => _ld.TranslationProvider.CreationDate;

		public string CreationUserName => _ld.TranslationProvider.CreationUserName;

		public BuiltinRecognizers BuiltinRecognizers => _ld.TranslationProvider.Recognizers;

		public TranslationMemoryLanguageDirectionImportExport(ITranslationMemoryLanguageDirection ld)
		{
			_ld = ld;
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] tus, ImportSettings settings)
		{
			return _ld.AddTranslationUnits(tus, settings);
		}

		public ImportResult[] AddTranslationUnitsMask(TranslationUnit[] tus, ImportSettings settings, bool[] mask)
		{
			return _ld.AddTranslationUnitsMasked(tus, settings, mask);
		}

		public void UpdateFieldDefinitions(FieldDefinitions mergedFieldDefinitions)
		{
			foreach (Field field in mergedFieldDefinitions)
			{
				FieldDefinition fieldDefinition = _ld.TranslationProvider.FieldDefinitions.FirstOrDefault((FieldDefinition x) => x.Name == field.Name);
				if (fieldDefinition != null)
				{
					UpdatePicklist(fieldDefinition, field);
				}
				else
				{
					_ld.TranslationProvider.FieldDefinitions.Add(new FieldDefinition(field, isReadOnly: false));
				}
			}
			Save();
		}

		private void UpdatePicklist(FieldDefinition existingField, Field incomingField)
		{
			PicklistField picklistField = null;
			FieldValueType valueType = incomingField.ValueType;
			if ((uint)(valueType - 4) <= 1u)
			{
				picklistField = (incomingField as PicklistField);
				foreach (string s in picklistField.PicklistItemNames)
				{
					if (existingField.PicklistItems.FirstOrDefault((PicklistItemDefinition x) => string.CompareOrdinal(x.Name, s) == 0) == null)
					{
						existingField.PicklistItems.Add(s);
					}
				}
			}
		}

		public void UpdateLanguageResources(List<LanguageResource> mergedLanguageResources)
		{
			foreach (LanguageResource lr in mergedLanguageResources)
			{
				LanguageResourceEntity languageResourceEntity = _ld.TranslationProvider.LanguageResourceBundles.Entities.FirstOrDefault((LanguageResourceEntity x) => x.UniqueId.Value == lr.ResourceId.Guid);
				if (languageResourceEntity != null)
				{
					UpdateLanguageResource(languageResourceEntity, lr);
				}
				else
				{
					LanguageResourceEntity languageResourceEntity2 = new LanguageResourceEntity();
					languageResourceEntity2.Id = new Identity(lr.ResourceId.Id);
					languageResourceEntity2.UniqueId = lr.ResourceId.Guid;
					languageResourceEntity2.Type = lr.Type;
					languageResourceEntity2.CultureName = lr.CultureName;
					languageResourceEntity2.Data = lr.Data;
					_ld.TranslationProvider.LanguageResourceBundles.Entities.Add(languageResourceEntity2);
				}
			}
			Save();
		}

		private void UpdateLanguageResource(LanguageResourceEntity entity, LanguageResource lr)
		{
			entity.Type = lr.Type;
			entity.CultureName = lr.CultureName;
			entity.Data = lr.Data;
		}

		private void Save()
		{
			_ld.TranslationProvider.Save();
			(_ld.TranslationProvider as AbstractLocalTranslationMemory)?.Refresh();
		}

		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iter)
		{
			IAdvancedContextTranslationMemoryLanguageDirection advancedContextTranslationMemoryLanguageDirection = _ld as IAdvancedContextTranslationMemoryLanguageDirection;
			if (advancedContextTranslationMemoryLanguageDirection != null)
			{
				return advancedContextTranslationMemoryLanguageDirection.GetTranslationUnitsWithContextContent(ref iter);
			}
			return _ld.GetTranslationUnits(ref iter);
		}
	}
}
