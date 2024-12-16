using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
					ResourceManager resourceManager = resourceMan = new ResourceManager("Sdl.Enterprise2.MultiTerm.Client.IdentityModel.Resources", typeof(Resources).Assembly);
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

		internal static string ApplicationServerNotReachable => ResourceManager.GetString("ApplicationServerNotReachable", resourceCulture);

		internal static string ClientBehaviorOnly => ResourceManager.GetString("ClientBehaviorOnly", resourceCulture);

		internal static string HttpEndpointOnly => ResourceManager.GetString("HttpEndpointOnly", resourceCulture);

		internal static string InvalidGlobalPermissionFormat => ResourceManager.GetString("InvalidGlobalPermissionFormat", resourceCulture);

		internal static string InvalidResourcePermissionFormat => ResourceManager.GetString("InvalidResourcePermissionFormat", resourceCulture);

		internal static string MissingCredentials => ResourceManager.GetString("MissingCredentials", resourceCulture);

		internal static string ServiceNotAvailable => ResourceManager.GetString("ServiceNotAvailable", resourceCulture);

		internal static string UnsupportedUriScheme => ResourceManager.GetString("UnsupportedUriScheme", resourceCulture);

		internal static string UserManagerOnly => ResourceManager.GetString("UserManagerOnly", resourceCulture);

		internal static string WindowsAuthNotHttp => ResourceManager.GetString("WindowsAuthNotHttp", resourceCulture);

		internal static string WindowsOnlyTcp => ResourceManager.GetString("WindowsOnlyTcp", resourceCulture);

		internal Resources()
		{
		}
	}
}
