using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization
{
	public interface IValueGetter
	{
		int GetValue(string key, int defaultValue);

		string GetValue(string key, string defaultValue);

		bool GetValue(string key, bool defaultValue);

		T GetValue<T>(string key, T defaultValue, bool discardKey) where T : ISettingsClass, new();

		List<string> GetStringList(string key, List<string> defaultValue);

		Dictionary<string, string> GetStringDictionary(string key, Dictionary<string, string> defaultValue);

		List<T> GetCompositeList<T>(string key, List<T> defaultValue) where T : ISettingsClass, new();
	}
}
