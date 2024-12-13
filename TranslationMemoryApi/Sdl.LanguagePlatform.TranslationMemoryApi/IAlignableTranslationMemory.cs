using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a translation memory.
	/// </summary>
	public interface IAlignableTranslationMemory : ITranslationMemory2015, ITranslationMemory, ITranslationProvider
	{
		/// <summary>
		/// Returns the status of fine-grained-alignment support for the TM
		/// </summary>
		FGASupport FGASupport
		{
			get;
		}

		/// <summary>
		/// Gets the number of TUs that do not have fine-grained alignment information.
		/// </summary>
		int UnalignedTranslationUnitCount
		{
			get;
		}

		/// <summary>
		/// Gets the AlignerDefinition that has been set for this TM, or null if there is none
		/// </summary>
		AlignerDefinition AlignerDefinition
		{
			get;
		}

		/// <summary>
		/// Gets the number of TUs that are postdated (added after creation of the model they were aligned with)
		/// and are now predated (a model exists created after they were added)
		/// </summary>
		int AlignedPredatedTranslationUnitCount
		{
			get;
		}

		/// <summary>
		/// Gets the number of TUs that have been added subsequent to the translation model being built
		/// </summary>
		int TranslationUnitNewerThanModelCount
		{
			get;
		}

		/// <summary>
		/// Indicates whether 'quick' alignment (i.e. alignment of any unaligned TUs, plus postdated TUs for which a newer model is now available) is recommended
		/// </summary>
		bool ShouldAlign
		{
			get;
		}

		/// <summary>
		/// Measures how well the model 'fits' the TM content, by counting out-of-vocabulary words
		/// </summary>
		TranslationModelFitness MeasureModelFitness(ref RegularIterator iterator, bool postdatedOrUnalignedOnly);

		/// <summary>
		/// Deletes any fine-grained alignment data from the TM
		/// </summary>
		void ClearAlignmentData();

		/// <summary>
		/// Performs fine-grained alignment on translation units
		/// </summary>
		/// <param name="unalignedOnly">If true, will only operate on translation units that do not already have any alignment information</param>
		/// <param name="unalignedOrPostdatedOnly">If true, will only operate on translation units that do not already have any alignment information or are postdated (see remarks). In this case, <paramref name="unalignedOnly" /> must be false.</param>
		/// <param name="iter"></param>
		/// <returns>True if there are more translation units to process, false otherwise</returns>
		/// <remarks>An aligned, postdated TU is one that has been aligned, but was added to the TM after the translation model used for alignment was built.</remarks>
		bool AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, ref RegularIterator iter);

		/// <summary>
		/// Performs bulk fine-grained alignment on translation units in a TM
		/// </summary>
		void AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, CancellationToken token, IProgress<int> progress);
	}
}
