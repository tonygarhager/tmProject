using System;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[Flags]
	public enum DateTimePatternType
	{
		Unknown = 0x0,
		LongDate = 0x1,
		ShortDate = 0x2,
		ShortTime = 0x4,
		LongTime = 0x8
	}
}
