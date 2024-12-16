using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class StringResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("Sdl.FileTypeSupport.Framework.PreviewControls.StringResources", typeof(StringResources).Assembly);
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
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

		/// <summary>
		///   Looks up a localized string similar to Cannot navigate to segment {0}..
		/// </summary>
		internal static string CannotNavigateToSegment => ResourceManager.GetString("CannotNavigateToSegment", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Browse for Associated External Preview Application.
		/// </summary>
		internal static string GenericExternalPreviewSettings_BrowseApp => ResourceManager.GetString("GenericExternalPreviewSettings_BrowseApp", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Applications (*.exe)|*.exe.
		/// </summary>
		internal static string GenericExternalPreviewSettings_BrowseAppsFilter => ResourceManager.GetString("GenericExternalPreviewSettings_BrowseAppsFilter", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The external preview can not be performed as the preview document is in use..
		/// </summary>
		internal static string GenericExternalPreviewSettings_SinglePreviewException => ResourceManager.GetString("GenericExternalPreviewSettings_SinglePreviewException", resourceCulture);

		internal StringResources()
		{
		}
	}
}
