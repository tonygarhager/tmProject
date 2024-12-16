using System;

namespace Sdl.Core.Globalization
{
	public sealed class LanguageDisplaySettings
	{
		private LanguageFormat _format;

		private bool _includeCountry = true;

		private bool _useFlags = true;

		public LanguageFormat Format
		{
			get
			{
				return _format;
			}
			set
			{
				_format = value;
				OnSettingsChanged();
			}
		}

		public bool IncludeCountry
		{
			get
			{
				return _includeCountry;
			}
			set
			{
				_includeCountry = value;
				OnSettingsChanged();
			}
		}

		public bool UseFlags
		{
			get
			{
				return _useFlags;
			}
			set
			{
				_useFlags = value;
				OnSettingsChanged();
			}
		}

		public event EventHandler SettingsChanged;

		internal LanguageDisplaySettings()
		{
		}

		private void OnSettingsChanged()
		{
			if (this.SettingsChanged != null)
			{
				this.SettingsChanged(this, EventArgs.Empty);
			}
		}
	}
}
