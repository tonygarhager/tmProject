using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization
{
	public interface IValueProcessor
	{
		void Process(string key, int value, int defaultValue);

		void Process(string key, string value, string defaultValue);

		void Process(string key, bool value, bool defaultValue);

		void Process(string key, ISettingsClass value, ISettingsClass defaultValue, bool discardKey);

		void Process(string key, List<string> value, List<string> defaultValue);

		void Process(string key, IReadOnlyDictionary<string, string> value, IReadOnlyDictionary<string, string> defaultValues);

		void Process(string key, IReadOnlyList<ISettingsClass> value, IReadOnlyList<ISettingsClass> defaultValue);
	}
}
