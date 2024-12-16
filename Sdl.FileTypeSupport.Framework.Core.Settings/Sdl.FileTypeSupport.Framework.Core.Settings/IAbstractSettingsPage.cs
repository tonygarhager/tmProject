namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public interface IAbstractSettingsPage
	{
		bool HasDefaultSettings
		{
			get;
		}

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
