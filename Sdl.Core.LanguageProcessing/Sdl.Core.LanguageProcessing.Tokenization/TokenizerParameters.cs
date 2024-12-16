using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class TokenizerParameters
	{
		private readonly List<Recognizer> _recognizers;

		private const int UriPriority = 100;

		private const int MeasurementPriority = 95;

		private const int CurrencyPriority = 90;

		private const int AcronymPriority = 85;

		private const int DatePriority = 80;

		private const int TimePriority = 75;

		private const int NumberPriority = 70;

		private const int IpPriority = 65;

		private const int AlphaNumericPriority = 60;

		private const int FallBackPriority = 0;

		public CultureInfo Culture
		{
			get;
		}

		internal Wordlist AdvancedTokenizationStopwordList
		{
			get;
		}

		public bool BreakOnWhitespace
		{
			get;
			set;
		}

		public bool CreateWhitespaceTokens
		{
			get;
			set;
		}

		internal bool ReclassifyAcronyms
		{
			get;
		}

		internal Recognizer this[int index] => _recognizers[index];

		public int Count => _recognizers.Count;

		public bool HasVariableRecognizer
		{
			get
			{
				if (Variables != null)
				{
					return Variables.Count > 0;
				}
				return false;
			}
		}

		public HashSet<string> Variables
		{
			get;
		}

		public string Signature
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<string> list = new List<string>();
				foreach (Recognizer recognizer in _recognizers)
				{
					list.Add(recognizer.GetSignature(Culture));
				}
				list.Sort((string a, string b) => string.CompareOrdinal(a, b));
				foreach (string item in list)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("-");
					}
					stringBuilder.Append(item);
				}
				stringBuilder.Append("CreateWhitespaceTokens");
				stringBuilder.Append(CreateWhitespaceTokens);
				stringBuilder.Append("BreakOnWhitespace");
				stringBuilder.Append(BreakOnWhitespace);
				if (Variables == null)
				{
					return stringBuilder.ToString();
				}
				foreach (string variable in Variables)
				{
					stringBuilder.Append("Var:");
					stringBuilder.Append(variable.Replace("|", "\\|"));
					stringBuilder.Append("|");
				}
				string twoLetterISOLanguageName = Culture.TwoLetterISOLanguageName;
				if (twoLetterISOLanguageName != null && twoLetterISOLanguageName == "ko")
				{
					stringBuilder.Append("-ko1-");
				}
				return stringBuilder.ToString();
			}
		}

		public TokenizerParameters(TokenizerSetup setup, IResourceDataAccessor accessor)
		{
			if (setup == null)
			{
				throw new ArgumentNullException("setup");
			}
			if (accessor == null)
			{
				accessor = new ResourceFileResourceAccessor();
			}
			BreakOnWhitespace = setup.BreakOnWhitespace;
			CreateWhitespaceTokens = setup.CreateWhitespaceTokens;
			Culture = CultureInfoExtensions.GetCultureInfo(setup.CultureName);
			_recognizers = new List<Recognizer>();
			ReclassifyAcronyms = false;
			CultureInfo cultureInfo = Culture;
			if (Culture.IsNeutralCulture)
			{
				cultureInfo = CultureInfoExtensions.GetRegionQualifiedCulture(Culture);
			}
			RecognizerSettings settings = new RecognizerSettings
			{
				BreakOnDash = ((setup.TokenizerFlags & TokenizerFlags.BreakOnDash) > TokenizerFlags.NoFlags),
				BreakOnHyphen = ((setup.TokenizerFlags & TokenizerFlags.BreakOnHyphen) > TokenizerFlags.NoFlags),
				BreakOnApostrophe = ((setup.TokenizerFlags & TokenizerFlags.BreakOnApostrophe) > TokenizerFlags.NoFlags)
			};
			if ((setup.BuiltinRecognizers & BuiltinRecognizers.RecognizeDates) != 0)
			{
				AddRecognizer(DateTimeRecognizer.Create(settings, accessor, cultureInfo, DateTimePatternType.LongDate | DateTimePatternType.ShortDate, 80));
			}
			if ((setup.BuiltinRecognizers & BuiltinRecognizers.RecognizeTimes) != 0)
			{
				AddRecognizer(DateTimeRecognizer.Create(settings, accessor, cultureInfo, DateTimePatternType.ShortTime | DateTimePatternType.LongTime, 75));
			}
			if ((setup.BuiltinRecognizers & BuiltinRecognizers.RecognizeNumbers) != 0)
			{
				AddRecognizer(NumberFSTRecognizer.Create(settings, accessor, cultureInfo, 70));
			}
			if ((setup.BuiltinRecognizers & BuiltinRecognizers.RecognizeAlphaNumeric) != 0)
			{
				Recognizer r = new AlphanumRecognizer(settings, 60, cultureInfo);
				AddRecognizer(r);
			}
			if ((setup.BuiltinRecognizers & BuiltinRecognizers.RecognizeAcronyms) != 0)
			{
				Recognizer recognizer = CreateAcronymRecognizer(settings, cultureInfo, 85);
				if (recognizer != null)
				{
					ReclassifyAcronyms = true;
					AddRecognizer(recognizer);
				}
				recognizer = CreateUriRecognizer(settings, cultureInfo, 100, setup.Culture);
				AddRecognizer(recognizer);
				AddRecognizer(CreateIpAddressRecognizer(settings, 65, setup.Culture));
				AddRecognizer(CreateEmailRecognizer(settings, cultureInfo, 100, setup.Culture));
			}
			if ((setup.BuiltinRecognizers & BuiltinRecognizers.RecognizeVariables) != 0)
			{
				try
				{
					Wordlist wordlist = new Wordlist();
					using (Stream stream = accessor.ReadResourceData(Culture, LanguageResourceType.Variables, fallback: true))
					{
						if (stream != null)
						{
							wordlist.Load(stream);
						}
					}
					Variables = InitializeVariables(cultureInfo, wordlist.Items);
				}
				catch
				{
				}
			}
			if ((setup.BuiltinRecognizers & BuiltinRecognizers.RecognizeMeasurements) != 0)
			{
				Recognizer r2 = MeasureFSTRecognizer.Create(settings, accessor, cultureInfo, 95);
				AddRecognizer(r2);
				r2 = CreateCurrencyRecognizer(settings, accessor, cultureInfo);
				AddRecognizer(r2);
			}
			bool separateClitics = setup.SeparateClitics && CultureInfoExtensions.UsesClitics(Culture);
			DefaultFallbackRecognizer r3 = CreateDefaultFallbackRecognizer(settings, separateClitics, accessor);
			AddRecognizer(r3);
			if (TokenizerHelper.UsesAdvancedTokenization(Culture))
			{
				try
				{
					Wordlist wordlist2 = new Wordlist();
					using (Stream stream2 = accessor.ReadResourceData(Culture, LanguageResourceType.Stopwords, fallback: true))
					{
						if (stream2 != null)
						{
							wordlist2.Load(stream2);
						}
					}
					AdvancedTokenizationStopwordList = wordlist2;
				}
				catch
				{
				}
			}
			SortRecognizers();
		}

		private static HashSet<string> InitializeVariables(CultureInfo actualCulture, IEnumerable<string> list)
		{
			if (!CultureInfoExtensions.UseFullWidth(actualCulture))
			{
				return new HashSet<string>(list);
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string item2 in list)
			{
				string item = StringUtilities.HalfWidthToFullWidth(item2);
				hashSet.Add(item);
			}
			return hashSet;
		}

		private static Recognizer CreateIpAddressRecognizer(RecognizerSettings settings, int priority, CultureInfo culture)
		{
			try
			{
				CharacterSet characterSet = new CharacterSet();
				characterSet.Add('0', '9');
				RegexRecognizer regexRecognizer = new RegexRecognizer(settings, TokenType.OtherTextPlaceable, priority, "IPADDRESS", "DEFAULT_IPADDRESS_RECOGNIZER", autoSubstitutable: true, culture);
				regexRecognizer.Add("\\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\\.((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))){3})\\b", characterSet);
				regexRecognizer.OnlyIfFollowedByNonwordCharacter = true;
				return regexRecognizer;
			}
			catch
			{
				return null;
			}
		}

		private static Recognizer CreateCurrencyRecognizer(RecognizerSettings settings, IResourceDataAccessor accessor, CultureInfo actualCulture)
		{
			return CurrencyFSTRecognizer.Create(settings, actualCulture, 90, accessor, CurrencyFSTEx.GetDefaults(actualCulture, LanguageMetadata.GetOrCreateMetadata(actualCulture.Name), accessor));
		}

		private static Recognizer CreateAcronymRecognizer(RecognizerSettings settings, CultureInfo actualCulture, int priority)
		{
			AcronymRecognizer acronymRecognizer = new AcronymRecognizer(settings, priority, actualCulture);
			acronymRecognizer.OnlyIfFollowedByNonwordCharacter = CultureInfoExtensions.UseBlankAsWordSeparator(actualCulture);
			return acronymRecognizer;
		}

		private static RegexRecognizer CreateUriRecognizer(RecognizerSettings settings, CultureInfo actualCulture, int priority, CultureInfo culture)
		{
			RegexRecognizer regexRecognizer = new RegexRecognizer(settings, TokenType.Uri, priority, "URI", "DEFAULT_URI_REGOCNIZER", autoSubstitutable: false, culture);
			CharacterSet characterSet = new CharacterSet();
			characterSet.Add('h');
			characterSet.Add('H');
			characterSet.Add('m');
			characterSet.Add('M');
			characterSet.Add('f');
			characterSet.Add('F');
			regexRecognizer.Add("((https|http|ftp|file)://)[\\p{L}\\p{N}\\p{Pc}\\p{Pd}\\p{Po}\\p{S}-['\"<>]]*[\\p{L}\\p{N}\\p{Pc}\\p{Pd}\\p{S}/]", characterSet, caseInsensitive: true);
			regexRecognizer.OnlyIfFollowedByNonwordCharacter = CultureInfoExtensions.UseBlankAsWordSeparator(actualCulture);
			return regexRecognizer;
		}

		private static RegexRecognizer CreateEmailRecognizer(RecognizerSettings settings, CultureInfo actualCulture, int priority, CultureInfo culture)
		{
			EmailRecognizer emailRecognizer = new EmailRecognizer(settings, priority, culture);
			emailRecognizer.OnlyIfFollowedByNonwordCharacter = CultureInfoExtensions.UseBlankAsWordSeparator(actualCulture);
			return emailRecognizer;
		}

		private void AddRecognizer(Recognizer r)
		{
			if (r != null)
			{
				if (r.Priority < 0)
				{
					throw new ArgumentOutOfRangeException("r", "priority");
				}
				_recognizers.Add(r);
			}
		}

		private void SortRecognizers()
		{
			_recognizers.Sort((Recognizer a, Recognizer b) => b.Priority - a.Priority);
		}

		private DefaultFallbackRecognizer CreateDefaultFallbackRecognizer(RecognizerSettings settings, bool separateClitics, IResourceDataAccessor accessor)
		{
			switch (Culture.TwoLetterISOLanguageName)
			{
			case "ja":
			case "zh":
				return new DefaultJAZHFallbackRecognizer(settings, TokenType.Unknown, 0, Culture, accessor);
			case "th":
			case "km":
				return new DefaultThaiFallbackRecognizer(settings, TokenType.Unknown, 0, Culture, accessor);
			case "ko":
				return (_recognizers.FindIndex((Recognizer f) => f.Type == TokenType.Number) != -1) ? new DefaultKoreanFallbackRecognizer(settings, TokenType.Unknown, 0, Culture, accessor) : new DefaultFallbackRecognizer(settings, TokenType.Unknown, 0, Culture, accessor);
			default:
				return new DefaultFallbackRecognizer(settings, TokenType.Unknown, 0, Culture, accessor);
			}
		}
	}
}
