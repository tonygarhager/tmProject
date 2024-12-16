using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.Globalization.LanguageRegistry
{
	public sealed class LanguageRegistryApi
	{
		private static readonly Lazy<LanguageRegistryApi> RegistryApi = new Lazy<LanguageRegistryApi>(() => new LanguageRegistryApi());

		private readonly LanguageRegistry _languageRegistry;

		public static LanguageRegistryApi Instance => RegistryApi.Value;

		private LanguageRegistryApi()
		{
			_languageRegistry = new LanguageRegistry();
			Initialize();
		}

		public void Initialize()
		{
			_languageRegistry.AllLanguages = new List<Language>();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("Sdl.Core.Globalization.LanguageRegistry.LanguageRegistry.json");
			if (manifestResourceStream != null)
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					string value = streamReader.ReadToEnd();
					DeserializeObject(value);
				}
			}
			foreach (Language language in _languageRegistry.AllLanguages.Where((Language x) => x.IsNeutral))
			{
				language.RegionalVariants = (from x in _languageRegistry.AllLanguages
					where !x.IsNeutral
					where object.Equals(x.ParentLanguage, language)
					select x).ToList();
			}
		}

		public IList<Language> GetAllLanguages()
		{
			return _languageRegistry.AllLanguages;
		}

		public IList<Language> GetAllNeutralLanguages()
		{
			return _languageRegistry.AllLanguages.Where((Language x) => x.IsNeutral).ToList();
		}

		public IList<Language> GetAllSpecificLanguages()
		{
			return _languageRegistry.AllLanguages.Where((Language x) => !x.IsNeutral).ToList();
		}

		public Language GetLanguage(string languageCode)
		{
			return _languageRegistry.AllLanguages.FirstOrDefault((Language x) => string.Equals(x.LanguageCode, languageCode, StringComparison.CurrentCultureIgnoreCase));
		}

		public Language GetLanguage(string alternativeLanguageCode, string productId)
		{
			Language language = GetLanguage(alternativeLanguageCode);
			if (language != null && language.SupportedProducts.Keys.Any((Product x) => x.ProductId == productId))
			{
				return language;
			}
			return _languageRegistry.AllLanguages.FirstOrDefault((Language x) => GetAlternativeLanguageCodes(productId, x.LanguageCode).Any((AlternativeLanguageCode s) => string.Equals(s.Code, alternativeLanguageCode, StringComparison.CurrentCultureIgnoreCase)));
		}

		public IList<Language> GetSupportedLanguages(string productId)
		{
			return _languageRegistry.AllLanguages.Where((Language x) => x.SupportedProducts.Keys.Any((Product y) => string.Equals(y.ProductId, productId))).ToList();
		}

		public IList<Product> GetAllRegisteredProducts()
		{
			IEnumerable<Dictionary<Product, List<AlternativeLanguageCode>>.KeyCollection> source = _languageRegistry.AllLanguages.Select((Language language) => language.SupportedProducts.Keys);
			return (from x in source.SelectMany((Dictionary<Product, List<AlternativeLanguageCode>>.KeyCollection x) => x)
				group x by x.ProductId into x
				select x.First()).ToList();
		}

		public IList<AlternativeLanguageCode> GetAlternativeLanguageCodes(string productId, string languageCode)
		{
			Language language = GetLanguage(languageCode);
			List<AlternativeLanguageCode> value = language.SupportedProducts.FirstOrDefault((KeyValuePair<Product, List<AlternativeLanguageCode>> x) => string.Equals(x.Key.ProductId, productId)).Value;
			if (value != null && value.Count > 0)
			{
				return value;
			}
			if (value != null && value.Count == 0)
			{
				return new List<AlternativeLanguageCode>
				{
					new AlternativeLanguageCode
					{
						Code = languageCode
					}
				};
			}
			return new List<AlternativeLanguageCode>();
		}

		public IList<AlternativeLanguageCode> GetProductSpecificCodeWithFallback(string sourceProductId, string sourceLanguageCode, string targetProductId, out MappingType mappingType)
		{
			Language language = GetLanguage(sourceLanguageCode, sourceProductId);
			IList<AlternativeLanguageCode> alternativeLanguageCodes = GetAlternativeLanguageCodes(targetProductId, language.LanguageCode);
			if (alternativeLanguageCodes.Count != 0)
			{
				mappingType = MappingType.ExactMapping;
				return alternativeLanguageCodes;
			}
			for (Language parentLanguage = language.ParentLanguage; parentLanguage != null; parentLanguage = parentLanguage.ParentLanguage)
			{
				Language defaultSpecificLanguage = parentLanguage.DefaultSpecificLanguage;
				IList<AlternativeLanguageCode> alternativeLanguageCodes2 = GetAlternativeLanguageCodes(targetProductId, defaultSpecificLanguage.LanguageCode);
				if (alternativeLanguageCodes2.Count != 0)
				{
					mappingType = MappingType.BestMapping;
					return alternativeLanguageCodes2;
				}
				IList<AlternativeLanguageCode> alternativeLanguageCodes3 = GetAlternativeLanguageCodes(targetProductId, parentLanguage.LanguageCode);
				if (alternativeLanguageCodes3.Count != 0)
				{
					mappingType = MappingType.BestMapping;
					return alternativeLanguageCodes3;
				}
			}
			mappingType = MappingType.None;
			return new List<AlternativeLanguageCode>();
		}

		public string SerializeObject()
		{
			foreach (Language item in _languageRegistry.AllLanguages.Where((Language x) => x.LanguageCode != ""))
			{
				if (item.ParentLanguage != null)
				{
					item.ParentIsoCode = item.ParentLanguage.LanguageCode;
					item.ParentLanguageCode = item.ParentLanguage.LanguageCode;
				}
				if (item.DefaultSpecificLanguage != null)
				{
					item.DefaultSpecificLanguageCode = item.DefaultSpecificLanguage.LanguageCode;
				}
			}
			return JsonConvert.SerializeObject(_languageRegistry, Formatting.None);
		}

		public void DeserializeObject(string value)
		{
			LanguageRegistry languageRegistry = JsonConvert.DeserializeObject<LanguageRegistry>(value);
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			foreach (CultureInfo culture in cultures)
			{
				if (languageRegistry.AllLanguages.FirstOrDefault((Language x) => x.LanguageCode == culture.Name) != null)
				{
					_languageRegistry.AllLanguages.Add(new Language(culture));
				}
			}
			foreach (Language lang in languageRegistry.AllLanguages)
			{
				Language language = _languageRegistry.AllLanguages.SingleOrDefault((Language x) => x.LanguageCode == lang.LanguageCode);
				if (language != null)
				{
					language.SupportedProducts = lang.SupportedProducts;
					language.EnglishName = lang.EnglishName;
					language.Direction = lang.Direction;
					if (lang.ParentLanguageCode != null)
					{
						language.ParentLanguage = GetLanguage(lang.ParentLanguageCode);
					}
					else
					{
						language.IsNeutral = true;
					}
					if (lang.DefaultSpecificLanguageCode != null)
					{
						language.DefaultSpecificLanguage = GetLanguage(lang.DefaultSpecificLanguageCode);
					}
				}
			}
		}
	}
}
