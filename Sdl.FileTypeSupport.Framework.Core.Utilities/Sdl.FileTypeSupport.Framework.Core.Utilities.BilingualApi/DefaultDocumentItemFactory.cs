#define TRACE
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public static class DefaultDocumentItemFactory
	{
		public const string ImplementationAssemblyName = "Sdl.FileTypeSupport.Framework.Implementation";

		public const string ImplementationClassName = "Sdl.FileTypeSupport.Framework.Bilingual.DocumentItemFactory";

		public static IDocumentItemFactory CreateInstance()
		{
			try
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Sdl.FileTypeSupport.Framework.Implementation";
				IDocumentItemFactory documentItemFactory = Assembly.Load(assemblyName).CreateInstance("Sdl.FileTypeSupport.Framework.Bilingual.DocumentItemFactory") as IDocumentItemFactory;
				if (documentItemFactory != null)
				{
					documentItemFactory.PropertiesFactory = DefaultPropertiesFactory.CreateInstance();
				}
				return documentItemFactory;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Failed to create default implementation of the document item factory.\r\n\r\n{ex.ToString()}");
				throw;
			}
		}
	}
}
