using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Validation;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Extension attribute for registering a translation provider Windows Forms user interface component 
	/// that provides user interface for specific types of translation providers.
	/// </summary>
	/// <remarks>
	/// <para>Use this extension attribute to mark up implementations of <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderWinFormsUI" />.</para>
	/// </remarks>
	[ExtensionPointInfo("Translation Provider Windows Forms Components", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TranslationProviderWinFormsUiAttribute : ExtensionAttribute
	{
		/// <summary>
		/// Validates an extension targeting this extension point. This method is called by the framework while generating the plug-in manifest.
		/// </summary>
		/// <param name="info">Information about the extension.</param>
		/// <param name="context">Provides functionality to validate extensions targeting the extension point
		/// and report warning or error messages</param>
		public override void Validate(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
			base.Validate(info, context);
			context.ValidateRequiredInterface(typeof(ITranslationProviderWinFormsUI));
		}
	}
}
