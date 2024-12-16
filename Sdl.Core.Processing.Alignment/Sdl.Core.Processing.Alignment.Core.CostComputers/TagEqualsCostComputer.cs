using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class TagEqualsCostComputer : AbstractTagCostComputer
	{
		protected override AlignmentCost GetAlignmentCost(IEnumerable<Tag> sourceTags, IEnumerable<Tag> targetTags)
		{
			if (sourceTags.Count() == 0 && targetTags.Count() == 0)
			{
				return AlignmentCost.MinValue;
			}
			if (sourceTags.Count() != targetTags.Count())
			{
				return AlignmentCost.MaxValue;
			}
			ICollection<Tag> collection = new List<Tag>(targetTags);
			foreach (Tag sourceTag in sourceTags)
			{
				bool flag = false;
				foreach (Tag item in collection)
				{
					if (AreTagsAligned(sourceTag, item))
					{
						collection.Remove(item);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return AlignmentCost.MaxValue;
				}
			}
			return AlignmentCost.MinValue;
		}

		private static bool AreTagsAligned(Tag sourceTag, Tag targetTag)
		{
			AlignmentTag alignmentTag = sourceTag as AlignmentTag;
			AlignmentTag alignmentTag2 = targetTag as AlignmentTag;
			if (alignmentTag != null && alignmentTag2 != null)
			{
				IAbstractTag bilingualTag = alignmentTag.BilingualTag;
				IAbstractTag bilingualTag2 = alignmentTag2.BilingualTag;
				return TagAligner.AreTagsAlignable(bilingualTag, bilingualTag2);
			}
			return false;
		}
	}
}
