using Newtonsoft.Json.Linq;
using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization
{
	public abstract class AbstractSettingsClass : FileTypeSettingsBase, ISettingsClass, ICloneable, IEquatable<ISettingsClass>
	{
		public abstract string SettingName
		{
			get;
		}

		protected virtual bool HasEmbeddedContentProcessorIds => false;

		public abstract void Read(IValueGetter valueGetter);

		public abstract void Save(IValueProcessor valueProcessor);

		public abstract object Clone();

		public abstract bool Equals(ISettingsClass other);

		public sealed override void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			if (string.IsNullOrEmpty(fileTypeConfigurationId))
			{
				return;
			}
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
			if (settingsGroup != null)
			{
				if (settingsBundle is JsonSettingsBundle)
				{
					JObject settingFromSettingsGroup = GetSettingFromSettingsGroup(settingsGroup, SettingName, new JObject());
					JsonValueGetter valueGetter = new JsonValueGetter(settingFromSettingsGroup);
					Read(valueGetter);
				}
				else
				{
					XmlValueGetter valueGetter2 = new XmlValueGetter(settingsGroup);
					Read(valueGetter2);
				}
			}
		}

		public sealed override void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			SaveToSettingsBundleHelper(this, settingsBundle, fileTypeConfigurationId, forceWritingDefaultValues: false);
		}

		public sealed override void SaveDefaultsToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			AbstractSettingsClass abstractSettingsClass = (AbstractSettingsClass)Clone();
			abstractSettingsClass.ResetToDefaults();
			SaveToSettingsBundleHelper(abstractSettingsClass, settingsBundle, fileTypeConfigurationId, forceWritingDefaultValues: true);
		}

		private void SaveToSettingsBundleHelper(AbstractSettingsClass setting, ISettingsBundle settingsBundle, string fileTypeConfigurationId, bool forceWritingDefaultValues)
		{
			if (string.IsNullOrEmpty(fileTypeConfigurationId))
			{
				return;
			}
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(fileTypeConfigurationId);
			if (settingsGroup == null)
			{
				return;
			}
			if (settingsBundle is JsonSettingsBundle)
			{
				JsonValueProcessor jsonValueProcessor = new JsonValueProcessor();
				setting.Save(jsonValueProcessor);
				UpdateSettingInSettingsGroup(settingsGroup, SettingName, jsonValueProcessor.CurrentObject, new JObject());
				if (HasEmbeddedContentProcessorIds)
				{
					AddOrRemoveEmbeddedContentProcessorIdsSection(setting, settingsBundle, fileTypeConfigurationId);
				}
			}
			else
			{
				XmlValueProcessor valueProcessor = new XmlValueProcessor(settingsGroup, string.Empty, forceWritingDefaultValues);
				setting.Save(valueProcessor);
			}
		}

		private void AddOrRemoveEmbeddedContentProcessorIdsSection(AbstractSettingsClass setting, ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			string id = fileTypeConfigurationId + "_SDL_embedded_content_processor_ids";
			List<string> list = setting.GetEmbeddedContentProcessorIds().Distinct().ToList();
			if (!list.Any())
			{
				settingsBundle.RemoveSettingsGroup(id);
				return;
			}
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup(id);
			JArray jArray = new JArray();
			foreach (string item in list)
			{
				jArray.Add(item);
			}
			UpdateSettingInSettingsGroup(settingsGroup, "SDL_embedded_content_processor_ids", jArray, new JArray());
		}

		public sealed override bool Equals(object obj)
		{
			return Equals(obj as ISettingsClass);
		}

		protected virtual IEnumerable<string> GetEmbeddedContentProcessorIds()
		{
			yield break;
		}
	}
}
