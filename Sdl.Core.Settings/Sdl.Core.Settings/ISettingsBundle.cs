using System.Collections.Generic;

namespace Sdl.Core.Settings
{
	public interface ISettingsBundle
	{
		bool IsDefault
		{
			get;
		}

		ISettingsBundle Parent
		{
			get;
			set;
		}

		bool IsEmpty
		{
			get;
		}

		T GetSettingsGroup<T>(string id) where T : ISettingsGroup, new();

		T GetSettingsGroup<T>() where T : ISettingsGroup, new();

		ISettingsGroup GetSettingsGroup(string id);

		bool AddSettingsGroup(ISettingsGroup settingsGroup);

		void RemoveSettingsGroup(string id);

		IEnumerable<string> GetSettingsGroupIds();

		bool ContainsSettingsGroup(string id);

		void Reset();

		void Assign(ISettingsBundle settings);
	}
}
