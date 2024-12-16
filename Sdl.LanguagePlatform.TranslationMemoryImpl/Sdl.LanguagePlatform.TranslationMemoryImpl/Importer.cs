using Sdl.Core.FineGrainedAlignment;
using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.FGA;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class Importer
	{
		private readonly CallContext _context;

		private readonly Searcher _duplicateSearcher;

		private Searcher _previousTranslationSearcher;

		private readonly Dictionary<string, AttributeDeclaration> _attributes;

		private readonly long _tokenizationSignatureHash;

		private PersistentObjectToken[] _batchAddResultIds;

		private List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> _batchDuplicateTus;

		private Tuple<AnnotatedTranslationUnit, ImportType, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>[] _batchTUs;

		private const int TuThresholdForBatchImport = 3;

		internal static HashSet<TranslationUnitOrigin> _OriginsToNormalize = new HashSet<TranslationUnitOrigin>
		{
			TranslationUnitOrigin.Unknown,
			TranslationUnitOrigin.ContextTM,
			TranslationUnitOrigin.AdaptiveMachineTranslation,
			TranslationUnitOrigin.AutomaticTranslation,
			TranslationUnitOrigin.MachineTranslation,
			TranslationUnitOrigin.Nmt
		};

		public List<ImportResult> DeleteResults
		{
			get;
			set;
		}

		public AnnotatedTranslationMemory Tm
		{
			get;
		}

		public bool SkipSynchronousFga
		{
			get;
			set;
		}

		internal long TokenizationSignatureHash => _tokenizationSignatureHash;

		public Importer(CallContext context, PersistentObjectToken tmId)
		{
			_context = context;
			Tm = _context.GetAnnotatedTranslationMemory(tmId);
			_tokenizationSignatureHash = _context.ResourceManager.GetCurrentTokenizationSignatureHash(Tm);
			_attributes = _context.Storage.GetAttributes(Tm.Tm.ResourceId.Id).ToDictionary((AttributeDeclaration attr) => attr.Name, StringComparer.OrdinalIgnoreCase);
			SearchSettings searchSettings = new SearchSettings
			{
				ComputeTranslationProposal = false,
				IsDocumentSearch = false,
				MaxResults = 100,
				MinScore = 100,
				SortSpecification = new SortSpecification("Sco/D"),
				Mode = SearchMode.DuplicateSearch
			};
			searchSettings.AddPenalty(PenaltyType.MemoryTagsDeleted, 1);
			_duplicateSearcher = new Searcher(_context, tmId, searchSettings);
			DeleteResults = new List<ImportResult>();
		}

		public ImportResult[] Import(IList<AnnotatedTranslationUnit> tus, IList<int> previousTranslationHashes, ImportSettings settings, IList<bool> mask, bool isUpdate)
		{
			return Import(tus, previousTranslationHashes, settings, mask, isUpdate, null);
		}

		private static List<T> InitializedList<T>(T value, int count)
		{
			T[] array = new T[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = value;
			}
			return array.ToList();
		}

		private long GetSegmentHash(AbstractAnnotatedSegment s)
		{
			if (!Tm.Tm.UsesLegacyHashes)
			{
				return s.StrictHash;
			}
			return s.Hash;
		}

		internal ImportResult[] Import(IList<AnnotatedTranslationUnit> tus, IList<int> previousTranslationHashes, ImportSettings settings, IList<bool> mask, bool isUpdate, IList<bool> retainFgaInfoList)
		{
			ImportResult[] array = new ImportResult[tus.Count];
			for (int i = 0; i < tus.Count; i++)
			{
				if (mask != null && !mask[i])
				{
					continue;
				}
				AnnotatedTranslationUnit annotatedTranslationUnit = tus[i];
				if (annotatedTranslationUnit == null)
				{
					continue;
				}
				if (annotatedTranslationUnit.TranslationUnit == null)
				{
					array[i] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.EmptyData);
					continue;
				}
				ErrorCode errorCode = ValidateTu(annotatedTranslationUnit.TranslationUnit, settings);
				if (errorCode != 0)
				{
					array[i] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, errorCode);
				}
				TruncateUserIdAtMaxLength(annotatedTranslationUnit.TranslationUnit);
			}
			if (settings?.Filter != null)
			{
				for (int j = 0; j < tus.Count; j++)
				{
					if (array[j] == null && tus[j] != null && !settings.Filter.Evaluate(tus[j].TranslationUnit))
					{
						array[j] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Discard, ErrorCode.OK);
					}
				}
			}
			int num = 0;
			HashSet<long> hashSet = new HashSet<long>();
			List<AnnotatedTranslationUnit> list = new List<AnnotatedTranslationUnit>();
			List<int> list2 = (previousTranslationHashes != null) ? new List<int>(previousTranslationHashes) : InitializedList(0, tus.Count);
			List<bool> list3 = (retainFgaInfoList != null) ? new List<bool>(retainFgaInfoList) : InitializedList(value: false, tus.Count);
			while (true)
			{
				List<bool> list4 = new List<bool>();
				int num2;
				while (true)
				{
					num2 = list.Count + num;
					if (num2 == tus.Count)
					{
						break;
					}
					AnnotatedTranslationUnit annotatedTranslationUnit2 = tus[num2];
					if (array[num2] != null)
					{
						annotatedTranslationUnit2 = null;
					}
					if (annotatedTranslationUnit2 != null)
					{
						long segmentHash = GetSegmentHash(annotatedTranslationUnit2.Source);
						if (hashSet.Contains(segmentHash))
						{
							break;
						}
						hashSet.Add(segmentHash);
					}
					list.Add(annotatedTranslationUnit2);
					list4.Add(mask == null || mask[num2]);
				}
				List<int> range = list2.GetRange(num, list.Count);
				list3.GetRange(num, list.Count);
				int count = list.Count;
				if (num + list.Count < tus.Count)
				{
					list.Add(tus[num + list.Count]);
					list4.Add(item: false);
					range.Add(0);
				}
				if (num > 0)
				{
					list.Insert(0, tus[num - 1]);
					list4.Insert(0, item: false);
					range.Insert(0, 0);
				}
				ImportResult[] array2 = ImportInner(list, range, settings, list4, isUpdate, list3);
				int num3 = (num > 0) ? 1 : 0;
				for (int k = 0; k < count; k++)
				{
					if (array[num + k] == null)
					{
						array[num + k] = array2[num3 + k];
					}
				}
				if (num2 == tus.Count)
				{
					break;
				}
				hashSet.Clear();
				list.Clear();
				num += count;
			}
			return array;
		}

		internal ImportResult[] ImportInner(IList<AnnotatedTranslationUnit> tus, IList<int> previousTranslationHashes, ImportSettings settings, IList<bool> mask, bool isUpdate, IList<bool> retainFgaInfoList)
		{
			ImportParameters importParameters = new ImportParameters
			{
				IsBatch = false
			};
			if (_context.Storage is SqlStorage && tus.Count > 3)
			{
				importParameters.IsBatch = true;
				_batchAddResultIds = new PersistentObjectToken[tus.Count];
				_batchTUs = new Tuple<AnnotatedTranslationUnit, ImportType, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>[tus.Count];
			}
			ImportResult[] array = new ImportResult[tus.Count];
			if (importParameters.IsBatch)
			{
				GetBatchDuplicates(tus, mask, array);
			}
			for (int i = 0; i < tus.Count; i++)
			{
				bool retainFgaInfo = retainFgaInfoList != null && retainFgaInfoList[i];
				if (tus[i] == null || (mask != null && !mask[i]))
				{
					array[i] = null;
				}
				else if (array[i] == null)
				{
					int previousTranslationHash = (previousTranslationHashes != null) ? previousTranslationHashes[i] : 0;
					TuContext tuContext = SetTuTextContext(tus, settings, i);
					try
					{
						importParameters.RetainFgaInfo = retainFgaInfo;
						importParameters.TuContext = tuContext;
						importParameters.Type = (isUpdate ? ImportType.Update : ImportType.Add);
						importParameters.PreviousTranslationHash = previousTranslationHash;
						importParameters.IndexInBatch = i;
						array[i] = ImportInternal(tus[i], settings, importParameters);
					}
					catch (Exception ex)
					{
						if (!(ex is NotImplementedException))
						{
							if (!(ex is StorageException))
							{
								if (!(ex is XmlException))
								{
									LanguagePlatformException ex2 = ex as LanguagePlatformException;
									if (ex2 != null)
									{
										array[i] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ex2.Description.ErrorCode);
									}
									else
									{
										array[i] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.Other);
									}
								}
								else
								{
									array[i] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.XmlError);
								}
							}
							else
							{
								array[i] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.StorageError);
							}
						}
						else
						{
							array[i] = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.NotImplemented);
						}
						tus[i] = null;
					}
				}
			}
			if (!importParameters.IsBatch)
			{
				return array;
			}
			FlushBatchToDb(settings);
			for (int j = 0; j < tus.Count; j++)
			{
				if (_batchAddResultIds[j] != null)
				{
					array[j].TuId = _batchAddResultIds[j];
				}
			}
			return array;
		}

		private void GetBatchDuplicates(IList<AnnotatedTranslationUnit> tus, IList<bool> mask, IReadOnlyList<ImportResult> res)
		{
			_batchDuplicateTus = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
			List<bool> list = new List<bool>(tus.Count);
			list.AddRange(tus.Select((AnnotatedTranslationUnit atu, int index) => res[index] == null && (mask == null || mask[index]) && atu?.TranslationUnit != null));
			List<SearchResults> list2 = _duplicateSearcher.DuplicateSearchBatch(tus.ToArray(), list.ToArray());
			List<int> duplicateMap = new List<int>();
			HashSet<int> hashSet = new HashSet<int>();
			for (int j = 0; j < tus.Count; j++)
			{
				SearchResults searchResults = list2[j];
				if (searchResults == null)
				{
					duplicateMap.Add(-1);
					continue;
				}
				searchResults.Results = searchResults.Results.FindAll((SearchResult x) => !x.ScoringResult.TagMismatch && !x.ScoringResult.MemoryTagsDeleted);
				searchResults.Results = searchResults.Results.FindAll((SearchResult x) => x.ScoringResult.IsExactMatch && !x.ScoringResult.TargetSegmentDiffers);
				while (searchResults.Count > 0)
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memoryTranslationUnit = searchResults.Results[0].MemoryTranslationUnit;
					if (!NotDuplicateFormat(tus[j].TranslationUnit, memoryTranslationUnit))
					{
						break;
					}
					searchResults.Results.RemoveAt(0);
				}
				if (searchResults.Count > 0)
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memoryTranslationUnit2 = searchResults.Results[0].MemoryTranslationUnit;
					duplicateMap.Add(memoryTranslationUnit2.ResourceId.Id);
					hashSet.Add(memoryTranslationUnit2.ResourceId.Id);
				}
				else
				{
					duplicateMap.Add(-1);
				}
			}
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> fullTusByIds = _context.Storage.GetFullTusByIds(Tm.Tm.ResourceId.Id, new List<int>(hashSet));
			int i;
			for (i = 0; i < tus.Count; i++)
			{
				if (duplicateMap[i] == -1)
				{
					_batchDuplicateTus.Add(null);
					continue;
				}
				Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu = fullTusByIds.Find((Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit x) => x.Id == duplicateMap[i]);
				Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = _context.ResourceManager.GetTranslationUnit(storageTu, Tm.Tm.FieldDeclarations, Tm.Tm.LanguageDirection.SourceCulture, Tm.Tm.LanguageDirection.TargetCulture);
				_batchDuplicateTus.Add(translationUnit);
			}
		}

		private TuContext SetTuTextContext(IList<AnnotatedTranslationUnit> tus, ImportSettings settings, int i)
		{
			if (settings == null || !settings.IsDocumentImport || settings.Filter != null)
			{
				return null;
			}
			TuContext tuContext = null;
			if (Tm.Tm.TextContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
			{
				tuContext = SetPreviousFollowingTuTextContext(tus, i);
			}
			if (Tm.Tm.TextContextMatchType == TextContextMatchType.PrecedingSourceAndTarget)
			{
				tuContext = SetPreviousTuTextContext(tus, i);
			}
			if (tuContext != null)
			{
				tus[i].TranslationUnit.Contexts.Add(tuContext);
			}
			return tuContext;
		}

		private TuContext SetPreviousTuTextContext(IList<AnnotatedTranslationUnit> tus, int i)
		{
			TuContext result = null;
			long context = 0L;
			long context2 = 0L;
			if (i == 0)
			{
				result = new TuContext(context, context2);
			}
			else if (tus[i - 1] != null)
			{
				context = ((tus[i - 1].Source == null) ? (-1) : GetSegmentHash(tus[i - 1].Source));
				context2 = ((tus[i - 1].Target == null) ? (-1) : GetSegmentHash(tus[i - 1].Target));
				if (context != -1 && context2 != -1)
				{
					result = new TuContext(context, context2);
				}
			}
			return result;
		}

		private TuContext SetPreviousFollowingTuTextContext(IList<AnnotatedTranslationUnit> tus, int i)
		{
			TuContext result = null;
			long context = 0L;
			long num = 0L;
			if (i < tus.Count - 1)
			{
				num = ((tus[i + 1].Source == null) ? (-1) : GetSegmentHash(tus[i + 1].Source));
			}
			if (i == 0)
			{
				result = new TuContext(context, num);
			}
			else if (tus[i - 1] != null)
			{
				context = ((tus[i - 1].Source == null) ? (-1) : GetSegmentHash(tus[i - 1].Source));
				if (context != -1 && num != -1)
				{
					result = new TuContext(context, num);
				}
			}
			return result;
		}

		private ErrorCode CheckLanguages(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, ImportSettings settings)
		{
			if (tu.SourceSegment.Culture == null)
			{
				return ErrorCode.SourceLanguageIncompatibleWithTM;
			}
			if (tu.TargetSegment.Culture == null)
			{
				return ErrorCode.TargetLanguageIncompatibleWithTM;
			}
			bool flag = settings?.CheckMatchingSublanguages ?? false;
			if (!object.Equals(tu.SourceSegment.Culture.Name, Tm.Tm.LanguageDirection.SourceCulture.Name))
			{
				if (flag || !CultureInfoExtensions.AreCompatible(Tm.Tm.LanguageDirection.SourceCulture, tu.SourceSegment.Culture))
				{
					return ErrorCode.SourceLanguageIncompatibleWithTM;
				}
				tu.SourceSegment.Culture = Tm.Tm.LanguageDirection.SourceCulture;
			}
			if (object.Equals(tu.TargetSegment.Culture.Name, Tm.Tm.LanguageDirection.TargetCulture.Name))
			{
				return ErrorCode.OK;
			}
			if (flag || !CultureInfoExtensions.AreCompatible(Tm.Tm.LanguageDirection.TargetCulture, tu.TargetSegment.Culture))
			{
				return ErrorCode.TargetLanguageIncompatibleWithTM;
			}
			tu.TargetSegment.Culture = Tm.Tm.LanguageDirection.TargetCulture;
			return ErrorCode.OK;
		}

		internal ImportResult ImportInternal(AnnotatedTranslationUnit atu, ImportSettings settings, ImportParameters importParameters)
		{
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = atu.TranslationUnit;
			ErrorCode errorCode = ValidateTu(translationUnit, settings);
			if (errorCode != 0)
			{
				return new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, errorCode);
			}
			TruncateUserIdAtMaxLength(translationUnit);
			SetTuContext(atu);
			ImportSettings.TUUpdateMode tUUpdateMode = importParameters.UpdateMode = GetTuUpdateMode(settings);
			if (_OriginsToNormalize.Contains(translationUnit.Origin))
			{
				translationUnit.Origin = TranslationUnitOrigin.TM;
			}
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateTu = GetDuplicateTu(atu, importParameters);
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit2 = null;
			bool idContextMatch = false;
			SearchResults results = null;
			if (importParameters.PreviousTranslationHash != 0 || (Tm.Tm.IdContextMatch && atu.TranslationUnit.IdContexts.Length > 0))
			{
				translationUnit2 = SearchPreviousTranslations(atu, importParameters.PreviousTranslationHash, out results, out idContextMatch);
				if (translationUnit2 != null)
				{
					List<Placeable> atuPlaceables = PlaceableComputer.ComputePlaceables(atu.Source.Segment, atu.Target.Segment);
					if (_duplicateSearcher.AreTusDuplicates(atu, atuPlaceables, translationUnit2) && HasPlaceableFormatChanges(atu.TranslationUnit, translationUnit2, source: true))
					{
						translationUnit2 = null;
					}
				}
			}
			else if (importParameters.UpdateMode != 0)
			{
				SearchPreviousTranslations(atu, 0, out results, out idContextMatch);
			}
			ImportResult result = new ImportResult();
			bool flag = MergeOrDeleteDuplicateTu(translationUnit, ref duplicateTu, settings, importParameters, result);
			if (importParameters.PreviousTranslationHash != 0 && translationUnit2 != null && (duplicateTu == null || translationUnit2.ResourceId.Id != duplicateTu.ResourceId.Id))
			{
				flag = MergeOrDeletePreviousTu(atu, translationUnit2, tUUpdateMode, settings, importParameters);
			}
			if (idContextMatch)
			{
				flag |= MergeOrDeleteIdContextDuplicate(atu, translationUnit2, settings, importParameters);
			}
			DeleteSourceSegmentDuplicates(tUUpdateMode, ref duplicateTu, results, result, translationUnit);
			return TuStorageUpdate(atu, settings, importParameters, result, translationUnit2, duplicateTu, flag);
		}

		private ImportResult TuStorageUpdate(AnnotatedTranslationUnit atu, ImportSettings settings, ImportParameters importParameters, ImportResult result, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit previousTranslation, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateTu, bool translationDeleted)
		{
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = atu.TranslationUnit;
			bool flag = settings?.ProjectSettings != null && settings.ProjectSettings.Count > 0;
			if (((result.Action == Sdl.LanguagePlatform.TranslationMemory.Action.Discard && duplicateTu != null) & flag) && duplicateTu.FieldValues.Merge(settings.ProjectSettings))
			{
				result.Action = Sdl.LanguagePlatform.TranslationMemory.Action.Merge;
			}
			switch (result.Action)
			{
			case Sdl.LanguagePlatform.TranslationMemory.Action.Add:
				if (flag)
				{
					translationUnit.FieldValues.Merge(settings.ProjectSettings);
				}
				result = AddTuToStorage(atu, settings, previousTranslation, result, importParameters);
				break;
			case Sdl.LanguagePlatform.TranslationMemory.Action.Merge:
			{
				if (flag)
				{
					duplicateTu?.FieldValues.Merge(settings.ProjectSettings);
				}
				importParameters.Type = ImportType.PartialUpdate;
				AnnotatedTranslationUnit duplicateaTu = new AnnotatedTranslationUnit(Tm, duplicateTu, keepTokens: false, keepPeripheralWhitespace: false);
				result = UpdateDuplicateTuInStorage(settings, translationUnit, duplicateaTu, importParameters);
				break;
			}
			case Sdl.LanguagePlatform.TranslationMemory.Action.Discard:
				UpdatePreviousTuInStorage(settings, translationUnit, previousTranslation, importParameters.PreviousTranslationHash, translationDeleted);
				break;
			}
			if (translationDeleted)
			{
				result.Action = Sdl.LanguagePlatform.TranslationMemory.Action.Overwrite;
			}
			return result;
		}

		private static ImportSettings.TUUpdateMode GetTuUpdateMode(ImportSettings settings)
		{
			if (settings != null)
			{
				if (settings.ExistingTUsUpdateMode == ImportSettings.TUUpdateMode.AddNew && settings.OverwriteExistingTUs)
				{
					settings.ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.Overwrite;
				}
				return settings.ExistingTUsUpdateMode;
			}
			return ImportSettings.TUUpdateMode.AddNew;
		}

		private bool NotDuplicateFormat(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit newTu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit dupTu)
		{
			return HasPlaceableFormatChanges(newTu, dupTu, source: true);
		}

		private Sdl.LanguagePlatform.TranslationMemory.TranslationUnit GetDuplicateTu(AnnotatedTranslationUnit atu, ImportParameters importParameters)
		{
			if (!importParameters.IsBatch)
			{
				SearchResults searchResults = _duplicateSearcher.Search(atu);
				if (searchResults != null)
				{
					searchResults.Results = searchResults.Results.FindAll((SearchResult x) => !x.ScoringResult.TagMismatch && !x.ScoringResult.MemoryTagsDeleted);
					while (searchResults.Count > 0)
					{
						Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memoryTranslationUnit = searchResults.Results[0].MemoryTranslationUnit;
						if (!NotDuplicateFormat(atu.TranslationUnit, memoryTranslationUnit))
						{
							break;
						}
						searchResults.Results.RemoveAt(0);
					}
				}
				if (searchResults != null)
				{
					searchResults.Results = searchResults.Results.FindAll((SearchResult x) => x.ScoringResult.IsExactMatch && !x.ScoringResult.TargetSegmentDiffers);
				}
				Sdl.LanguagePlatform.TranslationMemory.TranslationUnit result = null;
				if (searchResults != null && searchResults.Count > 0)
				{
					result = searchResults.Results[0].MemoryTranslationUnit;
				}
				return result;
			}
			return _batchDuplicateTus[importParameters.IndexInBatch];
		}

		private void SetTuContext(AnnotatedTranslationUnit atu)
		{
			if (atu.TranslationUnit.Contexts != null && atu.TranslationUnit.Contexts.Values.Any((TuContext x) => x.Segment1 != null || x.Segment2 != null))
			{
				TuContexts tuContexts = new TuContexts();
				foreach (TuContext value in atu.TranslationUnit.Contexts.Values)
				{
					if (value.Context1 == 0L && value.Context2 == 0L)
					{
						tuContexts.Add(value);
					}
					else if (value.Segment1 != null || value.Segment2 != null)
					{
						UpdateContext(value);
						value.Segment1 = null;
						value.Segment2 = null;
						tuContexts.Add(value);
					}
				}
				atu.TranslationUnit.Contexts = tuContexts;
			}
		}

		private void UpdateContext(TuContext tuc)
		{
			if (tuc.Segment1 != null)
			{
				AnnotatedSegment s = new AnnotatedSegment(Tm, tuc.Segment1, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: false);
				tuc.Context1 = GetSegmentHash(s);
			}
			if (tuc.Segment2 != null)
			{
				AnnotatedSegment s2 = new AnnotatedSegment(Tm, tuc.Segment2, Tm.Tm.TextContextMatchType == TextContextMatchType.PrecedingSourceAndTarget, keepTokens: false, keepPeripheralWhitespace: false);
				tuc.Context2 = GetSegmentHash(s2);
			}
		}

		private ErrorCode ValidateTu(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, ImportSettings settings)
		{
			ErrorCode errorCode = tu.Validate();
			if (errorCode != 0)
			{
				return errorCode;
			}
			if (settings != null && settings.TagCountLimit > 0)
			{
				int tagCount = tu.SourceSegment.GetTagCount();
				int tagCount2 = tu.TargetSegment.GetTagCount();
				if (tagCount > settings.TagCountLimit || tagCount2 > settings.TagCountLimit)
				{
					errorCode = ErrorCode.TagCountLimitExceeded;
				}
			}
			if (errorCode == ErrorCode.OK)
			{
				errorCode = CheckLanguages(tu, settings);
			}
			return errorCode;
		}

		private void TruncateUserIdAtMaxLength(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			tu.SystemFields.ChangeUser = tu.SystemFields.ChangeUser?.Substring(0, Math.Min(tu.SystemFields.ChangeUser.Length, 255));
			tu.SystemFields.CreationUser = tu.SystemFields.CreationUser?.Substring(0, Math.Min(tu.SystemFields.CreationUser.Length, 255));
			tu.SystemFields.UseUser = tu.SystemFields.UseUser?.Substring(0, Math.Min(tu.SystemFields.UseUser.Length, 255));
		}

		private bool MergeOrDeletePreviousTu(AnnotatedTranslationUnit atu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit previousTu, ImportSettings.TUUpdateMode updateOverWriteMode, ImportSettings settings, ImportParameters importParameters)
		{
			if (updateOverWriteMode != 0 && updateOverWriteMode != ImportSettings.TUUpdateMode.Overwrite)
			{
				MergeFieldValuesAndContexts(atu.TranslationUnit, previousTu, settings, null);
			}
			SetSystemFields(atu, previousTu, settings);
			if (updateOverWriteMode == ImportSettings.TUUpdateMode.LeaveUnchanged || updateOverWriteMode == ImportSettings.TUUpdateMode.AddNew)
			{
				return false;
			}
			bool result = _context.ResourceManager.DeleteTranslationUnit(Tm.Tm.ResourceId, previousTu.ResourceId);
			atu.TranslationUnit.ResourceId = previousTu.ResourceId;
			importParameters.Type = ImportType.Update;
			return result;
		}

		private void SetSystemFields(AnnotatedTranslationUnit atu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit previousTu, ImportSettings settings)
		{
			string text = IsUserOverwrite(settings) ? _context.UserName : atu.TranslationUnit.SystemFields.ChangeUser;
			atu.TranslationUnit.SystemFields.CreationDate = previousTu.SystemFields.CreationDate;
			atu.TranslationUnit.SystemFields.CreationUser = previousTu.SystemFields.CreationUser;
			atu.TranslationUnit.SystemFields.ChangeDate = DateTimeUtilities.Normalize(DateTime.Now);
			atu.TranslationUnit.SystemFields.ChangeUser = (text ?? _context.UserName);
		}

		private bool MergeOrDeleteIdContextDuplicate(AnnotatedTranslationUnit atu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateIdContext, ImportSettings settings, ImportParameters importParameters)
		{
			if (importParameters.UpdateMode != 0 && importParameters.UpdateMode != ImportSettings.TUUpdateMode.Overwrite)
			{
				MergeFieldValuesAndContexts(atu.TranslationUnit, duplicateIdContext, settings, null);
			}
			SetSystemFields(atu, duplicateIdContext, settings);
			bool result = _context.ResourceManager.DeleteTranslationUnit(Tm.Tm.ResourceId, duplicateIdContext.ResourceId);
			atu.TranslationUnit.ResourceId = duplicateIdContext.ResourceId;
			importParameters.Type = ImportType.Update;
			return result;
		}

		private void DeleteSourceSegmentDuplicates(ImportSettings.TUUpdateMode overwriteExistingMode, ref Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateTu, SearchResults sourceSegmentDuplicates, ImportResult result, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			if (sourceSegmentDuplicates == null || sourceSegmentDuplicates.Count <= 0)
			{
				return;
			}
			switch (overwriteExistingMode)
			{
			case ImportSettings.TUUpdateMode.OverwriteCurrent:
				break;
			case ImportSettings.TUUpdateMode.Overwrite:
				DeleteVariantTranslations(sourceSegmentDuplicates);
				break;
			case ImportSettings.TUUpdateMode.KeepMostRecent:
				if (!KeepMostRecentTu(sourceSegmentDuplicates, tu, ref duplicateTu))
				{
					result.Action = Sdl.LanguagePlatform.TranslationMemory.Action.Discard;
				}
				break;
			case ImportSettings.TUUpdateMode.LeaveUnchanged:
				result.Action = Sdl.LanguagePlatform.TranslationMemory.Action.Discard;
				duplicateTu = null;
				break;
			}
		}

		private bool MergeOrDeleteDuplicateTu(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, ref Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateTu, ImportSettings settings, ImportParameters importParameters, ImportResult result)
		{
			if (duplicateTu == null)
			{
				result.Action = Sdl.LanguagePlatform.TranslationMemory.Action.Add;
			}
			else
			{
				if (((!HasPlaceableFormatChanges(tu, duplicateTu, source: true) && HasPlaceableFormatChanges(tu, duplicateTu, source: false)) | HasAdditionalPlaceableFormatInformation(tu, duplicateTu, source: false)) || (duplicateTu.Origin == TranslationUnitOrigin.Alignment && tu.Origin != TranslationUnitOrigin.Alignment))
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = _context.ResourceManager.GetTranslationUnit(Tm.Tm.ResourceId, duplicateTu.ResourceId);
					MergeFieldValuesAndContexts(tu, translationUnit, settings, null);
					_context.ResourceManager.DeleteTranslationUnit(Tm.Tm.ResourceId, duplicateTu.ResourceId);
					duplicateTu = null;
					result.Action = Sdl.LanguagePlatform.TranslationMemory.Action.Add;
					importParameters.Type = ImportType.Update;
					return true;
				}
				result.Action = MergeFieldValuesAndContexts(duplicateTu, tu, settings, importParameters.TuContext);
			}
			return false;
		}

		private bool HasAdditionalPlaceableFormatInformation(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit incomingTu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit existingTu, bool source)
		{
			List<Token> list = source ? incomingTu.SourceSegment.Tokens : incomingTu.TargetSegment.Tokens;
			List<Token> list2 = source ? existingTu.SourceSegment.Tokens : existingTu.TargetSegment.Tokens;
			if (list == null || list2 == null || list.Count != list2.Count)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				ILocalizableToken localizableToken = list[i] as ILocalizableToken;
				if (localizableToken != null && localizableToken.FormatHasMoreInformation(list2[i] as ILocalizableToken))
				{
					return true;
				}
			}
			return false;
		}

		private bool HasPlaceableFormatChanges(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit incomingTu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit existingTu, bool source)
		{
			List<Token> list = source ? incomingTu.SourceSegment.Tokens : incomingTu.TargetSegment.Tokens;
			List<Token> list2 = source ? existingTu.SourceSegment.Tokens : existingTu.TargetSegment.Tokens;
			if (list == null || list2 == null || list.Count != list2.Count)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				ILocalizableToken localizableToken = list[i] as ILocalizableToken;
				if (localizableToken != null && !localizableToken.DoesFormatMatch(list2[i] as ILocalizableToken))
				{
					return true;
				}
			}
			return false;
		}

		private bool KeepMostRecentTu(SearchResults exactHits, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, ref Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateTu)
		{
			bool result = false;
			SearchResult recentExistingSearchResult;
			if (exactHits.Results.Count > 1)
			{
				recentExistingSearchResult = exactHits.Results.Where((SearchResult rs) => rs.ScoringResult.IsExactMatch && rs.ScoringResult.TargetSegmentDiffers).Aggregate((SearchResult x, SearchResult y) => (!(x.MemoryTranslationUnit.SystemFields.ChangeDate < y.MemoryTranslationUnit.SystemFields.ChangeDate)) ? x : y);
			}
			else
			{
				recentExistingSearchResult = exactHits.Results[0];
			}
			PersistentObjectToken[] array;
			if (tu.SystemFields.ChangeDate == default(DateTime) || tu.SystemFields.ChangeDate >= recentExistingSearchResult.MemoryTranslationUnit.SystemFields.ChangeDate)
			{
				array = (from sr in exactHits.Results
					where sr.ScoringResult.IsExactMatch && sr.ScoringResult.TargetSegmentDiffers
					select sr.MemoryTranslationUnit.ResourceId).ToArray();
				result = true;
			}
			else
			{
				array = (from sr in exactHits.Results
					where sr.ScoringResult.IsExactMatch && !object.Equals(recentExistingSearchResult.MemoryTranslationUnit.ResourceId, sr.MemoryTranslationUnit.ResourceId)
					select sr.MemoryTranslationUnit.ResourceId).ToArray();
				duplicateTu = null;
			}
			if (array.Length == 0)
			{
				return result;
			}
			try
			{
				DeleteResults.AddRange(array.Select((PersistentObjectToken t) => new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Delete, ErrorCode.OK)
				{
					TuId = new PersistentObjectToken(t.Id, t.Guid)
				}));
				_context.ResourceManager.DeleteTranslationUnits(Tm.Tm.ResourceId, array);
				return result;
			}
			catch
			{
				return result;
			}
		}

		private void DeleteVariantTranslations(SearchResults exactHits)
		{
			PersistentObjectToken[] array = (from sr in exactHits.Results
				where sr.ScoringResult.IsExactMatch && sr.ScoringResult.TargetSegmentDiffers
				select sr.MemoryTranslationUnit.ResourceId).ToArray();
			if (array.Length != 0)
			{
				try
				{
					List<PersistentObjectToken> source = _context.ResourceManager.DeleteTranslationUnits(Tm.Tm.ResourceId, array);
					DeleteResults.AddRange(source.Select((PersistentObjectToken tu) => new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Delete, ErrorCode.OK)
					{
						TuId = tu
					}));
				}
				catch
				{
				}
			}
		}

		private ImportResult AddTuToStorage(AnnotatedTranslationUnit atu, ImportSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit previousTranslation, ImportResult r, ImportParameters importParameters)
		{
			bool retainFgaInfo = importParameters.RetainFgaInfo;
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = atu.TranslationUnit;
			if (IsUserOverwrite(settings) && previousTranslation == null)
			{
				OverrideTranslationUnitUserNameFields(translationUnit);
			}
			InitializeTuSystemFields(translationUnit);
			if (translationUnit.SystemFields.UseCount < 0)
			{
				translationUnit.SystemFields.UseCount = ((settings != null && settings.IncrementUsageCount) ? 1 : 0);
			}
			if (translationUnit.ConfirmationLevel == ConfirmationLevel.Unspecified)
			{
				translationUnit.ConfirmationLevel = ConfirmationLevel.Translated;
			}
			AlignerDefinition alignerDef;
			bool updateAlignmentData = InittializeAlignmentData(atu, ref retainFgaInfo, out alignerDef);
			if (importParameters.Type == ImportType.Update && (atu.TranslationUnit.ResourceId == null || atu.TranslationUnit.ResourceId.Id == 0))
			{
				importParameters.Type = ImportType.Add;
			}
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu = GetStorageTu(atu);
			if (atu.TranslationUnit.ResourceId == null)
			{
				return r;
			}
			if (importParameters.Type == ImportType.Update)
			{
				storageTu.Id = atu.TranslationUnit.ResourceId.Id;
				storageTu.Guid = atu.TranslationUnit.ResourceId.Guid;
			}
			else
			{
				atu.TranslationUnit.ResourceId.Guid = storageTu.Guid;
			}
			r.Action = ComputeAttributeValues(translationUnit.FieldValues, storageTu, settings?.NewFields ?? ImportSettings.NewFieldsOption.Ignore);
			switch (r.Action)
			{
			case Sdl.LanguagePlatform.TranslationMemory.Action.Add:
				if (!importParameters.IsBatch)
				{
					_context.Storage.AddTu(storageTu, Tm.Tm.FuzzyIndexes, importParameters.Type == ImportType.Update, _tokenizationSignatureHash);
					_context.Storage.AddContexts(Tm.Tm.ResourceId.Id, storageTu.Id, atu.TranslationUnit.Contexts);
					_context.Storage.AddIdContexts(Tm.Tm.ResourceId.Id, storageTu.Id, Tm.Tm.IdContextMatch ? atu.TranslationUnit.IdContexts : null);
					r.TuId = new PersistentObjectToken(storageTu.Id, storageTu.Guid);
					UpdateAlignmentData(atu, retainFgaInfo, r.TuId, updateAlignmentData, alignerDef, storageTu);
				}
				else
				{
					AddTuToBatch(importParameters, atu, storageTu);
				}
				break;
			case Sdl.LanguagePlatform.TranslationMemory.Action.Error:
				r.ErrorCode = ErrorCode.TMImportFieldNotExists;
				break;
			}
			return r;
		}

		private static bool IsUserOverwrite(ImportSettings settings)
		{
			if (settings != null && settings.OverrideTuUserIdWithCurrentContextUser)
			{
				return !settings.UseTmUserIdFromBilingualFile;
			}
			return false;
		}

		private void FlushBatchToDb(ImportSettings settings)
		{
			Tuple<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit, ImportType>[] array = new Tuple<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit, ImportType>[_batchTUs.Length];
			for (int i = 0; i < _batchTUs.Length; i++)
			{
				if (_batchTUs[i] != null)
				{
					AnnotatedTranslationUnit item = _batchTUs[i].Item1;
					Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item2 = _batchTUs[i].Item3;
					item2.Contexts = item.TranslationUnit.Contexts;
					item2.IdContexts = item.TranslationUnit.IdContexts;
					if (_batchTUs[i].Item2 == ImportType.Update || _batchTUs[i].Item2 == ImportType.PartialUpdate)
					{
						item2.Id = item.TranslationUnit.ResourceId.Id;
					}
					array[i] = new Tuple<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit, ImportType>(item2, _batchTUs[i].Item2);
					ComputeAttributeValues(item.TranslationUnit.FieldValues, item2, settings?.NewFields ?? ImportSettings.NewFieldsOption.Ignore);
				}
			}
			List<Tuple<Guid, int>> dbResults = _context.Storage.AddTus(array, Tm.Tm.FuzzyIndexes, _tokenizationSignatureHash, Tm.Tm.ResourceId.Id);
			UpdateBatchResults(array, dbResults);
		}

		private void UpdateBatchResults(IReadOnlyList<Tuple<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit, ImportType>> storageTus, List<Tuple<Guid, int>> dbResults)
		{
			int num = 0;
			while (true)
			{
				if (num >= storageTus.Count)
				{
					return;
				}
				if (storageTus[num] != null)
				{
					Guid guidForThisTu = storageTus[num].Item1.Guid;
					Tuple<Guid, int> tuple = dbResults.Find((Tuple<Guid, int> x) => x.Item1 == guidForThisTu);
					if (tuple == null)
					{
						if (storageTus[num].Item2 == ImportType.Add)
						{
							break;
						}
					}
					else
					{
						_batchAddResultIds[num] = new PersistentObjectToken(tuple.Item2, tuple.Item1);
					}
				}
				num++;
			}
			throw new Exception("Id not returned for added TU");
		}

		private void AddTuToBatch(ImportParameters importParameters, AnnotatedTranslationUnit atu, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu)
		{
			_batchTUs[importParameters.IndexInBatch] = new Tuple<AnnotatedTranslationUnit, ImportType, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>(atu, importParameters.Type, storageTu);
		}

		private void UpdateAlignmentData(AnnotatedTranslationUnit atu, bool retainFgaInfo, PersistentObjectToken tuId, bool updateAlignmentData, AlignerDefinition alignerDef, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit sTu)
		{
			if (updateAlignmentData)
			{
				List<IAlignableContentPair> alignableContentPairs = new List<IAlignableContentPair>();
				if (sTu.InsertDate.HasValue)
				{
					atu.TranslationUnit.InsertDate = sTu.InsertDate.Value;
				}
				atu.TranslationUnit.ResourceId = tuId;
				Task task = new Task(delegate
				{
					ITranslationModelDataService translationModelDataService;
					if (!_context.IsFilebasedTm)
					{
						translationModelDataService = API.GetTranslationModelDataService();
					}
					else
					{
						ITranslationModelDataService translationModelDataService2 = new ContainerBasedTranslationModelDataService(_context.Container);
						translationModelDataService = translationModelDataService2;
					}
					ITranslationModelDataService translationModelDataService3 = translationModelDataService;
					using (translationModelDataService3)
					{
						if (!retainFgaInfo)
						{
							IFineGrainedAligner aligner = new SimpleAlignerBroker(translationModelDataService3).GetAligner(alignerDef);
							if (aligner != null)
							{
								alignableContentPairs.Add(GetAlignableTuWithNativeAlignments(atu));
								retainFgaInfo = aligner.Align(alignableContentPairs);
							}
						}
						if (retainFgaInfo)
						{
							TuAlignmentDataInternal alignmentDataInternal = ResourceManager.GetAlignmentDataInternal(Tm.Tm, atu);
							_context.AlignableStorage.UpdateTuAlignmentData(new List<TuAlignmentDataInternal>
							{
								alignmentDataInternal
							}, Tm.Tm.ResourceId.Id);
							_context.Complete();
						}
					}
				});
				_context.AddOnCompleteTask(task);
			}
		}

		private bool InittializeAlignmentData(AnnotatedTranslationUnit atu, ref bool retainFgaInfo, out AlignerDefinition alignerDef)
		{
			bool flag = false;
			alignerDef = null;
			if (!retainFgaInfo)
			{
				atu.TranslationUnit.AlignmentData = null;
				atu.TranslationUnit.AlignModelDate = null;
				atu.TranslationUnit.InsertDate = null;
			}
			else
			{
				flag = (atu.TranslationUnit.AlignmentData != null);
				if (flag && (atu.TranslationUnit.SourceSegment.Tokens.Count != atu.TranslationUnit.AlignmentData.Root().SourceLength || atu.TranslationUnit.TargetSegment.Tokens.Count != atu.TranslationUnit.AlignmentData.Root().TargetLength))
				{
					atu.TranslationUnit.AlignmentData = null;
					atu.TranslationUnit.AlignModelDate = null;
					atu.TranslationUnit.InsertDate = null;
					retainFgaInfo = false;
					flag = false;
				}
			}
			if (Tm.Tm.FGASupport == FGASupport.Automatic && !retainFgaInfo && !SkipSynchronousFga)
			{
				alignerDef = _context.AlignableCorpusManager.GetAlignableCorpus(_context.GetAlignableCorpusId(Tm.Tm.ResourceId)).GetAlignerDefinition();
				if (alignerDef != null)
				{
					flag = true;
				}
			}
			ValidateAlignmentData(atu);
			return flag;
		}

		private static void ValidateAlignmentData(AnnotatedTranslationUnit atu)
		{
			if ((atu.TranslationUnit.AlignmentData != null || atu.TranslationUnit.AlignModelDate.HasValue) && (atu.TranslationUnit.AlignmentData == null || !atu.TranslationUnit.AlignModelDate.HasValue))
			{
				throw new InvalidDataException("TranslationUnit.Alignment data and TranslationUnit.AlignModelDate must both be null or both non-null");
			}
		}

		private static AlignableTu GetAlignableTuWithNativeAlignments(AnnotatedTranslationUnit atu)
		{
			List<Placeable> placeables = PlaceableComputer.ComputePlaceables(atu.TranslationUnit.SourceSegment, atu.TranslationUnit.TargetSegment);
			AlignableTu alignableTu = new AlignableTu(atu.TranslationUnit)
			{
				AlignmentData = new LiftAlignedSpanPairSet((short)atu.TranslationUnit.SourceSegment.Tokens.Count, (short)atu.TranslationUnit.TargetSegment.Tokens.Count)
			};
			PlaceableComputer.ConvertPlaceablesToAlignments(placeables, alignableTu.AlignmentData, atu.TranslationUnit.SourceSegment.Tokens, atu.TranslationUnit.TargetSegment.Tokens);
			return alignableTu;
		}

		private ImportResult UpdateDuplicateTuInStorage(ImportSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, AnnotatedTranslationUnit duplicateaTu, ImportParameters importParameters)
		{
			ImportResult importResult = new ImportResult();
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = duplicateaTu.TranslationUnit;
			UpdateDuplicateUseCount(settings, tu, translationUnit);
			UpdateDuplicateUseDate(settings, tu, translationUnit);
			string useUser = IsUserOverwrite(settings) ? _context.UserName : (tu.SystemFields.CreationUser ?? _context.UserName);
			translationUnit.SystemFields.UseUser = useUser;
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit translationUnit2 = new Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit(Tm.Tm.ResourceId.Id, Guid.NewGuid(), null, null, translationUnit.SystemFields.CreationDate, translationUnit.SystemFields.CreationUser, translationUnit.SystemFields.ChangeDate, translationUnit.SystemFields.ChangeUser, translationUnit.SystemFields.UseDate, translationUnit.SystemFields.UseUser, translationUnit.SystemFields.UseCount, ResourceManager.GetTUFlags(translationUnit), null, null, null, null, null, 0, translationUnit.Format, translationUnit.Origin, translationUnit.ConfirmationLevel);
			importResult.Action = ComputeAttributeValues(translationUnit.FieldValues, translationUnit2, settings?.NewFields ?? ImportSettings.NewFieldsOption.Ignore);
			switch (importResult.Action)
			{
			case Sdl.LanguagePlatform.TranslationMemory.Action.Add:
				importResult.Action = Sdl.LanguagePlatform.TranslationMemory.Action.Merge;
				translationUnit2.Id = translationUnit.ResourceId.Id;
				translationUnit2.Guid = translationUnit.ResourceId.Guid;
				importResult.TuId = new PersistentObjectToken(translationUnit2.Id, translationUnit2.Guid);
				if (importParameters.IsBatch)
				{
					AddTuToBatch(importParameters, duplicateaTu, translationUnit2);
					break;
				}
				_context.Storage.UpdateTuHeader(translationUnit2, rewriteAttributes: true);
				if (translationUnit.Contexts != null)
				{
					_context.Storage.AddContexts(Tm.Tm.ResourceId.Id, translationUnit2.Id, translationUnit.Contexts);
				}
				if (translationUnit.IdContexts != null)
				{
					_context.Storage.AddIdContexts(Tm.Tm.ResourceId.Id, translationUnit2.Id, translationUnit.IdContexts);
				}
				break;
			case Sdl.LanguagePlatform.TranslationMemory.Action.Error:
				importResult.ErrorCode = ErrorCode.TMImportFieldNotExists;
				break;
			}
			return importResult;
		}

		private static void UpdateDuplicateUseCount(ImportSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateTu)
		{
			if (settings != null && (settings.ExistingTUsUpdateMode == ImportSettings.TUUpdateMode.LeaveUnchanged || settings.ExistingTUsUpdateMode == ImportSettings.TUUpdateMode.KeepMostRecent))
			{
				return;
			}
			if (settings != null && settings.IncrementUsageCount)
			{
				if (tu.SystemFields.UseCount <= 0)
				{
					duplicateTu.SystemFields.UseCount++;
				}
				else
				{
					duplicateTu.SystemFields.UseCount += tu.SystemFields.UseCount;
				}
			}
			else
			{
				duplicateTu.SystemFields.UseCount = Math.Max(duplicateTu.SystemFields.UseCount, tu.SystemFields.UseCount);
			}
		}

		private static void UpdateDuplicateUseDate(ImportSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit duplicateTu)
		{
			if (settings == null || (settings.ExistingTUsUpdateMode != ImportSettings.TUUpdateMode.LeaveUnchanged && settings.ExistingTUsUpdateMode != ImportSettings.TUUpdateMode.KeepMostRecent))
			{
				duplicateTu.SystemFields.UseDate = ((duplicateTu.SystemFields.UseDate >= tu.SystemFields.UseDate) ? duplicateTu.SystemFields.UseDate : tu.SystemFields.UseDate);
			}
		}

		private void UpdatePreviousTuInStorage(ImportSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit previousTranslation, int previousTranslationHash, bool previousTranslationDeleted)
		{
			if (!((settings == null || !settings.IncrementUsageCount || settings.ExistingTUsUpdateMode != ImportSettings.TUUpdateMode.OverwriteCurrent || previousTranslation == null || tu == null || previousTranslationHash != tu.TargetSegment.GetWeakHashCode()) | previousTranslationDeleted))
			{
				string luu = IsUserOverwrite(settings) ? _context.UserName : (previousTranslation.SystemFields.ChangeUser ?? _context.UserName);
				previousTranslation.ConfirmationLevel = tu.ConfirmationLevel;
				Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit translationUnit = new Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit(previousTranslation.ResourceId.Id, Guid.NewGuid(), Tm.Tm.ResourceId.Id, null, null, previousTranslation.SystemFields.CreationDate, previousTranslation.SystemFields.CreationUser, previousTranslation.SystemFields.ChangeDate, previousTranslation.SystemFields.ChangeUser, DateTimeUtilities.Normalize(DateTime.Now), luu, previousTranslation.SystemFields.UseCount + 1, ResourceManager.GetTUFlags(previousTranslation), null, null, null, null, null, 0, previousTranslation.Format, previousTranslation.Origin, tu.ConfirmationLevel);
				if (tu.Contexts != null)
				{
					_context.Storage.AddContexts(Tm.Tm.ResourceId.Id, translationUnit.Id, tu.Contexts);
				}
				if (tu.IdContexts != null)
				{
					_context.Storage.AddIdContexts(Tm.Tm.ResourceId.Id, translationUnit.Id, tu.IdContexts);
				}
				_context.Storage.UpdateTuHeader(translationUnit, rewriteAttributes: false);
			}
		}

		private void InitializeTuSystemFields(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			string userName = _context.UserName;
			DateTime creationDate = DateTimeUtilities.Normalize(DateTime.Now);
			if (tu.SystemFields.CreationDate == default(DateTime))
			{
				tu.SystemFields.CreationDate = creationDate;
			}
			if (tu.SystemFields.ChangeDate == default(DateTime))
			{
				tu.SystemFields.ChangeDate = tu.SystemFields.CreationDate;
			}
			if (tu.SystemFields.UseDate == default(DateTime))
			{
				tu.SystemFields.UseDate = tu.SystemFields.ChangeDate;
			}
			if (string.IsNullOrEmpty(tu.SystemFields.CreationUser))
			{
				tu.SystemFields.CreationUser = userName;
			}
			if (string.IsNullOrEmpty(tu.SystemFields.UseUser))
			{
				tu.SystemFields.UseUser = tu.SystemFields.CreationUser;
			}
			if (string.IsNullOrEmpty(tu.SystemFields.ChangeUser))
			{
				tu.SystemFields.ChangeUser = tu.SystemFields.CreationUser;
			}
		}

		private void OverrideTranslationUnitUserNameFields(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			tu.SystemFields.CreationUser = _context.UserName;
			tu.SystemFields.UseUser = _context.UserName;
			tu.SystemFields.ChangeUser = _context.UserName;
		}

		private Sdl.LanguagePlatform.TranslationMemory.Action ComputeAttributeValues(FieldValues fieldValues, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu, ImportSettings.NewFieldsOption newFields)
		{
			if (fieldValues == null)
			{
				return Sdl.LanguagePlatform.TranslationMemory.Action.Add;
			}
			List<AttributeValue> list = new List<AttributeValue>();
			foreach (FieldValue fieldValue in fieldValues)
			{
				if (!_attributes.TryGetValue(fieldValue.Name, out AttributeDeclaration value))
				{
					switch (newFields)
					{
					case ImportSettings.NewFieldsOption.SkipTranslationUnit:
						return Sdl.LanguagePlatform.TranslationMemory.Action.Discard;
					default:
						return Sdl.LanguagePlatform.TranslationMemory.Action.Error;
					case ImportSettings.NewFieldsOption.Ignore:
						break;
					}
				}
				else
				{
					AttributeValue storageAttributeValue = ResourceManager.GetStorageAttributeValue(value, fieldValue, newFields == ImportSettings.NewFieldsOption.Ignore);
					if (storageAttributeValue != null)
					{
						list.Add(storageAttributeValue);
					}
				}
			}
			storageTu.Attributes = ((list.Count > 0) ? list : null);
			return Sdl.LanguagePlatform.TranslationMemory.Action.Add;
		}

		private Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit GetStorageTu(AnnotatedTranslationUnit atu)
		{
			Tm.SourceTools.Stem(atu.Source.Segment);
			Tm.TargetTools.Stem(atu.Target.Segment);
			long hashCodeLong = Hash.GetHashCodeLong(SubsegmentUtilities.ComputeSimplifiedIdentityString(atu.Source.Segment));
			long hashCodeLong2 = Hash.GetHashCodeLong(SubsegmentUtilities.ComputeSimplifiedIdentityString(atu.Source.Segment));
			int serializationVersion = (!_context.IsFilebasedTm) ? 1 : 0;
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit translationUnit = new Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit(Tm.Tm.ResourceId.Id, Guid.NewGuid(), new Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.Segment(GetSegmentHash(atu.Source), hashCodeLong, null, null, null), new Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.Segment(GetSegmentHash(atu.Target), hashCodeLong2, null, null, null), atu.TranslationUnit.SystemFields.CreationDate, atu.TranslationUnit.SystemFields.CreationUser, atu.TranslationUnit.SystemFields.ChangeDate, atu.TranslationUnit.SystemFields.ChangeUser, atu.TranslationUnit.SystemFields.UseDate, atu.TranslationUnit.SystemFields.UseUser, atu.TranslationUnit.SystemFields.UseCount, ResourceManager.GetTUFlags(atu.TranslationUnit), TokenSerialization.SaveTokens(atu.Source.Segment), TokenSerialization.SaveTokens(atu.Target.Segment), atu.TranslationUnit.AlignmentData?.Save(), atu.TranslationUnit.AlignModelDate, atu.TranslationUnit.InsertDate, serializationVersion, atu.TranslationUnit.Format, atu.TranslationUnit.Origin, atu.TranslationUnit.ConfirmationLevel);
			_context.Storage.SerializeTuSegments(atu.TranslationUnit, translationUnit);
			if (translationUnit.SerializationVersion == 1)
			{
				CheckSegmentSerialization(atu.TranslationUnit.SourceSegment, translationUnit.Source, translationUnit.SourceTokenData);
				CheckSegmentSerialization(atu.TranslationUnit.SourceSegment, translationUnit.Target, translationUnit.TargetTokenData);
			}
			translationUnit.Source.Features = atu.Source.TmFeatureVector;
			translationUnit.Source.ConcordanceFeatures = atu.Source.ConcordanceFeatureVector;
			translationUnit.Contexts = atu.TranslationUnit.Contexts;
			translationUnit.IdContexts = atu.TranslationUnit.IdContexts;
			if ((Tm.Tm.FuzzyIndexes & FuzzyIndexes.TargetWordBased) != 0)
			{
				translationUnit.Target.Features = atu.Target.TmFeatureVector;
			}
			if ((Tm.Tm.FuzzyIndexes & FuzzyIndexes.TargetCharacterBased) != 0)
			{
				translationUnit.Target.ConcordanceFeatures = atu.Target.ConcordanceFeatureVector;
			}
			return translationUnit;
		}

		private static void RenderSegment(Sdl.LanguagePlatform.Core.Segment s, StringBuilder b)
		{
			if (s == null)
			{
				b.Append("Segment is null");
			}
			else if (s.Elements == null)
			{
				b.Append("Segment.Elements is null");
			}
			else
			{
				foreach (SegmentElement element in s.Elements)
				{
					b.Append(Environment.NewLine);
					if (element == null)
					{
						b.Append("Element is null");
					}
					else
					{
						b.Append(element.GetType().Name + " " + element?.ToString());
					}
				}
			}
		}

		internal static bool CheckSegmentSerialization(Sdl.LanguagePlatform.Core.Segment memSegment, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.Segment storageSegment, byte[] tokenData)
		{
			Sdl.LanguagePlatform.Core.Segment segment = null;
			try
			{
				segment = SegmentSerialization.Load(storageSegment, memSegment.Culture);
				TokenSerialization.LoadTokens(tokenData, segment);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.Logger.Log(ex);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Exception checking segment serialization. Memory segment was: ");
				RenderSegment(memSegment, stringBuilder);
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("Deserialized segment was: ");
				RenderSegment(segment, stringBuilder);
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("Memory tokens were: ");
				if (memSegment.Tokens == null)
				{
					LogManager.Logger.LogError("null collection");
					stringBuilder.Append(Environment.NewLine);
				}
				else
				{
					foreach (Token token in memSegment.Tokens)
					{
						stringBuilder.Append(Environment.NewLine);
						stringBuilder.Append((token == null) ? "null" : token.ToString());
					}
					stringBuilder.Append(Environment.NewLine);
				}
				LogManager.Logger.LogError(stringBuilder.ToString());
				return false;
			}
		}

		public void Reindex(AnnotatedTranslationUnit[] atus)
		{
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tus = atus.Select(GetStorageTuForReindex).ToList();
			_context.Storage.UpdateTuIndices(tus, Tm.Tm.FuzzyIndexes, _tokenizationSignatureHash, Tm.Tm.TextContextMatchType);
		}

		internal Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit GetStorageTuForReindex(AnnotatedTranslationUnit atu)
		{
			atu.TranslationUnit.SystemFields.CreationDate = DateTime.UtcNow;
			atu.TranslationUnit.SystemFields.ChangeDate = DateTime.UtcNow;
			atu.TranslationUnit.SystemFields.UseDate = DateTime.UtcNow;
			Tm.SourceTools.EnsureTokenizedSegment(atu.Source.Segment, forceRetokenization: true, allowTokenBundles: true);
			Tm.TargetTools.EnsureTokenizedSegment(atu.Target.Segment, forceRetokenization: true, allowTokenBundles: true);
			PlaceableComputer.ComputePlaceables(atu.Source.Segment, atu.Target.Segment);
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu = GetStorageTu(atu);
			storageTu.Id = atu.TranslationUnit.ResourceId.Id;
			storageTu.Guid = atu.TranslationUnit.ResourceId.Guid;
			return storageTu;
		}

		public void Reindex(AnnotatedTranslationUnit atu)
		{
			Reindex(new AnnotatedTranslationUnit[1]
			{
				atu
			});
		}

		private Sdl.LanguagePlatform.TranslationMemory.Action MergeFieldValuesAndContexts(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit victim, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit importTu, ImportSettings settings, TuContext tuContext)
		{
			if (victim == null || importTu == null)
			{
				throw new ArgumentNullException();
			}
			bool merged = false;
			switch (GetCustomFieldUpdateMode(settings))
			{
			case ImportSettings.FieldUpdateMode.Merge:
				merged |= victim.FieldValues.Merge(importTu.FieldValues);
				MergeContexts(victim, importTu, ref merged);
				break;
			case ImportSettings.FieldUpdateMode.Overwrite:
				merged |= victim.FieldValues.Assign(importTu.FieldValues);
				MergeContexts(victim, importTu, ref merged);
				break;
			}
			if (tuContext != null)
			{
				merged |= victim.Contexts.Merge(tuContext);
			}
			if (importTu.SystemFields.UseDate - victim.SystemFields.UseDate > TimeSpan.FromSeconds(1.0))
			{
				merged = true;
			}
			if (!merged)
			{
				return Sdl.LanguagePlatform.TranslationMemory.Action.Discard;
			}
			return Sdl.LanguagePlatform.TranslationMemory.Action.Merge;
		}

		private static void MergeContexts(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit victim, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit importTu, ref bool merged)
		{
			if (importTu.Contexts != null && importTu.Contexts.Length != 0)
			{
				if (victim.Contexts == null || victim.Contexts.Length == 0)
				{
					victim.Contexts = new TuContexts(importTu.Contexts);
					merged = true;
				}
				else
				{
					merged |= victim.Contexts.Merge(importTu.Contexts);
				}
			}
			if (importTu.IdContexts != null && importTu.IdContexts.Length != 0)
			{
				if (victim.IdContexts == null || victim.IdContexts.Length == 0)
				{
					victim.IdContexts = new TuIdContexts(importTu.IdContexts);
					merged = true;
				}
				else
				{
					merged |= victim.IdContexts.Merge(importTu.IdContexts);
				}
			}
		}

		private static ImportSettings.FieldUpdateMode GetCustomFieldUpdateMode(ImportSettings settings)
		{
			if (settings == null)
			{
				return ImportSettings.FieldUpdateMode.Merge;
			}
			if (settings.ExistingTUsUpdateMode != ImportSettings.TUUpdateMode.Overwrite)
			{
				return settings.ExistingFieldsUpdateMode;
			}
			return ImportSettings.FieldUpdateMode.Overwrite;
		}

		private void CreatePreviousTranslationSearcher()
		{
			SearchSettings settings = new SearchSettings
			{
				ComputeTranslationProposal = false,
				IsDocumentSearch = false,
				MaxResults = 100,
				MinScore = 100,
				SortSpecification = new SortSpecification("Sco/D"),
				Mode = SearchMode.ExactSearch
			};
			_previousTranslationSearcher = new Searcher(_context, Tm.Tm.ResourceId, settings);
		}

		private Sdl.LanguagePlatform.TranslationMemory.TranslationUnit SearchPreviousTranslations(AnnotatedTranslationUnit atu, int previousTranslationHash, out SearchResults results, out bool idContextMatch)
		{
			idContextMatch = false;
			if (_previousTranslationSearcher == null)
			{
				CreatePreviousTranslationSearcher();
			}
			results = _previousTranslationSearcher.Search(atu);
			if (results == null)
			{
				return null;
			}
			results.Results = results.Results.FindAll((SearchResult x) => !x.ScoringResult.TagMismatch && !x.ScoringResult.MemoryTagsDeleted);
			foreach (SearchResult item in (IEnumerable<SearchResult>)results)
			{
				if (item.ScoringResult.IsExactMatch && item.ScoringResult.IdContextMatch)
				{
					idContextMatch = true;
					return item.MemoryTranslationUnit;
				}
			}
			if (previousTranslationHash == 0)
			{
				return null;
			}
			return (from sr in results
				where sr.MemoryTranslationUnit.TargetSegment.GetWeakHashCode() == previousTranslationHash
				select sr.MemoryTranslationUnit).FirstOrDefault();
		}
	}
}
