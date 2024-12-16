using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.Extensions;
using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters;
using Sdl.Core.Bcm.BcmModel;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class BcmToTransUnitConverter
	{
		private readonly TransUnitConversionSettings _conversionSettings;

		public event EventHandler<BcmTuConversionEventArgs> TranslationUnitCreated;

		public BcmToTransUnitConverter()
		{
			_conversionSettings = TransUnitConversionSettings.BuildDefault();
		}

		public BcmToTransUnitConverter(TransUnitConversionSettings conversionSettings)
		{
			_conversionSettings = conversionSettings;
		}

		public TranslationUnit[] ConvertToTranslationUnits(string inputBcmJson)
		{
			ValidateArguments(inputBcmJson);
			Document inputDocument;
			try
			{
				inputDocument = JsonConvert.DeserializeObject<Document>(inputBcmJson);
			}
			catch (JsonReaderException innerException)
			{
				throw new InvalidOperationException("The input BCM JSON is invalid.", innerException);
			}
			return Convert(inputDocument);
		}

		public TranslationUnit[] ConvertToTranslationUnits(Document inputDocument)
		{
			ValidateArguments(inputDocument);
			return Convert(inputDocument);
		}

		public TranslationUnit[] ConvertToTranslationUnits(Fragment fragment)
		{
			ValidateArguments(fragment);
			return Convert(fragment);
		}

		protected virtual void OnTranslationUnitCreated(BcmTuConversionEventArgs e)
		{
			this.TranslationUnitCreated?.Invoke(this, e);
		}

		private TranslationUnit[] Convert(Document inputDocument)
		{
			switch (_conversionSettings.ConversionType)
			{
			case TransUnitConversionType.Bilingual:
			{
				BcmToBilingual bcmToBilingual = new BcmToBilingual();
				bcmToBilingual.TranslationUnitCreated += delegate(object sender, BcmTuConversionEventArgs args)
				{
					OnTranslationUnitCreated(args);
				};
				return bcmToBilingual.ConvertToTranslationUnits(inputDocument);
			}
			case TransUnitConversionType.Lingua:
			{
				BcmToLingua bcmToLingua = new BcmToLingua(inputDocument);
				bcmToLingua.TranslationUnitCreated += delegate(object sender, BcmTuConversionEventArgs args)
				{
					OnTranslationUnitCreated(args);
				};
				return bcmToLingua.ConvertToTranslationUnits(_conversionSettings.IncludeTokens, _conversionSettings.IncludeAlignmentData, _conversionSettings.IncludeUserNameSystemFields);
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private TranslationUnit[] Convert(Fragment fragment)
		{
			switch (_conversionSettings.ConversionType)
			{
			case TransUnitConversionType.Bilingual:
			{
				BcmToBilingual bcmToBilingual = new BcmToBilingual();
				return bcmToBilingual.ConvertToTranslationUnits(fragment);
			}
			case TransUnitConversionType.Lingua:
			{
				ParagraphUnit paragraphUnit = fragment.CreateParagraphUnit();
				BcmToLingua bcmToLingua = new BcmToLingua(fragment);
				return bcmToLingua.ConvertToTranslationUnits(paragraphUnit, _conversionSettings.IncludeTokens, _conversionSettings.IncludeAlignmentData, _conversionSettings.IncludeUserNameSystemFields, fragment.GetFile());
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private static void ValidateArguments(string inputBcmJson)
		{
			if (inputBcmJson == null)
			{
				throw new ArgumentNullException("inputBcmJson", "The input BCM JSON cannot be null.");
			}
			if (inputBcmJson == string.Empty)
			{
				throw new ArgumentException("The input BCM JSON cannot be empty.", "inputBcmJson");
			}
		}

		private void ValidateArguments(Document inputDocument)
		{
			if (inputDocument == null)
			{
				throw new ArgumentNullException("inputDocument", "The input BCM document cannot be null.");
			}
		}

		private void ValidateArguments(Fragment inputFragment)
		{
			if (inputFragment == null)
			{
				throw new ArgumentNullException("inputFragment", "The input fragment cannot be null.");
			}
		}
	}
}
