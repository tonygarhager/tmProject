using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Operations
{
	public static class NormalizeSegmentExtensions
	{
		public static void NormalizeTextItems(this MarkupDataContainer segment)
		{
			int i = 1;
			IEnumerable<TextMarkup> enumerable = segment.AllSubItems.OfType<TextMarkup>();
			for (TextMarkup[] array = (enumerable as TextMarkup[]) ?? enumerable.ToArray(); i < array.Length; i++)
			{
				TextMarkup previousText = GetPreviousText(array, i);
				TextMarkup current = array[i];
				if (TextItemsNearEachOther(previousText, current))
				{
					MergeText(previousText, current);
				}
			}
		}

		private static TextMarkup GetPreviousText(TextMarkup[] textMarkups, int index)
		{
			while (index - 1 >= 0 && textMarkups[index - 1].Parent == null)
			{
				index--;
			}
			return textMarkups[index - 1];
		}

		private static bool TextItemsNearEachOther(TextMarkup previous, TextMarkup current)
		{
			if (previous.Parent.Equals(current.Parent))
			{
				return previous.IndexInParent + 1 == current.IndexInParent;
			}
			return false;
		}

		private static void MergeText(TextMarkup previous, TextMarkup current)
		{
			previous.Text += current.Text;
			previous.Parent.RemoveAt(current.IndexInParent);
		}

		public static void NormalizeRevisions(this MarkupDataContainer segment)
		{
			int i = 1;
			IEnumerable<RevisionContainer> enumerable = segment.AllSubItems.OfType<RevisionContainer>();
			for (RevisionContainer[] array = (enumerable as RevisionContainer[]) ?? enumerable.ToArray(); i < array.Length; i++)
			{
				RevisionContainer previousRevisions = GetPreviousRevisions(array, i);
				RevisionContainer revisionContainer = array[i];
				if (MergeableRevisionNearEachOther(previousRevisions, revisionContainer))
				{
					revisionContainer.MoveToContainer(previousRevisions, 0, previousRevisions.Count, revisionContainer.Count);
					revisionContainer.RemoveFromParent();
				}
			}
			segment.NormalizeTextItems();
		}

		private static bool MergeableRevisionNearEachOther(RevisionContainer previous, RevisionContainer current)
		{
			if (previous.Parent.Equals(current.Parent) && previous.IndexInParent + 1 == current.IndexInParent && previous.RevisionType == current.RevisionType)
			{
				return previous.Author == current.Author;
			}
			return false;
		}

		private static RevisionContainer GetPreviousRevisions(RevisionContainer[] revisions, int index)
		{
			while (index - 1 >= 0 && revisions[index - 1].Parent == null)
			{
				index--;
			}
			return revisions[index - 1];
		}
	}
}
