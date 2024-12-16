using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public abstract class AbstractMetaDataContainer : IMetaDataContainer
	{
		protected Dictionary<string, string> _MetaData = new Dictionary<string, string>();

		public bool HasMetaData => _MetaData.Count > 0;

		public IEnumerable<KeyValuePair<string, string>> MetaData => _MetaData;

		public int MetaDataCount => _MetaData.Count;

		protected AbstractMetaDataContainer()
		{
		}

		protected AbstractMetaDataContainer(AbstractMetaDataContainer other)
		{
			ReplaceMetaDataWithCloneOf(other._MetaData);
		}

		public void ReplaceMetaDataWithCloneOf(IEnumerable<KeyValuePair<string, string>> toClone)
		{
			_MetaData.Clear();
			foreach (KeyValuePair<string, string> item in toClone)
			{
				_MetaData.Add(item.Key, item.Value);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AbstractMetaDataContainer abstractMetaDataContainer = (AbstractMetaDataContainer)obj;
			if (_MetaData.Count != abstractMetaDataContainer._MetaData.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, string> metaDatum in _MetaData)
			{
				if (!abstractMetaDataContainer._MetaData.TryGetValue(metaDatum.Key, out string value))
				{
					return false;
				}
				if (value == null != (metaDatum.Value == null))
				{
					return false;
				}
				if (value != null && !value.Equals(metaDatum.Value))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (KeyValuePair<string, string> metaDatum in _MetaData)
			{
				num ^= ((metaDatum.Key.GetHashCode() << 16) ^ metaDatum.Value.GetHashCode());
			}
			return num;
		}

		public override string ToString()
		{
			return _MetaData.ToString();
		}

		public void ClearMetaData()
		{
			_MetaData.Clear();
		}

		public string GetMetaData(string key)
		{
			if (_MetaData.TryGetValue(key, out string value))
			{
				return value;
			}
			return null;
		}

		public bool MetaDataContainsKey(string key)
		{
			return _MetaData.ContainsKey(key);
		}

		public bool RemoveMetaData(string key)
		{
			return _MetaData.Remove(key);
		}

		public void SetMetaData(string key, string value)
		{
			_MetaData[key] = value;
		}
	}
}
