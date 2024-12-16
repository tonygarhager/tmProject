using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi;
using Sdl.Core.Bcm.BcmModel.Collections;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	internal sealed class ContextTable
	{
		private int _contextInfoTableId;

		private int _structureInfoTableId;

		private IContextProperties _lastContextProperties;

		private ContextPropertiesItem _lastCtxPropsItem;

		private readonly HashDictionaryInt<IContextInfo, ContextInfoPair> _contextInfos;

		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Objects,
			NullValueHandling = NullValueHandling.Ignore,
			PreserveReferencesHandling = PreserveReferencesHandling.Objects
		};

		public List<ContextInfoPair> ContextInfoTable
		{
			get;
			set;
		}

		public List<StructureInfoPair> StructureInfoTable
		{
			get;
			set;
		}

		public Dictionary<string, ContextPropertiesItem> ContextMapping
		{
			get;
			set;
		}

		public ContextTable()
		{
			ContextInfoTable = new List<ContextInfoPair>();
			StructureInfoTable = new List<StructureInfoPair>();
			ContextMapping = new Dictionary<string, ContextPropertiesItem>();
			_contextInfos = new HashDictionaryInt<IContextInfo, ContextInfoPair>();
		}

		public void AddContextProperties(string paragraphUnitId, IContextProperties contextProperties)
		{
			if (_lastContextProperties == contextProperties)
			{
				ContextMapping[paragraphUnitId] = _lastCtxPropsItem;
				return;
			}
			_lastContextProperties = contextProperties;
			if (contextProperties == null)
			{
				_lastCtxPropsItem = null;
				ContextMapping[paragraphUnitId] = null;
				return;
			}
			List<int> contextInfoIds = AddContexts(contextProperties.Contexts);
			int? structureInfoId = AddStructureInfo(contextProperties.StructureInfo);
			ContextPropertiesItem value = _lastCtxPropsItem = new ContextPropertiesItem
			{
				ContextInfoIds = contextInfoIds,
				StructureInfoId = structureInfoId
			};
			ContextMapping.Add(paragraphUnitId, value);
		}

		private List<int> AddContexts(IEnumerable<IContextInfo> contexts)
		{
			List<int> list = new List<int>();
			foreach (IContextInfo context in contexts)
			{
				int num = 1;
				ContextInfo contextInfo = context as ContextInfo;
				if (!_contextInfos.TryGetValue(contextInfo, out ContextInfoPair value))
				{
					ContextInfoPair contextInfoPair = new ContextInfoPair
					{
						ContextInfo = contextInfo,
						Id = ++_contextInfoTableId
					};
					ContextInfoTable.Add(contextInfoPair);
					_contextInfos.Add(contextInfo, contextInfoPair);
					num = _contextInfoTableId;
				}
				else
				{
					num = value.Id;
				}
				list.Add(num);
			}
			return list;
		}

		private int? AddStructureInfo(IStructureInfo structureInfo)
		{
			if (structureInfo == null)
			{
				return null;
			}
			StructureInfo newStructureInfo = structureInfo.Clone() as StructureInfo;
			newStructureInfo.ParentStructure = null;
			StructureInfoPair structureInfoPair = StructureInfoTable.FirstOrDefault((StructureInfoPair x) => newStructureInfo.Id == x.StructureInfoWithParent.StructureInfo.Id);
			int value;
			if (structureInfoPair == null)
			{
				StructureInfoPair structureInfoPair2 = new StructureInfoPair
				{
					StructureInfoWithParent = new StructureInfoWithParent
					{
						StructureInfo = newStructureInfo
					}
				};
				if (structureInfo.ParentStructure != null)
				{
					int? num = AddStructureInfo(structureInfo.ParentStructure);
					if (num.HasValue)
					{
						structureInfoPair2.StructureInfoWithParent.ParentId = num.Value;
					}
				}
				structureInfoPair2.Id = ++_structureInfoTableId;
				StructureInfoTable.Add(structureInfoPair2);
				value = _structureInfoTableId;
			}
			else
			{
				value = structureInfoPair.Id;
			}
			return value;
		}

		public Tuple<ContextPropertiesItem, IContextProperties> GetContextProperties(string punitId, IPropertiesFactory propertiesFactory, ref ContextPropertiesItem lastCtxInfoItem, out bool changeContext)
		{
			changeContext = false;
			IContextProperties contextProperties = propertiesFactory.CreateContextProperties();
			if (!ContextMapping.ContainsKey(punitId))
			{
				return null;
			}
			ContextPropertiesItem contextPropertiesItem = ContextMapping[punitId];
			if (lastCtxInfoItem != null && lastCtxInfoItem == contextPropertiesItem)
			{
				return null;
			}
			changeContext = true;
			if (contextPropertiesItem == null)
			{
				return null;
			}
			foreach (int contextInfoId in contextPropertiesItem.ContextInfoIds)
			{
				ContextInfoPair item = new ContextInfoPair
				{
					Id = contextInfoId
				};
				int index = ContextInfoTable.BinarySearch(item, new ContextInfoPairIdComparer());
				contextProperties.Contexts.Add(ContextInfoTable.ElementAt(index).ContextInfo.Clone() as IContextInfo);
			}
			if (contextPropertiesItem.StructureInfoId.HasValue)
			{
				StructureInfoPair item2 = new StructureInfoPair
				{
					Id = contextPropertiesItem.StructureInfoId.Value
				};
				int index = StructureInfoTable.BinarySearch(item2, new StructureInfoPairIdComparer());
				if (index >= 0)
				{
					contextProperties.StructureInfo = (StructureInfoTable.ElementAt(index).StructureInfoWithParent.StructureInfo.Clone() as IStructureInfo);
				}
			}
			return new Tuple<ContextPropertiesItem, IContextProperties>(contextPropertiesItem, contextProperties);
		}

		public string SerializeFullContextsToFile()
		{
			string tempFileName = Path.GetTempFileName();
			using (FileStream stream = new FileStream(tempFileName, FileMode.Open))
			{
				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Compress))
				{
					string s = JsonConvert.SerializeObject(this, JsonSerializerSettings);
					byte[] bytes = Encoding.UTF8.GetBytes(s);
					deflateStream.Write(bytes, 0, bytes.Length);
					return tempFileName;
				}
			}
		}

		public static ContextTable DeserializeFullContexts(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			using (FileStream stream = new FileStream(filePath, FileMode.Open))
			{
				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
				{
					int num;
					do
					{
						byte[] array = new byte[4096];
						num = deflateStream.Read(array, 0, 4096);
						stringBuilder.Append(Encoding.UTF8.GetString(array));
					}
					while (num > 0);
				}
			}
			ContextTable contextTable = JsonConvert.DeserializeObject<ContextTable>(stringBuilder.ToString(), JsonSerializerSettings);
			foreach (StructureInfoPair item in contextTable.StructureInfoTable)
			{
				int parentId = item.StructureInfoWithParent.ParentId;
				StructureInfo structureInfo = item.StructureInfoWithParent.StructureInfo;
				if (parentId > 0 && structureInfo.ParentStructure == null)
				{
					StructureInfoPair structureInfoPair = contextTable.StructureInfoTable.First((StructureInfoPair x) => x.Id == parentId);
					structureInfo.ParentStructure = structureInfoPair.StructureInfoWithParent.StructureInfo;
				}
			}
			return contextTable;
		}
	}
}
