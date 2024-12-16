using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmModel;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters
{
	internal interface ITranslationUnitToBcm
	{
		event EventHandler<TuConversionEventArgs> TranslationUnitConverted;

		Document ConvertToDocument(TranslationUnit[] inputTransUnits);
	}
}
