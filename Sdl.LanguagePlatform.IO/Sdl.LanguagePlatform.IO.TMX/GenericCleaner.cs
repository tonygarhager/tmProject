using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class GenericCleaner
	{
		public static bool ApplyCleanupHeuristics(TranslationUnit tu)
		{
			bool result = false;
			if (ApplyCleanupHeuristics(tu.SourceSegment))
			{
				result = true;
			}
			if (ApplyCleanupHeuristics(tu.TargetSegment))
			{
				result = true;
			}
			return result;
		}

		private static bool ApplyCleanupHeuristics(Segment segment)
		{
			bool flag = false;
			bool result = false;
			do
			{
				flag = false;
				if (segment.DeleteEmptyTagPairs(onlyInPeripheralPositions: true))
				{
					flag = true;
					result = true;
				}
			}
			while (flag);
			return result;
		}
	}
}
