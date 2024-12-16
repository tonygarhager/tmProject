using System;

namespace Sdl.Core.Bcm.BcmModel.Tokenization
{
	[Flags]
	public enum DateTimePatternType
	{
		LongDate = 0x1,
		ShortDate = 0x2,
		ShortTime = 0x4,
		LongTime = 0x8
	}
}
