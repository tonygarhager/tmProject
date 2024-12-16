using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing
{
	internal interface IWordBoundaryFinder
	{
		List<int> FindJaBoundaries(string jastr);

		List<int> FindZhBoundaries(string zhstr, string culture);
	}
}
