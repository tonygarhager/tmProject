using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class TagCountCostComputer : AbstractTagCostComputer
	{
		protected override AlignmentCost GetAlignmentCost(IEnumerable<Tag> sourceTags, IEnumerable<Tag> targetTags)
		{
			int tagCount = AbstractTagCostComputer.GetTagCount(sourceTags, TagType.Start);
			int tagCount2 = AbstractTagCostComputer.GetTagCount(targetTags, TagType.Start);
			if (tagCount != tagCount2)
			{
				return AlignmentCost.MaxValue;
			}
			int tagCount3 = AbstractTagCostComputer.GetTagCount(sourceTags, TagType.Standalone);
			int tagCount4 = AbstractTagCostComputer.GetTagCount(targetTags, TagType.Standalone);
			if (tagCount3 != tagCount4)
			{
				return AlignmentCost.MaxValue;
			}
			return AlignmentCost.MinValue;
		}
	}
}
