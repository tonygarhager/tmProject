using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Resources
{
	public abstract class ResourceStorage : IResourceDataAccessor
	{
		private List<string> _resourceNames;

		public string KeyPrefix
		{
			get;
		}

		public ResourceStorage()
			: this(null)
		{
		}

		public ResourceStorage(string keyPrefix)
		{
			KeyPrefix = keyPrefix;
		}

		public abstract List<string> GetAllResourceKeys();

		public abstract Stream ReadResourceData(CultureInfo culture, LanguageResourceType t, bool fallback);

		public abstract byte[] GetResourceData(CultureInfo culture, LanguageResourceType t, bool fallback);

		public static string GetResourceTypeName(LanguageResourceType t)
		{
			switch (t)
			{
			case LanguageResourceType.Variables:
				return "Variables.txt";
			case LanguageResourceType.Abbreviations:
				return "Abbreviations.txt";
			case LanguageResourceType.OrdinalFollowers:
				return "OrdinalFollowers.txt";
			case LanguageResourceType.SegmentationRules:
				return "SegmentationRules.xml";
			case LanguageResourceType.TokenizerSettings:
				return "TokenizerSettings.xml";
			case LanguageResourceType.StemmingRules:
				return "StemmingRules.txt";
			case LanguageResourceType.Stopwords:
				return "Stopwords.txt";
			case LanguageResourceType.DatePatterns:
				return "DatePatterns.xml";
			case LanguageResourceType.TimePatterns:
				return "TimePatterns.xml";
			case LanguageResourceType.NumberPatterns:
				return "NumberPatterns.xml";
			case LanguageResourceType.MeasurementPatterns:
				return "MeasurementPatterns.xml";
			case LanguageResourceType.CharTrigramVector:
				return "CharTrigrams.dat";
			case LanguageResourceType.ShortDateFST:
				return "ShortDate.fst";
			case LanguageResourceType.LongDateFST:
				return "LongDate.fst";
			case LanguageResourceType.ShortTimeFST:
				return "ShortTime.fst";
			case LanguageResourceType.LongTimeFST:
				return "LongTime.fst";
			case LanguageResourceType.CurrencySymbols:
				return "CurrencySymbols.txt";
			case LanguageResourceType.PhysicalUnits:
				return "PhysicalUnits.txt";
			case LanguageResourceType.NumberFST:
				return "Number.fst";
			case LanguageResourceType.MeasurementFST:
				return "Measurement.fst";
			case LanguageResourceType.GenericRecognizers:
				return "GenericRecognizers.xml";
			case LanguageResourceType.LongDateFSTEx:
				return "LongDate.fstex";
			case LanguageResourceType.ShortDateFSTEx:
				return "ShortDate.fstex";
			case LanguageResourceType.NumberFSTEx:
				return "Number.fstex";
			case LanguageResourceType.MeasurementFSTEx:
				return "Measurement.fstex";
			case LanguageResourceType.ShortTimeFSTEx:
				return "ShortTime.fstex";
			case LanguageResourceType.LongTimeFSTEx:
				return "LongTime.fstex";
			case LanguageResourceType.CurrencyFST:
				return "Currency.fst";
			case LanguageResourceType.CurrencyFSTEx:
				return "Currency.fstex";
			default:
				throw new ArgumentException("Illegal enum");
			}
		}

		public static Wordlist LoadWordlist(IResourceDataAccessor accessor, CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			using (Stream stream = accessor.ReadResourceData(culture, t, fallback))
			{
				if (stream == null)
				{
					return null;
				}
				Wordlist wordlist = new Wordlist();
				wordlist.Load(stream);
				return wordlist;
			}
		}

		public string GetName(string culturePrefix, LanguageResourceType t)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(KeyPrefix))
			{
				stringBuilder.Append(KeyPrefix);
				stringBuilder.Append(".");
			}
			if (!string.IsNullOrEmpty(culturePrefix))
			{
				stringBuilder.Append(culturePrefix);
				stringBuilder.Append("_");
			}
			stringBuilder.Append(GetResourceTypeName(t));
			return stringBuilder.ToString();
		}

		public string GetName(CultureInfo culture, LanguageResourceType t)
		{
			if (culture == null || CultureInfo.InvariantCulture.LCID == culture.LCID)
			{
				return GetName("", t);
			}
			return GetName(culture.Name, t);
		}

		public string GetResourceName(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			EnsureResourceNames();
			string name = GetName(culture, t);
			string text = Find(name);
			if (!fallback)
			{
				return text;
			}
			bool flag = false;
			while (text == null && culture.LCID != CultureInfo.InvariantCulture.LCID)
			{
				if (culture.Parent.LCID == CultureInfo.InvariantCulture.LCID && !flag)
				{
					string languageGroupName = CultureInfoExtensions.GetLanguageGroupName(culture);
					if (languageGroupName != null)
					{
						name = GetName(languageGroupName, t);
						text = Find(name);
					}
					flag = true;
				}
				else
				{
					culture = culture.Parent;
					name = GetName(culture, t);
					text = Find(name);
				}
			}
			return text;
		}

		private void EnsureResourceNames()
		{
			if (_resourceNames == null)
			{
				_resourceNames = (GetAllResourceKeys() ?? new List<string>());
			}
		}

		public List<CultureInfo> GetSupportedCultures(LanguageResourceType t)
		{
			EnsureResourceNames();
			List<CultureInfo> list = new List<CultureInfo>();
			string resourceTypeName = GetResourceTypeName(t);
			foreach (string resourceName in _resourceNames)
			{
				if (KeyPrefix == null || resourceName.StartsWith(KeyPrefix, StringComparison.OrdinalIgnoreCase))
				{
					string text = (KeyPrefix == null) ? resourceName : resourceName.Substring(KeyPrefix.Length + 1);
					if (text.EndsWith(resourceTypeName, StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							string name = text.Substring(0, text.Length - resourceTypeName.Length - 1);
							list.Add(CultureInfoExtensions.GetCultureInfo(name));
						}
						catch
						{
						}
					}
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}

		private string Find(string fullName)
		{
			return _resourceNames.Find((string s) => s.Equals(fullName, StringComparison.OrdinalIgnoreCase));
		}

		public ResourceStatus GetResourceStatus(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			string resourceName = GetResourceName(culture, t, fallback);
			if (resourceName != null)
			{
				return ResourceStatus.Loadable;
			}
			return ResourceStatus.NotAvailable;
		}

		internal static byte[] StreamToByteArray(Stream stream)
		{
			if (stream == null)
			{
				return null;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] buffer = new byte[4096];
				int num;
				do
				{
					num = stream.Read(buffer, 0, 4096);
					if (num > 0)
					{
						memoryStream.Write(buffer, 0, num);
					}
				}
				while (num == 4096);
				return memoryStream.ToArray();
			}
		}
	}
}
