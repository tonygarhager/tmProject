using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents setup information retrieved from a legacy translation memory (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyTranslationMemory.GetSetup" />).
	/// </summary>
	public interface ILegacyTranslationMemorySetup
	{
		/// <summary>
		/// Gets the name of the translation memory.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets the language directions of the translation memory.
		/// </summary>
		ILegacyLanguageDirectionData[] LanguageDirections
		{
			get;
		}

		/// <summary>
		/// Gets the total number of translation units across all available language directions. Returns <code>-1</code> if this is not available 
		/// (or cannot easily be obtained).
		/// </summary>
		int TotalTranslationUnitCount
		{
			get;
		}

		/// <summary>
		/// Gets the fields present in the legacy translation memory.
		/// </summary>
		IList<FieldDefinition> Fields
		{
			get;
		}

		/// <summary>
		/// Gets the copyright string. Can be null.
		/// </summary>
		string Copyright
		{
			get;
		}

		/// <summary>
		/// Gets the translation memory description. Can be null.
		/// </summary>
		string Description
		{
			get;
		}

		/// <summary>
		/// Gets which entities are being recognized when working with the translation memory.
		/// </summary>
		BuiltinRecognizers Recognizers
		{
			get;
		}

		/// <summary>
		/// Gets the expiration date of the translation memory. Can be null.
		/// </summary>
		DateTime? ExpirationDate
		{
			get;
		}

		/// <summary>
		/// Gets the index types that are present in the translation memory.
		/// </summary>
		FuzzyIndexes FuzzyIndexes
		{
			get;
		}

		/// <summary>
		/// Gets or sets the flags affecting tokenizer behaviour for this TM.
		/// <remarks>Note that changing tokenizer flags may require reindexing.</remarks>
		/// </summary>
		TokenizerFlags TokenizerFlags
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the flags affecting word count behaviour for this TM.
		/// </summary>
		WordCountFlags WordCountFlags
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the fine-grained alignment support for this TM.
		/// </summary>
		FGASupport FGASupport
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyTranslationMemorySetup.TextContextMatchType" /> specified for this TM when it was created.
		/// </summary>
		TextContextMatchType TextContextMatchType
		{
			get;
			set;
		}

		/// <summary>
		/// Indicates whether this TM was created with IdContextMatch support
		/// </summary>
		bool UsesIdContextMatch
		{
			get;
			set;
		}
	}
}
