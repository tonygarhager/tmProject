using System;

namespace Sdl.Core.PluginFramework
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public sealed class PluginAttribute : Attribute
	{
		private string _name;

		[PluginResource]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public PluginAttribute(string name)
		{
			_name = name;
		}
	}
}
