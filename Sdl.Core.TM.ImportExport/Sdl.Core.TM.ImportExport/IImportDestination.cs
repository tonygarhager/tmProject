using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.TM.ImportExport
{
	public interface IImportDestination
	{
		CultureInfo SourceLanguage
		{
			get;
		}

		CultureInfo TargetLanguage
		{
			get;
		}

		FieldDefinitions FieldDefinitions
		{
			get;
		}

		bool UsesLegacyHashes
		{
			get;
		}

		LanguageResource[] LanguageResources
		{
			get;
		}

		[Obsolete]
		ImportResult[] AddTranslationUnits(TranslationUnit[] tus, ImportSettings settings);

		ImportResult[] AddTranslationUnitsMask(TranslationUnit[] tus, ImportSettings settings, bool[] mask);

		void UpdateFieldDefinitions(FieldDefinitions mergedFieldDefinitions);

		void UpdateLanguageResources(List<LanguageResource> mergedLanguageResources);
	}
}
