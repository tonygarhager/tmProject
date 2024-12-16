using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public class FontMappingRule
	{
		private const string ApplyFontJsonKey = "ApplyFont";

		private const string FontsJsonKey = "Fonts";

		private const string LanguageCultureNameJsonKey = "LanguageCultureName";

		private const string LcidJsonKey = "Lcid";

		public string LanguageCultureName
		{
			get;
			set;
		}

		public string Lcid
		{
			get;
			set;
		}

		public List<string> Font
		{
			get;
			set;
		}

		public string ApplyFont
		{
			get;
			set;
		}

		public override bool Equals(object obj)
		{
			FontMappingRule fontMappingRule = obj as FontMappingRule;
			if (fontMappingRule == null)
			{
				return false;
			}
			if (!object.Equals(fontMappingRule.LanguageCultureName, LanguageCultureName))
			{
				return false;
			}
			if (!object.Equals(fontMappingRule.Lcid, Lcid))
			{
				return false;
			}
			if (fontMappingRule.Font != null != (Font != null))
			{
				return false;
			}
			if (fontMappingRule.Font != null && Font != null)
			{
				if (fontMappingRule.Font.Count != Font.Count)
				{
					return false;
				}
				if (fontMappingRule.Font.Where((string t, int i) => !object.Equals(t, Font[i])).Any())
				{
					return false;
				}
			}
			return object.Equals(fontMappingRule.ApplyFont, ApplyFont);
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (!string.IsNullOrEmpty(LanguageCultureName))
			{
				num ^= LanguageCultureName.GetHashCode();
			}
			else if (Lcid != null)
			{
				num ^= Lcid.GetHashCode();
			}
			if (Font != null)
			{
				for (int i = 0; i < Font.Count; i++)
				{
					string text = Font[i];
					if (text != null)
					{
						num ^= (i + 1) * text.GetHashCode();
					}
				}
			}
			if (ApplyFont != null)
			{
				num ^= ApplyFont.GetHashCode();
			}
			return num;
		}

		internal void SaveToJson(JsonValueProcessor processor)
		{
			processor.Process("ApplyFont", ApplyFont, string.Empty);
			processor.Process("Fonts", Font, new List<string>());
			processor.Process("LanguageCultureName", LanguageCultureName, string.Empty);
			processor.Process("Lcid", Lcid, string.Empty);
		}

		internal void PopulateFromJson(JsonValueGetter valueGetter)
		{
			ApplyFont = valueGetter.GetValue("ApplyFont", string.Empty);
			Font = valueGetter.GetStringList("Fonts", new List<string>());
			LanguageCultureName = valueGetter.GetValue("LanguageCultureName", string.Empty);
			Lcid = valueGetter.GetValue("Lcid", string.Empty);
		}
	}
}
