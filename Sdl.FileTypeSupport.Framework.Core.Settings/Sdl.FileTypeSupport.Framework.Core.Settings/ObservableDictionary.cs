using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	[Serializable]
	public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged, ISerializable
	{
		private readonly object _lock = new object();

		private readonly IDictionary<TKey, TValue> _dictionary;

		private const string SettingKey = "Key";

		private const string SettingValue = "Value";

		public ICollection<TKey> Keys
		{
			get
			{
				lock (_lock)
				{
					return new List<TKey>(_dictionary.Keys);
				}
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				lock (_lock)
				{
					return new List<TValue>(_dictionary.Values);
				}
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				lock (_lock)
				{
					if (_dictionary.ContainsKey(key))
					{
						return _dictionary[key];
					}
				}
				return default(TValue);
			}
			set
			{
				lock (_lock)
				{
					if (_dictionary.ContainsKey(key))
					{
						_dictionary[key] = value;
					}
					else
					{
						_dictionary.Add(key, value);
					}
					OnCollectionChanged();
				}
			}
		}

		public int Count => _dictionary.Count;

		public bool IsReadOnly => _dictionary.IsReadOnly;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableDictionary()
			: this(5)
		{
		}

		public ObservableDictionary(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			_dictionary = new Dictionary<TKey, TValue>(size);
		}

		public void Add(TKey key, TValue value)
		{
			lock (_lock)
			{
				if (!_dictionary.ContainsKey(key))
				{
					_dictionary.Add(key, value);
					OnCollectionChanged();
				}
				else
				{
					_dictionary[key] = value;
					OnCollectionChanged();
				}
			}
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			lock (_lock)
			{
				if (_dictionary.ContainsKey(key))
				{
					bool result = _dictionary.Remove(key);
					OnCollectionChanged();
					return result;
				}
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			lock (_lock)
			{
				if (_dictionary.ContainsKey(key))
				{
					value = _dictionary[key];
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			lock (_lock)
			{
				_dictionary.Add(item);
			}
			OnCollectionChanged();
		}

		public void Clear()
		{
			lock (_lock)
			{
				_dictionary.Clear();
			}
			OnCollectionChanged();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			lock (_lock)
			{
				_dictionary.CopyTo(array, arrayIndex);
			}
			OnCollectionChanged();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			lock (_lock)
			{
				bool result = _dictionary.Remove(item);
				OnCollectionChanged();
				return result;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
		}

		private void OnCollectionChanged()
		{
			OnNotifyPropertyChanged("Count");
			OnNotifyPropertyChanged("Item[]");
			OnNotifyPropertyChanged("Keys");
			OnNotifyPropertyChanged("Values");
			OnNotifyCollectionChanged();
		}

		private void OnNotifyCollectionChanged()
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		private void OnNotifyPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected ObservableDictionary(SerializationInfo info, StreamingContext context)
			: this()
		{
			int @int = info.GetInt32("ItemCount");
			for (int i = 0; i < @int; i++)
			{
				KeyValuePair<TKey, TValue> keyValuePair = (KeyValuePair<TKey, TValue>)info.GetValue($"Item{i}", typeof(KeyValuePair<TKey, TValue>));
				Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ItemCount", Count);
			int num = 0;
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					info.AddValue($"Item{num}", current, typeof(KeyValuePair<TKey, TValue>));
					num++;
				}
			}
		}

		public virtual void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listSettingId)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			ClearDictionaryItemSettings(settingsGroup, listSettingId);
			settingsGroup.GetSetting<bool>(listSettingId).Value = true;
			int num = 0;
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					string id = listSettingId + "Key" + num.ToString();
					string id2 = listSettingId + "Value" + num.ToString();
					settingsGroup.GetSetting<TKey>(id).Value = current.Key;
					settingsGroup.GetSetting<TValue>(id2).Value = current.Value;
					num++;
				}
			}
		}

		public virtual void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listSettingId)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			int num = 0;
			bool flag;
			do
			{
				flag = false;
				bool found = false;
				bool found2 = false;
				string listItemSetting = listSettingId + "Key" + num.ToString();
				string listItemSetting2 = listSettingId + "Value" + num.ToString();
				TKey dictionaryItemFromSettings = GetDictionaryItemFromSettings<TKey>(settingsGroup, listItemSetting, out found);
				TValue dictionaryItemFromSettings2 = GetDictionaryItemFromSettings<TValue>(settingsGroup, listItemSetting2, out found2);
				if (found && found2)
				{
					flag = true;
					Add(dictionaryItemFromSettings, dictionaryItemFromSettings2);
				}
				num++;
			}
			while (flag);
		}

		protected virtual void ClearDictionaryItemSettings(ISettingsGroup settingsGroup, string listSettingId)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			int num = 0;
			if (settingsGroup.ContainsSetting(listSettingId))
			{
				settingsGroup.RemoveSetting(listSettingId);
			}
			bool flag;
			do
			{
				flag = false;
				string settingId = listSettingId + "Key" + num.ToString();
				string settingId2 = listSettingId + "Value" + num.ToString();
				if (settingsGroup.ContainsSetting(settingId))
				{
					settingsGroup.RemoveSetting(settingId);
					flag = true;
				}
				if (settingsGroup.ContainsSetting(settingId2))
				{
					settingsGroup.RemoveSetting(settingId2);
					flag = true;
				}
				num++;
			}
			while (flag);
		}

		protected virtual T GetDictionaryItemFromSettings<T>(ISettingsGroup settingsGroup, string listItemSetting, out bool found)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			T result = default(T);
			found = false;
			if (settingsGroup.ContainsSetting(listItemSetting))
			{
				found = true;
				return settingsGroup.GetSetting<T>(listItemSetting);
			}
			return result;
		}
	}
}
