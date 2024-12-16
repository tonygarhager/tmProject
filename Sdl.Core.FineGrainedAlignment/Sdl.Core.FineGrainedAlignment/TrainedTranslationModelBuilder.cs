using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.FineGrainedAlignment
{
	public class TrainedTranslationModelBuilder
	{
		private readonly IAlignableCorpusManager _corpusManager;

		private readonly TrainedTranslationModel _translationModel;

		private const int _ReportProgressPeriod = 100;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public TrainedTranslationModelBuilder(TrainedTranslationModel translationModel, IAlignableCorpusManager corpusManager)
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

		private DbDataLocation CreateDataLocation(IChiSquaredTranslationModelStore store)
		{
			string tempPath = Path.GetTempPath();
			string text = Path.Combine(tempPath, Guid.NewGuid().ToString());
			Directory.CreateDirectory(text);
			return new DbDataLocation(text, store);
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
			if ((double)num * 1.0 / (double)totalPairs > 0.1)
			{
				return true;
			}
			if (num > 2000)
			{
				return true;
			}
			return false;
		}

		private bool CanBuildModel(int totalPairs)
		{
			return true;
		}

		public TrainedModelBuildResults BuildModel()
		{
			int totalPairs = 0;
			List<IAlignableCorpus> corpora = GetCorpora(out totalPairs);
			CultureInfo sourceCulture = _translationModel.SourceCulture;
			CultureInfo targetCulture = _translationModel.TargetCulture;
			DataEncoder dataEncoder = new DataEncoder(sourceCulture, targetCulture, forTrainedModel: true, stemming: false);
			DbDataLocation dbDataLocation = CreateDataLocation(_translationModel.Store);
			using (TempFileRemover tempFileRemover = new TempFileRemover())
			{
				tempFileRemover.AddDirectory(dbDataLocation.Directory.FullName);
				tempFileRemover.AddFile(dbDataLocation.GetComponentFileName(DataFileType.FrequencyCountsFile, sourceCulture));
				tempFileRemover.AddFile(dbDataLocation.GetComponentFileName(DataFileType.FrequencyCountsFile, targetCulture));
				tempFileRemover.AddFile(dbDataLocation.GetComponentFileName(DataFileType.TokenFile, sourceCulture));
				tempFileRemover.AddFile(dbDataLocation.GetComponentFileName(DataFileType.TokenFileIndex, sourceCulture));
				tempFileRemover.AddFile(dbDataLocation.GetComponentFileName(DataFileType.TokenFile, targetCulture));
				tempFileRemover.AddFile(dbDataLocation.GetComponentFileName(DataFileType.TokenFileIndex, targetCulture));
				DbVocabularyFile sourceVocab = _translationModel.SourceVocab;
				DbVocabularyFile targetVocab = _translationModel.TargetVocab;
				sourceVocab.Add(" ");
				targetVocab.Add(" ");
				using (FrequencyFileWriter2 frequencyFileWriter = new FrequencyFileWriter2(dbDataLocation, sourceCulture))
				{
					using (FrequencyFileWriter2 frequencyFileWriter2 = new FrequencyFileWriter2(dbDataLocation, targetCulture))
					{
						using (TokenFileWriter2 tokenFileWriter = new TokenFileWriter2(dbDataLocation, sourceCulture))
						{
							using (TokenFileWriter2 tokenFileWriter2 = new TokenFileWriter2(dbDataLocation, targetCulture))
							{
								tokenFileWriter.Create();
								tokenFileWriter2.Create();
								int num = 0;
								int num2 = 0;
								for (int i = 0; i < corpora.Count; i++)
								{
									IAlignableCorpus alignableCorpus = corpora[i];
									foreach (IAlignableContentPair item in alignableCorpus.Pairs())
									{
										if (dataEncoder.Encode(item, incremental: false, sourceVocab, targetVocab, tokenFileWriter, tokenFileWriter2, null, null, frequencyFileWriter, frequencyFileWriter2))
										{
											num++;
										}
										else
										{
											num2++;
										}
										if (num % 100 == 0)
										{
											OnProgress(TranslationModelProgressStage.Encoding, num + num2, totalPairs);
										}
									}
								}
								frequencyFileWriter.Save();
								frequencyFileWriter2.Save();
								tokenFileWriter.Close();
								tokenFileWriter2.Close();
							}
						}
					}
				}
				TrainedModelComputer trainedModelComputer = new TrainedModelComputer(dbDataLocation, sourceCulture, targetCulture, _translationModel.SourceVocab, _translationModel.TargetVocab);
				OnProgress(TranslationModelProgressStage.Computing, 0);
				trainedModelComputer.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
				{
					OnProgress(TranslationModelProgressStage.Computing, args.ProgressNumber);
					_ = args.ProgressNumber % 1000;
				};
				trainedModelComputer.Compute(3);
				TrainedModelBuildResults trainedModelBuildResults = new TrainedModelBuildResults();
				trainedModelBuildResults.TranslationMatrix = trainedModelComputer.TranslationTable;
				trainedModelBuildResults.ReversedTranslationMatrix = trainedModelComputer.ReversedTranslationTable;
				return trainedModelBuildResults;
			}
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
