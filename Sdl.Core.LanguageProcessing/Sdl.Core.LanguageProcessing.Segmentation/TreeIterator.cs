namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class TreeIterator
	{
		private bool _visitedLastNodeChildren;

		private Node _alreadyVisitedNode;

		private readonly bool _visitChildren;

		public Node CurrentNode
		{
			get;
			set;
		}

		public Node Root
		{
			get;
		}

		public TreeIterator(Node currentNode, bool visitChildrenOfCurrentNode = false)
		{
			CurrentNode = currentNode;
			if (CurrentNode != null)
			{
				Root = currentNode.Root;
			}
			_visitChildren = visitChildrenOfCurrentNode;
		}

		public Node Next()
		{
			if (CurrentNode == null || (CurrentNode.Depth == 0 && CurrentNode.IsLast))
			{
				if (CurrentNode == null)
				{
					return null;
				}
				if (!CurrentNode.HasChild || _visitedLastNodeChildren)
				{
					CurrentNode = null;
					return null;
				}
				_visitedLastNodeChildren = true;
			}
			if (CurrentNode.HasChild)
			{
				CurrentNode = CurrentNode.FirstChild;
			}
			else if (CurrentNode.HasNext)
			{
				CurrentNode = CurrentNode.Next;
			}
			else if (CurrentNode.HasParent)
			{
				while (CurrentNode.HasParent && !CurrentNode.Parent.IsRoot && !CurrentNode.Parent.HasNext)
				{
					CurrentNode = CurrentNode.Parent;
				}
				CurrentNode = CurrentNode.Parent;
				if (!CurrentNode.HasNext)
				{
					CurrentNode = null;
					return null;
				}
				CurrentNode = CurrentNode.Next;
			}
			return CurrentNode;
		}

		public Node Previous()
		{
			_visitedLastNodeChildren = false;
			if (CurrentNode == null || (CurrentNode.Depth == 0 && CurrentNode.IsFirst))
			{
				CurrentNode = null;
				_alreadyVisitedNode = null;
				return null;
			}
			if (_alreadyVisitedNode == null && CurrentNode.HasParent && !_visitChildren)
			{
				_alreadyVisitedNode = CurrentNode.Parent;
			}
			if (CurrentNode.HasChild && _alreadyVisitedNode == null)
			{
				while (CurrentNode.HasChild)
				{
					_alreadyVisitedNode = CurrentNode;
					CurrentNode = CurrentNode.LastChild;
				}
				return CurrentNode;
			}
			if (CurrentNode.HasPrevious)
			{
				CurrentNode = CurrentNode.Previous;
				if (!CurrentNode.HasChild)
				{
					return CurrentNode;
				}
				while (CurrentNode.HasChild)
				{
					CurrentNode = CurrentNode.LastChild;
				}
			}
			else if (CurrentNode.Depth > 0)
			{
				CurrentNode = CurrentNode.Parent;
			}
			return CurrentNode;
		}

		public Node PreviousStarter()
		{
			while (CurrentNode != null && !CurrentNode.IsSegmentStarter)
			{
				Previous();
			}
			return CurrentNode;
		}

		public Node NextStarter()
		{
			while (CurrentNode != null && !CurrentNode.IsSegmentStarter)
			{
				Next();
			}
			return CurrentNode;
		}

		public Node PreviousEnder()
		{
			while (CurrentNode != null && !CurrentNode.IsSegmentEnder)
			{
				Previous();
			}
			return CurrentNode;
		}

		public void MoveToLastItemInTree()
		{
			if (Root != null)
			{
				CurrentNode = Root.LastChild;
				while (!CurrentNode.IsLockedContent && CurrentNode.HasChild)
				{
					CurrentNode = CurrentNode.LastChild;
				}
			}
		}

		public Node PreviousExclude()
		{
			while (CurrentNode != null && !CurrentNode.Exclude)
			{
				Previous();
			}
			return CurrentNode;
		}

		public Node PeekNext()
		{
			Node currentNode = CurrentNode;
			Node result = Next();
			CurrentNode = currentNode;
			return result;
		}

		public Node PeekPrevious()
		{
			Node currentNode = CurrentNode;
			Node result = Previous();
			CurrentNode = currentNode;
			return result;
		}
	}
}
