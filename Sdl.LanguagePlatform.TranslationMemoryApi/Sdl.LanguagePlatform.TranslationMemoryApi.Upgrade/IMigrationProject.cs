using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IMigrationProject
	{
		IInputTranslationMemoryCollection InputTranslationMemories
		{
			get;
		}

		IOutputTranslationMemoryCollection OutputTranslationMemories
		{
			get;
		}

		void ProcessAll(EventHandler<ProgressEventArgs> progressEventHandler);
	}
}
