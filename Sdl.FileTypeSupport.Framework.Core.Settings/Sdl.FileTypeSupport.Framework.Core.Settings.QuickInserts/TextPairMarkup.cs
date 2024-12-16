using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public class TextPairMarkup : BaseMarkupDataType
	{
		private const string SettingStartText = "StartText";

		private const string SettingEndText = "EndText";

		private const string DefaultStartText = "";

		private const string DefalutEndText = "";

		private string _startText;

		private string _endText;

		public string StartText
		{
			get
			{
				return _startText;
			}
			set
			{
				_startText = value;
				OnPropertyChanged("StartText");
			}
		}

		public string EndText
		{
			get
			{
				return _endText;
			}
			set
			{
				_endText = value;
				OnPropertyChanged("EndText");
			}
		}

		public TextPairMarkup()
		{
			ResetToDefaults();
		}

		public override string ToString()
		{
			return StartText + ", " + EndText;
		}

		public override void Read(IValueGetter valueGetter)
		{
			StartText = valueGetter.GetValue("StartText", "");
			EndText = valueGetter.GetValue("EndText", "EndText");
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process("StartText", StartText, "");
			valueProcessor.Process("EndText", EndText, "");
		}

		public override object Clone()
		{
			return new TextPairMarkup
			{
				StartText = StartText,
				EndText = EndText
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			TextPairMarkup textPairMarkup = other as TextPairMarkup;
			if (textPairMarkup != null && textPairMarkup.StartText == StartText)
			{
				return textPairMarkup.EndText == EndText;
			}
			return false;
		}

		public override void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "StartText", StartText, "");
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "EndText", EndText, "");
		}

		public override void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			StartText = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "StartText", "");
			EndText = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "EndText", "");
		}

		public override void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
		{
			settingsGroup.RemoveSetting(listItemSetting + "StartText");
			settingsGroup.RemoveSetting(listItemSetting + "EndText");
		}

		public override void ResetToDefaults()
		{
			StartText = "";
			EndText = "";
		}
	}
}
