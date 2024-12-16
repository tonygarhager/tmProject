using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class BomNode : Node
	{
		public IAbstractMarkupData Item
		{
			get;
			set;
		}

		public override bool IsAbstractTag
		{
			get
			{
				if (!(Item is ITagPair))
				{
					return Item is IPlaceholderTag;
				}
				return true;
			}
		}

		public override SegmentationHint SegmentationHint
		{
			get
			{
				SegmentationHint result = SegmentationHint.MayExclude;
				IAbstractMarkupData item = Item;
				if (!(item is ITagPair) && !(item is IPlaceholderTag))
				{
					if (!(item is IRevisionMarker) && !(item is ICommentMarker))
					{
						if (!(item is ILockedContent))
						{
							if (item is ISegment)
							{
								result = SegmentationHint.Exclude;
							}
						}
						else
						{
							result = SegmentationHint.MayExclude;
						}
					}
					else
					{
						result = SegmentationHint.Include;
					}
				}
				else
				{
					result = ((Item as ITagPair)?.StartTagProperties.SegmentationHint ?? ((IPlaceholderTag)Item).Properties.SegmentationHint);
				}
				return result;
			}
		}

		public override bool CanHide => (Item as ITagPair)?.StartTagProperties.CanHide ?? false;

		public override bool IsTagPair => Item is ITagPair;

		public override bool IsRevisionMarker => Item is IRevisionMarker;

		public override bool IsPlaceholderTag => Item is IPlaceholderTag;

		public override bool IsText => Item is IText;

		public override bool IsLocationMarker => Item is ILocationMarker;

		public override string Text
		{
			get
			{
				if (!IsText)
				{
					return null;
				}
				return ((IText)Item).Properties.Text;
			}
		}

		public override bool IsWhitespace
		{
			get
			{
				IText text = Item as IText;
				if (text != null)
				{
					return SegmentorUtility.IsTextWhiteSpace(text);
				}
				return false;
			}
		}

		public override bool IsNonWhiteSpaceText
		{
			get
			{
				IText text = Item as IText;
				if (text != null)
				{
					return !SegmentorUtility.IsTextWhiteSpace(text);
				}
				return false;
			}
		}

		public override bool IsContainer
		{
			get
			{
				if (!(Item is IAbstractMarkupDataContainer) && !(Item is ILockedContent))
				{
					return Item == null;
				}
				return true;
			}
		}

		public override bool IsWordStop
		{
			get
			{
				if (IsTagPair)
				{
					return (Item as ITagPair)?.StartTagProperties.IsWordStop ?? false;
				}
				return (Item as IPlaceholderTag)?.Properties.IsWordStop ?? false;
			}
		}

		public override bool IsLockedContent => Item is ILockedContent;

		public override bool IsSegment => Item is ISegment;

		public override bool IsCommentMarker => Item is ICommentMarker;

		public override bool IsInsideTagContainer
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.IsTagPair)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override bool IsInsideRevisionContainer
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
					if (parent.IsRevisionMarker)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override bool IsInsideCommentContainer
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
					if (parent.IsCommentMarker)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override bool IsInsideRevisionContainerWithNoMayExcludeInChain
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.MayExclude || parent.Exclude)
					{
						return false;
					}
					if (parent.IsRevisionMarker)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override bool IsInsideCommentContainerWithNoMayExcludeInChain
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.MayExclude || parent.Exclude)
					{
						return false;
					}
					if (parent.IsCommentMarker)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override bool IsInsideTagStopContainer
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.IsWordStop)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override bool IsInsideTagAnyIncludeContainer
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if ((parent.Include || parent.IncludeWithText) && parent.IsTagPair)
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

		public override bool IsInsideTagIncludeContainer
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.Include && parent.IsTagPair)
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

		public override bool IsInsideTagIncludeContainerWithMayExcludesInChain
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.MayExclude)
					{
						parent = parent.Parent;
						continue;
					}
					if (parent.Include || parent.IncludeWithText)
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

		public override bool IsInsideCanHideTagContainersUpToRoot
		{
			get
			{
				Node node = this;
				if (!node.IsContainer && node.HasParent && !node.Parent.IsRoot)
				{
					node = node.Parent;
				}
				while (!node.IsRoot)
				{
					if (!node.IsTagPair)
					{
						return false;
					}
					if (node.MayExclude && !node.CanHide)
					{
						return false;
					}
					if (node.Exclude)
					{
						return false;
					}
					if (node.AnyInclude && !node.CanHide)
					{
						return false;
					}
					node = node.Parent;
				}
				return true;
			}
		}

		public override bool IsInsideMayExcludeOrExcludeContainer
		{
			get
			{
				Node parent = Parent;
				while (parent != null && !parent.IsRoot)
				{
					if (parent.MayExclude || parent.Exclude)
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override bool IsInsideIncludeRevisionOrMayExcludeContainer
		{
			get
			{
				if (!IsInsideMayExcludeOrExcludeContainer && !IsInsideTagAnyIncludeContainer && !IsInsideRevisionContainer)
				{
					return IsInsideCommentContainer;
				}
				return true;
			}
		}

		public override bool IsAloneInMayExcludeContainer
		{
			get
			{
				if (Parent.IsRoot)
				{
					return false;
				}
				if (base.BranchCount == 1)
				{
					return Parent.MayExclude;
				}
				return false;
			}
		}

		public override bool LockedContentHasChild
		{
			get
			{
				ILockedContent lockedContent = Item as ILockedContent;
				if (lockedContent != null)
				{
					return lockedContent.Content.Count > 0;
				}
				return false;
			}
		}

		public override bool LockedContentOnlyInChainContainingText
		{
			get
			{
				bool result = false;
				Node node = First;
				do
				{
					if (node.IsLockedContent && node.LockedContentOnlyContainsText)
					{
						result = true;
					}
					if (!node.IsLockedContent && !node.IsLocationMarker)
					{
						return false;
					}
					node = node.Next;
				}
				while (node != null);
				return result;
			}
		}

		public override bool LockedContentOnlyContainsWhitespace
		{
			get
			{
				ILockedContent lockedContent = Item as ILockedContent;
				if (lockedContent == null)
				{
					return false;
				}
				foreach (IAbstractMarkupData item in lockedContent.Content)
				{
					IText text = item as IText;
					if (text == null)
					{
						return false;
					}
					if (!SegmentorUtility.IsTextWhiteSpace(text))
					{
						return false;
					}
				}
				return true;
			}
		}

		public override bool LockedContentOnlyContainsText
		{
			get
			{
				if (!IsLockedContent)
				{
					return false;
				}
				ILockedContent lockedContent = Item as ILockedContent;
				if (lockedContent != null && lockedContent.Content.Any((IAbstractMarkupData locked) => !(locked is IText) && !(locked is ILocationMarker)))
				{
					return false;
				}
				return !LockedContentOnlyContainsWhitespace;
			}
		}

		public override SegmentationHint LockedContentSegmentationHint
		{
			get
			{
				BomSegmentationTreeManager bomSegmentationTreeManager = new BomSegmentationTreeManager();
				if (!IsLockedContent)
				{
					return SegmentationHint.MayExclude;
				}
				if (LockedContentOnlyContainsText && base.BranchCount == 1 && base.Depth == 1 && !InAnyTextFlow)
				{
					return SegmentationHint.Exclude;
				}
				ILockedContent lockedContent = Item as ILockedContent;
				if (lockedContent != null)
				{
					bomSegmentationTreeManager.PopulateTree(lockedContent.Content);
				}
				TreeIterator treeIterator = new TreeIterator(bomSegmentationTreeManager.Tree.Root.LastChild);
				treeIterator.MoveToLastItemInTree();
				bool flag = false;
				bool flag2 = false;
				while (treeIterator.CurrentNode != null)
				{
					if (treeIterator.CurrentNode.Depth == 1 && treeIterator.CurrentNode.IsFirst && treeIterator.CurrentNode.Include && treeIterator.CurrentNode.BranchCount == 1)
					{
						return SegmentationHint.Include;
					}
					if (treeIterator.CurrentNode.Exclude && !treeIterator.CurrentNode.Include && !treeIterator.CurrentNode.IsText)
					{
						return SegmentationHint.Exclude;
					}
					if (treeIterator.CurrentNode.MayExclude && (treeIterator.CurrentNode.IsPlaceholderTag || treeIterator.CurrentNode.IsTagPair))
					{
						flag = true;
					}
					if (treeIterator.CurrentNode.IncludeWithText && (treeIterator.CurrentNode.IsPlaceholderTag || treeIterator.CurrentNode.IsTagPair))
					{
						flag2 = true;
					}
					treeIterator.Previous();
				}
				if (flag)
				{
					return SegmentationHint.MayExclude;
				}
				if (!flag2)
				{
					return SegmentationHint.Include;
				}
				return SegmentationHint.IncludeWithText;
			}
		}

		public override int LeadingWhitespaceCount
		{
			get
			{
				int result = 0;
				IText text = Item as IText;
				if (text != null)
				{
					result = SegmentorUtility.GetLeadingWhitespacesCount(text.Properties.Text, 0);
				}
				return result;
			}
		}

		public override int TrailingWhitespaceCount
		{
			get
			{
				int result = 0;
				SegmentorUtility segmentorUtility = new SegmentorUtility();
				IText text = Item as IText;
				if (text != null)
				{
					result = segmentorUtility.GetTrailingWhitespacesCount(text.Properties.Text);
				}
				return result;
			}
		}

		public override int TrailingWhitespaceSplitIndex
		{
			get
			{
				int result = 0;
				IText text = Item as IText;
				if (text != null)
				{
					result = text.Properties.Text.Length - TrailingWhitespaceCount;
				}
				return result;
			}
		}

		public override int TextCount
		{
			get
			{
				int result = 0;
				IText text = Item as IText;
				if (text != null)
				{
					result = text.Properties.Text.Length;
				}
				return result;
			}
		}

		public override Node Split()
		{
			int indexInParent = Item.IndexInParent;
			IAbstractMarkupDataContainer parent = Item.Parent;
			if (parent is IParagraph || !parent.CanBeSplit)
			{
				return this;
			}
			IAbstractMarkupData abstractMarkupData = parent as IAbstractMarkupData;
			if (abstractMarkupData == null)
			{
				return null;
			}
			int indexInParent2 = abstractMarkupData.IndexInParent;
			IAbstractMarkupDataContainer parent2 = abstractMarkupData.Parent;
			IAbstractMarkupData abstractMarkupData2 = parent.Split(indexInParent) as IAbstractMarkupData;
			(abstractMarkupData2 as ITagPair)?.StartTagProperties.SetMetaData("SDL:AutoCloned", true.ToString(CultureInfo.InvariantCulture));
			parent2.Insert(indexInParent2 + 1, abstractMarkupData2);
			BomSegmentationTreeManager bomSegmentationTreeManager = new BomSegmentationTreeManager();
			bomSegmentationTreeManager.PopulateTreeWithRoot(abstractMarkupData2);
			Node node = Parent.InsertNext(bomSegmentationTreeManager.Tree.Root.FirstChild);
			ITagPair tagPair = parent as ITagPair;
			if (tagPair != null && tagPair.Count == 0)
			{
				parent2.Remove(tagPair);
				Parent.Remove();
			}
			CopyTreeData(this, node.FirstChild);
			BomNode bomNode = this;
			Node next = bomNode.Next;
			while (true)
			{
				bomNode.Remove();
				if (next == null)
				{
					break;
				}
				bomNode = (next as BomNode);
				next = bomNode.Next;
			}
			return node;
		}

		public override Node Split(bool promoteDataToParent)
		{
			int indexInParent = Item.IndexInParent;
			IAbstractMarkupDataContainer parent = Item.Parent;
			if (parent is IParagraph || !parent.CanBeSplit)
			{
				return this;
			}
			IAbstractMarkupData abstractMarkupData = parent as IAbstractMarkupData;
			if (abstractMarkupData == null)
			{
				return null;
			}
			int indexInParent2 = abstractMarkupData.IndexInParent;
			IAbstractMarkupDataContainer parent2 = abstractMarkupData.Parent;
			IAbstractMarkupData abstractMarkupData2 = parent.Split(indexInParent) as IAbstractMarkupData;
			(abstractMarkupData2 as ITagPair)?.StartTagProperties.SetMetaData("SDL:AutoCloned", true.ToString(CultureInfo.InvariantCulture));
			parent2.Insert(indexInParent2 + 1, abstractMarkupData2);
			BomSegmentationTreeManager bomSegmentationTreeManager = new BomSegmentationTreeManager();
			bomSegmentationTreeManager.PopulateTreeWithRoot(abstractMarkupData2);
			Node node = Parent.InsertNext(bomSegmentationTreeManager.Tree.Root.FirstChild);
			if (abstractMarkupData is ITagPair && parent.Count == 0)
			{
				parent2.Remove(abstractMarkupData);
				Parent.Remove();
			}
			CopyTreeData(this, node.FirstChild);
			BomNode bomNode = this;
			if (promoteDataToParent)
			{
				PromoteDataToParent(node, bomNode);
			}
			Node next = bomNode.Next;
			while (true)
			{
				bomNode.Remove();
				if (next == null)
				{
					break;
				}
				bomNode = (next as BomNode);
				next = bomNode.Next;
			}
			if (!promoteDataToParent || !node.HasPrevious || !node.Previous.IsSegmentStarter || node.Previous.IsSegmentEnder)
			{
				return node;
			}
			node.IsSegmentStarter = true;
			if (node.Previous.IsCommentMarker && node.Previous.BranchContainsMultipleSegmentEnders)
			{
				return node;
			}
			if (node.Previous.DescendantStarters != 0 || node.Previous.DescendantEnders != 1)
			{
				node.Previous.IsSegmentStarter = false;
			}
			return node;
		}

		public override Node SplitText(int splitPosition, bool leadingSpaces)
		{
			IText text = Item as IText;
			if (splitPosition > text.Properties.Text.Length)
			{
				return this;
			}
			IAbstractMarkupDataContainer parent = text.Parent;
			IText item = text.Split(splitPosition);
			parent.Insert(text.IndexInParent + 1, item);
			Node node = InsertNext(new BomNode
			{
				Item = item
			});
			if (!leadingSpaces)
			{
				return node;
			}
			node.IsSegmentStarter = base.IsSegmentStarter;
			node.IsSegmentEnder = base.IsSegmentEnder;
			base.IsSegmentStarter = false;
			base.IsSegmentEnder = false;
			return node;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Item.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			BomNode bomNode = obj as BomNode;
			if (base.Equals(obj))
			{
				if (Item != null || bomNode?.Item != null)
				{
					if (Item != null)
					{
						return Item.Equals(bomNode?.Item);
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public override void MoveSpaceOutsideContainer(bool isLeading)
		{
			if (!IsText || !HasNext || (BranchCount != 2 && !First.IsWhitespace && !isLeading) || Parent.IsRoot || Item.Parent is IParagraph || !Parent.IsContainer || (isLeading && !SegmentorUtility.IsTextWhiteSpace(Item)))
			{
				return;
			}
			BomNode bomNode = Next as BomNode;
			if (bomNode != null && !isLeading && !SegmentorUtility.IsTextWhiteSpace(bomNode.Item))
			{
				return;
			}
			Node node = isLeading ? Next : this;
			Node starterParent = node.GetStarterParent();
			Node ancestorNodeBelowRoot = node.GetAncestorNodeBelowRoot();
			if (!ancestorNodeBelowRoot.IsSegmentEnder && starterParent.ItemsEqual(ancestorNodeBelowRoot) && !starterParent.IsSegmentEnder)
			{
				return;
			}
			Node nonIsolatedTopLevelNodeInAncestorBranch = node.NonIsolatedTopLevelNodeInAncestorBranch;
			if (ancestorNodeBelowRoot.IsSegmentEnder && nonIsolatedTopLevelNodeInAncestorBranch != null && !node.Parent.ItemsEqual(nonIsolatedTopLevelNodeInAncestorBranch.LastChild))
			{
				return;
			}
			if (!isLeading)
			{
				if (!ancestorNodeBelowRoot.IsSegmentEnder && (!starterParent.IsSegmentEnder || starterParent.IsRoot) && !node.Parent.IsSegmentEnder)
				{
					return;
				}
				node = node.Next;
			}
			while (node != null && !node.IsRoot)
			{
				Node node2 = node.Split(promoteDataToParent: true);
				if (node.ItemsEqual(node2))
				{
					break;
				}
				node = node2;
				if (!node.Include)
				{
					node.MarkedAsExclude = true;
				}
				if (node.BranchCount != 2)
				{
					break;
				}
			}
			if (isLeading && starterParent.Next != null)
			{
				starterParent.Next.IsSegmentStarter = starterParent.IsSegmentStarter;
				starterParent.Next.IsSegmentEnder = starterParent.IsSegmentEnder;
				starterParent.IsSegmentStarter = false;
				starterParent.IsSegmentEnder = false;
			}
		}

		public override bool ItemsEqual(Node other)
		{
			BomNode bomNode = other as BomNode;
			if (bomNode == null)
			{
				return false;
			}
			return Item == bomNode.Item;
		}

		public override string ToString()
		{
			string text = "";
			switch (SegmentationHint)
			{
			case SegmentationHint.Exclude:
				text = "e";
				break;
			case SegmentationHint.Include:
				text = "i";
				break;
			case SegmentationHint.IncludeWithText:
				text = "t";
				break;
			case SegmentationHint.MayExclude:
				text = "m";
				break;
			}
			string text2 = "";
			IAbstractMarkupData item = Item;
			if (!(item is IPlaceholderTag))
			{
				if (item is ITagPair || item is IRevisionMarker)
				{
					Node node = FirstChild;
					string text3 = "<" + ((Item is ITagPair) ? "t" : "r") + text + ">";
					if (node != null)
					{
						while (node.HasNext && node.Parent.Equals(this))
						{
							text3 += node.ToString();
							node = node.Next;
						}
						text3 += node.ToString();
					}
					return text3 + "</" + ((Item is ITagPair) ? "t" : "r") + text + ">";
				}
				IAbstractMarkupData item2 = Item;
				if (!(item2 is IRevisionMarker))
				{
					if (item2 is ILockedContent)
					{
						text = "";
						text2 = "l";
					}
				}
				else
				{
					text2 = "r";
				}
				if (Item is IRevisionMarker || Item is ILockedContent)
				{
					return $"<{text2}{text}>{Item}</{text2}{text}>";
				}
				if (!base.IsRoot || FirstChild == null)
				{
					if (Item != null)
					{
						return Item.ToString();
					}
					return "";
				}
				Node node2 = FirstChild;
				string text4 = FirstChild.ToString();
				while (node2.HasNext)
				{
					node2 = node2.Next;
					text4 += node2?.ToString();
				}
				return "<root>" + text4 + "</root>";
			}
			return "<p" + text + "/>";
		}
	}
}
