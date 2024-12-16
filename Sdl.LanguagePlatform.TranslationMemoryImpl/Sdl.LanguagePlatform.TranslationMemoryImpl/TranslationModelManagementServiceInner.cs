using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class TranslationModelManagementServiceInner : TranslationModelDataServiceInner
	{
		public Func<IAlignableCorpusManager> GetAlignableCorpusManager = delegate
		{
			throw new NotImplementedException();
		};

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		private void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		private void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			this.Progress?.Invoke(this, progressEventArgs);
		}

		public void CreateTranslationModelContainerSchema(Container container)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "CreateTranslationModelContainerSchema"))
			{
				translationModelCallContext.TranslationModelStorage.CreateTranslationModelContainerSchema();
				translationModelCallContext.Complete();
			}
		}

		public void DropTranslationModelContainerSchema(Container container)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "DropTranslationModelContainerSchema"))
			{
				translationModelCallContext.TranslationModelStorage.DropTranslationModelContainerSchema();
				translationModelCallContext.Complete();
			}
		}

		public TranslationModelId AddModel(Container container, string name, List<AlignableCorpusId> corpora, CultureInfo sourceCulture, CultureInfo targetCulture, TranslationModelTypes type)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "AddModel"))
			{
				TranslationModelId result = translationModelCallContext.TranslationModelManager.AddModel(name, corpora, sourceCulture, targetCulture, type);
				translationModelCallContext.Complete();
				return result;
			}
		}

		public bool CanBuildModel(Container container, TranslationModelId modelId)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "CanBuildModel"))
			{
				return translationModelCallContext.TranslationModelManager.GetModel(modelId).CanBuildModel(GetAlignableCorpusManager());
			}
		}

		public bool ShouldBuildModel(Container container, TranslationModelId modelId)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "ShouldBuildModel"))
			{
				return translationModelCallContext.TranslationModelManager.GetModel(modelId).ShouldBuildModel(GetAlignableCorpusManager());
			}
		}

		public void DeleteModel(Container container, TranslationModelId modelId)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "DeleteModel"))
			{
				translationModelCallContext.TranslationModelManager.DeleteModel(modelId);
				translationModelCallContext.Complete();
			}
		}

		public void ClearModel(Container container, TranslationModelId modelId)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "ClearModel"))
			{
				translationModelCallContext.TranslationModelManager.ClearModel(modelId);
				translationModelCallContext.Complete();
			}
		}

		public void BuildModel(Container container, TranslationModelId translationModelId)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "BuildTranslationModel"))
			{
				TranslationModel model = translationModelCallContext.TranslationModelManager.GetModel(translationModelId);
				IAlignableCorpusManager corpusManager = GetAlignableCorpusManager();
				model.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
				{
					OnProgress(args);
				};
				model.BuildModel(corpusManager);
				translationModelCallContext.Complete();
			}
		}
	}
}
