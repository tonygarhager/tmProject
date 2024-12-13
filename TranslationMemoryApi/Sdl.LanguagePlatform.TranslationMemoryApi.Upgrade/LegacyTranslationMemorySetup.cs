using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class LegacyTranslationMemorySetup : ILegacyTranslationMemorySetup
	{
		public string Name
		{
			get;
			set;
		}

		public ILegacyLanguageDirectionData[] LanguageDirections
		{
			get;
			set;
		}

		public int TotalTranslationUnitCount
		{
			get;
			set;
		}

		public IList<FieldDefinition> Fields
		{
			get;
			set;
		}

		public string Copyright
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public BuiltinRecognizers Recognizers
		{
			get;
			set;
		}

		public DateTime? ExpirationDate
		{
			get;
			set;
		}

		public FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		public TokenizerFlags TokenizerFlags
		{
			get;
			set;
		}

		public WordCountFlags WordCountFlags
		{
			get;
			set;
		}

		public FGASupport FGASupport
		{
			get;
			set;
		}

		public TextContextMatchType TextContextMatchType
		{
			get;
			set;
		}

		public bool UsesIdContextMatch
		{
			get;
			set;
		}

		public LegacyTranslationMemorySetup()
		{
			FuzzyIndexes = (FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetWordBased);
			Recognizers = (BuiltinRecognizers.RecognizeDates | BuiltinRecognizers.RecognizeTimes | BuiltinRecognizers.RecognizeNumbers | BuiltinRecognizers.RecognizeAcronyms | BuiltinRecognizers.RecognizeVariables | BuiltinRecognizers.RecognizeMeasurements);
			WordCountFlags = WordCountFlags.BreakOnTag;
			TokenizerFlags = TokenizerFlags.DefaultFlags;
			TextContextMatchType = TextContextMatchType.PrecedingSourceAndTarget;
		}
	}
}
