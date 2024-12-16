using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Flags]
	public enum TranslationMemoryProperties
	{
		None = 0x0,
		LanguageDirections = 0x1,
		Fields = 0x2,
		LanguageResources = 0x4,
		LanguageResourceData = 0x8,
		Container = 0x10,
		ScheduledOperations = 0x20,
		All = 0x3F
	}
}
