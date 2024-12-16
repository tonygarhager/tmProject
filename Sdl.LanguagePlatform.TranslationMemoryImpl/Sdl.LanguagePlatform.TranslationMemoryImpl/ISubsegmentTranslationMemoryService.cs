using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[ServiceContract]
	public interface ISubsegmentTranslationMemoryService : ITranslationMemoryService, IDisposable
	{
		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SubsegmentSearchResultsCollection SubsegmentSearchSegment(Container container, PersistentObjectToken tmId, SubsegmentSearchSettings settings, Segment segment);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(Container container, PersistentObjectToken tmId, SubsegmentSearchSettings settings, Segment[] segments);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		List<SubsegmentMatchType> SupportedSubsegmentMatchTypes(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SegmentAndSubsegmentSearchResults SearchSegment(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SegmentAndSubsegmentSearchResults[] SearchSegments(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SegmentAndSubsegmentSearchResults SearchTranslationUnit(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit tu);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] tus);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] tus, bool[] mask);

		TranslationUnit ExtractFragment(Container container, PersistentObjectToken tmId, TranslationUnit tu, SegmentRange sourceRange, SegmentRange targetRange);
	}
}
