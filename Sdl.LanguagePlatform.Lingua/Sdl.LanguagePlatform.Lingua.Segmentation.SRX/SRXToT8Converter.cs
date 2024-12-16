using Sdl.LanguagePlatform.Core.Segmentation;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Lingua.Segmentation.SRX
{
	public class SRXToT8Converter
	{
		public static SegmentationRules Convert(LanguageRule r, CultureInfo culture)
		{
			SegmentationRules segmentationRules = new SegmentationRules(culture, (r != null) ? r.Name : "Paragraph-no-rules");
			if (r == null)
			{
				return segmentationRules;
			}
			List<Rule> list = new List<Rule>();
			List<Rule> list2 = new List<Rule>();
			foreach (Rule rule in r.Rules)
			{
				if (rule.IsBreak)
				{
					list.Add(rule);
				}
				else
				{
					list2.Add(rule);
				}
			}
			foreach (Rule item2 in list)
			{
				SegmentationRule segmentationRule = new SegmentationRule();
				segmentationRule.Description.SetText(CultureInfo.InvariantCulture, "Auto-converted from SRX");
				segmentationRule.Origin = RuleOrigin.Migration;
				segmentationRule.Type = RuleType.Other;
				segmentationRule.MatchingContext = new SegmentationContext
				{
					PrecedingContext = new Context(item2.BeforeBreak ?? string.Empty),
					FollowingContext = new Context(item2.AfterBreak ?? string.Empty),
					ContextType = ContextType.Other
				};
				foreach (Rule item3 in list2)
				{
					SegmentationContext item = new SegmentationContext
					{
						PrecedingContext = new Context(item3.BeforeBreak ?? string.Empty),
						FollowingContext = new Context(item3.AfterBreak ?? string.Empty),
						ContextType = ContextType.OtherException
					};
					segmentationRule.Exceptions.Add(item);
				}
				segmentationRules.AddRule(segmentationRule);
			}
			return segmentationRules;
		}
	}
}
