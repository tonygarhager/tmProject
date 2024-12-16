using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class PluginResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("Sdl.FileTypeSupport.Bilingual.SdlXliff.PluginResources", typeof(PluginResources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string Plugin_Name => ResourceManager.GetString("Plugin_Name", resourceCulture);

		internal static string SdlXliff_FilterComponentBuilderExtension_Description => ResourceManager.GetString("SdlXliff_FilterComponentBuilderExtension_Description", resourceCulture);

		internal static string SdlXliff_FilterComponentBuilderExtension_Name => ResourceManager.GetString("SdlXliff_FilterComponentBuilderExtension_Name", resourceCulture);

		internal PluginResources()
		{
		}
	}
}
