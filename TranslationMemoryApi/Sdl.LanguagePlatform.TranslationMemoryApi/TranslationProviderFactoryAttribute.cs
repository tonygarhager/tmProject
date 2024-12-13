using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Validation;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Extension attribute for registering a translation provider factory that supports a specific types of translation providers.
	/// </summary>
	/// <remarks>
	/// <para>Use this extension attribute to mark up implementations of <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderFactory" />.</para>
	/// <para>The registered translation provider factories are used to instantiate translation providers when calling
	/// <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderManager.CreateTranslationProvider(System.Uri,System.String,Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderCredentialStore)" />.</para>
	/// </remarks>
	[ExtensionPointInfo("Translation Provider Factories", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TranslationProviderFactoryAttribute : ExtensionAttribute
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
			context.ValidateRequiredInterface(typeof(ITranslationProviderFactory));
		}
	}
}
