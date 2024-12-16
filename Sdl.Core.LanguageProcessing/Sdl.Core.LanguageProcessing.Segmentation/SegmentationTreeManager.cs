using System;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public abstract class SegmentationTreeManager
	{
		private TreeIterator _iteratorStarterEnder;

		protected SegmentationEngineRunner _segmentationEngineRunner;

		public virtual Tree Tree => null;

		public virtual void SetLastEnder()
		{
			if (Tree?.Root?.LastChild == null || Tree.Root.FirstChild.LockedContentOnlyInChainContainingText)
			{
				return;
			}
			if (Tree.Root.LastChild.BranchCount == 1 && Tree.Root.LastChild.IsLockedContent && Tree.Root.LastChild.LockedContentInclude && Tree.Root.LastChild.LockedContentHasChild)
			{
				Tree.Root.LastChild.IsSegmentEnder = true;
				return;
			}
			if (ContainsAllIncludeLockedContentInBranch(Tree.Root.LastChild))
			{
				Tree.Root.LastChild.IsSegmentEnder = true;
				return;
			}
			if (Tree.Root.LastChild.IsWhitespace && Tree.Root.LastChild.IsSegmentEnder && Tree.Root.LastChild.HasPrevious)
			{
				Tree.Root.LastChild.IsSegmentEnder = false;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = null;
			while (treeIterator.CurrentNode != null)
			{
				Node currentNode = treeIterator.CurrentNode;
				if (currentNode.Include && (currentNode.IsTagPair || (currentNode.IsPlaceholderTag && !currentNode.IsWordStop) || currentNode.IsRevisionMarker))
				{
					if (!currentNode.IsRevisionMarker || (currentNode.HasChild && currentNode.FirstChild.NonWhitespaceTextItemsInBranchCount != 0))
					{
						treeIterator.CurrentNode.IsSegmentEnder = true;
						return;
					}
					treeIterator.CurrentNode.IsSegmentEnder = false;
				}
				if (currentNode.IsPlaceholderTag && !currentNode.IsWordStop && currentNode.HasPrevious && currentNode.IncludeWithText)
				{
					Node previous = currentNode.Previous;
					while (previous.HasPrevious && !previous.IsSegmentEnder && (previous.IsWhitespace || (previous.IsPlaceholderTag && previous.AnyInclude)))
					{
						previous = previous.Previous;
					}
					if (previous.IsText && !previous.IsWhitespace)
					{
						currentNode.IsSegmentEnder = true;
						previous.IsSegmentEnder = false;
						return;
					}
				}
				if (currentNode.IsLockedContent && currentNode.LockedContentInclude && !currentNode.IsInsideMayExcludeOrExcludeContainer)
				{
					currentNode.IsSegmentEnder = true;
					Node previousEnderInBranch = currentNode.PreviousEnderInBranch;
					if (previousEnderInBranch != null && previousEnderInBranch.Next.ItemsEqual(currentNode))
					{
						previousEnderInBranch.IsSegmentEnder = false;
					}
					return;
				}
				if (currentNode.IsText)
				{
					if (!currentNode.IsWhitespace)
					{
						node = treeIterator.CurrentNode;
						break;
					}
					currentNode.IsSegmentEnder = false;
				}
				treeIterator.Previous();
			}
			if (node == null)
			{
				return;
			}
			treeIterator = new TreeIterator(node);
			while (treeIterator.CurrentNode.HasParagraphNext)
			{
				Node node2 = treeIterator.Next();
				if (node2.MayExclude && !node2.CanHide)
				{
					break;
				}
				if (node2.IncludeWithText && !node2.IsWordStop && !HasExcludeBeforeTextInBranch(node2))
				{
					node2.IsSegmentEnder = true;
					return;
				}
			}
			if (!node.IsInsideSegment)
			{
				node.IsSegmentEnder = true;
			}
		}

		private static bool ContainsAllIncludeLockedContentInBranch(Node node)
		{
			for (Node node2 = node; node2 != null; node2 = node2.Previous)
			{
				if (!node2.IsLockedContent || !node2.AnyInclude || !node2.LockedContentHasChild || node2.LockedContentOnlyContainsWhitespace)
				{
					return false;
				}
			}
			return true;
		}

		public virtual void SetRemainingEnders()
		{
			if (Tree?.Root?.FirstChild != null && !Tree.Root.FirstChild.LockedContentOnlyInChainContainingText && !SetRemainingEnders(handlePlaceholders: false))
			{
				SetRemainingEnders(handlePlaceholders: true);
			}
		}

		public virtual bool SetRemainingEnders(bool handlePlaceholders)
		{
			bool result = false;
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = treeIterator.CurrentNode;
			while (node != null && !node.IsSegmentEnder)
			{
				node = treeIterator.Previous();
			}
			Node node2 = node;
			if (node == null)
			{
				return false;
			}
			Node node3 = null;
			do
			{
				Node node4 = (!handlePlaceholders) ? ((!node.IsText || node.IsWhitespace || node.ItemsEqual(node2)) ? FindPreviousTextNode(node, allowWhitespace: false) : node) : ((!node.IsPlaceholderTag || !node.Include || node.ItemsEqual(node2)) ? FindPreviousPlaceholderNode(node) : node);
				if (node4 != null)
				{
					bool flag = HasExcludeBoundary(node4, node2);
					node3 = node4;
					if (node4.IsSegmentEnder)
					{
						if (node4.IsWhitespace)
						{
							node4.IsSegmentEnder = false;
							Node node5 = FindPreviousTextNode(node4, allowWhitespace: false);
							if (node5 != null)
							{
								node5.IsSegmentEnder = true;
							}
						}
						else
						{
							node2 = node4;
						}
					}
					if (flag && !node4.IsSegmentEnder)
					{
						node4.IsSegmentEnder = true;
						result = true;
						node2 = node4;
						treeIterator = new TreeIterator(node4);
					}
					if (node.IsWhitespace && node.IsSegmentEnder)
					{
						node4.IsSegmentEnder = true;
						node.IsSegmentEnder = false;
					}
				}
				else if ((!handlePlaceholders && node.IsText && !node.IsWhitespace) || (handlePlaceholders && node.IsPlaceholderTag && node.Include))
				{
					if (node3 != null)
					{
						if (!HasExcludeBoundary(node, node3))
						{
							return result;
						}
					}
					else if (HasExcludeBoundary(node, node2))
					{
						node.IsSegmentEnder = true;
						result = true;
					}
				}
				node = treeIterator.Previous();
			}
			while (node != null);
			return result;
		}

		public virtual Node GetNextIncludeEnders(Node textNode)
		{
			if (!textNode.HasNext)
			{
				return textNode;
			}
			Node next = textNode.Next;
			Node result = textNode;
			while (next != null && !next.IsWhitespace && ((next.IsPlaceholderTag && !next.IsWordStop) || next.IsLockedContent) && (next.AnyInclude || (next.LockedContentAnyInclude && !next.IsIsolatedTopLevelNode)))
			{
				if (next.IsSegmentStarter)
				{
					return result;
				}
				if (!next.IsWhitespace)
				{
					result = next;
				}
				next = next.Next;
			}
			return result;
		}

		public virtual Node GetNextIncludeEndersIncludingWhitespace(Node textNode)
		{
			if (!textNode.HasNext)
			{
				return textNode;
			}
			bool flag = textNode.MayExclude && !textNode.IsText && !textNode.CanHide;
			bool flag2 = false;
			Node next = textNode.Next;
			Node result = textNode;
			while (next != null && next.CanIncludeAterText)
			{
				if (next.Include || next.ContainsOnlyIncludeInside)
				{
					flag2 = true;
				}
				if (next.IsOnlyIncludeTagContentContainer)
				{
					next.ResetStartersAndEndersInContainer();
					next.IsSegmentStarter = false;
					next.IsSegmentEnder = false;
				}
				if (!next.IsWhitespace)
				{
					result = next;
				}
				next = next.Next;
			}
			if (flag && !flag2)
			{
				return textNode;
			}
			return result;
		}

		public virtual Node FindPreviousTextNode(Node node, bool allowWhitespace)
		{
			TreeIterator treeIterator = new TreeIterator(node);
			treeIterator.Previous();
			while (treeIterator.CurrentNode != null && (!treeIterator.CurrentNode.IsText || (treeIterator.CurrentNode.IsWhitespace && !allowWhitespace)) && (!treeIterator.CurrentNode.IsWhitespace || !treeIterator.CurrentNode.IsSegmentEnder))
			{
				if (treeIterator.CurrentNode.IsLockedContent && treeIterator.CurrentNode.BranchIndex == 0 && treeIterator.CurrentNode.IsInsideMayExcludeOrExcludeContainer && treeIterator.CurrentNode.Parent.BranchIndex == 0 && treeIterator.CurrentNode.Parent.Parent.IsRoot)
				{
					return node;
				}
				treeIterator.Previous();
			}
			return treeIterator.CurrentNode;
		}

		public virtual Node FindPreviousLockedContentIncludeNode(Node node)
		{
			TreeIterator treeIterator = new TreeIterator(node);
			treeIterator.Previous();
			while (treeIterator.CurrentNode != null && !treeIterator.CurrentNode.LockedContentAnyInclude)
			{
				treeIterator.Previous();
			}
			return treeIterator.CurrentNode;
		}

		public virtual Node FindPreviousPlaceholderNode(Node node)
		{
			TreeIterator treeIterator = new TreeIterator(node);
			Node node2 = treeIterator.Previous();
			while (node2 != null && (!node2.IsPlaceholderTag || !node2.Include))
			{
				node2 = treeIterator.Previous();
			}
			return node2;
		}

		public virtual bool HasExcludeBeforeTextInBranch(Node node)
		{
			for (Node previous = node.Previous; previous != null; previous = previous.Previous)
			{
				if (previous.IsText && !previous.IsWhitespace)
				{
					return false;
				}
				if (previous.MayExclude)
				{
					return !previous.CanHide;
				}
				if (previous.Exclude)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool HasExcludeBoundary(Node textNode, Node currentNode, bool considerAnyExclude = false, bool considerSegmentEnder = false)
		{
			if (textNode.ItemsEqual(currentNode))
			{
				return false;
			}
			Node commonParent = GetCommonParent(textNode, currentNode);
			bool flag = considerAnyExclude ? HasAnyExcludeTagPairInChain(textNode, commonParent) : HasExcludeTagPairInChain(textNode, commonParent);
			bool flag2 = considerAnyExclude ? HasAnyExcludeTagPairInChain(currentNode, commonParent) : HasExcludeTagPairInChain(currentNode, commonParent);
			if (flag | flag2)
			{
				return true;
			}
			TreeIterator treeIterator = new TreeIterator(textNode);
			while (treeIterator.Next() != null && !treeIterator.CurrentNode.ItemsEqual(currentNode))
			{
				if (treeIterator.CurrentNode.Exclude || treeIterator.CurrentNode.LockedContentExclude || (considerSegmentEnder && treeIterator.CurrentNode.IsSegmentEnder))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool HasExcludeTagPairInChain(Node node, Node commonParent)
		{
			if (!node.HasParent)
			{
				return false;
			}
			Node parent = node.Parent;
			while (parent.HasParent && !parent.ItemsEqual(commonParent))
			{
				if (parent.Exclude)
				{
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		public virtual bool HasAnyExcludeTagPairInChain(Node node, Node commonParent)
		{
			if (!node.HasParent)
			{
				return false;
			}
			Node parent = node.Parent;
			while (parent.HasParent && !parent.ItemsEqual(commonParent))
			{
				if (parent.AnyExclude)
				{
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		public virtual bool HasAnyExcludeTagInBranch(Node from, Node upTo)
		{
			Node node = from;
			while (node != null && !node.ItemsEqual(upTo))
			{
				if (node.IsAbstractTag && (node.AnyExclude || node.IsWordStop))
				{
					return true;
				}
				node = node.Next;
			}
			return false;
		}

		public virtual Node GetCommonParent(Node item1, Node item2)
		{
			Node node = item1;
			do
			{
				node = node.Parent;
				Node node2 = item2;
				do
				{
					node2 = node2.Parent;
					if (node == node2)
					{
						return node;
					}
				}
				while (node2 != null && !node2.IsRoot);
			}
			while (node != null && !node.IsRoot);
			return node;
		}

		public virtual void SetStarters()
		{
			if (!SetStarterOnSingleLockedContent() && !SetStarters(handlePlaceholders: false) && !SetStarters(handlePlaceholders: true))
			{
				bool setStarter = false;
				TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
				treeIterator.MoveToLastItemInTree();
				if (HasEnderBetweenNodes(Tree.Root.FirstChild, treeIterator.CurrentNode))
				{
					SetStartersEndersForTagPairs(Tree.Root.FirstChild, ref setStarter);
				}
			}
		}

		public void FixUpAdjacentStarters()
		{
			if (Tree?.Root?.FirstChild == null)
			{
				return;
			}
			Node firstChild = Tree.Root.FirstChild;
			TreeIterator treeIterator = new TreeIterator(firstChild);
			Node node = null;
			while (treeIterator.CurrentNode != null)
			{
				firstChild = treeIterator.CurrentNode;
				if (firstChild.IsSegmentStarter)
				{
					if (node != null)
					{
						firstChild.IsSegmentStarter = false;
						if (firstChild.IsSegmentEnder)
						{
							node = null;
						}
					}
					else
					{
						node = (firstChild.IsSegmentEnder ? null : firstChild);
					}
				}
				else if (firstChild.IsSegmentEnder)
				{
					node = null;
				}
				treeIterator.Next();
			}
		}

		public virtual void SetStartersEndersForTagPairs(Node node, ref bool setStarter, Node currentStarter = null)
		{
			for (Node node2 = node; node2 != null; node2 = node2.Next)
			{
				if (node2.HasChild)
				{
					SetStartersEndersForTagPairs(node2.FirstChild, ref setStarter, currentStarter);
					if (!setStarter && node2.AnyInclude && !HasEnderBetweenNodes(currentStarter, node2))
					{
						node2.IsSegmentStarter = true;
						currentStarter = node2;
					}
				}
				else
				{
					if (node2.Include)
					{
						if (node2.IsFirst)
						{
							node2.IsSegmentStarter = true;
							setStarter = true;
						}
						if (HasExcludeInBranch(node2) || node2.IsLast)
						{
							node2.IsSegmentEnder = true;
						}
					}
					Node parent = node2.Parent;
					Node node3 = node2;
					while (parent != null && parent.Include && node3.IsNonWhiteSpaceText)
					{
						setStarter = true;
						node3.PromoteStarter();
						node3.PromoteEnder();
						node3 = parent;
						parent = parent.Parent;
					}
				}
			}
		}

		public virtual bool HasExcludeInBranch(Node node)
		{
			for (Node next = node.Next; next != null; next = next.Next)
			{
				if (next.Exclude)
				{
					return true;
				}
			}
			for (Node next = node.Previous; next != null; next = next.Previous)
			{
				if (next.Exclude)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool SetStarters(bool handlePlaceholders)
		{
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = treeIterator.CurrentNode;
			bool result = false;
			while (true)
			{
				if (node != null && !node.IsSegmentEnder)
				{
					node = treeIterator.Previous();
					continue;
				}
				if (node == null)
				{
					break;
				}
				Node previousPotentialStarter = GetPreviousPotentialStarter(node, handlePlaceholders);
				if (previousPotentialStarter == null)
				{
					break;
				}
				result = true;
				previousPotentialStarter.IsSegmentStarter = true;
				treeIterator = new TreeIterator(previousPotentialStarter);
				node = treeIterator.Previous();
				if (node == null)
				{
					break;
				}
			}
			return result;
		}

		public virtual Node PreviousStarterAppearsBeforePreviousEnder(Node node)
		{
			TreeIterator treeIterator = new TreeIterator(node);
			while (treeIterator.CurrentNode != null)
			{
				if (treeIterator.CurrentNode.IsSegmentEnder)
				{
					return null;
				}
				if (treeIterator.CurrentNode.IsSegmentStarter)
				{
					return treeIterator.CurrentNode;
				}
				treeIterator.Previous();
			}
			return null;
		}

		public virtual bool HasEnderBetweenNodes(Node firstNode, Node secondNode)
		{
			if (firstNode == null)
			{
				return false;
			}
			TreeIterator treeIterator = new TreeIterator(firstNode);
			while (treeIterator.CurrentNode != secondNode && !treeIterator.CurrentNode.IsSegmentEnder)
			{
				treeIterator.Next();
			}
			if (treeIterator.CurrentNode.Equals(secondNode))
			{
				return treeIterator.CurrentNode.IsSegmentEnder;
			}
			return true;
		}

		public virtual Node GetPreviousPotentialStarter(Node node, bool handlePlaceholders)
		{
			Node node2 = node;
			if (node2.HasPrevious && node2.Previous.IsSegmentEnder)
			{
				return node2;
			}
			bool flag = true;
			do
			{
				Node node3 = (!handlePlaceholders) ? FindPreviousTextNode(node2, allowWhitespace: false) : FindPreviousPlaceholderNode(node2);
				if (node3 == null)
				{
					Node node4 = FindPreviousLockedContentIncludeNode(node2);
					if (node4 == null)
					{
						if ((handlePlaceholders || !node.IsText || node.IsWhitespace) && (!handlePlaceholders || !node.IsPlaceholderTag || !node.Include) && (!node.IsLockedContent || !node.LockedContentInclude || !node.LockedContentHasChild || node.LockedContentOnlyContainsWhitespace))
						{
							break;
						}
						return node;
					}
					node3 = node4;
				}
				Node node5 = (!handlePlaceholders) ? FindPreviousTextNode(node3, allowWhitespace: false) : FindPreviousPlaceholderNode(node3);
				Node node6 = FindPreviousLockedContentIncludeNode(node3);
				if (node5 == null && node6 == null)
				{
					if (HasExcludeBoundary(node3, node2, considerAnyExclude: false, considerSegmentEnder: true))
					{
						return node;
					}
					if (!node3.IsSegmentEnder || !node.IsSegmentEnder)
					{
						return node3;
					}
					if (node.IsText)
					{
						return node;
					}
					node3.IsSegmentEnder = false;
					return node3;
				}
				if (node5 == null)
				{
					node5 = node6;
				}
				bool flag2 = HasExcludeBoundary(node3, node2, considerAnyExclude: false, considerSegmentEnder: true);
				bool flag3 = HasExcludeBoundary(node5, node3, considerAnyExclude: false, considerSegmentEnder: true);
				if (!flag2)
				{
					if (node3.IsSegmentEnder)
					{
						return node;
					}
					if (node5.IsSegmentEnder)
					{
						return node3;
					}
					if (flag3)
					{
						return node3;
					}
					if (node5.ItemsEqual(node3))
					{
						return node3;
					}
					if (node2 == node3)
					{
						flag = false;
					}
					node2 = node3;
					continue;
				}
				return node;
			}
			while (flag);
			return null;
		}

		public virtual Node GetPreviousIncludeStarters(Node previousTextNode)
		{
			if (!previousTextNode.HasPrevious)
			{
				return previousTextNode;
			}
			Node node = previousTextNode.PreviousEnderInBranch;
			if (node == null)
			{
				node = previousTextNode;
				Node result = previousTextNode;
				if (node.BranchIndex > 0)
				{
					while (node.Previous != null && (node.Previous.IsWhitespace || ((node.Previous.IsAbstractTag || node.Previous.IsLockedContent) && (node.Previous.AnyInclude || node.Previous.LockedContentAnyInclude) && !node.Previous.ContainsExcludeInside && !node.Previous.IsWordStop && node.Previous.DescendantStarters <= 0 && node.Previous.DescendantEnders <= 0)))
					{
						node = node.Previous;
						if (node.IsAbstractTag || node.IsLockedContent)
						{
							result = node;
						}
					}
					return result;
				}
			}
			if (node.HasNext)
			{
				node = node.Next;
			}
			if (node != null && node.HasNext && node.Next.IsSegmentStarter)
			{
				if (!HasExcludeBoundary(node, previousTextNode) && !node.AnyExclude && node.Next.IsSegmentStarter && node.HasPrevious && (node.Previous.IsWhitespace || (node.Previous.IsText && char.IsWhiteSpace(node.Previous.Text[node.Previous.Text.Length - 1]))))
				{
					return node;
				}
				if (node.IsLockedContent && node.HasPrevious && node.Previous.IsSegmentEnder && node.LockedContentAnyInclude)
				{
					return node;
				}
			}
			while (node != null && node.HasNext && !node.IsWhitespace && node.LeadingWhitespaceCount == 0 && !node.ItemsEqual(previousTextNode))
			{
				node = node.Next;
			}
			if (node != null && node.HasNext && !node.Next.IsSegmentStarter)
			{
				if (HasExcludeBoundary(node, previousTextNode))
				{
					while (node != null && node.HasNext && !node.IsSegmentStarter && HasExcludeBoundary(node, previousTextNode))
					{
						node = node.Next;
					}
					if (node == null || !node.Exclude)
					{
						return node;
					}
					return previousTextNode;
				}
				if (node.ItemsEqual(previousTextNode))
				{
					return node;
				}
				if (!node.IsWhitespace)
				{
					return node.Next;
				}
				while (HasAnyExcludeTagInBranch(node, previousTextNode))
				{
					Node nextIncludeTagInBranch = node.NextIncludeTagInBranch;
					if (nextIncludeTagInBranch != null && HasAnyExcludeTagInBranch(nextIncludeTagInBranch, previousTextNode))
					{
						node = nextIncludeTagInBranch;
						continue;
					}
					if (nextIncludeTagInBranch == null)
					{
						return previousTextNode;
					}
					node = node.Next;
				}
				return node.Next;
			}
			if (node != null && (!node.IsSegmentStarter || !node.HasPrevious || node.Previous.IsSegmentEnder))
			{
				return previousTextNode;
			}
			Node result2 = previousTextNode;
			if (node == null)
			{
				return result2;
			}
			node = node.Previous;
			while (node != null && !node.IsSegmentEnder && node.HasPrevious)
			{
				if (node.IsPlaceholderTag)
				{
					if (node.Exclude)
					{
						return result2;
					}
					if ((node.AnyInclude || node.ContainsOnlyIncludeInside) && !node.IsWordStop)
					{
						if (!node.HasPrevious || !node.Previous.IsSegmentEnder)
						{
							result2 = node;
						}
						else if (node.HasPrevious && node.Previous.IsSegmentEnder && node.Previous.IsText)
						{
							string text = node.Previous.Text;
							if (text.Length <= 0 || !char.IsWhiteSpace(text[text.Length - 1]))
							{
								return previousTextNode;
							}
							result2 = node;
						}
					}
				}
				node = node.Previous;
			}
			return result2;
		}

		public virtual Node GetPreviousIncludeStartersIncludingContainers(Node previousTextNode)
		{
			if (!previousTextNode.HasPrevious)
			{
				return previousTextNode;
			}
			bool flag = previousTextNode.MayExclude && !previousTextNode.IsText && !previousTextNode.CanHide;
			bool flag2 = false;
			Node previousEnderInBranch = previousTextNode.PreviousEnderInBranch;
			Node node = previousEnderInBranch?.NextWhitespaceItemInBranch;
			Node previous = previousTextNode.Previous;
			Node node2 = previousTextNode;
			while (previous != null && previous.CanIncludeBeforeText)
			{
				if (previous.Include || (previous.IsContainer && previous.MayExclude && previous.ContainsOnlyIncludeInside))
				{
					flag2 = true;
				}
				if (!previous.IsWhitespace && !previous.ContainsOnlyWhitespace && !previous.IsWordStop)
				{
					node2 = previous;
				}
				if (previous.ItemsEqual(node))
				{
					if (previousEnderInBranch != null && HasExcludeBoundary(previousEnderInBranch, node))
					{
						Node node3 = node?.NextIncludeTagInBranch;
						if (node3 != null)
						{
							if (flag && !flag2)
							{
								return previousTextNode;
							}
							return node3;
						}
					}
					if (previousEnderInBranch != null && (!previousEnderInBranch.Next.ItemsEqual(node) || HasExcludeBoundary(node, previousTextNode)))
					{
						return previousTextNode;
					}
					Node node4 = node;
					do
					{
						if (node4 != null)
						{
							node4 = node4.Next;
							if (node4.ItemsEqual(node2))
							{
								return node2;
							}
						}
					}
					while (node4.HasNext && node4.Next.IsWhitespace && !node4.ItemsEqual(previousTextNode));
					return node4;
				}
				if (previous.IsWhitespace && previous.HasPrevious && previous.Previous.IsPlaceholderTag && previous.Previous.HasPrevious && previous.Previous.Previous.IsSegmentEnder)
				{
					return previousTextNode;
				}
				if (previous.IsWhitespace)
				{
					Node node5 = previous;
					while (node5.HasPrevious && node5.Previous.Include && node5.Previous.CanHide && node5.Previous.ContainsOnlyIncludeWithTextContentInside)
					{
						node2 = node5.Previous;
						node5 = node5.Previous;
					}
				}
				previous = previous.Previous;
			}
			if (flag && !flag2)
			{
				return previousTextNode;
			}
			return node2;
		}

		public virtual void ProcessLeadingSpaces(bool moveOutsideContainer)
		{
			TreeIterator treeIterator = new TreeIterator(Tree.Root.FirstChild);
			for (Node node = treeIterator.CurrentNode; node != null; node = treeIterator.Next())
			{
				if (node.IsText && !node.IsWhitespace)
				{
					int leadingWhitespaceCount = node.LeadingWhitespaceCount;
					if (leadingWhitespaceCount > 0 && ShouldPushLeadingWhitespace(node))
					{
						node.SplitText(leadingWhitespaceCount, leadingSpaces: true);
						if (moveOutsideContainer && !node.Next.IsSegmentStarter && PreviousStarterAppearsBeforePreviousEnder(node) == null)
						{
							node.MoveSpaceOutsideContainer(isLeading: true);
						}
					}
				}
			}
			treeIterator = new TreeIterator(Tree.Root.FirstChild);
			for (Node node = treeIterator.CurrentNode; node != null; node = treeIterator.Next())
			{
				if (node.IsContainer && node.IsSegmentStarter && !node.IsSegmentEnder && node.HasChild && node.FirstChild.IsWhitespace && node.FirstChild.BranchCount > 1)
				{
					Node next = node.FirstChild.Next;
					Node node2 = next.Split();
					node2.IsSegmentStarter = true;
					node2.Previous.IsSegmentStarter = false;
				}
			}
		}

		public virtual void ProcessTrailingSpaces(bool moveOutsideContainer)
		{
			if (Tree?.Root?.FirstChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.FirstChild);
			for (Node node = treeIterator.CurrentNode; node != null; node = treeIterator.Next())
			{
				if (node.IsText && !node.IsWhitespace && (node.IsSegmentEnder || node.IsLast))
				{
					int trailingWhitespaceCount = node.TrailingWhitespaceCount;
					if (trailingWhitespaceCount > 0 && ShouldPushTrailingWhitespace(node))
					{
						node.SplitText(node.TrailingWhitespaceSplitIndex, leadingSpaces: false);
						if (moveOutsideContainer && !node.IsSegmentStarter)
						{
							node.MoveSpaceOutsideContainer(isLeading: false);
						}
					}
				}
			}
		}

		private static bool ShouldPushLeadingWhitespace(Node currentNode)
		{
			return !currentNode.IsInsideTagInclude;
		}

		private static bool ShouldPushTrailingWhitespace(Node currentNode)
		{
			return !currentNode.IsInsideTagInclude;
		}

		public virtual void HandleContainerSplits()
		{
			HandleExcludeSplits();
			HandleStarterSplits();
			HandleEnderSplilts();
			HandleStarterEnderPairsInContainerSplits();
			HandleStarterEnderGroupingsContainerSplits();
		}

		public virtual void HandleExcludeSplits()
		{
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			treeIterator.PreviousExclude();
			while (treeIterator.CurrentNode != null)
			{
				Node node = treeIterator.CurrentNode;
				bool flag = true;
				if (node != null && node.HasParent && node.Parent.AnyInclude && !node.Parent.IsRoot && (node.EndersInBranchCount >= 1 || (node.HasNext && node.Next.IsSegmentStarter)))
				{
					if (node.HasNext)
					{
						Node node2 = node.Next;
						while (node2 != null && node2.IsWhitespace)
						{
							node2 = node2.Next;
						}
						if (node2 != null)
						{
							while (!node2.Parent.IsRoot && !node2.Parent.AnyExclude)
							{
								node2 = node2.Split();
								node2.IsSegmentStarter = node.Parent.IsSegmentStarter;
								if (node.BranchOnlyContainsExludes)
								{
									node.Parent.IsSegmentStarter = false;
									node.Parent.IsSegmentEnder = false;
								}
								treeIterator = new TreeIterator(node2);
							}
						}
					}
					if (node.HasPrevious || node.Parent.HasPrevious)
					{
						if (!node.HasPrevious)
						{
							node = node.Parent;
						}
						while (node.HasPrevious && (node.Previous.IsWhitespace || node.Previous.Exclude))
						{
							node = node.Previous;
						}
						Node node3 = null;
						if (node.EndersInBranchCount >= 1)
						{
							node3 = node.Split();
						}
						if (node3 != null && node3.Previous.HasPrevious)
						{
							Node node4 = PreviousStarterAppearsBeforePreviousEnder(node3.Previous.Previous);
							if (node4 != null)
							{
								node3.Previous.IsSegmentStarter = false;
							}
						}
						if (node3 != null)
						{
							node3.IsSegmentStarter = false;
						}
						if (node3 == null)
						{
							break;
						}
						treeIterator = new TreeIterator(node3);
						treeIterator.Previous();
					}
				}
				else
				{
					treeIterator.Previous();
					flag = false;
				}
				if (flag)
				{
					treeIterator.Previous();
				}
				treeIterator.PreviousExclude();
			}
		}

		public virtual void HandleStarterSplits()
		{
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = treeIterator.CurrentNode;
			do
			{
				IL_002d:
				if (node != null && !node.IsSegmentEnder)
				{
					node = treeIterator.Previous();
					goto IL_002d;
				}
				Node node2 = node;
				while (node != null && !node.IsSegmentStarter)
				{
					node = treeIterator.Previous();
				}
				Node node3 = node;
				if (node3 == null)
				{
					break;
				}
				if (node3.IsFirst && node3.HasParent && !node3.Parent.Equals(node2.Parent) && node3.Depth > node2.Depth)
				{
					node3.IsSegmentStarter = false;
					node3.Parent.IsSegmentStarter = true;
					node3 = node3.Parent;
				}
				Node commonParent = GetCommonParent(node3, node2);
				int num = Math.Abs(node3.Depth - commonParent.Depth);
				int num2 = Math.Abs(node2.Depth - commonParent.Depth);
				int num3 = Math.Abs(num - num2);
				if ((node3.BranchCount == 1 && num3 != 0) || HasPreviousSiblingInChain(node3, commonParent) || (node3.HasParent && node3.Parent.Include && node3.IsNodeSurroundedByWhitespaceInBranch))
				{
					while (((!node3.HasPrevious && node3.HasParent && !node3.Parent.IsRoot) || (node3.HasParent && node3.Parent.Include && node3.IsNodeSurroundedByWhitespaceInBranch)) && !node3.Parent.Contains(node2))
					{
						node3 = node3.Parent;
					}
					num = Math.Abs(node3.Depth - commonParent.Depth);
				}
				if (commonParent.IsRoot)
				{
					Node ancestorNodeBelowRoot = node3.GetAncestorNodeBelowRoot();
					if (ancestorNodeBelowRoot.ContainsOnlyIncludeContentInside && ancestorNodeBelowRoot.DescendantStarters == 1 && ancestorNodeBelowRoot.DescendantEnders == 0)
					{
						ancestorNodeBelowRoot.IsSegmentStarter = true;
						node3.IsSegmentStarter = false;
						node = treeIterator.Previous();
						continue;
					}
				}
				if ((num > 1 && node3.BranchCount != 1 && node3.BranchIndex != 0) || (HasPreviousSiblingInChain(node3, commonParent) && node3.IsSegmentStarterEnderPair && !commonParent.IsRoot && node3.PreviousNonWhitespace.IsSegmentEnder && commonParent.DescendantStarters != commonParent.DescendantEnders && !node3.PreviousNonWhitespace.IsSegmentStarterEnderPair))
				{
					int num4 = node3.IsSegmentStarterEnderPair ? num : (num - 1);
					if (commonParent.IncludeWithText && HasCanHideOrIncludeOnlyTagPairsInChain(node3, commonParent))
					{
						num4++;
					}
					Node node4 = node3;
					if ((node4.HasPrevious && node4.Previous.IsWhitespace && node4.Previous.HasPrevious && (node4.Parent.AnyInclude || node4.Parent.MayExclude)) || (node4.HasParent && node4.Include && node4.IsText && node4.HasPrevious && node4.Previous.IsWhitespace && node4.Previous.IsFirst))
					{
						node4 = node4.Previous;
					}
					for (int i = 0; i < num4; i++)
					{
						node4 = node4.Split(promoteDataToParent: true);
					}
					treeIterator = new TreeIterator(node4);
					node = treeIterator.CurrentNode;
				}
				else
				{
					node = treeIterator.Previous();
				}
			}
			while (node != null);
		}

		private static bool HasCanHideOrIncludeOnlyTagPairsInChain(Node node, Node commonParent)
		{
			if (!node.HasParent)
			{
				return false;
			}
			Node parent = node.Parent;
			while (parent.HasParent && !parent.ItemsEqual(commonParent))
			{
				if (parent.CanHide || parent.AnyInclude)
				{
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		public virtual bool HasPreviousSiblingInChain(Node starter, Node commonParent)
		{
			while (!starter.ItemsEqual(commonParent))
			{
				if (starter.HasPrevious)
				{
					return true;
				}
				starter = starter.Parent;
			}
			return false;
		}

		public virtual void HandleStarterEnderPairsInContainerSplits()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = treeIterator.CurrentNode.IsSegmentEnder ? treeIterator.CurrentNode : null;
			Node node2 = treeIterator.CurrentNode.IsSegmentStarter ? treeIterator.CurrentNode : null;
			while (treeIterator.CurrentNode != null)
			{
				Node node3 = node ?? treeIterator.PreviousEnder();
				node = null;
				if (node3 == null)
				{
					break;
				}
				Node node4 = node2 ?? treeIterator.PreviousStarter();
				node2 = null;
				if (node4 == null)
				{
					break;
				}
				Node topLevelIncludeNodeInAncestorBranch = node4.TopLevelIncludeNodeInAncestorBranch;
				if ((topLevelIncludeNodeInAncestorBranch != null && topLevelIncludeNodeInAncestorBranch.ContainsOnlySegmentStarterEnderPairs && !topLevelIncludeNodeInAncestorBranch.ContainsSingleSegmentStarterEnderPair && node4.HasParent && (node4.IsInsideCommentContainer || node4.IsInsideRevisionContainer || node4.IsInsideTagAnyIncludeContainer) && node4.Parent.Contains(node3)) || (node4.HasParent && node4.Parent.CanHide))
				{
					topLevelIncludeNodeInAncestorBranch = node4.TopLevelIncludeNodeInAncestorBranch;
					bool flag = false;
					if (topLevelIncludeNodeInAncestorBranch == null)
					{
						Node node5 = node4.IsolatedTopLevelCanHideNode;
						if (node5 == null && node4.Parent.CanHide)
						{
							node5 = node4.Parent;
						}
						if (node5 == null || node4.BranchCount <= 0 || !node4.ItemsEqual(node3) || !CanIncludeNextEnderOrPreviousStarterData(node5, node5))
						{
							treeIterator.Previous();
							continue;
						}
						flag = true;
						topLevelIncludeNodeInAncestorBranch = node5;
					}
					int containersContainingStarterEnderPairsOrStarterEnderPairCount = node4.ContainersContainingStarterEnderPairsOrStarterEnderPairCount;
					if (containersContainingStarterEnderPairsOrStarterEnderPairCount > 0 && node4.Parent.DescendantEnders > 1)
					{
						Node node6 = node3;
						Node node7 = null;
						for (int i = 0; i < containersContainingStarterEnderPairsOrStarterEnderPairCount; i++)
						{
							if (node6 == null)
							{
								continue;
							}
							Node previousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch = node6.PreviousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
							if (previousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch == null && i != containersContainingStarterEnderPairsOrStarterEnderPairCount - 1)
							{
								break;
							}
							if (node6.IsContainer)
							{
								node7 = ((!node6.HasChild || !node6.ContainsSingleSegmentStarterEnderPair) ? SplitContainerWithStarterEnderPairs(node6) : DoUpToTopLevelIncludeParentSplits(node6));
							}
							else if (i < containersContainingStarterEnderPairsOrStarterEnderPairCount - 1 || containersContainingStarterEnderPairsOrStarterEnderPairCount == 1)
							{
								Node node8 = node6.NodeAfterPreviousEnderInBranchOrCurrentNode;
								if (node8.ContainsExcludeInside && node6.IsSegmentStarterEnderPair)
								{
									node8 = node6;
								}
								while ((node8.IsWhitespaceToStartOfBranch && node8.IsWhitespaceToEndOfBranch) || node8.BranchCount == 1)
								{
									node8 = node8.Parent;
								}
								if ((node8.IsFirst || node8.IsWhitespaceToStartOfBranch) && node8.HasNext)
								{
									node8 = node8.Next;
								}
								if (node8.IsSegmentEnder)
								{
									while (node8.HasNext && !node8.IsSegmentStarter)
									{
										node8 = node8.Next;
									}
								}
								node7 = node8.Split();
								DoUpToTopLevelIncludeParentSplits(node7);
							}
							node6 = previousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
						}
						if (node7 != null)
						{
							treeIterator = new TreeIterator(node7);
						}
						else if (node4 != null)
						{
							treeIterator = new TreeIterator(node4);
						}
						if (flag)
						{
							treeIterator = new TreeIterator(topLevelIncludeNodeInAncestorBranch);
						}
					}
					else
					{
						Node node9 = DoUpToTopLevelIncludeParentSplits(node4);
						if (node9 != null)
						{
							topLevelIncludeNodeInAncestorBranch = node9.TopLevelIncludeNodeInAncestorBranch;
							treeIterator = ((topLevelIncludeNodeInAncestorBranch != null) ? new TreeIterator(topLevelIncludeNodeInAncestorBranch) : new TreeIterator(node9));
						}
						else
						{
							Node nonIsolatedTopLevelIncludeNodeInAncestorBranch = node4.NonIsolatedTopLevelIncludeNodeInAncestorBranch;
							treeIterator = new TreeIterator(nonIsolatedTopLevelIncludeNodeInAncestorBranch);
						}
					}
				}
				else if (topLevelIncludeNodeInAncestorBranch != null && topLevelIncludeNodeInAncestorBranch.DescendantStarters == topLevelIncludeNodeInAncestorBranch.DescendantEnders && node4.HasParent && (node4.IsInsideCommentContainer || node4.IsInsideRevisionContainer || node4.IsInsideTagAnyIncludeContainer) && node4.ItemsEqual(node3))
				{
					Node node10 = DoUpToTopLevelIncludeParentSplits(node4);
					topLevelIncludeNodeInAncestorBranch = node10?.TopLevelIncludeNodeInAncestorBranch;
					treeIterator = ((topLevelIncludeNodeInAncestorBranch != null) ? new TreeIterator(topLevelIncludeNodeInAncestorBranch) : new TreeIterator(node10));
				}
				treeIterator.Previous();
			}
		}

		private static Node SplitContainerWithStarterEnderPairs(Node node)
		{
			if (!node.HasChild)
			{
				return node;
			}
			Node node2 = node.LastChild;
			int containersContainingStarterEnderPairsOrStarterEnderPairCount = node2.ContainersContainingStarterEnderPairsOrStarterEnderPairCount;
			if (!node2.IsSegmentStarterEnderPair && !node2.ContainsOnlySegmentStarterEnderPairs)
			{
				node2 = node2.PreviousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
			}
			Node node3 = null;
			if (containersContainingStarterEnderPairsOrStarterEnderPairCount <= 0)
			{
				return null;
			}
			for (int i = 0; i < containersContainingStarterEnderPairsOrStarterEnderPairCount; i++)
			{
				if (node2 == null)
				{
					break;
				}
				Node previousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch = node2.PreviousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
				if (node2.IsContainer)
				{
					node3 = (node2.ContainsSingleSegmentStarterEnderPair ? DoUpToTopLevelIncludeParentSplits(node2) : SplitContainerWithStarterEnderPairs(node2));
				}
				else if (i < containersContainingStarterEnderPairsOrStarterEnderPairCount - 1)
				{
					node3 = node2.NodeAfterPreviousEnderInBranchOrCurrentNode.Split();
					DoUpToTopLevelIncludeParentSplits(node3);
				}
				else
				{
					DoUpToTopLevelIncludeParentSplits(node2);
				}
				node2 = previousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
			}
			return node3;
		}

		private static Node DoUpToTopLevelIncludeParentSplits(Node workingNode)
		{
			if (workingNode == null)
			{
				return null;
			}
			Node node = workingNode.NonIsolatedTopLevelIncludeNodeInAncestorBranch ?? (workingNode.IsContainer ? workingNode : workingNode.Parent);
			if (node == null || !node.IsContainer)
			{
				return workingNode;
			}
			Node topLevelIncludeContainer = GetTopLevelIncludeContainer(node);
			if (topLevelIncludeContainer == null || (topLevelIncludeContainer.DescendantStarters <= 1 && !topLevelIncludeContainer.ContainsExcludeInside))
			{
				return workingNode;
			}
			int num = node.Depth - topLevelIncludeContainer.Depth;
			Node node2 = null;
			if (node.HasNext)
			{
				node2 = node.Next;
				for (int i = 0; i < num; i++)
				{
					node2 = node2.Split();
				}
			}
			if (node.HasPrevious)
			{
				node2 = node;
				for (int j = 0; j < num; j++)
				{
					node2 = node2.Split();
				}
			}
			if (num != 0 || (!workingNode.IsSegmentStarterEnderPair && !workingNode.ContainsSingleSegmentStarterEnderPair) || workingNode.BranchCount <= 1)
			{
				return node2;
			}
			node2 = workingNode;
			if (node2.IsFirst)
			{
				return node2;
			}
			num = workingNode.Depth - node.Depth;
			for (int k = 0; k < num; k++)
			{
				node2 = node2.Split();
			}
			return node2;
		}

		private void HandleStarterEnderGroupingsContainerSplits()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = treeIterator.CurrentNode;
			do
			{
				IL_004e:
				if (node != null && !node.IsSegmentEnder)
				{
					node = treeIterator.Previous();
					goto IL_004e;
				}
				Node currentEnder = node;
				while (node != null && !node.IsSegmentStarter)
				{
					node = treeIterator.Previous();
				}
				Node currentStarter = node;
				if (currentStarter == null)
				{
					break;
				}
				if (currentStarter.Depth != currentEnder.Depth || !IsSegmentUnit(currentStarter, currentEnder))
				{
					PromoteStarterEnderToCommonLevel(ref currentStarter, ref currentEnder);
				}
				if (IsSegmentUnit(currentStarter, currentEnder) && currentStarter.Parent != null && !currentStarter.Parent.Exclude && currentStarter.Parent.MayExclude && currentStarter.Parent.IsInsideIncludeContainer)
				{
					while (currentStarter?.Parent != null && currentEnder.Parent != null && !currentStarter.Parent.IsRoot && currentStarter.Parent.ItemsEqual(currentEnder.Parent) && !currentStarter.Parent.Exclude && currentStarter.Parent.DescendantStarters == 1 && !currentStarter.Include)
					{
						currentStarter.IsSegmentStarter = false;
						currentEnder.IsSegmentEnder = false;
						currentStarter = currentStarter.Parent;
						currentEnder = currentEnder.Parent;
						currentStarter.IsSegmentStarter = true;
						currentEnder.IsSegmentEnder = true;
					}
				}
				if (currentEnder.HasNext)
				{
					Node topLevelIncludeContainer = GetTopLevelIncludeContainer(currentStarter);
					Node topLevelIncludeContainer2 = GetTopLevelIncludeContainer(currentEnder.Next);
					if (topLevelIncludeContainer != null && topLevelIncludeContainer2 != null && topLevelIncludeContainer.ItemsEqual(topLevelIncludeContainer2) && topLevelIncludeContainer.DescendantStarters > 1)
					{
						Node node2 = currentEnder.Next;
						int depth = topLevelIncludeContainer2.Depth;
						while (node2.Depth > depth)
						{
							node2 = node2.NodeAfterPreviousEnderInBranchOrCurrentNode.Split(promoteDataToParent: true);
						}
					}
				}
				if (IsSegmentUnit(currentStarter, currentEnder) && !currentStarter.IsSegmentStarterEnderPair)
				{
					Node ancestorNodeBelowRoot = currentStarter.GetAncestorNodeBelowRoot();
					if (ancestorNodeBelowRoot != null && currentStarter.Depth == currentEnder.Depth && ancestorNodeBelowRoot == currentEnder.GetAncestorNodeBelowRoot() && ancestorNodeBelowRoot.DescendantStarters == 1 && ancestorNodeBelowRoot.DescendantEnders == 1 && ancestorNodeBelowRoot.Include && ancestorNodeBelowRoot.ContainsOnlyIncludeContentInside)
					{
						currentStarter.PromoteStarter();
						currentEnder.PromoteEnder();
						node = treeIterator.Previous();
						continue;
					}
					if ((currentStarter.IsInsideAnyIncludeOrCanHideContainer && (!currentStarter.MayExclude || !currentStarter.IsTagPair || !currentStarter.Parent.IncludeWithText) && currentStarter.HasPrevious && !currentStarter.IsWhitespaceToStartOfBranch && currentEnder.HasNext && !currentEnder.IsWhitespaceToEndOfBranch) || (currentStarter.IsInsideIncludeWithTextContainerWithMayExcludeInChain && (currentStarter.IsText || currentEnder.IsText)))
					{
						Node node3 = currentStarter;
						Node node4 = currentEnder.Next;
						if (node3.Root.FirstChild.BranchCount != 1)
						{
							while (!node3.Parent.IsRoot && node3.IsInsideAnyIncludeOrCanHideContainer && node4 != null)
							{
								node4 = node4.Split(promoteDataToParent: true);
								node3 = node3.NodeAfterPreviousEnderInBranchOrCurrentNode.Split(promoteDataToParent: true);
							}
						}
						if (!node3.ItemsEqual(currentStarter))
						{
							treeIterator = new TreeIterator(node3);
						}
					}
					else if ((currentStarter.IsInsideAnyIncludeOrCanHideContainer && (!currentStarter.MayExclude || !currentStarter.IsTagPair || !currentStarter.Parent.IncludeWithText) && currentStarter.HasPrevious && !currentStarter.IsWhitespaceToStartOfBranch) || (currentStarter.IsInsideIncludeWithTextContainerWithMayExcludeInChain && (currentStarter.IsText || currentEnder.IsText)))
					{
						Node node5 = currentStarter;
						while (!node5.Parent.IsRoot && (node5.IsInsideTagIncludeContainerWithMayExcludesInChain || node5.Parent.AnyInclude))
						{
							node5 = node5.NodeAfterPreviousEnderInBranchOrCurrentNode.Split(promoteDataToParent: true);
						}
						if (!node5.ItemsEqual(currentStarter))
						{
							treeIterator = new TreeIterator(node5);
						}
					}
					else if ((currentEnder.IsInsideAnyIncludeOrCanHideContainer && (!currentEnder.HasNext || !currentEnder.Next.MayExclude) && (!currentEnder.MayExclude || !currentEnder.IsTagPair || !currentEnder.Parent.IncludeWithText) && currentEnder.HasNext && !currentEnder.IsWhitespaceToEndOfBranch) || (currentStarter.IsInsideIncludeWithTextContainerWithMayExcludeInChain && (currentStarter.IsText || currentEnder.IsText)))
					{
						Node node6 = currentEnder.Next;
						while (!node6.Parent.IsRoot && ((node6.IsInsideAnyIncludeOrCanHideContainer && CanIncludeNextEnderOrPreviousStarterData(currentStarter.Parent, currentEnder.Parent)) || node6.IsInsideTagIncludeContainerWithMayExcludesInChain || node6.Parent.AnyInclude))
						{
							node6 = node6.NodeAfterPreviousEnderInBranchOrCurrentNode.Split(promoteDataToParent: true);
						}
						if (!node6.Equals(currentEnder.Next))
						{
							treeIterator = new TreeIterator(node6);
						}
					}
				}
				node = treeIterator.Previous();
			}
			while (node != null);
		}

		private static void PromoteStarterEnderToCommonLevel(ref Node currentStarter, ref Node currentEnder)
		{
			bool flag = IsSegmentUnit(currentStarter, currentEnder);
			bool flag2 = false;
			while (!flag2 && !flag && currentStarter != null && currentEnder != null)
			{
				flag2 = true;
				while (!currentStarter.Parent.IsRoot && currentStarter.Depth > currentEnder.Depth)
				{
					currentStarter.PromoteStarter();
					currentStarter = currentStarter.Parent;
					flag2 = false;
				}
				while (!currentEnder.Parent.IsRoot && currentEnder.Depth > currentStarter.Depth)
				{
					currentEnder.PromoteEnder();
					currentEnder = currentEnder.Parent;
					flag2 = false;
				}
				while (!currentStarter.Parent.IsRoot && !currentEnder.Parent.IsRoot && currentStarter != null && currentEnder != null && !IsSegmentUnit(currentStarter, currentEnder))
				{
					currentStarter.PromoteStarter();
					currentStarter = currentStarter.Parent;
					currentEnder.PromoteEnder();
					currentEnder = currentEnder.Parent;
					flag2 = false;
				}
				if (IsSegmentUnit(currentStarter, currentEnder) || (currentStarter.Depth == currentEnder.Depth && currentStarter.Parent.IsRoot && currentEnder.Parent.IsRoot))
				{
					flag = true;
				}
			}
		}

		private static bool IsSegmentUnit(Node starter, Node ender)
		{
			if (!starter.IsSegmentStarter || !ender.IsSegmentEnder || starter.DescendantStarters != 0 || starter.DescendantEnders != 0 || ender.DescendantStarters != 0 || ender.DescendantEnders != 0)
			{
				return false;
			}
			while (ender != null && ender.HasPrevious && !ender.ItemsEqual(starter) && ender.DescendantStarters == 0 && ender.DescendantEnders == 0)
			{
				ender = ender.Previous;
			}
			return starter.ItemsEqual(ender);
		}

		public virtual void HandleEnderSplilts()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = treeIterator.CurrentNode;
			do
			{
				IL_004e:
				if (node != null && !node.IsSegmentEnder)
				{
					node = treeIterator.Previous();
					goto IL_004e;
				}
				Node node2 = node;
				if (node2 == null)
				{
					break;
				}
				while (node != null && !node.IsSegmentStarter)
				{
					node = treeIterator.Previous();
				}
				Node node3 = node;
				if (node3 == null)
				{
					break;
				}
				Node commonParent = GetCommonParent(node3, node2);
				int num = Math.Abs(node2.Depth - commonParent.Depth);
				int num2 = Math.Abs(node3.Depth - commonParent.Depth);
				int num3 = Math.Abs(num2 - num);
				if ((node2.BranchCount == 1 && num3 != 0) || HasNextSiblingInChain(node2, commonParent))
				{
					while ((!node2.HasNext || (node2.HasParent && node2.Parent.Include && node2.IsText && node2.Next.IsWhitespace && node2.Next.IsLast)) && node2.HasParent && !node2.Parent.IsRoot && !node2.Parent.Contains(node3))
					{
						node2 = node2.Parent;
					}
					num = Math.Abs(node2.Depth - commonParent.Depth);
				}
				if (commonParent.IsRoot)
				{
					Node ancestorNodeBelowRoot = node2.GetAncestorNodeBelowRoot();
					if (ancestorNodeBelowRoot.ContainsOnlyIncludeContentInside && ancestorNodeBelowRoot.DescendantStarters == 0 && ancestorNodeBelowRoot.DescendantEnders == 1)
					{
						ancestorNodeBelowRoot.IsSegmentEnder = true;
						node2.IsSegmentEnder = false;
						node = treeIterator.Previous();
						continue;
					}
				}
				if (num > 1 && HasExcludeBetweenNodes(ref node2, commonParent) && !node3.Contains(node2))
				{
					int num4 = Math.Abs(node2.Depth - commonParent.Depth) - 1;
					Node node4 = node2.Next;
					if (node2.HasParent && node2.Parent.Include && node2.HasNext && node2.Next.IsWhitespace && node2.Next.IsLast)
					{
						node4 = node2;
					}
					for (int i = 0; i < num4; i++)
					{
						node4 = node4.Split(promoteDataToParent: true);
					}
					treeIterator = new TreeIterator(node4);
				}
				node = treeIterator.Previous();
			}
			while (node != null);
		}

		public virtual bool HasNextSiblingInChain(Node ender, Node commonParent)
		{
			while (!ender.ItemsEqual(commonParent))
			{
				if (ender.HasNext)
				{
					return true;
				}
				ender = ender.Parent;
			}
			return false;
		}

		public virtual bool HasExcludeBetweenNodes(ref Node node, Node commonParent)
		{
			if (node.HasNext)
			{
				return true;
			}
			TreeIterator treeIterator = new TreeIterator(node);
			while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth - 1 > commonParent.Depth)
			{
				if (treeIterator.CurrentNode.IsPlaceholderTag && treeIterator.CurrentNode.MayExclude && !treeIterator.CurrentNode.CanHide)
				{
					node = treeIterator.CurrentNode.Previous;
					return true;
				}
				treeIterator.Next();
			}
			return false;
		}

		public virtual void DoFinalPromotion()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node node = treeIterator.CurrentNode;
			Node node2 = null;
			Node node3 = null;
			while (node != null && !node.IsRoot)
			{
				if (node.IsSegmentEnder)
				{
					node2 = node;
					node3 = null;
				}
				if (node.IsSegmentStarter)
				{
					node3 = node;
				}
				if (node3 != null && node2 != null && !HasExcludeBoundary(node3, node2))
				{
					while (node3.Depth > node2.Depth)
					{
						node3.PromoteStarter();
						node3 = node3.Parent;
					}
					while (node2.Depth > node3.Depth)
					{
						TreeIterator treeIterator2 = new TreeIterator(node2);
						Node node4 = treeIterator2.NextStarter();
						if (node4 != null && node3.Contains(node4))
						{
							node2 = null;
							break;
						}
						node2.PromoteEnder();
						node2 = node2.Parent;
					}
					if (node3.Equals(node2))
					{
						while (node3.HasParent && !node3.Parent.IsRoot && ((node3.IsInsideAnyIncludeContainer && !node3.IsInsideIncludeWithTextContainerWithMayExcludeInChain) || (node3.Parent.CanHide && node3.NonWhitespaceTextItemsInBranchCount == 1 && (node3.IsInsideAnyIncludeContainer || CanIncludeNextEnderOrPreviousStarterData(node3.Parent, node2.Parent)))) && !node3.Parent.ContainsExcludeInside && !node3.Parent.Exclude && !node3.Parent.IsRoot && (!node3.IsNodeSurroundedByWhitespaceInBranch || node3.IsInsideAnyIncludeContainer) && (!node3.Parent.CanHide || node3.Parent.Depth != 1 || (node3.Parent.DescendantStarters == 1 && node3.Parent.DescendantEnders == 1 && node3.Parent.BranchCount != 1)))
						{
							node3.PromoteStarter();
							node3 = node3.Parent;
							node2.PromoteEnder();
							node2 = node2.Parent;
						}
					}
					if (IsInSameContainer(node3, node2) && !node3.HasPrevious && !node2.HasNext)
					{
						if (node3.Parent.AnyInclude || (node3.Parent.CanHide && node3.Parent.Depth == 1 && node3.Parent.BranchCount > 1 && (node3.IsInsideAnyIncludeContainer || CanIncludeNextEnderOrPreviousStarterData(node3.Parent, node2.Parent)) && !node3.Parent.ContainsExcludeInside))
						{
							Node topLevelIncludeContainer = GetTopLevelIncludeContainer(node3);
							while (!node3.ItemsEqual(topLevelIncludeContainer) && (node3.Parent.AnyInclude || (node3.Parent.CanHide && (node3.IsInsideAnyIncludeContainer || CanIncludeNextEnderOrPreviousStarterData(node3.Parent, node2.Parent)) && !node3.Parent.ContainsExcludeInside)) && (!ContainsMayExcludeBetweenParentAndChild(node3.Parent, node3) || !node3.Parent.IncludeWithText))
							{
								node3.PromoteStarter();
								node3 = node3.Parent;
								node2.PromoteEnder();
								node2 = node2.Parent;
							}
						}
						else
						{
							Node topLevelIncludeContainer2 = GetTopLevelIncludeContainer(node3);
							if (topLevelIncludeContainer2 != null && !topLevelIncludeContainer2.ContainsExcludeInside && !topLevelIncludeContainer2.ItemsEqual(node3) && (!topLevelIncludeContainer2.IncludeWithText || !node3.Parent.MayExclude))
							{
								Node previousIncludeStarters = GetPreviousIncludeStarters(topLevelIncludeContainer2);
								Node nextIncludeEndersIncludingWhitespace = GetNextIncludeEndersIncludingWhitespace(topLevelIncludeContainer2);
								previousIncludeStarters.IsSegmentStarter = true;
								nextIncludeEndersIncludingWhitespace.IsSegmentEnder = true;
								node3.IsSegmentStarter = false;
								node2.IsSegmentEnder = false;
							}
						}
					}
					if (node3 != null && node2 != null)
					{
						Node commonParent = GetCommonParent(node3, node2);
						if ((node3.Parent.MayExclude || node3.Parent.AnyInclude || node3.Parent.CanHide) && (node2.Parent.MayExclude || node2.Parent.AnyInclude || node2.Parent.CanHide) && !StarterAppearsBeforeEnderInBranch(node2))
						{
							if (commonParent.AnyInclude && !commonParent.ContainsExcludeInside)
							{
								bool flag = ContainsMayExcludeBetweenParentAndChild(commonParent, node3);
								while (!node3.ItemsEqual(commonParent) && (!flag || !StarterAppearsBeforeEnderInBranch(node2)))
								{
									node3.PromoteStarter();
									node3 = node3.Parent;
									node2.PromoteEnder();
									node2 = node2.Parent;
								}
							}
							else
							{
								while (!StarterAppearsBeforeEnderInBranch(node2))
								{
									node3.PromoteStarter();
									node3 = node3.Parent;
									node2.PromoteEnder();
									node2 = node2.Parent;
								}
							}
						}
					}
					node3 = null;
					node2 = null;
				}
				else if (node3 != null)
				{
					if ((node3.Parent.IsInsideAnyIncludeOrCanHideContainer || node3.Parent.AnyInclude) && (node3.Parent.Depth != 1 || !node3.Parent.CanHide) && (node3.IsFirst || node3.IsWhitespaceToStartOfBranch) && (node3.Parent.LastChild.IsSegmentEnder || node3.Parent.DescendantEnders == 1 || (node3.Parent.IsSegmentEnder && node3.Parent.DescendantEnders == 0)))
					{
						while (node3.HasParent && !node3.Parent.IsRoot && !node3.Parent.ContainsExcludeInside && (node3.Parent.AnyInclude || node3.Parent.MayExclude) && (node3.IsText || !node3.Parent.IncludeWithText) && (node3.BranchCount <= 1 || (node3.Parent.DescendantEnders == 0 && node3.Parent.IsSegmentEnder && node3.Parent.DescendantStarters == 1) || node3.IsNodeSurroundedByWhitespaceInBranch))
						{
							node3.PromoteStarter();
							node3 = node3.Parent;
						}
					}
					node2 = null;
				}
				else if (node2 != null && (node2.Parent.IsInsideAnyIncludeOrCanHideContainer || node2.Parent.AnyInclude) && (node2.Parent.Depth != 1 || !node2.Parent.CanHide) && (node2.IsLast || node2.IsWhitespaceToEndOfBranch) && (node2.Parent.FirstChild.IsSegmentStarter || node2.Parent.DescendantStarters == 1))
				{
					bool flag2 = true;
					if (node2.Parent.CanHide)
					{
						Node previousStarterInBranch = node2.PreviousStarterInBranch;
						if (previousStarterInBranch != null && IsSegmentUnit(previousStarterInBranch, node2))
						{
							flag2 = false;
						}
					}
					if ((ContainsMayExcludeBetweenParentAndChild(node2.Parent, node2) && node2.Parent.IncludeWithText) || node2.Parent.MayExclude)
					{
						flag2 = false;
					}
					if (flag2)
					{
						while (node2.HasParent && !node2.Parent.IsRoot && !node2.Parent.ContainsExcludeInside && (node2.Parent.AnyInclude || node2.Parent.MayExclude) && (node2.IsText || !node2.Parent.IncludeWithText) && (node2.BranchCount <= 1 || (node2.Parent.DescendantStarters == 1 && node2.Parent.DescendantStarters == node2.Parent.DescendantEnders) || node2.IsNodeSurroundedByWhitespaceInBranch))
						{
							node2.PromoteEnder();
							node2 = node2.Parent;
						}
					}
				}
				if (node.HasNext && node.IsSegmentStarter && !node.IsSegmentEnder && node.Next.IsSegmentStarter)
				{
					node.Next.IsSegmentStarter = false;
				}
				node = treeIterator.Previous();
			}
			FixUpAdjacentStarters();
			PromoteSingleSiblingStartersEnders();
			PromoteSingleStarterEnderToTopLevelCanHide();
		}

		private void PromoteSingleStarterEnderToTopLevelCanHide()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild, visitChildrenOfCurrentNode: true);
			Node node = treeIterator.CurrentNode.IsSegmentStarter ? treeIterator.CurrentNode : null;
			while (treeIterator.CurrentNode != null)
			{
				Node node2 = node ?? treeIterator.PreviousStarter();
				node = null;
				if (node2 == null)
				{
					break;
				}
				Node topLevelCanHideWithOneStarterEnderPair = node2.TopLevelCanHideWithOneStarterEnderPair;
				if (topLevelCanHideWithOneStarterEnderPair != null && CanIncludeNextEnderOrPreviousStarterData(topLevelCanHideWithOneStarterEnderPair, topLevelCanHideWithOneStarterEnderPair))
				{
					topLevelCanHideWithOneStarterEnderPair.ResetStartersAndEndersInContainer();
					topLevelCanHideWithOneStarterEnderPair.IsSegmentStarter = true;
					topLevelCanHideWithOneStarterEnderPair.IsSegmentEnder = true;
					if (topLevelCanHideWithOneStarterEnderPair.HasPrevious)
					{
						treeIterator = new TreeIterator(topLevelCanHideWithOneStarterEnderPair.Previous, visitChildrenOfCurrentNode: true);
					}
					else
					{
						if (!topLevelCanHideWithOneStarterEnderPair.HasParent || !topLevelCanHideWithOneStarterEnderPair.Parent.HasPrevious)
						{
							break;
						}
						treeIterator = new TreeIterator(topLevelCanHideWithOneStarterEnderPair.Parent.Previous, visitChildrenOfCurrentNode: true);
					}
				}
				treeIterator.Previous();
			}
		}

		private bool CanIncludeNextEnderOrPreviousStarterData(Node starter, Node ender)
		{
			return (GetPreviousIncludeStartersIncludingContainers(starter) != null && !GetPreviousIncludeStartersIncludingContainers(starter).ItemsEqual(starter)) || (GetNextIncludeEndersIncludingWhitespace(ender) != null && !GetNextIncludeEndersIncludingWhitespace(ender).ItemsEqual(ender));
		}

		public void GetInitialIncludesWithStartersAndEnders()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild, visitChildrenOfCurrentNode: true);
			Node currentNode = treeIterator.CurrentNode;
			Node node = null;
			Node node2 = null;
			while (currentNode != null && !currentNode.IsRoot)
			{
				if (currentNode.IsSegmentEnder)
				{
					node = currentNode;
					node2 = null;
				}
				if (currentNode.IsSegmentStarter)
				{
					node2 = currentNode;
				}
				if (node2 != null)
				{
					Node previousIncludeStarters = GetPreviousIncludeStarters(node2);
					if (!previousIncludeStarters.ItemsEqual(node2))
					{
						node2.IsSegmentStarter = false;
						previousIncludeStarters.IsSegmentStarter = true;
						treeIterator = new TreeIterator(previousIncludeStarters);
						treeIterator.Previous();
						currentNode = treeIterator.CurrentNode;
						node2 = null;
						node = null;
						continue;
					}
				}
				if (node != null)
				{
					Node nextIncludeEnders = GetNextIncludeEnders(node);
					if (!nextIncludeEnders.ItemsEqual(node))
					{
						node.IsSegmentEnder = false;
						nextIncludeEnders.IsSegmentEnder = true;
					}
				}
				node2 = null;
				node = null;
				treeIterator.Previous();
				currentNode = treeIterator.CurrentNode;
			}
		}

		public void GetIncludesWithStartersAndEnders()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(Tree.Root.LastChild);
			treeIterator.MoveToLastItemInTree();
			Node currentNode = treeIterator.CurrentNode;
			Node node = null;
			Node node2 = null;
			while (currentNode != null && !currentNode.IsRoot)
			{
				if (currentNode.IsSegmentEnder)
				{
					node = currentNode;
					node2 = null;
				}
				if (currentNode.IsSegmentStarter)
				{
					node2 = currentNode;
				}
				if (node2 != null && node != null && node2.ItemsEqual(node))
				{
					Node node3 = node2;
					bool flag = false;
					while (node3.IsAloneInMayExcludeContainer)
					{
						node3 = node3.Parent;
					}
					Node previousIncludeStartersIncludingContainers = GetPreviousIncludeStartersIncludingContainers(node3);
					Node nextIncludeEndersIncludingWhitespace = GetNextIncludeEndersIncludingWhitespace(node3);
					if (!previousIncludeStartersIncludingContainers.ItemsEqual(node3))
					{
						node2.IsSegmentEnder = false;
						node2.IsSegmentStarter = false;
						node3.IsSegmentEnder = true;
						previousIncludeStartersIncludingContainers.IsSegmentStarter = true;
						flag = true;
					}
					if (!nextIncludeEndersIncludingWhitespace.ItemsEqual(node3))
					{
						if (node.IsSegmentEnder)
						{
							node.IsSegmentEnder = false;
							if (node2.IsSegmentStarter)
							{
								node2.IsSegmentStarter = false;
								node3.IsSegmentStarter = true;
							}
						}
						flag = true;
						nextIncludeEndersIncludingWhitespace.IsSegmentEnder = true;
					}
					if (flag)
					{
						break;
					}
				}
				if (node != null)
				{
					Node nextIncludeEndersIncludingWhitespace2 = GetNextIncludeEndersIncludingWhitespace(node);
					if (!nextIncludeEndersIncludingWhitespace2.ItemsEqual(node))
					{
						node.IsSegmentEnder = false;
						nextIncludeEndersIncludingWhitespace2.IsSegmentEnder = true;
					}
				}
				if (node2 != null)
				{
					Node previousIncludeStartersIncludingContainers2 = GetPreviousIncludeStartersIncludingContainers(node2);
					if (!previousIncludeStartersIncludingContainers2.ItemsEqual(node2))
					{
						node2.IsSegmentStarter = false;
						previousIncludeStartersIncludingContainers2.IsSegmentStarter = true;
					}
				}
				node2 = null;
				node = null;
				treeIterator.Previous();
				currentNode = treeIterator.CurrentNode;
			}
		}

		private void PromoteSingleSiblingStartersEnders()
		{
			if (Tree?.Root?.FirstChild?.FirstChild == null)
			{
				return;
			}
			Node node = Tree.Root.FirstChild.FirstChild;
			Node node2 = null;
			Node node3 = null;
			while (node != null)
			{
				if (node.IsSegmentStarter)
				{
					if (node2 != null)
					{
						return;
					}
					node2 = node;
				}
				if (node.IsSegmentEnder)
				{
					if (node3 == null)
					{
						node3 = node;
					}
					else
					{
						node3.IsSegmentEnder = false;
						node3 = node;
					}
				}
				node = node.Next;
			}
			if (node2 != null && node3 != null && node2.HasParent && !node2.Parent.IsRoot && node2.Parent.Include && node2.BranchCount == 2)
			{
				node2.IsSegmentStarter = false;
				node2.Parent.IsSegmentStarter = true;
				node3.IsSegmentEnder = false;
				node3.Parent.IsSegmentEnder = true;
			}
		}

		private static Node GetTopLevelIncludeContainer(Node starter)
		{
			if (!starter.HasParent)
			{
				return null;
			}
			if (!starter.IsInsideTagIncludeContainerWithMayExcludesInChain && !starter.IsInsideCanHideTagContainersUpToRoot)
			{
				return starter;
			}
			if (starter.Parent.IsRoot && starter.AnyInclude)
			{
				return starter;
			}
			Node parent = starter.Parent;
			bool flag = false;
			while (((!parent.IsRoot && !parent.Exclude) || parent.CanHide) && (!parent.AnyInclude || parent.MayExclude))
			{
				if (parent.MayExclude)
				{
					flag = true;
				}
				if (parent.Parent.Depth == 1 && parent.Parent.Include && parent.Parent.DescendantStarters > 0)
				{
					return parent.Parent;
				}
				parent = parent.Parent;
			}
			if (parent.IsRoot || parent.Exclude)
			{
				return null;
			}
			if (parent.IsContainer && parent.AnyInclude)
			{
				return parent;
			}
			while ((!parent.IsRoot && parent.HasParent && parent.Parent.AnyInclude) || (parent.HasParent && parent.Parent.MayExclude))
			{
				if (parent.MayExclude)
				{
					flag = true;
				}
				parent = parent.Parent;
			}
			if (flag)
			{
				return null;
			}
			return parent;
		}

		private static bool ContainsMayExcludeBetweenParentAndChild(Node parent, Node child)
		{
			if (!parent.Contains(child))
			{
				return false;
			}
			Node node = child;
			while (node != null && !node.Parent.IsRoot && !node.ItemsEqual(parent))
			{
				if (node.MayExclude && node.IsTagPair && !node.CanHide)
				{
					return true;
				}
				node = node.Parent;
			}
			return false;
		}

		public virtual bool IsInSameContainer(Node starter, Node ender)
		{
			for (Node node = starter; node != null; node = node.Next)
			{
				if (node == ender)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool StarterAppearsBeforeEnderInBranch(Node ender)
		{
			while (ender != null)
			{
				if (ender.IsSegmentStarter)
				{
					return true;
				}
				ender = ender.Previous;
			}
			return false;
		}

		internal void FinalizeWhitespace()
		{
			for (Node node = FindNextStarterNode(); node != null; node = FindNextStarterNode())
			{
				Node nextStartingWhitespaceItemInSequence = node.NextStartingWhitespaceItemInSequence;
				if (nextStartingWhitespaceItemInSequence != null)
				{
					bool flag = (!node.Include && !node.IsInsideTagIncludeContainer) || node.IsRevisionMarker || node.IsCommentMarker;
					if (node.IsWhitespace && node.HasNext && !node.Previous.IsSegmentStarter)
					{
						node.IsSegmentStarter = false;
						node.Next.IsSegmentStarter = true;
						flag = false;
					}
					if (flag)
					{
						nextStartingWhitespaceItemInSequence.HasProcessedAlready = true;
						nextStartingWhitespaceItemInSequence = (nextStartingWhitespaceItemInSequence.HasNext ? nextStartingWhitespaceItemInSequence.Next : nextStartingWhitespaceItemInSequence.Parent.Next);
						if (nextStartingWhitespaceItemInSequence != null)
						{
							while (nextStartingWhitespaceItemInSequence.HasParent && !nextStartingWhitespaceItemInSequence.Parent.IsRoot && (nextStartingWhitespaceItemInSequence.Parent.AnyInclude || (nextStartingWhitespaceItemInSequence.Parent.MayExclude && nextStartingWhitespaceItemInSequence.AncestorContainerChainIsStarter)))
							{
								nextStartingWhitespaceItemInSequence = nextStartingWhitespaceItemInSequence.Split(promoteDataToParent: true);
								if (nextStartingWhitespaceItemInSequence.IsSegmentStarter)
								{
									break;
								}
							}
							if (nextStartingWhitespaceItemInSequence.IsContainer && nextStartingWhitespaceItemInSequence.Previous.IsContainer)
							{
								nextStartingWhitespaceItemInSequence.IsSegmentStarter |= nextStartingWhitespaceItemInSequence.Previous.IsSegmentStarter;
								nextStartingWhitespaceItemInSequence.IsSegmentEnder |= nextStartingWhitespaceItemInSequence.Previous.IsSegmentEnder;
								nextStartingWhitespaceItemInSequence.Previous.IsSegmentStarter = false;
								nextStartingWhitespaceItemInSequence.Previous.IsSegmentEnder = false;
							}
						}
					}
				}
				Node node2 = FindNextEnderNode();
				if (node2 != null)
				{
					if (node == node2)
					{
						MoveToNextBoundary();
					}
					Node node3 = node2.PreviousEndingWhitespaceItemInSequence;
					if (node3 != null)
					{
						bool flag2 = ((!IsSegmentUnit(node, node2) || !node2.IsTagPair || !node2.Contains(node3) || !node2.ContainsOnlyWhitespace) && !node2.Include && !node2.IsInsideTagIncludeContainer) || node2.IsRevisionMarker || node2.IsCommentMarker;
						if (node2.IsWhitespace && node2.HasPrevious && !node2.Previous.IsSegmentEnder)
						{
							node2.IsSegmentEnder = false;
							node2.Previous.IsSegmentEnder = true;
							flag2 = false;
						}
						if (flag2)
						{
							while (node3.HasParent && !node3.Parent.IsRoot && (node3.Parent.AnyInclude || (node3.Parent.MayExclude && node3.AncestorContainerChainIsEnder)))
							{
								while (node3.HasPrevious && node3.Previous.IsWhitespace)
								{
									node3 = node3.Previous;
								}
								node3 = node3.Split(promoteDataToParent: true);
								if (node3.IsSegmentEnder)
								{
									break;
								}
							}
						}
						if (node2.IsTagPair && node2.ContainsOnlyWhitespace && node2.MayExclude && node2.Previous.Equals(node) && !node2.IsSegmentStarter && !node.IsSegmentEnder)
						{
							node2.IsSegmentEnder = false;
							node.IsSegmentEnder = true;
						}
					}
				}
			}
			ResetIterator();
		}

		public virtual bool HasStarterInChain(Node node)
		{
			while (!node.IsRoot && node.HasParent && (!node.HasPrevious || node.IsSegmentStarter))
			{
				if (node.IsSegmentStarter)
				{
					return true;
				}
				node = node.Parent;
			}
			return false;
		}

		public virtual bool HasEnderInChain(Node node)
		{
			while (!node.IsRoot && node.HasParent && (!node.HasNext || node.IsSegmentEnder))
			{
				if (node.IsSegmentEnder)
				{
					return true;
				}
				node = node.Parent;
			}
			return false;
		}

		public virtual bool SetStarterOnSingleLockedContent()
		{
			if (Tree?.Root?.LastChild == null)
			{
				return false;
			}
			if (Tree.Root.FirstChild.LockedContentOnlyInChainContainingText)
			{
				return true;
			}
			if (Tree.Root.FirstChild.BranchCount == 1 && Tree.Root.FirstChild.IsLockedContent && Tree.Root.FirstChild.IsSegmentEnder && Tree.Root.FirstChild.LockedContentHasChild && !Tree.Root.FirstChild.LockedContentOnlyContainsWhitespace && Tree.Root.FirstChild.LockedContentInclude)
			{
				Tree.Root.FirstChild.IsSegmentStarter = true;
				return true;
			}
			if (!ContainsAllIncludeLockedContentInBranch(Tree.Root.LastChild))
			{
				return false;
			}
			Tree.Root.FirstChild.IsSegmentStarter = true;
			return true;
		}

		public virtual Node FindNextStarterNode()
		{
			if (InitializeIterator() == null)
			{
				return null;
			}
			while (!_iteratorStarterEnder.CurrentNode.IsSegmentStarter)
			{
				Node node = _iteratorStarterEnder.Next();
				if (node == null)
				{
					return null;
				}
			}
			return _iteratorStarterEnder.CurrentNode;
		}

		public virtual Node FindNextEnderNode()
		{
			if (InitializeIterator() == null)
			{
				return null;
			}
			while (!_iteratorStarterEnder.CurrentNode.IsSegmentEnder)
			{
				Node node = _iteratorStarterEnder.Next();
				if (node == null)
				{
					return null;
				}
			}
			return _iteratorStarterEnder.CurrentNode;
		}

		private Node InitializeIterator()
		{
			if (Tree?.Root?.FirstChild == null)
			{
				return null;
			}
			if (_iteratorStarterEnder == null)
			{
				_iteratorStarterEnder = new TreeIterator(Tree.Root.FirstChild);
			}
			return _iteratorStarterEnder.CurrentNode;
		}

		private void ResetIterator()
		{
			_iteratorStarterEnder = null;
		}

		public virtual bool MoveToNextBoundary()
		{
			Node node = _iteratorStarterEnder.Next();
			return node != null;
		}

		public virtual void RunSegmentationEngine(SegmentationEngine engine)
		{
			_segmentationEngineRunner.RunSegmentationEngine(engine, Tree);
		}

		public virtual void ApplySegmentationInfo()
		{
			_segmentationEngineRunner.ApplySegmentationInfo();
		}
	}
}
