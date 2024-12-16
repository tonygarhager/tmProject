using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.ObjectModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationMemory : ITranslationProvider
	{
		ReadOnlyCollection<LanguagePair> SupportedLanguageDirections
		{
			get;
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

		DateTime CreationDate
		{
			get;
		}

		string CreationUserName
		{
			get;
		}

		DateTime? ExpirationDate
		{
			get;
			set;
		}

		FieldDefinitionCollection FieldDefinitions
		{
			get;
		}

		LanguageResourceBundleCollection LanguageResourceBundles
		{
			get;
		}

		BuiltinRecognizers Recognizers
		{
			get;
			set;
		}

		FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		FuzzyIndexTuningSettings FuzzyIndexTuningSettings
		{
			get;
			set;
		}

		DateTime? FuzzyIndexStatisticsRecomputedAt
		{
			get;
		}

		int? FuzzyIndexStatisticsSize
		{
			get;
		}

		new ITranslationMemoryLanguageDirection GetLanguageDirection(LanguagePair languageDirection);

		void Save();

		void Delete();

		bool HasPermission(string permission);

		int GetTranslationUnitCount();

		bool ShouldRecomputeFuzzyIndexStatistics();

		void RecomputeFuzzyIndexStatistics();
	}
}
