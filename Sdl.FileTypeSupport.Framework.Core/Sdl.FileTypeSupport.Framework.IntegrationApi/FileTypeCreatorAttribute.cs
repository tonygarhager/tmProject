using Sdl.Core.PluginFramework;
using Sdl.Core.PluginFramework.Validation;
using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[ExtensionPointInfo("File Type Creators", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class FileTypeCreatorAttribute : ExtensionAttribute
	{
		public override void Validate(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
			base.Validate(info, context);
			context.ValidateRequiredInterface(typeof(IFileTypeCreator));
		}
	}
}
