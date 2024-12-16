using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Util
{
	internal class TopologicalSorter<T> where T : ITopologicalSortable
	{
		private IList<T> _items;

		private List<string>[] _expandedInsertAfterIds;

		private bool[] _visited;

		private List<T> _sortedItems;

		private Dictionary<string, List<int>> _itemIdToIndexMap;

		internal TopologicalSorter(IList<T> items)
		{
			_items = items;
		}

		internal List<T> Execute()
		{
			Init();
			ExpandLocations();
			for (int i = 0; i < _items.Count; i++)
			{
				int num = -1;
				for (int j = 0; j < _items.Count; j++)
				{
					if (!_visited[j])
					{
						bool flag = true;
						foreach (string item in _expandedInsertAfterIds[j])
						{
							if (item != _items[j].Id)
							{
								foreach (int item2 in _itemIdToIndexMap[item])
								{
									if (!_visited[item2])
									{
										flag = false;
										break;
									}
								}
								if (!flag)
								{
									break;
								}
							}
						}
						if (flag && (num == -1 || (_items[j].Priority > _items[num].Priority && (_items[j].InsertAfter != null || _items[j].InsertBefore != null))))
						{
							num = j;
						}
					}
				}
				if (num == -1)
				{
					foreach (T item3 in _items)
					{
						_ = item3;
					}
					foreach (T sortedItem in _sortedItems)
					{
						_ = sortedItem;
					}
					break;
				}
				_sortedItems.Add(_items[num]);
				_visited[num] = true;
			}
			return _sortedItems;
		}

		private void Init()
		{
			_visited = new bool[_items.Count];
			_sortedItems = new List<T>(_items.Count);
			_itemIdToIndexMap = new Dictionary<string, List<int>>(_items.Count);
			_expandedInsertAfterIds = new List<string>[_items.Count];
			for (int i = 0; i < _items.Count; i++)
			{
				ITopologicalSortable topologicalSortable = _items[i];
				_visited[i] = false;
				if (!_itemIdToIndexMap.TryGetValue(topologicalSortable.Id, out List<int> value))
				{
					value = new List<int>(1);
					_itemIdToIndexMap[topologicalSortable.Id] = value;
				}
				value.Add(i);
			}
			for (int j = 0; j < _items.Count; j++)
			{
				ITopologicalSortable topologicalSortable2 = _items[j];
				_expandedInsertAfterIds[j] = new List<string>(2);
				if (string.IsNullOrEmpty(topologicalSortable2.InsertAfter))
				{
					continue;
				}
				string[] array = ParseInsertClause(topologicalSortable2.InsertAfter);
				foreach (string text in array)
				{
					if (_itemIdToIndexMap.ContainsKey(text))
					{
						_expandedInsertAfterIds[j].Add(text);
					}
				}
			}
		}

		private static string[] ParseInsertClause(string insertValue)
		{
			if (string.IsNullOrEmpty(insertValue))
			{
				return new string[0];
			}
			return insertValue.Split(',', ';');
		}

		private void ExpandLocations()
		{
			for (int i = 0; i < _items.Count; i++)
			{
				string insertBefore = _items[i].InsertBefore;
				if (string.IsNullOrEmpty(insertBefore))
				{
					continue;
				}
				string[] array = ParseInsertClause(insertBefore);
				foreach (string key in array)
				{
					if (_itemIdToIndexMap.ContainsKey(key))
					{
						foreach (int item in _itemIdToIndexMap[key])
						{
							_expandedInsertAfterIds[item].Add(_items[i].Id);
						}
					}
				}
			}
		}
	}
}
