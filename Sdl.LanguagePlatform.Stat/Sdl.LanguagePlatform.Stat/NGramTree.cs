using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class NGramTree
	{
		public abstract class TreeIterator
		{
			public abstract int Key
			{
				get;
			}

			public abstract int Samples
			{
				get;
			}

			public abstract int UserData
			{
				get;
			}

			public abstract int ChildCount
			{
				get;
			}

			public abstract bool IsValid
			{
				get;
			}

			public abstract bool Navigate(int key);

			public abstract TreeIterator NavigateToChild(int index);
		}

		private class NodeTreeIterator : TreeIterator
		{
			private Node _node;

			public override bool IsValid => _node != null;

			public override int Key
			{
				get
				{
					if (_node == null)
					{
						throw new LanguagePlatformException(ErrorCode.InternalError);
					}
					return _node.Key;
				}
			}

			public override int Samples
			{
				get
				{
					if (_node == null)
					{
						throw new LanguagePlatformException(ErrorCode.InternalError);
					}
					return _node.Samples;
				}
			}

			public override int UserData
			{
				get
				{
					if (_node == null)
					{
						throw new LanguagePlatformException(ErrorCode.InternalError);
					}
					return _node.UserData;
				}
			}

			public override int ChildCount
			{
				get
				{
					if (_node == null)
					{
						throw new LanguagePlatformException(ErrorCode.InternalError);
					}
					return _node.ChildCount;
				}
			}

			public NodeTreeIterator(Node n)
			{
				_node = n;
			}

			public override bool Navigate(int key)
			{
				if (_node == null)
				{
					return false;
				}
				_node = _node.FindChild(key);
				return _node != null;
			}

			public override TreeIterator NavigateToChild(int index)
			{
				if (_node == null || index < 0 || index >= _node.ChildCount)
				{
					return null;
				}
				return new NodeTreeIterator(_node.GetChildAt(index));
			}
		}

		private class BinaryTreeIterator : TreeIterator
		{
			private byte[] _data;

			private int _offset;

			private int _key;

			private int _samples;

			private int _userData;

			private int _childCount;

			private List<int> _childKeys;

			private List<int> _childOffsets;

			public override int Key => _key;

			public override int Samples => _samples;

			public override int UserData => _userData;

			public override int ChildCount => _childCount;

			public override bool IsValid
			{
				get
				{
					if (_data != null && _offset >= 0)
					{
						return _offset < _data.Length;
					}
					return false;
				}
			}

			public BinaryTreeIterator(byte[] data, int offset)
			{
				_data = data;
				Update(offset);
			}

			private void Update(int offset)
			{
				if (_data == null || offset < 0 || offset >= _data.Length)
				{
					_data = null;
					_offset = -1;
					return;
				}
				_offset = offset;
				int offset2 = _offset;
				_key = BitConverter.ToInt32(_data, offset2);
				offset2 += 4;
				_samples = BitConverter.ToInt32(_data, offset2);
				offset2 += 4;
				_userData = BitConverter.ToInt32(_data, offset2);
				offset2 += 4;
				_childCount = BitConverter.ToInt32(_data, offset2);
				offset2 += 4;
				if (_childCount > 0)
				{
					if (_childKeys == null)
					{
						_childKeys = new List<int>();
					}
					else
					{
						_childKeys.Clear();
					}
					if (_childOffsets == null)
					{
						_childOffsets = new List<int>();
					}
					else
					{
						_childOffsets.Clear();
					}
					for (int i = 0; i < _childCount; i++)
					{
						_childKeys.Add(BitConverter.ToInt32(_data, offset2));
						offset2 += 4;
					}
					int num = offset2 + _childCount * 4;
					for (int j = 0; j < _childCount; j++)
					{
						_childOffsets.Add(BitConverter.ToInt32(_data, offset2) + num);
						offset2 += 4;
					}
				}
			}

			public override bool Navigate(int key)
			{
				if (_data == null || _childCount == 0)
				{
					return false;
				}
				int num = _childKeys.BinarySearch(key);
				if (num < 0)
				{
					_data = null;
					return false;
				}
				Update(_childOffsets[num]);
				return _data != null;
			}

			public override TreeIterator NavigateToChild(int index)
			{
				if (_data == null || _childCount == 0 || index < 0 || index >= _childCount)
				{
					return null;
				}
				return new BinaryTreeIterator(_data, _childOffsets[index]);
			}
		}

		private class Node : IComparer<Node>, IComparable<Node>
		{
			private List<Node> _sub;

			public int Key;

			public int Samples;

			public int UserData = -1;

			public int ChildCount
			{
				get
				{
					List<Node> sub = _sub;
					if (sub == null)
					{
						return 0;
					}
					return sub.Count;
				}
			}

			public Node FindChild(int key)
			{
				if (_sub == null)
				{
					return null;
				}
				int count = _sub.Count;
				if (count == 0)
				{
					return null;
				}
				if (count < 7)
				{
					for (int i = 0; i < count; i++)
					{
						if (_sub[i].Key == key)
						{
							return _sub[i];
						}
					}
					return null;
				}
				Node item = new Node
				{
					Key = key
				};
				int num = _sub.BinarySearch(item);
				if (num < 0)
				{
					return null;
				}
				return _sub[num];
			}

			public Node GetChildAt(int index)
			{
				if (_sub == null || index < 0 || index >= _sub.Count)
				{
					return null;
				}
				return _sub[index];
			}

			public void Add(Node child)
			{
				if (child == null)
				{
					throw new ArgumentNullException("child");
				}
				if (_sub == null)
				{
					_sub = new List<Node>
					{
						child
					};
					return;
				}
				int num = _sub.BinarySearch(child);
				if (num >= 0)
				{
					throw new LanguagePlatformException(ErrorCode.InternalError);
				}
				_sub.Insert(~num, child);
			}

			public bool Verify()
			{
				if (_sub == null)
				{
					return true;
				}
				int num = -1;
				int num2 = 0;
				foreach (Node item in _sub)
				{
					num2 += item.Samples;
					if (item.Key <= num)
					{
						return false;
					}
					num = item.Key;
				}
				return num2 == Samples;
			}

			public byte[] Serialize()
			{
				List<byte[]> list = new List<byte[]>();
				MemoryStream memoryStream = new MemoryStream();
				List<Node> sub = _sub;
				int num = (sub != null) ? sub.Count : 0;
				int num2 = 0;
				byte[] bytes = BitConverter.GetBytes(Key);
				memoryStream.Write(bytes, 0, bytes.Length);
				num2 += 4;
				bytes = BitConverter.GetBytes(Samples);
				memoryStream.Write(bytes, 0, bytes.Length);
				num2 += 4;
				bytes = BitConverter.GetBytes(UserData);
				memoryStream.Write(bytes, 0, bytes.Length);
				num2 += 4;
				bytes = BitConverter.GetBytes(num);
				memoryStream.Write(bytes, 0, bytes.Length);
				num2 += 4;
				int num3 = int.MinValue;
				for (int i = 0; i < num; i++)
				{
					num3 = _sub[i].Key;
					list.Add(_sub[i].Serialize());
				}
				for (int j = 0; j < num; j++)
				{
					if (_sub != null)
					{
						bytes = BitConverter.GetBytes(_sub[j].Key);
					}
					memoryStream.Write(bytes, 0, bytes.Length);
					num2 += 4;
				}
				int num4 = 0;
				for (int k = 0; k < num; k++)
				{
					bytes = BitConverter.GetBytes(num4);
					memoryStream.Write(bytes, 0, bytes.Length);
					num4 += list[k].Length;
					num2 += 4;
				}
				for (int l = 0; l < num; l++)
				{
					memoryStream.Write(list[l], 0, list[l].Length);
					num2 += list[l].Length;
				}
				return memoryStream.ToArray();
			}

			public int Compare(Node x, Node y)
			{
				if (x == null)
				{
					throw new ArgumentNullException("x");
				}
				if (y == null)
				{
					throw new ArgumentNullException("y");
				}
				return x.Key - y.Key;
			}

			public int CompareTo(Node other)
			{
				return Key - other.Key;
			}
		}

		private const int SizeofInt = 4;

		private const int Version = 20061209;

		private Node _root;

		private byte[] _data;

		private BinaryTreeIterator _rootIter;

		private readonly string _fileName;

		public bool Exists => File.Exists(_fileName);

		public int N
		{
			get;
			private set;
		}

		public int Samples => _root?.Samples ?? _rootIter.Samples;

		public TreeIterator Root
		{
			get
			{
				if (_root == null)
				{
					return new BinaryTreeIterator(_data, 0);
				}
				return new NodeTreeIterator(_root);
			}
		}

		public bool IsLocked => _data != null;

		public NGramTree(string fileName, int n)
		{
			_fileName = fileName;
			_root = new Node
			{
				Key = -1,
				Samples = 0,
				UserData = 0
			};
			N = n;
			_data = null;
			_rootIter = null;
		}

		public void Dump(VocabularyFile v, TextWriter stream)
		{
			stream.WriteLine(N);
			DumpInternal(Root, v, 0, stream);
		}

		private static void DumpInternal(TreeIterator n, IKeyToStringMapper v, int level, TextWriter stream)
		{
			for (int i = 0; i < level; i++)
			{
				stream.Write('\t');
			}
			stream.WriteLine("{0}\tf={1}\td={2}", (v == null) ? n.Key.ToString() : v.Lookup(n.Key), n.Samples, n.UserData);
			for (int j = 0; j < n.ChildCount; j++)
			{
				DumpInternal(n.NavigateToChild(j), v, level + 1, stream);
			}
		}

		public void Add(IList<int> keys, int userData)
		{
			if (keys.Count != N && N > 0)
			{
				throw new ArgumentException();
			}
			if (_root == null)
			{
				throw new LanguagePlatformException(ErrorCode.ReadonlyResource);
			}
			Node node = _root;
			node.Samples++;
			for (int i = 0; i < keys.Count; i++)
			{
				Node node2 = node.FindChild(keys[i]);
				if (node2 == null)
				{
					node2 = new Node
					{
						Key = keys[i],
						Samples = 0
					};
					node.Add(node2);
				}
				if (i == keys.Count - 1)
				{
					node2.UserData = userData;
				}
				node2.Samples++;
				node = node2;
			}
		}

		public TreeIterator Find(IList<int> sample)
		{
			TreeIterator treeIterator = (_root != null) ? ((TreeIterator)new NodeTreeIterator(_root)) : ((TreeIterator)new BinaryTreeIterator(_data, 0));
			int num = 0;
			while (treeIterator.IsValid && (num < N || N == 0) && num < sample.Count)
			{
				treeIterator.Navigate(sample[num]);
				num++;
			}
			return treeIterator;
		}

		public int GetCount(IList<int> sample)
		{
			TreeIterator treeIterator = Find(sample);
			if (!treeIterator.IsValid)
			{
				return 0;
			}
			return treeIterator.Samples;
		}

		public void Load()
		{
			if (!string.IsNullOrEmpty(_fileName))
			{
				using (Stream stream = File.OpenRead(_fileName))
				{
					int num = (int)stream.Length;
					if (num < 12)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					byte[] array = new byte[12];
					if (stream.Read(array, 0, 12) != 12)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					int num2 = BitConverter.ToInt32(array, 0);
					if (num2 != 20061209)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					int num3 = BitConverter.ToInt32(array, 4);
					if (num3 != num - 12)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					N = BitConverter.ToInt32(array, 8);
					if (N < 0)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					_data = new byte[num3];
					if (stream.Read(_data, 0, num3) != num3)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					_root = null;
					_rootIter = new BinaryTreeIterator(_data, 0);
				}
			}
		}

		public void Save()
		{
			if (!string.IsNullOrEmpty(_fileName))
			{
				using (Stream stream = File.Create(_fileName))
				{
					byte[] array = _root.Serialize();
					stream.Write(BitConverter.GetBytes(20061209), 0, 4);
					stream.Write(BitConverter.GetBytes(array.Length), 0, 4);
					stream.Write(BitConverter.GetBytes(N), 0, 4);
					stream.Write(array, 0, array.Length);
					stream.Close();
				}
			}
		}
	}
}
