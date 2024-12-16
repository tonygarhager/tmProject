using Sdl.Core.PluginFramework;
using System;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	public abstract class AbstractCommandBarItemAttribute : ExtensionAttribute
	{
		public Type ContextByType
		{
			get;
			set;
		}

		protected AbstractCommandBarItemAttribute()
		{
		}

		protected AbstractCommandBarItemAttribute(string id, string name, string description)
			: base(id, name, description)
		{
		}
	}
}
