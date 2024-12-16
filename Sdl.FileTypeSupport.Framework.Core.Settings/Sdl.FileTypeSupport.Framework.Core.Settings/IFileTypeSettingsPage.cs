using Sdl.Core.Settings;
using System;
using System.Windows.Controls;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public interface IFileTypeSettingsPage : IAbstractSettingsPage, IDisposable
	{
		ISettingsBundle SettingsBundle
		{
			get;
		}

		bool IsInitialized
		{
			get;
		}

		void Init(ISettingsBundle settingsBundle);

		Control GetControl();
	}
}
