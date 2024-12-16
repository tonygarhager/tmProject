using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public abstract class TranslationModel
	{
		protected TranslationModelId _id;

		protected string _name;

		protected List<AlignableCorpusId> _corpusIds;

		protected CultureInfo _srcCulture;

		protected CultureInfo _trgCulture;

		protected DateTime? _modelDate;

		public abstract TranslationModelTypes ModelType
		{
			get;
		}

		public DateTime? TranslationModelDate => _modelDate;

		public CultureInfo SourceCulture => _srcCulture;

		public CultureInfo TargetCulture => _trgCulture;

		public IEnumerable<AlignableCorpusId> CorpusIds => _corpusIds;

		public string Name => _name;

		public TranslationModelId Id => _id;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public abstract void BuildModel(IAlignableCorpusManager corpusManager);

		public abstract bool CanBuildModel(IAlignableCorpusManager corpusManager);

		public abstract bool ShouldBuildModel(IAlignableCorpusManager corpusManager);

		public abstract TranslationModelFitness MeasureModelFitness(List<IAlignableContentPair> pairs);

		protected void OnProgress(TranslationModelProgressStage progressStage)
		{
			OnProgress(progressStage, 0);
		}

		protected void OnProgress(TranslationModelProgressStage progressStage, int progressNumber)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, -1);
			OnProgress(progressEventArgs);
		}

		protected void OnProgress(TranslationModelProgressStage progressStage, int progressNumber, int limit)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, limit);
			OnProgress(progressEventArgs);
		}

		protected void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		protected void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			if (this.Progress != null)
			{
				this.Progress(this, progressEventArgs);
			}
		}
	}
}
