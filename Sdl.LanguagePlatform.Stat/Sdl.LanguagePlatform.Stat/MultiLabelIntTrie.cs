using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat
{
	internal class MultiLabelIntTrie
	{
		internal class Node : IComparable<Node>
		{
			private List<Node> _children;

			public int[] Path;

			public int Key = -1;

			public int ChildCount
			{
				get
				{
					List<Node> children = _children;
					if (children == null)
					{
						return 0;
					}
					return children.Count;
				}
			}

			public Node FindChild(IList<int> keys, int p, bool create, ref bool isPrefix)
			{
				Node node = null;
				int num = 0;
				if (keys == null || keys.Count == 0 || p == keys.Count)
				{
					isPrefix = true;
					return this;
				}
				if (_children != null && _children.Count > 0)
				{
					Node node2 = new Node();
					node2.Path = new int[1]
					{
						keys[p]
					};
					Node item = node2;
					int num2 = _children.BinarySearch(item);
					if (num2 >= 0)
					{
						node = _children[num2];
						num = GetCommonPrefixLength(keys, p, node.Path, 0);
					}
				}
				if (num == 0)
				{
					if (create)
					{
						isPrefix = true;
						return AddChild(keys, p);
					}
					isPrefix = false;
					return null;
				}
				if (node != null && num == node.Path.Length)
				{
					return node.FindChild(keys, p + num, create, ref isPrefix);
				}
				if (!create)
				{
					isPrefix = (num == keys.Count - p);
					return null;
				}
				if (node != null)
				{
					Node node3 = node.Split(num);
				}
				isPrefix = true;
				return node?.AddChild(keys, p + num);
			}

			private Node AddChild(IList<int> keys, int index)
			{
				if (keys == null || keys.Count == 0 || index == keys.Count)
				{
					return this;
				}
				int num = keys.Count - index;
				Node node = new Node();
				node.Path = new int[num];
				Node node2 = node;
				for (int i = 0; i < num; i++)
				{
					node2.Path[i] = keys[index + i];
				}
				if (_children == null)
				{
					_children = new List<Node>
					{
						node2
					};
				}
				else
				{
					int num2 = _children.BinarySearch(node2);
					_children.Insert(~num2, node2);
				}
				return node2;
			}

			private Node Split(int p)
			{
				int num = Path.Length;
				int[] path = Path;
				Node node = new Node();
				node.Key = Key;
				node._children = _children;
				node.Path = new int[Path.Length - p];
				Node node2 = node;
				Array.Copy(path, p, node2.Path, 0, Path.Length - p);
				_children = new List<Node>
				{
					node2
				};
				Path = new int[p];
				Array.Copy(path, 0, Path, 0, p);
				Key = -1;
				return node2;
			}

			public int CompareTo(Node other)
			{
				return Path[0] - other.Path[0];
			}
		}

		public Node Root
		{
			get;
		}

		internal static int GetCommonPrefixLength(IList<int> l1, int start1, IList<int> l2, int start2)
		{
			if (l1 == null || l1.Count == 0 || l2 == null || l2.Count == 0)
			{
				return 0;
			}
			int count = l1.Count;
			int count2 = l2.Count;
			int num = 0;
			while (start1 < count && start2 < count2 && l1[start1] == l2[start2])
			{
				start1++;
				start2++;
				num++;
			}
			return num;
		}

		public MultiLabelIntTrie()
		{
			Root = new Node
			{
				Key = -1
			};
		}

		public void Add(IList<int> path, int key)
		{
			if (Root == null)
			{
				throw new LanguagePlatformException(ErrorCode.ReadonlyResource);
			}
			if (key < 0)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			bool isPrefix = false;
			Node node = Root.FindChild(path, 0, create: true, ref isPrefix);
			node.Key = key;
		}

		public bool Find(IList<int> path, out bool isPrefix, out int key)
		{
			key = 0;
			isPrefix = false;
			Node node = Root.FindChild(path, 0, create: false, ref isPrefix);
			if (node == null || node.Key < 0)
			{
				return false;
			}
			key = node.Key;
			return true;
		}
	}
}
