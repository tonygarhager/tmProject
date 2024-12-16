using Sdl.Core.PluginFramework;
using System;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[ExtensionPointInfo("Initializers", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class ApplicationInitializerAttribute : ExtensionAttribute
	{
	}
}
