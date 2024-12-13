using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.ObjectModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a translation memory.
	/// </summary>
	public interface ITranslationMemory : ITranslationProvider
	{
		/// <summary>
		/// Gets the list of language directions which are supported by this translation memory.
		/// </summary>
		ReadOnlyCollection<LanguagePair> SupportedLanguageDirections
		{
			get;
		}

		/// <summary>
		/// Gets or sets a general description of the translation memory.
		/// </summary>
		/// <remarks>You have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> in order to persist changes to this property.</remarks>
		string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the copyright string for this translation memory.
		/// </summary>
		/// <remarks>You have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> in order to persist changes to this property.</remarks>
		string Copyright
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the creation date of this translation memory.
		/// </summary>
		DateTime CreationDate
		{
			get;
		}

		/// <summary>
		/// Gets the creation user of this translation memory.
		/// </summary>
		string CreationUserName
		{
			get;
		}

		/// <summary>
		/// Gets or sets the expiration date for this translation memory.
		/// </summary>
		/// <remarks>You have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> in order to persist changes to this property.</remarks>
		DateTime? ExpirationDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the custom fields defined for this TM.
		/// </summary>
		/// <remarks>In case this is a server-based translation memory, which is associated with a fields template (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.IFieldsTemplate" />),
		/// this returns a read-only fields collection identical to the template's fields collection. In all other cases,
		/// the field collection returned can be modified. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> to persist any changes made to
		/// the fields.</remarks>
		FieldDefinitionCollection FieldDefinitions
		{
			get;
		}

		/// <summary>
		/// Gets the language resources which are associated with this TM.
		/// </summary>
		/// <remarks>In case this is a server-based translation memory, which is associated with a language resources template (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ILanguageResourcesTemplate" />),
		/// this returns a read-only language resources collection identical to the template's language resources collection. In all other cases,
		/// the language resources collection returned can be modified. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> to persist any changes made to
		/// language resources.</remarks>
		LanguageResourceBundleCollection LanguageResourceBundles
		{
			get;
		}

		/// <summary>
		/// Gets or sets the recognizers which are enabled for this TM. 
		/// <remarks>Note that changing recognizers may require reindexing. In addition, in 
		/// some cases duplicate TUs may be in the TM if recognizers are enabled which have 
		/// been disabled before.</remarks>
		/// </summary>
		BuiltinRecognizers Recognizers
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the set of fuzzy indices defined on this TM.
		/// </summary>
		FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the fuzzy index tuning settings for the TM.
		/// </summary>
		FuzzyIndexTuningSettings FuzzyIndexTuningSettings
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the time of the last fuzzy index statistics recomputation of this TM, if available.
		/// </summary>
		DateTime? FuzzyIndexStatisticsRecomputedAt
		{
			get;
		}

		/// <summary>
		/// Gets the size of the TM at the point of the last fuzzy index statistics recomputation, 
		/// if available.
		/// </summary>
		int? FuzzyIndexStatisticsSize
		{
			get;
		}

		/// <summary>
		/// Gets a specified translation memory language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>A translation provider for the specified language direction, or null if no language direction matches.</returns>
		new ITranslationMemoryLanguageDirection GetLanguageDirection(LanguagePair languageDirection);

		/// <summary>
		/// Saves changes made to properties of this translation memory.
		/// </summary>
		void Save();

		/// <summary>
		/// Deletes this translation memory.
		/// </summary>
		/// <remarks>In the case of a file-based translation memory, this deletes the translation memory file itself. 
		/// Server-based translation memories are deleted from the server, including all their content.</remarks>
		void Delete();

		/// <summary>
		/// Checks whether the current user has the specified permission on this translation memory.
		/// </summary>
		/// <param name="permission">A permission ID. See <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryPermissions" />.</param>
		/// <returns>True if the user has the specified permission for this TM.</returns>
		bool HasPermission(string permission);

		/// <summary>
		/// Gets the total translation unit count for all language directions in this translation memory.
		/// </summary>
		/// <returns>The total translation unit count for this TM.</returns>
		int GetTranslationUnitCount();

		/// <summary>
		/// Gets a flag which indicates whether it is recommended to recompute the fuzzy
		/// index statistics (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.RecomputeFuzzyIndexStatistics" />).
		/// </summary>
		/// <returns></returns>
		bool ShouldRecomputeFuzzyIndexStatistics();

		/// <summary>
		/// Synchronously recomputes the fuzzy index statistics of the TM.
		/// </summary>
		void RecomputeFuzzyIndexStatistics();
	}
}
