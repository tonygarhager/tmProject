using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Util;
using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[ExtensionPointInfo("File Type Component Builder Extension", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class FileTypeComponentBuilderExtensionAttribute : SortableExtensionAttribute
	{
		public string OriginalFileType
		{
			get;
			set;
		}

		public FileTypeComponentBuilderExtensionAttribute()
		{
		}

		public FileTypeComponentBuilderExtensionAttribute(string id, string name, string description)
			: base(id, name, description)
		{
		}
	}
}
