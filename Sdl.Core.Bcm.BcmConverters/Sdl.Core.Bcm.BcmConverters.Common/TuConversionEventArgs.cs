using Sdl.Core.Bcm.BcmModel;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	public class TuConversionEventArgs : EventArgs
	{
		public TranslationUnit TranslationUnit
		{
			get;
			set;
		}

		public ParagraphUnit ParagraphUnit
		{
			get;
			set;
		}

		public TuConversionEventArgs(TranslationUnit translationUnit, ParagraphUnit paragraphUnit)
		{
			TranslationUnit = translationUnit;
			ParagraphUnit = paragraphUnit;
		}
	}
}
