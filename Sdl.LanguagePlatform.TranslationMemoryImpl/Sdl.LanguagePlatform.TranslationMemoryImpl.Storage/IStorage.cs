using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal interface IStorage : IDisposable
	{
		bool IsInMemoryStorage
		{
			get;
		}

		bool DeleteTmRequiresEmptyTm
		{
			get;
		}

		bool IsReadOnly
		{
			get;
		}

		void Flush();

		void CreateSchema();

		void DropSchema();

		bool SchemaExists();

		bool AddTm(TranslationMemory tm);

		TranslationMemory GetTm(int id);

		TranslationMemory GetTm(string name);

		List<TranslationMemory> GetTms();

		bool UpdateTm(TranslationMemory tm);

		bool DeleteTm(int key);

		bool DeleteTmSchema(int key);

		bool CleanupSchema();

		bool AddAttribute(AttributeDeclaration attribute);

		List<AttributeDeclaration> GetAttributes(int tmId);

		AttributeDeclaration GetAttribute(int tmId, string name);

		AttributeDeclaration GetAttribute(int tmId, int id);

		bool RenameAttribute(int tmId, int attributeKey, string newName);

		bool DeleteAttribute(int tmId, int key);

		bool DeleteAttribute(int tmId, string name);

		int AddPicklistValue(int tmId, int attributeId, PickValue value);

		PickValue GetPicklistValue(int tmId, int key);

		bool RenamePicklistValue(int tmId, int attributeId, string oldName, string newName);

		bool DeletePicklistValue(int tmId, int attributeId, string value);

		int DeleteOrphanContexts(int tmId, TextContextMatchType textContextMatchType);

		string GetParameter(string parameter);

		string GetParameter(int tmId, string parameter);

		void SetParameter(string parameter, string value);

		void SetParameter(int tmId, string parameter, string value);

		string GetVersion();

		int ResolveResourceGuid(Guid guid);

		int ResolveTranslationMemoryGuid(Guid guid);

		int ResolveTranslationUnitGuid(int tmId, Guid guid);

		int ResolveAttributeGuid(int tmId, Guid guid);

		int ResolvePicklistValueGuid(int tmId, Guid guid);

		List<Resource> GetResources(bool includeData);

		List<Resource> GetResources(int tmId, bool includeData);

		Resource GetResource(int key, bool includeData);

		int GetResourcesWriteCount();

		bool AddResource(Resource resource);

		bool DeleteResource(int key);

		bool UpdateResource(Resource resource);

		List<TranslationMemory> GetTMsByResourceId(int resourceId);

		bool AttachTmResource(int tmId, int resourceId);

		bool DetachTmResource(int tmId, int resourceId);

		void AddTu(TranslationUnit tu, FuzzyIndexes indexes, bool keepId, long tokenizationSignatureHash);

		List<Tuple<Guid, int>> AddTus(Tuple<TranslationUnit, ImportType>[] batchTUs, FuzzyIndexes indexes, long tokenizationSignatureHash, int tmid);

		TranslationUnit GetTu(int tmId, int key, bool idContextMatch);

		int GetTuCount(int tmId);

		List<TranslationUnit> GetTus(int tmId, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture);

		List<TranslationUnit> GetTusForReindex(int tmId, int startAfter, int count, long currentSigHash);

		List<TranslationUnit> GetTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture);

		List<TranslationUnit> GetTus(int tmId, List<Tuple<int, long, long>> tuAndHash, bool idContextMatch);

		List<PersistentObjectToken> GetTuIds(int tmId, int startAfter, int count, bool forward);

		bool UpdateTuHeader(TranslationUnit tu, bool rewriteAttributes);

		void UpdateTuIndices(List<TranslationUnit> tus, FuzzyIndexes indexes, long tokenizationSignatureHash, TextContextMatchType textContextMatchType);

		bool DeleteTu(int tmId, PersistentObjectToken key, TextContextMatchType textContextMatchType, bool deleteOrphanContexts);

		List<PersistentObjectToken> DeleteTus(int tmId, List<PersistentObjectToken> keys, TextContextMatchType textContextMatchType, bool deleteOrphanContexts);

		int DeleteAllTus(int tmId);

		TuContexts GetTextContexts(int tmId, int tuId);

		void AddContexts(int tmId, int tuId, TuContexts contexts);

		void AddIdContexts(int tmId, int tuId, TuIdContexts contexts);

		void AddorUpdateLastSearch(int tmId, List<int> tuIds, DateTime lastSearch);

		List<TranslationUnit> ExactSearch(int tmId, long sourceHash, long targetHash, int maxHits, DateTime lastChangeDate, int skipRows, TuContextData tuContextData, bool descendingOrder, List<int> tuIdsToSkip);

		List<TranslationUnit> GetFullTusByIds(int tmId, List<int> tuIds);

		List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, FilterExpression filter);

		List<TranslationUnit> DuplicateSearch(int tmId, List<long> sourceHashes, List<long> targetHashes);

		List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, int maxHits);

		List<TranslationUnit> SubsegmentSearch(int tmId, List<long> fragmentHashes, byte minFragmentLength, byte minSigWords, int maxHits, Dictionary<int, HashSet<long>> hashesPerTu);

		List<TranslationUnit> FuzzySearch(int tmId, List<int> features, FuzzyIndexes index, int minScore, int maxHits, bool concordance, int lastTuId, TuContextData tuContextData, bool descendingOrder);

		Dictionary<int, List<TranslationUnit>> FuzzySearch(int tmId, Dictionary<int, List<int>> features, int minScore, int maxHits, FilterExpression hardFilter);

		List<TranslationUnit> DuplicateSearch(int tmId, long lastHash, int lastTuId, int count, bool forward, bool targetSegments);

		void RecomputeFrequencies(int tmId);

		void ClearFuzzyIndex(FuzzyIndexes index);

		void BeginTransaction();

		void CommitTransaction();

		void AbortTransaction();

		bool? GetReindexRequired(int tmId, long currentSignatureHash);

		int GetTuCountForReindex(int tmId, long currentSignatureHash);

		List<int[]> GetDuplicateSegmentHashes(int tmId, bool target, long? currentSigHash);

		void AddDeduplicatedContextHashes(int tmId, ref List<int[]> tuIdsWithDupSourceHashes, ref List<int[]> tuIdsWithDupTargetHashes);

		void DeserializeTuSegments(TranslationUnit storageTu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, CultureInfo sourceCulture, CultureInfo targetCulture);

		void SerializeTuSegments(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, TranslationUnit storageTu);

		void ClearFuzzyCache(Container container);

		List<PersistentObjectToken> DeleteTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, TextContextMatchType textContextMatchType, bool deleteOrphanContexts);
	}
}
