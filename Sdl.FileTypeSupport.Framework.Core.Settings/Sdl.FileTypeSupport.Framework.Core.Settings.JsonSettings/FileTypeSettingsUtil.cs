using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.JsonSettings
{
	public static class FileTypeSettingsUtil
	{
		public static ISettingsBundle CreateJsonSettingsBundle(FileTypeSettingsJson fileTypeSettingsJson)
		{
			ISettingsBundle jsonSettingsBundle = SettingsUtil.CreateJsonSettingsBundle(null);
			string data = SettingsUtil.SerializeJsonSettingsBundle(jsonSettingsBundle);
			return CreateJsonSettingsBundleFromData(fileTypeSettingsJson, data);
		}

		public static ISettingsBundle CreateJsonSettingsBundleFromData(FileTypeSettingsJson fileTypeSettingsJson, string data)
		{
			JObject jObject = JObject.Parse(data);
			UpdateWellKnownSectionInJsonBundle(jObject, fileTypeSettingsJson.FileTypeId, "FileTypeSettingsSection", fileTypeSettingsJson.FilterSettings);
			UpdateWellKnownSectionInJsonBundle(jObject, fileTypeSettingsJson.FileTypeId, "SDL_embedded_content_processor_ids", fileTypeSettingsJson.EmbeddedContentProcessors);
			UpdateWellKnownSectionInJsonBundle(jObject, fileTypeSettingsJson.FileTypeId, "SDL_Verification_Section", fileTypeSettingsJson.VerificationSettings);
			UpdateWellKnownSectionInJsonBundle(jObject, fileTypeSettingsJson.FileTypeId, "SDL_Preview_Section", fileTypeSettingsJson.PreviewSettings);
			SetComponentBuilderIdOnJsonBundle(fileTypeSettingsJson.FilterSettings, fileTypeSettingsJson.ComponentBuilderId);
			data = JsonConvert.SerializeObject(jObject);
			return SettingsUtil.DeserializeJsonSettingsBundle(data);
		}

		public static JObject GetWellKnownSectionFromJsonBundle(JObject jBundle, string fileTypeId, string settingsNameKey)
		{
			if (jBundle == null)
			{
				return JObject.Parse("{}");
			}
			string str = (settingsNameKey == "FileTypeSettingsSection") ? fileTypeId : (fileTypeId + "_" + settingsNameKey);
			string path = "_groups.['" + str + "']";
			JToken jToken = jBundle.SelectToken(path);
			return jToken as JObject;
		}

		public static string GetComponentBuilderIdFromJsonBundle(JObject jGroup)
		{
			if (jGroup == null)
			{
				return null;
			}
			string path = "_settings.['FileTypeConfiguration_ComponentBuilderId']";
			return jGroup.SelectToken(path)?["Value"].Value<string>();
		}

		private static void UpdateWellKnownSectionInJsonBundle(dynamic jBundle, string fileTypeId, string settingsNameKey, JObject updatedJObject)
		{
			string text = (settingsNameKey == "FileTypeSettingsSection") ? fileTypeId : (fileTypeId + "_" + settingsNameKey);
			dynamic val = jBundle._groups;
			if (updatedJObject != null)
			{
				val[text] = updatedJObject;
			}
		}

		private static void SetComponentBuilderIdOnJsonBundle(JObject jGroup, string componentBuilderId)
		{
			if (jGroup != null)
			{
				string propertyName = "_settings.['FileTypeConfiguration_ComponentBuilderId']";
				if (!string.IsNullOrEmpty(componentBuilderId))
				{
					jGroup[propertyName] = componentBuilderId;
				}
			}
		}
	}
}
