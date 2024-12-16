using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.Core.PluginFramework
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class StringResources
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
					resourceMan = new ResourceManager("Sdl.Core.PluginFramework.StringResources", typeof(StringResources).Assembly);
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

		internal static string Plugin_FailedToLoadDefinition => ResourceManager.GetString("Plugin_FailedToLoadDefinition", resourceCulture);

		internal static string Plugin_FailedToLoadResources => ResourceManager.GetString("Plugin_FailedToLoadResources", resourceCulture);

		internal static string Plugin_InvalidDefinitionRoot => ResourceManager.GetString("Plugin_InvalidDefinitionRoot", resourceCulture);

		internal static string Plugin_RequiredAttributeMissing => ResourceManager.GetString("Plugin_RequiredAttributeMissing", resourceCulture);

		internal static string PluginDeserializer_ObjectResolverNotFound => ResourceManager.GetString("PluginDeserializer_ObjectResolverNotFound", resourceCulture);

		internal static string PluginRegistry_ExtensionPointNotFound => ResourceManager.GetString("PluginRegistry_ExtensionPointNotFound", resourceCulture);

		internal StringResources()
		{
		}
	}
}
