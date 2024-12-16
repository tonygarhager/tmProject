using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.Core.Globalization
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
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
					ResourceManager resourceManager = resourceMan = new ResourceManager("Sdl.Core.Globalization.Resources", typeof(Resources).Assembly);
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

		internal static string ResourceHasNoAssembly => ResourceManager.GetString("ResourceHasNoAssembly", resourceCulture);

		internal static string ResourceHasNoSlash => ResourceManager.GetString("ResourceHasNoSlash", resourceCulture);

		internal static string ResourceIdHasNoNamespace => ResourceManager.GetString("ResourceIdHasNoNamespace", resourceCulture);

		internal static string ResourceIdStartsWithSlash => ResourceManager.GetString("ResourceIdStartsWithSlash", resourceCulture);

		internal static string UnsupportedCodepageExceptionMessage => ResourceManager.GetString("UnsupportedCodepageExceptionMessage", resourceCulture);

		internal static string UnsupportedIsoAbbreviationMessage => ResourceManager.GetString("UnsupportedIsoAbbreviationMessage", resourceCulture);

		internal Resources()
		{
		}
	}
}
