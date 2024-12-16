using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public class ChiSquaredTranslationModel : TranslationModel
	{
		private readonly IChiSquaredTranslationModelStoreReadOnly _store;

		private DbVocabularyFile _srcVocab;

		private DbVocabularyFile _trgVocab;

		private DbDoubleSparseMatrix _matrix;

		private bool _building;

		private bool _loaded;

		private DateTime _buildStartTime;

		private int _sampleCount;

		private int _version;

		public const int LatestModelVersion = 2;

		public IChiSquaredTranslationModelStoreReadOnly Store => _store;

		public int Version => _version;

		internal bool UseWordStems => Version >= 2;

		public int SampleCount
		{
			get
			{
				return _sampleCount;
			}
			set
			{
				_sampleCount = value;
			}
		}

		internal DbVocabularyFile SourceVocab => _srcVocab;

		internal DbVocabularyFile TargetVocab => _trgVocab;

		internal DbDoubleSparseMatrix Matrix => _matrix;

		public override TranslationModelTypes ModelType => TranslationModelTypes.ChiSquared;

		public ChiSquaredTranslationModel(IChiSquaredTranslationModelStoreReadOnly store, string name, TranslationModelId id, List<AlignableCorpusId> corpusIds, CultureInfo sourceCulture, CultureInfo targetCulture, DateTime? modelDate, int sampleCount, int version)
		{
			if (corpusIds == null)
			{
				throw new ArgumentNullException("corpusIds");
			}
			if (corpusIds.Count == 0)
			{
				throw new ArgumentException("corpora");
			}
			if (corpusIds[0] == null)
			{
				throw new Exception("corpusIds[0] is null");
			}
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			_store = store;
			_id = id;
			_name = name;
			_corpusIds = corpusIds;
			_srcCulture = sourceCulture;
			_trgCulture = targetCulture;
			_modelDate = modelDate;
			_sampleCount = sampleCount;
			_version = version;
			InitData();
		}

		private void InitData()
		{
			_srcVocab = new DbVocabularyFile(Store, target: false);
			_srcVocab.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(TranslationModelProgressStage.Saving, args.ProgressNumber, _srcVocab.Count);
			};
			_trgVocab = new DbVocabularyFile(Store, target: true);
			_trgVocab.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(TranslationModelProgressStage.Saving, args.ProgressNumber, _trgVocab.Count);
			};
			_matrix = new DbDoubleSparseMatrix();
		}

		internal void Unload()
		{
			InitData();
		}

		private void PrepareForBuild()
		{
			if (_building)
			{
				throw new Exception("Already building");
			}
			Unload();
			_building = true;
			_loaded = false;
			_buildStartTime = DateTime.Now;
		}

		private IChiSquaredTranslationModelStore GetWriteableStore()
		{
			IChiSquaredTranslationModelStore chiSquaredTranslationModelStore = Store as IChiSquaredTranslationModelStore;
			if (chiSquaredTranslationModelStore == null)
			{
				throw new Exception("The translation model store is read-only");
			}
			return chiSquaredTranslationModelStore;
		}

		public void SetLatestVersion()
		{
			_version = 2;
		}

		public override bool ShouldBuildModel(IAlignableCorpusManager corpusManager)
		{
			ChiSquaredTranslationModelBuilder chiSquaredTranslationModelBuilder = new ChiSquaredTranslationModelBuilder(this, corpusManager);
			chiSquaredTranslationModelBuilder.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args);
				_ = args.ProgressNumber % 1000;
			};
			return chiSquaredTranslationModelBuilder.ShouldBuildModel();
		}

		public override bool CanBuildModel(IAlignableCorpusManager corpusManager)
		{
			ChiSquaredTranslationModelBuilder chiSquaredTranslationModelBuilder = new ChiSquaredTranslationModelBuilder(this, corpusManager);
			chiSquaredTranslationModelBuilder.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args);
				_ = args.ProgressNumber % 1000;
			};
			return chiSquaredTranslationModelBuilder.CanBuildModel();
		}

		public override void BuildModel(IAlignableCorpusManager corpusManager)
		{
			ChiSquaredTranslationModelBuilder chiSquaredTranslationModelBuilder = new ChiSquaredTranslationModelBuilder(this, corpusManager);
			chiSquaredTranslationModelBuilder.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args);
				_ = args.ProgressNumber % 1000;
			};
			PrepareForBuild();
			ApplyBuildResults(chiSquaredTranslationModelBuilder.BuildModel(GetWriteableStore()));
		}

		private void MeasureModelFitness(List<IAlignableContentPair> pairs, TranslationModelFitness results, bool target)
		{
			List<Token> list = new List<Token>();
			foreach (IAlignableContentPair pair in pairs)
			{
				list.AddRange(target ? pair.TargetTokens : pair.SourceTokens);
			}
			MeasureModelFitness(list, target, results);
		}

		public override TranslationModelFitness MeasureModelFitness(List<IAlignableContentPair> pairs)
		{
			TranslationModelFitness translationModelFitness = new TranslationModelFitness
			{
				SourceIVTokenCounts = new Dictionary<string, int>(),
				SourceOOVTokenCounts = new Dictionary<string, int>(),
				TargetIVTokenCounts = new Dictionary<string, int>(),
				TargetOOVTokenCounts = new Dictionary<string, int>()
			};
			if (pairs == null)
			{
				return translationModelFitness;
			}
			MeasureModelFitness(pairs, translationModelFitness, target: false);
			MeasureModelFitness(pairs, translationModelFitness, target: true);
			return translationModelFitness;
		}

		public void MeasureModelFitness(List<Token> tokens, bool target, TranslationModelFitness results)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			DataEncoder dataEncoder = new DataEncoder(base.SourceCulture, base.TargetCulture, forTrainedModel: false, UseWordStems);
			List<string> list = new List<string>();
			dataEncoder.GetTokenStrings(tokens, list, forTraining: false, target);
			for (int i = 0; i < list.Count; i++)
			{
				string key = list[i];
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, 1);
				}
				else
				{
					dictionary[key]++;
				}
			}
			List<TranslationModelVocabEntry> list2 = Store.LoadVocab(target, dictionary.Keys);
			Dictionary<string, int> dictionary2 = target ? results.TargetIVTokenCounts : results.SourceIVTokenCounts;
			Dictionary<string, int> dictionary3 = target ? results.TargetOOVTokenCounts : results.SourceOOVTokenCounts;
			foreach (TranslationModelVocabEntry item in list2)
			{
				if (dictionary.ContainsKey(item.Token))
				{
					AddOrUpdate(dictionary2, item.Token, dictionary[item.Token]);
					dictionary.Remove(item.Token);
				}
			}
			foreach (KeyValuePair<string, int> item2 in dictionary)
			{
				AddOrUpdate(dictionary3, item2.Key, item2.Value);
			}
		}

		private void AddOrUpdate(Dictionary<string, int> dictionary, string key, int value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] += value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		internal void Load()
		{
			if (_building)
			{
				throw new Exception("Already building");
			}
			if (!_loaded)
			{
				_srcVocab.Load();
				_trgVocab.Load();
				_matrix = DbDoubleSparseMatrix.Load(Store, isReversedMatrix: false);
				_loaded = true;
			}
		}

		private void ApplyBuildResults(SparseMatrix<double> results)
		{
			if (!_building)
			{
				throw new Exception("Not building");
			}
			IChiSquaredTranslationModelStore writeableStore = GetWriteableStore();
			writeableStore.SetTranslationModelDate(null);
			writeableStore.ClearTranslationModel();
			_matrix = new DbDoubleSparseMatrix(results);
			_matrix.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(this, args);
			};
			_matrix.Save(writeableStore, isReversedMatrix: false);
			_srcVocab.Save(writeableStore);
			_trgVocab.Save(writeableStore);
			_building = false;
			_loaded = true;
			writeableStore.SetSampleCount(SampleCount);
			_modelDate = DateTimeUtilities.Normalize(_buildStartTime);
			writeableStore.SetTranslationModelDate(_modelDate);
		}

		internal void PartialLoad(HashSet<string> sourceStrings, HashSet<string> targetStrings)
		{
			Unload();
			if (_building)
			{
				throw new Exception("Already building");
			}
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			List<TranslationModelVocabEntry> list = Store.LoadVocab(target: false, sourceStrings);
			foreach (TranslationModelVocabEntry item in list)
			{
				_srcVocab.Add(item.Key, item.Token);
				hashSet.Add(item.Key);
			}
			list = Store.LoadVocab(target: true, targetStrings);
			foreach (TranslationModelVocabEntry item2 in list)
			{
				_trgVocab.Add(item2.Key, item2.Token);
				hashSet2.Add(item2.Key);
			}
			List<TranslationModelMatrixEntry> list2 = Store.LoadMatrixData(hashSet, hashSet2, isReversedMatrix: false);
			foreach (TranslationModelMatrixEntry item3 in list2)
			{
				_matrix[item3.SourceKey, item3.TargetKey] = item3.Value;
			}
		}
	}
}
