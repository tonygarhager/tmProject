namespace Sdl.Core.Settings.Implementation
{
	public abstract class AbstractSettingsBundleAware : ISettingsBundleAware
	{
		private ISettingsBundle _settingsBundle;

		public ISettingsBundle SettingsBundle
		{
			get
			{
				return _settingsBundle;
			}
			set
			{
				_settingsBundle = value;
			}
		}

		public virtual void SetSettingsBundle(ISettingsBundle settingsBundle)
		{
			_settingsBundle = settingsBundle;
		}
	}
}
