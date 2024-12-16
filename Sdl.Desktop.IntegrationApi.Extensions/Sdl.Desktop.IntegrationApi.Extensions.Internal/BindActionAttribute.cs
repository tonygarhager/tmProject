using Sdl.Core.PluginFramework;
using System;

namespace Sdl.Desktop.IntegrationApi.Extensions.Internal
{
	[ExtensionPointInfo("Bind actions", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class BindActionAttribute : ActionAttribute
	{
		public BindActionAttribute(string actionId)
		{
			base.Id = actionId;
		}
	}
}
