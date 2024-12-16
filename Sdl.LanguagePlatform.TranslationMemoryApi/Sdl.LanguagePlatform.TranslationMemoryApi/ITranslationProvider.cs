using Sdl.LanguagePlatform.Core;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationProvider
	{
		ProviderStatusInfo StatusInfo
		{
			get;
		}

		Uri Uri
		{
			get;
		}

		string Name
		{
			get;
		}

		bool SupportsTaggedInput
		{
			get;
		}

		bool SupportsScoring
		{
			get;
		}

		bool SupportsSearchForTranslationUnits
		{
			get;
		}

		bool SupportsMultipleResults
		{
			get;
		}

		bool SupportsFilters
		{
			get;
		}

		bool SupportsPenalties
		{
			get;
		}

		bool SupportsStructureContext
		{
			get;
		}

		bool SupportsDocumentSearches
		{
			get;
		}

		bool SupportsUpdate
		{
			get;
		}

		bool SupportsPlaceables
		{
			get;
		}

		bool SupportsTranslation
		{
			get;
		}

		bool SupportsFuzzySearch
		{
			get;
		}

		bool SupportsConcordanceSearch
		{
			get;
		}

		bool SupportsSourceConcordanceSearch
		{
			get;
		}

		bool SupportsTargetConcordanceSearch
		{
			get;
		}

		bool SupportsWordCounts
		{
			get;
		}

		TranslationMethod TranslationMethod
		{
			get;
		}

		bool IsReadOnly
		{
			get;
		}

		bool SupportsLanguageDirection(LanguagePair languageDirection);

		ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection);

		void RefreshStatusInfo();

		string SerializeState();

		void LoadState(string translationProviderState);
	}
}
