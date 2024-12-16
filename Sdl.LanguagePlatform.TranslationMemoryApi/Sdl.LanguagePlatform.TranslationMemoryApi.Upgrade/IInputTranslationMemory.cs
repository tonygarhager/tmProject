using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IInputTranslationMemory
	{
		ILegacyTranslationMemory TranslationMemory
		{
			get;
		}

		ILegacyTranslationMemorySetup Setup
		{
			get;
		}

		string TmxFilePath
		{
			get;
			set;
		}

		TmxFileStatus TmxFileStatus
		{
			get;
		}

		void ExportTmxFile(EventHandler<BatchExportedEventArgs> progressEventHandler);
	}
}
