using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public class ObservableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly object _lock = new object();

		private readonly List<T> _list;

		public T this[int index]
		{
			get
			{
				lock (_lock)
				{
					return _list[index];
				}
			}
			set
			{
				lock (_lock)
				{
					_list[index] = value;
					OnCollectionChanged();
				}
			}
		}

		public int Count => _list.Count;

		public bool IsReadOnly => false;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public static ObservableList<string> CreateObservableStringList(string[] strings)
		{
			ObservableList<string> observableList = new ObservableList<string>();
			observableList.AddRange(strings);
			return observableList;
		}

		public ObservableList()
		{
			_list = new List<T>();
		}

		public ObservableList(IEnumerable<T> collection)
			: this()
		{
			AddRange(collection);
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			lock (_lock)
			{
				_list.InsertRange(index, collection);
				OnCollectionChanged();
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			lock (_lock)
			{
				_list.AddRange(collection);
				OnCollectionChanged();
			}
		}

		public T[] ToArray()
		{
			lock (_lock)
			{
				return _list.ToArray();
			}
		}

		public void Sort()
		{
			lock (_lock)
			{
				_list.Sort();
			}
		}

		public void Sort(IComparer<T> comparer)
		{
			lock (_lock)
			{
				_list.Sort(comparer);
			}
		}

		public void Sort(Comparison<T> comparison)
		{
			lock (_lock)
			{
				_list.Sort(comparison);
			}
		}

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			lock (_lock)
			{
				_list.Insert(index, item);
				OnCollectionChanged();
			}
		}

		public T Find(Predicate<T> match)
		{
			foreach (T item in _list)
			{
				if (match(item))
				{
					return item;
				}
			}
			return default(T);
		}

		public void RemoveAt(int index)
		{
			lock (_lock)
			{
				_list.RemoveAt(index);
				OnCollectionChanged();
			}
		}

		public void Add(T item)
		{
			lock (_lock)
			{
				_list.Add(item);
				OnCollectionChanged();
			}
		}

		public void Clear()
		{
			lock (_lock)
			{
				_list.Clear();
				OnCollectionChanged();
			}
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			lock (_lock)
			{
				_list.CopyTo(array, arrayIndex);
			}
		}

		public bool Remove(T item)
		{
			lock (_lock)
			{
				bool result = _list.Remove(item);
				OnCollectionChanged();
				return result;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_list).GetEnumerator();
		}

		private void OnCollectionChanged()
		{
			OnNotifyPropertyChanged("Count");
			OnNotifyPropertyChanged("Item[]");
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

		public virtual void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listSettingId)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			ClearListItemSettings(settingsGroup, listSettingId);
			settingsGroup.GetSetting<bool>(listSettingId).Value = true;
			int num = 0;
			using (IEnumerator<T> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					string id = listSettingId + num.ToString();
					settingsGroup.GetSetting<T>(id).Value = current;
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
			if (!settingsGroup.ContainsSetting(listSettingId))
			{
				return;
			}
			Clear();
			int num = 0;
			bool foundSetting;
			do
			{
				foundSetting = false;
				string listItemSetting = listSettingId + num.ToString();
				T listItemFromSettings = GetListItemFromSettings(settingsGroup, listItemSetting, out foundSetting);
				if (foundSetting)
				{
					Add(listItemFromSettings);
				}
				num++;
			}
			while (foundSetting);
		}

		public virtual void ClearListItemSettings(ISettingsGroup settingsGroup, string listSettingId)
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
				string settingId = listSettingId + num.ToString();
				if (settingsGroup.ContainsSetting(settingId))
				{
					flag = true;
					settingsGroup.RemoveSetting(settingId);
				}
				num++;
			}
			while (flag);
		}

		protected virtual T GetListItemFromSettings(ISettingsGroup settingsGroup, string listItemSetting, out bool foundSetting)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			T result = default(T);
			foundSetting = false;
			if (settingsGroup.ContainsSetting(listItemSetting))
			{
				foundSetting = true;
				return settingsGroup.GetSetting<T>(listItemSetting);
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			ObservableList<T> observableList = obj as ObservableList<T>;
			if (observableList == null)
			{
				return false;
			}
			if (observableList.Count != Count)
			{
				return false;
			}
			for (int i = 0; i < observableList.Count; i++)
			{
				if (!object.Equals(observableList[i], this[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 13;
			for (int i = 0; i < Count; i++)
			{
				num += (i + 1) * ((this[i] != null) ? this[i].GetHashCode() : 171);
			}
			return num;
		}
	}
}
