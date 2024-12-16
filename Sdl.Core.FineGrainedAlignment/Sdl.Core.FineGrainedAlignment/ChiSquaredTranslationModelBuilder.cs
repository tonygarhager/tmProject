using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.FineGrainedAlignment
{
	public class ChiSquaredTranslationModelBuilder
	{
		private readonly IAlignableCorpusManager _corpusManager;

		private readonly ChiSquaredTranslationModel _translationModel;

		private const int _ReportProgressPeriod = 100;

		public TranslationModel Model => _translationModel;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public ChiSquaredTranslationModelBuilder(ChiSquaredTranslationModel translationModel, IAlignableCorpusManager corpusManager)
		{
			if (corpusManager == null)
			{
				throw new ArgumentNullException("corpusManager");
			}
			if (translationModel == null)
			{
				throw new ArgumentNullException("translationModel");
			}
			_translationModel = translationModel;
			_corpusManager = corpusManager;
		}

		public bool CanBuildModel()
		{
			int totalPairs = 0;
			GetCorpora(out totalPairs);
			return CanBuildModel(totalPairs);
		}

		public bool ShouldBuildModel()
		{
			int totalPairs = 0;
			List<IAlignableCorpus> corpora = GetCorpora(out totalPairs);
			return ShouldBuildModel(totalPairs, corpora);
		}

		private bool ShouldBuildModel(int totalPairs, List<IAlignableCorpus> corpora)
		{
			if (!CanBuildModel(totalPairs))
			{
				return false;
			}
			if (!_translationModel.TranslationModelDate.HasValue)
			{
				return true;
			}
			int num = 0;
			foreach (IAlignableCorpus item in corpora)
			{
				num += item.GetPostdatedContentPairCount(_translationModel.TranslationModelDate.Value);
			}
			int num2 = totalPairs - num;
			int val = (int)((double)num2 * 0.1 + (double)num2 * ((double)num2 * 1.0 / 4000000.0));
			val = Math.Min(val, num2);
			if (num > val)
			{
				return true;
			}
			return false;
		}

		private bool CanBuildModel(int totalPairs)
		{
			return totalPairs >= 500;
		}

		private List<IAlignableCorpus> GetCorpora(out int totalPairs)
		{
			List<IAlignableCorpus> list = new List<IAlignableCorpus>();
			OnProgress(TranslationModelProgressStage.Preparing, _translationModel.CorpusIds.Count());
			int num = 0;
			totalPairs = 0;
			foreach (AlignableCorpusId corpusId in _translationModel.CorpusIds)
			{
				IAlignableCorpus alignableCorpus = _corpusManager.GetAlignableCorpus(corpusId);
				list.Add(alignableCorpus);
				totalPairs += alignableCorpus.PairCount;
				num++;
				OnProgress(TranslationModelProgressStage.Preparing, num, _translationModel.CorpusIds.Count());
			}
			return list;
		}

		private IEnumerable<Pair<IntSegment>> EncodePairs(List<IAlignableCorpus> corpora, Action onEncoded, DataEncoder de, DbVocabularyFile srcVocab, DbVocabularyFile trgVocab)
		{
			int encoded = 0;
			foreach (IAlignableCorpus item in corpora)
			{
				IEnumerable<IAlignableContentPair> pairs = item.Pairs();
				pairs = AlignableContentPairWrapper.WrapEnumerable(pairs);
				foreach (IAlignableContentPair item2 in pairs)
				{
					Pair<IntSegment> pair = new Pair<IntSegment>();
					pair.Left = new IntSegment();
					pair.Right = new IntSegment();
					if (de.Encode(item2, srcVocab, trgVocab, null, null, out pair.Left, out pair.Right, incremental: false, null, null, null, null))
					{
						onEncoded?.Invoke();
						encoded++;
						yield return pair;
					}
				}
			}
		}

		public SparseMatrix<double> BuildModel(IChiSquaredTranslationModelStore store)
		{
			int totalPairs = 0;
			List<IAlignableCorpus> corpora = GetCorpora(out totalPairs);
			CultureInfo sourceCulture = _translationModel.SourceCulture;
			CultureInfo targetCulture = _translationModel.TargetCulture;
			_translationModel.SetLatestVersion();
			DataEncoder de = new DataEncoder(sourceCulture, targetCulture, forTrainedModel: false, _translationModel.UseWordStems);
			DbDataLocation location = CreateDataLocation(store);
			int encoded = 0;
			DbVocabularyFile sourceVocab = _translationModel.SourceVocab;
			DbVocabularyFile targetVocab = _translationModel.TargetVocab;
			BilingualChiSquareComputer3 bilingualChiSquareComputer = new BilingualChiSquareComputer3(location, EncodePairs(corpora, delegate
			{
				encoded++;
			}, de, sourceVocab, targetVocab));
			OnProgress(TranslationModelProgressStage.Analysing, 0);
			int totalCountSteps = -1;
			bilingualChiSquareComputer.Progress = (EventHandler<TranslationModelProgressEventArgs>)Delegate.Combine(bilingualChiSquareComputer.Progress, (EventHandler<TranslationModelProgressEventArgs>)delegate(object sender, TranslationModelProgressEventArgs args)
			{
				if (args.ProgressStage == TranslationModelProgressStage.Encoding)
				{
					totalCountSteps = args.ProgressNumber;
				}
				else if (args.ProgressStage == TranslationModelProgressStage.Analysing)
				{
					OnProgress(TranslationModelProgressStage.Analysing, args.ProgressNumber, totalPairs);
				}
				else if (args.ProgressStage == TranslationModelProgressStage.Computing)
				{
					OnProgress(TranslationModelProgressStage.Computing, args.ProgressNumber);
				}
				else if (args.ProgressStage == TranslationModelProgressStage.Merging)
				{
					OnProgress(TranslationModelProgressStage.Merging, args.ProgressNumber, totalCountSteps);
				}
				_ = args.ProgressNumber % 1000;
			});
			SparseMatrix<double> result = bilingualChiSquareComputer.Compute();
			_translationModel.SampleCount = encoded;
			return result;
		}

		private DbDataLocation CreateDataLocation(IChiSquaredTranslationModelStore store)
		{
			string tempPath = Path.GetTempPath();
			string text = Path.Combine(tempPath, Guid.NewGuid().ToString());
			Directory.CreateDirectory(text);
			return new DbDataLocation(text, store);
		}

		private void OnProgress(TranslationModelProgressStage progressStage)
		{
			OnProgress(progressStage, 0);
		}

		private void OnProgress(TranslationModelProgressStage progressStage, int progressNumber)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, -1);
			OnProgress(progressEventArgs);
		}

		private void OnProgress(TranslationModelProgressStage progressStage, int progressNumber, int progressLimit)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, progressLimit);
			OnProgress(progressEventArgs);
		}

		private void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		private void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			if (this.Progress != null)
			{
				this.Progress(this, progressEventArgs);
				if (progressEventArgs.Cancel)
				{
					throw new TranslationModelCancelException();
				}
			}
		}
	}
}
