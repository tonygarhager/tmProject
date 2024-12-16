using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal interface IAlignmentProvider
	{
		int Items
		{
			get;
		}

		AlignmentTable GetAlignment(int segmentNumber, out List<BilingualPhrase> phrases);

		void Close();
	}
}
