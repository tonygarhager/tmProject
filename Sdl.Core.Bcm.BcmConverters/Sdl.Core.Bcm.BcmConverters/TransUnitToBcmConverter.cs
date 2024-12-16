using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters;
using Sdl.Core.Bcm.BcmModel;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class TransUnitToBcmConverter
	{
		private readonly TransUnitConversionSettings _conversionSettings;

		public event EventHandler<TuConversionEventArgs> TranslationUnitConverted;

		public event EventHandler<BcmTuConversionEventArgs> TuProcessed;

		public TransUnitToBcmConverter()
		{
			_conversionSettings = TransUnitConversionSettings.BuildDefault();
		}

		public TransUnitToBcmConverter(TransUnitConversionSettings conversionSettings)
		{
			_conversionSettings = conversionSettings;
		}

		public string ConvertToJson(TranslationUnit[] inputTransUnits)
		{
			ValidateArguments(inputTransUnits);
			Document value = ConvertToDocument(inputTransUnits);
			return JsonConvert.SerializeObject(value);
		}

		public Document ConvertToBcmDocument(TranslationUnit[] inputTransUnits)
		{
			ValidateArguments(inputTransUnits);
			return ConvertToDocument(inputTransUnits);
		}

		public Fragment ConvertToFragment(TranslationUnit[] inputTransUnits)
		{
			ValidateArguments(inputTransUnits);
			return ConvertToBcmFragment(inputTransUnits);
		}

		protected virtual void OnTranslationUnitConverted(TuConversionEventArgs e)
		{
			this.TranslationUnitConverted?.Invoke(this, e);
		}

		private Document ConvertToDocument(TranslationUnit[] inputTransUnits)
		{
			switch (_conversionSettings.ConversionType)
			{
			case TransUnitConversionType.Bilingual:
				return GetDocumentFromBilingualTu(inputTransUnits);
			case TransUnitConversionType.Lingua:
				return GetDocumentFromLinguaTu(inputTransUnits);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private Fragment ConvertToBcmFragment(TranslationUnit[] inputTransUnits)
		{
			switch (_conversionSettings.ConversionType)
			{
			case TransUnitConversionType.Bilingual:
				return GetFragmentFromBilingualTu(inputTransUnits);
			case TransUnitConversionType.Lingua:
				return GetFragmentFromLinguaTu(inputTransUnits);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private Document GetDocumentFromBilingualTu(TranslationUnit[] inputTransUnits)
		{
			TranslationUnitToBilingualBcm translationUnitToBilingualBcm = new TranslationUnitToBilingualBcm(_conversionSettings);
			translationUnitToBilingualBcm.TranslationUnitConverted += delegate(object sender, TuConversionEventArgs args)
			{
				OnTranslationUnitConverted(args);
			};
			return translationUnitToBilingualBcm.ConvertToDocument(inputTransUnits);
		}

		private Document GetDocumentFromLinguaTu(TranslationUnit[] inputTransUnits)
		{
			LinguaToBcmConverter linguaToBcmConverter = new LinguaToBcmConverter();
			linguaToBcmConverter.TranslationUnitConverted += delegate(object sender, TuConversionEventArgs args)
			{
				OnTranslationUnitConverted(args);
			};
			return linguaToBcmConverter.ConvertToDocument(inputTransUnits, _conversionSettings.IncludeTokens, _conversionSettings.IncludeAlignmentData, _conversionSettings.IncludeUserNameSystemFields);
		}

		private Fragment GetFragmentFromBilingualTu(TranslationUnit[] inputTransUnits)
		{
			TranslationUnitToBilingualBcm translationUnitToBilingualBcm = new TranslationUnitToBilingualBcm(_conversionSettings);
			translationUnitToBilingualBcm.TranslationUnitConverted += delegate(object sender, TuConversionEventArgs args)
			{
				OnTranslationUnitConverted(args);
			};
			return translationUnitToBilingualBcm.ConvertToFragment(inputTransUnits);
		}

		private Fragment GetFragmentFromLinguaTu(TranslationUnit[] inputTransUnits)
		{
			LinguaToBcmConverter linguaToBcmConverter = new LinguaToBcmConverter();
			linguaToBcmConverter.TranslationUnitConverted += delegate(object sender, TuConversionEventArgs args)
			{
				OnTranslationUnitConverted(args);
			};
			return linguaToBcmConverter.ConvertToFragment(inputTransUnits, _conversionSettings.IncludeTokens, _conversionSettings.IncludeAlignmentData, _conversionSettings.IncludeUserNameSystemFields);
		}

		private static void ValidateArguments(ICollection<TranslationUnit> inputTranslationUnits)
		{
			if (inputTranslationUnits == null)
			{
				throw new ArgumentNullException("inputTranslationUnits", "Input translations units shouldn't be null.");
			}
			if (inputTranslationUnits.Count == 0)
			{
				throw new ArgumentException("Input translation units is empty.", "inputTranslationUnits");
			}
			if (inputTranslationUnits.Any((TranslationUnit tu) => tu == null))
			{
				throw new ArgumentNullException("inputTranslationUnits", "Input translation units array should not contain null items");
			}
		}
	}
}
