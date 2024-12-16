using System;

namespace Sdl.Core.Settings
{
	public interface ISettingsPage : IDisposable
	{
		object DataSource
		{
			get;
			set;
		}

		bool HasDefaultSettings
		{
			get;
		}

		object GetControl();

		void OnActivate();

		void OnDeactivate();

		bool ValidateInput();

		void Save();

		void AfterSave();

		void Cancel();

		void AfterCancel();

		void ResetToDefaults();
	}
}
