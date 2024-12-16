using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ILegacyTranslationMemorySetup
	{
		string Name
		{
			get;
		}

		ILegacyLanguageDirectionData[] LanguageDirections
		{
			get;
		}

		int TotalTranslationUnitCount
		{
			get;
		}

		IList<FieldDefinition> Fields
		{
			get;
		}

		string Copyright
		{
			get;
		}

		string Description
		{
			get;
		}

		BuiltinRecognizers Recognizers
		{
			get;
		}

		DateTime? ExpirationDate
		{
			get;
		}

		FuzzyIndexes FuzzyIndexes
		{
			get;
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
	}
}
