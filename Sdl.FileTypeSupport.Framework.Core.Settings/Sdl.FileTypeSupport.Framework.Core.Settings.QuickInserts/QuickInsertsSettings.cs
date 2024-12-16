using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public class QuickInsertsSettings : AbstractSettingsClass
	{
		private const string ListId = "QuickInsertsList";

		private readonly ComplexObservableList<QuickInsertSettings> _settingsList = new ComplexObservableList<QuickInsertSettings>();

		public ComplexObservableList<QuickInsertSettings> QuickInserts => _settingsList;

		public override string SettingName => "QuickInsertSettings";

		public override void Read(IValueGetter valueGetter)
		{
			List<QuickInsertSettings> compositeList = valueGetter.GetCompositeList("QuickInsertsList", new List<QuickInsertSettings>());
			_settingsList.Clear();
			_settingsList.AddRange(compositeList);
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process("QuickInsertsList", _settingsList.ToList(), new List<QuickInsertSettings>());
		}

		public override object Clone()
		{
			QuickInsertsSettings defaultQuickInsertsSettings = GetDefaultQuickInsertsSettings();
			defaultQuickInsertsSettings.QuickInserts.Clear();
			defaultQuickInsertsSettings.QuickInserts.AddRange(_settingsList.Select((QuickInsertSettings item) => item.Clone() as QuickInsertSettings));
			return defaultQuickInsertsSettings;
		}

		public override bool Equals(ISettingsClass other)
		{
			return (other as QuickInsertsSettings)?._settingsList.Equals(_settingsList) ?? false;
		}

		public override void ResetToDefaults()
		{
			_settingsList.Clear();
		}

		protected virtual QuickInsertsSettings GetDefaultQuickInsertsSettings()
		{
			return new QuickInsertsSettings();
		}
	}
}
