using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Flags]
	public enum DatabaseServerProperties
	{
		None = 0x0,
		Containers = 0x1,
		ContainerTranslationMemories = 0x2,
		All = 0x3
	}
}
