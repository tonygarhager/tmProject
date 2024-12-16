using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class ContainerBasedTranslationModelService : ContainerBasedTranslationModelDataService, ITranslationModelManagementService, ITranslationModelDataService, IDisposable
	{
		private readonly TranslationModelManagementServiceInner _tms = new TranslationModelManagementServiceInner();

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		private void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		private void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			this.Progress?.Invoke(this, progressEventArgs);
		}

		public ContainerBasedTranslationModelService(Container container, IAlignableCorpusManager corpusManager)
			: base(container)
		{
			_tms.GetAlignableCorpusManager = (() => corpusManager);
			TmsData = _tms;
			_tms.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
			{
				OnProgress(args);
			};
		}

		public void CreateTranslationModelContainerSchema()
		{
			_tms.CreateTranslationModelContainerSchema(Container);
		}

		public void DropTranslationModelContainerSchema()
		{
			_tms.DropTranslationModelContainerSchema(Container);
		}

		public TranslationModelId AddModel(string name, List<AlignableCorpusId> corpora, CultureInfo sourceCulture, CultureInfo targetCulture, TranslationModelTypes type)
		{
			return _tms.AddModel(Container, name, corpora, sourceCulture, targetCulture, type);
		}

		public void BuildModel(TranslationModelId translationModelId)
		{
			_tms.BuildModel(Container, translationModelId);
		}

		public void CreateSchema()
		{
			_tms.CreateTranslationModelContainerSchema(Container);
		}

		public void DropSchema()
		{
			_tms.DropTranslationModelContainerSchema(Container);
		}

		public bool CanBuildModel(TranslationModelId modelId)
		{
			return _tms.CanBuildModel(Container, modelId);
		}

		public void ClearModel(TranslationModelId modelId)
		{
			_tms.ClearModel(Container, modelId);
		}

		public void DeleteModel(TranslationModelId modelId)
		{
			_tms.DeleteModel(Container, modelId);
		}

		public bool ShouldBuildModel(TranslationModelId modelId)
		{
			return _tms.ShouldBuildModel(Container, modelId);
		}

		public void UpdateModel(TranslationModelId id, string name, List<AlignableCorpusId> corpora)
		{
			throw new NotImplementedException();
		}
	}
}
