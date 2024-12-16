using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface IAlignableTranslationMemory : ITranslationMemory2015, ITranslationMemory, ITranslationProvider
	{
		FGASupport FGASupport
		{
			get;
		}

		int UnalignedTranslationUnitCount
		{
			get;
		}

		AlignerDefinition AlignerDefinition
		{
			get;
		}

		int AlignedPredatedTranslationUnitCount
		{
			get;
		}

		int TranslationUnitNewerThanModelCount
		{
			get;
		}

		bool ShouldAlign
		{
			get;
		}

		TranslationModelFitness MeasureModelFitness(ref RegularIterator iterator, bool postdatedOrUnalignedOnly);

		void ClearAlignmentData();

		bool AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, ref RegularIterator iter);

		void AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, CancellationToken token, IProgress<int> progress);
	}
}
