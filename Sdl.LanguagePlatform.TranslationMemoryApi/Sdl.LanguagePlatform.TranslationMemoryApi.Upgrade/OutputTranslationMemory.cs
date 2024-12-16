using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class OutputTranslationMemory : IOutputTranslationMemory
	{
		private class FieldNameComparer : IEqualityComparer<FieldDefinition>
		{
			public bool Equals(FieldDefinition x, FieldDefinition y)
			{
				return x.Name == y.Name;
			}

			public int GetHashCode(FieldDefinition obj)
			{
				return obj.Name.GetHashCode();
			}
		}

		private readonly InputLanguagePairCollection _inputLanguagePairCollection;

		private readonly TranslationMemorySetupOptions _setup;

		private ITranslationMemory _translationMemory;

		public IInputLanguageDirectionDataCollection InputLanguageDirections => _inputLanguagePairCollection;

		public ITranslationMemoryCreator TranslationMemoryCreator
		{
			get;
			set;
		}

		public ITranslationMemorySetupOptions Setup => _setup;

		public int ExpectedTUCount
		{
			get
			{
				int num = -1;
				foreach (IInputLanguageDirectionData item in _inputLanguagePairCollection)
				{
					int translationUnitCount = item.LanguageDirectionData.TranslationUnitCount;
					if (translationUnitCount != -1)
					{
						num = ((num != -1) ? (num + translationUnitCount) : translationUnitCount);
					}
				}
				return num;
			}
		}

		public ITranslationMemory TranslationMemory => _translationMemory;

		public OutputTranslationMemory()
		{
			_inputLanguagePairCollection = new InputLanguagePairCollection();
			_inputLanguagePairCollection.ItemAdded += _inputLanguagePairCollection_ItemAdded;
			_inputLanguagePairCollection.ItemRemoved += _inputLanguagePairCollection_ItemRemoved;
			_setup = new TranslationMemorySetupOptions();
		}

		private void _inputLanguagePairCollection_ItemRemoved(object sender, ItemCollectionRemovedEventArgs<IInputLanguageDirectionData> e)
		{
		}

		private void _inputLanguagePairCollection_ItemAdded(object sender, ItemCollectionAddedEventArgs<IInputLanguageDirectionData> e)
		{
			AddLanguageDirection(e.Item);
		}

		private void AddLanguageDirection(IInputLanguageDirectionData languageDirectionData)
		{
			LanguagePair languagePair = new LanguagePair(languageDirectionData.LanguageDirectionData.SourceLanguage.Culture, languageDirectionData.LanguageDirectionData.TargetLanguage.Culture);
			if (Setup.LanguageDirections.FirstOrDefault((LanguagePair lp) => lp.Equals(languagePair)) == null)
			{
				Setup.LanguageDirections.Add(languagePair);
			}
		}

		public void PopulateSetup()
		{
			ValidateInputLanguagePairs();
			ILegacyTranslationMemorySetup setup = InputLanguageDirections[0].TranslationMemory.Setup;
			Setup.Name = setup.Name;
			Setup.Copyright = setup.Copyright;
			Setup.Description = setup.Description;
			Setup.ExpirationDate = setup.ExpirationDate;
			Setup.FuzzyIndexes = setup.FuzzyIndexes;
			Setup.WordCountFlags = setup.WordCountFlags;
			Setup.TokenizerFlags = setup.TokenizerFlags;
			Setup.FGASupport = setup.FGASupport;
			Setup.LanguageDirections.Clear();
			foreach (IInputLanguageDirectionData inputLanguageDirection in InputLanguageDirections)
			{
				AddLanguageDirection(inputLanguageDirection);
			}
			Setup.Recognizers = GetDefaultRecognizers(InputLanguageDirections);
			Setup.InputFieldIdentifierMappings = GetInputFieldIdentifierMappings(InputLanguageDirections);
			Setup.Fields = GetFields(InputLanguageDirections, Setup.InputFieldIdentifierMappings);
			Setup.LanguageResources = GetLanguageResources(InputLanguageDirections);
			Setup.UsesIdContextMatch = setup.UsesIdContextMatch;
			Setup.TextContextMatchType = ((setup.TextContextMatchType == (TextContextMatchType)0) ? TextContextMatchType.PrecedingSourceAndTarget : setup.TextContextMatchType);
		}

		private IDictionary<CultureInfo, ILegacyLanguageResources> GetLanguageResources(IInputLanguageDirectionDataCollection inputLanguageDirections)
		{
			IDictionary<CultureInfo, ILegacyLanguageResources> dictionary = new Dictionary<CultureInfo, ILegacyLanguageResources>();
			foreach (IInputLanguageDirectionData inputLanguageDirection in inputLanguageDirections)
			{
				if (!dictionary.ContainsKey(inputLanguageDirection.LanguageDirectionData.SourceLanguage.Culture) && inputLanguageDirection.LanguageDirectionData.SuggestedLanguageResources != null)
				{
					dictionary[inputLanguageDirection.LanguageDirectionData.SourceLanguage.Culture] = inputLanguageDirection.LanguageDirectionData.SuggestedLanguageResources;
				}
			}
			return dictionary;
		}

		private IList<FieldDefinition> GetFields(IInputLanguageDirectionDataCollection inputLanguagePairs, IFieldIdentifierMappingsCollection inputFieldNameMappings)
		{
			List<FieldDefinition> list = new List<FieldDefinition>();
			foreach (IInputLanguageDirectionData inputLanguagePair in inputLanguagePairs)
			{
				IInputTranslationMemory translationMemory = inputLanguagePair.TranslationMemory;
				IList<FieldDefinition> fields = translationMemory.Setup.Fields;
				IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings = inputFieldNameMappings.GetFieldIdentifierMappings(translationMemory);
				FieldsMerger.MergeFieldLists(list, fields, fieldIdentifierMappings);
			}
			return list;
		}

		private BuiltinRecognizers GetDefaultRecognizers(IInputLanguageDirectionDataCollection inputLanguagePairs)
		{
			int num = 127;
			foreach (IInputLanguageDirectionData inputLanguagePair in inputLanguagePairs)
			{
				IInputTranslationMemory translationMemory = inputLanguagePair.TranslationMemory;
				int num2 = (int)translationMemory.Setup.Recognizers;
				if (translationMemory.TranslationMemory != null && translationMemory.TranslationMemory is TmxLegacyTranslationMemory && num2 == 0)
				{
					num2 = 127;
				}
				num &= num2;
			}
			return (BuiltinRecognizers)num;
		}

		private IFieldIdentifierMappingsCollection GetInputFieldIdentifierMappings(IInputLanguageDirectionDataCollection inputLanguagePairs)
		{
			IInputTranslationMemoryCollection inputTranslationMemories = GetInputTranslationMemories(inputLanguagePairs);
			IFieldIdentifierMappingsCollection fieldIdentifierMappingsCollection = new InputFieldIdentifierMappingsCollection();
			foreach (IInputTranslationMemory item2 in inputTranslationMemories)
			{
				IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings = GetFieldIdentifierMappings(item2.Setup.Fields);
				IFieldIdentifierMappings item = new InputFieldIdentifierMappings(item2, fieldIdentifierMappings);
				fieldIdentifierMappingsCollection.Add(item);
			}
			MapSingleMultipleFieldsToMultipleFields(fieldIdentifierMappingsCollection);
			return fieldIdentifierMappingsCollection;
		}

		private IInputTranslationMemoryCollection GetInputTranslationMemories(IInputLanguageDirectionDataCollection inputLanguagePairs)
		{
			IInputTranslationMemoryCollection inputTranslationMemoryCollection = new InputTranslationMemoryCollection();
			foreach (IInputLanguageDirectionData inputLanguagePair in inputLanguagePairs)
			{
				IInputTranslationMemory translationMemory = inputLanguagePair.TranslationMemory;
				if (!inputTranslationMemoryCollection.Contains(translationMemory))
				{
					inputTranslationMemoryCollection.Add(translationMemory);
				}
			}
			return inputTranslationMemoryCollection;
		}

		private IDictionary<FieldIdentifier, FieldIdentifier> GetFieldIdentifierMappings(IList<FieldDefinition> fields)
		{
			Dictionary<FieldIdentifier, FieldIdentifier> dictionary = new Dictionary<FieldIdentifier, FieldIdentifier>();
			IList<string> list = new List<string>();
			foreach (FieldDefinition field in fields)
			{
				FieldValueType valueType = field.ValueType;
				string name = field.Name;
				FieldIdentifier key = new FieldIdentifier(valueType, name);
				if (name != null)
				{
					string text = Field.RemoveIllegalChars(name);
					if (text.Length == 0)
					{
						text = StringResources.OutputTranslationMemory_DefaultFieldName;
					}
					string str = text;
					int num = 2;
					while (list.Contains(text, StringComparer.OrdinalIgnoreCase))
					{
						text = str + "_" + num.ToString();
						num++;
					}
					dictionary[key] = new FieldIdentifier(valueType, text);
					list.Add(text);
				}
			}
			return dictionary;
		}

		private void MapSingleMultipleFieldsToMultipleFields(IFieldIdentifierMappingsCollection inputFieldIdentifierMappingsCollection)
		{
			IDictionary<string, IList<FieldValueType>> nameTypes = GetNameTypes(inputFieldIdentifierMappingsCollection);
			foreach (KeyValuePair<string, IList<FieldValueType>> item in nameTypes)
			{
				string key = item.Key;
				IList<FieldValueType> value = item.Value;
				if (value.Contains(FieldValueType.SingleString) && value.Contains(FieldValueType.MultipleString))
				{
					ChangeFieldValueType(inputFieldIdentifierMappingsCollection, key, FieldValueType.SingleString, FieldValueType.MultipleString);
				}
				if (value.Contains(FieldValueType.SinglePicklist) && value.Contains(FieldValueType.MultiplePicklist))
				{
					ChangeFieldValueType(inputFieldIdentifierMappingsCollection, key, FieldValueType.SinglePicklist, FieldValueType.MultiplePicklist);
				}
			}
		}

		private IDictionary<string, IList<FieldValueType>> GetNameTypes(IFieldIdentifierMappingsCollection inputFieldIdentifierMappingsCollection)
		{
			IDictionary<string, IList<FieldValueType>> dictionary = new Dictionary<string, IList<FieldValueType>>();
			foreach (IFieldIdentifierMappings item in inputFieldIdentifierMappingsCollection)
			{
				IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings = item.FieldIdentifierMappings;
				foreach (FieldIdentifier key in fieldIdentifierMappings.Keys)
				{
					string fieldName = key.FieldName;
					FieldValueType fieldValueType = key.FieldValueType;
					if (!dictionary.TryGetValue(fieldName, out IList<FieldValueType> value))
					{
						value = new List<FieldValueType>();
						dictionary.Add(fieldName, value);
					}
					if (!value.Contains(fieldValueType))
					{
						value.Add(fieldValueType);
					}
				}
			}
			return dictionary;
		}

		private void ChangeFieldValueType(IFieldIdentifierMappingsCollection inputFieldIdentifierMappingsCollection, string name, FieldValueType fromType, FieldValueType toType)
		{
			foreach (IFieldIdentifierMappings item in inputFieldIdentifierMappingsCollection)
			{
				IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings = item.FieldIdentifierMappings;
				foreach (KeyValuePair<FieldIdentifier, FieldIdentifier> item2 in fieldIdentifierMappings)
				{
					FieldIdentifier key = item2.Key;
					FieldIdentifier value = item2.Value;
					if (object.Equals(key.FieldName, name) && object.Equals(key.FieldValueType, fromType) && object.Equals(value.FieldName, name) && object.Equals(value.FieldValueType, fromType))
					{
						value.FieldValueType = toType;
					}
				}
			}
		}

		public void Validate()
		{
			if (!IsValid(out string errorMessage))
			{
				throw new InvalidOperationException(errorMessage);
			}
		}

		public bool IsValid(out string errorMessage)
		{
			if (TranslationMemoryCreator == null)
			{
				errorMessage = StringResources.OutputTranslationMemory_ErrorMessage_LocationNotSpecified;
				return false;
			}
			if (!TranslationMemoryCreator.IsValid(out string errorMessage2))
			{
				errorMessage = errorMessage2;
				return false;
			}
			if (!TranslationMemoryCreator.IsValidName(Setup.Name, out string errorMessage3))
			{
				errorMessage = errorMessage3;
				return false;
			}
			if (!TranslationMemoryCreator.IsValidCopyright(Setup.Copyright, out string errorMessage4))
			{
				errorMessage = errorMessage4;
				return false;
			}
			if (!TranslationMemoryCreator.IsValidDescription(Setup.Description, out string errorMessage5))
			{
				errorMessage = errorMessage5;
				return false;
			}
			if (!IsInputLanguagePairsValid(out string errorMessage6))
			{
				errorMessage = errorMessage6;
				return false;
			}
			int num = Setup.Fields.Distinct(new FieldNameComparer()).Count();
			if (num != Setup.Fields.Count)
			{
				errorMessage = StringResources.OutputTranslationMemory_ErrorMessage_FieldConflicts;
				return false;
			}
			errorMessage = null;
			return true;
		}

		private void ValidateInputLanguagePairs()
		{
			if (!IsInputLanguagePairsValid(out string errorMessage))
			{
				throw new InvalidOperationException(errorMessage);
			}
		}

		private bool IsInputLanguagePairsValid(out string errorMessage)
		{
			if (InputLanguageDirections.Count == 0)
			{
				errorMessage = StringResources.OutputTranslationMemory_ErrorMessage_InputLanguagePairsNotSpecified;
				return false;
			}
			errorMessage = null;
			return true;
		}

		public void Process(EventHandler<ProgressEventArgs> progressEventHandler)
		{
			Validate();
			foreach (IInputLanguageDirectionData item in _inputLanguagePairCollection)
			{
				if (item.TranslationMemory.TmxFileStatus != TmxFileStatus.Available)
				{
					throw new InvalidOperationException(string.Format(StringResources.OutputTranslationMemory_ErrorMessage_TMXNotAvailable, item.TranslationMemory.TranslationMemory.Url));
				}
			}
			if (_translationMemory == null)
			{
				if (!ReportProgress(progressEventHandler, 0, string.Format(StringResources.OutputTranslationMemory_ProgressMessage_CreatingTranslationMemory, Setup.Name)))
				{
					return;
				}
				CreateTranslationMemory();
			}
			int languagePairsToImport = _inputLanguagePairCollection.Count((IInputLanguageDirectionData p) => !p.IsImportComplete);
			bool cancelled = false;
			double percentComplete = 0.0;
			double num = 100.0 / (double)languagePairsToImport;
			foreach (IInputLanguageDirectionData item2 in _inputLanguagePairCollection)
			{
				if (!item2.IsImportComplete)
				{
					Import(item2, delegate(object sender, ProgressEventArgs e)
					{
						int percent = (int)(percentComplete + (double)e.PercentComplete / (double)languagePairsToImport);
						if (!ReportProgress(progressEventHandler, percent, e.InfoMessage))
						{
							e.Cancel = true;
							cancelled = true;
						}
					});
					if (cancelled)
					{
						return;
					}
					percentComplete += num;
				}
			}
			ReportProgress(progressEventHandler, 100, StringResources.OutputTranslationMemory_ProgressMessage_Completed);
		}

		public void CreateTranslationMemory()
		{
			if (_translationMemory == null)
			{
				Validate();
				_translationMemory = TranslationMemoryCreator.CreateEmptyTranslationMemory(Setup);
			}
		}

		private bool ReportProgress(EventHandler<ProgressEventArgs> progressEventHandler, int percent, string message)
		{
			if (progressEventHandler == null)
			{
				return true;
			}
			ProgressEventArgs progressEventArgs = new ProgressEventArgs(percent, message);
			progressEventHandler(this, progressEventArgs);
			return !progressEventArgs.Cancel;
		}

		public void Import(IInputLanguageDirectionData inputLanguageDirection, EventHandler<ProgressEventArgs> progressEventHandler)
		{
			if (_translationMemory != null)
			{
				LanguagePair mappedLanguagePair = GetMappedLanguagePair(new LanguagePair(inputLanguageDirection.LanguageDirectionData.SourceLanguage.Culture, inputLanguageDirection.LanguageDirectionData.TargetLanguage.Culture));
				ImportSettings importSettings = new ImportSettings
				{
					CheckMatchingSublanguages = true,
					IncrementUsageCount = false,
					IsDocumentImport = false,
					FieldIdentifierMappings = Setup.InputFieldIdentifierMappings.GetFieldIdentifierMappings(inputLanguageDirection.TranslationMemory),
					ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge,
					ExistingTUsUpdateMode = inputLanguageDirection.ImportTuUpdateModeSettings,
					InvalidTranslationUnitsExportPath = ((!string.IsNullOrEmpty(inputLanguageDirection.InvalidTranslationUnitsExportPath)) ? inputLanguageDirection.InvalidTranslationUnitsExportPath : GenerateErrorTMXPath(inputLanguageDirection.TranslationMemory.TmxFilePath, mappedLanguagePair)),
					PlainText = inputLanguageDirection.ImportPlainText,
					TUProcessingMode = inputLanguageDirection.ImportCompatibilitySettings,
					NewFields = ImportSettings.NewFieldsOption.Ignore
				};
				ResetImportStatistics(inputLanguageDirection.ImportStatistics);
				ITranslationMemoryLanguageDirection languageDirection = _translationMemory.GetLanguageDirection(mappedLanguagePair);
				TranslationMemoryImporter translationMemoryImporter = new TranslationMemoryImporter(languageDirection)
				{
					ImportSettings = importSettings
				};
				bool cancelled = false;
				translationMemoryImporter.BatchImported += delegate(object sender, BatchImportedEventArgs e)
				{
					AssignImportStatistics(inputLanguageDirection.ImportStatistics, e.Statistics);
					if (progressEventHandler != null)
					{
						int percentComplete;
						string infoMessage;
						if (inputLanguageDirection.LanguageDirectionData.TranslationUnitCount < 0)
						{
							percentComplete = -1;
							infoMessage = string.Format(StringResources.OutputTranslationMemory_ProgressMessage_Importing, inputLanguageDirection.TranslationMemory.TranslationMemory.Url, e.Statistics.TotalRead, e.Statistics.TotalImported, e.Statistics.Errors);
						}
						else if (inputLanguageDirection.LanguageDirectionData.TranslationUnitCount == 0)
						{
							percentComplete = 0;
							infoMessage = string.Format(StringResources.OutputTranslationMemory_ProgressMessage_Importing, inputLanguageDirection.TranslationMemory.TranslationMemory.Url, e.Statistics.TotalRead, e.Statistics.TotalImported, e.Statistics.Errors);
						}
						else
						{
							percentComplete = e.Statistics.TotalRead * 100 / inputLanguageDirection.LanguageDirectionData.TranslationUnitCount;
							infoMessage = string.Format(StringResources.OutputTranslationMemory_ProgressMessage_ImportingWithTotal, inputLanguageDirection.TranslationMemory.TranslationMemory.Url, e.Statistics.TotalRead, e.Statistics.TotalImported, e.Statistics.Errors, inputLanguageDirection.LanguageDirectionData.TranslationUnitCount);
						}
						ProgressEventArgs progressEventArgs = new ProgressEventArgs(percentComplete, infoMessage);
						progressEventHandler(this, progressEventArgs);
						if (progressEventArgs.Cancel)
						{
							cancelled = true;
							e.Cancel = true;
						}
					}
				};
				translationMemoryImporter.Import(inputLanguageDirection.TranslationMemory.TmxFilePath);
				AssignImportStatistics(inputLanguageDirection.ImportStatistics, translationMemoryImporter.Statistics);
				if (!cancelled)
				{
					((InputLanguagePair)inputLanguageDirection).IsImportComplete = true;
				}
			}
		}

		private LanguagePair GetMappedLanguagePair(LanguagePair languagePair)
		{
			CultureInfo mappedCultureInfo = GetMappedCultureInfo(languagePair.SourceCulture);
			CultureInfo mappedCultureInfo2 = GetMappedCultureInfo(languagePair.TargetCulture);
			return new LanguagePair(mappedCultureInfo, mappedCultureInfo2);
		}

		private CultureInfo GetMappedCultureInfo(CultureInfo cultureInfo)
		{
			return CultureInfo.GetCultureInfo(cultureInfo.Name);
		}

		private static void ResetImportStatistics(ImportStatistics s)
		{
			s.AddedTranslationUnits = 0;
			s.DiscardedTranslationUnits = 0;
			s.Errors = 0;
			s.MergedTranslationUnits = 0;
			s.OverwrittenTranslationUnits = 0;
			s.RawTUs = 0;
			s.TotalRead = 0;
		}

		private static void AssignImportStatistics(ImportStatistics to, ImportStatistics from)
		{
			if (to != null && from != null)
			{
				to.AddedTranslationUnits = from.AddedTranslationUnits;
				to.DiscardedTranslationUnits = from.DiscardedTranslationUnits;
				to.Errors = from.Errors;
				to.MergedTranslationUnits = from.MergedTranslationUnits;
				to.OverwrittenTranslationUnits = from.OverwrittenTranslationUnits;
				to.RawTUs = from.RawTUs;
				to.TotalRead = from.TotalRead;
			}
		}

		private string GenerateErrorTMXPath(string importFile, LanguagePair languageDirection)
		{
			FileInfo fileInfo = new FileInfo(importFile);
			StringBuilder stringBuilder = new StringBuilder(fileInfo.Directory.FullName);
			stringBuilder.Append(Path.DirectorySeparatorChar);
			stringBuilder.Append(Path.GetFileNameWithoutExtension(fileInfo.Name));
			stringBuilder.Append("_");
			stringBuilder.Append(languageDirection.SourceCultureName);
			stringBuilder.Append("_");
			stringBuilder.Append(languageDirection.TargetCultureName);
			stringBuilder.Append("_errors.tmx");
			return stringBuilder.ToString();
		}
	}
}
