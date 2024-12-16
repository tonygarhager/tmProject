namespace Sdl.Core.Bcm.BcmModel.Operations.Merge
{
	internal class ContainerParentSplitter
	{
		internal static MarkupDataContainer SplitContainerParents(MarkupDataContainer item, MarkupDataContainer commonAncestor)
		{
			MarkupDataContainer markupDataContainer = item;
			MarkupDataContainer parent = markupDataContainer.Parent;
			while (parent != null && !(parent is Paragraph) && (commonAncestor == null || !parent.Equals(commonAncestor)))
			{
				if (parent.Count > 1)
				{
					int indexInParent = item.IndexInParent;
					MarkupDataContainer before = SplitBefore(parent, indexInParent);
					SplitAfter(item, before, parent);
				}
				markupDataContainer = parent;
				parent = markupDataContainer.Parent;
			}
			return markupDataContainer;
		}

		private static void SplitAfter(MarkupDataContainer item, MarkupDataContainer before, MarkupDataContainer parent)
		{
			int indexInParent = before.IndexInParent;
			int indexInParent2 = item.IndexInParent;
			MarkupDataContainer markupDataContainer = before.SplitAt(indexInParent2 + 1);
			if (markupDataContainer.Count > 0)
			{
				parent.Parent.InsertAt(indexInParent + 1, markupDataContainer);
			}
		}

		private static MarkupDataContainer SplitBefore(MarkupDataContainer parent, int itemIndex)
		{
			if (itemIndex <= 0)
			{
				return parent;
			}
			int indexInParent = parent.IndexInParent;
			MarkupDataContainer markupDataContainer = parent.SplitAt(itemIndex);
			if (markupDataContainer.Count > 0)
			{
				parent.Parent.InsertAt(indexInParent + 1, markupDataContainer);
			}
			return markupDataContainer;
		}
	}
}
