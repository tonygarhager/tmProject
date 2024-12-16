using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Validation;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[ExtensionPointInfo("Translation Provider Factories", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TranslationProviderFactoryAttribute : ExtensionAttribute
	{
		public override void Validate(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
			base.Validate(info, context);
			context.ValidateRequiredInterface(typeof(ITranslationProviderFactory));
		}
	}
}
