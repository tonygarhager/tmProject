using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Collections
{
	public class HashDictionaryInt<TKey, TValue>
	{
		public Dictionary<int, List<KeyValuePair<TKey, TValue>>> _backingDictionary;

		public TValue this[TKey key]
		{
			get
			{
				int hashCodeInternal = GetHashCodeInternal(key);
				if (_backingDictionary.TryGetValue(hashCodeInternal, out List<KeyValuePair<TKey, TValue>> value))
				{
					if (value.Count > 1)
					{
						List<KeyValuePair<TKey, TValue>> list = value.FindAll((KeyValuePair<TKey, TValue> kv) => kv.Key.Equals(key));
						if (list.Count == 1)
						{
							return list.First().Value;
						}
						if (list.Count > 1)
						{
							throw new InvalidOperationException("Multiple matches found in collection");
						}
					}
					else if (value.First().Key.Equals(key))
					{
						return value.First().Value;
					}
				}
				throw new InvalidOperationException("Key not found in collection");
			}
		}

		public HashDictionaryInt()
		{
			_backingDictionary = new Dictionary<int, List<KeyValuePair<TKey, TValue>>>();
		}

		public void Add(TKey key, TValue value)
		{
			int hashCodeInternal = GetHashCodeInternal(key);
			if (_backingDictionary.TryGetValue(hashCodeInternal, out List<KeyValuePair<TKey, TValue>> value2))
			{
				if (value2.All((KeyValuePair<TKey, TValue> kv) => kv.Key.Equals(key)))
				{
					throw new InvalidOperationException("Item with same key already exists in collection");
				}
				value2.Add(new KeyValuePair<TKey, TValue>(key, value));
			}
			else
			{
				List<KeyValuePair<TKey, TValue>> value3 = new List<KeyValuePair<TKey, TValue>>
				{
					new KeyValuePair<TKey, TValue>(key, value)
				};
				_backingDictionary.Add(hashCodeInternal, value3);
			}
		}

		public bool ContainsKey(TKey key)
		{
			int hashCodeInternal = GetHashCodeInternal(key);
			if (_backingDictionary.TryGetValue(hashCodeInternal, out List<KeyValuePair<TKey, TValue>> value))
			{
				if (value.Count > 1)
				{
					List<KeyValuePair<TKey, TValue>> list = value.FindAll((KeyValuePair<TKey, TValue> kv) => kv.Key.Equals(key));
					if (list.Count == 1)
					{
						return true;
					}
					if (list.Count > 1)
					{
						throw new InvalidOperationException("Multiple matches found in collection");
					}
				}
				else if (value.First().Key.Equals(key))
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			_backingDictionary.Clear();
		}

		public bool Remove(TKey key)
		{
			int hashCodeInternal = GetHashCodeInternal(key);
			if (_backingDictionary.TryGetValue(hashCodeInternal, out List<KeyValuePair<TKey, TValue>> value))
			{
				if (value.Count > 1)
				{
					List<KeyValuePair<TKey, TValue>> list = value.FindAll((KeyValuePair<TKey, TValue> kv) => kv.Key.Equals(key));
					if (list.Count == 1)
					{
						_backingDictionary[hashCodeInternal].Remove(list.First());
					}
					if (list.Count > 1)
					{
						throw new InvalidOperationException("Multiple matches found in collection");
					}
				}
				else if (value.First().Key.Equals(key))
				{
					_backingDictionary.Remove(hashCodeInternal);
				}
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (ContainsKey(key))
			{
				value = this[key];
				return true;
			}
			value = default(TValue);
			return false;
		}

		private int GetHashCodeInternal(TKey key)
		{
			int num = key.GetHashCode();
			IContextInfo contextInfo = key as IContextInfo;
			if (contextInfo != null)
			{
				int metadataKeyValueHashCode = GetMetadataKeyValueHashCode(contextInfo);
				num = num * 397 + metadataKeyValueHashCode;
			}
			return num;
		}

		private int GetMetadataKeyValueHashCode(IContextInfo info)
		{
			int num = 0;
			foreach (KeyValuePair<string, string> metaDatum in info.MetaData)
			{
				num += metaDatum.Key.GetHashCode() * 397 + metaDatum.Value.GetHashCode() * 397;
			}
			return num;
		}
	}
}
