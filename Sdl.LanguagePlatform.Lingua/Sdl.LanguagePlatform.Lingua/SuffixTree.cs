using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class SuffixTree<T> where T : IComparable<T>
	{
		private int _nextNodeId;

		public SuffixTreeNode<T> Root
		{
			get;
		}

		public SuffixTree()
		{
			_nextNodeId = 0;
			Root = new SuffixTreeNode<T>(_nextNodeId++);
		}

		public SuffixTreeNode<T> CreateNode()
		{
			return new SuffixTreeNode<T>(_nextNodeId++);
		}

		public SuffixTreeNode<T> Traverse(IList<T> s)
		{
			if (s == null)
			{
				throw new ArgumentNullException();
			}
			if (s.Count == 0)
			{
				return Root;
			}
			SuffixTreeNode<T> suffixTreeNode = Root;
			for (int i = 0; i < s.Count; i++)
			{
				if (suffixTreeNode == null)
				{
					break;
				}
				suffixTreeNode = suffixTreeNode.Traverse(s[i]);
			}
			return suffixTreeNode;
		}

		public bool Verify()
		{
			if (Root == null)
			{
				return false;
			}
			if (Root.Parent == null)
			{
				return Verify(Root);
			}
			return false;
		}

		public void Dump(string fn)
		{
			using (StreamWriter wtr = File.CreateText(fn))
			{
				Dump(wtr);
			}
		}

		public void Dump(TextWriter wtr)
		{
			if (Root == null)
			{
				wtr.WriteLine("Root is null");
			}
			Dump(Root, 0, wtr);
		}

		private static void Dump(SuffixTreeNode<T> n, int level, TextWriter wtr)
		{
			if (n.HasEdges)
			{
				for (int i = 0; i < level; i++)
				{
					wtr.Write("  ");
				}
				wtr.WriteLine("Node {0}", n.ID);
				foreach (SuffixTreeEdge<T> edge in n.Edges)
				{
					for (int j = 0; j < level; j++)
					{
						wtr.Write("  ");
					}
					wtr.Write("+- ");
					bool flag = true;
					foreach (T item in edge.Label)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							wtr.Write("/");
						}
						wtr.Write(item.ToString());
					}
					if (edge.EndNode.Position.HasValue)
					{
						wtr.WriteLine(" -> {0} (pos={1})", edge.EndNode.ID, edge.EndNode.Position.Value);
					}
					else
					{
						wtr.WriteLine(" -> {0}", edge.EndNode.ID);
					}
				}
				foreach (SuffixTreeEdge<T> edge2 in n.Edges)
				{
					Dump(edge2.EndNode, level + 1, wtr);
				}
			}
		}

		private static bool Verify(SuffixTreeNode<T> n)
		{
			if (n.Edges == null)
			{
				return true;
			}
			List<T> list = new List<T>();
			foreach (SuffixTreeEdge<T> edge in n.Edges)
			{
				if (edge.Label == null || edge.Label.Count == 0)
				{
					return false;
				}
				if (edge.EndNode == null)
				{
					return false;
				}
				if (edge.EndNode.Parent != n)
				{
					return false;
				}
				list.Add(edge.Label[0]);
			}
			list.Sort();
			for (int i = 1; i < list.Count; i++)
			{
				if (list[i].CompareTo(list[i - 1]) == 0)
				{
					return false;
				}
			}
			return n.Edges.All((SuffixTreeEdge<T> e) => Verify(e.EndNode));
		}
	}
}
