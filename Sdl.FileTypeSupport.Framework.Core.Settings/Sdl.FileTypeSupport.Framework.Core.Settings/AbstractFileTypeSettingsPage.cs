using Sdl.Core.Settings;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public abstract class AbstractFileTypeSettingsPage<SettingsControlType, SettingsType> : AbstractSettingsPage, IFileTypeConfigurationAware where SettingsControlType : new()where SettingsType : FileTypeSettingsBase, new()
	{
		private SettingsControlType _control;

		private SettingsType _settings;

		public ISettingsBundle SettingsBundle
		{
			get
			{
				return (ISettingsBundle)base.DataSource;
			}
			set
			{
				base.DataSource = value;
			}
		}

		public virtual SettingsType Settings
		{
			get
			{
				if (_settings == null)
				{
					_settings = CreateAndInitializeSettings();
				}
				return _settings;
			}
			protected set
			{
				_settings = value;
				IFileTypeSettingsAware<SettingsType> fileTypeSettingsAware = _control as IFileTypeSettingsAware<SettingsType>;
				if (fileTypeSettingsAware != null)
				{
					fileTypeSettingsAware.Settings = _settings;
				}
			}
		}

		protected SettingsControlType Control
		{
			get
			{
				return _control;
			}
			set
			{
				_control = value;
			}
		}

		public string FileTypeConfigurationId
		{
			get;
			set;
		}

		public List<string> SubContentFileTypeConfigurationIds
		{
			get;
			set;
		}

		~AbstractFileTypeSettingsPage()
		{
			Dispose(disposing: false);
		}

		protected virtual void Dispose(bool disposing)
		{
			(_settings as IDisposable)?.Dispose();
			_settings = null;
		}

		protected virtual SettingsControlType CreateAndInitializeControl()
		{
			SettingsControlType val = new SettingsControlType();
			IFileTypeSettingsAware<SettingsType> fileTypeSettingsAware = val as IFileTypeSettingsAware<SettingsType>;
			if (fileTypeSettingsAware != null)
			{
				fileTypeSettingsAware.Settings = Settings;
				return val;
			}
			throw new Exception("Control doesn't implement IFileTypeSettingsAware<SettingsType> or settings page doesn't override CreateAndInitializeControl()");
		}

		protected virtual SettingsType CreateAndInitializeSettings()
		{
			SettingsType val = new SettingsType();
			if (SettingsBundle != null)
			{
				val.PopulateFromSettingsBundle(SettingsBundle, FileTypeConfigurationId);
			}
			return val;
		}

		public override object GetControl()
		{
			if (_control == null)
			{
				_control = CreateAndInitializeControl();
			}
			return _control;
		}

		public override void Save()
		{
			if (_settings != null)
			{
				_settings.SaveToSettingsBundle(SettingsBundle, FileTypeConfigurationId);
			}
		}

		public override void ResetToDefaults()
		{
			if (_settings != null)
			{
				_settings.ResetToDefaults();
			}
		}

		public override void Refresh()
		{
			if (_settings != null)
			{
				_settings.PopulateFromSettingsBundle(SettingsBundle, FileTypeConfigurationId);
			}
		}

		public override void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
