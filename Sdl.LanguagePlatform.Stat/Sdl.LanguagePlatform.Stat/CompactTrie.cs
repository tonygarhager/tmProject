using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.Stat
{
	internal class CompactTrie<T> where T : struct, IComparable<T>
	{
		internal class Node : IComparable<Node>
		{
			private Node[] _children;

			public T[] Path;

			public int Key = -1;

			public IList<Node> Children => _children;

			public int ChildCount
			{
				get
				{
					Node[] children = _children;
					if (children == null)
					{
						return 0;
					}
					return children.Length;
				}
			}

			public bool IsFinal => Key >= 0;

			public Node FindChild(IList<T> keys, int p, bool create, ref bool isPrefix)
			{
				Node node = null;
				int num = 0;
				if (keys == null || keys.Count == 0 || p == keys.Count)
				{
					isPrefix = true;
					return this;
				}
				if (_children != null && _children.Length != 0)
				{
					Node node2 = new Node();
					node2.Path = new T[1]
					{
						keys[p]
					};
					Node value = node2;
					int num2 = Array.BinarySearch(_children, value);
					if (num2 >= 0)
					{
						node = _children[num2];
						num = CompactTrie<T>.GetCommonPrefixLength(keys, p, (IList<T>)node.Path, 0);
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
				return node.AddChild(keys, p + num);
			}

			private Node AddChild(IList<T> keys, int index)
			{
				if (keys == null || keys.Count == 0 || index == keys.Count)
				{
					return this;
				}
				int num = keys.Count - index;
				Node node = new Node();
				node.Path = new T[num];
				Node node2 = node;
				for (int i = 0; i < num; i++)
				{
					node2.Path[i] = keys[index + i];
				}
				if (_children == null)
				{
					AppendChild(node2);
				}
				else
				{
					int num2 = Array.BinarySearch(_children, node2);
					InsertChild(~num2, node2);
				}
				return node2;
			}

			private Node Split(int p)
			{
				int num = Path.Length;
				T[] path = Path;
				Node node = new Node();
				node.Key = Key;
				node._children = _children;
				node.Path = new T[Path.Length - p];
				Node node2 = node;
				Array.Copy(path, p, node2.Path, 0, Path.Length - p);
				_children = null;
				AppendChild(node2);
				Path = new T[p];
				Array.Copy(path, 0, Path, 0, p);
				Key = -1;
				return node2;
			}

			private void AppendChild(Node n)
			{
				if (_children == null)
				{
					_children = new Node[1];
					_children[0] = n;
					return;
				}
				Node[] children = _children;
				_children = new Node[children.Length + 1];
				Array.Copy(children, _children, children.Length);
				_children[children.Length] = n;
			}

			private void InsertChild(int index, Node n)
			{
				if (_children == null || index == _children.Length)
				{
					AppendChild(n);
					return;
				}
				Node[] children = _children;
				_children = new Node[children.Length + 1];
				Array.Copy(children, 0, _children, 0, index);
				Array.Copy(children, index, _children, index + 1, children.Length - index);
				_children[index] = n;
			}

			public int CompareTo(Node other)
			{
				return Path[0].CompareTo(other.Path[0]);
			}
		}

		private class TrieEnumeratorImpl : IEnumerator<KeyValuePair<IList<T>, int>>, IDisposable, IEnumerator
		{
			private struct Position
			{
				public readonly Node Node;

				public int NextEdge;

				public Position(Node n)
				{
					Node = n;
					NextEdge = -1;
				}
			}

			private readonly Node _root;

			private readonly List<Position> _path;

			private KeyValuePair<IList<T>, int>? _current;

			private readonly IList<T> _prefix;

			object IEnumerator.Current => Current;

			public KeyValuePair<IList<T>, int> Current
			{
				get
				{
					if (!_current.HasValue)
					{
						throw new InvalidOperationException();
					}
					return _current.Value;
				}
			}

			public TrieEnumeratorImpl(Node root, IList<T> prefix = null)
			{
				_root = root;
				_path = new List<Position>();
				_prefix = prefix;
				_current = null;
				Reset();
			}

			private void SetCurrent()
			{
				if (_path == null || _path.Count == 0)
				{
					_current = null;
					return;
				}
				Position position = Peek();
				Node node = position.Node;
				if (node == null || !node.IsFinal)
				{
					_current = null;
					return;
				}
				List<T> list = new List<T>();
				if (_prefix != null)
				{
					list.AddRange(_prefix);
				}
				for (int i = 1; i < _path.Count; i++)
				{
					list.AddRange(_path[i].Node.Path);
				}
				_current = new KeyValuePair<IList<T>, int>(list, node.Key);
			}

			public void Dispose()
			{
				_path.Clear();
				_current = null;
			}

			public bool MoveNext()
			{
				_current = null;
				while (_path.Count > 0)
				{
					Position p = Pop();
					if (p.NextEdge < 0)
					{
						p.NextEdge++;
						if (p.Node.IsFinal)
						{
							Push(p);
							SetCurrent();
							return true;
						}
					}
					if (p.NextEdge < p.Node.ChildCount)
					{
						Node n = p.Node.Children[p.NextEdge];
						p.NextEdge++;
						Push(p);
						Push(new Position(n));
					}
				}
				return false;
			}

			private void Push(Position p)
			{
				_path.Add(p);
			}

			private Position Peek()
			{
				return _path[_path.Count - 1];
			}

			private Position Pop()
			{
				Position result = _path[_path.Count - 1];
				_path.RemoveAt(_path.Count - 1);
				return result;
			}

			public void Reset()
			{
				_path.Clear();
				Push(new Position(_root));
				_current = null;
			}
		}

		public Node Root
		{
			get;
			private set;
		}

		internal static int GetCommonPrefixLength(IList<T> l1, int start1, IList<T> l2, int start2)
		{
			if (l1 == null || l1.Count == 0 || l2 == null || l2.Count == 0)
			{
				return 0;
			}
			int count = l1.Count;
			int count2 = l2.Count;
			int num = 0;
			while (start1 < count && start2 < count2 && l1[start1].CompareTo(l2[start2]) == 0)
			{
				start1++;
				start2++;
				num++;
			}
			return num;
		}

		internal static bool AreEqual(IList<T> l1, IList<T> l2)
		{
			if (l1 == null || l2 == null || l1.Count != l2.Count)
			{
				return false;
			}
			return !l1.Where((T t, int c) => t.CompareTo(l2[c]) != 0).Any();
		}

		public CompactTrie()
		{
			Root = new Node
			{
				Key = -1
			};
		}

		public IEnumerator<KeyValuePair<IList<T>, int>> GetEnumerator()
		{
			return new TrieEnumeratorImpl(Root);
		}

		public void Add(IList<T> path, int key)
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

		public void Clear()
		{
			Root = new Node
			{
				Key = -1
			};
		}

		public bool Find(IList<T> path, out bool isPrefix, out int key)
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

		public bool Contains(IList<T> path, out int key)
		{
			bool isPrefix;
			return Find(path, out isPrefix, out key);
		}
	}
}
