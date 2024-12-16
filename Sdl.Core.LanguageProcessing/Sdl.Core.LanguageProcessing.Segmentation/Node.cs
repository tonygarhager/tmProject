using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using System;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class Node
	{
		public bool IsSegmentStarter
		{
			get;
			set;
		}

		public bool IsSegmentEnder
		{
			get;
			set;
		}

		public bool HasProcessedAlready
		{
			get;
			set;
		}

		public bool IsRoot => Parent == null;

		public virtual Node Root
		{
			get
			{
				Node node = this;
				while (node.Parent != null)
				{
					node = node.Parent;
				}
				return node;
			}
		}

		public virtual Node Parent
		{
			get;
			set;
		}

		public virtual Node Next
		{
			get;
			set;
		}

		public virtual Node Previous
		{
			get;
			set;
		}

		public virtual Node FirstChild
		{
			get;
			set;
		}

		public virtual Node First
		{
			get
			{
				Node node = this;
				while (node.Previous != null)
				{
					node = node.Previous;
				}
				return node;
			}
		}

		public virtual Node Last
		{
			get
			{
				Node node = this;
				while (node.Next != null)
				{
					node = node.Next;
				}
				return node;
			}
		}

		public bool ContainsExcludeInside
		{
			get
			{
				if (!IsContainer)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if (treeIterator.CurrentNode.Exclude)
					{
						return true;
					}
					treeIterator.Next();
				}
				return false;
			}
		}

		public bool ContainsOnlyMayExcludeInside
		{
			get
			{
				if (!IsContainer)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if (treeIterator.CurrentNode.MayExclude || treeIterator.CurrentNode.IsWhitespace)
					{
						treeIterator.Next();
						continue;
					}
					return false;
				}
				return true;
			}
		}

		public bool ContainsIncludeInside
		{
			get
			{
				if (!IsContainer)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if (treeIterator.CurrentNode.Include)
					{
						return true;
					}
					treeIterator.Next();
				}
				return false;
			}
		}

		public bool ContainsOnlyIncludeInside
		{
			get
			{
				if (!IsContainer || !HasChild)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if (!treeIterator.CurrentNode.Include)
					{
						return false;
					}
					treeIterator.Next();
				}
				return true;
			}
		}

		public bool ContainsOnlyIncludeContentInside
		{
			get
			{
				if (!IsContainer || !HasChild)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if (!treeIterator.CurrentNode.Include && !treeIterator.CurrentNode.IsText && !treeIterator.CurrentNode.LockedContentInclude)
					{
						return false;
					}
					treeIterator.Next();
				}
				return true;
			}
		}

		public bool ContainsOnlyIncludeWithTextContentInside
		{
			get
			{
				if (!IsContainer || !HasChild)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if (!treeIterator.CurrentNode.IncludeWithText && !treeIterator.CurrentNode.IsText && !treeIterator.CurrentNode.LockedContentInclude)
					{
						return false;
					}
					treeIterator.Next();
				}
				return true;
			}
		}

		public bool IsOnlyIncludeTagContentContainer
		{
			get
			{
				if (!IsContainer || !AnyInclude || !IsAbstractTag)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				bool flag = true;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if ((!treeIterator.CurrentNode.IsAbstractTag || !treeIterator.CurrentNode.AnyInclude) && !treeIterator.CurrentNode.IsWhitespace)
					{
						return false;
					}
					if (!treeIterator.CurrentNode.IsWhitespace)
					{
						flag = false;
					}
					treeIterator.Next();
				}
				return !flag;
			}
		}

		public bool IsOnlyIncludeWithTextNode
		{
			get
			{
				if (!IsContainer)
				{
					return false;
				}
				TreeIterator treeIterator = new TreeIterator(this);
				int depth = Depth;
				bool flag = true;
				treeIterator.Next();
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
				{
					if (!treeIterator.CurrentNode.IncludeWithText && !treeIterator.CurrentNode.CanHide)
					{
						flag = false;
					}
					treeIterator.Next();
				}
				if (flag)
				{
					if (!IncludeWithText)
					{
						return CanHide;
					}
					return true;
				}
				return false;
			}
		}

		public bool CanIncludeAterText
		{
			get
			{
				if (MarkedAsExclude)
				{
					return false;
				}
				bool flag = IsOnlyIncludeTagContentContainer;
				if (flag && IsSegmentStarter && (!IsSegmentEnder || DescendantStarters != DescendantEnders || DescendantStarters > 1))
				{
					flag = false;
				}
				bool flag2 = !IsSegmentStarter && (IsWhitespace || ((IsPlaceholderTag || IsTagPair) && !IsWordStop && (AnyInclude || ContainsIncludeInside || IsOnlyIncludeWithTextNode) && DescendantStarters == 0 && DescendantEnders == 0 && !ContainsExcludeInside && !ContainsOnlyWhitespace));
				bool flag3 = IsLockedContent && !LockedContentAnyExclude && LockedContentHasChild && !IsSegmentStarter;
				return flag | flag2 | flag3;
			}
		}

		public bool CanIncludeBeforeText
		{
			get
			{
				if (!IsSegmentStarter && !IsSegmentEnder)
				{
					if (!IsWhitespace)
					{
						if ((((IsPlaceholderTag || IsTagPair) && !IsWordStop) || IsLockedContent) && (AnyInclude || ContainsIncludeInside || IsOnlyIncludeWithTextNode) && DescendantStarters == 0 && DescendantEnders == 0)
						{
							if (!ContainsExcludeInside)
							{
								return !ContainsOnlyMayExcludeInside;
							}
							return false;
						}
						return false;
					}
					return true;
				}
				return false;
			}
		}

		public virtual Node LastChild => FirstChild?.Last;

		public int Depth
		{
			get
			{
				int num = 0;
				Node node = this;
				while (!node.IsRoot)
				{
					node = node.Parent;
					num++;
				}
				return num;
			}
		}

		public int BranchIndex
		{
			get
			{
				int num = 0;
				Node node = First;
				while (node != this)
				{
					node = node.Next;
					num++;
				}
				return num;
			}
		}

		public int BranchCount
		{
			get
			{
				int num = 0;
				Node node = First;
				while (node != null)
				{
					node = node.Next;
					num++;
				}
				return num;
			}
		}

		public bool HasPrevious => Previous != null;

		public bool HasNext => Next != null;

		public bool HasParagraphNext
		{
			get
			{
				Node node = this;
				while (node.Next == null && !node.Parent.IsRoot)
				{
					node = node.Parent;
				}
				return node.Next != null;
			}
		}

		public bool HasParent => Parent != null;

		public bool HasChild => FirstChild != null;

		public virtual bool LockedContentHasChild => false;

		public virtual bool LockedContentOnlyInChainContainingText => false;

		public virtual bool LockedContentOnlyContainsWhitespace => false;

		public virtual bool LockedContentOnlyContainsText => false;

		public bool IsFirst => BranchIndex == 0;

		public bool IsLast => BranchIndex == BranchCount - 1;

		public bool ContainsOnlyWhitespace
		{
			get
			{
				if (HasChild)
				{
					return FirstChild.NonWhitespaceTextItemsInBranchCount == 0;
				}
				return false;
			}
		}

		public bool IsIsolatedTopLevelNode
		{
			get
			{
				if (Depth == 1 && BranchCount == 1 && Root != null && Root.HasChild)
				{
					return Root.FirstChild.ItemsEqual(this);
				}
				return false;
			}
		}

		public Node IsolatedTopLevelCanHideNode
		{
			get
			{
				Node node = this;
				while (!node.Parent.IsRoot && node.Parent.CanHide && node.Parent.BranchCount == 1)
				{
					node = node.Parent;
				}
				if (!node.ItemsEqual(this))
				{
					return node;
				}
				return null;
			}
		}

		public Node TopLevelCanHideWithOneStarterEnderPair
		{
			get
			{
				Node node = this;
				while (node.Parent != null && !node.Parent.IsRoot && node.Parent.CanHide && node.Parent.DescendantStarters == 1 && node.Parent.DescendantEnders == 1 && !node.Parent.ContainsExcludeInside)
				{
					node = node.Parent;
				}
				if (!node.ItemsEqual(this))
				{
					return node;
				}
				return null;
			}
		}

		public bool IsIsolatedNodeToRoot
		{
			get
			{
				Node node = this;
				while (node != null && !node.IsRoot && node.BranchCount == 1)
				{
					node = node.Parent;
				}
				return node?.IsRoot ?? false;
			}
		}

		public Node NonIsolatedTopLevelIncludeNodeInAncestorBranch
		{
			get
			{
				Node node = this;
				Node result = null;
				if (Include)
				{
					result = this;
				}
				while (node != null && (node.NonWhitespaceTextItemsInBranchCount == 1 || node.Include) && (node.BranchCount == 1 || node.IsNodeSurroundedByWhitespaceInBranch) && node.HasParent && !node.Parent.IsRoot && !node.Exclude)
				{
					node = node.Parent;
					if (node.Include)
					{
						result = node;
					}
				}
				if (node != null && node.Exclude)
				{
					return null;
				}
				return result;
			}
		}

		public bool AncestorContainerChainIsEnder
		{
			get
			{
				if (!HasParent)
				{
					return false;
				}
				Node parent = Parent;
				while (parent != null && !parent.IsRoot && !parent.IsSegmentEnder)
				{
					parent = parent.Parent;
				}
				return parent?.IsSegmentEnder ?? false;
			}
		}

		public bool AncestorContainerChainIsStarter
		{
			get
			{
				if (!HasParent)
				{
					return false;
				}
				Node parent = Parent;
				while (parent != null && !parent.IsRoot && !parent.IsSegmentStarter)
				{
					parent = parent.Parent;
				}
				return parent?.IsSegmentStarter ?? false;
			}
		}

		public Node TopLevelIncludeNodeInAncestorBranch
		{
			get
			{
				Node node = this;
				bool flag = false;
				if (node.IsRoot)
				{
					return null;
				}
				Node node2 = null;
				if (AnyInclude && IsContainer)
				{
					node2 = this;
				}
				if (MayExclude && IsContainer && !CanHide)
				{
					flag = true;
				}
				while (node != null && ((node.HasChild && node.FirstChild.NonWhitespaceTextItemsInBranchCount == 1) || node.CanHide || node.MayExclude || !node.IsContainer) && node.HasParent && !node.Parent.IsRoot && !node.Exclude)
				{
					node = node.Parent;
					if (node.AnyInclude)
					{
						node2 = node;
					}
					if (node.MayExclude && node.IsContainer && !node.CanHide)
					{
						flag = true;
					}
				}
				if (node != null && node.Exclude)
				{
					return null;
				}
				if ((node2?.IncludeWithText ?? false) & flag)
				{
					return null;
				}
				return node2;
			}
		}

		public virtual bool IsInsideAnyIncludeContainer
		{
			get
			{
				if (!IsInsideCommentContainer && !IsInsideRevisionContainer)
				{
					return IsInsideTagAnyIncludeContainer;
				}
				return true;
			}
		}

		public virtual bool IsInsideIncludeContainer
		{
			get
			{
				if (!IsInsideCommentContainer && !IsInsideRevisionContainer)
				{
					return IsInsideTagInclude;
				}
				return true;
			}
		}

		public virtual bool IsInsideTagInclude
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.Exclude)
					{
						return false;
					}
					if (parent.Include && parent.IsTagPair)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public virtual bool IsInsideExcludeContainer
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.Exclude)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public virtual bool IsSegmentStarterEnderPair
		{
			get
			{
				if (IsSegmentStarter)
				{
					return IsSegmentEnder;
				}
				return false;
			}
		}

		public Node NonIsolatedTopLevelNodeInAncestorBranch
		{
			get
			{
				Node node = this;
				while (node != null && node.NonWhitespaceTextItemsInBranchCount == 1 && node.HasParent && !node.Parent.IsRoot && !node.Exclude)
				{
					node = node.Parent;
				}
				if (node != null && node.Exclude)
				{
					return null;
				}
				return node;
			}
		}

		public Node PreviousEnderInBranch
		{
			get
			{
				Node previous = Previous;
				while (previous != null && !previous.IsSegmentEnder && (!previous.IsContainer || (previous.DescendantStarters <= 0 && previous.DescendantEnders <= 0)))
				{
					previous = previous.Previous;
				}
				if (previous != null && !previous.IsSegmentEnder && previous.IsContainer && (previous.DescendantStarters > 0 || previous.DescendantEnders > 0))
				{
					return null;
				}
				return previous;
			}
		}

		public Node PreviousEnderInTree
		{
			get
			{
				TreeIterator treeIterator = new TreeIterator(this);
				treeIterator.Previous();
				while (treeIterator.CurrentNode != null && !treeIterator.CurrentNode.IsSegmentStarter && !treeIterator.CurrentNode.IsSegmentEnder)
				{
					treeIterator.Previous();
				}
				if (treeIterator.CurrentNode != null && treeIterator.CurrentNode.IsSegmentEnder)
				{
					return treeIterator.CurrentNode;
				}
				return null;
			}
		}

		public Node PreviousStarterInBranch
		{
			get
			{
				Node previous = Previous;
				while (previous != null && !previous.IsSegmentStarter)
				{
					previous = previous.Previous;
				}
				return previous;
			}
		}

		public bool ContainsSingleSegmentStarterEnderPair
		{
			get
			{
				if (!IsContainer || DescendantStarters != 1 || DescendantEnders != 1 || !ContainsOnlySegmentStarterEnderPairs)
				{
					return false;
				}
				return !ContainsExcludeInside;
			}
		}

		public Node PreviousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch
		{
			get
			{
				for (Node previous = Previous; previous != null; previous = previous.Previous)
				{
					if (previous.IsSegmentStarter && previous.IsSegmentEnder)
					{
						return previous;
					}
					if (previous.IsContainer && previous.DescendantStarters >= 1 && previous.DescendantStarters == previous.DescendantEnders && previous.ContainsOnlySegmentStarterEnderPairs && !previous.ContainsExcludeInside)
					{
						return previous;
					}
				}
				return null;
			}
		}

		public Node PreviousContainerWithOneOrMoreStarterEnderPairsInsideInBranch
		{
			get
			{
				for (Node previous = Previous; previous != null; previous = previous.Previous)
				{
					if (previous.IsContainer && previous.DescendantStarters >= 1 && previous.DescendantStarters == previous.DescendantEnders && previous.ContainsOnlySegmentStarterEnderPairs && !previous.ContainsExcludeInside)
					{
						return previous;
					}
				}
				return null;
			}
		}

		public int ContainersContainingStarterEnderPairsOrStarterEnderPairCount
		{
			get
			{
				int num = 0;
				Node last = Last;
				if (last == null)
				{
					return num;
				}
				if (last.IsSegmentStarterEnderPair || last.ContainsOnlySegmentStarterEnderPairs)
				{
					num++;
				}
				last = last.PreviousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
				if (last != null && (last.IsSegmentStarterEnderPair || last.ContainsOnlySegmentStarterEnderPairs))
				{
					num++;
					last = last.PreviousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
				}
				while (last != null)
				{
					num++;
					last = last.PreviousContainerWithOneOrMoreStarterEnderPairsInsideOrStarterEnderPairInBranch;
				}
				return num;
			}
		}

		public virtual SegmentationHint SegmentationHint
		{
			get
			{
				throw new NotImplementedException("SegmentationHint not implemented in Base class");
			}
		}

		public bool AnyExclude
		{
			get
			{
				if (!MayExclude && !Exclude)
				{
					return LockedContentExclude;
				}
				return true;
			}
		}

		public bool AnyInclude
		{
			get
			{
				if (!Include && !IncludeWithText)
				{
					return LockedContentInclude;
				}
				return true;
			}
		}

		public bool LockedContentAnyInclude
		{
			get
			{
				if (!LockedContentInclude)
				{
					return LockedContentIncludeWithText;
				}
				return true;
			}
		}

		public bool LockedContentInclude => LockedContentSegmentationHint == SegmentationHint.Include;

		public bool LockedContentIncludeWithText => LockedContentSegmentationHint == SegmentationHint.IncludeWithText;

		public bool LockedContentExclude => LockedContentSegmentationHint == SegmentationHint.Exclude;

		public bool LockedContentAnyExclude
		{
			get
			{
				if (LockedContentSegmentationHint != SegmentationHint.Exclude)
				{
					return LockedContentSegmentationHint == SegmentationHint.MayExclude;
				}
				return true;
			}
		}

		public int UniqueId
		{
			get;
			set;
		}

		public virtual bool IsWordStop => false;

		public virtual bool IsLockedContent => false;

		public virtual bool IsSegment => false;

		public virtual SegmentationHint LockedContentSegmentationHint => SegmentationHint.MayExclude;

		public bool MarkedAsExclude
		{
			get;
			set;
		}

		public bool MayExclude => SegmentationHint == SegmentationHint.MayExclude;

		public bool Exclude => SegmentationHint == SegmentationHint.Exclude;

		public bool Include => SegmentationHint == SegmentationHint.Include;

		public bool IncludeWithText => SegmentationHint == SegmentationHint.IncludeWithText;

		public virtual bool CanHide
		{
			get
			{
				throw new NotImplementedException("CanHide not implemented in Base class");
			}
		}

		public virtual bool IsTagPair
		{
			get
			{
				throw new NotImplementedException("IsTagPair not implemented in Base class");
			}
		}

		public virtual bool IsAbstractTag
		{
			get
			{
				throw new NotImplementedException("IsTagPair not implemented in Base class");
			}
		}

		public virtual bool IsRevisionMarker
		{
			get
			{
				throw new NotImplementedException("IsRevisionMarker not implemented in Base class");
			}
		}

		public virtual bool IsPlaceholderTag
		{
			get
			{
				throw new NotImplementedException("IsPlaceholderTag not implemented in Base class");
			}
		}

		public virtual bool IsText
		{
			get
			{
				throw new NotImplementedException("IsText not implemented in Base class");
			}
		}

		public virtual bool IsLocationMarker
		{
			get
			{
				throw new NotImplementedException("IsText not implemented in Base class");
			}
		}

		public virtual bool IsNonWhiteSpaceText
		{
			get
			{
				throw new NotImplementedException("IsWhitespace not implemented in Base class");
			}
		}

		public virtual bool IsWhitespace
		{
			get
			{
				int leadingWhitespacesCount = GetLeadingWhitespacesCount(Text, 0);
				return leadingWhitespacesCount == Text.Length;
			}
		}

		public virtual bool IsWhitespaceToEndOfBranch
		{
			get
			{
				if (!HasNext)
				{
					return false;
				}
				for (Node next = Next; next != null; next = next.Next)
				{
					if (!next.IsWhitespace)
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool IsWhitespaceToStartOfBranch
		{
			get
			{
				if (!HasPrevious)
				{
					return false;
				}
				for (Node previous = Previous; previous != null; previous = previous.Previous)
				{
					if (!previous.IsWhitespace)
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool IsInsideSegment
		{
			get
			{
				if (IsSegment)
				{
					return true;
				}
				Node node = this;
				while (node != null && !node.IsRoot)
				{
					if (node.IsSegment)
					{
						return true;
					}
					node = node.Parent;
				}
				return false;
			}
		}

		public virtual int TextItemsInBranchCount
		{
			get
			{
				TreeIterator treeIterator = new TreeIterator(Parent.FirstChild);
				int num = 0;
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Parent.Equals(Parent))
				{
					if (treeIterator.CurrentNode.IsText)
					{
						num++;
					}
					treeIterator.Next();
				}
				return num;
			}
		}

		public virtual bool InAnyTextFlow
		{
			get
			{
				Node parent = Parent;
				while (!parent.IsRoot && parent.BranchCount == 1)
				{
					parent = parent.Parent;
				}
				if (parent.IsRoot)
				{
					return false;
				}
				return parent.TextItemsInBranchCount > 0;
			}
		}

		public virtual bool IsNodeSurroundedByWhitespaceInBranch
		{
			get
			{
				if ((!IsFirst || !IsWhitespaceToEndOfBranch) && (!IsLast || !IsWhitespaceToStartOfBranch))
				{
					if (IsWhitespaceToStartOfBranch)
					{
						return IsWhitespaceToEndOfBranch;
					}
					return false;
				}
				return true;
			}
		}

		public virtual bool ParentIsLastContainerEnderInFlow
		{
			get
			{
				if (!HasParent || Parent.IsRoot)
				{
					return false;
				}
				if (Parent.IsSegmentEnder)
				{
					return true;
				}
				Node parent = Parent;
				while (parent.HasParent && !parent.Parent.IsRoot && parent.BranchIndex == parent.BranchCount - 1 && !parent.Parent.Exclude)
				{
					parent = parent.Parent;
				}
				return parent.IsSegmentEnder;
			}
		}

		public virtual bool ParentIsFirstContainerStarterInFlow
		{
			get
			{
				if (!HasParent || Parent.IsRoot)
				{
					return false;
				}
				Node parent = Parent;
				while (!parent.IsSegmentStarter && parent.HasParent && !parent.Parent.IsRoot && parent.BranchIndex == 0 && !parent.Parent.Exclude)
				{
					parent = parent.Parent;
				}
				return parent.IsSegmentStarter;
			}
		}

		public virtual int NonWhitespaceTextItemsInBranchCount
		{
			get
			{
				TreeIterator treeIterator = new TreeIterator(Parent.FirstChild);
				int num = 0;
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Parent.Equals(Parent))
				{
					if (!treeIterator.CurrentNode.IsWhitespace)
					{
						num++;
					}
					treeIterator.Next();
				}
				return num;
			}
		}

		public virtual int EndersInBranchCount
		{
			get
			{
				int num = 0;
				for (Node node = Parent.FirstChild; node != null; node = node.Next)
				{
					if (node.IsSegmentEnder)
					{
						num++;
					}
				}
				return num;
			}
		}

		public virtual int StartersInBranchCount
		{
			get
			{
				int num = 0;
				for (Node node = Parent.FirstChild; node != null; node = node.Next)
				{
					if (node.IsSegmentStarter)
					{
						num++;
					}
				}
				return num;
			}
		}

		public virtual int DescendantStarters
		{
			get
			{
				if (!HasChild)
				{
					return 0;
				}
				int num = 0;
				TreeIterator treeIterator = new TreeIterator(FirstChild);
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > Depth)
				{
					if (treeIterator.CurrentNode.IsSegmentStarter)
					{
						num++;
					}
					treeIterator.Next();
				}
				return num;
			}
		}

		public virtual int DescendantEnders
		{
			get
			{
				if (!HasChild)
				{
					return 0;
				}
				int num = 0;
				TreeIterator treeIterator = new TreeIterator(FirstChild);
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > Depth)
				{
					if (treeIterator.CurrentNode.IsSegmentEnder)
					{
						num++;
					}
					treeIterator.Next();
				}
				return num;
			}
		}

		public virtual bool IsContainer
		{
			get
			{
				throw new NotImplementedException("IsContainer not implemented in Base class");
			}
		}

		public virtual int LeadingWhitespaceCount
		{
			get
			{
				throw new NotImplementedException("LeadingWhitespaceCount not implemented in Base class");
			}
		}

		public virtual int TrailingWhitespaceCount
		{
			get
			{
				throw new NotImplementedException("TrailingWhitespaceCount not implemented in Base class");
			}
		}

		public virtual int TrailingWhitespaceSplitIndex
		{
			get
			{
				throw new NotImplementedException("TrailingWhitespaceSplitIndex not implemented in Base class");
			}
		}

		public virtual int TextCount
		{
			get
			{
				throw new NotImplementedException("TextCount not implemented in Base class");
			}
		}

		public virtual string Text
		{
			get
			{
				throw new NotImplementedException("Text not implemented in Base class");
			}
		}

		public virtual bool IsCommentMarker
		{
			get
			{
				throw new NotImplementedException("IsCommentMarker not implemented in Base class");
			}
		}

		public virtual bool IsInsideTagContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideTagContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideRevisionContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideRevisionContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideCommentContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideCommentContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideRevisionContainerWithNoMayExcludeInChain
		{
			get
			{
				throw new NotImplementedException("IsInsideRevisionContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideCommentContainerWithNoMayExcludeInChain
		{
			get
			{
				throw new NotImplementedException("IsInsideCommentContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideTagIncludeContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideTagIncludeContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideTagAnyIncludeContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideTagIncludeContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideAnyIncludeOrCanHideContainer
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.Include || parent.IncludeWithText || parent.CanHide)
					{
						return true;
					}
					if (parent.Exclude)
					{
						return false;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public virtual bool IsInsideIncludeWithTextContainerWithMayExcludeInChain
		{
			get
			{
				Node parent = Parent;
				bool result = false;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.MayExclude && !parent.CanHide)
					{
						result = true;
					}
					if (parent.IncludeWithText)
					{
						return result;
					}
					if (parent.Exclude)
					{
						return false;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public virtual bool IsInsideSegmentEnder
		{
			get
			{
				Node node = this;
				while (node.HasParent && !node.Parent.IsRoot)
				{
					if (node.IsSegmentEnder)
					{
						return true;
					}
					node = node.Parent;
				}
				return false;
			}
		}

		public virtual bool IsInsideSegmentStarter
		{
			get
			{
				Node node = this;
				while (node.HasParent && !node.Parent.IsRoot)
				{
					if (node.IsSegmentStarter)
					{
						return true;
					}
					node = node.Parent;
				}
				return false;
			}
		}

		public virtual bool IsInsideTagIncludeContainerWithMayExcludesInChain
		{
			get
			{
				throw new NotImplementedException("IsInsideTagIncludeContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideIncludeRevisionOrMayExcludeContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideIncludeRevisionOrMayExcludeContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideTagStopContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideTagStopContainer not implemented in Base class");
			}
		}

		public virtual bool IsAloneInMayExcludeContainer
		{
			get
			{
				throw new NotImplementedException("IsAloneInMayExcludeContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideMayExcludeOrExcludeContainer
		{
			get
			{
				throw new NotImplementedException("IsInsideMayExcludeTagIncludeContainer not implemented in Base class");
			}
		}

		public virtual bool IsInsideCanHideTagContainersUpToRoot
		{
			get
			{
				throw new NotImplementedException("IsInsideMayExcludeTagIncludeContainer not implemented in Base class");
			}
		}

		public virtual Node PreviousNonWhitespace
		{
			get
			{
				Node previous = Previous;
				while (previous != null && previous.HasPrevious && previous.IsWhitespace)
				{
					previous = previous.Previous;
				}
				return previous;
			}
		}

		public virtual Node NextNonWhitespace
		{
			get
			{
				Node next = Next;
				while (next != null && next.HasNext && next.IsWhitespace)
				{
					next = next.Next;
				}
				return next;
			}
		}

		public virtual Node NextWhitespaceItemInBranch
		{
			get
			{
				Node next = Next;
				while (next != null && !next.IsWhitespace)
				{
					next = next.Next;
				}
				return next;
			}
		}

		public virtual Node NextStartingWhitespaceItemInSequence
		{
			get
			{
				if (IsWhitespace)
				{
					return this;
				}
				Node node = this;
				while (node != null && node.IsContainer && node.FirstChild != null)
				{
					node = node.FirstChild;
				}
				if (node != null && node.IsWhitespace)
				{
					return node;
				}
				return null;
			}
		}

		public virtual Node PreviousEndingWhitespaceItemInSequence
		{
			get
			{
				if (IsWhitespace)
				{
					return this;
				}
				Node node = this;
				while (node != null && node.IsContainer && node.LastChild != null)
				{
					node = node.LastChild;
				}
				if (node != null && node.IsWhitespace)
				{
					return node;
				}
				return null;
			}
		}

		public virtual Node PreviousExlcudeInBranch
		{
			get
			{
				for (Node previous = Previous; previous != null; previous = previous.Previous)
				{
					if (previous.Exclude)
					{
						return previous;
					}
				}
				return null;
			}
		}

		public virtual Node NextIncludeTagInBranch
		{
			get
			{
				for (Node next = Next; next != null; next = next.Next)
				{
					if (next.IsSegmentStarter)
					{
						return null;
					}
					if (next.IsAbstractTag && next.AnyInclude && !next.IsText && !next.IsWordStop)
					{
						return next;
					}
				}
				return null;
			}
		}

		public virtual bool BranchOnlyContainsExludes
		{
			get
			{
				Node node = First;
				bool flag = true;
				while (node != null)
				{
					flag &= node.Exclude;
					node = node.Next;
				}
				return flag;
			}
		}

		public virtual bool BranchContainsIncludes
		{
			get
			{
				for (Node node = First; node != null; node = node.Next)
				{
					if (node.AnyInclude && !node.ItemsEqual(this))
					{
						return true;
					}
				}
				return false;
			}
		}

		public virtual bool BranchContainsMultipleSegmentEnders
		{
			get
			{
				Node node = FirstChild;
				int num = 0;
				while (node != null)
				{
					if (node.IsSegmentEnder)
					{
						num++;
					}
					if (node.IsSegmentStarter)
					{
						return false;
					}
					node = node.Next;
				}
				return num > 1;
			}
		}

		public virtual bool ContainsOnlySegmentStarterEnderPairs
		{
			get
			{
				if (!IsContainer || !HasChild)
				{
					return false;
				}
				int depth = Depth;
				Node lastChild = LastChild;
				while (lastChild.IsContainer && lastChild.HasChild)
				{
					lastChild = lastChild.LastChild;
				}
				TreeIterator treeIterator = new TreeIterator(lastChild);
				int num = 0;
				while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth >= depth)
				{
					if (treeIterator.CurrentNode.IsSegmentStarter && !treeIterator.CurrentNode.IsSegmentEnder)
					{
						return false;
					}
					if (!treeIterator.CurrentNode.IsSegmentStarter && treeIterator.CurrentNode.IsSegmentEnder)
					{
						return false;
					}
					if (treeIterator.CurrentNode.IsSegmentStarter && treeIterator.CurrentNode.IsSegmentEnder)
					{
						num++;
					}
					treeIterator.Previous();
				}
				return num > 0;
			}
		}

		public virtual Node NodeAfterPreviousEnderInBranchOrCurrentNode
		{
			get
			{
				Node previous = Previous;
				while (previous != null && !previous.IsSegmentEnder && previous.DescendantEnders == 0)
				{
					previous = previous.Previous;
				}
				if (previous != null && (previous.IsSegmentEnder || previous.DescendantEnders > 0))
				{
					return previous.Next;
				}
				return this;
			}
		}

		public Node()
		{
			IsSegmentEnder = false;
			IsSegmentStarter = false;
			HasProcessedAlready = false;
		}

		public virtual Node Split()
		{
			throw new NotImplementedException("Split not implemented in Base class");
		}

		public virtual Node Split(bool promoteDataToParent)
		{
			throw new NotImplementedException("Split not implemented in Base class");
		}

		public virtual Node SplitText(int splitPosition, bool leadingSpaces)
		{
			throw new NotImplementedException("SplitText not implemented in Base class");
		}

		public override int GetHashCode()
		{
			return IsSegmentStarter.GetHashCode() ^ IsSegmentEnder.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Node node = obj as Node;
			if (node == null)
			{
				return false;
			}
			if (IsSegmentStarter != node.IsSegmentStarter)
			{
				return false;
			}
			return IsSegmentEnder == node.IsSegmentEnder;
		}

		protected void PromoteDataToParent(Node newNode, Node currentNode)
		{
			if (((newNode.MayExclude && newNode.DescendantStarters <= 0 && newNode.DescendantEnders <= 0 && newNode.HasNext && currentNode.HasNext && currentNode.Next.DescendantEnders == 0 && !currentNode.Next.IsSegmentEnder && !currentNode.Last.IsSegmentEnder) || (newNode.Parent.HasNext && NextEnderAppearsBeforeNextStarter(newNode.FirstChild))) && (!currentNode.IsSegmentStarter || !currentNode.IsSegmentEnder))
			{
				newNode.IsSegmentStarter = (newNode.IsSegmentStarter || currentNode.IsSegmentStarter);
				newNode.IsSegmentEnder = (newNode.IsSegmentEnder || currentNode.IsSegmentEnder);
				currentNode.IsSegmentStarter = false;
				currentNode.IsSegmentEnder = false;
				newNode.FirstChild.IsSegmentStarter = false;
				newNode.FirstChild.IsSegmentEnder = false;
			}
		}

		protected void CopyTreeData(Node treeNode, Node newNode)
		{
			TreeIterator treeIterator = new TreeIterator(treeNode);
			TreeIterator treeIterator2 = new TreeIterator(newNode);
			int depth = treeIterator.CurrentNode.Depth;
			treeIterator2.CurrentNode.IsSegmentStarter = treeIterator.CurrentNode.IsSegmentStarter;
			treeIterator2.CurrentNode.IsSegmentEnder = treeIterator.CurrentNode.IsSegmentEnder;
			while (treeIterator.Next() != null && treeIterator.CurrentNode.Depth >= depth)
			{
				treeIterator2.Next();
				treeIterator2.CurrentNode.IsSegmentStarter = treeIterator.CurrentNode.IsSegmentStarter;
				treeIterator2.CurrentNode.IsSegmentEnder = treeIterator.CurrentNode.IsSegmentEnder;
			}
		}

		public Node GetAncestorNodeBelowRoot()
		{
			Node node = this;
			while (!node.Parent.IsRoot)
			{
				node = node.Parent;
			}
			return node;
		}

		public bool Contains(Node searchNode)
		{
			if (!IsContainer)
			{
				return false;
			}
			TreeIterator treeIterator = new TreeIterator(this);
			int depth = Depth;
			treeIterator.Next();
			while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
			{
				if (treeIterator.CurrentNode.Equals(searchNode))
				{
					return true;
				}
				treeIterator.Next();
			}
			return false;
		}

		public virtual Node Add()
		{
			Node node = new Node
			{
				Previous = Last,
				Next = null,
				Parent = Parent,
				FirstChild = null
			};
			Last.Next = node;
			return node;
		}

		public virtual Node Add(Node node)
		{
			node.Previous = Last;
			node.Next = null;
			node.Parent = Parent;
			Last.Next = node;
			return node;
		}

		public virtual Node Insert()
		{
			Node node = new Node
			{
				Previous = null,
				Next = First,
				Parent = Parent,
				FirstChild = null
			};
			First.Previous = node;
			return node;
		}

		public virtual Node Insert(Node node)
		{
			node.Previous = null;
			node.Next = First;
			node.Parent = Parent;
			First.Previous = node;
			return node;
		}

		public virtual Node AddChild()
		{
			Node node = new Node
			{
				Parent = this,
				Next = null,
				FirstChild = null
			};
			if (HasChild)
			{
				node.Previous = FirstChild.Last;
				FirstChild.Last.Next = node;
			}
			else
			{
				FirstChild = node;
			}
			return node;
		}

		public virtual Node AddChild(Node node)
		{
			node.Parent = this;
			node.Next = null;
			if (HasChild)
			{
				node.Previous = FirstChild.Last;
				FirstChild.Last.Next = node;
			}
			else
			{
				FirstChild = node;
			}
			return node;
		}

		public virtual Node InsertChild(int index)
		{
			Node node = new Node
			{
				Parent = this,
				FirstChild = null
			};
			if (HasChild)
			{
				if (index > FirstChild.BranchCount + 1)
				{
					return null;
				}
				Node node2 = FirstChild;
				while (node2 != null && node2.BranchIndex != index)
				{
					node2 = node2.Next;
				}
				if (node2 == null)
				{
					return AddChild();
				}
				node.Next = node2;
				node.Previous = node2.Previous;
				node2.Previous = node;
			}
			else
			{
				if (index != 0)
				{
					return null;
				}
				FirstChild = node;
			}
			return node;
		}

		public virtual Node InsertChild(int index, Node node)
		{
			node.Parent = this;
			if (HasChild)
			{
				if (index > FirstChild.BranchCount + 1)
				{
					return null;
				}
				Node node2 = FirstChild;
				while (node2 != null && node2.BranchIndex != index)
				{
					node2 = node2.Next;
				}
				if (node2 == null)
				{
					return AddChild(node);
				}
				node.Next = node2;
				node.Previous = node2.Previous;
				node2.Previous = node;
			}
			else
			{
				if (index != 0)
				{
					return null;
				}
				FirstChild = node;
			}
			return node;
		}

		public virtual Node InsertNext()
		{
			Node node = new Node
			{
				Next = Next,
				Previous = this,
				Parent = Parent,
				FirstChild = null
			};
			if (Next != null)
			{
				Next.Previous = node;
			}
			Next = node;
			return node;
		}

		public virtual Node InsertNext(Node node)
		{
			node.Next = Next;
			node.Previous = this;
			node.Parent = Parent;
			if (Next != null)
			{
				Next.Previous = node;
			}
			Next = node;
			return node;
		}

		public virtual Node InsertPrevious()
		{
			Node node = new Node
			{
				Next = this,
				Previous = Previous,
				Parent = Parent,
				FirstChild = null
			};
			if (Previous != null)
			{
				Previous.Next = node;
			}
			Previous = node;
			return node;
		}

		public virtual Node Cut()
		{
			Node previous = Previous;
			Node next = Next;
			if (previous != null)
			{
				previous.Next = Next;
			}
			if (next != null)
			{
				next.Previous = Previous;
			}
			if (HasParent && this == Parent.FirstChild)
			{
				Parent.FirstChild = Parent.FirstChild.Next;
			}
			Previous = null;
			Next = null;
			Parent = null;
			return this;
		}

		public void Remove()
		{
			Cut();
		}

		public static int GetLeadingWhitespacesCount(string text, int startIndex)
		{
			int i;
			for (i = startIndex; i < text.Length && CharacterProperties.IsWhitespace(text[i]); i++)
			{
			}
			return i - startIndex;
		}

		public virtual void MoveSpaceOutsideContainer(bool isLeading)
		{
			throw new NotImplementedException("MoveSpaceOutsideContainer not implemented in Base class");
		}

		public Node GetStarterParent()
		{
			Node node = this;
			while (!node.IsRoot && !node.IsSegmentStarter)
			{
				node = node.Parent;
			}
			return node;
		}

		public virtual bool ItemsEqual(Node other)
		{
			throw new NotImplementedException("ItemsEqual not implemented in Base class");
		}

		public void PromoteStarter()
		{
			Parent.IsSegmentStarter = true;
			IsSegmentStarter = false;
		}

		public void PromoteEnder()
		{
			Parent.IsSegmentEnder = true;
			IsSegmentEnder = false;
		}

		public virtual void ResetStartersAndEndersInContainer()
		{
			if (!IsContainer || !HasChild)
			{
				return;
			}
			TreeIterator treeIterator = new TreeIterator(this);
			int depth = Depth;
			treeIterator.Next();
			while (treeIterator.CurrentNode != null && treeIterator.CurrentNode.Depth > depth)
			{
				if (treeIterator.CurrentNode.IsSegmentStarter)
				{
					treeIterator.CurrentNode.IsSegmentStarter = false;
				}
				if (treeIterator.CurrentNode.IsSegmentEnder)
				{
					treeIterator.CurrentNode.IsSegmentEnder = false;
				}
				treeIterator.Next();
			}
		}

		public virtual bool NextEnderAppearsBeforeNextStarter(Node node)
		{
			TreeIterator treeIterator = new TreeIterator(node);
			bool result = false;
			while (treeIterator.Next() != null)
			{
				if (treeIterator.CurrentNode.IsSegmentEnder && !treeIterator.CurrentNode.IsSegmentStarter)
				{
					result = true;
				}
				if (treeIterator.CurrentNode.IsSegmentStarter)
				{
					return result;
				}
			}
			return result;
		}
	}
}
