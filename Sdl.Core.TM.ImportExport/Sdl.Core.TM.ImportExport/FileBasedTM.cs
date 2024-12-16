using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.TM.ImportExport
{
	internal class FileBasedTM : IImportDestination, IExportOrigin
	{
		private readonly FileContainer _container;

		private readonly TranslationMemorySetup _tm;

		public CultureInfo SourceLanguage => _tm.LanguageDirection.SourceCulture;

		public CultureInfo TargetLanguage => _tm.LanguageDirection.TargetCulture;

		public FieldDefinitions FieldDefinitions => Service.GetFields(_container, _tm.ResourceId);

		public string FilePath
		{
			get;
		}

		public bool UsesLegacyHashes => _tm.UsesLegacyHashes;

		private API Service => new API();

		public LanguageResource[] LanguageResources => Service.GetLanguageResources(_container, includeData: true);

		public string Name => _tm.Name;

		public TokenizerFlags TokenizerFlags => _tm.TokenizerFlags;

		public WordCountFlags WordCountFlags => _tm.WordCountFlags;

		public TextContextMatchType TextContextMatchType => _tm.TextContextMatchType;

		public bool UsesIdContextMatch => _tm.IdContextMatch;

		public bool IncludesContextContent => true;

		public DateTime CreationDate => _tm.CreationDate;

		public string CreationUserName => _tm.CreationUser;

		public BuiltinRecognizers BuiltinRecognizers => _tm.Recognizers;

		public FileBasedTM(string path)
		{
			_container = new FileContainer(path);
			FilePath = path;
			_tm = GetTranslationMemorySetup(checkPermissions: true);
		}

		public FileBasedTM(string path, string name, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes fuzzyIndexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, bool fgaSupport, TextContextMatchType textContextMatchType, bool usesIdContextMatch, bool usesLegacyHashes)
		{
			_container = new FileContainer(path, createIfNotExists: true);
			FilePath = path;
			Service.CreateSchema(_container);
			_tm = new TranslationMemorySetup
			{
				LanguageDirection = new LanguagePair(sourceLanguage, targetLanguage),
				Name = Path.GetFileNameWithoutExtension(FilePath),
				FuzzyIndexes = fuzzyIndexes,
				Recognizers = recognizers,
				TokenizerFlags = tokenizerFlags,
				WordCountFlags = wordCountFlags,
				FGASupport = ((!fgaSupport) ? FGASupport.Off : FGASupport.Automatic),
				TextContextMatchType = textContextMatchType,
				IdContextMatch = usesIdContextMatch,
				UsesLegacyHashes = usesLegacyHashes
			};
			_tm.ResourceId = Service.CreateTranslationMemory(_container, _tm);
		}

		private TranslationMemorySetup GetTranslationMemorySetup(bool checkPermissions)
		{
			TranslationMemorySetup[] translationMemories = Service.GetTranslationMemories(_container, checkPermissions);
			if (translationMemories == null || translationMemories.Length == 0)
			{
				throw new LanguagePlatformException(ErrorCode.TMNotFound);
			}
			if (translationMemories.Length > 1)
			{
				throw new InvalidOperationException("Storing multiple translation memories in a file-based container is not supported.");
			}
			return translationMemories[0];
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] tus, ImportSettings settings)
		{
			return Service.AddTranslationUnits(_container, _tm.ResourceId, tus, settings);
		}

		public ImportResult[] AddTranslationUnitsMask(TranslationUnit[] tus, ImportSettings settings, bool[] mask)
		{
			return Service.AddTranslationUnitsMasked(_container, _tm.ResourceId, tus, settings, mask);
		}

		public void UpdateFieldDefinitions(FieldDefinitions mergedFieldDefinitions)
		{
			FieldDefinitions fieldDefinitions = FieldDefinitions;
			Dictionary<PersistentObjectToken, HashSet<string>> dictionary = new Dictionary<PersistentObjectToken, HashSet<string>>();
			List<Field> list = new List<Field>();
			foreach (Field def in mergedFieldDefinitions)
			{
				Field field = fieldDefinitions.FirstOrDefault((Field x) => string.CompareOrdinal(x.Name, def.Name) == 0);
				Field field2 = list.FirstOrDefault((Field x) => string.Equals(x.Name, def.Name, StringComparison.InvariantCultureIgnoreCase));
				if (field == null && field2 == null)
				{
					list.Add(def);
				}
				else if (def.ValueType == FieldValueType.MultiplePicklist || def.ValueType == FieldValueType.SinglePicklist)
				{
					foreach (string picklistItemName in def.PicklistItemNames)
					{
						if (!field.PicklistItemNames.Contains(picklistItemName))
						{
							if (!dictionary.TryGetValue(field.ResourceId, out HashSet<string> value))
							{
								value = new HashSet<string>();
								dictionary.Add(field.ResourceId, value);
							}
							value.Add(picklistItemName);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				FieldDefinitions fields = new FieldDefinitions(list);
				Service.AddFields(_container, _tm.ResourceId, fields);
			}
			foreach (PersistentObjectToken key in dictionary.Keys)
			{
				Service.AddPicklistValues(_container, _tm.ResourceId, key, dictionary[key].ToArray());
			}
		}

		public void UpdateLanguageResources(List<LanguageResource> mergedLanguageResources)
		{
			foreach (LanguageResource mergedLanguageResource in mergedLanguageResources)
			{
				if (mergedLanguageResource.ResourceId == null)
				{
					Service.CreateLanguageResource(_container, mergedLanguageResource);
				}
				else
				{
					Service.UpdateLanguageResource(_container, mergedLanguageResource);
				}
			}
		}

		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iter)
		{
			return Service.GetTranslationUnitsWithContextContent(_container, _tm.ResourceId, ref iter);
		}
	}
}
