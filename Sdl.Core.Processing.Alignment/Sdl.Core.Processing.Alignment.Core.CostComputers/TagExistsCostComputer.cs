using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class TagExistsCostComputer : AbstractTagCostComputer
	{
		protected override AlignmentCost GetAlignmentCost(IEnumerable<Tag> sourceTags, IEnumerable<Tag> targetTags)
		{
			bool flag = AbstractTagCostComputer.HasTag(sourceTags, TagType.Start);
			bool flag2 = AbstractTagCostComputer.HasTag(targetTags, TagType.Start);
			if (flag != flag2)
			{
				return AlignmentCost.MaxValue;
			}
			bool flag3 = AbstractTagCostComputer.HasTag(sourceTags, TagType.Standalone);
			bool flag4 = AbstractTagCostComputer.HasTag(targetTags, TagType.Standalone);
			if (flag3 != flag4)
			{
				return AlignmentCost.MaxValue;
			}
			return AlignmentCost.MinValue;
		}
	}
}
