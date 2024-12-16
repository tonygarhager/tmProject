using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Util;
using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[ExtensionPointInfo("File Type Component Builder", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class FileTypeComponentBuilderAttribute : SortableExtensionAttribute
	{
		private bool _isTemplate;

		public bool IsTemplate
		{
			get
			{
				return _isTemplate;
			}
			set
			{
				_isTemplate = value;
			}
		}

		public FileTypeComponentBuilderAttribute()
		{
		}

		public FileTypeComponentBuilderAttribute(string id, string name, string description)
			: base(id, name, description)
		{
		}

		public FileTypeComponentBuilderAttribute(string id, string name, string description, bool isTemplate)
			: base(id, name, description)
		{
			_isTemplate = isTemplate;
		}
	}
}
