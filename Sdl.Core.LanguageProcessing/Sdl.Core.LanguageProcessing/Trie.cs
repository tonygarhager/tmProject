using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing
{
	public class Trie<T, V> : IEnumerable<KeyValuePair<IList<T>, V>>, IEnumerable where T : IComparable<T>
	{
		private class Node : IComparable<Node>
		{
			private List<Node> _children;

			public bool IsFinal
			{
				get;
				private set;
			}

			public V Value
			{
				get;
				private set;
			}

			public T Label
			{
				get;
			}

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

			public Node(T edgeLabel)
			{
				_children = null;
				Value = default(V);
				IsFinal = false;
				Label = edgeLabel;
			}

			public void SetValue(V v)
			{
				Value = v;
				IsFinal = true;
			}

			public Node Traverse(T c, bool create)
			{
				Node node = null;
				int num = -1;
				if (_children != null)
				{
					Node item = new Node(c);
					num = _children.BinarySearch(item);
					if (num >= 0)
					{
						return _children[num];
					}
				}
				if (!create)
				{
					return null;
				}
				node = new Node(c);
				if (_children == null)
				{
					_children = new List<Node>
					{
						node
					};
				}
				else
				{
					_children.Insert(~num, node);
				}
				return node;
			}

			public T GetChildLabelAt(int index)
			{
				return _children[index].Label;
			}

			public Node GetChildNodeAt(int index)
			{
				return _children[index];
			}

			public int CompareTo(Node other)
			{
				return Label.CompareTo(other.Label);
			}
		}

		private class TrieIteratorImpl : TrieIterator<T, V>
		{
			private Node _node;

			private readonly List<T> _path;

			public override bool IsValid => _node != null;

			public override V Value
			{
				get
				{
					if (_node == null)
					{
						throw new InvalidOperationException();
					}
					return _node.Value;
				}
			}

			public override IList<T> Path => _path;

			public override bool IsFinal
			{
				get
				{
					if (_node == null)
					{
						throw new InvalidOperationException();
					}
					return _node.IsFinal;
				}
			}

			public TrieIteratorImpl(Node n)
			{
				_node = (n ?? throw new ArgumentNullException());
				_path = new List<T>();
			}

			public override bool Traverse(T key)
			{
				if (_node == null)
				{
					throw new InvalidOperationException();
				}
				_node = _node.Traverse(key, create: false);
				if (_node != null)
				{
					_path.Add(_node.Label);
				}
				return _node != null;
			}

			public override IEnumerator<KeyValuePair<IList<T>, V>> GetEnumerator()
			{
				if (_node == null)
				{
					throw new InvalidOperationException();
				}
				return new TrieEnumeratorImpl(_node, _path);
			}
		}

		private class TrieEnumeratorImpl : TrieEnumerator<T, V>
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

			private KeyValuePair<IList<T>, V>? _current;

			private readonly IList<T> _prefix;

			public override KeyValuePair<IList<T>, V> Current
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
					list.Add(_path[i].Node.Label);
				}
				_current = new KeyValuePair<IList<T>, V>(list, node.Value);
			}

			public override void Dispose()
			{
				_path.Clear();
			}

			public override bool MoveNext()
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
						Node childNodeAt = p.Node.GetChildNodeAt(p.NextEdge);
						p.NextEdge++;
						Push(p);
						Push(new Position(childNodeAt));
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

			public sealed override void Reset()
			{
				_path.Clear();
				Push(new Position(_root));
				_current = null;
			}
		}

		private Node _root;

		public bool IsEnumerationSupported => true;

		public Trie()
		{
			_root = new Node(default(T));
		}

		public void Clear()
		{
			_root = new Node(default(T));
		}

		public TrieIterator<T, V> GetIterator()
		{
			if (_root == null)
			{
				throw new InvalidOperationException();
			}
			return new TrieIteratorImpl(_root);
		}

		public void Add(IList<T> s, V nodeValue)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			Node node = Traverse(s, create: true);
			node.SetValue(nodeValue);
		}

		public void Update(IList<T> s, V nodeValue)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			Node node = Traverse(s, create: true);
			if (node == null)
			{
				throw new InvalidOperationException();
			}
			node.SetValue(nodeValue);
		}

		private Node Traverse(IList<T> s, bool create = false)
		{
			Node node = _root;
			if (s == null)
			{
				return node;
			}
			for (int i = 0; i < s.Count; i++)
			{
				if (node == null)
				{
					break;
				}
				node = node.Traverse(s[i], create);
			}
			return node;
		}

		public V Lookup(IList<T> s)
		{
			Node node = Traverse(s);
			if (node != null && node.IsFinal)
			{
				return node.Value;
			}
			return default(V);
		}

		public bool Contains(IList<T> s)
		{
			Node node = Traverse(s);
			if (node != null)
			{
				return node.IsFinal;
			}
			return false;
		}

		public bool Contains(IList<T> s, out V value)
		{
			Node node = Traverse(s);
			value = default(V);
			if (node == null || !node.IsFinal)
			{
				return false;
			}
			value = node.Value;
			return true;
		}

		public IEnumerator<KeyValuePair<IList<T>, V>> GetEnumerator()
		{
			if (_root == null)
			{
				throw new InvalidOperationException();
			}
			return new TrieEnumeratorImpl(_root);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
