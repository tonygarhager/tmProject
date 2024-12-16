using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua
{
	internal static class SuffixTreeComputer<T> where T : IComparable<T>
	{
		public static SuffixTree<T> Create(IList<T> s)
		{
			if (s == null)
			{
				throw new ArgumentNullException();
			}
			if (s.Count == 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return CreateNaive(s);
		}

		private static SuffixTree<T> CreateNaive(IList<T> s)
		{
			SuffixTree<T> suffixTree = new SuffixTree<T>();
			for (int i = 0; i < s.Count; i++)
			{
				SuffixTreeNode<T> suffixTreeNode = suffixTree.Root;
				for (int j = i; j < s.Count; j++)
				{
					T t = s[j];
					if (suffixTreeNode.HasEdgeStartingWith(t))
					{
						suffixTreeNode = suffixTreeNode.Traverse(t);
						continue;
					}
					SuffixTreeEdge<T> suffixTreeEdge = new SuffixTreeEdge<T>();
					suffixTreeEdge.Append(t);
					suffixTreeEdge.EndNode = suffixTree.CreateNode();
					suffixTreeNode.AddEdge(suffixTreeEdge);
					suffixTreeNode = suffixTreeEdge.EndNode;
				}
				suffixTreeNode.Position = i;
			}
			return suffixTree;
		}
	}
}
