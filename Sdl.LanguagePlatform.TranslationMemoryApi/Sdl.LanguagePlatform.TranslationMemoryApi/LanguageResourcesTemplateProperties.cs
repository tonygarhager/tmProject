using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Flags]
	public enum LanguageResourcesTemplateProperties
	{
		None = 0x0,
		LanguageResources = 0x1,
		LanguageResourcesData = 0x2,
		TranslationMemories = 0x4,
		All = 0x7
	}
}
