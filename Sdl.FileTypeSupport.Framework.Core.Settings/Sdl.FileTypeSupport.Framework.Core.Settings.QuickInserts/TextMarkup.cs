using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public class TextMarkup : BaseMarkupDataType
	{
		private const string SettingText = "Text";

		private const string DefaultText = "";

		private string _text;

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public TextMarkup()
		{
			ResetToDefaults();
		}

		public override string ToString()
		{
			return Text;
		}

		public override void Read(IValueGetter valueGetter)
		{
			Text = valueGetter.GetValue("Text", "");
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process("Text", Text, "");
		}

		public override bool Equals(ISettingsClass other)
		{
			TextMarkup textMarkup = other as TextMarkup;
			if (textMarkup != null)
			{
				return textMarkup.Text == Text;
			}
			return false;
		}

		public override object Clone()
		{
			return new TextMarkup
			{
				Text = Text
			};
		}

		public override void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "Text", Text, "");
		}

		public override void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			Text = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "Text", "");
		}

		public override void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
		{
			settingsGroup.RemoveSetting(listItemSetting + "Text");
		}

		public override void ResetToDefaults()
		{
			Text = "";
		}
	}
}
