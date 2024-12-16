using System;

namespace Sdl.LanguagePlatform.Core
{
	[Flags]
	public enum SearchOption
	{
		None = 0x0,
		CaseInsensitive = 0x1,
		DiacriticsInsensitive = 0x2
	}
}
