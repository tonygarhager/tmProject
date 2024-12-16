using Sdl.Core.Api.DataAccess;
using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;
using System;
using System.ServiceModel;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Services
{
	[ServiceContract(Name = "TMAdmin", Namespace = "http://sdl.com/languageplatform/2010")]
	public interface ITranslationMemoryAdministration2014
	{
		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		DatabaseServerEntity CreateDatabaseServer(DatabaseServerEntity databaseServer, string resourceGroupPath);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		DatabaseServerEntity[] GetDatabaseServers(string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		DatabaseServerEntity GetDatabaseServerById(Identity databaseServerId, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		DatabaseServerEntity GetDatabaseServerByUniqueId(Guid databaseServerId, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		DatabaseServerEntity[] GetDatabaseServersByName(string name, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		DatabaseServerEntity GetDatabaseServerByPath(string path, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		DatabaseServerEntity UpdateDatabaseServer(DatabaseServerEntity databaseServer);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeleteDatabaseServer(Identity databaseServer, bool deleteContainers);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity CreateContainer(ContainerEntity container, string resourceGroupPath);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity[] GetContainersByDatabaseServerId(Identity databaseServerId, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity[] GetContainers(string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity GetContainerById(Identity containerId, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity GetContainerByUniqueId(Guid containerId, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity[] GetContainersByName(string name, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity GetContainerByPath(string path, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ContainerEntity UpdateContainer(ContainerEntity container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeleteContainer(Identity id, bool deleteContainerDatabase);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntityQueryFilters GetTranslationMemoriesQueryFilters();

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		PagedTranslationMemoryEntities GetTranslationMemoriesByQuery(TranslationMemoryEntityQuery query);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity[] GetTranslationMemories(string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity[] GetTranslationMemoriesByFieldGroupTemplateId(Identity fieldGroupTemplateId, string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity[] GetTranslationMemoriesByLanguageResourceTemplateId(Identity languageResourceTemplateId, string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity GetTranslationMemoryById(Identity id, string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity GetTranslationMemoryByUniqueId(Guid uniqueId, string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity[] GetTranslationMemoriesByName(string name, string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity GetTranslationMemoryByPath(string path, string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity CreateTranslationMemory(TranslationMemoryEntity translationMemoryEntity, string resourceGroupPath);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity UpdateTranslationMemory(TranslationMemoryEntity translationMemoryEntity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeleteTranslationMemory(Identity id);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageDirectionEntity GetLanguageDirectionById(Identity languageDirectionId, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageDirectionEntity[] GetLanguageDirectionsByTranslationMemoryId(Identity id, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationMemoryEntity[] GetTranslationMemoryByContainerId(Identity id, string[] relationships, bool includeLanguageResourceData, bool includeScheduledOperations);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		string[] GetTranslationMemoryNamesByContainerId(Identity id);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		int DeleteAllTranslationUnits(Identity languageDirectionId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		bool DeleteTranslationUnit(Identity languageDirectionId, PersistentObjectToken tuId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		int DeleteTranslationUnits(Identity languageDirectionId, PersistentObjectToken[] tuIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		int DeleteTranslationUnitsWithIterator(Identity languageDirectionId, ref RegularIterator iterator);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		int EditTranslationUnits(Identity languageDirectionId, EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] tuIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		int EditTranslationUnitsWithIterator(Identity languageDirectionId, EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		TranslationUnit[] GetDuplicateTranslationUnits(Identity languageDirectionId, ref DuplicateIterator iter);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		TranslationUnit GetTranslationUnit(Identity languageDirectionId, PersistentObjectToken tuId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		TranslationUnit[] GetTranslationUnits(Identity languageDirectionId, ref RegularIterator iter);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		int GetTuCount(Identity languageDirectionId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		TranslationUnit[] PreviewEditTranslationUnitsWithIterator(Identity languageDirectionId, EditScript editScript, ref RegularIterator iterator);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		bool ReindexTranslationUnits(Identity languageDirectionId, ref RegularIterator iterator);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult[] UpdateTranslationUnitsMasked(Identity languageDirectionId, TranslationUnit[] tus, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult[] AddOrUpdateTranslationUnits(Identity languageDirectionId, TranslationUnit[] tus, int[] previousTranslationHashes, ImportSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult[] AddOrUpdateTranslationUnitsMasked(Identity languageDirectionId, TranslationUnit[] tus, int[] previousTranslationHashes, ImportSettings settings, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult AddTranslationUnit(Identity languageDirectionId, TranslationUnit tu, ImportSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult[] AddTranslationUnits(Identity languageDirectionId, TranslationUnit[] tus, ImportSettings settings);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult[] AddTranslationUnitsMasked(Identity languageDirectionId, TranslationUnit[] tus, ImportSettings settings, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		SearchResults SearchSegment(Identity languageDirectionId, SearchSettings settings, Segment segment);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		SearchResults[] SearchSegments(Identity languageDirectionId, SearchSettings settings, Segment[] segments);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		SearchResults[] SearchSegmentsMasked(Identity languageDirectionId, SearchSettings settings, Segment[] segments, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		SearchResults SearchText(Identity languageDirectionId, SearchSettings settings, string segment);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		SearchResults SearchTranslationUnit(Identity languageDirectionId, SearchSettings settings, TranslationUnit tu);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		SearchResults[] SearchTranslationUnits(Identity languageDirectionId, SearchSettings settings, TranslationUnit[] tus);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		SearchResults[] SearchTranslationUnitsMasked(Identity languageDirectionId, SearchSettings settings, TranslationUnit[] tus, bool[] mask);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult UpdateTranslationUnit(Identity languageDirectionId, TranslationUnit tu);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		ImportResult[] UpdateTranslationUnits(Identity languageDirectionId, TranslationUnit[] tus);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		bool ApplyFieldsToTranslationUnit(Identity languageDirectionId, FieldValues values, bool overwrite, PersistentObjectToken tuId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		[OperationTimeout(Minutes = 10)]
		int ApplyFieldsToTranslationUnits(Identity languageDirectionId, FieldValues values, bool overwrite, PersistentObjectToken[] tuIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationSequenceEntity CreateTranslationSequence(TranslationSequenceEntity translationSequenceEntity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationSequenceEntity GetTranslationSequenceByUniqueId(Guid id, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationSequenceEntity GetTranslationSequenceByName(string name, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationSequenceEntity[] GetTranslationSequences(string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationSequenceItemEntity[] GetTranslationSequenceItemsByTranslationSequenceId(Identity id);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TranslationSequenceEntity UpdateTranslationSequence(TranslationSequenceEntity translationSequenceEntity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeleteTranslationSequence(Identity id);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldEntity CreateField(FieldEntity field);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldEntity[] GetFieldsByGroupId(Guid groupId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldEntity[] GetFields();

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldEntity GetFieldByUniqueId(Guid fieldId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldEntity GetFieldById(Identity fieldId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldEntity GetFieldByName(string name, Guid groupTemplateEntity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldEntity UpdateField(FieldEntity field);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeleteField(FieldEntity fieldEntity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		PicklistItemEntity CreatePicklist(PicklistItemEntity item);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		PicklistItemEntity[] GetPicklistItems(Guid fieldId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		PicklistItemEntity[] GetPickListItems();

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		PicklistItemEntity GetPicklistItemByUniqueId(Guid picklistItemId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		PicklistItemEntity GetPickListItemById(Identity picklistItemId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		PicklistItemEntity UpdatePicklistItem(PicklistItemEntity picklistItem);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeletePicklistItem(PicklistItemEntity picklistItemEntity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldGroupTemplateEntity CreateFieldGroupTemplate(FieldGroupTemplateEntity fieldGroupTemplate, string resourceGroupPath);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldGroupTemplateEntity[] GetFieldGroupTemplatesByName(string name, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldGroupTemplateEntity GetFieldGroupTemplateByPath(string path, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldGroupTemplateEntity[] GetFieldGroupTemplates(string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldGroupTemplateEntity GetFieldGroupTemplateByUniqueId(Guid uniqueId, string[] relationships);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		FieldGroupTemplateEntity UpdateFieldGroupTemplate(FieldGroupTemplateEntity fieldGroupTemplate);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeleteFieldGroupTemplate(Identity id);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		TMSequenceRef[] GetTMSequences();

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void DeleteLanguageResourceTemplate(Identity id);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceEntity[] GetLanguageResourcesByTemplateId(Identity id);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceTemplateEntity GetLanguageResourceTemplateByUniqueId(Guid uniqueId, string[] relationships, bool includeLanguageResourceData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceTemplateEntity[] GetLanguageResourceTemplatesByName(string name, string[] relationships, bool includesLanguageResourceData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceTemplateEntity GetLanguageResourceTemplateByPath(string path, string[] relationships, bool includesLanguageResourceData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceEntity[] GetDefaultLanguageResources(string cultureName);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceTemplateEntity CreateLanguageResourceTemplate(LanguageResourceTemplateEntity languageResourceTemplateEntity, string resourceGroupPath);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceTemplateEntity UpdateLanguageResourceTemplate(LanguageResourceTemplateEntity languageResourceTemplateEntity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LanguageResourceTemplateEntity[] GetLanguageResourceTemplates(string[] relationships, bool includesLanguageResourceData);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ImportEntity CreateTranslationMemoryImport(ImportEntity importInfo);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void QueueTranslationMemoryImport(Identity importId, string importFileName, bool recomputeFuzzyIndex);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void UpdateImportStatistics(Identity importId, ImportStatistics statistics);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void ExitImportOnError(Identity importId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ImportEntity GetTranslationMemoryImportById(Identity importId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ImportEntity GetTranslationMemoryImportByWorkItemUniqueId(Guid workItemId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ExportEntity QueueTranslationMemoryExport(ExportEntity export);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ExportEntity GetTranslationMemoryExportById(Identity exportId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		ExportEntity GetTranslationMemoryExportByWorkItemUniqueId(Guid workItemId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void UpdateExportStatistics(Identity exportId, ExportStatistics statitics);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void ExitExportOnError(Identity exportId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		void UpdateLicensingRestrictions(LicensingRestrictionsEntity entity);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LicensingRestrictionsEntity GetLicensingRestrictions();

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		LicensingRestrictionsEntity GetLicensingRestrictionsByLabel(string label);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		RecomputeStatisticsResult RecomputeFuzzyIndexStatistics(Identity translationMemoryId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ScheduledOperationEntity ScheduleRecomputeStatistics(Identity translationMemoryId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void ReindexTranslationMemory(Identity translationMemoryId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ScheduledOperationEntity ScheduleReindexTranslationMemory(Identity translationMemoryId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ScheduledOperationEntity GetScheduledOperation(Guid workItemId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void ApplyFieldGroupChangeSet(Identity translationMemoryId, FieldGroupChangeSet changeset);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void ApplyLanguageResourceGroupChangeSet(Identity translationMemoryId, LanguageResourceGroupChangeSet changeset);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteTranslationWorkItemsByUniqueIds(Guid[] workItemIds);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		[FaultContract(typeof(ServiceError))]
		int UpdateTranslationUnitCount(Identity languageDirectionId);
	}
}
