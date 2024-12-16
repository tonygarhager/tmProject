using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class InMemoryStorage : IStorage, IDisposable
	{
		private static InMemoryStorageImpl _schema;

		private readonly XmlSegmentSerializer _xmlSegmentSerializer = new XmlSegmentSerializer();

		public bool IsInMemoryStorage => true;

		public bool DeleteTmRequiresEmptyTm => false;

		public bool IsReadOnly => false;

		public void DeserializeTuSegments(TranslationUnit storageTu, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			tu.SourceSegment = _xmlSegmentSerializer.DeserializeSegment(storageTu.Source.Text);
			tu.TargetSegment = _xmlSegmentSerializer.DeserializeSegment(storageTu.Target.Text);
		}

		public virtual void SerializeTuSegments(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu, TranslationUnit storageTu)
		{
			storageTu.Source.Text = _xmlSegmentSerializer.SerializeSegment(tu.SourceSegment);
			storageTu.Target.Text = _xmlSegmentSerializer.SerializeSegment(tu.TargetSegment);
		}

		public void BeginTransaction()
		{
		}

		public void CommitTransaction()
		{
		}

		public void AbortTransaction()
		{
		}

		public bool? GetReindexRequired(int tmId, long currentSignatureHash)
		{
			return false;
		}

		public int GetTuCountForReindex(int tmId, long currentSignatureHash)
		{
			throw new NotImplementedException();
		}

		public List<int[]> GetDuplicateSegmentHashes(int tmId, bool target, long? currentSigHash)
		{
			throw new NotImplementedException();
		}

		public void AddDeduplicatedContextHashes(int tmId, ref List<int[]> dupSourceHashDictionary, ref List<int[]> dupTargetHashDictionary)
		{
			throw new NotImplementedException();
		}

		public void ClearFuzzyCache(Container container)
		{
			throw new NotImplementedException();
		}

		public List<PersistentObjectToken> DeleteTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			throw new NotImplementedException();
		}

		private static void VerifySchema()
		{
			if (_schema == null)
			{
				throw new StorageException(ErrorCode.StorageSchemaDoesntExist);
			}
		}

		public void Flush()
		{
		}

		public void CreateSchema()
		{
			if (_schema == null)
			{
				_schema = new InMemoryStorageImpl();
				_schema.CreateSchema();
				return;
			}
			throw new StorageException(ErrorCode.StorageSchemaAlreadyExists);
		}

		public void DropSchema()
		{
			VerifySchema();
			_schema.DropSchema();
			_schema = null;
		}

		public bool SchemaExists()
		{
			return _schema != null;
		}

		public string GetParameter(string parameter)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetParameter(parameter);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public string GetParameter(int tmId, string name)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetParameter(tmId, name);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public void SetParameter(string name, string value)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				_schema.SetParameter(name, value);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public void SetParameter(int tmId, string name, string value)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				_schema.SetParameter(tmId, name, value);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public string GetVersion()
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetVersion();
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<Resource> GetResources(bool includeData)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetResources(includeData);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int GetResourcesWriteCount()
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetResourcesWriteCount();
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<Resource> GetResources(int tmId, bool includeData)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetResources(tmId, includeData);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public Resource GetResource(int key, bool includeData)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetResource(key, includeData);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool AddResource(Resource resource)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.AddResource(resource);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool DeleteResource(int key)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteResource(key);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool UpdateResource(Resource resource)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.UpdateResource(resource);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<TranslationMemory> GetTMsByResourceId(int resourceId)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTMsByResourceId(resourceId);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool AttachTmResource(int tmId, int resourceId)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.AttachTmResource(tmId, resourceId);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool DetachTmResource(int tmId, int resourceId)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DetachTmResource(tmId, resourceId);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<TranslationMemory> GetTms()
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTms();
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int ResolveResourceGuid(Guid guid)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.ResolveResourceGuid(guid);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int ResolveTranslationMemoryGuid(Guid guid)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.ResolveTranslationMemoryGuid(guid);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int ResolveTranslationUnitGuid(int tmId, Guid guid)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.ResolveTranslationUnitGuid(tmId, guid);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int ResolveAttributeGuid(int tmId, Guid guid)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.ResolveAttributeGuid(tmId, guid);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int ResolvePicklistValueGuid(int tmId, Guid guid)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.ResolvePicklistValueGuid(tmId, guid);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public TranslationMemory GetTm(int id)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTm(id);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public TranslationMemory GetTm(string name)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTm(name);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool AddTm(TranslationMemory tm)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.AddTm(tm);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool DeleteTm(int key)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteTm(key);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool DeleteTmSchema(int key)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteTmSchema(key);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool UpdateTm(TranslationMemory tm)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.UpdateTm(tm);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int GetTuCount(int tmId)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTuCount(tmId);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<AttributeDeclaration> GetAttributes(int tmId)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetAttributes(tmId);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public AttributeDeclaration GetAttribute(int tmId, string name)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetAttribute(tmId, name);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public AttributeDeclaration GetAttribute(int tmId, int id)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetAttribute(tmId, id);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool AddAttribute(AttributeDeclaration attribute)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.AddAttribute(attribute);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool DeleteAttribute(int tmId, int key)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteAttribute(tmId, key);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool DeleteAttribute(int tmId, string name)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteAttribute(tmId, name);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public PickValue GetPicklistValue(int tmId, int key)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetPicklistValue(tmId, key);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int AddPicklistValue(int tmId, int attributeId, PickValue value)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.AddPicklistValue(tmId, attributeId, value);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int DeleteOrphanContexts(int tmId, TextContextMatchType textContextMatchType)
		{
			throw new NotImplementedException();
		}

		public bool DeletePicklistValue(int tmId, int attributeId, string value)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeletePicklistValue(tmId, attributeId, value);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool RenameAttribute(int tmId, int attributeKey, string newName)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.RenameAttribute(tmId, attributeKey, newName);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool RenamePicklistValue(int tmId, int attributeId, string oldName, string newName)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.RenamePicklistValue(tmId, attributeId, oldName, newName);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public void AddTu(TranslationUnit tu, FuzzyIndexes indexes, bool keepId, long tokenizationSignatureHash)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				_schema.AddTu(tu, indexes, keepId, tokenizationSignatureHash);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<Tuple<Guid, int>> AddTus(Tuple<TranslationUnit, ImportType>[] batchTUs, FuzzyIndexes indexes, long tokenizationSignatureHash, int tmid)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetTus(int tmId, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTus(tmId, startAfter, count, forward, idContextMatch, includeContextContent: false, TextContextMatchType.PrecedingAndFollowingSource, null, null);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<TranslationUnit> GetTusForReindex(int tmId, int startAfter, int count, long currentSigHash)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetTus(int tmId, List<Tuple<int, long, long>> tuAndHash, bool unused)
		{
			throw new NotImplementedException();
		}

		public List<PersistentObjectToken> GetTuIds(int tmId, int startAfter, int count, bool forward)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTuIds(tmId, startAfter, count, forward);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public TranslationUnit GetTu(int tmId, int key, bool idContextMatch)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTu(tmId, key, idContextMatch);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool DeleteTu(int tmId, PersistentObjectToken key, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteTu(tmId, key, TextContextMatchType.PrecedingSourceAndTarget, deleteOrphanContexts);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<PersistentObjectToken> DeleteTus(int tmId, List<PersistentObjectToken> keys, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteTus(tmId, keys, TextContextMatchType.PrecedingSourceAndTarget, deleteOrphanContexts);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public int DeleteAllTus(int tmId)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DeleteAllTus(tmId);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public bool UpdateTuHeader(TranslationUnit tu, bool rewriteAttributes)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.UpdateTuHeader(tu, rewriteAttributes);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		private void UpdateTuIndices(TranslationUnit tu, FuzzyIndexes indexes, long tokenizationSignatureHash, TextContextMatchType textContextMatchType)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				_schema.UpdateTuIndices(new List<TranslationUnit>
				{
					tu
				}, indexes, tokenizationSignatureHash, textContextMatchType);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public void UpdateTuIndices(List<TranslationUnit> tus, FuzzyIndexes indexes, long tokenizationSignatureHash, TextContextMatchType textContextMatchType)
		{
			foreach (TranslationUnit tu in tus)
			{
				UpdateTuIndices(tu, indexes, tokenizationSignatureHash, textContextMatchType);
			}
		}

		public void ClearFuzzyIndex(FuzzyIndexes index)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				_schema.ClearFuzzyIndex(index);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public TuContexts GetTextContexts(int tmId, int tuId)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.GetTextContexts(tmId, tuId);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public void AddContexts(int tmId, int tuId, TuContexts contexts)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				_schema.AddContexts(tmId, tuId, contexts);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public void AddIdContexts(int tmId, int tuId, TuIdContexts contexts)
		{
		}

		public List<TranslationUnit> FuzzySearch(int tmId, List<int> features, FuzzyIndexes index, int minScore, int maxHits, bool concordance, int lastTuId, TuContextData tuContextData, bool descendingOrder)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.FuzzySearch(tmId, features, index, minScore, maxHits, concordance, lastTuId, tuContextData, descendingOrder);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, FilterExpression hardFilter)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, int maxHits)
		{
			throw new NotImplementedException();
		}

		public void AddorUpdateLastSearch(int tmId, List<int> tuIds, DateTime lastSearch)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> ExactSearch(int tmId, long sourceHash, long targetHash, int maxHits, DateTime lastChangeDate, int skipRows, TuContextData tuContextData, bool descendingOrder, List<int> tuIdsToSkip)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.ExactSearch(tmId, sourceHash, targetHash, maxHits, lastChangeDate, skipRows, tuContextData, descendingOrder, tuIdsToSkip);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public List<TranslationUnit> DuplicateSearch(int tmId, long lastHash, int lastTuId, int count, bool forward, bool targetSegments)
		{
			VerifySchema();
			try
			{
				Monitor.Enter(_schema);
				return _schema.DuplicateSearch(tmId, lastHash, lastTuId, count, forward, targetSegments);
			}
			finally
			{
				Monitor.Exit(_schema);
			}
		}

		public void RecomputeFrequencies(int tmId)
		{
			VerifySchema();
		}

		public void Dispose()
		{
		}

		public List<TranslationUnit> SubsegmentSearch(int tmId, List<long> fragmentHashes, byte minFragmentLength, byte minSigWords, int maxHits, Dictionary<int, HashSet<long>> hashesPerTu)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			throw new NotImplementedException();
		}

		public bool CleanupSchema()
		{
			return true;
		}

		public void ScheduleTusForAlignment(int id, List<int> ids)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> FuzzySearch(int tmId, Dictionary<int, List<int>> features, int minScore, int maxHits)
		{
			throw new NotImplementedException();
		}

		Dictionary<int, List<TranslationUnit>> IStorage.FuzzySearch(int tmId, Dictionary<int, List<int>> features, int minScore, int maxHits, FilterExpression hardFilter)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> DuplicateSearch(int tmId, List<long> sourceHashes, List<long> targetHashes)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetFullTusByIds(int tmId, List<int> tuIds)
		{
			throw new NotImplementedException();
		}
	}
}
