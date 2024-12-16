using System;
using System.Globalization;

namespace Sdl.Core.TM.ImportExport
{
	public class LanguageMismatchException : Exception
	{
		public CultureInfo OriginSourceLanguage;

		public CultureInfo OriginTargetLanguage;

		public CultureInfo DestinationSourceLanguage;

		public CultureInfo DestinationTargetLanguage;

		public LanguageMismatchException(CultureInfo originSourceLanguage, CultureInfo originTargetLanguage, CultureInfo destinationSourceLanguage, CultureInfo destinationTargetLanguage)
		{
			OriginSourceLanguage = originSourceLanguage;
			OriginTargetLanguage = originTargetLanguage;
			DestinationSourceLanguage = destinationSourceLanguage;
			DestinationTargetLanguage = destinationTargetLanguage;
		}
	}
}
