using System;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework
{
	public sealed class VersionInfo
	{
		public static Version GetFrameworkVersion()
		{
			return Assembly.GetAssembly(typeof(VersionInfo)).GetName().Version;
		}
	}
}
