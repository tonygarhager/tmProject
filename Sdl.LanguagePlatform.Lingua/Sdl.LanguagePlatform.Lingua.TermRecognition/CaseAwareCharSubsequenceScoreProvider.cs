using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Lingua.Alignment;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	public class CaseAwareCharSubsequenceScoreProvider : ISequenceAlignmentItemScoreProvider<char>
	{
		private readonly bool _useToBase;

		private readonly bool _normalizeWidths;

		public bool MaySkip => true;

		public CaseAwareCharSubsequenceScoreProvider(bool useToBase = true)
		{
			_useToBase = useToBase;
		}

		internal CaseAwareCharSubsequenceScoreProvider(bool useToBase, bool normalizeWidths)
		{
			_useToBase = useToBase;
			_normalizeWidths = normalizeWidths;
		}

		public int GetAlignScore(char a, char b)
		{
			if (a == b)
			{
				return 2;
			}
			a = char.ToLowerInvariant(a);
			b = char.ToLowerInvariant(b);
			if (a == b)
			{
				return 1;
			}
			if (_useToBase)
			{
				a = CharacterProperties.ToBase(a);
				b = CharacterProperties.ToBase(b);
				if (a == b)
				{
					return 1;
				}
			}
			if (!_normalizeWidths)
			{
				return -1;
			}
			string input = string.Empty + a.ToString();
			string input2 = string.Empty + b.ToString();
			input = StringUtilities.HalfWidthToFullWidth2(input);
			input2 = StringUtilities.HalfWidthToFullWidth2(input2);
			if (string.CompareOrdinal(input, input2) == 0)
			{
				return 1;
			}
			return -1;
		}

		public int GetSourceSkipScore(char a)
		{
			return -1;
		}

		public int GetTargetSkipScore(char a)
		{
			return -1;
		}
	}
}
