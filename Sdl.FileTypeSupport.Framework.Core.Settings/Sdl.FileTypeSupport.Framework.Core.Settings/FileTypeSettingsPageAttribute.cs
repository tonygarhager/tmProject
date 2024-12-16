using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Util;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	[ExtensionPointInfo("File Type Settings Page", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class FileTypeSettingsPageAttribute : SortableExtensionAttribute
	{
		public string HelpTopic
		{
			get;
			set;
		}

		public FileTypeSettingsPageAttribute()
		{
		}

		public FileTypeSettingsPageAttribute(string id, string name, string description)
			: base(id, name, description)
		{
		}
	}
}
