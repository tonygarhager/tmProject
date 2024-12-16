using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.FGA;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public interface IAlignableTranslationMemoryService
	{
		[Obsolete("replaced by GetUnalignedCount")]
		int GetUnalignedUnscheduledTUCount(Container container, PersistentObjectToken tmId, int schedule_delta, DateTime modelDate);

		[Obsolete("replaced by GetUnalignedCount")]
		int GetUnalignedTranslationUnitCount(Container container, PersistentObjectToken tmId);

		int GetUnalignedCount(Container container, PersistentObjectToken tmId, DateTime? modelDate);

		bool IsModelFree(Container container, PersistentObjectToken tmId);

		TranslationModelFitness MeasureModelFitness(Container container, PersistentObjectToken tmId, ref RegularIterator iter, TranslationModelId modelId, bool postdatedOrUnalignedOnly);

		void ClearAlignmentData(Container container, PersistentObjectToken tmId);

		int GetPostdatedTranslationUnitCount(Container container, PersistentObjectToken tmId);

		int GetPostdatedTranslationUnitCount(Container container, PersistentObjectToken tmId, DateTime modelDate);

		int GetAlignedPredatedTranslationUnitCount(Container container, PersistentObjectToken tmId);

		void SetAlignerDefinition(Container container, PersistentObjectToken tmId, AlignerDefinition definition);

		AlignerDefinition GetAlignerDefinition(Container container, PersistentObjectToken tmId);

		bool AlignTranslationUnits(Container container, PersistentObjectToken tmId, bool unalignedOnly, bool unalignedOrPostdated, ref RegularIterator iter);

		void AlignTranslationUnits(Container container, PersistentObjectToken tmId, bool unalignedOnly, bool unalignedOrPostdated, CancellationToken token, IProgress<int> progress);

		ImportResult[] UpdateAlignmentData(Container container, PersistentObjectToken tmId, TuAlignmentData[] alignmentData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationUnit[] GetAlignableTranslationUnits(Container container, PersistentObjectToken tmId, ref RegularIterator iter, bool unalignedOnly, bool unalignedOrPostdated);

		[Obsolete("replaced by GetAlignmentTimestamps method")]
		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		List<int> UnalignedTusUpdateSchedule(Container container, PersistentObjectToken tmId, ref RegularIterator iter, int schedule_delta, DateTime modelDate);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		List<(int, DateTime)> GetAlignmentTimestamps(Container container, PersistentObjectToken tmId, ref RegularIterator iter, DateTime modelDate);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		List<(int, DateTime)> GetAlignmentTimestamps(Container container, PersistentObjectToken tmId, List<int> tuIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationUnit[] GetAlignableTranslationUnits(Container container, PersistentObjectToken tmId, List<int> TuIds);

		TranslationUnit[] GetFullTranslationUnits(Container container, PersistentObjectToken tmId, List<int> tuIds);
	}
}
