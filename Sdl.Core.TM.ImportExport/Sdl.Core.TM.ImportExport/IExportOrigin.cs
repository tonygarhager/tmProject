using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Globalization;

namespace Sdl.Core.TM.ImportExport
{
	public interface IExportOrigin
	{
		string Name
		{
			get;
		}

		CultureInfo SourceLanguage
		{
			get;
		}

		CultureInfo TargetLanguage
		{
			get;
		}

		TokenizerFlags TokenizerFlags
		{
			get;
		}

		WordCountFlags WordCountFlags
		{
			get;
		}

		TextContextMatchType TextContextMatchType
		{
			get;
		}

		bool UsesIdContextMatch
		{
			get;
		}

		bool IncludesContextContent
		{
			get;
		}

		FieldDefinitions FieldDefinitions
		{
			get;
		}

		LanguageResource[] LanguageResources
		{
			get;
		}

		DateTime CreationDate
		{
			get;
		}

		string CreationUserName
		{
			get;
		}

		BuiltinRecognizers BuiltinRecognizers
		{
			get;
		}

		TranslationUnit[] GetTranslationUnits(ref RegularIterator iter);
	}
}
