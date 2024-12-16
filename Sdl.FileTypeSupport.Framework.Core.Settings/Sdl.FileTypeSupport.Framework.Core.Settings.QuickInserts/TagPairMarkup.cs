using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public class TagPairMarkup : BaseMarkupDataType
	{
		private const string SettingStartTagText = "StartTagText";

		private const string SettingEndTagText = "EndTagText";

		private const string SettingCanHide = "CanHide";

		private const string SettingFormatting = "TagPairFormatting";

		private const string DefaultStartTagText = "";

		private const string DefaultEndTagText = "";

		private const bool DefaultCanHide = true;

		private string _startTagText;

		private string _endTagText;

		private bool _canHide;

		private FormattingGroupSettings _formatting;

		public string StartTagText
		{
			get
			{
				return _startTagText;
			}
			set
			{
				_startTagText = value;
				OnPropertyChanged("StartTagText");
			}
		}

		public string EndTagText
		{
			get
			{
				return _endTagText;
			}
			set
			{
				_endTagText = value;
				OnPropertyChanged("EndTagText");
			}
		}

		public bool CanHide
		{
			get
			{
				return _canHide;
			}
			set
			{
				_canHide = value;
				OnPropertyChanged("CanHide");
			}
		}

		public FormattingGroupSettings Formatting
		{
			get
			{
				return _formatting;
			}
			set
			{
				_formatting = value;
				OnPropertyChanged("Formatting");
			}
		}

		public override string ToString()
		{
			return "<" + StartTagText + "></" + EndTagText + ">";
		}

		public override void Read(IValueGetter valueGetter)
		{
			StartTagText = valueGetter.GetValue("StartTagText", "");
			EndTagText = valueGetter.GetValue("EndTagText", "");
			CanHide = valueGetter.GetValue("CanHide", CanHide);
			Formatting = valueGetter.GetValue("TagPairFormatting", new FormattingGroupSettings(), discardKey: false);
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process("StartTagText", StartTagText, "");
			valueProcessor.Process("EndTagText", EndTagText, "");
			valueProcessor.Process("CanHide", CanHide, defaultValue: true);
			valueProcessor.Process("TagPairFormatting", Formatting, new FormattingGroupSettings(), discardKey: false);
		}

		public override object Clone()
		{
			TagPairMarkup tagPairMarkup = new TagPairMarkup();
			tagPairMarkup.StartTagText = StartTagText;
			tagPairMarkup.EndTagText = EndTagText;
			tagPairMarkup.CanHide = CanHide;
			tagPairMarkup.Formatting = (Formatting.Clone() as FormattingGroupSettings);
			return tagPairMarkup;
		}

		public override bool Equals(ISettingsClass other)
		{
			TagPairMarkup tagPairMarkup = other as TagPairMarkup;
			if (tagPairMarkup != null && tagPairMarkup.StartTagText == StartTagText && tagPairMarkup.EndTagText == EndTagText && tagPairMarkup.CanHide == CanHide)
			{
				return tagPairMarkup.Formatting.Equals(Formatting);
			}
			return false;
		}

		public TagPairMarkup()
		{
			ResetToDefaults();
		}

		public override void ResetToDefaults()
		{
			_startTagText = "";
			_endTagText = "";
			_canHide = true;
			_formatting = new FormattingGroupSettings();
		}

		public override void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "StartTagText", StartTagText, "");
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "EndTagText", EndTagText, "");
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "CanHide", CanHide, defaultValue: true);
			_formatting.SaveToSettingsGroup(settingsGroup, listItemSetting + "TagPairFormatting");
		}

		public override void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			StartTagText = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "StartTagText", "");
			EndTagText = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "EndTagText", "");
			CanHide = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "CanHide", defaultValue: true);
			_formatting.PopulateFromSettingsGroup(settingsGroup, listItemSetting + "TagPairFormatting");
		}

		public override void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
		{
			settingsGroup.RemoveSetting(listItemSetting + "StartTagText");
			settingsGroup.RemoveSetting(listItemSetting + "EndTagText");
			settingsGroup.RemoveSetting(listItemSetting + "CanHide");
			_formatting.ClearListItemSettings(settingsGroup, listItemSetting + "TagPairFormatting");
		}
	}
}
