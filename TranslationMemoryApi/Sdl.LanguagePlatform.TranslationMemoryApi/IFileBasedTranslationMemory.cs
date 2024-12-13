using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents an file based Translation Memory
	/// </summary>
	public interface IFileBasedTranslationMemory : IAlignableTranslationMemory, ITranslationMemory2015, ITranslationMemory, ITranslationProvider, IReindexableTranslationMemory, ILocalTranslationMemory, IAdvancedContextTranslationMemory
	{
		/// <summary>
		/// Returns true if the TM has enough data for the translation model associated with it to be built
		/// </summary>
		bool CanBuildModel
		{
			get;
		}

		/// <summary>
		/// Indicates whether a build (or rebuild) of the translation model is recommended
		/// </summary>
		bool ShouldBuildModel
		{
			get;
		}

		/// <summary>
		/// Returns the status of fine-grained-alignment support for the TM
		/// </summary>
		new FGASupport FGASupport
		{
			get;
			set;
		}

		/// <summary>
		/// Provides details of the translation model associated with this file-based TM
		/// </summary>
		TranslationModelDetails ModelDetails
		{
			get;
		}

		/// <summary>
		/// Reports the progress of building a translation model
		/// </summary>
		event EventHandler<TranslationModelProgressEventArgs> TranslationModelProgress;

		/// <summary>
		/// Builds the translation model associated with this file-based TM
		/// </summary>
		void BuildModel();

		/// <summary>
		/// Clears the translation model
		/// </summary>
		void ClearModel();

		/// <summary>
		/// Performs a save operation
		/// </summary>
		/// <param name="progress"></param>
		/// <param name="cancellationToken"></param>
		void Save(IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken);
	}
}
