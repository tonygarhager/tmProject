using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ITranslationMemorySetupOptions
	{
		string Name
		{
			get;
			set;
		}

		string Description
		{
			get;
			set;
		}

		string Copyright
		{
			get;
			set;
		}

		DateTime? ExpirationDate
		{
			get;
			set;
		}

		ICollection<LanguagePair> LanguageDirections
		{
			get;
		}

		FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		BuiltinRecognizers Recognizers
		{
			get;
			set;
		}

		TokenizerFlags TokenizerFlags
		{
			get;
			set;
		}

		WordCountFlags WordCountFlags
		{
			get;
			set;
		}

		FGASupport FGASupport
		{
			get;
			set;
		}

		IList<FieldDefinition> Fields
		{
			get;
			set;
		}

		IFieldIdentifierMappingsCollection InputFieldIdentifierMappings
		{
			get;
			set;
		}

		IDictionary<CultureInfo, ILegacyLanguageResources> LanguageResources
		{
			get;
			set;
		}

		TextContextMatchType TextContextMatchType
		{
			get;
			set;
		}

		bool UsesIdContextMatch
		{
			get;
			set;
		}

		bool UsesLegacyHashes
		{
			get;
			set;
		}
	}
}
