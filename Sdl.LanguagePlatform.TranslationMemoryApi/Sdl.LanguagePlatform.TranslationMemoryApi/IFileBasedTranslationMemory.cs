using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface IFileBasedTranslationMemory : IAlignableTranslationMemory, ITranslationMemory2015, ITranslationMemory, ITranslationProvider, IReindexableTranslationMemory, ILocalTranslationMemory, IAdvancedContextTranslationMemory
	{
		bool CanBuildModel
		{
			get;
		}

		bool ShouldBuildModel
		{
			get;
		}

		new FGASupport FGASupport
		{
			get;
			set;
		}

		TranslationModelDetails ModelDetails
		{
			get;
		}

		event EventHandler<TranslationModelProgressEventArgs> TranslationModelProgress;

		void BuildModel();

		void ClearModel();

		void Save(IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken);
	}
}
