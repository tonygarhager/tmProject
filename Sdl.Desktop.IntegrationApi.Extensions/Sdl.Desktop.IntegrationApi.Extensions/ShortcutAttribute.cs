using Sdl.Core.PluginFramework;
using System;
using System.Windows.Forms;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ShortcutAttribute : AuxiliaryExtensionAttribute
	{
		public Keys Shortcut
		{
			get;
			set;
		}

		public ShortcutAttribute(Keys shortcut)
		{
			Shortcut = shortcut;
		}
	}
}
