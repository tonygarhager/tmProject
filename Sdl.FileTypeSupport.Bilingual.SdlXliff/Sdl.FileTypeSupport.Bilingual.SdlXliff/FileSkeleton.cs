using Murmur;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	[Serializable]
	public class FileSkeleton
	{
		public enum TagIdSearchType
		{
			SearchAllTags,
			SearchStartTags,
			SearchPlaceholderTags,
			SearchStructureTags
		}

		private Dictionary<TagId, IStartTagProperties> _StartTags = new Dictionary<TagId, IStartTagProperties>();

		private Dictionary<TagId, IEndTagProperties> _EndTags = new Dictionary<TagId, IEndTagProperties>();

		private Dictionary<TagId, IList<ISubSegmentReference>> _SubSegments = new Dictionary<TagId, IList<ISubSegmentReference>>();

		private Dictionary<TagId, IPlaceholderTagProperties> _PlaceholderTags = new Dictionary<TagId, IPlaceholderTagProperties>();

		private Dictionary<TagId, IStructureTagProperties> _StructureTags = new Dictionary<TagId, IStructureTagProperties>();

		private Dictionary<IFormattingGroup, string> _FormattingDefinitions = new Dictionary<IFormattingGroup, string>();

		private Dictionary<string, IFormattingGroup> _FormattingById = new Dictionary<string, IFormattingGroup>();

		private int _LastUsedFormattingDefinitionId;

		private Dictionary<long, string> _ContextDefinitions = new Dictionary<long, string>();

		private Dictionary<string, string> _StructureInfoDefinitions = new Dictionary<string, string>();

		private Dictionary<string, IContextInfo> _ContextsById = new Dictionary<string, IContextInfo>();

		private Dictionary<string, IStructureInfo> _StructureInfosById = new Dictionary<string, IStructureInfo>();

		private List<List<IParagraphUnit>> _puStore = new List<List<IParagraphUnit>>();

		private IFileProperties _fileProperties;

		private int _LastUsedContextDefinitionId;

		private int _LastUsedStructureInfoDefinitionId;

		private IStructureInfo _LastStoredStructureInfo;

		public IFileProperties FileProperties
		{
			get
			{
				return _fileProperties;
			}
			set
			{
				_fileProperties = value;
			}
		}

		public List<List<IParagraphUnit>> PuStore => _puStore;

		public List<IParagraphUnit> CurrentPuStore => _puStore[_puStore.Count - 1];

		public bool HasTags
		{
			get
			{
				if (_StartTags.Count > 0 || _PlaceholderTags.Count > 0 || _StructureTags.Count > 0)
				{
					return true;
				}
				return false;
			}
		}

		public bool HasContextDefinitions
		{
			get
			{
				if (_ContextsById.Count > 0)
				{
					return true;
				}
				return false;
			}
		}

		public bool HasStructureDefinitions
		{
			get
			{
				if (_StructureInfoDefinitions.Count > 0)
				{
					return true;
				}
				return false;
			}
		}

		public bool HasFormattingDefinitions
		{
			get
			{
				if (_FormattingDefinitions.Count > 0)
				{
					return true;
				}
				return false;
			}
		}

		public IEnumerable<TagId> PairedTagIds
		{
			get
			{
				foreach (TagId key in _StartTags.Keys)
				{
					yield return key;
				}
			}
		}

		public IEnumerable<TagId> PlaceholderTagIds
		{
			get
			{
				foreach (TagId key in _PlaceholderTags.Keys)
				{
					yield return key;
				}
			}
		}

		public IEnumerable<TagId> StructureTagIds
		{
			get
			{
				foreach (TagId key in _StructureTags.Keys)
				{
					yield return key;
				}
			}
		}

		public IEnumerable<KeyValuePair<string, IContextInfo>> ContextDefinitionsById => _ContextsById;

		public IStructureInfo LastStoredStructureInfo => _LastStoredStructureInfo;

		public IEnumerable<KeyValuePair<string, IStructureInfo>> StructureDefinitionsById => _StructureInfosById;

		public IEnumerable<KeyValuePair<string, IFormattingGroup>> FormattingDefinitionsById => _FormattingById;

		public bool HasTagWithId(TagId id, TagIdSearchType searchType)
		{
			if ((searchType == TagIdSearchType.SearchAllTags || searchType == TagIdSearchType.SearchStartTags) && _StartTags.ContainsKey(id))
			{
				return true;
			}
			if ((searchType == TagIdSearchType.SearchAllTags || searchType == TagIdSearchType.SearchPlaceholderTags) && _PlaceholderTags.ContainsKey(id))
			{
				return true;
			}
			if ((searchType == TagIdSearchType.SearchAllTags || searchType == TagIdSearchType.SearchStructureTags) && _StructureTags.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public void AddTagPair(ITagPair tagPair)
		{
			_StartTags[tagPair.StartTagProperties.TagId] = tagPair.StartTagProperties;
			_EndTags[tagPair.StartTagProperties.TagId] = tagPair.EndTagProperties;
			if (tagPair.HasSubSegmentReferences)
			{
				_SubSegments[tagPair.StartTagProperties.TagId] = tagPair.SubSegments.ToList();
			}
			if (tagPair.StartTagProperties.Formatting != null)
			{
				StoreFormatting(tagPair.StartTagProperties.Formatting);
			}
		}

		public void AddPlaceholderTag(IPlaceholderTag placeholder)
		{
			_PlaceholderTags[placeholder.Properties.TagId] = placeholder.Properties;
			if (placeholder.HasSubSegmentReferences)
			{
				_SubSegments[placeholder.Properties.TagId] = placeholder.SubSegments.ToList();
			}
		}

		public void AddStructureTag(IStructureTag tag)
		{
			_StructureTags[tag.Properties.TagId] = tag.Properties;
			if (tag.HasSubSegmentReferences)
			{
				_SubSegments[tag.Properties.TagId] = tag.SubSegments.ToList();
			}
		}

		public IPlaceholderTagProperties GetPlaceholderTagProperties(TagId id)
		{
			if (_PlaceholderTags.ContainsKey(id))
			{
				return _PlaceholderTags[id];
			}
			return null;
		}

		public IStructureTagProperties GetStructureTagProperties(TagId id)
		{
			if (_StructureTags.ContainsKey(id))
			{
				return _StructureTags[id];
			}
			return null;
		}

		public IStartTagProperties GetStartTagProperties(TagId id)
		{
			if (_StartTags.ContainsKey(id))
			{
				return _StartTags[id];
			}
			return null;
		}

		public IEndTagProperties GetEndTagProperties(TagId id)
		{
			if (_EndTags.ContainsKey(id))
			{
				return _EndTags[id];
			}
			return null;
		}

		public IList<ISubSegmentReference> GetSubSegments(TagId id)
		{
			if (_SubSegments.ContainsKey(id))
			{
				return _SubSegments[id];
			}
			return null;
		}

		public bool HasContextDefinitionWithId(string id)
		{
			if (_ContextsById.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public void AddContextDefinition(string id, IContextInfo contextDefinition)
		{
			_ContextsById.Add(id, contextDefinition);
		}

		public string StoreContext(IContextInfo definition)
		{
			long objectHash = GetObjectHash(definition);
			if (_ContextDefinitions.TryGetValue(objectHash, out string value))
			{
				return value;
			}
			value = (++_LastUsedContextDefinitionId).ToString("d", CultureInfo.InvariantCulture);
			IContextInfo contextInfo = (IContextInfo)definition.Clone();
			_ContextDefinitions.Add(objectHash, value);
			_ContextsById.Add(value, contextInfo);
			if (contextInfo.DefaultFormatting != null)
			{
				StoreFormatting(contextInfo.DefaultFormatting);
			}
			return value;
		}

		private long GetObjectHash(object obj)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(memoryStream, obj);
				byte[] buffer = memoryStream.ToArray();
				byte[] bytes = ((HashAlgorithm)(object)MurmurHash.Create128(0u, false, (AlgorithmPreference)0)).ComputeHash(buffer);
				return ConvertByteArray(bytes);
			}
		}

		private long ConvertByteArray(byte[] bytes)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToInt64(bytes, 0);
		}

		public IContextInfo GetContext(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			if (_ContextsById.ContainsKey(id))
			{
				return _ContextsById[id];
			}
			return null;
		}

		public bool HasStructureDefinitionWithId(string id)
		{
			if (_StructureInfosById.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public void AddStructureDefinition(string id, IStructureInfo structureDefinition)
		{
			_StructureInfosById.Add(id, structureDefinition);
		}

		public string StoreStructure(IStructureInfo definition)
		{
			if (_StructureInfoDefinitions.TryGetValue(definition.Id, out string value))
			{
				return value;
			}
			value = (++_LastUsedStructureInfoDefinitionId).ToString("d", CultureInfo.InvariantCulture);
			AddParentStructures(definition, ref value);
			_StructureInfoDefinitions.Add(definition.Id, value);
			_StructureInfosById.Add(value, definition);
			_LastStoredStructureInfo = definition;
			if (definition.ContextInfo != null)
			{
				StoreContext(definition.ContextInfo);
			}
			return value;
		}

		private void AddParentStructures(IStructureInfo structureInfo, ref string id)
		{
			while (structureInfo.ParentStructure != null)
			{
				if (!_StructureInfoDefinitions.TryGetValue(structureInfo.ParentStructure.Id, out string value))
				{
					_StructureInfoDefinitions.Add(structureInfo.ParentStructure.Id, id);
				}
				else if (!string.IsNullOrEmpty(value))
				{
					structureInfo = structureInfo.ParentStructure;
					continue;
				}
				if (!_StructureInfosById.ContainsKey(id))
				{
					_StructureInfosById.Add(id, structureInfo.ParentStructure);
				}
				if (structureInfo.ParentStructure.ContextInfo != null)
				{
					StoreContext(structureInfo.ParentStructure.ContextInfo);
				}
				id = (++_LastUsedStructureInfoDefinitionId).ToString("d", CultureInfo.InvariantCulture);
				structureInfo = structureInfo.ParentStructure;
			}
		}

		public IStructureInfo GetStructure(string id)
		{
			if (_StructureInfosById.ContainsKey(id))
			{
				return _StructureInfosById[id];
			}
			return null;
		}

		public string GetStructureId(IStructureInfo structureInfo)
		{
			if (structureInfo != null && _StructureInfoDefinitions.ContainsKey(structureInfo.Id))
			{
				return _StructureInfoDefinitions[structureInfo.Id];
			}
			return null;
		}

		public bool HasFormattingDefinitionWithId(string id)
		{
			if (_FormattingById.ContainsKey(id))
			{
				return true;
			}
			return false;
		}

		public void AddFormattingDefinition(string id, IFormattingGroup formatDefinition)
		{
			_FormattingDefinitions.Add(formatDefinition, id);
			_FormattingById.Add(id, formatDefinition);
		}

		public string StoreFormatting(IFormattingGroup definition)
		{
			if (_FormattingDefinitions.TryGetValue(definition, out string value))
			{
				return value;
			}
			value = (++_LastUsedFormattingDefinitionId).ToString("d", CultureInfo.InvariantCulture);
			IFormattingGroup formattingGroup = (IFormattingGroup)definition.Clone();
			_FormattingDefinitions.Add(formattingGroup, value);
			_FormattingById.Add(value, formattingGroup);
			return value;
		}

		public IFormattingGroup GetFormatting(string id)
		{
			if (_FormattingById.ContainsKey(id))
			{
				return _FormattingById[id];
			}
			return null;
		}
	}
}
