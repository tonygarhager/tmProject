#define TRACE
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public static class DefaultPropertiesFactory
	{
		public const string ImplementationAssemblyName = "Sdl.FileTypeSupport.Framework.Implementation";

		public const string ImplementationClassName = "Sdl.FileTypeSupport.Framework.Native.PropertiesFactory";

		public static IPropertiesFactory CreateInstance()
		{
			try
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Sdl.FileTypeSupport.Framework.Implementation";
				return Assembly.Load(assemblyName).CreateInstance("Sdl.FileTypeSupport.Framework.Native.PropertiesFactory") as IPropertiesFactory;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Failed to create default implementation of the properties factory.\r\n\r\n{ex.ToString()}");
				throw;
			}
		}
	}
}
