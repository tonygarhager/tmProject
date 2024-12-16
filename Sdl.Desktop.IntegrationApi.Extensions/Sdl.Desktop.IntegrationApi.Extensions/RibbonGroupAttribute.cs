using Sdl.Core.PluginFramework;
using System;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[ExtensionPointInfo("Ribbon Groups", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class RibbonGroupAttribute : AbstractCommandBarItemAttribute
	{
		public RibbonGroupAttribute(string id, string name, Type contextByType)
			: base(id, name, string.Empty)
		{
			base.ContextByType = contextByType;
		}

		public RibbonGroupAttribute(string id, string name)
			: base(id, name, string.Empty)
		{
		}

		public RibbonGroupAttribute(string id, Type contextByType)
			: this(id, id + "_Name", contextByType)
		{
			base.Description = id + "_Description";
		}

		public RibbonGroupAttribute(string id)
			: this(id, id + "_Name", null)
		{
			base.Description = id + "_Description";
		}
	}
}
