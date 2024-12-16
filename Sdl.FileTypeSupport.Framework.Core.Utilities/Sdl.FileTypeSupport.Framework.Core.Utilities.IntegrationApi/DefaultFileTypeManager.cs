#define TRACE
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	public static class DefaultFileTypeManager
	{
		public const string ImplementationAssemblyName = "Sdl.FileTypeSupport.Framework.Implementation";

		public const string ImplementationClassName = "Sdl.FileTypeSupport.Framework.Integration.PocoFilterManager";

		public static IFileTypeManager CreateInstance()
		{
			try
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Sdl.FileTypeSupport.Framework.Implementation";
				return Assembly.Load(assemblyName).CreateInstance("Sdl.FileTypeSupport.Framework.Integration.PocoFilterManager") as IFileTypeManager;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Failed to create default implementation of the FileTypeManager.\r\n\r\n{ex.ToString()}");
				throw;
			}
		}

		public static IFileTypeManager CreateInstance(bool autoLoadFileTypes)
		{
			try
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Sdl.FileTypeSupport.Framework.Implementation";
				object[] args = new object[1]
				{
					autoLoadFileTypes
				};
				return Assembly.Load(assemblyName).CreateInstance("Sdl.FileTypeSupport.Framework.Integration.PocoFilterManager", ignoreCase: true, BindingFlags.Default, null, args, null, null) as IFileTypeManager;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Failed to create default implementation of the FileTypeManager.\r\n\r\n{ex.ToString()}");
				throw;
			}
		}
	}
}
