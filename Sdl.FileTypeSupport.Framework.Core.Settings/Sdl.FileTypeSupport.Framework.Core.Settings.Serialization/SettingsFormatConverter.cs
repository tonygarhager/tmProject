using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization
{
	public static class SettingsFormatConverter
	{
		public static void ConvertSettings<T>(ISettingsBundle sourceSettingsBundle, ISettingsBundle targetSettingsBundle, string fileTypeId) where T : FileTypeSettingsBase, new()
		{
			T val = new T();
			val.PopulateFromSettingsBundle(sourceSettingsBundle, fileTypeId);
			val.SaveToSettingsBundle(targetSettingsBundle, fileTypeId);
		}
	}
}
