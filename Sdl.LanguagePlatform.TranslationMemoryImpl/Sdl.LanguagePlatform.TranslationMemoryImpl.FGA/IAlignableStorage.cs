using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal interface IAlignableStorage : IStorage, IDisposable
	{
		void ClearAlignmentData(int tmId);

		int GetPostdatedTranslationUnitCount(int tmId, DateTime modelDate);

		int GetAlignedPredatedTranslationUnitCount(int tmId, DateTime modelDate);

		int GetUnalignedCount(int tmId, DateTime? modelDate);

		AlignerDefinition GetAlignerDefinition(int tmId);

		void SetAlignerDefinition(int tmId, AlignerDefinition definition);

		bool[] UpdateTuAlignmentData(IEnumerable<TuAlignmentDataInternal> tuAlignmentDatas, int tmId);

		int GetPairCount(int tmId);

		void PrepareForModelBuild(int tmId);

		void SetIsAlignmentEnabled(int tmId, bool enabled);

		List<TranslationUnit> GetAlignableTus(int tmId, int startAfter, int count, bool unalignedOnly, bool unalignedOrPostdated);

		List<(int, DateTime)> GetAlignmentTimestamps(int tmId, List<int> tuIds);

		List<(int, DateTime)> GetAlignmentTimestamps(int tmId, int startAfter, int count, DateTime modelDate);

		List<TranslationUnit> GetAlignableTus(int tmId, List<int> tuIds);
	}
}
