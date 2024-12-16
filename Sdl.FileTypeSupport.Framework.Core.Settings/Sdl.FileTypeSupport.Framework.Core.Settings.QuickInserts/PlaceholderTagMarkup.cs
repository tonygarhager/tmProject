using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public class PlaceholderTagMarkup : BaseMarkupDataType
	{
		private const string SettingText = "PlaceholderTagText";

		private const string SettingTextEquivalent = "TextEquivalent";

		private const string DefaultText = "";

		private const string DefaultTextEquivalent = "";

		private string _tagText;

		private string _textEquivalent;

		public string Text
		{
			get
			{
				return _tagText;
			}
			set
			{
				_tagText = value;
				OnPropertyChanged("Text");
			}
		}

		public string TextEquivalent
		{
			get
			{
				return _textEquivalent;
			}
			set
			{
				_textEquivalent = value;
				OnPropertyChanged("TextEquivalent");
			}
		}

		public PlaceholderTagMarkup()
		{
			ResetToDefaults();
		}

		public override string ToString()
		{
			return "<" + Text + " />";
		}

		public override void Read(IValueGetter valueGetter)
		{
			Text = valueGetter.GetValue("PlaceholderTagText", "");
			TextEquivalent = valueGetter.GetValue("TextEquivalent", "");
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process("PlaceholderTagText", Text, "");
			valueProcessor.Process("TextEquivalent", TextEquivalent, "");
		}

		public override object Clone()
		{
			return new PlaceholderTagMarkup
			{
				Text = Text,
				TextEquivalent = TextEquivalent
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			PlaceholderTagMarkup placeholderTagMarkup = other as PlaceholderTagMarkup;
			if (placeholderTagMarkup != null && placeholderTagMarkup.Text == Text)
			{
				return placeholderTagMarkup.TextEquivalent == TextEquivalent;
			}
			return false;
		}

		public override void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "PlaceholderTagText", Text, "");
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "TextEquivalent", TextEquivalent, "");
		}

		public override void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			Text = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "PlaceholderTagText", "");
			TextEquivalent = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "TextEquivalent", "");
		}

		public override void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
		{
			settingsGroup.RemoveSetting(listItemSetting + "PlaceholderTagText");
			settingsGroup.RemoveSetting(listItemSetting + "TextEquivalent");
		}

		public override void ResetToDefaults()
		{
			Text = "";
			TextEquivalent = "";
		}
	}
}
