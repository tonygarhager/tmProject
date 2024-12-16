using ICSharpCode.SharpZipLib.Zip;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.IO.TMX;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sdl.Core.TM.ImportExport
{
	public class Importer
	{
		public const int DefaultTranslationUnitChunkSize = 25;

		public const int MaxTranslationUnitChunkSize = 200;

		private IFileTypeManager _defaultFileTypeManager;

		protected readonly List<TranslationUnit> Buffer = new List<TranslationUnit>();

		protected TMXWriter ErrorOutput;

		protected int BadTuCount;

		private TMXStartOfInputEvent _startOfInputEvent;

		private ImportSettings _importSettings;

		private int _chunkSize = 25;

		private string _invalidTranslationUnitsExportPath;

		private FileBasedTM _conversionTm;

		public ImportStatistics Statistics
		{
			get;
			set;
		}

		public IFileTypeManager FileTypeManager
		{
			get;
			set;
		}

		public bool CanChangeImportFile
		{
			get;
			set;
		}

		public string TranslationMemoryUserIdSetting
		{
			get;
			set;
		}

		private TranslationUnit LastTuInBatch
		{
			get;
			set;
		}

		private IImportDestination ImportDestination
		{
			get;
			set;
		}

		public int ChunkSize
		{
			get
			{
				return _chunkSize;
			}
			set
			{
				if (value <= 0)
				{
					_chunkSize = 25;
				}
				else if (value > 200)
				{
					_chunkSize = 200;
				}
				else
				{
					_chunkSize = value;
				}
			}
		}

		public ImportSettings ImportSettings
		{
			get
			{
				ImportSettings importSettings = _importSettings;
				if (importSettings == null)
				{
					ImportSettings obj = new ImportSettings
					{
						IsDocumentImport = false,
						CheckMatchingSublanguages = false,
						IncrementUsageCount = false,
						NewFields = ImportSettings.NewFieldsOption.Ignore,
						PlainText = false,
						InvalidTranslationUnitsExportPath = InvalidTranslationUnitsExportPath
					};
					ImportSettings importSettings2 = obj;
					_importSettings = obj;
					importSettings = importSettings2;
				}
				return importSettings;
			}
			set
			{
				_importSettings = value;
				if (!string.IsNullOrEmpty(_importSettings?.InvalidTranslationUnitsExportPath))
				{
					InvalidTranslationUnitsExportPath = _importSettings.InvalidTranslationUnitsExportPath;
				}
			}
		}

		public string InvalidTranslationUnitsExportPath
		{
			get
			{
				return _invalidTranslationUnitsExportPath;
			}
			set
			{
				_invalidTranslationUnitsExportPath = value;
				if (!string.IsNullOrEmpty(_invalidTranslationUnitsExportPath) && _importSettings != null)
				{
					_importSettings.InvalidTranslationUnitsExportPath = _invalidTranslationUnitsExportPath;
				}
			}
		}

		private IImportDestination ActualImportDestination
		{
			get
			{
				IImportDestination conversionTm = _conversionTm;
				return conversionTm ?? ImportDestination;
			}
		}

		public event EventHandler<BatchImportedEventArgs> BatchImported;

		public Importer()
		{
		}

		public Importer(IImportDestination importDestination)
		{
			ImportDestination = importDestination;
		}

		public bool Import(string fileName)
		{
			return Import(fileName, ImportDestination);
		}

		public bool Import(string fileName, IImportDestination importDestination)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			FileInfo fileInfo = new FileInfo(fileName);
			ImportDestination = importDestination;
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException(fileInfo.FullName);
			}
			if (ImportDestination == null)
			{
				throw new InvalidOperationException("ImportDestination cannot be null.");
			}
			LastTuInBatch = null;
			switch (Path.GetExtension(fileInfo.Name).TrimStart('.').ToLower())
			{
			case "tmx":
			case "gz":
				ImportTmxFile(fileName);
				break;
			case "zip":
				ImportTmxArchive(fileName);
				break;
			case "sdltm":
				ImportSdltmFile(fileName, CanChangeImportFile);
				break;
			default:
				ImportBilingualFile(fileName);
				break;
			}
			return true;
		}

		public IFileTypeManager GetDefaultFileTypeManager()
		{
			return _defaultFileTypeManager ?? (_defaultFileTypeManager = DefaultFileTypeManager.CreateInstance(autoLoadFileTypes: true));
		}

		private void ImportTmxFile(string fileName)
		{
			TMXReaderSettings readerSettings = CreateTmxReaderSettings();
			_importSettings.IsDocumentImport = false;
			using (TMXReader reader = new TMXReader(fileName, readerSettings))
			{
				Statistics = ImportTmxFile(reader);
			}
			if (_conversionTm != null)
			{
				UpgradeTempFileAndImport();
			}
		}

		private void ImportTmxArchive(string fileName)
		{
			TMXReaderSettings readerSettings = CreateTmxReaderSettings();
			_importSettings.IsDocumentImport = false;
			using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(fileName)))
			{
				zipInputStream.GetNextEntry();
				using (TMXReader reader = new TMXReader(new StreamReader(zipInputStream), readerSettings))
				{
					Statistics = ImportTmxFile(reader);
				}
			}
			if (_conversionTm != null)
			{
				UpgradeTempFileAndImport();
			}
		}

		private TMXReaderSettings CreateTmxReaderSettings()
		{
			TMXReaderSettings tMXReaderSettings = new TMXReaderSettings(new TUStreamContext(new LanguagePair(ImportDestination.SourceLanguage, ImportDestination.TargetLanguage), ImportDestination.FieldDefinitions), validateAgainstSchema: true, resolveNeutralCultures: true, plainText: false)
			{
				SkipUnknownCultures = true,
				ValidateAgainstSchema = false,
				PlainText = ImportSettings.PlainText,
				CleanupMode = ImportSettings.TUProcessingMode
			};
			if (ImportSettings.NewFields == ImportSettings.NewFieldsOption.Ignore)
			{
				tMXReaderSettings.Context.MayAddNewFields = false;
			}
			tMXReaderSettings.FieldIdentifierMappings = ImportSettings.FieldIdentifierMappings;
			return tMXReaderSettings;
		}

		private ImportStatistics ImportTmxFile(TMXReader reader)
		{
			ImportStatistics importStatistics = new ImportStatistics();
			Event @event;
			while ((@event = reader.Read()) != null)
			{
				importStatistics.RawTUs = reader.RawTUsRead;
				StartOfInputEvent startOfInputEvent = @event as StartOfInputEvent;
				if (startOfInputEvent != null)
				{
					ProcessStartOfInput(startOfInputEvent, reader);
					continue;
				}
				TUEvent tUEvent = @event as TUEvent;
				if (tUEvent != null)
				{
					if (tUEvent.TranslationUnit.ConfirmationLevel == ConfirmationLevel.Unspecified)
					{
						tUEvent.TranslationUnit.ConfirmationLevel = ConfirmationLevel.ApprovedSignOff;
					}
					if (ImportTranslationUnit(tUEvent.TranslationUnit, importStatistics) == ImportExportResponse.Cancel)
					{
						break;
					}
				}
			}
			Flush(importStatistics);
			return importStatistics;
		}

		private void ProcessStartOfInput(StartOfInputEvent e, TMXReader reader)
		{
			_startOfInputEvent = (e as TMXStartOfInputEvent);
			if (_startOfInputEvent != null && reader.Flavor == TranslationUnitFormat.SDLTradosStudio2009 && !ImportDestination.UsesLegacyHashes && !_startOfInputEvent.IncludesContextContent)
			{
				string tempFileName = Path.GetTempFileName();
				_conversionTm = new FileBasedTM(tempFileName, "ConversionTM", ImportDestination.SourceLanguage, ImportDestination.TargetLanguage, FuzzyIndexes.SourceWordBased, _startOfInputEvent.BuiltinRecognizers, _startOfInputEvent.TokenizerFlags, _startOfInputEvent.WordCountFlags, fgaSupport: false, _startOfInputEvent.TextContextMatchType, _startOfInputEvent.UsesIdContextMatch, usesLegacyHashes: true);
				_conversionTm.UpdateFieldDefinitions(ImportDestination.FieldDefinitions);
				_conversionTm.UpdateLanguageResources(new List<LanguageResource>(ImportDestination.LanguageResources));
			}
			if (_startOfInputEvent?.Fields != null && ImportSettings.NewFields == ImportSettings.NewFieldsOption.AddToSetup)
			{
				UpdateFieldDefinitions(_startOfInputEvent.Fields, throwOnError: false);
			}
		}

		private void ImportSdltmFile(string fileName, bool upgradePermitted = false)
		{
			_importSettings.IsDocumentImport = false;
			if (upgradePermitted)
			{
				UpgradeTmIfNeeded(fileName);
			}
			LastTuInBatch = null;
			IExportOrigin exportOrigin = new FileBasedTM(fileName);
			CheckLanguageDirections(exportOrigin.SourceLanguage, exportOrigin.TargetLanguage, out bool languageDirectionCompatible);
			if (!languageDirectionCompatible)
			{
				throw new LanguageMismatchException(exportOrigin.SourceLanguage, exportOrigin.TargetLanguage, ImportDestination.SourceLanguage, ImportDestination.TargetLanguage);
			}
			FieldDefinitions fieldDefinitions = exportOrigin.FieldDefinitions;
			if (fieldDefinitions != null)
			{
				UpdateFieldDefinitions(fieldDefinitions, throwOnError: false);
			}
			LanguageResource[] languageResources = exportOrigin.LanguageResources;
			if (languageResources != null)
			{
				UpdateLanguageResources(languageResources);
			}
			RegularIterator iter = new RegularIterator
			{
				MaxCount = 200,
				Forward = true
			};
			ImportStatistics importStatistics = new ImportStatistics();
			TranslationUnit[] translationUnits;
			do
			{
				translationUnits = exportOrigin.GetTranslationUnits(ref iter);
				importStatistics.RawTUs += translationUnits.Length;
			}
			while (ImportTranslationUnit(translationUnits, importStatistics) != ImportExportResponse.Cancel && iter.ScannedTranslationUnits > 0);
			Flush(importStatistics);
			Statistics = importStatistics;
		}

		private ImportExportResponse ImportTranslationUnit(IList<TranslationUnit> translationUnits, ImportStatistics stats)
		{
			if (ImportSettings.EditScript != null)
			{
				EditScriptApplier.Apply(ImportSettings.EditScript, translationUnits);
			}
			Buffer.AddRange(translationUnits);
			foreach (TranslationUnit item in Buffer)
			{
				item.ResourceId = new PersistentObjectToken();
				if (item.ConfirmationLevel == ConfirmationLevel.Unspecified)
				{
					item.ConfirmationLevel = ConfirmationLevel.ApprovedSignOff;
				}
			}
			if (Buffer.Count < ChunkSize)
			{
				return ImportExportResponse.Continue;
			}
			return ProcessBufferedTUs(stats);
		}

		private void UpdateLanguageResources(IEnumerable<LanguageResource> languageResources)
		{
			List<LanguageResource> source = ImportDestination.LanguageResources.ToList();
			List<LanguageResource> list = new List<LanguageResource>();
			foreach (LanguageResource resource in languageResources)
			{
				if (resource.Type == LanguageResourceType.Variables || resource.Type == LanguageResourceType.Abbreviations || resource.Type == LanguageResourceType.OrdinalFollowers)
				{
					if (source.Any((LanguageResource x) => x.Type == resource.Type))
					{
						LanguageResource languageResource = source.FirstOrDefault((LanguageResource x) => x.Type == resource.Type);
						if (languageResource != null)
						{
							Wordlist wordlist = new Wordlist();
							Wordlist wordlist2 = new Wordlist();
							wordlist.Load(languageResource.Data);
							wordlist2.Load(resource.Data);
							wordlist2.Merge(wordlist);
							languageResource.Data = wordlist2.GetBytes();
							list.Add(languageResource);
						}
					}
					else
					{
						LanguageResource item = new LanguageResource
						{
							Type = resource.Type,
							CultureName = resource.CultureName,
							Data = resource.Data
						};
						list.Add(item);
					}
				}
			}
			ActualImportDestination.UpdateLanguageResources(list);
		}

		private void ImportBilingualFile(string fileName)
		{
			IMultiFileConverter converter = (FileTypeManager ?? GetDefaultFileTypeManager()).GetConverter(new string[1]
			{
				fileName
			}, CultureInfo.InvariantCulture, new Codepage("utf-8"), null);
			BilingualContentImporter bilingualContentImporter = new BilingualContentImporter(this, ImportDestination.SourceLanguage, ImportDestination.TargetLanguage);
			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(bilingualContentImporter));
			converter.Parse();
			Statistics = bilingualContentImporter.ImportStatistics;
		}

		internal ImportExportResponse AddTranslationUnitToBuffer(TranslationUnit translationUnit, ImportStatistics stats)
		{
			CheckLanguageDirections(translationUnit.SourceSegment.Culture, translationUnit.TargetSegment.Culture, out bool languageDirectionCompatible);
			if (!languageDirectionCompatible)
			{
				stats.DiscardedTranslationUnits++;
				stats.TotalRead++;
				return ImportExportResponse.Continue;
			}
			if (ImportSettings.EditScript != null)
			{
				EditScriptApplier.Apply(ImportSettings.EditScript, translationUnit);
			}
			Buffer.Add(translationUnit);
			return ImportExportResponse.Continue;
		}

		internal ImportExportResponse ImportTranslationUnit(TranslationUnit translationUnit, ImportStatistics stats)
		{
			AddTranslationUnitToBuffer(translationUnit, stats);
			if (Buffer.Count < ChunkSize)
			{
				return ImportExportResponse.Continue;
			}
			return ProcessBufferedTUs(stats);
		}

		internal bool CheckLanguageDirections(CultureInfo sourceLang, CultureInfo targetLang, out bool languageDirectionCompatible, bool allowReverse = false)
		{
			if (sourceLang == null)
			{
				throw new ArgumentNullException("sourceLang");
			}
			if (targetLang == null)
			{
				throw new ArgumentNullException("targetLang");
			}
			LanguagePair languagePair = new LanguagePair(sourceLang, targetLang);
			LanguagePair languagePair2 = new LanguagePair(ImportDestination.SourceLanguage, ImportDestination.TargetLanguage);
			languageDirectionCompatible = (_importSettings.CheckMatchingSublanguages ? languagePair2.Equals(languagePair) : languagePair2.IsCompatible(languagePair));
			if (languageDirectionCompatible || !allowReverse)
			{
				return false;
			}
			languagePair = new LanguagePair(targetLang, sourceLang);
			languageDirectionCompatible = (_importSettings.CheckMatchingSublanguages ? languagePair2.Equals(languagePair) : languagePair2.IsCompatible(languagePair));
			return true;
		}

		protected internal ImportExportResponse Flush(ImportStatistics stats)
		{
			ImportExportResponse result = ImportExportResponse.Continue;
			if (Buffer.Count > 0)
			{
				result = ProcessBufferedTUs(stats);
			}
			try
			{
				if (ErrorOutput == null)
				{
					return result;
				}
				ErrorOutput.Emit(new EndOfInputEvent());
				ErrorOutput.Close();
				ErrorOutput.Dispose();
				ErrorOutput = null;
				return result;
			}
			finally
			{
				ErrorOutput = null;
			}
		}

		protected void OutputErrorTu(TranslationUnit tu, ImportResult result)
		{
			if (ImportSettings.InvalidTranslationUnitsExportPath != null)
			{
				if (ErrorOutput == null)
				{
					ErrorOutput = new TMXWriter(ImportSettings.InvalidTranslationUnitsExportPath);
					ErrorOutput.Emit(_startOfInputEvent ?? new StartOfInputEvent());
				}
				ErrorOutput.Emit(new CommentEvent(string.Format(CultureInfo.InvariantCulture, "Error: {0}", new object[1]
				{
					result.ErrorCode.ToString()
				})));
				ErrorOutput.Emit(new TUEvent(tu));
			}
		}

		protected ImportExportResponse OnBatchImported(ImportResults batchResults, ImportStatistics overallResults)
		{
			EventHandler<BatchImportedEventArgs> batchImported = this.BatchImported;
			if (batchImported == null)
			{
				return ImportExportResponse.Continue;
			}
			BatchImportedEventArgs batchImportedEventArgs = new BatchImportedEventArgs(overallResults)
			{
				BatchResults = batchResults
			};
			batchImported(this, batchImportedEventArgs);
			if (!batchImportedEventArgs.Cancel)
			{
				return ImportExportResponse.Continue;
			}
			return ImportExportResponse.Cancel;
		}

		private void UpdateFieldDefinitions(FieldDefinitions fields, bool throwOnError)
		{
			FieldDefinitions fieldDefinitions = ActualImportDestination.FieldDefinitions;
			foreach (Field field in fields.Fields)
			{
				HandleFieldDefinition(fieldDefinitions, field, throwOnError);
			}
			ActualImportDestination.UpdateFieldDefinitions(fieldDefinitions);
		}

		private static void HandleFieldDefinition(FieldDefinitions tmFields, Field field, bool throwOnError)
		{
			if (!Field.IsValidName(field.Name))
			{
				field.Name = Field.RemoveIllegalChars(field.Name);
			}
			if (Field.IsSystemFieldName(field.Name))
			{
				return;
			}
			Field field2 = tmFields[field.Name];
			if (field2 == null)
			{
				Field field3 = CreateFieldDefinition(field.ValueType, field.Name);
				tmFields.Add(field3);
				field2 = field3;
			}
			else if (field2.ValueType != field.ValueType)
			{
				if (throwOnError)
				{
					throw new LanguagePlatformException(ErrorCode.TMImportIncompatibleFieldTypes, string.Format(CultureInfo.InvariantCulture, "{0} ({1}, {2})", new object[3]
					{
						field.Name,
						field.ValueType,
						field2.ValueType
					}));
				}
				return;
			}
			if (field.ValueType == FieldValueType.SinglePicklist || field.ValueType == FieldValueType.MultiplePicklist)
			{
				HandlePickListField(field, field2, throwOnError);
			}
		}

		private static void HandlePickListField(Field field, IField templateField, bool throwOnError)
		{
			PicklistField picklistField = field as PicklistField;
			if (picklistField != null)
			{
				PicklistField picklistField2 = templateField as PicklistField;
				if (picklistField2 != null && (templateField.ValueType == FieldValueType.SinglePicklist || templateField.ValueType == FieldValueType.MultiplePicklist))
				{
					foreach (PicklistItem item in picklistField.Picklist)
					{
						if (!templateField.PicklistItemNames.Contains(item.Name))
						{
							picklistField2.Picklist.Add(item.Name);
						}
					}
					return;
				}
			}
			if (throwOnError)
			{
				throw new LanguagePlatformException(ErrorCode.TMImportIncompatibleFieldTypes);
			}
		}

		private void UpgradeTmIfNeeded(string filePath)
		{
			FileContainer container = new FileContainer(filePath);
			API aPI = new API();
			TranslationMemorySetup translationMemorySetup = aPI.GetTranslationMemories(container, checkPermissions: false)[0];
			if (translationMemorySetup.UsesLegacyHashes)
			{
				translationMemorySetup.UsesLegacyHashes = false;
				aPI.ChangeTranslationMemory(container, translationMemorySetup);
				ImportStatistics stats = new ImportStatistics();
				CancellationTokenSource cts = new CancellationTokenSource();
				aPI.SelectiveReindexTranslationUnits(container, translationMemorySetup.ResourceId, cts.Token, new Progress<int>(delegate(int x)
				{
					EventHandler<BatchImportedEventArgs> batchImported = this.BatchImported;
					stats.TotalRead = x;
					if (batchImported != null)
					{
						BatchImportedEventArgs batchImportedEventArgs = new BatchImportedEventArgs(stats);
						batchImported(this, batchImportedEventArgs);
						if (batchImportedEventArgs.Cancel)
						{
							cts.Cancel();
						}
					}
				}));
			}
		}

		private void PrepareBufferedTUsForImport(out TranslationUnit[] tus, out bool[] mask)
		{
			tus = Buffer.ToArray();
			if (ImportSettings.NewFields == ImportSettings.NewFieldsOption.AddToSetup)
			{
				bool flag = false;
				FieldDefinitions tmFieldsDefinitions = ActualImportDestination.FieldDefinitions;
				TranslationUnit[] array = tus;
				foreach (TranslationUnit translationUnit in array)
				{
					if (!ImportSettings.UseTmUserIdFromBilingualFile)
					{
						UpdateTranslationUnitUser(translationUnit);
					}
					flag = translationUnit.FieldValues.Where((FieldValue fv) => !Field.IsSystemFieldName(fv.Name)).Aggregate(flag, (bool current, FieldValue fieldValue) => GetFieldDefinitionsChanges(tmFieldsDefinitions, fieldValue, current));
				}
				if (flag)
				{
					ActualImportDestination.UpdateFieldDefinitions(tmFieldsDefinitions);
				}
			}
			mask = Enumerable.Repeat(element: true, tus.Length).ToArray();
			if (LastTuInBatch != null)
			{
				tus = new TranslationUnit[1]
				{
					LastTuInBatch
				}.Concat(tus).ToArray();
				mask = new bool[1].Concat(mask).ToArray();
			}
		}

		private ImportExportResponse ProcessBufferedTUs(ImportStatistics stats)
		{
			PrepareBufferedTUsForImport(out TranslationUnit[] tus, out bool[] mask);
			ImportResult[] array = ActualImportDestination.AddTranslationUnitsMask(tus, ImportSettings, mask);
			ImportResults importResults = new ImportResults(array);
			stats.Add(importResults);
			if (ImportSettings.InvalidTranslationUnitsExportPath != null)
			{
				HandleInvalidTUs(tus, array);
			}
			stats.BadTranslationUnits = BadTuCount;
			ImportExportResponse result = OnBatchImported(importResults, stats);
			LastTuInBatch = Buffer.Last();
			Buffer.Clear();
			return result;
		}

		private void UpdateTranslationUnitUser(TranslationUnit translationUnit)
		{
			translationUnit.SystemFields.CreationUser = TranslationMemoryUserIdSetting;
			translationUnit.SystemFields.ChangeUser = TranslationMemoryUserIdSetting;
		}

		private void HandleInvalidTUs(IReadOnlyList<TranslationUnit> tus, IReadOnlyList<ImportResult> importResult)
		{
			for (int i = 0; i < importResult.Count; i++)
			{
				if (importResult[i] != null && importResult[i].ErrorCode != 0)
				{
					OutputErrorTu(tus[i], importResult[i]);
					BadTuCount++;
				}
			}
		}

		private static bool GetFieldDefinitionsChanges(FieldDefinitions tmFieldsDefinitions, FieldValue fieldValue, bool hasFieldChanges)
		{
			Field field = tmFieldsDefinitions[fieldValue.Name];
			if (field == null)
			{
				field = CreateFieldDefinition(fieldValue.ValueType, fieldValue.Name);
				tmFieldsDefinitions.Add(field);
				hasFieldChanges = true;
			}
			switch (field.ValueType)
			{
			case FieldValueType.SinglePicklist:
			{
				SinglePicklistFieldValue spfv = fieldValue as SinglePicklistFieldValue;
				PicklistField picklistField2 = field as PicklistField;
				if (picklistField2?.Picklist.FirstOrDefault((PicklistItem x) => string.CompareOrdinal(x.Name, spfv?.Value.Name) == 0) == null)
				{
					picklistField2?.Picklist.Add(spfv?.Value.Name);
					hasFieldChanges = true;
				}
				break;
			}
			case FieldValueType.MultiplePicklist:
			{
				MultiplePicklistFieldValue multiplePicklistFieldValue = fieldValue as MultiplePicklistFieldValue;
				PicklistField picklistField = field as PicklistField;
				if (multiplePicklistFieldValue?.Values != null)
				{
					foreach (PicklistItem item in multiplePicklistFieldValue.Values)
					{
						if (picklistField?.Picklist.FirstOrDefault((PicklistItem x) => string.CompareOrdinal(x.Name, item.Name) == 0) == null)
						{
							picklistField?.Picklist.Add(item.Name);
							hasFieldChanges = true;
						}
					}
					return hasFieldChanges;
				}
				break;
			}
			}
			return hasFieldChanges;
		}

		private static Field CreateFieldDefinition(FieldValueType fieldValueType, string name)
		{
			if (fieldValueType != FieldValueType.SinglePicklist && fieldValueType != FieldValueType.MultiplePicklist)
			{
				return new Field(name, fieldValueType);
			}
			return new PicklistField(name, fieldValueType);
		}

		private void UpgradeTempFileAndImport()
		{
			string filePath = _conversionTm.FilePath;
			bool num = CmInfoMayNeedConverting(_conversionTm.SourceLanguage) || CmInfoMayNeedConverting(_conversionTm.TargetLanguage);
			_conversionTm = null;
			if (num)
			{
				RecoverJazhcmInfo(filePath);
			}
			int totalRead = Statistics.TotalRead;
			int discardedTranslationUnits = Statistics.DiscardedTranslationUnits;
			_ = Statistics.AddedTranslationUnits;
			int errors = Statistics.Errors;
			int badTranslationUnits = Statistics.BadTranslationUnits;
			int mergedTranslationUnits = Statistics.MergedTranslationUnits;
			int overwrittenTranslationUnits = Statistics.OverwrittenTranslationUnits;
			int rawTUs = Statistics.RawTUs;
			ImportSdltmFile(filePath, upgradePermitted: true);
			File.Delete(filePath);
			Statistics.TotalRead = totalRead;
			Statistics.DiscardedTranslationUnits = discardedTranslationUnits;
			Statistics.Errors = errors;
			Statistics.BadTranslationUnits = badTranslationUnits;
			Statistics.MergedTranslationUnits += mergedTranslationUnits;
			Statistics.OverwrittenTranslationUnits += overwrittenTranslationUnits;
			Statistics.RawTUs = rawTUs;
		}

		private static bool CmInfoMayNeedConverting(CultureInfo ci)
		{
			if (!(ci.TwoLetterISOLanguageName.ToLowerInvariant() == "ja"))
			{
				return ci.TwoLetterISOLanguageName.ToLowerInvariant() == "zh";
			}
			return true;
		}

		private void RecoverJazhcmInfo(string tmPath)
		{
			FileContainer container = new FileContainer(tmPath);
			API aPI = new API();
			TranslationMemorySetup translationMemorySetup = aPI.GetTranslationMemories(container, checkPermissions: false)[0];
			CancellationTokenSource cts = new CancellationTokenSource();
			ImportStatistics stats = new ImportStatistics();
			aPI.RecoverJAZHCMInfo(container, translationMemorySetup.ResourceId, new Progress<TranslationMemoryProgress>(delegate(TranslationMemoryProgress x)
			{
				EventHandler<BatchImportedEventArgs> batchImported = this.BatchImported;
				stats.TotalRead = x.Count;
				if (batchImported != null)
				{
					BatchImportedEventArgs batchImportedEventArgs = new BatchImportedEventArgs(stats);
					batchImported(this, batchImportedEventArgs);
					if (batchImportedEventArgs.Cancel)
					{
						cts.Cancel();
					}
				}
			}), cts.Token);
		}
	}
}
