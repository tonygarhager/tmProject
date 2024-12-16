using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class CompactTrie<T> where T : struct, IComparable<T>
	{
		internal class Node : IComparable<Node>
		{
			private Node[] _Children;

			public T[] Path;

			public int Key = -1;

			public IList<Node> Children => _Children;

			public int ChildCount
			{
				get
				{
					if (_Children != null)
					{
						return _Children.Length;
					}
					return 0;
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
				if (_Children != null && _Children.Length != 0)
				{
					Node node2 = new Node();
					node2.Path = new T[1]
					{
						keys[p]
					};
					int num2 = Array.BinarySearch(_Children, node2);
					if (num2 >= 0)
					{
						node = _Children[num2];
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
				if (num == node.Path.Length)
				{
					return node.FindChild(keys, p + num, create, ref isPrefix);
				}
				if (!create)
				{
					isPrefix = (num == keys.Count - p);
					return null;
				}
				Node node3 = node.Split(num);
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
				for (int i = 0; i < num; i++)
				{
					node.Path[i] = keys[index + i];
				}
				if (_Children == null)
				{
					AppendChild(node);
				}
				else
				{
					int num2 = Array.BinarySearch(_Children, node);
					InsertChild(~num2, node);
				}
				return node;
			}

			private Node Split(int p)
			{
				int num = Path.Length;
				T[] path = Path;
				Node node = new Node();
				node.Key = Key;
				node._Children = _Children;
				node.Path = new T[Path.Length - p];
				Array.Copy(path, p, node.Path, 0, Path.Length - p);
				_Children = null;
				AppendChild(node);
				Path = new T[p];
				Array.Copy(path, 0, Path, 0, p);
				Key = -1;
				return node;
			}

			private void AppendChild(Node n)
			{
				if (_Children == null)
				{
					_Children = new Node[1];
					_Children[0] = n;
					return;
				}
				Node[] children = _Children;
				_Children = new Node[children.Length + 1];
				Array.Copy(children, _Children, children.Length);
				_Children[children.Length] = n;
			}

			private void InsertChild(int index, Node n)
			{
				if (_Children == null || index == _Children.Length)
				{
					AppendChild(n);
					return;
				}
				Node[] children = _Children;
				_Children = new Node[children.Length + 1];
				Array.Copy(children, 0, _Children, 0, index);
				Array.Copy(children, index, _Children, index + 1, children.Length - index);
				_Children[index] = n;
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

			private readonly Node _Root;

			private readonly List<Position> _Path;

			private KeyValuePair<IList<T>, int>? _Current;

			private readonly IList<T> _Prefix;

			object IEnumerator.Current => Current;

			public KeyValuePair<IList<T>, int> Current
			{
				get
				{
					if (!_Current.HasValue || !_Current.HasValue)
					{
						throw new InvalidOperationException();
					}
					return _Current.Value;
				}
			}

			public TrieEnumeratorImpl(Node root, IList<T> prefix)
			{
				_Root = root;
				_Path = new List<Position>();
				_Prefix = prefix;
				_Current = null;
				Reset();
			}

			public TrieEnumeratorImpl(Node root)
				: this(root, (IList<T>)null)
			{
			}

			private void SetCurrent()
			{
				if (_Path == null || _Path.Count == 0)
				{
					_Current = null;
					return;
				}
				Position position = Peek();
				Node node = position.Node;
				if (node == null || !node.IsFinal)
				{
					_Current = null;
					return;
				}
				List<T> list = new List<T>();
				if (_Prefix != null)
				{
					list.AddRange(_Prefix);
				}
				for (int i = 1; i < _Path.Count; i++)
				{
					list.AddRange(_Path[i].Node.Path);
				}
				_Current = new KeyValuePair<IList<T>, int>(list, node.Key);
			}

			public void Dispose()
			{
				_Path.Clear();
				_Current = null;
			}

			public bool MoveNext()
			{
				_Current = null;
				while (_Path.Count > 0)
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
				_Path.Add(p);
			}

			private Position Peek()
			{
				return _Path[_Path.Count - 1];
			}

			private Position Pop()
			{
				Position result = _Path[_Path.Count - 1];
				_Path.RemoveAt(_Path.Count - 1);
				return result;
			}

			public void Reset()
			{
				_Path.Clear();
				Push(new Position(_Root));
				_Current = null;
			}
		}

		private Node _Root;

		public Node Root => _Root;

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
			for (int i = 0; i < l1.Count; i++)
			{
				if (l1[i].CompareTo(l2[i]) != 0)
				{
					return false;
				}
			}
			return true;
		}

		public CompactTrie()
		{
			_Root = new Node();
			_Root.Key = -1;
		}

		public IEnumerator<KeyValuePair<IList<T>, int>> GetEnumerator()
		{
			return new TrieEnumeratorImpl(_Root);
		}

		public void Add(IList<T> path, int key)
		{
			if (_Root == null)
			{
				throw new LanguagePlatformException(ErrorCode.ReadonlyResource);
			}
			if (key < 0)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			bool isPrefix = false;
			Node node = _Root.FindChild(path, 0, create: true, ref isPrefix);
			node.Key = key;
		}

		public void Clear()
		{
			_Root = new Node();
			_Root.Key = -1;
		}

		public bool Find(IList<T> path, out bool isPrefix, out int key)
		{
			key = 0;
			isPrefix = false;
			Node node = _Root.FindChild(path, 0, create: false, ref isPrefix);
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
