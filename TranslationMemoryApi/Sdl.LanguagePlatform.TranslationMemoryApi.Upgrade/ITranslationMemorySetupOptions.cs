using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents the setup information that will be used to create an output translation memory (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.Setup" />).
	/// </summary>
	public interface ITranslationMemorySetupOptions
	{
		/// <summary>
		/// Gets or sets the name of the translation memory.
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dsecription of the translation memory. Can be null.
		/// </summary>
		string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the copyright string of the translation memory. Can be null.
		/// </summary>
		string Copyright
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the expiration date of the translation memory. Can be null.
		/// </summary>
		DateTime? ExpirationDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the language direction of this translation memory.
		/// </summary>
		ICollection<LanguagePair> LanguageDirections
		{
			get;
		}

		/// <summary>
		/// Gets or sets the fuzzy indexes that will be created in the translation memory.
		/// </summary>
		FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets which entities should be recognized by the translation memory.
		/// </summary>
		BuiltinRecognizers Recognizers
		{
			get;
			set;
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
		/// Gets or sets the list of fields for this translation memory. When using an automatic method of generating the setup information,
		/// this list can contain multiple fields with the same name but of different types. These differences must be resolved before
		/// starting the migration process or will result in an exception being thrown.
		/// </summary>
		IList<FieldDefinition> Fields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the field identifier mappings, which map field names and types from the 
		/// input (legacy) field definitions to the output field definitions.
		/// </summary>
		IFieldIdentifierMappingsCollection InputFieldIdentifierMappings
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the language resources to use for this translation memory. Can be null, 
		/// in which case the default resources will be used.
		/// </summary>
		IDictionary<CultureInfo, ILegacyLanguageResources> LanguageResources
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITranslationMemorySetupOptions.TextContextMatchType" /> specified for this TM when it was created.
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

		/// <summary>
		/// Indicates whether this TM uses legacy hashes
		/// </summary>
		bool UsesLegacyHashes
		{
			get;
			set;
		}
	}
}
