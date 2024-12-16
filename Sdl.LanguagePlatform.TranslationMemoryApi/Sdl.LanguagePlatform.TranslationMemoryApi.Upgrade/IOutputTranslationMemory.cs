using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IOutputTranslationMemory
	{
		IInputLanguageDirectionDataCollection InputLanguageDirections
		{
			get;
		}

		ITranslationMemoryCreator TranslationMemoryCreator
		{
			get;
			set;
		}

		ITranslationMemorySetupOptions Setup
		{
			get;
		}

		int ExpectedTUCount
		{
			get;
		}

		ITranslationMemory TranslationMemory
		{
			get;
		}

		void PopulateSetup();

		void Process(EventHandler<ProgressEventArgs> progressEventHandler);

		void Validate();

		bool IsValid(out string errorMessage);

		void CreateTranslationMemory();

		void Import(IInputLanguageDirectionData languageDirectionData, EventHandler<ProgressEventArgs> progressEventHandler);
	}
}
