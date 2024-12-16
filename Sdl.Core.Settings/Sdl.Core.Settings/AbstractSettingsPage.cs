using System;

namespace Sdl.Core.Settings
{
	public abstract class AbstractSettingsPage : ISettingsPage, IDisposable
	{
		public object DataSource
		{
			get;
			set;
		}

		public virtual bool HasDefaultSettings => true;

		public abstract object GetControl();

		public virtual void OnActivate()
		{
		}

		public virtual void OnDeactivate()
		{
		}

		public virtual bool ValidateInput()
		{
			return true;
		}

		public virtual void Save()
		{
		}

		public virtual void AfterSave()
		{
		}

		public virtual void Cancel()
		{
		}

		public virtual void AfterCancel()
		{
		}

		public virtual void ResetToDefaults()
		{
		}

		public virtual void Refresh()
		{
		}

		public abstract void Dispose();
	}
}
