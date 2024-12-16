#define TRACE
using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using Sdl.LanguagePlatform.TranslationMemoryImpl.FGA;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class API : ISubsegmentTranslationMemoryService, ITranslationMemoryService, IDisposable, ITranslationModelService, IAlignableTranslationMemoryService, IReindexableTranslationMemoryService
	{
		internal static Func<ITranslationModelDataService> GetTranslationModelDataService;

		private const string _ExternalAlignerKey = "ExternalAligner";

		private static bool? _externalAligner;

		private static readonly object _locker;

		public static readonly string TM8TraceCategory;

		private readonly TraceSource _TM8TraceSource = new TraceSource(TM8TraceCategory, SourceLevels.Warning);

		private readonly AnnotatedTmManager _ATMManager;

		private int _TULimit;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		internal static bool UseExternalAligner()
		{
			if (_externalAligner.HasValue)
			{
				return _externalAligner.Value;
			}
			lock (_locker)
			{
				if (_externalAligner.HasValue)
				{
					return _externalAligner.Value;
				}
				_externalAligner = false;
				string text = ConfigurationManager.AppSettings["ExternalAligner"];
				if (text != null)
				{
					_externalAligner = (string.CompareOrdinal(text, "true") != 0);
				}
				return _externalAligner.Value;
			}
		}

		private void Log(Exception e)
		{
			_TM8TraceSource.TraceEvent(TraceEventType.Error, 0, e.ToString());
		}

		private static bool HasFGASupport(FGASupport fgaSupport)
		{
			if (fgaSupport != FGASupport.NonAutomatic)
			{
				return fgaSupport == FGASupport.Automatic;
			}
			return true;
		}

		private void Log(FaultDescription f)
		{
			TraceEventType eventType;
			switch (f.Status)
			{
			case FaultStatus.Success:
				eventType = TraceEventType.Information;
				break;
			case FaultStatus.Warning:
				eventType = TraceEventType.Warning;
				break;
			case FaultStatus.Error:
				eventType = TraceEventType.Error;
				break;
			case FaultStatus.Fatal:
				eventType = TraceEventType.Critical;
				break;
			default:
				eventType = TraceEventType.Error;
				break;
			}
			string message = $"{f.Message} (Status {f.Status})";
			_TM8TraceSource.TraceEvent(eventType, 0, message);
		}

		public API()
		{
			_ATMManager = new AnnotatedTmManager();
		}

		static API()
		{
			GetTranslationModelDataService = delegate
			{
				throw new NotImplementedException();
			};
			_locker = new object();
			TM8TraceCategory = "Sdl.LanguagePlatform.TranslationMemory";
		}

		private CallContext CreateCallContext(Container container, string methodName)
		{
			CallContext callContext = new CallContext(container, null, _ATMManager);
			if (callContext.IsFilebasedTm && _TULimit == 0)
			{
				_TULimit = -1;
			}
			callContext.TuLimit = _TULimit;
			if ((callContext.IsInMemoryTm || callContext.IsFilebasedTm) && !_ATMManager.IsCacheEnabled)
			{
				_ATMManager.EnableCache();
			}
			return callContext;
		}

		public bool CreateSchema(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "CreateSchema"))
				{
					_ATMManager.Clear();
					callContext.ResourceManager.CreateSchema();
					callContext.Complete();
					return true;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool CleanContainerSchema(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "CleanContainerSchema"))
				{
					_ATMManager.Clear();
					callContext.ResourceManager.CleanSchema();
					callContext.Complete();
					return true;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool DropSchema(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "DropSchema"))
				{
					_ATMManager.Clear();
					callContext.ResourceManager.DropSchema();
					callContext.Complete();
					return true;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool SchemaExists(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "SchemaExists"))
				{
					bool result = callContext.ResourceManager.SchemaExists();
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void RecomputeStatistics(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "RecomputeStatistics"))
				{
					callContext.ResourceManager.RecomputeStatistics(tmId.Id);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public ImportResult AddTranslationUnit(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, ImportSettings settings)
		{
			return AddOrUpdateTranslationUnit(container, tmId, tu, 0, settings);
		}

		public ImportResult AddOrUpdateTranslationUnit(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, int previousTranslationHash, ImportSettings settings)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tu == null)
				{
					throw new ArgumentNullException("tu");
				}
				using (CallContext callContext = CreateCallContext(container, "AddOrUpdateTranslationUnit"))
				{
					_ = settings?.NewFields;
					Clean(tu);
					Importer importer = callContext.GetImporter(tmId);
					ImportResult result = importer.ImportInternal(importParameters: new ImportParameters
					{
						IsBatch = false,
						Type = ImportType.Add,
						TuContext = null,
						RetainFgaInfo = false,
						PreviousTranslationHash = previousTranslationHash
					}, atu: new AnnotatedTranslationUnit(importer.Tm, tu, keepTokens: false, keepPeripheralWhitespace: false), settings: settings);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public ImportResult[] AddTranslationUnits(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, ImportSettings settings)
		{
			return AddOrUpdateTranslationUnitsMasked(container, tmId, tus, null, settings, null);
		}

		public ImportResult[] AddOrUpdateTranslationUnits(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, int[] previousTranslationHashes, ImportSettings settings)
		{
			return AddOrUpdateTranslationUnitsMasked(container, tmId, tus, previousTranslationHashes, settings, null);
		}

		public ImportResult[] AddTranslationUnitsMasked(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, ImportSettings settings, bool[] mask)
		{
			return AddOrUpdateTranslationUnitsMasked(container, tmId, tus, null, settings, mask);
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tus == null)
				{
					throw new ArgumentNullException("tus");
				}
				if (mask != null && mask.Length != tus.Length)
				{
					throw new ArgumentException("If provided, the mask must have the same cardinality as the TU collection.");
				}
				if (previousTranslationHashes != null && previousTranslationHashes.Length != tus.Length)
				{
					throw new ArgumentException("If provided, the array of previous translation hashes must have the same cardinality as the TU collection.");
				}
				_ = settings?.NewFields;
				using (CallContext callContext = CreateCallContext(container, "AddOrUpdateTranslationUnitsMasked"))
				{
					Importer importer = callContext.GetImporter(tmId);
					if (tus.Length > 10)
					{
						importer.SkipSynchronousFga = true;
					}
					AnnotatedTranslationUnit[] array = new AnnotatedTranslationUnit[tus.Length];
					for (int i = 0; i < tus.Length; i++)
					{
						Clean(tus[i]);
						array[i] = ((tus[i] == null) ? null : new AnnotatedTranslationUnit(importer.Tm, tus[i], keepTokens: false, keepPeripheralWhitespace: false));
					}
					ImportResult[] array2 = importer.Import(array, previousTranslationHashes, settings, mask, isUpdate: false);
					if (importer.DeleteResults != null && importer.DeleteResults.Any())
					{
						List<ImportResult> list = array2.ToList();
						list.AddRange(importer.DeleteResults);
						importer.DeleteResults = new List<ImportResult>();
						array2 = list.ToArray();
					}
					callContext.Complete();
					return array2;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool? GetReindexRequired(Container container, PersistentObjectToken tmId)
		{
			using (CallContext callContext = CreateCallContext(container, "GetReindexRequired"))
			{
				return callContext.ResourceManager.GetReindexRequired(tmId);
			}
		}

		public int GetTuCountForReindex(Container container, PersistentObjectToken tmId)
		{
			using (CallContext callContext = CreateCallContext(container, "GetTuCountForReindex"))
			{
				return callContext.ResourceManager.GetTuCountForReindex(tmId);
			}
		}

		public void SelectiveReindexTranslationUnits(Container container, PersistentObjectToken tmId, CancellationToken token, IProgress<int> progress)
		{
			using (CallContext callContext = CreateCallContext(container, "SelectiveReindexTranslationUnits"))
			{
				callContext.ResourceManager.SelectiveReindexTranslationUnits(tmId, token, progress);
			}
		}

		public ImportResult[] UpdateAlignmentData(Container container, PersistentObjectToken tmId, TuAlignmentData[] alignmentData)
		{
			try
			{
				List<ImportResult> list = new List<ImportResult>();
				if (alignmentData.Length != 0)
				{
					using (CallContext callContext = CreateCallContext(container, "UpdateAlignmentData"))
					{
						List<int> list2 = (from tuAlignmentData in alignmentData
							where tuAlignmentData.AlignmentData != null
							select tuAlignmentData.tuId.Id).ToList();
						if (list2.Count == 0)
						{
							return list.ToArray();
						}
						List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> alignableTus = callContext.AlignableStorage.GetAlignableTus(tmId.Id, list2);
						Dictionary<int, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> dictionary = new Dictionary<int, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
						AnnotatedTranslationMemory annotatedTranslationMemory = callContext.GetAnnotatedTranslationMemory(tmId);
						foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in alignableTus)
						{
							dictionary.Add(item.Id, callContext.ResourceManager.GetAlignableTu(item, annotatedTranslationMemory.Tm.LanguageDirection.SourceCulture, annotatedTranslationMemory.Tm.LanguageDirection.TargetCulture));
						}
						List<TuAlignmentDataInternal> list3 = new List<TuAlignmentDataInternal>();
						foreach (TuAlignmentData tuAlignmentData2 in alignmentData)
						{
							if (dictionary.ContainsKey(tuAlignmentData2.tuId.Id))
							{
								Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = dictionary[tuAlignmentData2.tuId.Id];
								translationUnit.AlignmentData = tuAlignmentData2.AlignmentData;
								translationUnit.AlignModelDate = tuAlignmentData2.AlignModelDate;
								translationUnit.InsertDate = tuAlignmentData2.InsertDate;
								AnnotatedTranslationUnit tu = new AnnotatedTranslationUnit(annotatedTranslationMemory, translationUnit, keepTokens: true, keepPeripheralWhitespace: false);
								TuAlignmentDataInternal alignmentDataInternal = ResourceManager.GetAlignmentDataInternal(annotatedTranslationMemory.Tm, tu);
								list3.Add(alignmentDataInternal);
							}
						}
						bool[] array = callContext.AlignableStorage.UpdateTuAlignmentData(list3, tmId.Id);
						int num = 0;
						bool[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							ImportResult importResult = array2[i] ? new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Add) : new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Discard);
							importResult.TuId = new PersistentObjectToken(list2[num], Guid.NewGuid());
							list.Add(importResult);
							num++;
						}
						callContext.Complete();
						return list.ToArray();
					}
				}
				return list.ToArray();
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public ImportResult UpdateTranslationUnit(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tu == null)
				{
					throw new ArgumentNullException("tu");
				}
				if (tu.ResourceId == null || tu.ResourceId.Id == 0)
				{
					throw new ArgumentNullException("ResourceId");
				}
				using (CallContext callContext = CreateCallContext(container, "UpdateTranslationUnit"))
				{
					Clean(tu);
					ImportResult result = callContext.ResourceManager.UpdateTranslationUnit(tmId, tu);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public ImportResult[] UpdateTranslationUnits(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus)
		{
			return UpdateTranslationUnitsMasked(container, tmId, tus, null);
		}

		public ImportResult[] UpdateTranslationUnitsMasked(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, bool[] mask)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tus == null)
				{
					throw new ArgumentNullException("tus");
				}
				if (mask != null && mask.Length != tus.Length)
				{
					throw new ArgumentException("If provided, the mask must have the same cardinality as the TU collection.");
				}
				using (CallContext callContext = CreateCallContext(container, "UpdateTranslationUnitsMasked"))
				{
					Clean(tus);
					ImportResult[] result = callContext.ResourceManager.UpdateTranslationUnits(tmId, tus, mask);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit GetTranslationUnit(Container container, PersistentObjectToken tmId, PersistentObjectToken tuId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuId == null || tuId.Id == 0)
				{
					throw new ArgumentNullException("tuId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationUnit"))
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = callContext.ResourceManager.GetTranslationUnit(tmId, tuId);
					callContext.Complete();
					return translationUnit;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] GetAlignableTranslationUnits(Container container, PersistentObjectToken tmId, List<int> tuIds)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuIds == null)
				{
					throw new ArgumentNullException("tuIds");
				}
				using (CallContext callContext = CreateCallContext(container, "GetAlignableTranslationUnits"))
				{
					List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = callContext.ResourceManager.GetAlignableTranslationUnits(tmId, tuIds);
					callContext.Complete();
					if (list == null)
					{
						list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
					}
					return list.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] GetFullTranslationUnits(Container container, PersistentObjectToken tmId, List<int> tuIds)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuIds == null)
				{
					throw new ArgumentNullException("tuIds");
				}
				using (CallContext callContext = CreateCallContext(container, "GetFullTranslationUnits"))
				{
					List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = callContext.ResourceManager.GetFullTranslationUnits(tmId, tuIds);
					callContext.Complete();
					if (list == null)
					{
						list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
					}
					return list.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] GetAlignableTranslationUnits(Container container, PersistentObjectToken tmId, ref RegularIterator iter, bool unalignedOnly, bool unalignedOrPostdated)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iter == null)
				{
					throw new ArgumentNullException("iter");
				}
				using (CallContext callContext = CreateCallContext(container, "GetAlignableTranslationUnits"))
				{
					List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> list = callContext.ResourceManager.GetTusForAlignment(tmId, iter, unalignedOnly, unalignedOrPostdated);
					callContext.Complete();
					if (list == null)
					{
						list = new List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit>();
					}
					return list.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit ExtractFragment(Container container, PersistentObjectToken tmId, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, SegmentRange sourceRange, SegmentRange targetRange)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "ExtractFragment"))
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = tu.ExtractFragment(sourceRange, targetRange);
					callContext.GetAnnotatedTranslationMemory(tmId).SourceTools.EnsureTokenizedSegment(translationUnit.SourceSegment, forceRetokenization: false, allowTokenBundles: true);
					return translationUnit;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] GetTranslationUnits(Container container, PersistentObjectToken tmId, ref RegularIterator iter)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iter == null)
				{
					throw new ArgumentNullException("iter");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationUnits"))
				{
					List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> translationUnits = callContext.ResourceManager.GetTranslationUnits(tmId, iter);
					callContext.Complete();
					return translationUnits.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] GetTranslationUnitsWithContextContent(Container container, PersistentObjectToken tmId, ref RegularIterator iter)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iter == null)
				{
					throw new ArgumentNullException("iter");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationUnitsWithContextContent"))
				{
					Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = callContext.Storage.GetTm(tmId.Id);
					List<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> translationUnitsWithContextContent = callContext.ResourceManager.GetTranslationUnitsWithContextContent(tmId, iter, tm.TextContextMatchType, tm.LanguageDirection.SourceCulture, tm.LanguageDirection.TargetCulture, tm.IdContextMatch);
					callContext.Complete();
					return translationUnitsWithContextContent.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool ReindexTranslationUnits(Container container, PersistentObjectToken tmId, ref RegularIterator iter)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iter == null)
				{
					throw new ArgumentNullException("iter");
				}
				using (CallContext callContext = CreateCallContext(container, "ReindexTranslationUnits"))
				{
					if (callContext.ResourceManager.ReindexTranslationUnits(tmId, iter))
					{
						callContext.Complete();
						return true;
					}
					callContext.Complete();
					return false;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void ReindexTranslationUnits(Container container, PersistentObjectToken tmId, CancellationToken token, IProgress<int> progress)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "ReindexTranslationUnits"))
				{
					callContext.ResourceManager.ReindexTranslationUnits(tmId, token, progress, selective: false);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SearchResults SearchTranslationUnit(Container container, PersistentObjectToken tmId, SearchSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (tu == null)
				{
					throw new ArgumentNullException("tu");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchTranslationUnit"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					Clean(tu);
					SearchResults result = searcher.Search(new AnnotatedTranslationUnit(searcher.TM, tu, keepTokens: false, keepPeripheralWhitespace: true));
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SegmentAndSubsegmentSearchResults SearchTranslationUnit(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegSettings, SubsegmentSearchCondition condition, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (tu == null)
				{
					throw new ArgumentNullException("tu");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchTranslationUnit"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					SubsegmentSearcher subsegmentSearcher = null;
					if (subsegSettings != null)
					{
						subsegmentSearcher = new SubsegmentSearcher(callContext, tmId, subsegSettings);
					}
					Clean(tu);
					SearchResults searchResults = searcher.Search(new AnnotatedTranslationUnit(searcher.TM, tu, keepTokens: false, keepPeripheralWhitespace: true));
					SubsegmentSearchResultsCollection subsegResults = null;
					if (subsegSettings != null && (condition == SubsegmentSearchCondition.AlwaysSearch || !searchResults.Results.Exists((SearchResult x) => x.ScoringResult.IsExactMatch)))
					{
						subsegResults = subsegmentSearcher.Search(new AnnotatedSegment(searcher.TM, tu.SourceSegment, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true));
					}
					callContext.Complete();
					return new SegmentAndSubsegmentSearchResults(searchResults, subsegResults);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SearchResults[] SearchTranslationUnits(Container container, PersistentObjectToken tmId, SearchSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus)
		{
			return SearchTranslationUnitsMasked(container, tmId, settings, tus, null);
		}

		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegSettings, SubsegmentSearchCondition condition, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus)
		{
			return SearchTranslationUnitsMasked(container, tmId, settings, subsegSettings, condition, tus, null);
		}

		public SearchResults[] SearchTranslationUnitsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, bool[] mask)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (tus == null)
				{
					throw new ArgumentNullException("tus");
				}
				if (mask != null && mask.Length != tus.Length)
				{
					throw new ArgumentException("If provided, the mask must have the same cardinality as the search TU collection.");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchTranslationUnitsMasked"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					AnnotatedTranslationUnit[] array = new AnnotatedTranslationUnit[tus.Length];
					for (int i = 0; i < tus.Length; i++)
					{
						try
						{
							Clean(tus[i]);
							array[i] = ((tus[i] == null) ? null : new AnnotatedTranslationUnit(searcher.TM, tus[i], keepTokens: false, keepPeripheralWhitespace: true));
						}
						catch
						{
							array[i] = null;
						}
					}
					SearchResults[] result = searcher.Search(array, mask);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SearchResults[] SearchTranslationUnitsBatch(Container container, PersistentObjectToken tmId, SearchSettings settings, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, bool[] mask, int batchSize, bool overwriteTranslation)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (tus == null)
				{
					throw new ArgumentNullException("tus");
				}
				if (batchSize <= 0)
				{
					throw new Exception("argument batchSize should be > 0 ");
				}
				using (CallContext callContext = CreateCallContext(container, "ExactSearchTranslationUnitsBatch"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					AnnotatedTranslationUnit[] array = new AnnotatedTranslationUnit[tus.Length];
					for (int i = 0; i < tus.Length; i++)
					{
						try
						{
							Clean(tus[i]);
							array[i] = ((tus[i] == null) ? null : new AnnotatedTranslationUnit(searcher.TM, tus[i], keepTokens: false, keepPeripheralWhitespace: true));
						}
						catch
						{
							array[i] = null;
						}
					}
					FieldDefinitions fields = callContext.ResourceManager.GetFields(tmId);
					List<SearchResults> list = searcher.SearchBatch(array, mask, batchSize, fields, overwriteTranslation);
					callContext.Complete();
					return list.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegSettings, SubsegmentSearchCondition condition, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] tus, bool[] mask)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (tus == null)
				{
					throw new ArgumentNullException("tus");
				}
				if (mask != null && mask.Length != tus.Length)
				{
					throw new ArgumentException("If provided, the mask must have the same cardinality as the search TU collection.");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchTranslationUnitsMasked"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					SubsegmentSearcher subsegmentSearcher = null;
					if (subsegSettings != null)
					{
						subsegmentSearcher = new SubsegmentSearcher(callContext, tmId, subsegSettings);
					}
					AnnotatedTranslationUnit[] array = new AnnotatedTranslationUnit[tus.Length];
					AnnotatedSegment[] array2 = new AnnotatedSegment[tus.Length];
					for (int i = 0; i < tus.Length; i++)
					{
						try
						{
							Clean(tus[i]);
							array[i] = ((tus[i] == null) ? null : new AnnotatedTranslationUnit(searcher.TM, tus[i], keepTokens: false, keepPeripheralWhitespace: true));
							array2[i] = ((tus[i] == null) ? null : new AnnotatedSegment(searcher.TM, tus[i].SourceSegment, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true));
						}
						catch
						{
							array[i] = null;
							array2[i] = null;
						}
					}
					SearchResults[] array3 = searcher.Search(array, mask);
					SubsegmentSearchResultsCollection[] subsegResults = null;
					bool[] array4 = mask;
					if (subsegSettings != null)
					{
						if (condition == SubsegmentSearchCondition.SearchIfNoExactSegmentMatch)
						{
							array4 = new bool[tus.Length];
							for (int j = 0; j < tus.Length; j++)
							{
								if (array3[j] != null)
								{
									array4[j] = !array3[j].Results.Exists((SearchResult x) => x.ScoringResult.IsExactMatch);
								}
							}
						}
						subsegResults = subsegmentSearcher.Search(array2, array4);
					}
					callContext.Complete();
					return CombineResults(array3, subsegResults);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SearchResults SearchSegment(Container container, PersistentObjectToken tmId, SearchSettings settings, Sdl.LanguagePlatform.Core.Segment segment)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (segment == null)
				{
					throw new ArgumentNullException("segment");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchSegment"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					Clean(segment);
					SearchResults result = searcher.Search(new AnnotatedSegment(searcher.TM, segment, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true));
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SegmentAndSubsegmentSearchResults SearchSegment(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegSettings, SubsegmentSearchCondition condition, Sdl.LanguagePlatform.Core.Segment segment)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (segment == null)
				{
					throw new ArgumentNullException("segment");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchSegment"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					SubsegmentSearcher subsegmentSearcher = null;
					if (subsegSettings != null)
					{
						subsegmentSearcher = new SubsegmentSearcher(callContext, tmId, subsegSettings);
					}
					Clean(segment);
					AnnotatedSegment annotatedSegment = new AnnotatedSegment(searcher.TM, segment, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true);
					SearchResults searchResults = searcher.Search(annotatedSegment);
					SubsegmentSearchResultsCollection subsegResults = null;
					if (subsegSettings != null && (condition == SubsegmentSearchCondition.AlwaysSearch || !searchResults.Results.Exists((SearchResult x) => x.ScoringResult.IsExactMatch)))
					{
						subsegResults = subsegmentSearcher.Search(annotatedSegment);
					}
					SegmentAndSubsegmentSearchResults result = new SegmentAndSubsegmentSearchResults(searchResults, subsegResults);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SubsegmentSearchResultsCollection SubsegmentSearchSegment(Container container, PersistentObjectToken tmId, SubsegmentSearchSettings settings, Sdl.LanguagePlatform.Core.Segment segment)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (segment == null)
				{
					throw new ArgumentNullException("segment");
				}
				if (!settings.ComputeTranslationProposal)
				{
					throw new Exception("ComputeTranslationProposal must be true for SubsegmentSearchSegment");
				}
				using (CallContext callContext = CreateCallContext(container, "SubsegmentSearchSegment"))
				{
					SubsegmentSearcher subsegmentSearcher = new SubsegmentSearcher(callContext, tmId, settings);
					Clean(segment);
					SubsegmentSearchResultsCollection result = subsegmentSearcher.Search(new AnnotatedSegment(subsegmentSearcher.TM, segment, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true));
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(Container container, PersistentObjectToken tmId, SubsegmentSearchSettings settings, Sdl.LanguagePlatform.Core.Segment[] segments)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (segments == null || segments.Length == 0)
				{
					throw new ArgumentNullException("segments");
				}
				if (!settings.ComputeTranslationProposal)
				{
					throw new Exception("ComputeTranslationProposal must be true for SubsegmentSearchSegment");
				}
				using (CallContext callContext = CreateCallContext(container, "SubsegmentSearchSegment"))
				{
					SubsegmentSearcher subsegmentSearcher = new SubsegmentSearcher(callContext, tmId, settings);
					AnnotatedSegment[] array = new AnnotatedSegment[segments.Length];
					bool[] array2 = new bool[segments.Length];
					for (int i = 0; i < segments.Length; i++)
					{
						Clean(segments[i]);
						array[i] = ((segments[i] == null) ? null : new AnnotatedSegment(subsegmentSearcher.TM, segments[i], isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true));
						array2[i] = true;
					}
					SubsegmentSearchResultsCollection[] result = subsegmentSearcher.Search(array, array2);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SearchResults SearchText(Container container, PersistentObjectToken tmId, SearchSettings settings, string segment)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (!string.IsNullOrEmpty(segment))
				{
					using (CallContext callContext = CreateCallContext(container, "SearchText"))
					{
						SearchResults result = new Searcher(callContext, tmId, settings).Search(segment);
						callContext.Complete();
						return result;
					}
				}
				return null;
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public SearchResults[] SearchSegments(Container container, PersistentObjectToken tmId, SearchSettings settings, Sdl.LanguagePlatform.Core.Segment[] segments)
		{
			return SearchSegmentsMasked(container, tmId, settings, segments, null);
		}

		public SegmentAndSubsegmentSearchResults[] SearchSegments(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegSettings, SubsegmentSearchCondition condition, Sdl.LanguagePlatform.Core.Segment[] segments)
		{
			return SearchSegmentsMasked(container, tmId, settings, subsegSettings, condition, segments, null);
		}

		public SearchResults[] SearchSegmentsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, Sdl.LanguagePlatform.Core.Segment[] segments, bool[] mask)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (mask != null && mask.Length != segments.Length)
				{
					throw new ArgumentException("If provided, the mask must have the same cardinality as the search segment collection.");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchSegmentsMasked"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					AnnotatedSegment[] array = new AnnotatedSegment[segments.Length];
					for (int i = 0; i < segments.Length; i++)
					{
						Clean(segments[i]);
						array[i] = ((segments[i] == null) ? null : new AnnotatedSegment(searcher.TM, segments[i], isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true));
					}
					SearchResults[] result = searcher.Search(array, mask);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		private SegmentAndSubsegmentSearchResults[] CombineResults(IReadOnlyList<SearchResults> segmentResults, IReadOnlyList<SubsegmentSearchResultsCollection> subsegResults)
		{
			SegmentAndSubsegmentSearchResults[] array = new SegmentAndSubsegmentSearchResults[segmentResults.Count];
			for (int i = 0; i < segmentResults.Count; i++)
			{
				if (segmentResults[i] != null)
				{
					array[i] = new SegmentAndSubsegmentSearchResults(segmentResults[i], (subsegResults != null) ? subsegResults[i] : null);
				}
			}
			return array;
		}

		public SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Sdl.LanguagePlatform.Core.Segment[] segments, bool[] mask)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				if (mask != null && mask.Length != segments.Length)
				{
					throw new ArgumentException("If provided, the mask must have the same cardinality as the search segment collection.");
				}
				using (CallContext callContext = CreateCallContext(container, "SearchSegmentsMasked"))
				{
					Searcher searcher = new Searcher(callContext, tmId, settings);
					SubsegmentSearcher subsegmentSearcher = null;
					if (subsegmentSettings != null)
					{
						subsegmentSearcher = new SubsegmentSearcher(callContext, tmId, subsegmentSettings);
					}
					AnnotatedSegment[] array = new AnnotatedSegment[segments.Length];
					for (int i = 0; i < segments.Length; i++)
					{
						Clean(segments[i]);
						array[i] = ((segments[i] == null) ? null : new AnnotatedSegment(searcher.TM, segments[i], isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true));
					}
					SearchResults[] array2 = searcher.Search(array, mask);
					SubsegmentSearchResultsCollection[] subsegResults = null;
					bool[] array3 = mask;
					if (subsegmentSettings != null)
					{
						if (condition == SubsegmentSearchCondition.SearchIfNoExactSegmentMatch)
						{
							array3 = new bool[segments.Length];
							for (int j = 0; j < segments.Length; j++)
							{
								if (array2[j] != null)
								{
									array3[j] = !array2[j].Results.Exists((SearchResult x) => x.ScoringResult.IsExactMatch);
								}
							}
						}
						subsegResults = subsegmentSearcher.Search(array, array3);
					}
					callContext.Complete();
					return CombineResults(array2, subsegResults);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] GetDuplicateTranslationUnits(Container container, PersistentObjectToken tmId, ref DuplicateIterator iter)
		{
			return GetDuplicateTranslationUnits(container, tmId, ref iter, targetSegments: false);
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] GetDuplicateTranslationUnits(Container container, PersistentObjectToken tmId, ref DuplicateIterator iter, bool targetSegments)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iter == null)
				{
					throw new ArgumentNullException("iter");
				}
				using (CallContext callContext = CreateCallContext(container, "GetDuplicateTranslationUnits"))
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] result = callContext.ResourceManager.SearchDuplicateTranslationUnits(tmId, iter, targetSegments);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool DeleteTranslationUnit(Container container, PersistentObjectToken tmId, PersistentObjectToken tuId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuId == null || tuId.Id == 0)
				{
					throw new ArgumentNullException("tuId");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteTranslationUnit"))
				{
					bool result = callContext.ResourceManager.DeleteTranslationUnit(tmId, tuId);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int DeleteTranslationUnits(Container container, PersistentObjectToken tmId, PersistentObjectToken[] tuIds)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuIds == null)
				{
					throw new ArgumentNullException("tuIds");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteTranslationUnits"))
				{
					List<PersistentObjectToken> list = callContext.ResourceManager.DeleteTranslationUnits(tmId, tuIds);
					callContext.Complete();
					return list.Count;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int EditTranslationUnitsWithIterator(Container container, PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (editScript == null)
				{
					throw new ArgumentNullException("editScript");
				}
				if (iterator == null)
				{
					throw new ArgumentNullException("iterator");
				}
				using (CallContext callContext = CreateCallContext(container, "EditTranslationUnitsWithIterator"))
				{
					int result = callContext.ResourceManager.EditTranslationUnits(tmId, editScript, updateMode, ref iterator);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public List<PersistentObjectToken> EditTranslationUnitsWithFilter(Container container, PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (editScript == null)
				{
					throw new ArgumentNullException("editScript");
				}
				if (iterator == null)
				{
					throw new ArgumentNullException("iterator");
				}
				using (CallContext callContext = CreateCallContext(container, "EditTranslationUnitsWithIterator"))
				{
					List<PersistentObjectToken> result = callContext.ResourceManager.EditTranslationUnitsWithFilter(tmId, editScript, updateMode, ref iterator);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] PreviewEditTranslationUnitsWithIterator(Container container, PersistentObjectToken tmId, EditScript editScript, ref RegularIterator iterator)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (editScript == null)
				{
					throw new ArgumentNullException("editScript");
				}
				if (iterator == null)
				{
					throw new ArgumentNullException("iterator");
				}
				using (CallContext callContext = CreateCallContext(container, "PreviewEditTranslationUnitsWithIterator"))
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[] result = callContext.ResourceManager.PreviewEditTranslationUnits(tmId, editScript, ref iterator);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int EditTranslationUnits(Container container, PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] tuIds)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (editScript == null)
				{
					throw new ArgumentNullException("editScript");
				}
				using (CallContext callContext = CreateCallContext(container, "EditTranslationUnits"))
				{
					int result = callContext.ResourceManager.EditTranslationUnits(tmId, editScript, updateMode, tuIds);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		[Obsolete("Use DeleteTranslationUnitsWithIterator instead.")]
		public int DeleteMatchingTranslationUnits(Container container, PersistentObjectToken tmId, FilterExpression filter)
		{
			RegularIterator iterator = new RegularIterator
			{
				Filter = filter,
				Forward = true
			};
			int num = 0;
			int num2;
			do
			{
				num2 = DeleteTranslationUnitsWithIterator(container, tmId, ref iterator);
				num += num2;
			}
			while (num2 > 0);
			return num;
		}

		public int DeleteTranslationUnitsWithIterator(Container container, PersistentObjectToken tmId, ref RegularIterator iterator)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iterator == null)
				{
					throw new ArgumentNullException("iterator");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteTranslationUnitsWithIterator"))
				{
					List<PersistentObjectToken> list = callContext.ResourceManager.DeleteTranslationUnits(tmId, ref iterator);
					callContext.Complete();
					return list.Count;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public List<PersistentObjectToken> DeleteTranslationUnitsWithFilter(Container container, PersistentObjectToken tmId, ref RegularIterator iterator)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iterator == null)
				{
					throw new ArgumentNullException("iterator");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteTranslationUnitsWithFilter"))
				{
					List<PersistentObjectToken> result = callContext.ResourceManager.DeleteTranslationUnits(tmId, ref iterator);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int DeleteAllTranslationUnits(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteAllTranslationUnits"))
				{
					List<PersistentObjectToken> list = callContext.ResourceManager.DeleteAllTranslationUnits(tmId);
					callContext.Complete();
					return list.Count;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool ApplyFieldsToTranslationUnit(Container container, PersistentObjectToken tmId, FieldValues values, bool overwrite, PersistentObjectToken tuId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuId == null || tuId.Id == 0)
				{
					throw new ArgumentNullException("tuId");
				}
				if (values == null)
				{
					throw new ArgumentNullException("values");
				}
				using (CallContext callContext = CreateCallContext(container, "ApplyFieldsToTranslationUnit"))
				{
					int num = callContext.ResourceManager.ApplyFieldsToTus(tmId, values, overwrite, new PersistentObjectToken[1]
					{
						tuId
					});
					callContext.Complete();
					return num > 0;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int ApplyFieldsToTranslationUnits(Container container, PersistentObjectToken tmId, FieldValues values, bool overwrite, PersistentObjectToken[] tuIds)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuIds == null)
				{
					throw new ArgumentNullException("tuIds");
				}
				if (values == null)
				{
					throw new ArgumentNullException("values");
				}
				using (CallContext callContext = CreateCallContext(container, "ApplyFieldsToTranslationUnits"))
				{
					int result = callContext.ResourceManager.ApplyFieldsToTus(tmId, values, overwrite, tuIds);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public TranslationMemorySetup[] GetTranslationMemories(Container container, bool checkPermissions)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetTranslationMemories"))
				{
					TranslationMemorySetup[] translationMemories = callContext.ResourceManager.GetTranslationMemories(checkPermissions);
					callContext.Complete();
					return translationMemories;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken CreateTranslationMemory(Container container, TranslationMemorySetup setup)
		{
			try
			{
				if (setup == null)
				{
					throw new ArgumentNullException("setup");
				}
				using (CallContext callContext = CreateCallContext(container, "CreateTranslationMemory"))
				{
					_ATMManager.Clear();
					List<AlignableCorpusId> list = new List<AlignableCorpusId>();
					if (!callContext.ResourceManager.CreateTranslationMemory(setup, list))
					{
						throw new LanguagePlatformException(ErrorCode.TMAlreadyExists, setup.Name);
					}
					callContext.Complete();
					if (!callContext.IsFilebasedTm || list.Count <= 0)
					{
						return setup.ResourceId;
					}
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						ModelBasedAlignerDefinition definition = new ModelBasedAlignerDefinition(containerBasedTranslationModelService.AddModel("File-based model", list, setup.LanguageDirection.SourceCulture, setup.LanguageDirection.TargetCulture, TranslationModelTypes.ChiSquared));
						callContext.AlignableStorage.SetAlignerDefinition(setup.ResourceId.Id, definition);
						callContext.Complete();
					}
					return setup.ResourceId;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public TranslationMemorySetup GetTranslationMemoryFromId(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationMemoryFromId"))
				{
					TranslationMemorySetup translationMemory = callContext.ResourceManager.GetTranslationMemory(tmId);
					callContext.Complete();
					return translationMemory;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public TranslationMemorySetup GetTranslationMemoryFromName(Container container, string name)
		{
			try
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException("TM Name cannot be null or empty", "name");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationMemoryFromName"))
				{
					TranslationMemorySetup translationMemory = callContext.ResourceManager.GetTranslationMemory(name);
					callContext.Complete();
					return translationMemory;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken GetTranslationMemoryId(Container container, string name)
		{
			try
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException("TM Name cannot be null or empty", "name");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationMemoryId"))
				{
					PersistentObjectToken translationMemoryId = callContext.ResourceManager.GetTranslationMemoryId(name);
					callContext.Complete();
					return translationMemoryId;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool ChangeTranslationMemory(Container container, TranslationMemorySetup setup)
		{
			return ChangeTranslationMemory(container, setup, null, CancellationToken.None);
		}

		public bool RecoverJAZHCMInfo(Container container, PersistentObjectToken tmId, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken)
		{
			return RecoverJAZHCMInfo(container, tmId, progress, cancellationToken, null);
		}

		public bool RecoverJAZHCMInfo(Container container, PersistentObjectToken tmId, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken, bool? skipConversionChecks)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "RecoverCMInfo"))
				{
					bool result = callContext.ResourceManager.RecoverJAZHCMInfo(tmId, progress, cancellationToken);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool ChangeTranslationMemory(Container container, TranslationMemorySetup setup, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken)
		{
			try
			{
				if (setup == null)
				{
					throw new ArgumentNullException("setup");
				}
				if (setup.ResourceId == null || setup.ResourceId.Id == 0)
				{
					throw new ArgumentException("The object's resource ID is null or has no value", "ResourceId");
				}
				using (CallContext callContext = CreateCallContext(container, "ChangeTranslationMemory"))
				{
					_ATMManager.Clear();
					List<AlignableCorpusId> list = new List<AlignableCorpusId>();
					bool deletedTranslationModel;
					bool result = callContext.ResourceManager.ChangeTranslationMemory(setup, list, out deletedTranslationModel, progress, cancellationToken);
					callContext.Complete();
					_ATMManager.Clear();
					if (!deletedTranslationModel && list.Count <= 0)
					{
						return result;
					}
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						if (deletedTranslationModel)
						{
							ChiSquaredTranslationModelId modelId = new ChiSquaredTranslationModelId
							{
								InternalId = 1
							};
							containerBasedTranslationModelService.DeleteModel(modelId);
						}
						if (list.Count <= 0)
						{
							return result;
						}
						ModelBasedAlignerDefinition definition = new ModelBasedAlignerDefinition(containerBasedTranslationModelService.AddModel("File-based model", list, setup.LanguageDirection.SourceCulture, setup.LanguageDirection.TargetCulture, TranslationModelTypes.ChiSquared));
						callContext.AlignableStorage.SetAlignerDefinition(setup.ResourceId.Id, definition);
						callContext.Complete();
					}
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool DeleteTranslationMemory(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteTranslationMemory"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.DeleteTranslationMemory(tmId);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool DeleteTranslationMemorySchema(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteTranslationMemorySchema"))
				{
					bool result = callContext.ResourceManager.DeleteTranslationMemorySchema(tmId);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int GetTuCount(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTuCount"))
				{
					int tuCount = callContext.ResourceManager.GetTuCount(tmId);
					callContext.Complete();
					if (tuCount <= -1)
					{
						throw new LanguagePlatformException(ErrorCode.TMNotFound, tmId.ToString());
					}
					return tuCount;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public FieldDefinitions GetFields(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetFields"))
				{
					FieldDefinitions fields = callContext.ResourceManager.GetFields(tmId);
					callContext.Complete();
					return fields;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken AddField(Container container, PersistentObjectToken tmId, Field field)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (field == null)
				{
					throw new ArgumentNullException("field");
				}
				using (CallContext callContext = CreateCallContext(container, "AddField"))
				{
					_ATMManager.Clear();
					PersistentObjectToken result = callContext.ResourceManager.AddField(tmId, field);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken[] AddFields(Container container, PersistentObjectToken tmId, FieldDefinitions fields)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (fields == null)
				{
					throw new ArgumentNullException("fields");
				}
				using (CallContext callContext = CreateCallContext(container, "AddFields"))
				{
					_ATMManager.Clear();
					PersistentObjectToken[] result = callContext.ResourceManager.AddFields(tmId, fields) ?? throw new LanguagePlatformException(ErrorCode.TMNotFound);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool RemoveField(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (fieldId == null || fieldId.Id == 0)
				{
					throw new ArgumentNullException("fieldId");
				}
				using (CallContext callContext = CreateCallContext(container, "RemoveField"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.RemoveField(tmId, fieldId);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken AddPicklistValue(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string value)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (fieldId == null || fieldId.Id == 0)
				{
					throw new ArgumentNullException("fieldId");
				}
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				using (CallContext callContext = CreateCallContext(container, "AddPicklistValue"))
				{
					_ATMManager.Clear();
					PersistentObjectToken result = callContext.ResourceManager.AddPicklistValue(tmId, fieldId, value) ?? throw new LanguagePlatformException(ErrorCode.TMNotFound);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken[] AddPicklistValues(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string[] values)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (fieldId == null || fieldId.Id == 0)
				{
					throw new ArgumentNullException("fieldId");
				}
				if (values == null || values.Length == 0)
				{
					throw new ArgumentNullException("values");
				}
				using (CallContext callContext = CreateCallContext(container, "AddPicklistValues"))
				{
					_ATMManager.Clear();
					PersistentObjectToken[] result = callContext.ResourceManager.AddPicklistValues(tmId, fieldId, values) ?? throw new LanguagePlatformException(ErrorCode.TMNotFound);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool RemovePicklistValue(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string value)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (fieldId == null || fieldId.Id == 0)
				{
					throw new ArgumentNullException("fieldId");
				}
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				using (CallContext callContext = CreateCallContext(container, "RemovePicklistValue"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.RemovePicklistValue(tmId, fieldId, value);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool RenamePicklistValue(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string previousName, string newName)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (fieldId == null || fieldId.Id == 0)
				{
					throw new ArgumentNullException("fieldId");
				}
				if (string.IsNullOrEmpty(newName))
				{
					throw new ArgumentNullException("newName");
				}
				using (CallContext callContext = CreateCallContext(container, "RenamePicklistValue"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.RenamePicklistValue(tmId, fieldId, previousName, newName);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool RenameField(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string newName)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (fieldId == null || fieldId.Id == 0)
				{
					throw new ArgumentNullException("fieldId");
				}
				if (string.IsNullOrEmpty(newName))
				{
					throw new ArgumentNullException("newName");
				}
				using (CallContext callContext = CreateCallContext(container, "RenameField"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.RenameField(tmId, fieldId, newName);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public LanguageResource[] GetLanguageResources(Container container, bool includeData)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetLanguageResources"))
				{
					List<LanguageResource> languageResources = callContext.ResourceManager.GetLanguageResources(includeData);
					callContext.Complete();
					return languageResources.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public LanguageResource[] GetTranslationMemoryLanguageResources(Container container, PersistentObjectToken tmId, bool includeData)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationMemoryLanguageResources"))
				{
					List<LanguageResource> languageResources = callContext.ResourceManager.GetLanguageResources(tmId, includeData);
					callContext.Complete();
					return languageResources.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken[] GetTranslationMemoriesByLanguageResource(Container container, PersistentObjectToken resourceId)
		{
			try
			{
				if (resourceId == null || resourceId.Id == 0)
				{
					throw new ArgumentNullException("resourceId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetTranslationMemoriesByLanguageResource"))
				{
					List<PersistentObjectToken> resourceTMs = callContext.ResourceManager.GetResourceTMs(resourceId);
					callContext.Complete();
					return resourceTMs.ToArray();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public LanguageResource GetLanguageResource(Container container, PersistentObjectToken resourceId, bool includeData)
		{
			try
			{
				if (resourceId == null || resourceId.Id == 0)
				{
					throw new ArgumentNullException("resourceId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetLanguageResource"))
				{
					LanguageResource languageResource = callContext.ResourceManager.GetLanguageResource(resourceId, includeData);
					callContext.Complete();
					return languageResource;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public PersistentObjectToken CreateLanguageResource(Container container, LanguageResource resource)
		{
			try
			{
				if (resource == null)
				{
					throw new ArgumentNullException("resource");
				}
				using (CallContext callContext = CreateCallContext(container, "CreateLanguageResource"))
				{
					_ATMManager.Clear();
					if (!callContext.ResourceManager.CreateLanguageResource(resource))
					{
						throw new LanguagePlatformException(ErrorCode.TMResourceAlreadyExists);
					}
					callContext.Complete();
					return resource.ResourceId;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool DeleteLanguageResource(Container container, PersistentObjectToken resourceId)
		{
			try
			{
				if (resourceId == null || resourceId.Id == 0)
				{
					throw new ArgumentNullException("resourceId");
				}
				using (CallContext callContext = CreateCallContext(container, "DeleteLanguageResource"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.DeleteLanguageResource(resourceId);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool UpdateLanguageResource(Container container, LanguageResource resource)
		{
			try
			{
				if (resource == null)
				{
					throw new ArgumentNullException("resource");
				}
				if (resource.ResourceId == null || resource.ResourceId.Id == 0)
				{
					throw new ArgumentNullException("ResourceId");
				}
				using (CallContext callContext = CreateCallContext(container, "UpdateLanguageResource"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.UpdateLanguageResource(resource);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool AssignLanguageResourceToTranslationMemory(Container container, PersistentObjectToken resourceId, PersistentObjectToken tmId)
		{
			try
			{
				if (resourceId == null || resourceId.Id == 0)
				{
					throw new ArgumentNullException("resourceId");
				}
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "AssignLanguageResourceToTranslationMemory"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.AttachTMResource(tmId, resourceId);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool UnassignLanguageResourceFromTranslationMemory(Container container, PersistentObjectToken resourceId, PersistentObjectToken tmId)
		{
			try
			{
				if (resourceId == null || resourceId.Id == 0)
				{
					throw new ArgumentNullException("resourceId");
				}
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "UnassignLanguageResourceFromTranslationMemory"))
				{
					_ATMManager.Clear();
					bool result = callContext.ResourceManager.DetachTmResource(tmId, resourceId);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		private FaultException<FaultDescription> GenerateFault(Exception e)
		{
			LanguagePlatformException ex = e as LanguagePlatformException;
			if (ex == null)
			{
				SqlException ex2 = e as SqlException;
				if (ex2 == null)
				{
					SQLiteException ex3 = e as SQLiteException;
					if (ex3 != null)
					{
						SQLiteException ex4 = ex3;
						Log(ex4);
						ErrorCode code;
						switch (ex4.ErrorCode)
						{
						case 11:
							code = ErrorCode.SQLiteCorrupt;
							break;
						case 1:
							code = ((ex4.Message == null || ex4.Message.IndexOf("no such table:", StringComparison.Ordinal) <= 0) ? ErrorCode.SQLiteOtherError : ErrorCode.TMOrContainerMissing);
							break;
						default:
							code = ErrorCode.SQLiteOtherError;
							break;
						}
						FaultDescription faultDescription = new FaultDescription(code, FaultStatus.Error, ex3.Message);
						return new FaultException<FaultDescription>(faultDescription, faultDescription.Message);
					}
					Log(e);
					return new FaultException<FaultDescription>(new FaultDescription(e), e.Message);
				}
				Log(ex2);
				ErrorCode errorCode = ErrorCode.OK;
				if (ex2.Number == 2812)
				{
					errorCode = ErrorCode.TMOrContainerMissing;
				}
				if (errorCode == ErrorCode.OK)
				{
					return new FaultException<FaultDescription>(new FaultDescription(ex2), ex2.Message);
				}
				FaultDescription faultDescription2 = new FaultDescription(errorCode, FaultStatus.Error, ex2.Message);
				return new FaultException<FaultDescription>(faultDescription2, faultDescription2.Message);
			}
			LanguagePlatformException ex5 = ex;
			Log(ex5.Description);
			return new FaultException<FaultDescription>(ex5.Description, ex5.Message);
		}

		public ServerInfo QueryServerInfo(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "QueryServerInfo"))
				{
					ServerInfo serverInfo = callContext.GetServerInfo();
					callContext.Complete();
					return serverInfo;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void SetAdministratorPassword(Container container, PersistentObjectToken tmId, string pwd)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "SetAdministratorPassword"))
				{
					if (!callContext.IsFilebasedTm)
					{
						throw new NotSupportedException();
					}
					callContext.ResourceManager.SetPassword(tmId, Permission.Administrator, pwd);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void SetMaintenancePassword(Container container, PersistentObjectToken tmId, string pwd)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "SetMaintenancePassword"))
				{
					if (!callContext.IsFilebasedTm)
					{
						throw new NotSupportedException();
					}
					callContext.ResourceManager.SetPassword(tmId, Permission.Maintenance, pwd);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void SetReadWritePassword(Container container, PersistentObjectToken tmId, string pwd)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "SetReadWritePassword"))
				{
					if (!callContext.IsFilebasedTm)
					{
						throw new NotSupportedException();
					}
					callContext.ResourceManager.SetPassword(tmId, Permission.ReadWrite, pwd);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void SetReadOnlyPassword(Container container, PersistentObjectToken tmId, string pwd)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "SetReadOnlyPassword"))
				{
					if (!callContext.IsFilebasedTm)
					{
						throw new NotSupportedException();
					}
					callContext.ResourceManager.SetPassword(tmId, Permission.ReadOnly, pwd);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool HasReadOnlyPassword(Container container, PersistentObjectToken tmId)
		{
			return HasPassword(container, tmId, Permission.ReadOnly);
		}

		public bool HasReadWritePassword(Container container, PersistentObjectToken tmId)
		{
			return HasPassword(container, tmId, Permission.ReadWrite);
		}

		public bool HasMaintenancePassword(Container container, PersistentObjectToken tmId)
		{
			return HasPassword(container, tmId, Permission.Maintenance);
		}

		public bool HasAdministratorPassword(Container container, PersistentObjectToken tmId)
		{
			return HasPassword(container, tmId, Permission.Administrator);
		}

		private bool HasPassword(Container container, PersistentObjectToken tmId, Permission permission)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "HasPassword"))
				{
					if (!callContext.IsFilebasedTm)
					{
						throw new NotSupportedException();
					}
					bool result = callContext.ResourceManager.HasPassword(tmId, permission);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Permission IsPasswordSet(Container container, PersistentObjectToken tmId, string pwd)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "IsPasswordSet"))
				{
					if (!callContext.IsFilebasedTm)
					{
						throw new NotSupportedException();
					}
					Permission result = callContext.ResourceManager.IsPasswordSet(tmId, pwd);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public Permission GetPermissions(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetPermissions"))
				{
					Permission permissions = callContext.GetPermissions(tmId);
					callContext.Complete();
					return permissions;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public TranslationMemorySetup[] GetTranslationMemoryIds(Container container, bool checkPermissions)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetTranslationMemoryIds"))
				{
					TranslationMemorySetup[] translationMemories = callContext.ResourceManager.GetTranslationMemories(checkPermissions);
					callContext.Complete();
					return translationMemories;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public FuzzyIndexTuningSettings GetFuzzyIndexTuningSettings(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "GetFuzzyIndexTuningSettings"))
				{
					FuzzyIndexTuningSettings fuzzyIndexTuningSettings = callContext.ResourceManager.GetFuzzyIndexTuningSettings(tmId.Id);
					callContext.Complete();
					return fuzzyIndexTuningSettings;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void SetFuzzyIndexTuningSettings(Container container, PersistentObjectToken tmId, FuzzyIndexTuningSettings settings)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				using (CallContext callContext = CreateCallContext(container, "SetFuzzyIndexTuningSettings"))
				{
					callContext.ResourceManager.SetFuzzyIndexTuningSettings(tmId.Id, settings);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void ClearFuzzyCache(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "ClearFuzzyCache"))
				{
					callContext.ResourceManager.ClearFuzzyCache(container);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool HasFuzzyCacheNonEmpty(Container container, PersistentObjectToken tmId)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "HasFuzzyCacheNonEmpty"))
				{
					bool result = callContext.ResourceManager.HasFuzzyCacheNonEmpty(container);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		void IDisposable.Dispose()
		{
		}

		private static void Clean(Sdl.LanguagePlatform.Core.Segment s)
		{
			if (s != null)
			{
				s.Tokens = null;
			}
		}

		private void Clean(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu)
		{
			if (tu != null)
			{
				Clean(tu.SourceSegment);
				Clean(tu.TargetSegment);
			}
		}

		private void Clean(IList<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> tus)
		{
			if (tus != null)
			{
				foreach (Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu in tus)
				{
					Clean(tu);
				}
			}
		}

		public List<SubsegmentMatchType> SupportedSubsegmentMatchTypes(Container container, PersistentObjectToken tmId)
		{
			try
			{
				List<SubsegmentMatchType> list = new List<SubsegmentMatchType>();
				list.Add(SubsegmentMatchType.TM_TDB);
				using (CallContext callContext = CreateCallContext(container, "SupportedSubsegmentMatchTypes"))
				{
					if (HasFGASupport(callContext.ResourceManager.GetTranslationMemory(tmId).FGASupport))
					{
						list.Add(SubsegmentMatchType.DTA);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		[Obsolete("replaced by GetUnalignedCount")]
		public int GetUnalignedTranslationUnitCount(Container container, PersistentObjectToken tmId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetUnalignedTranslationUnitCount"))
				{
					return callContext.AlignableCorpusManager.GetAlignableCorpus(callContext.GetAlignableCorpusId(tmId)).UnalignedCount(null);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		[Obsolete("replaced by GetUnalignedCount")]
		public int GetUnalignedUnscheduledTUCount(Container container, PersistentObjectToken tmId, int scheduleDelta, DateTime modelDate)
		{
			throw new NotImplementedException();
		}

		public int GetUnalignedCount(Container container, PersistentObjectToken tmId, DateTime? modelDate)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetUnalignedTUCount"))
				{
					return callContext.AlignableCorpusManager.GetAlignableCorpus(callContext.GetAlignableCorpusId(tmId)).UnalignedCount(modelDate);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool IsModelFree(Container container, PersistentObjectToken tmId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "IsModelFree"))
				{
					return callContext.AlignableCorpusManager.GetAlignableCorpus(callContext.GetAlignableCorpusId(tmId)).GetAlignerDefinition()?.IsModelFree ?? false;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void ClearAlignmentData(Container container, PersistentObjectToken tmId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "ClearAlignmentData"))
				{
					callContext.AlignableStorage.ClearAlignmentData(tmId.Id);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public TranslationModelFitness MeasureModelFitness(Container container, PersistentObjectToken tmId, ref RegularIterator iter, TranslationModelId modelId, bool unalignedOrPostdatedOnly)
		{
			throw new NotImplementedException();
		}

		public void AlignTranslationUnits(Container container, PersistentObjectToken tmId, bool unalignedOnly, bool unalignedOrPostdated, CancellationToken token, IProgress<int> progress)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				using (CallContext callContext = CreateCallContext(container, "AlignTranslationUnits"))
				{
					callContext.ResourceManager.AlignTranslationUnits(tmId, token, progress, unalignedOnly, unalignedOrPostdated);
					callContext.ResourceManager.RecomputeStatistics(tmId.Id);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		[Obsolete]
		public bool AlignTranslationUnits(Container container, PersistentObjectToken tmId, bool unalignedOnly, bool unalignedOrPostdated, ref RegularIterator iter)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (iter == null)
				{
					throw new ArgumentNullException("iter");
				}
				using (CallContext callContext = CreateCallContext(container, "AlignTranslationUnits"))
				{
					bool result = callContext.ResourceManager.AlignTranslationUnits(tmId, iter, unalignedOnly, unalignedOrPostdated);
					callContext.Complete();
					return result;
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int GetPostdatedTranslationUnitCount(Container container, PersistentObjectToken tmId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetAlignedPostdatedTranslationUnitCount"))
				{
					if (!HasFGASupport(callContext.ResourceManager.GetTranslationMemory(tmId).FGASupport))
					{
						return 0;
					}
					IAlignableCorpus alignableCorpus = callContext.AlignableCorpusManager.GetAlignableCorpus(callContext.GetAlignableCorpusId(tmId));
					AlignerDefinition alignerDefinition = alignableCorpus.GetAlignerDefinition();
					if (alignerDefinition == null)
					{
						return 0;
					}
					if (alignerDefinition.IsModelFree)
					{
						return 0;
					}
					ModelBasedAlignerDefinition modelBasedAlignerDefinition = alignerDefinition as ModelBasedAlignerDefinition;
					ITranslationModelDataService translationModelDataService;
					if (!callContext.IsFilebasedTm)
					{
						translationModelDataService = GetTranslationModelDataService();
					}
					else
					{
						ITranslationModelDataService translationModelDataService2 = new ContainerBasedTranslationModelDataService(callContext.Container);
						translationModelDataService = translationModelDataService2;
					}
					ITranslationModelDataService translationModelDataService3 = translationModelDataService;
					TranslationModelDetails modelDetails;
					using (translationModelDataService3)
					{
						modelDetails = translationModelDataService3.GetModelDetails(modelBasedAlignerDefinition?.ModelId);
						if (!modelDetails.ModelDate.HasValue)
						{
							return 0;
						}
					}
					return alignableCorpus.GetPostdatedContentPairCount(modelDetails.ModelDate.Value);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int GetPostdatedTranslationUnitCount(Container container, PersistentObjectToken tmId, DateTime modelDate)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetAlignedPostdatedTranslationUnitCount"))
				{
					if (!HasFGASupport(callContext.ResourceManager.GetTranslationMemory(tmId).FGASupport))
					{
						return 0;
					}
					return callContext.AlignableCorpusManager.GetAlignableCorpus(callContext.GetAlignableCorpusId(tmId)).GetPostdatedContentPairCount(modelDate);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public int GetAlignedPredatedTranslationUnitCount(Container container, PersistentObjectToken tmId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetAlignedPredatedTranslationUnitCount"))
				{
					if (!HasFGASupport(callContext.ResourceManager.GetTranslationMemory(tmId).FGASupport))
					{
						return 0;
					}
					IAlignableCorpus alignableCorpus = callContext.AlignableCorpusManager.GetAlignableCorpus(callContext.GetAlignableCorpusId(tmId));
					AlignerDefinition alignerDefinition = alignableCorpus.GetAlignerDefinition();
					if (alignerDefinition == null)
					{
						return 0;
					}
					if (alignerDefinition.IsModelFree)
					{
						return 0;
					}
					ModelBasedAlignerDefinition modelBasedAlignerDefinition = alignerDefinition as ModelBasedAlignerDefinition;
					ITranslationModelDataService translationModelDataService;
					if (!callContext.IsFilebasedTm)
					{
						translationModelDataService = GetTranslationModelDataService();
					}
					else
					{
						ITranslationModelDataService translationModelDataService2 = new ContainerBasedTranslationModelDataService(callContext.Container);
						translationModelDataService = translationModelDataService2;
					}
					ITranslationModelDataService translationModelDataService3 = translationModelDataService;
					TranslationModelDetails modelDetails;
					using (translationModelDataService3)
					{
						modelDetails = translationModelDataService3.GetModelDetails(modelBasedAlignerDefinition?.ModelId);
						if (!modelDetails.ModelDate.HasValue)
						{
							return 0;
						}
					}
					return alignableCorpus.GetAlignedPredatedContentPairCount(modelDetails.ModelDate.Value);
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void SetAlignerDefinition(Container container, PersistentObjectToken tmId, AlignerDefinition definition)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "SetAlignerDefinition"))
				{
					if (definition != null && !HasFGASupport(callContext.ResourceManager.GetTranslationMemory(tmId).FGASupport))
					{
						throw new Exception("TM is unable to store alignment data");
					}
					callContext.AlignableStorage.SetAlignerDefinition(tmId.Id, definition);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public AlignerDefinition GetAlignerDefinition(Container container, PersistentObjectToken tmId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "GetAlignerDefinition"))
				{
					return callContext.AlignableCorpusManager.GetAlignableCorpus(callContext.GetAlignableCorpusId(tmId)).GetAlignerDefinition();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		[Obsolete]
		public void ScheduleTusForAlignment(Container container, PersistentObjectToken tmId, PersistentObjectToken[] tuIds)
		{
			throw new NotImplementedException();
		}

		[Obsolete("replaced by GetAlignmentTimestamps")]
		public List<int> UnalignedTusUpdateSchedule(Container container, PersistentObjectToken tmId, ref RegularIterator iter, int scheduleDelta, DateTime modelDate)
		{
			throw new NotImplementedException();
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(Container container, PersistentObjectToken tmId, ref RegularIterator iter, DateTime modelDate)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				List<(int, DateTime)> alignmentTimestamps;
				using (CallContext callContext = CreateCallContext(container, "GetAlignmentTimestamps"))
				{
					alignmentTimestamps = callContext.ResourceManager.GetAlignmentTimestamps(tmId, iter, modelDate);
					callContext.Complete();
				}
				return alignmentTimestamps;
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public List<(int, DateTime)> GetAlignmentTimestamps(Container container, PersistentObjectToken tmId, List<int> tuIds)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				List<(int, DateTime)> alignmentTimestamps;
				using (CallContext callContext = CreateCallContext(container, "GetAlignmentTimestamps"))
				{
					alignmentTimestamps = callContext.ResourceManager.GetAlignmentTimestamps(tmId, tuIds);
					callContext.Complete();
				}
				return alignmentTimestamps;
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void AddorUpdateLastSearch(Container container, PersistentObjectToken tmId, List<PersistentObjectToken> tuIds, DateTime lastSearch)
		{
			try
			{
				if (tmId == null || tmId.Id == 0)
				{
					throw new ArgumentNullException("tmId");
				}
				if (tuIds == null || tuIds.Count == 0)
				{
					throw new ArgumentNullException("tuIds");
				}
				using (CallContext callContext = CreateCallContext(container, "AddorUpdateLastSearch"))
				{
					_ATMManager.Clear();
					List<int> tuIds2 = (from x in tuIds
						where x != null
						select x.Id).ToList();
					callContext.Storage.AddorUpdateLastSearch(tmId.Id, tuIds2, lastSearch);
					callContext.Complete();
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		private static void CheckFileBased(CallContext context)
		{
		}

		public TranslationModelDetails[] GetAllModelDetails(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelDataService containerBasedTranslationModelDataService = new ContainerBasedTranslationModelDataService(callContext.Container))
					{
						return containerBasedTranslationModelDataService.GetAllModelDetails();
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public TranslationModelDetails GetModelDetails(Container container, TranslationModelId translationModelId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						return containerBasedTranslationModelService.GetModelDetails(translationModelId);
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool CanBuildModel(Container container, TranslationModelId translationModelId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						return containerBasedTranslationModelService.CanBuildModel(translationModelId);
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void DeleteModel(Container container, TranslationModelId translationModelId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						containerBasedTranslationModelService.DeleteModel(translationModelId);
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool ShouldBuildModel(Container container, TranslationModelId translationModelId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						return containerBasedTranslationModelService.ShouldBuildModel(translationModelId);
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public TranslationModelId AddModel(Container container, string name, List<PersistentObjectToken> corpora, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						List<AlignableCorpusId> list = new List<AlignableCorpusId>();
						foreach (PersistentObjectToken item in corpora)
						{
							list.Add(new StorageBasedAlignableCorpusId(item));
						}
						return containerBasedTranslationModelService.AddModel(name, list, sourceCulture, targetCulture, TranslationModelTypes.ChiSquared);
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void ClearModel(Container container, TranslationModelId translationModelId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						containerBasedTranslationModelService.ClearModel(translationModelId);
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public void BuildModel(Container container, TranslationModelId translationModelId)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "BuildTranslationModel"))
				{
					CheckFileBased(callContext);
					if (callContext.IsFilebasedTm)
					{
						Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory translationMemory = callContext.Storage.GetTms()[0];
						callContext.AlignableStorage.PrepareForModelBuild(translationMemory.Id);
					}
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						containerBasedTranslationModelService.Progress += OnProgress;
						containerBasedTranslationModelService.BuildModel(translationModelId);
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool CreateTranslationModelContainerSchema(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "CreateTranslationModelSchema"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						containerBasedTranslationModelService.CreateTranslationModelContainerSchema();
						callContext.Complete();
						return true;
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		public bool DropTranslationModelContainerSchema(Container container)
		{
			try
			{
				using (CallContext callContext = CreateCallContext(container, "CreateTranslationModelSchema"))
				{
					CheckFileBased(callContext);
					using (ContainerBasedTranslationModelService containerBasedTranslationModelService = new ContainerBasedTranslationModelService(callContext.Container, callContext.AlignableCorpusManager))
					{
						containerBasedTranslationModelService.DropTranslationModelContainerSchema();
						callContext.Complete();
						return true;
					}
				}
			}
			catch (Exception e)
			{
				throw GenerateFault(e);
			}
		}

		private void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		private void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			this.Progress?.Invoke(this, progressEventArgs);
		}
	}
}
