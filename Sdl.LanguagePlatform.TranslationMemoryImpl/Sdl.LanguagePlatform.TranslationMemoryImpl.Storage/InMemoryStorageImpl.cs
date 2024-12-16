using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Lingua.Index;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class InMemoryStorageImpl : IStorage, IDisposable
	{
		private class TuKeyComparer : IComparer<TranslationUnit>
		{
			public int Compare(TranslationUnit x, TranslationUnit y)
			{
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				return x.Id - y.Id;
			}
		}

		private readonly XmlSegmentSerializer _xmlSegmentSerializer = new XmlSegmentSerializer();

		private readonly List<TranslationUnit> _tUs;

		private readonly Dictionary<int, TranslationUnit> _tuKeyIndex;

		private readonly List<TranslationMemory> _tms;

		private readonly List<Resource> _resources;

		private readonly Dictionary<string, string> _parameters;

		private readonly List<Pair<int>> _resourceTmRelations;

		private readonly List<AttributeDeclaration> _attributeDeclarations;

		private int _tmIdSequence;

		private int _picklistIdSequence;

		private int _tuIdSequence;

		private int _resourceIdSequence;

		private int _attributeIdSequence;

		private readonly TuKeyComparer _tuKeyComparer;

		private readonly Dictionary<long, List<TranslationUnit>> _tuSourceHashIndex;

		private readonly InMemoryFuzzyIndex[] _fuzzyIndices;

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

		public InMemoryStorageImpl()
		{
			_tUs = new List<TranslationUnit>();
			_tuKeyIndex = new Dictionary<int, TranslationUnit>();
			_tms = new List<TranslationMemory>();
			_parameters = new Dictionary<string, string>();
			_resources = new List<Resource>();
			_resourceTmRelations = new List<Pair<int>>();
			_attributeDeclarations = new List<AttributeDeclaration>();
			_tuSourceHashIndex = new Dictionary<long, List<TranslationUnit>>();
			_tuKeyComparer = new TuKeyComparer();
			_fuzzyIndices = new InMemoryFuzzyIndex[10];
			for (int i = 0; i < 10; i++)
			{
				if (i == 2 || i == 1 || i == 8 || i == 4)
				{
					_fuzzyIndices[i] = new InMemoryFuzzyIndex();
				}
			}
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

		public void Flush()
		{
		}

		public void CreateSchema()
		{
			_parameters.Add("VERSION", "8.02");
		}

		public void DropSchema()
		{
		}

		public bool SchemaExists()
		{
			return true;
		}

		public string GetParameter(string parameter)
		{
			if (!_parameters.TryGetValue(parameter, out string value))
			{
				return null;
			}
			return value;
		}

		public string GetParameter(int tmId, string name)
		{
			throw new NotImplementedException();
		}

		public void SetParameter(string name, string value)
		{
			if (_parameters.ContainsKey(name))
			{
				_parameters[name] = value;
			}
			else
			{
				_parameters.Add(name, value);
			}
		}

		public void SetParameter(int tmId, string name, string value)
		{
			throw new NotImplementedException();
		}

		public string GetVersion()
		{
			return GetParameter("VERSION");
		}

		public List<Resource> GetResources(bool includeData)
		{
			return _resources;
		}

		public List<Resource> GetResources(int tmId, bool includeData)
		{
			List<Resource> list = new List<Resource>();
			foreach (Pair<int> resourceTmRelation in _resourceTmRelations)
			{
				if (resourceTmRelation.Right == tmId)
				{
					Resource resource = GetResource(resourceTmRelation.Left, includeData);
					if (resource != null)
					{
						list.Add(resource);
					}
				}
			}
			return list;
		}

		public int GetResourcesWriteCount()
		{
			return -1;
		}

		public Resource GetResource(int key, bool includeData)
		{
			foreach (Resource resource in _resources)
			{
				if (resource.Id == key)
				{
					return resource;
				}
			}
			return null;
		}

		public bool AddResource(Resource resource)
		{
			if (_resources.FindIndex((Resource r) => string.Equals(r.Language, resource.Language, StringComparison.OrdinalIgnoreCase) && r.Type == resource.Type) >= 0)
			{
				return false;
			}
			resource.Id = ++_resourceIdSequence;
			_resources.Add(resource);
			return true;
		}

		public bool DeleteResource(int key)
		{
			for (int num = _resourceTmRelations.Count - 1; num >= 0; num--)
			{
				if (_resourceTmRelations[num].Left == key)
				{
					_resourceTmRelations.RemoveAt(num);
				}
			}
			for (int i = 0; i < _resources.Count; i++)
			{
				if (_resources[i].Id == key)
				{
					_resources.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public bool UpdateResource(Resource resource)
		{
			for (int i = 0; i < _resources.Count; i++)
			{
				if (_resources[i].Id == resource.Id)
				{
					_resources[i] = resource;
					return true;
				}
			}
			return false;
		}

		public List<TranslationMemory> GetTMsByResourceId(int resourceId)
		{
			List<TranslationMemory> list = new List<TranslationMemory>();
			foreach (Pair<int> resourceTmRelation in _resourceTmRelations)
			{
				if (resourceTmRelation.Left == resourceId)
				{
					list.Add(GetTm(resourceTmRelation.Right));
				}
			}
			return list;
		}

		public bool AttachTmResource(int tmId, int resourceId)
		{
			foreach (Pair<int> resourceTmRelation in _resourceTmRelations)
			{
				if (resourceTmRelation.Left == resourceId && resourceTmRelation.Right == tmId)
				{
					return false;
				}
			}
			_resourceTmRelations.Add(new Pair<int>(resourceId, tmId));
			return true;
		}

		public bool DetachTmResource(int tmId, int resourceId)
		{
			for (int i = 0; i < _resourceTmRelations.Count; i++)
			{
				if (_resourceTmRelations[i].Left == resourceId && _resourceTmRelations[i].Right == tmId)
				{
					_resourceTmRelations.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public int ResolveResourceGuid(Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveTranslationMemoryGuid(Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveTranslationUnitGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolveAttributeGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public int ResolvePicklistValueGuid(int tmId, Guid guid)
		{
			throw new NotImplementedException();
		}

		public List<TranslationMemory> GetTms()
		{
			return new List<TranslationMemory>(_tms);
		}

		public TranslationMemory GetTm(int id)
		{
			foreach (TranslationMemory tm in _tms)
			{
				if (tm.Id == id)
				{
					return tm;
				}
			}
			return null;
		}

		public TranslationMemory GetTm(string name)
		{
			foreach (TranslationMemory tm in _tms)
			{
				if (tm.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return tm;
				}
			}
			return null;
		}

		public bool AddTm(TranslationMemory tm)
		{
			if (GetTm(tm.Name) != null)
			{
				return false;
			}
			tm.Id = ++_tmIdSequence;
			tm.CreationDate = DbStorageBase.Normalize(tm.CreationDate);
			_tms.Add(tm);
			return true;
		}

		public bool DeleteTm(int key)
		{
			DeleteAllTus(key);
			for (int i = 0; i < _tms.Count; i++)
			{
				if (_tms[i].Id == key)
				{
					_tms.RemoveAt(i);
					_attributeDeclarations.RemoveAll((AttributeDeclaration x) => x.TMId == key);
					_resourceTmRelations.RemoveAll((Pair<int> x) => x.Right == key);
					return true;
				}
			}
			return false;
		}

		public bool UpdateTm(TranslationMemory tm)
		{
			for (int i = 0; i < _tms.Count; i++)
			{
				if (_tms[i].Id == tm.Id)
				{
					_tms[i] = tm;
					return true;
				}
			}
			return false;
		}

		public int GetTuCount(int tmId)
		{
			if (GetTm(tmId) == null)
			{
				throw new StorageException(ErrorCode.TMOrContainerMissing);
			}
			int num = 0;
			foreach (TranslationUnit tU in _tUs)
			{
				if (tU != null && tU.TranslationMemoryId == tmId)
				{
					num++;
				}
			}
			return num;
		}

		public List<AttributeDeclaration> GetAttributes(int tmId)
		{
			return _attributeDeclarations.Where((AttributeDeclaration t) => t.TMId == tmId).ToList();
		}

		public AttributeDeclaration GetAttribute(int tmId, string name)
		{
			return _attributeDeclarations.FirstOrDefault((AttributeDeclaration t) => t.TMId == tmId && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public AttributeDeclaration GetAttribute(int tmId, int id)
		{
			return _attributeDeclarations.FirstOrDefault((AttributeDeclaration t) => t.Id == id);
		}

		public bool AddAttribute(AttributeDeclaration attribute)
		{
			if (GetAttribute(attribute.TMId, attribute.Name) != null)
			{
				throw new StorageException(ErrorCode.StorageFieldAlreadyExists, attribute.Name);
			}
			attribute.Id = ++_attributeIdSequence;
			_attributeDeclarations.Add(attribute);
			if ((attribute.Type != FieldValueType.SinglePicklist && attribute.Type != FieldValueType.MultiplePicklist) || attribute.Picklist == null)
			{
				return true;
			}
			foreach (PickValue item in attribute.Picklist)
			{
				item.Id = ++_picklistIdSequence;
			}
			return true;
		}

		public void DeleteAllAttributeValues(int tmId, int attributeId)
		{
			throw new NotImplementedException();
		}

		public bool DeleteAttribute(int tmId, int key)
		{
			DeleteAllAttributeValues(tmId, key);
			for (int num = _attributeDeclarations.Count - 1; num >= 0; num--)
			{
				if (_attributeDeclarations[num].Id == key)
				{
					_attributeDeclarations.RemoveAt(num);
					return true;
				}
			}
			return false;
		}

		public bool DeleteAttribute(int tmId, string name)
		{
			for (int i = 0; i < _attributeDeclarations.Count; i++)
			{
				if (_attributeDeclarations[i].TMId == tmId && _attributeDeclarations[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					DeleteAllAttributeValues(tmId, _attributeDeclarations[i].Id);
					_attributeDeclarations.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public PickValue GetPicklistValue(int tmId, int key)
		{
			foreach (AttributeDeclaration attributeDeclaration in _attributeDeclarations)
			{
				if ((attributeDeclaration.Type == FieldValueType.SinglePicklist || attributeDeclaration.Type == FieldValueType.MultiplePicklist) && attributeDeclaration.Picklist != null)
				{
					foreach (PickValue item in attributeDeclaration.Picklist)
					{
						if (item.Id == key)
						{
							return item;
						}
					}
				}
			}
			return null;
		}

		public int AddPicklistValue(int tmId, int attributeId, PickValue value)
		{
			foreach (AttributeDeclaration attributeDeclaration in _attributeDeclarations)
			{
				if (attributeDeclaration.Id == attributeId)
				{
					if (attributeDeclaration.Type != FieldValueType.MultiplePicklist && attributeDeclaration.Type != FieldValueType.SinglePicklist)
					{
						throw new StorageException(ErrorCode.StorageIncompatibleAttributeType);
					}
					if (attributeDeclaration.Picklist == null)
					{
						attributeDeclaration.Picklist = new List<PickValue>();
					}
					else if (attributeDeclaration.Picklist.FindIndex((PickValue piv) => piv.Value.Equals(value.Value, StringComparison.OrdinalIgnoreCase)) >= 0)
					{
						throw new StorageException(ErrorCode.StoragePicklistValueAlreadyExists, value.Value);
					}
					PickValue pickValue = new PickValue(++_picklistIdSequence, Guid.NewGuid(), value.Value);
					attributeDeclaration.Picklist.Add(pickValue);
					return pickValue.Id;
				}
			}
			throw new StorageException(ErrorCode.StorageFieldNotFound);
		}

		public bool DeletePicklistValue(int tmId, int attributeId, string value)
		{
			foreach (AttributeDeclaration attributeDeclaration in _attributeDeclarations)
			{
				if (attributeDeclaration.Id == attributeId)
				{
					if (attributeDeclaration.Type != FieldValueType.MultiplePicklist && attributeDeclaration.Type != FieldValueType.SinglePicklist)
					{
						throw new StorageException(ErrorCode.StorageIncompatibleAttributeType, attributeDeclaration.Name);
					}
					if (attributeDeclaration.Picklist == null)
					{
						return false;
					}
					int num = attributeDeclaration.Picklist.FindIndex((PickValue piv) => piv.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
					if (num >= 0)
					{
						attributeDeclaration.Picklist.RemoveAt(num);
						return true;
					}
				}
			}
			return false;
		}

		public int DeleteOrphanContexts(int tmId, TextContextMatchType textContextMatchType)
		{
			throw new NotImplementedException();
		}

		public bool RenameAttribute(int tmId, int attributeKey, string newName)
		{
			if (GetAttribute(tmId, newName) != null)
			{
				throw new StorageException(ErrorCode.StorageFieldAlreadyExists, newName);
			}
			AttributeDeclaration attribute = GetAttribute(tmId, attributeKey);
			if (attribute == null || attribute.TMId != tmId)
			{
				return false;
			}
			attribute.Name = newName;
			return true;
		}

		private static int FindPickValueIndex(List<PickValue> list, string name)
		{
			return list.FindIndex((PickValue piv) => piv.Value.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public bool RenamePicklistValue(int tmId, int attributeId, string oldName, string newName)
		{
			foreach (AttributeDeclaration attributeDeclaration in _attributeDeclarations)
			{
				if (attributeDeclaration.Id == attributeId)
				{
					if (attributeDeclaration.Type != FieldValueType.MultiplePicklist && attributeDeclaration.Type != FieldValueType.SinglePicklist)
					{
						throw new StorageException(ErrorCode.StorageIncompatibleAttributeType, attributeDeclaration.Name);
					}
					if (attributeDeclaration.Picklist == null)
					{
						return false;
					}
					int num = FindPickValueIndex(attributeDeclaration.Picklist, oldName);
					if (num >= 0)
					{
						if (FindPickValueIndex(attributeDeclaration.Picklist, newName) >= 0)
						{
							throw new StorageException(ErrorCode.StoragePicklistValueAlreadyExists, newName);
						}
						attributeDeclaration.Picklist[num].Value = newName;
						return true;
					}
				}
			}
			return false;
		}

		public void AddTu(TranslationUnit tu, FuzzyIndexes indexes, bool keepId, long tokenizationSignatureHash)
		{
			if (keepId && tu.Id <= 0)
			{
				keepId = false;
			}
			tu.CreationDate = DbStorageBase.Normalize(tu.CreationDate);
			tu.ChangeDate = DbStorageBase.Normalize(tu.ChangeDate);
			int num = 0;
			if (keepId)
			{
				num = GetTuIndex(tu.Id);
				if (num >= 0)
				{
					keepId = false;
				}
			}
			if (keepId)
			{
				_tUs.Insert(~num, tu);
			}
			else
			{
				tu.Id = ++_tuIdSequence;
				_tUs.Add(tu);
			}
			_tuKeyIndex.Add(tu.Id, tu);
			IndexTu(tu, indexes);
		}

		public List<Tuple<Guid, int>> AddTus(Tuple<TranslationUnit, ImportType>[] batchTUs, FuzzyIndexes indexes, long tokenizationSignatureHash, int tmid)
		{
			throw new NotImplementedException();
		}

		private void IndexTu(TranslationUnit tu, FuzzyIndexes indexes)
		{
			if (_tuSourceHashIndex.TryGetValue(tu.Source.Hash, out List<TranslationUnit> value))
			{
				int tuIndex = GetTuIndex(value, tu.Id);
				value.Insert(~tuIndex, tu);
			}
			else
			{
				value = new List<TranslationUnit>();
				value.Add(tu);
				_tuSourceHashIndex.Add(tu.Source.Hash, value);
			}
			if ((indexes & FuzzyIndexes.SourceCharacterBased) != 0)
			{
				_fuzzyIndices[2].Add(tu.Id, tu.Source.ConcordanceFeatures);
			}
			if ((indexes & FuzzyIndexes.SourceWordBased) != 0)
			{
				_fuzzyIndices[1].Add(tu.Id, tu.Source.Features);
			}
			if ((indexes & FuzzyIndexes.TargetCharacterBased) != 0)
			{
				_fuzzyIndices[4].Add(tu.Id, tu.Target.ConcordanceFeatures);
			}
			if ((indexes & FuzzyIndexes.TargetWordBased) != 0)
			{
				_fuzzyIndices[8].Add(tu.Id, tu.Target.Features);
			}
		}

		private void UnindexTu(TranslationUnit tu, FuzzyIndexes indexes)
		{
			if (_tuSourceHashIndex.TryGetValue(tu.Source.Hash, out List<TranslationUnit> value))
			{
				value.Remove(tu);
			}
			if ((indexes & FuzzyIndexes.SourceCharacterBased) != 0)
			{
				_fuzzyIndices[2].Delete(tu.Id);
			}
			if ((indexes & FuzzyIndexes.SourceWordBased) != 0)
			{
				_fuzzyIndices[1].Delete(tu.Id);
			}
			if ((indexes & FuzzyIndexes.TargetCharacterBased) != 0)
			{
				_fuzzyIndices[4].Delete(tu.Id);
			}
			if ((indexes & FuzzyIndexes.TargetWordBased) != 0)
			{
				_fuzzyIndices[8].Delete(tu.Id);
			}
		}

		public List<TranslationUnit> GetTus(int tmId, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			if (_tUs == null || _tUs.Count == 0)
			{
				return list;
			}
			if (forward)
			{
				if (startAfter >= _tUs[_tUs.Count - 1].Id)
				{
					return list;
				}
			}
			else if (startAfter < _tUs[0].Id)
			{
				return list;
			}
			TranslationUnit translationUnit = new TranslationUnit(startAfter, Guid.Empty, tmId, null, null, DateTimeUtilities.Normalize(DateTime.MinValue), null, DateTimeUtilities.Normalize(DateTime.MinValue), null, DateTimeUtilities.Normalize(DateTime.MinValue), null, 0, 0, null, null, null, null, null, 0);
			int num = _tUs.BinarySearch(translationUnit, translationUnit);
			if (forward)
			{
				num = ((num < 0) ? (~num) : (num + 1));
				for (int i = num; i < _tUs.Count; i++)
				{
					if (list.Count >= count)
					{
						break;
					}
					if (_tUs[i] != null && _tUs[i].Id > startAfter && _tUs[i].TranslationMemoryId == tmId)
					{
						list.Add(_tUs[i]);
					}
				}
			}
			else
			{
				if (num < 0)
				{
					num = ~num - 1;
				}
				int num2 = num;
				while (num2 >= 0 && list.Count < count)
				{
					if (_tUs[num2] != null && _tUs[num2].Id <= startAfter && _tUs[num2].TranslationMemoryId == tmId)
					{
						list.Add(_tUs[num2]);
					}
					num2--;
				}
			}
			return list;
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
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			for (int i = 0; i < _tUs.Count; i++)
			{
				if (list.Count >= count)
				{
					break;
				}
				if (_tUs[i] != null && _tUs[i].Id > startAfter && _tUs[i].TranslationMemoryId == tmId)
				{
					list.Add(new PersistentObjectToken(_tUs[i].Id, Guid.Empty));
				}
			}
			return list;
		}

		private int GetTuIndex(int key)
		{
			return GetTuIndex(_tUs, key);
		}

		private int GetTuIndex(List<TranslationUnit> tus, int key)
		{
			TranslationUnit item = new TranslationUnit(key, Guid.Empty, 0, null, null, DateTimeUtilities.Normalize(DateTime.MinValue), null, DateTimeUtilities.Normalize(DateTime.MinValue), null, DateTimeUtilities.Normalize(DateTime.MinValue), null, 0, 0, null, null, null, null, null, 0);
			return tus.BinarySearch(item, _tuKeyComparer);
		}

		public TranslationUnit GetTu(int tmId, int key, bool idContextMatch)
		{
			if (!_tuKeyIndex.TryGetValue(key, out TranslationUnit value))
			{
				return null;
			}
			return value;
		}

		public bool DeleteTu(int tmId, PersistentObjectToken key, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			return DeleteTuAt(GetTuIndex(key.Id));
		}

		private bool DeleteTuAt(int idx)
		{
			if (idx < 0 || _tUs[idx] == null)
			{
				return false;
			}
			long hash = _tUs[idx].Source.Hash;
			if (_tuSourceHashIndex.ContainsKey(hash))
			{
				_tuSourceHashIndex[hash].Remove(_tUs[idx]);
			}
			InMemoryFuzzyIndex[] fuzzyIndices = _fuzzyIndices;
			for (int i = 0; i < fuzzyIndices.Length; i++)
			{
				fuzzyIndices[i]?.Delete(_tUs[idx].Id);
			}
			_tuKeyIndex.Remove(_tUs[idx].Id);
			_tUs.RemoveAt(idx);
			return true;
		}

		public List<PersistentObjectToken> DeleteTus(int tmId, List<PersistentObjectToken> keys, TextContextMatchType textContextMatchType, bool deleteOrphanContexts)
		{
			List<PersistentObjectToken> list = new List<PersistentObjectToken>();
			foreach (PersistentObjectToken key in keys)
			{
				if (DeleteTu(tmId, key, TextContextMatchType.PrecedingSourceAndTarget, deleteOrphanContexts))
				{
					list.Add(key);
				}
			}
			return list;
		}

		public int DeleteAllTus(int tmId)
		{
			int num = 0;
			for (int num2 = _tUs.Count - 1; num2 >= 0; num2--)
			{
				if (_tUs[num2] != null && _tUs[num2].TranslationMemoryId == tmId && DeleteTuAt(num2))
				{
					num++;
				}
			}
			return num;
		}

		public bool UpdateTuHeader(TranslationUnit tu, bool rewriteAttributes)
		{
			TranslationUnit tu2 = GetTu(0, tu.Id, idContextMatch: false);
			if (tu2 == null)
			{
				return false;
			}
			if (rewriteAttributes)
			{
				tu2.Attributes = tu.Attributes;
			}
			tu2.ChangeDate = DbStorageBase.Normalize(tu.ChangeDate);
			tu2.ChangeUser = tu.ChangeUser;
			tu2.CreationDate = DbStorageBase.Normalize(tu.CreationDate);
			tu2.CreationUser = tu.CreationUser;
			tu2.LastUsedDate = tu.LastUsedDate;
			tu2.LastUsedUser = tu.LastUsedUser;
			tu2.UsageCounter = tu.UsageCounter;
			return true;
		}

		private void UpdateTuIndices(TranslationUnit tu, FuzzyIndexes indexes)
		{
			TranslationUnit tu2 = GetTu(0, tu.Id, idContextMatch: false);
			if (tu2 != null)
			{
				UnindexTu(tu2, indexes);
				tu2.Source = tu.Source;
				tu2.Target = tu.Target;
				IndexTu(tu2, indexes);
			}
		}

		public void UpdateTuIndices(List<TranslationUnit> tus, FuzzyIndexes indexes, long tokenizationSignatureHash, TextContextMatchType textContextMatchType)
		{
			foreach (TranslationUnit tu in tus)
			{
				UpdateTuIndices(tu, indexes);
			}
		}

		public void ClearFuzzyIndex(FuzzyIndexes index)
		{
			throw new NotImplementedException();
		}

		public TuContexts GetTextContexts(int tmId, int tuId)
		{
			return GetTu(tmId, tuId, idContextMatch: false)?.Contexts;
		}

		public void AddContexts(int tmId, int tuId, TuContexts contexts)
		{
			TranslationUnit tu = GetTu(tmId, tuId, idContextMatch: false);
			if (tu != null)
			{
				if (tu.Contexts == null)
				{
					tu.Contexts = contexts;
				}
				else
				{
					tu.Contexts.Merge(contexts);
				}
			}
		}

		public void AddIdContexts(int tmId, int tuId, TuIdContexts contexts)
		{
			throw new NotImplementedException();
		}

		public void DeleteContexts(int tmId, int tuId)
		{
			GetTu(tmId, tuId, idContextMatch: false)?.Contexts.Clear();
		}

		public List<TranslationUnit> FuzzySearch(int tmId, List<int> features, FuzzyIndexes index, int minScore, int maxHits, bool concordance, int lastTuId, TuContextData tuContextData, bool descendingOrder)
		{
			List<TranslationUnit> list = new List<TranslationUnit>();
			if (features == null || features.Count == 0)
			{
				return new List<TranslationUnit>();
			}
			ScoringMethod scoringMethod = (!concordance) ? ScoringMethod.Dice : ScoringMethod.Query;
			List<Hit> list2 = _fuzzyIndices[(int)index].Search(features, maxHits, minScore, lastTuId, scoringMethod, delegate(int key)
			{
				TranslationUnit tu = GetTu(tmId, key, idContextMatch: false);
				return tu != null && tu.TranslationMemoryId == tmId;
			}, descendingOrder);
			if (list2 != null && list2.Count > 0)
			{
				list.AddRange(list2.Select((Hit h) => GetTu(tmId, h.Key, idContextMatch: false)));
			}
			return list;
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
			List<TranslationUnit> list = new List<TranslationUnit>();
			if (!_tuSourceHashIndex.TryGetValue(sourceHash, out List<TranslationUnit> value))
			{
				return list;
			}
			if (value.Count == 0)
			{
				return list;
			}
			for (int i = 0; i < value.Count; i++)
			{
				if (list.Count >= maxHits)
				{
					break;
				}
				if (value[i].TranslationMemoryId == tmId && !(value[i].ChangeDate <= lastChangeDate) && (targetHash == 0L || value[i].Target.Hash == targetHash))
				{
					list.Add(value[i]);
				}
			}
			return list;
		}

		public List<TranslationUnit> DuplicateSearch(int tmId, long lastHash, int lastTuId, int count, bool forward, bool targetSegments)
		{
			throw new NotImplementedException();
		}

		public void RecomputeFrequencies(int tmId)
		{
		}

		public void Dispose()
		{
		}

		public void CollectAndDumpFeatureFrequencies(string path)
		{
			try
			{
				SortedDictionary<int, int> sortedDictionary = new SortedDictionary<int, int>();
				foreach (TranslationUnit tU in _tUs)
				{
					if (tU != null)
					{
						foreach (int feature in tU.Source.Features)
						{
							if (sortedDictionary.ContainsKey(feature))
							{
								sortedDictionary[feature]++;
							}
							else
							{
								sortedDictionary[feature] = 1;
							}
						}
					}
				}
				using (TextWriter textWriter = File.CreateText(path))
				{
					foreach (KeyValuePair<int, int> item in sortedDictionary)
					{
						if (item.Value > 10)
						{
							textWriter.WriteLine("{0},{1}", item.Key, item.Value);
						}
					}
				}
			}
			catch
			{
			}
		}

		public List<TranslationUnit> ExactSearch(int tmId, List<long> sourceHashes, FilterExpression hardFilter)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> SubsegmentSearch(int tmId, List<long> fragmentHashes, byte minFragmentLength, byte minSigWords, int maxHits, Dictionary<int, HashSet<long>> hashesPerTu)
		{
			throw new NotImplementedException();
		}

		public List<TranslationUnit> GetTusFiltered(int tmId, FilterExpression filter, int startAfter, int count, bool forward, bool idContextMatch, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			throw new NotImplementedException();
		}

		public bool DeleteTmSchema()
		{
			return true;
		}

		public bool DeleteTmSchema(int key)
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
