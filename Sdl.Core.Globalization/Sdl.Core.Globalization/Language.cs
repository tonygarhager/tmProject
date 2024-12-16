using Newtonsoft.Json;
using Sdl.Core.Globalization.LanguageRegistry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;

namespace Sdl.Core.Globalization
{
	[Serializable]
	[DataContract]
	public class Language : ICloneable
	{
		private enum EvaluationResult
		{
			NotTested,
			NotSupported,
			Supported
		}

		private static LanguageDisplaySettings _defaultDisplaySettings;

		private static Dictionary<string, Image> _flags;

		private string _isoAbbreviation;

		private HashSet<string> _useBlankAsWordSeparatorExceptions;

		private HashSet<string> _useBlankAsSentenceSeparatorLanguagues;

		[NonSerialized]
		private CultureInfo _lazyCultureInfo;

		[NonSerialized]
		private EvaluationResult _evaluationResult;

		private string _languageCode;

		private bool _isNeutral;

		private string _englishName;

		private string _direction;

		public static LanguageDisplaySettings DefaultDisplaySettings => _defaultDisplaySettings;

		[DataMember]
		[JsonProperty(Order = 8)]
		public string IsoAbbreviation
		{
			get
			{
				return _isoAbbreviation;
			}
			set
			{
				_isoAbbreviation = value;
				_lazyCultureInfo = null;
				_evaluationResult = EvaluationResult.NotTested;
			}
		}

		[IgnoreDataMember]
		public CultureInfo CultureInfo
		{
			get
			{
				if (_isoAbbreviation != null)
				{
					EvaluateCultureInfo();
					if (_evaluationResult == EvaluationResult.NotSupported)
					{
						throw new UnsupportedLanguageException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedIsoAbbreviationMessage, new object[1]
						{
							_isoAbbreviation
						}));
					}
				}
				return _lazyCultureInfo;
			}
			set
			{
				if (value == null)
				{
					IsoAbbreviation = null;
					_evaluationResult = EvaluationResult.NotTested;
				}
				else
				{
					_isoAbbreviation = value.Name;
					_lazyCultureInfo = value;
					_evaluationResult = EvaluationResult.Supported;
				}
			}
		}

		public bool UseBlankAsWordSeparator
		{
			get
			{
				if (CultureInfo == null)
				{
					throw new ArgumentNullException();
				}
				return !_useBlankAsWordSeparatorExceptions.Contains(CultureInfo.TwoLetterISOLanguageName);
			}
		}

		public bool UseBlankAsSentenceSeparator
		{
			get
			{
				if (CultureInfo == null)
				{
					throw new ArgumentNullException();
				}
				return _useBlankAsSentenceSeparatorLanguagues.Contains(CultureInfo.TwoLetterISOLanguageName);
			}
		}

		public bool IsValid => !string.IsNullOrEmpty(_isoAbbreviation);

		public bool IsSupported
		{
			get
			{
				if (!IsValid)
				{
					return false;
				}
				EvaluateCultureInfo();
				return _evaluationResult == EvaluationResult.Supported;
			}
		}

		public bool UsesCharacterCounts
		{
			get
			{
				switch (IsoAbbreviation.ToLower(CultureInfo.InvariantCulture))
				{
				case "zh-hk":
				case "zh-mo":
				case "zh-cn":
				case "zh-chs":
				case "zh-sg":
				case "zh-tw":
				case "zh-cht":
				case "ja":
				case "ja-jp":
				case "th":
				case "th-th":
					return true;
				default:
					return false;
				}
			}
		}

		[DataMember]
		[JsonProperty(Order = 1)]
		public string LanguageCode
		{
			get
			{
				return _languageCode;
			}
			set
			{
				_languageCode = value;
			}
		}

		[JsonProperty(Order = 6)]
		[JsonConverter(typeof(ProductDictionaryConverter))]
		public Dictionary<Product, List<AlternativeLanguageCode>> SupportedProducts
		{
			get;
			set;
		}

		[JsonIgnore]
		public bool IsNeutral
		{
			get
			{
				return _isNeutral;
			}
			internal set
			{
				_isNeutral = value;
			}
		}

		[JsonIgnore]
		public string LdmlData
		{
			get;
		}

		[DataMember]
		[JsonProperty(Order = 2)]
		public string EnglishName
		{
			get
			{
				return _englishName;
			}
			set
			{
				_englishName = value;
			}
		}

		[DataMember]
		[JsonProperty(Order = 3)]
		public string Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		[JsonIgnore]
		public IList<Language> RegionalVariants
		{
			get;
			set;
		}

		[JsonIgnore]
		public Language ParentLanguage
		{
			get;
			internal set;
		}

		[DataMember]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Order = 4)]
		[DefaultValue("")]
		public string ParentLanguageCode
		{
			get;
			set;
		}

		[DataMember]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 5)]
		public string DefaultSpecificLanguageCode
		{
			get;
			set;
		}

		[JsonIgnore]
		public Language DefaultSpecificLanguage
		{
			get;
			internal set;
		}

		[DataMember]
		[Obsolete("This property is obsolete.Using DefaultSpecificLanguageCode recommended")]
		[JsonIgnore]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 7)]
		public string DefaultSpecificCultureCode
		{
			get;
			set;
		}

		[JsonIgnore]
		[Obsolete("This property is obsolete. Using DefaultSpecificLanguage recommended")]
		public Language DefaultSpecificCulture
		{
			get;
			set;
		}

		[DataMember]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Order = 9)]
		[DefaultValue("")]
		[Obsolete("This property is obsolete. Using ParentLanguageCode recommended")]
		public string ParentIsoCode
		{
			get;
			set;
		}

		public string DisplayName => GetDisplayName(DefaultDisplaySettings.Format, DefaultDisplaySettings.IncludeCountry);

		[Obsolete("This property is obsolete. Using GetFlagImage function recommended")]
		public Image Image => GetImage(DefaultDisplaySettings.UseFlags);

		static Language()
		{
			_defaultDisplaySettings = new LanguageDisplaySettings();
			try
			{
				_flags = new Dictionary<string, Image>();
			}
			catch (Exception)
			{
			}
		}

		public static bool IsNullOrInvalid(Language language)
		{
			if (language != null)
			{
				return !language.IsValid;
			}
			return true;
		}

		public static Language[] GetAllLanguages()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			Language[] array = new Language[cultures.Length];
			for (int i = 0; i < cultures.Length; i++)
			{
				array[i] = new Language(cultures[i]);
			}
			return array;
		}

		public static bool Equals(string isoAbbreviation1, string isoAbbreviation2)
		{
			return string.Equals(isoAbbreviation1, isoAbbreviation2, StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool Equals(Language language1, Language language2)
		{
			if (IsNullOrInvalid(language1) && IsNullOrInvalid(language2))
			{
				return true;
			}
			return language1.Equals(language2);
		}

		public Language(CultureInfo cultureInfo)
			: this()
		{
			if (cultureInfo != null)
			{
				_languageCode = cultureInfo.Name;
				_isoAbbreviation = cultureInfo.Name;
				_evaluationResult = EvaluationResult.Supported;
				_englishName = cultureInfo.EnglishName;
				_isNeutral = cultureInfo.IsNeutralCulture;
				if (IsoAbbreviation == "")
				{
					_isNeutral = true;
				}
				LdmlData = "";
				SupportedProducts = new Dictionary<Product, List<AlternativeLanguageCode>>();
			}
			else
			{
				_evaluationResult = EvaluationResult.NotTested;
			}
			_lazyCultureInfo = cultureInfo;
		}

		public Language(string isoAbbreviation)
			: this()
		{
			_isoAbbreviation = isoAbbreviation;
			_evaluationResult = EvaluationResult.NotTested;
		}

		public Language()
		{
			_useBlankAsWordSeparatorExceptions = new HashSet<string>(new string[6]
			{
				"th",
				"km",
				"ja",
				"chs",
				"cht",
				"zh"
			}, StringComparer.OrdinalIgnoreCase);
			_useBlankAsSentenceSeparatorLanguagues = new HashSet<string>(new string[2]
			{
				"th",
				"km"
			}, StringComparer.OrdinalIgnoreCase);
		}

		protected Language(Language other)
		{
			_evaluationResult = other._evaluationResult;
			_isoAbbreviation = other._isoAbbreviation;
			_lazyCultureInfo = other._lazyCultureInfo;
		}

		public object Clone()
		{
			return new Language(this);
		}

		private void EvaluateCultureInfo()
		{
			if (_evaluationResult == EvaluationResult.NotTested)
			{
				try
				{
					_lazyCultureInfo = new CultureInfo(IsoAbbreviation);
					_evaluationResult = EvaluationResult.Supported;
				}
				catch (ArgumentException)
				{
					_evaluationResult = EvaluationResult.NotSupported;
				}
			}
		}

		[Obsolete("This function is obsolete.Using LanguageRegistryApi.GetAlternativeLanguageCodes recommended ")]
		public string[] GetAlternativeLanguageCodes(string productId)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<Product, List<AlternativeLanguageCode>> supportedProduct in SupportedProducts)
			{
				if (supportedProduct.Key.ProductId == productId)
				{
					foreach (AlternativeLanguageCode item in supportedProduct.Value)
					{
						list.Add(item.Code);
					}
				}
			}
			return list.ToArray();
		}

		public string GetDisplayName(LanguageFormat languageFormat, bool includeCountry)
		{
			if (IsValid)
			{
				try
				{
					return FormatLanguageUsingCultureInfo(this, languageFormat, includeCountry);
				}
				catch
				{
					return FormatLanguageUsingStringResources(this, includeCountry);
				}
			}
			return FormatLanguageUsingStringResources(this, includeCountry);
		}

		private static string FormatLanguageUsingCultureInfo(Language language, LanguageFormat languageFormat, bool includeCountry)
		{
			switch (languageFormat)
			{
			case LanguageFormat.EnglishName:
				if (!includeCountry)
				{
					return StripCountry(language.CultureInfo.EnglishName);
				}
				return language.CultureInfo.EnglishName;
			case LanguageFormat.NativeName:
			{
				string nativeName = language.CultureInfo.NativeName;
				string text = char.ToUpper(nativeName[0]).ToString() + nativeName.Substring(1);
				if (!includeCountry)
				{
					return StripCountry(text);
				}
				return text;
			}
			case LanguageFormat.ISOCode:
				return FormatIsoCode(language.IsoAbbreviation, includeCountry);
			default:
				return "";
			}
		}

		private static string FormatIsoCode(string isoCode, bool includeCountry)
		{
			if (includeCountry)
			{
				return isoCode.ToUpper(CultureInfo.InvariantCulture);
			}
			int num = isoCode.IndexOf('-');
			if (num == -1)
			{
				return isoCode.ToUpper(CultureInfo.InvariantCulture);
			}
			return isoCode.Substring(0, num).ToUpper(CultureInfo.InvariantCulture);
		}

		private static string FormatLanguageUsingStringResources(Language language, bool includeCountry)
		{
			ResourceManager resourceManager = new ResourceManager(typeof(LanguagesResource));
			string @string = resourceManager.GetString(language.IsoAbbreviation.Replace("-", "_"));
			if (!includeCountry)
			{
				return StripCountry(@string);
			}
			return @string;
		}

		private static string StripCountry(string language)
		{
			int num = language.IndexOf('(');
			if (num >= 0)
			{
				return language.Substring(0, num).Trim();
			}
			return language;
		}

		[Obsolete("This function is obsolete. Using GetFlagImage function recommended")]
		public Image GetImage(bool useFlag)
		{
			string key = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[2]
			{
				IsoAbbreviation,
				useFlag.ToString()
			});
			if (_flags.TryGetValue(key, out Image value))
			{
				return value;
			}
			value = ((!useFlag) ? CreateFlagFromIsoAbbreviation(this) : (GetEmbeddedFlag(this) ?? CreateFlagFromIsoAbbreviation(this)));
			_flags[key] = value;
			return value;
		}

		public Image GetFlagImage(bool useFlag = true, float scaleFactor = 1f, int baseSize = 24)
		{
			string key = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[3]
			{
				IsoAbbreviation,
				useFlag.ToString(),
				baseSize.ToString()
			});
			int imageSize = GetImageSize(scaleFactor, baseSize);
			if (_flags.TryGetValue(key, out Image value))
			{
				return value;
			}
			value = ((!useFlag) ? CreateFlagIconFromIsoAbbreviation(this, scaleFactor, imageSize) : GetEmbeddedFlagIcon(this, scaleFactor, imageSize));
			_flags[key] = value;
			return value;
		}

		private static int GetImageSize(float scaleFactor, int baseSize)
		{
			int[] array = new int[7]
			{
				16,
				24,
				32,
				48,
				64,
				128,
				256
			};
			int num = array.Length - 1;
			while (num > 0 && (float)array[num] > (float)baseSize * scaleFactor)
			{
				num--;
			}
			return array[num];
		}

		private static Image CreateFlagIconFromIsoAbbreviation(Language language, float scaleFactor = 1f, int imageSize = 16)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Sdl.Core.Globalization.Flags.no-flag.ico");
			if (manifestResourceStream == null)
			{
				return null;
			}
			Icon icon = new Icon(manifestResourceStream, new Size(imageSize, imageSize));
			Bitmap bitmap = icon.ToBitmap();
			Graphics graphics = Graphics.FromImage(bitmap);
			string text = language.IsValid ? FormatIsoCode(language.IsoAbbreviation, includeCountry: false) : "NE";
			float num = (text.Length > 2) ? 4.5f : 6.5f;
			num = num * (float)imageSize / 16f / scaleFactor;
			Font font = new Font("sans-serif", num, FontStyle.Regular);
			SizeF sizeF = graphics.MeasureString(text, font);
			int num2 = (int)((double)((float)imageSize - sizeF.Height) / 2.0);
			int num3 = (imageSize - (int)sizeF.Width) / 2;
			Brush brush = new SolidBrush(Color.FromArgb(67, 74, 101));
			graphics.DrawString(text, font, brush, num3, num2);
			graphics.Dispose();
			bitmap.MakeTransparent(Color.White);
			return bitmap;
		}

		private static Image GetEmbeddedFlagIcon(Language language, float scaleFactor = 1f, int imageSize = 16)
		{
			try
			{
				string arg = language.IsValid ? language.IsoAbbreviation.ToLower(CultureInfo.InvariantCulture) : "none";
				string text = $"Sdl.Core.Globalization.Flags.{arg}.ico";
				ManifestResourceInfo manifestResourceInfo = Assembly.GetExecutingAssembly().GetManifestResourceInfo(text);
				if (manifestResourceInfo == null)
				{
					return CreateFlagIconFromIsoAbbreviation(language, scaleFactor, imageSize);
				}
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text);
				if (manifestResourceStream == null)
				{
					return CreateFlagIconFromIsoAbbreviation(language, scaleFactor, imageSize);
				}
				Icon icon = new Icon(manifestResourceStream, new Size(imageSize, imageSize));
				Bitmap bitmap = icon.ToBitmap();
				bitmap.MakeTransparent();
				return bitmap;
			}
			catch
			{
				return CreateFlagIconFromIsoAbbreviation(language, scaleFactor, imageSize);
			}
		}

		private static Image CreateFlagFromIsoAbbreviation(Language language)
		{
			Bitmap bitmap = new Bitmap(23, 14, PixelFormat.Format32bppRgb);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.FillRectangle(Brushes.LightGray, 0, 0, 23, 14);
			Font font = new Font("Arial", 8f, FontStyle.Regular);
			string text = language.IsValid ? FormatIsoCode(language.IsoAbbreviation, includeCountry: false) : "NE";
			SizeF sizeF = graphics.MeasureString(text, font);
			int num = (int)((14.0 - (double)sizeF.Height) / 2.0);
			int num2 = (23 - (int)sizeF.Width) / 2;
			graphics.DrawString(text, font, SystemBrushes.WindowText, num2, num);
			graphics.Dispose();
			return bitmap;
		}

		private static Image GetEmbeddedFlag(Language language)
		{
			try
			{
				string str = language.IsValid ? language.IsoAbbreviation.ToLower(CultureInfo.InvariantCulture) : "none";
				string text = "Sdl.Core.Globalization.Flags." + str + ".bmp";
				ManifestResourceInfo manifestResourceInfo = Assembly.GetExecutingAssembly().GetManifestResourceInfo(text);
				if (manifestResourceInfo != null)
				{
					Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text);
					if (manifestResourceStream != null)
					{
						return new Bitmap(manifestResourceStream);
					}
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Language language = (Language)obj;
			if (_isoAbbreviation == null != (language._isoAbbreviation == null))
			{
				return false;
			}
			if (_isoAbbreviation != null && !_isoAbbreviation.Equals(language._isoAbbreviation, StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return _isoAbbreviation ?? string.Empty;
		}

		public override int GetHashCode()
		{
			if (_isoAbbreviation == null)
			{
				return 0;
			}
			return _isoAbbreviation.ToLower(CultureInfo.InvariantCulture).GetHashCode();
		}
	}
}
