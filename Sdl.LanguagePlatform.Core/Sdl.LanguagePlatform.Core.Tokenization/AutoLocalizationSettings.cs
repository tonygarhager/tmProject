using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	public class AutoLocalizationSettings
	{
		private char _UnitSeparator;

		[DataMember]
		public string LongDatePattern
		{
			get;
			set;
		}

		[DataMember]
		public string ShortDatePattern
		{
			get;
			set;
		}

		[DataMember]
		public string LongTimePattern
		{
			get;
			set;
		}

		[DataMember]
		public UnitSeparationMode UnitSeparationMode
		{
			get;
			set;
		}

		[DataMember]
		public char UnitSeparator
		{
			get
			{
				return _UnitSeparator;
			}
			set
			{
				if (value == '\0' || char.IsWhiteSpace(value))
				{
					_UnitSeparator = value;
					return;
				}
				throw new ArgumentOutOfRangeException("value");
			}
		}

		[DataMember]
		public string ShortTimePattern
		{
			get;
			set;
		}

		[DataMember]
		public LocalizationParametersSource LocalizationParametersSource
		{
			get;
			set;
		}

		[DataMember]
		public BuiltinRecognizers DisableAutoSubstitution
		{
			get;
			set;
		}

		[DataMember]
		public string NumberGroupSeparator
		{
			get;
			set;
		}

		[DataMember]
		public string NumberDecimalSeparator
		{
			get;
			set;
		}

		[DataMember]
		public string CurrencyGroupSeparator
		{
			get;
			set;
		}

		[DataMember]
		public string CurrencyDecimalSeparator
		{
			get;
			set;
		}

		[DataMember]
		public CurrencySymbolPosition? CurrencySymbolPosition
		{
			get;
			set;
		}

		[DataMember]
		public bool FormatDateTimeUniformly
		{
			get;
			set;
		}

		[DataMember]
		public bool FormatNumberSeparatorsUniformly
		{
			get;
			set;
		}

		[DataMember]
		public bool FormatCurrencySeparatorsUniformly
		{
			get;
			set;
		}

		[DataMember]
		public bool FormatCurrencyPositionUniformly
		{
			get;
			set;
		}

		public bool AttemptAutoSubstitution(Token t)
		{
			if (t == null)
			{
				throw new ArgumentNullException();
			}
			if (!t.IsSubstitutable)
			{
				return false;
			}
			switch (t.Type)
			{
			case TokenType.Date:
				return (DisableAutoSubstitution & BuiltinRecognizers.RecognizeDates) == 0;
			case TokenType.Time:
				return (DisableAutoSubstitution & BuiltinRecognizers.RecognizeTimes) == 0;
			case TokenType.Variable:
				return (DisableAutoSubstitution & BuiltinRecognizers.RecognizeVariables) == 0;
			case TokenType.Number:
				return (DisableAutoSubstitution & BuiltinRecognizers.RecognizeNumbers) == 0;
			case TokenType.Measurement:
				return (DisableAutoSubstitution & BuiltinRecognizers.RecognizeMeasurements) == 0;
			case TokenType.Acronym:
				return (DisableAutoSubstitution & BuiltinRecognizers.RecognizeAcronyms) == 0;
			case TokenType.AlphaNumeric:
				return (DisableAutoSubstitution & BuiltinRecognizers.RecognizeAlphaNumeric) == 0;
			default:
				return true;
			}
		}
	}
}
