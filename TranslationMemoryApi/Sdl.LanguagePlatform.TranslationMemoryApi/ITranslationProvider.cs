using Sdl.LanguagePlatform.Core;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a multi-lingual translation engine. This can for instance be a translation memory or machine translation engine. 
	/// </summary>
	public interface ITranslationProvider
	{
		/// <summary>
		/// Gets the status info for the provider.
		/// </summary>
		ProviderStatusInfo StatusInfo
		{
			get;
		}

		/// <summary>
		///  Gets a URI which uniquely identifies this translation provider.
		/// </summary>
		Uri Uri
		{
			get;
		}

		/// <summary>
		/// Gets the user-friendly name of this provider. It is not necessarily unique across the system.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether or not the translation provider engine supports 
		/// processing of tagged input and will return input tags in the translation proposals. 
		/// If false, the engine may drop all tags from search segments, and may not return tags
		/// in the output.
		/// </summary>
		bool SupportsTaggedInput
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider produces scored results 
		/// in the rank between [minScore, 100]. If false, no scores will be computed.
		/// </summary>
		bool SupportsScoring
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports searching 
		/// for bilingual input TUs (i.e. the SearchTranslationUnit
		/// method group). If false, the service only supports searching for simple (monolingual) 
		/// segments.
		/// </summary>
		bool SupportsSearchForTranslationUnits
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the engine may return multiple results for a search. 
		/// If false, at most one result (and translation proposal) will be computed.
		/// </summary>
		bool SupportsMultipleResults
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports filters for 
		/// text and attribute fields. If false, filters supplied in the search settings 
		/// will be ignored.
		/// </summary>
		bool SupportsFilters
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports penalties 
		/// of various types. If false, all penalties supplied in the search settings will be ignored.
		/// </summary>
		bool SupportsPenalties
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports a structure 
		/// context specification and will consider it for searches. If false, the structure 
		/// context will be ignored.
		/// </summary>
		bool SupportsStructureContext
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports document searches 
		/// and will take document (textual) context into account.
		/// </summary>
		bool SupportsDocumentSearches
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports updating/adding 
		/// of translation units. If false, all update/add methods will return without effect. Note 
		/// that implementations should not throw exceptions for unsupported methods.
		/// </summary>
		bool SupportsUpdate
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports the detection 
		/// and handling of placeables, and will return them as part of the search result.
		/// </summary>
		bool SupportsPlaceables
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports translation 
		/// search (exact and/or fuzzy search) and the generation of translation proposals. In most 
		/// cases this flag will be <c>true</c>. If this 
		/// value is <c>false</c>, the translation provider can not be used to request translations, 
		/// but can still be used to search for concordance matches, if <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsConcordanceSearch" />
		/// is <c>true</c>. 
		/// </summary>
		bool SupportsTranslation
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports fuzzy search. 
		/// This value should be <c>false</c> if <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTranslation" /> is <c>false</c>.
		/// </summary>
		bool SupportsFuzzySearch
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports concordance 
		/// search in the source or the target language. For more detailed information, 
		/// see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSourceConcordanceSearch" /> 
		/// and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTargetConcordanceSearch" />.
		/// </summary>
		bool SupportsConcordanceSearch
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports concordance 
		/// search in the source language.
		/// </summary>
		bool SupportsSourceConcordanceSearch
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports concordance 
		/// search in the target language.
		/// </summary>
		bool SupportsTargetConcordanceSearch
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider will compute and return 
		/// word counts in searches.
		/// </summary>
		bool SupportsWordCounts
		{
			get;
		}

		/// <summary>
		/// Gets the primary method how the translation provider generates translations.
		/// </summary>
		TranslationMethod TranslationMethod
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider or its underlying storage 
		/// are read-only. If this flag is <c>true</c>, no data can be deleted, updated, or 
		/// added.
		/// </summary>
		bool IsReadOnly
		{
			get;
		}

		/// <summary>
		/// Checks whether this translation provider supports the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>True if the specified language direction is supported.</returns>
		bool SupportsLanguageDirection(LanguagePair languageDirection);

		/// <summary>
		/// Gets a translation provider for the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>The language direction matching the given source and target language.</returns>
		ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection);

		/// <summary>
		/// Ensures that the provider's status information (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.StatusInfo" />) is refreshed, 
		/// in case it is cached.
		/// </summary>
		void RefreshStatusInfo();

		/// <summary>
		/// Serializes any meaningful state information for this translation provider that can be stored in projects 
		/// and sent around the supply chain.
		/// </summary>
		/// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
		/// <returns>A string representing the state of this translation provider that can later be passed into 
		/// the <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.LoadState(System.String)" /> method to restore the state after creating a new translation provider.</returns>
		string SerializeState();

		/// <summary>
		/// Loads previously serialized state information into this translation provider instance.
		/// </summary>
		/// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
		/// <param name="translationProviderState">A string representing the state of translation provider that was previously saved
		/// using <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SerializeState" />.</param>
		void LoadState(string translationProviderState);
	}
}
