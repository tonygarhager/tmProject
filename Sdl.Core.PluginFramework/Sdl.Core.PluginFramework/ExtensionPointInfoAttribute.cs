using System;

namespace Sdl.Core.PluginFramework
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExtensionPointInfoAttribute : Attribute
	{
		private string _name;

		private ExtensionPointBehavior _behavior;

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

		public ExtensionPointBehavior Behavior
		{
			get
			{
				return _behavior;
			}
			set
			{
				_behavior = value;
			}
		}

		public ExtensionPointInfoAttribute(string name, ExtensionPointBehavior behavior)
		{
			_name = name;
			_behavior = behavior;
		}
	}
}
