using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class SuffixTreeNode<T> where T : IComparable<T>
	{
		public SuffixTreeNode<T> Parent
		{
			get;
			set;
		}

		public SuffixTreeNode<T> SuffixLink
		{
			get;
			set;
		}

		public int Length
		{
			get;
			set;
		}

		public int? Position
		{
			get;
			set;
		}

		public int ID
		{
			get;
		}

		public bool HasEdges
		{
			get
			{
				if (Edges != null)
				{
					return Edges.Count > 0;
				}
				return false;
			}
		}

		public List<SuffixTreeEdge<T>> Edges
		{
			get;
			set;
		}

		public SuffixTreeNode(int id)
		{
			ID = id;
		}

		public IEnumerable<int> CollectPositions()
		{
			return FindRecursive((SuffixTreeNode<T> n) => n.Position.HasValue)?.Select((SuffixTreeNode<T> n) => n.Position.Value);
		}

		private List<SuffixTreeNode<T>> FindRecursive(Predicate<SuffixTreeNode<T>> predicate)
		{
			List<SuffixTreeNode<T>> list = new List<SuffixTreeNode<T>>();
			if (predicate(this))
			{
				list.Add(this);
			}
			if (Edges == null)
			{
				return list;
			}
			foreach (SuffixTreeEdge<T> edge in Edges)
			{
				List<SuffixTreeNode<T>> list2 = edge.EndNode.FindRecursive(predicate);
				if (list2 != null && list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
			return list;
		}

		public bool HasEdgeStartingWith(T t)
		{
			return FindEdgeStartingWith(t) != null;
		}

		public SuffixTreeEdge<T> FindEdgeStartingWith(T t)
		{
			if (Edges == null || Edges.Count == 0)
			{
				return null;
			}
			return Edges.FirstOrDefault((SuffixTreeEdge<T> e) => e.LabelStartsWith(t));
		}

		public SuffixTreeNode<T> Traverse(T t)
		{
			SuffixTreeEdge<T> suffixTreeEdge = FindEdgeStartingWith(t);
			if (suffixTreeEdge == null)
			{
				return null;
			}
			return suffixTreeEdge.EndNode;
		}

		public void AddEdge(SuffixTreeEdge<T> edge)
		{
			if (((edge != null) ? edge.Label : null) == null || edge.Label.Count == 0)
			{
				throw new ArgumentException();
			}
			if (edge.EndNode == null)
			{
				throw new ArgumentException();
			}
			if (Edges == null)
			{
				Edges = new List<SuffixTreeEdge<T>>();
			}
			edge.EndNode.Parent = this;
			Edges.Add(edge);
		}
	}
}
