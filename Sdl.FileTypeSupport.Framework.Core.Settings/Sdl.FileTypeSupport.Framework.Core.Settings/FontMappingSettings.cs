using Newtonsoft.Json.Linq;
using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization.Implementation;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public class FontMappingSettings : FileTypeSettingsBase
	{
		private const string JsonSettingName = "FontMappingSettings";

		private const string SettingEnableFontMapping = "EnableFontMapping";

		private const string SettingFontMappingRules = "FontMappingRules";

		private const string SettingCustomFonts = "CustomFonts";

		private const bool DefaultEnableFontMapping = true;

		private static readonly Dictionary<int, string> _allLanguages = InitAllLanguages();

		private bool _enableFontMapping;

		private ObservableList<FontMappingRule> _fontMappingRules;

		private ObservableList<string> _customFontList = new ObservableList<string>();

		public static Dictionary<int, string> AllLanguages => _allLanguages;

		public bool EnableFontMapping
		{
			get
			{
				return _enableFontMapping;
			}
			set
			{
				_enableFontMapping = value;
				OnPropertyChanged("EnableFontMapping");
			}
		}

		public ObservableList<FontMappingRule> FontMappingRules
		{
			get
			{
				return _fontMappingRules;
			}
			internal set
			{
				_fontMappingRules = value;
				OnPropertyChanged("FontMappingRules");
			}
		}

		private static ObservableList<FontMappingRule> DefaultFontMappingRules => new ObservableList<FontMappingRule>
		{
			GetFontMappingRule("ar-SA", "1025", "<AllFonts>", "Arial"),
			GetFontMappingRule("zh-TW", "1028", "<AllFonts>", "PMingLiU"),
			GetFontMappingRule("th-TH", "1054", "<AllFonts>", "Tahoma"),
			GetFontMappingRule("he-IL", "1037", "<AllFonts>", "Arial"),
			GetFontMappingRule("vi-VN", "1066", "<AllFonts>", "Arial"),
			GetFontMappingRule("ja-JP", "1041", "<AllFonts>", "MS Mincho"),
			GetFontMappingRule("zh-CN", "2052", "<AllFonts>", "SimSun")
		};

		public ObservableList<string> CustomFontList
		{
			get
			{
				return _customFontList;
			}
			internal set
			{
				_customFontList = value;
				OnPropertyChanged("CustomFontList");
			}
		}

		public FontMappingSettings()
		{
			ResetToDefaults();
		}

		public Dictionary<string, string> GetFontMappingRule(string languageCultureCode, string lcid)
		{
			List<FontMappingRule> list = _fontMappingRules.Where((FontMappingRule fontMappingRule) => languageCultureCode == fontMappingRule.LanguageCultureName || lcid == fontMappingRule.Lcid).ToList();
			if (!list.Any())
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (FontMappingRule item in list)
			{
				for (int i = 0; i < item.Font.Count; i++)
				{
					if (!dictionary.ContainsKey(item.Font[i]))
					{
						dictionary.Add(item.Font[i], item.ApplyFont);
					}
				}
			}
			return dictionary;
		}

		public override void ResetToDefaults()
		{
			EnableFontMapping = true;
			FontMappingRules = DefaultFontMappingRules;
			CustomFontList.Clear();
		}

		public override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			if (settingsBundle is JsonSettingsBundle)
			{
				PopulateFromJsonSettingsBundle(settingsBundle, fileTypeConfigurationId);
			}
			else
			{
				PopulateFromXmlSettingsBundle(settingsBundle, fileTypeConfigurationId);
			}
		}

		private void PopulateFromXmlSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
			EnableFontMapping = GetSettingFromSettingsGroup(settingsGroup, "EnableFontMapping", defaultValue: true);
			if (settingsGroup.ContainsSetting("FontMappingRules"))
			{
				ObservableList<FontMappingRule> observableList = new ObservableList<FontMappingRule>();
				observableList.PopulateFromSettingsGroup(settingsGroup, "FontMappingRules");
				FontMappingRules = observableList;
			}
			else
			{
				FontMappingRules = DefaultFontMappingRules;
			}
			if (settingsGroup.ContainsSetting("CustomFonts"))
			{
				ObservableList<string> observableList2 = new ObservableList<string>();
				observableList2.PopulateFromSettingsGroup(settingsGroup, "CustomFonts");
				CustomFontList = observableList2;
			}
		}

		private void PopulateFromJsonSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
			JObject settingFromSettingsGroup = GetSettingFromSettingsGroup(settingsGroup, "FontMappingSettings", new JObject());
			JsonValueGetter jsonValueGetter = new JsonValueGetter(settingFromSettingsGroup);
			EnableFontMapping = jsonValueGetter.GetValue("EnableFontMapping", defaultValue: true);
			if (settingFromSettingsGroup.TryGetValue("FontMappingRules", out JToken value))
			{
				JArray jArray = value as JArray;
				if (jArray != null)
				{
					FontMappingRules = new ObservableList<FontMappingRule>();
					foreach (JToken item in jArray)
					{
						JObject jObject = item as JObject;
						if (jObject != null)
						{
							FontMappingRule fontMappingRule = new FontMappingRule();
							fontMappingRule.PopulateFromJson(new JsonValueGetter(jObject));
							FontMappingRules.Add(fontMappingRule);
						}
					}
					goto IL_00be;
				}
			}
			FontMappingRules = DefaultFontMappingRules;
			goto IL_00be;
			IL_00be:
			CustomFontList = new ObservableList<string>(jsonValueGetter.GetStringList("CustomFonts", new List<string>()));
		}

		public override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			if (settingsBundle is JsonSettingsBundle)
			{
				SaveToJsonSettingsBundle(settingsBundle, fileTypeConfigurationId);
			}
			else
			{
				SaveToXmlSettingsBundle(settingsBundle, fileTypeConfigurationId);
			}
		}

		private void SaveToXmlSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
			UpdateSettingInSettingsGroup(settingsGroup, "EnableFontMapping", EnableFontMapping, defaultValue: true);
			if (!FontMappingRules.Equals(DefaultFontMappingRules))
			{
				FontMappingRules.SaveToSettingsGroup(settingsGroup, "FontMappingRules");
			}
			else
			{
				FontMappingRules.ClearListItemSettings(settingsGroup, "FontMappingRules");
			}
			if (CustomFontList.Count > 0)
			{
				CustomFontList.SaveToSettingsGroup(settingsGroup, "CustomFonts");
			}
			else
			{
				CustomFontList.ClearListItemSettings(settingsGroup, "CustomFonts");
			}
		}

		private void SaveToJsonSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			SaveSettingToJsonSettingsBundle(this, settingsBundle, fileTypeConfigurationId);
		}

		private void SaveSettingToJsonSettingsBundle(FontMappingSettings setting, ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
			JsonValueProcessor jsonValueProcessor = new JsonValueProcessor();
			jsonValueProcessor.Process("EnableFontMapping", setting.EnableFontMapping, defaultValue: true);
			JArray jArray = new JArray();
			foreach (FontMappingRule fontMappingRule in setting.FontMappingRules)
			{
				JsonValueProcessor jsonValueProcessor2 = new JsonValueProcessor();
				fontMappingRule.SaveToJson(jsonValueProcessor2);
				jArray.Add(jsonValueProcessor2.CurrentObject);
			}
			jsonValueProcessor.CurrentObject.Add("FontMappingRules", jArray);
			jsonValueProcessor.Process("CustomFonts", setting.CustomFontList.ToList(), new List<string>());
			UpdateSettingInSettingsGroup(settingsGroup, "FontMappingSettings", jsonValueProcessor.CurrentObject, new JObject());
		}

		public override void SaveDefaultsToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			if (settingsBundle is JsonSettingsBundle)
			{
				SaveDefaultsToJsonSettingsBundle(settingsBundle, fileTypeConfigurationId);
			}
			else
			{
				SaveDefaultsToXmlSettingsBundle(settingsBundle, fileTypeConfigurationId);
			}
		}

		private void SaveDefaultsToXmlSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
			SaveInGroup(settingsGroup, "EnableFontMapping", settingValue: true);
			DefaultFontMappingRules.SaveToSettingsGroup(settingsGroup, "FontMappingRules");
		}

		private void SaveDefaultsToJsonSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			FontMappingSettings fontMappingSettings = new FontMappingSettings();
			fontMappingSettings.ResetToDefaults();
			SaveSettingToJsonSettingsBundle(fontMappingSettings, settingsBundle, fileTypeConfigurationId);
		}

		public string GetSettingsInXml()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement("FontMappingRules");
			xDocument.Add(xElement);
			foreach (FontMappingRule fontMappingRule in _fontMappingRules)
			{
				if ((fontMappingRule.LanguageCultureName == null && fontMappingRule.Lcid == null) || fontMappingRule.Font == null || fontMappingRule.ApplyFont == null)
				{
					break;
				}
				XElement xElement2 = new XElement("FontMappingRule");
				xElement.Add(xElement2);
				XElement xElement3 = new XElement("LanguageAndRegion");
				xElement2.Add(xElement3);
				string value = (!string.IsNullOrEmpty(fontMappingRule.LanguageCultureName)) ? fontMappingRule.LanguageCultureName : GetLanguageNameFromLcid(fontMappingRule.Lcid);
				xElement3.Add(new XText(value));
				if (fontMappingRule.Font.Count == 1 && fontMappingRule.Font[0] == "<AllFonts>")
				{
					xElement2.Add(new XElement("AllFonts"));
				}
				else
				{
					foreach (string item in fontMappingRule.Font)
					{
						XElement xElement4 = new XElement("Font");
						xElement2.Add(xElement4);
						xElement4.Add(new XText(item));
					}
				}
				xElement2.Add(new XElement("AllFontTypes"));
				XElement xElement5 = new XElement("ApplyFont");
				xElement2.Add(xElement5);
				xElement5.Add(new XText(fontMappingRule.ApplyFont));
			}
			return xDocument.ToString();
		}

		private static FontMappingRule GetFontMappingRule(string languageCultureCode, string lcid, string sourceFonts, string targetFont)
		{
			FontMappingRule fontMappingRule = new FontMappingRule
			{
				LanguageCultureName = languageCultureCode,
				Lcid = lcid
			};
			List<string> list = new List<string>();
			list.Add(sourceFonts);
			fontMappingRule.Font = list;
			fontMappingRule.ApplyFont = targetFont;
			return fontMappingRule;
		}

		private static Dictionary<int, string> InitAllLanguages()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			foreach (CultureInfo cultureInfo in cultures)
			{
				if (!dictionary.ContainsKey(cultureInfo.LCID))
				{
					dictionary.Add(cultureInfo.LCID, cultureInfo.Name);
				}
			}
			return dictionary;
		}

		private static string GetLanguageNameFromLcid(string lcid)
		{
			return AllLanguages[int.Parse(lcid)];
		}
	}
}
