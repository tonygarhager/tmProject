using System;

namespace Sdl.Core.PluginFramework.Util
{
	[AttributeUsage(AttributeTargets.Class)]
	public class SortableExtensionAttribute : ExtensionAttribute
	{
		private string _insertBefore;

		private string _insertAfter;

		public string InsertBefore
		{
			get
			{
				return _insertBefore;
			}
			set
			{
				_insertBefore = value;
			}
		}

		public string InsertAfter
		{
			get
			{
				return _insertAfter;
			}
			set
			{
				_insertAfter = value;
			}
		}

		public SortableExtensionAttribute()
		{
		}

		public SortableExtensionAttribute(string id, string name, string description)
			: base(id, name, description)
		{
		}
	}
}
