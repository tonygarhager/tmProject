using Sdl.Core.PluginFramework;
using System;

namespace Sdl.Common.Licensing.Provider.Core
{
	[ExtensionPointInfo("SDL Licensing Provider Factories", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class LicensingProviderFactoryAttribute : ExtensionAttribute
	{
	}
}
