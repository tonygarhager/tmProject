using Sdl.Core.LanguageProcessing.ICU2;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing
{
	internal class StandardWordBoundaryFinder : IWordBoundaryFinder
	{
		private readonly WordBoundaryFinder _finder = new WordBoundaryFinder();

		public List<int> FindJaBoundaries(string jastr)
		{
			return _finder.FindJaBoundaries(jastr);
		}

		public List<int> FindZhBoundaries(string zhstr, string culture)
		{
			return _finder.FindZhBoundaries(zhstr, culture);
		}
	}
}
