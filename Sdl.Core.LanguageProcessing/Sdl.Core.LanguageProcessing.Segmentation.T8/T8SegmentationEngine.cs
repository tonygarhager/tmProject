using Sdl.LanguagePlatform.Core.Segmentation;

namespace Sdl.Core.LanguageProcessing.Segmentation.T8
{
	public class T8SegmentationEngine : SegmentationEngine
	{
		private readonly SegmentationRules _rules;

		public T8SegmentationEngine(SegmentationRules rules)
			: base(rules.Culture)
		{
			_rules = (rules ?? throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationEmptyRuleSet));
		}

		public override Chunk GetNextChunk(string text, int startIndex, bool assumeEof, bool followedByWordBreak)
		{
			if (string.IsNullOrEmpty(text) || startIndex >= text.Length)
			{
				return null;
			}
			int neutralPrefixLength = GetNeutralPrefixLength(text, startIndex);
			if (neutralPrefixLength > 0)
			{
				return new Chunk(startIndex, neutralPrefixLength, ChunkType.Whitespace, SegmentationMethod.Whitespace);
			}
			int num = -1;
			foreach (SegmentationRule rule in _rules)
			{
				if (rule.IsEnabled)
				{
					int num2 = rule.FindFirstMatch(text, startIndex, assumeEof, followedByWordBreak);
					if (num2 >= 0 && (num < 0 || num2 < num))
					{
						num = num2;
					}
				}
			}
			if (num >= 0)
			{
				return new Chunk(startIndex, num - startIndex, ChunkType.BreakAfter, SegmentationMethod.Rule);
			}
			if (!assumeEof)
			{
				return null;
			}
			int num3 = text.Length - startIndex;
			while (num3 > 0 && char.IsWhiteSpace(text[startIndex + num3 - 1]))
			{
				num3--;
			}
			if (num3 == 0)
			{
				return null;
			}
			return new Chunk(startIndex, num3, ChunkType.BreakAfter, SegmentationMethod.EndOfInput);
		}
	}
}
