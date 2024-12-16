using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using System;
using System.ComponentModel;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public class QuickInsertSettings : AbstractSettingsClass, ISerializableListItem, INotifyPropertyChanged
	{
		private const string SettingCommandId = "CommandId";

		private const string SettingCommandName = "CommandName";

		private const string SettingDescription = "Description";

		private const string SettingDisplayOnToolbar = "DisplayOnToolbar";

		private const string SettingMarkupDataType = "MarkupDataType";

		private const string SettingMarkupData = "MarkupData";

		private const string DefaultCommandId = "";

		private const string DefaultCommandName = "";

		private const string DefaultDescription = "";

		private const bool DefaultDisplayOnToolbar = false;

		private string commandId;

		private string commandName;

		private string description;

		private bool displayOnToolbar;

		private BaseMarkupDataType _markupData;

		private BaseMarkupDataType DefaultMarkup => new TextMarkup();

		public override string SettingName => "QuickInsertDetails";

		public string CommandId
		{
			get
			{
				return commandId;
			}
			set
			{
				commandId = value;
				OnPropertyChanged("CommandId");
			}
		}

		public string CommandName
		{
			get
			{
				return commandName;
			}
			set
			{
				commandName = value;
				OnPropertyChanged("CommandName");
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
				OnPropertyChanged("Description");
			}
		}

		public bool DisplayOnToolbar
		{
			get
			{
				return displayOnToolbar;
			}
			set
			{
				displayOnToolbar = value;
				OnPropertyChanged("DisplayOnToolbar");
			}
		}

		public BaseMarkupDataType Markup
		{
			get
			{
				return _markupData;
			}
			set
			{
				_markupData = value;
				OnPropertyChanged("Markup");
			}
		}

		public new event PropertyChangedEventHandler PropertyChanged;

		public QuickInsertSettings()
		{
			ResetToDefaults();
		}

		public override void Read(IValueGetter valueGetter)
		{
			CommandId = valueGetter.GetValue("CommandId", "");
			CommandName = valueGetter.GetValue("CommandName", "");
			Description = valueGetter.GetValue("Description", "");
			DisplayOnToolbar = valueGetter.GetValue("DisplayOnToolbar", defaultValue: false);
			string value = valueGetter.GetValue("MarkupDataType", string.Empty);
			if (value == null)
			{
				return;
			}
			if (!(value == "TextPairMarkup"))
			{
				if (!(value == "TagPairMarkup"))
				{
					if (!(value == "PlaceholderTagMarkup"))
					{
						if (value == "TextMarkup")
						{
							Markup = valueGetter.GetValue("MarkupData", new TextMarkup(), discardKey: false);
						}
					}
					else
					{
						Markup = valueGetter.GetValue("MarkupData", new PlaceholderTagMarkup(), discardKey: false);
					}
				}
				else
				{
					Markup = valueGetter.GetValue("MarkupData", new TagPairMarkup(), discardKey: false);
				}
			}
			else
			{
				Markup = valueGetter.GetValue("MarkupData", new TextPairMarkup(), discardKey: false);
			}
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process("CommandId", CommandId, "");
			valueProcessor.Process("CommandName", CommandName, "");
			valueProcessor.Process("Description", Description, "");
			valueProcessor.Process("DisplayOnToolbar", DisplayOnToolbar, defaultValue: false);
			if (Markup != null)
			{
				valueProcessor.Process("MarkupDataType", Markup.GetType().Name, string.Empty);
				valueProcessor.Process("MarkupData", Markup, DefaultMarkup, discardKey: false);
			}
		}

		public override object Clone()
		{
			QuickInsertSettings quickInsertSettings = new QuickInsertSettings();
			quickInsertSettings.CommandId = CommandId;
			quickInsertSettings.CommandName = CommandName;
			quickInsertSettings.Description = Description;
			quickInsertSettings.DisplayOnToolbar = DisplayOnToolbar;
			quickInsertSettings.Markup = (Markup.Clone() as BaseMarkupDataType);
			return quickInsertSettings;
		}

		public override bool Equals(ISettingsClass other)
		{
			QuickInsertSettings quickInsertSettings = other as QuickInsertSettings;
			if (quickInsertSettings != null && quickInsertSettings.CommandId == CommandId && quickInsertSettings.CommandName == CommandName && quickInsertSettings.Description == Description && quickInsertSettings.DisplayOnToolbar == DisplayOnToolbar)
			{
				return quickInsertSettings.Markup.Equals(Markup);
			}
			return false;
		}

		public override void ResetToDefaults()
		{
			CommandId = "";
			CommandName = "";
			Description = "";
			DisplayOnToolbar = false;
			Markup = DefaultMarkup;
		}

		public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "CommandId", CommandId, "");
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "CommandName", CommandName, "");
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "Description", Description, "");
			UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + "DisplayOnToolbar", DisplayOnToolbar, defaultValue: false);
			settingsGroup.GetSetting<string>(listItemSetting + "MarkupDataType").Value = Markup.GetType().Name;
			Markup.SaveToSettingsGroup(settingsGroup, listItemSetting + "MarkupData");
		}

		public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			CommandId = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "CommandId", "");
			CommandName = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "CommandName", "");
			Description = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "Description", "");
			DisplayOnToolbar = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + "DisplayOnToolbar", defaultValue: false);
			string type = settingsGroup.GetSetting<string>(listItemSetting + "MarkupDataType");
			MarkupDataFactory markupDataFactory = new MarkupDataFactory();
			BaseMarkupDataType baseMarkupDataType = markupDataFactory.CreateMarkupDataType(type);
			if (baseMarkupDataType != null)
			{
				baseMarkupDataType.PopulateFromSettingsGroup(settingsGroup, listItemSetting + "MarkupData");
				Markup = baseMarkupDataType;
			}
		}

		public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
		{
			settingsGroup.RemoveSetting(listItemSetting + "CommandId");
			settingsGroup.RemoveSetting(listItemSetting + "CommandName");
			settingsGroup.RemoveSetting(listItemSetting + "Description");
			settingsGroup.RemoveSetting(listItemSetting + "DisplayOnToolbar");
			settingsGroup.RemoveSetting(listItemSetting + "MarkupDataType");
			settingsGroup.RemoveSetting(listItemSetting + "MarkupData");
		}

		protected new void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		protected new T GetSettingFromSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T defaultValue)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			if (settingsGroup.ContainsSetting(settingName))
			{
				return settingsGroup.GetSetting<T>(settingName).Value;
			}
			return defaultValue;
		}

		protected new void UpdateSettingInSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue, T defaultValue)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			if (settingsGroup.ContainsSetting(settingName))
			{
				settingsGroup.GetSetting<T>(settingName).Value = settingValue;
			}
			else if (!settingValue.Equals(defaultValue))
			{
				settingsGroup.GetSetting<T>(settingName).Value = settingValue;
			}
		}
	}
}
