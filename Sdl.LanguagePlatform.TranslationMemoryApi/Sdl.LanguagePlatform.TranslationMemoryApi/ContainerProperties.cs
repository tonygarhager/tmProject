using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Flags]
	public enum ContainerProperties
	{
		None = 0x0,
		DatabaseServer = 0x1,
		TranslationMemories = 0x2,
		All = 0x3
	}
}
