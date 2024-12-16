using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Formatting;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting
{
	public class FormattingInflator
	{
		public static IFormattingGroup InflateFormatting(FormattingGroupSettings formattingGroupSettings)
		{
			IFormattingItemFactory formattingItemFactory = new FormattingItemFactory();
			IFormattingGroup formattingGroup = new FormattingGroup();
			if (formattingGroupSettings != null)
			{
				foreach (string key in formattingGroupSettings.FormattingItems.Keys)
				{
					formattingGroup.Add(formattingItemFactory.CreateFormattingItem(key, formattingGroupSettings.FormattingItems[key]));
				}
				return formattingGroup;
			}
			return formattingGroup;
		}

		public static IDictionary<string, string> InflateFormattingToDictionary(FormattingGroupSettings formattingGroupSettings)
		{
			IFormattingItemFactory formattingItemFactory = new FormattingItemFactory();
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			if (formattingGroupSettings != null)
			{
				foreach (string key in formattingGroupSettings.FormattingItems.Keys)
				{
					dictionary.Add(key, formattingGroupSettings.FormattingItems[key]);
				}
				return dictionary;
			}
			return dictionary;
		}

		public static FormattingGroupSettings DeflateFormatting(IFormattingGroup formattingGroup)
		{
			FormattingGroupSettings formattingGroupSettings = new FormattingGroupSettings();
			if (formattingGroup != null)
			{
				foreach (IFormattingItem value in formattingGroup.Values)
				{
					formattingGroupSettings.FormattingItems.Add(value.FormattingName, value.StringValue);
				}
				return formattingGroupSettings;
			}
			return formattingGroupSettings;
		}

		public static FormattingGroupSettings DeflateFormatting(IDictionary<string, string> formatting)
		{
			FormattingGroupSettings formattingGroupSettings = new FormattingGroupSettings();
			if (formatting != null)
			{
				foreach (KeyValuePair<string, string> item in formatting)
				{
					formattingGroupSettings.FormattingItems.Add(item.Key, item.Value);
				}
				return formattingGroupSettings;
			}
			return formattingGroupSettings;
		}
	}
}
