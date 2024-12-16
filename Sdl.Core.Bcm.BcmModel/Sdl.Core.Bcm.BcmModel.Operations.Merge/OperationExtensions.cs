using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Operations.Merge
{
	internal static class OperationExtensions
	{
		internal static List<MarkupData> GetElementsInBetween(this MarkupDataContainer first, MarkupDataContainer second)
		{
			List<MarkupData> list = new List<MarkupData>();
			List<MarkupData> list2 = first.ParentParagraph.AllSubItems.ToList();
			int num = list2.IndexOf(first);
			int num2 = list2.IndexOf(second);
			for (int i = num + 1; i < num2; i++)
			{
				MarkupData item = list2[i];
				if (first.AllSubItems.Contains(item) || first.Ancestors.Contains(item) || second.AllSubItems.Contains(item) || second.Ancestors.Contains(item) || list.OfType<MarkupDataContainer>().Any((MarkupDataContainer x) => x.AllSubItems.Contains(item)))
				{
					continue;
				}
				if (item.Equals(second))
				{
					break;
				}
				if (item.IsContainer)
				{
					MarkupDataContainer markupDataContainer = item as MarkupDataContainer;
					if (markupDataContainer.AllSubItems.Contains(second))
					{
						continue;
					}
				}
				list.Add(item);
			}
			return list;
		}

		public static bool Validate(this MarkupDataContainer container)
		{
			List<MarkupData> source = container.AllSubItems.Where((MarkupData x) => x.Parent != null && x.ParentParagraph != null && x.ParentParagraphUnit != null).ToList();
			return source.Any();
		}
	}
}
