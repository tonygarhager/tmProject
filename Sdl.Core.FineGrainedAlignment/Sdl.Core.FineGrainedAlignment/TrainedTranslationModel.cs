using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public class TrainedTranslationModel : TranslationModel
	{
		private readonly IChiSquaredTranslationModelStore _store;

		private DbVocabularyFile _srcVocab;

		private DbVocabularyFile _trgVocab;

		private DbDoubleSparseMatrix _matrix;

		private DbDoubleSparseMatrix _reverseMatrix;

		private bool _building;

		private bool _loaded;

		private DateTime _buildStartTime;

		internal const string NULL_TOKEN = " ";

		public IChiSquaredTranslationModelStore Store => _store;

		internal DbVocabularyFile SourceVocab => _srcVocab;

		internal DbVocabularyFile TargetVocab => _trgVocab;

		internal DbDoubleSparseMatrix Matrix => _matrix;

		internal DbDoubleSparseMatrix ReverseMatrix => _reverseMatrix;

		public override TranslationModelTypes ModelType => TranslationModelTypes.Trained;

		public TrainedTranslationModel(IChiSquaredTranslationModelStore store, string name, TranslationModelId id, List<AlignableCorpusId> corpusIds, CultureInfo sourceCulture, CultureInfo targetCulture, DateTime? modelDate)
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
			InitData();
		}

		private void InitData()
		{
			_srcVocab = new DbVocabularyFile(Store, target: false);
			_trgVocab = new DbVocabularyFile(Store, target: true);
			_matrix = new DbDoubleSparseMatrix();
			_reverseMatrix = new DbDoubleSparseMatrix();
		}

		public int TotalVocabSize(bool target)
		{
			return Store.TotalVocabSize(target);
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
			Store.ClearTranslationModel();
			_buildStartTime = DateTime.Now;
		}

		public override bool ShouldBuildModel(IAlignableCorpusManager corpusManager)
		{
			TrainedTranslationModelBuilder trainedTranslationModelBuilder = new TrainedTranslationModelBuilder(this, corpusManager);
			trainedTranslationModelBuilder.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args);
				_ = args.ProgressNumber % 1000;
			};
			return trainedTranslationModelBuilder.ShouldBuildModel();
		}

		public override bool CanBuildModel(IAlignableCorpusManager corpusManager)
		{
			TrainedTranslationModelBuilder trainedTranslationModelBuilder = new TrainedTranslationModelBuilder(this, corpusManager);
			trainedTranslationModelBuilder.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args);
				_ = args.ProgressNumber % 1000;
			};
			return trainedTranslationModelBuilder.CanBuildModel();
		}

		public override void BuildModel(IAlignableCorpusManager corpusManager)
		{
			TrainedTranslationModelBuilder trainedTranslationModelBuilder = new TrainedTranslationModelBuilder(this, corpusManager);
			trainedTranslationModelBuilder.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args);
				_ = args.ProgressNumber % 1000;
			};
			PrepareForBuild();
			ApplyBuildResults(trainedTranslationModelBuilder.BuildModel());
		}

		public override TranslationModelFitness MeasureModelFitness(List<IAlignableContentPair> pairs)
		{
			throw new NotImplementedException();
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
				_reverseMatrix = DbDoubleSparseMatrix.Load(Store, isReversedMatrix: true);
				_loaded = true;
			}
		}

		private void ApplyBuildResults(TrainedModelBuildResults results)
		{
			if (!_building)
			{
				throw new Exception("Not building");
			}
			_matrix = new DbDoubleSparseMatrix(results.TranslationMatrix);
			_reverseMatrix = new DbDoubleSparseMatrix(results.ReversedTranslationMatrix);
			_matrix.Save(Store, isReversedMatrix: false);
			_reverseMatrix.Save(Store, isReversedMatrix: true);
			_srcVocab.Save(Store);
			_trgVocab.Save(Store);
			_building = false;
			_loaded = true;
			_modelDate = DateTimeUtilities.Normalize(_buildStartTime);
			Store.SetTranslationModelDate(_modelDate);
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
				_srcVocab.Add(item.Key, item.Token, item.Occurrences);
				hashSet.Add(item.Key);
			}
			list = Store.LoadVocab(target: true, targetStrings);
			foreach (TranslationModelVocabEntry item2 in list)
			{
				_trgVocab.Add(item2.Key, item2.Token, item2.Occurrences);
				hashSet2.Add(item2.Key);
			}
			List<TranslationModelMatrixEntry> list2 = Store.LoadMatrixData(hashSet, hashSet2, isReversedMatrix: false);
			foreach (TranslationModelMatrixEntry item3 in list2)
			{
				_matrix[item3.SourceKey, item3.TargetKey] = item3.Value;
			}
			list2 = Store.LoadMatrixData(hashSet2, hashSet, isReversedMatrix: true);
			foreach (TranslationModelMatrixEntry item4 in list2)
			{
				_reverseMatrix[item4.SourceKey, item4.TargetKey] = item4.Value;
			}
		}
	}
}
