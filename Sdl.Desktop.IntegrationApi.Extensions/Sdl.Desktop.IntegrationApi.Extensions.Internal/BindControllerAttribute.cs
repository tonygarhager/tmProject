using Sdl.Core.PluginFramework;
using System;

namespace Sdl.Desktop.IntegrationApi.Extensions.Internal
{
	[ExtensionPointInfo("Bind ControllerView", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class BindControllerAttribute : ExtensionAttribute
	{
		public string ViewId
		{
			get;
			set;
		}

		public BindControllerAttribute(string viewId)
		{
			ViewId = viewId;
		}
	}
}
