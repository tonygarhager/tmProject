using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Validation;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[ExtensionPointInfo("Translation Provider Windows Forms Components", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TranslationProviderWinFormsUiAttribute : ExtensionAttribute
	{
		public override void Validate(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
			base.Validate(info, context);
			context.ValidateRequiredInterface(typeof(ITranslationProviderWinFormsUI));
		}
	}
}
