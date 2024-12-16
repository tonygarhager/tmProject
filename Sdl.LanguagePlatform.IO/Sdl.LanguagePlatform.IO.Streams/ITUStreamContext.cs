using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;

namespace Sdl.LanguagePlatform.IO.Streams
{
	internal interface ITUStreamContext
	{
		CultureInfo SourceCulture
		{
			get;
		}

		CultureInfo TargetCulture
		{
			get;
		}

		bool CheckMatchingSublanguages
		{
			get;
		}

		FieldDefinitions FieldDefinitions
		{
			get;
		}

		bool MayAddNewFields
		{
			get;
			set;
		}
	}
}
