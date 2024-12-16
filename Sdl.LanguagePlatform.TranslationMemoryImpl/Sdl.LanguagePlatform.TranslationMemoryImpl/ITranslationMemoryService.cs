using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[ServiceContract]
	public interface ITranslationMemoryService : IDisposable
	{
		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken AddField(Container container, PersistentObjectToken tmId, Field field);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken[] AddFields(Container container, PersistentObjectToken tmId, FieldDefinitions fields);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken AddPicklistValue(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string value);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken[] AddPicklistValues(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string[] values);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult AddTranslationUnit(Container container, PersistentObjectToken tmId, TranslationUnit tu, ImportSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult AddOrUpdateTranslationUnit(Container container, PersistentObjectToken tmId, TranslationUnit tu, int previousTranslationHash, ImportSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult[] AddTranslationUnits(Container container, PersistentObjectToken tmId, TranslationUnit[] tus, ImportSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult[] AddOrUpdateTranslationUnits(Container container, PersistentObjectToken tmId, TranslationUnit[] tus, int[] previousTranslationHashes, ImportSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult[] AddTranslationUnitsMasked(Container container, PersistentObjectToken tmId, TranslationUnit[] tus, ImportSettings settings, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult[] AddOrUpdateTranslationUnitsMasked(Container container, PersistentObjectToken tmId, TranslationUnit[] tus, int[] previousTranslationHashes, ImportSettings settings, bool[] mask);

		void AddorUpdateLastSearch(Container container, PersistentObjectToken tmId, List<PersistentObjectToken> tuIds, DateTime lastSearch);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool AssignLanguageResourceToTranslationMemory(Container container, PersistentObjectToken resourceId, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool ChangeTranslationMemory(Container container, TranslationMemorySetup setup);

		[OperationContract]
		bool RecoverJAZHCMInfo(Container container, PersistentObjectToken tmId, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken);

		bool ChangeTranslationMemory(Container container, TranslationMemorySetup setup, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken CreateLanguageResource(Container container, LanguageResource resource);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool CreateSchema(Container container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken CreateTranslationMemory(Container container, TranslationMemorySetup setup);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool DeleteLanguageResource(Container container, PersistentObjectToken resourceId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool DeleteTranslationMemory(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool DeleteTranslationMemorySchema(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int DeleteAllTranslationUnits(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool DeleteTranslationUnit(Container container, PersistentObjectToken tmId, PersistentObjectToken tuId);

		List<PersistentObjectToken> DeleteTranslationUnitsWithFilter(Container container, PersistentObjectToken tmId, ref RegularIterator iterator);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int DeleteTranslationUnitsWithIterator(Container container, PersistentObjectToken tmId, ref RegularIterator iterator);

		[Obsolete]
		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int DeleteMatchingTranslationUnits(Container container, PersistentObjectToken tmId, FilterExpression filter);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int DeleteTranslationUnits(Container container, PersistentObjectToken tmId, PersistentObjectToken[] tuIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int EditTranslationUnitsWithIterator(Container container, PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator);

		List<PersistentObjectToken> EditTranslationUnitsWithFilter(Container container, PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationUnit[] PreviewEditTranslationUnitsWithIterator(Container container, PersistentObjectToken tmId, EditScript editScript, ref RegularIterator iterator);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int EditTranslationUnits(Container container, PersistentObjectToken tmId, EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] tuIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool DropSchema(Container container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool CleanContainerSchema(Container container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool SchemaExists(Container container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		FieldDefinitions GetFields(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		LanguageResource GetLanguageResource(Container container, PersistentObjectToken resource, bool includeData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		LanguageResource[] GetLanguageResources(Container container, bool includeData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		LanguageResource[] GetTranslationMemoryLanguageResources(Container container, PersistentObjectToken tmId, bool includeData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken[] GetTranslationMemoriesByLanguageResource(Container container, PersistentObjectToken resourceId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationMemorySetup GetTranslationMemoryFromName(Container container, string name);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationMemorySetup GetTranslationMemoryFromId(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationMemorySetup[] GetTranslationMemories(Container container, bool checkPermissions);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		PersistentObjectToken GetTranslationMemoryId(Container container, string name);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationUnit GetTranslationUnit(Container container, PersistentObjectToken tmId, PersistentObjectToken tuId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationUnit[] GetTranslationUnits(Container container, PersistentObjectToken tmId, ref RegularIterator iter);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationUnit[] GetTranslationUnitsWithContextContent(Container container, PersistentObjectToken tmId, ref RegularIterator iter);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool ReindexTranslationUnits(Container container, PersistentObjectToken tmId, ref RegularIterator iter);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void ReindexTranslationUnits(Container container, PersistentObjectToken tmId, CancellationToken token, IProgress<int> progress);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationUnit[] GetDuplicateTranslationUnits(Container container, PersistentObjectToken tmId, ref DuplicateIterator iter);

		TranslationUnit[] GetDuplicateTranslationUnits(Container container, PersistentObjectToken tmId, ref DuplicateIterator iter, bool targetSegments);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int GetTuCount(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool RemoveField(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool RemovePicklistValue(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string value);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool RenamePicklistValue(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string previousName, string newName);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool RenameField(Container container, PersistentObjectToken tmId, PersistentObjectToken fieldId, string newName);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults SearchSegment(Container container, PersistentObjectToken tmId, SearchSettings settings, Segment segment);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults[] SearchSegments(Container container, PersistentObjectToken tmId, SearchSettings settings, Segment[] segments);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults[] SearchSegmentsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, Segment[] segments, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults SearchText(Container container, PersistentObjectToken tmId, SearchSettings settings, string segment);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults SearchTranslationUnit(Container container, PersistentObjectToken tmId, SearchSettings settings, TranslationUnit tu);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults[] SearchTranslationUnits(Container container, PersistentObjectToken tmId, SearchSettings settings, TranslationUnit[] tus);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults[] SearchTranslationUnitsMasked(Container container, PersistentObjectToken tmId, SearchSettings settings, TranslationUnit[] tus, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		SearchResults[] SearchTranslationUnitsBatch(Container container, PersistentObjectToken tmId, SearchSettings settings, TranslationUnit[] tus, bool[] mask, int batchSize, bool overwriteTranslation);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool UnassignLanguageResourceFromTranslationMemory(Container container, PersistentObjectToken resourceId, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool UpdateLanguageResource(Container container, LanguageResource resource);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult UpdateTranslationUnit(Container container, PersistentObjectToken tmId, TranslationUnit tu);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult[] UpdateTranslationUnits(Container container, PersistentObjectToken tmId, TranslationUnit[] tus);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ImportResult[] UpdateTranslationUnitsMasked(Container container, PersistentObjectToken tmId, TranslationUnit[] tus, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool ApplyFieldsToTranslationUnit(Container container, PersistentObjectToken tmId, FieldValues values, bool overwrite, PersistentObjectToken tuId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		int ApplyFieldsToTranslationUnits(Container container, PersistentObjectToken tmId, FieldValues values, bool overwrite, PersistentObjectToken[] tuIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void RecomputeStatistics(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		ServerInfo QueryServerInfo(Container container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void SetAdministratorPassword(Container container, PersistentObjectToken tmId, string pwd);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void SetMaintenancePassword(Container container, PersistentObjectToken tmId, string pwd);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void SetReadWritePassword(Container container, PersistentObjectToken tmId, string pwd);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void SetReadOnlyPassword(Container container, PersistentObjectToken tmId, string pwd);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		FuzzyIndexTuningSettings GetFuzzyIndexTuningSettings(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void SetFuzzyIndexTuningSettings(Container container, PersistentObjectToken tmId, FuzzyIndexTuningSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void ClearFuzzyCache(Container container, PersistentObjectToken tmId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool HasFuzzyCacheNonEmpty(Container container, PersistentObjectToken tmId);

		void ScheduleTusForAlignment(Container container, PersistentObjectToken tmId, PersistentObjectToken[] tuIds);
	}
}
