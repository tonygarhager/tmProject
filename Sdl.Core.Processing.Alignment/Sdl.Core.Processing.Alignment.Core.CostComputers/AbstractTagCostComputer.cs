using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal abstract class AbstractTagCostComputer : IAlignmentCostComputer
	{
		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			IEnumerable<Tag> sourceTags = CollectTags(sourceElements);
			IEnumerable<Tag> targetTags = CollectTags(targetElements);
			return GetAlignmentCost(sourceTags, targetTags);
		}

		protected abstract AlignmentCost GetAlignmentCost(IEnumerable<Tag> sourceTags, IEnumerable<Tag> targetTags);

		private static IEnumerable<Tag> CollectTags(IEnumerable<AlignmentElement> alignmentElements)
		{
			ICollection<Tag> collection = new List<Tag>();
			if (alignmentElements == null)
			{
				return collection;
			}
			foreach (AlignmentElement alignmentElement in alignmentElements)
			{
				foreach (SegmentElement element in alignmentElement.Content.Elements)
				{
					Tag tag = element as Tag;
					if (tag != null)
					{
						switch (tag.Type)
						{
						case TagType.Start:
							collection.Add(tag);
							break;
						case TagType.Standalone:
							collection.Add(tag);
							break;
						}
					}
				}
			}
			return collection;
		}

		protected static bool HasTag(IEnumerable<Tag> tags, TagType tagType)
		{
			return GetTagCount(tags, tagType) > 0;
		}

		protected static int GetTagCount(IEnumerable<Tag> tags, TagType tagType)
		{
			return tags.Count((Tag tag) => tag.Type == tagType);
		}
	}
}
