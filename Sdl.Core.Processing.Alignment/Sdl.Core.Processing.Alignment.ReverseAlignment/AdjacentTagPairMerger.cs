using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	public class AdjacentTagPairMerger
	{
		private IDocumentItemFactory _itemFactory = new DocumentItemFactory();

		public void MergeAdjacentTagPairs(IAbstractMarkupDataContainer container)
		{
			ITagPair tagPair = null;
			ITagPair tagPair2 = null;
			bool flag = false;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < container.Count; i++)
			{
				ITagPair tagPair3 = container[i] as ITagPair;
				if (tagPair3 == null)
				{
					tagPair = null;
					tagPair2 = null;
					continue;
				}
				MergeAdjacentTagPairs(tagPair3);
				if (tagPair == null)
				{
					tagPair = tagPair3;
					continue;
				}
				if (StartTagPropertiesMatch(tagPair3, tagPair))
				{
					tagPair2 = tagPair3;
				}
				else
				{
					tagPair = tagPair3;
				}
				if (tagPair2 != null)
				{
					num = tagPair.IndexInParent;
					num2 = tagPair2.IndexInParent;
					if (LookAhead(tagPair, tagPair2, container))
					{
						continue;
					}
					int indexInParent = tagPair.IndexInParent;
					MergeTags(container, tagPair, tagPair2);
					MergeAdjacentTagPairs(container[indexInParent] as IAbstractMarkupDataContainer);
					flag = true;
					tagPair2 = null;
				}
				if (flag)
				{
					flag = false;
					int num3 = num2 - num;
					i -= num3;
					tagPair = null;
				}
			}
		}

		private bool LookAhead(ITagPair startingTagPair, ITagPair lastMergeableTagPair, IAbstractMarkupDataContainer container)
		{
			if (lastMergeableTagPair.IndexInParent < container.Count - 1 && container[lastMergeableTagPair.IndexInParent + 1] is ITagPair && StartTagPropertiesMatch((ITagPair)container[lastMergeableTagPair.IndexInParent + 1], startingTagPair))
			{
				return true;
			}
			return false;
		}

		private void MergeTags(IAbstractMarkupDataContainer container, ITagPair startingTagPair, ITagPair lastMergeableTagPair)
		{
			IList<IAbstractMarkupData> list = new List<IAbstractMarkupData>();
			ITagPair tagPair = _itemFactory.CreateTagPair((IStartTagProperties)startingTagPair.StartTagProperties.Clone(), (IEndTagProperties)startingTagPair.EndTagProperties.Clone());
			for (int i = startingTagPair.IndexInParent; i <= lastMergeableTagPair.IndexInParent; i++)
			{
				ITagPair tagPair2 = container[i] as ITagPair;
				list.Add(tagPair2);
				tagPair2?.MoveAllItemsTo(tagPair, tagPair.Count);
			}
			container.Insert(startingTagPair.IndexInParent, tagPair);
			foreach (IAbstractMarkupData item in list)
			{
				container.Remove(item);
			}
		}

		private static bool StartTagPropertiesMatch(ITagPair currentTagPair, ITagPair startingTagPair)
		{
			if (currentTagPair.StartTagProperties == startingTagPair.StartTagProperties || currentTagPair.StartTagProperties.TagId.Equals(startingTagPair.StartTagProperties.TagId))
			{
				return currentTagPair.StartTagProperties.MetaDataContainsKey("SDL:AutoCloned");
			}
			return false;
		}
	}
}
