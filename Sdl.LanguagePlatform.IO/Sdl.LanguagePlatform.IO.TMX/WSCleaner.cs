using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class WSCleaner
	{
		public static bool ApplyCleanupHeuristics(TranslationUnit tu)
		{
			if (!ApplyCleanupHeuristics(tu.SourceSegment))
			{
				return ApplyCleanupHeuristics(tu.TargetSegment);
			}
			return true;
		}

		private static bool ApplyCleanupHeuristics(Segment segment)
		{
			return false;
		}
	}
}
