using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class LegacyLanguagePair : ILegacyLanguageDirectionData
	{
		public LegacyLanguage SourceLanguage
		{
			get;
			set;
		}

		public LegacyLanguage TargetLanguage
		{
			get;
			set;
		}

		public int TranslationUnitCount
		{
			get;
			set;
		}

		public ILegacyLanguageResources[] AvailableLanguageResources
		{
			get;
			set;
		}

		public ILegacyLanguageResources SuggestedLanguageResources
		{
			get;
			set;
		}

		public LegacyLanguagePair(LegacyLanguage sourceLanguage, LegacyLanguage targetLanguage)
		{
			SourceLanguage = (sourceLanguage ?? throw new ArgumentNullException("sourceLanguage"));
			TargetLanguage = (targetLanguage ?? throw new ArgumentNullException("targetLanguage"));
		}
	}
}
