using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Flags]
	public enum LanguageDirectionProperties
	{
		None = 0x0,
		Container = 0x1,
		Fields = 0x2,
		LanguageResources = 0x4,
		AssociatedImports = 0x8,
		AssociatedExports = 0x10,
		All = 0x1E
	}
}
