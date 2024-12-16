using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.FileTypeSupport.Framework.Core.Properties
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
					ResourceManager resourceManager = resourceMan = new ResourceManager("Sdl.FileTypeSupport.Framework.Core.Properties.Resources", typeof(Resources).Assembly);
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

		internal static string BackColorName => ResourceManager.GetString("BackColorName", resourceCulture);

		internal static string BoldName => ResourceManager.GetString("BoldName", resourceCulture);

		internal static string FalseName => ResourceManager.GetString("FalseName", resourceCulture);

		internal static string FontName => ResourceManager.GetString("FontName", resourceCulture);

		internal static string FontSizeName => ResourceManager.GetString("FontSizeName", resourceCulture);

		internal static string InheritName => ResourceManager.GetString("InheritName", resourceCulture);

		internal static string InvalidImageLocation => ResourceManager.GetString("InvalidImageLocation", resourceCulture);

		internal static string InvalidSuperSubName => ResourceManager.GetString("InvalidSuperSubName", resourceCulture);

		internal static string ItalicName => ResourceManager.GetString("ItalicName", resourceCulture);

		internal static string LeftToRightName => ResourceManager.GetString("LeftToRightName", resourceCulture);

		internal static string NormalSuperSubName => ResourceManager.GetString("NormalSuperSubName", resourceCulture);

		internal static string ResourceNotFound => ResourceManager.GetString("ResourceNotFound", resourceCulture);

		internal static string RightToLeftName => ResourceManager.GetString("RightToLeftName", resourceCulture);

		internal static string StrikethroughName => ResourceManager.GetString("StrikethroughName", resourceCulture);

		internal static string SubscriptName => ResourceManager.GetString("SubscriptName", resourceCulture);

		internal static string SuperscriptName => ResourceManager.GetString("SuperscriptName", resourceCulture);

		internal static string TextColorName => ResourceManager.GetString("TextColorName", resourceCulture);

		internal static string TextDirectionName => ResourceManager.GetString("TextDirectionName", resourceCulture);

		internal static string TextPositionName => ResourceManager.GetString("TextPositionName", resourceCulture);

		internal static string TrueName => ResourceManager.GetString("TrueName", resourceCulture);

		internal static string UnderlineName => ResourceManager.GetString("UnderlineName", resourceCulture);

		internal static string UnknownFormatting => ResourceManager.GetString("UnknownFormatting", resourceCulture);

		internal Resources()
		{
		}
	}
}
