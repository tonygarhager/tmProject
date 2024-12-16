using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class SuffixTreeEdge<T> where T : IComparable<T>
	{
		public List<T> Label
		{
			get;
			set;
		}

		public SuffixTreeNode<T> EndNode
		{
			get;
			set;
		}

		public SuffixTreeEdge()
		{
		}

		public SuffixTreeEdge(IList<T> label, SuffixTreeNode<T> endNode)
		{
			Label = new List<T>();
			Label.AddRange(label);
			EndNode = endNode;
		}

		public bool LabelStartsWith(T t)
		{
			if (Label == null || Label.Count == 0)
			{
				return false;
			}
			return Label[0].CompareTo(t) == 0;
		}

		public void Append(T t)
		{
			if (Label == null)
			{
				Label = new List<T>();
			}
			Label.Add(t);
		}

		public SuffixTreeNode<T> SplitBefore(SuffixTree<T> owner, int splitPoint)
		{
			if (splitPoint <= 0 || splitPoint >= Label.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			SuffixTreeNode<T> suffixTreeNode = owner.CreateNode();
			SuffixTreeEdge<T> suffixTreeEdge = new SuffixTreeEdge<T>
			{
				EndNode = EndNode
			};
			suffixTreeNode.Parent = EndNode.Parent;
			EndNode.Parent = suffixTreeNode;
			EndNode = suffixTreeNode;
			for (int i = splitPoint; i < Label.Count; i++)
			{
				suffixTreeEdge.Append(Label[i]);
			}
			suffixTreeNode.AddEdge(suffixTreeEdge);
			Label.RemoveRange(splitPoint, Label.Count - splitPoint);
			return suffixTreeNode;
		}
	}
}
