using Sdl.Core.PluginFramework.Validation;
using System;

namespace Sdl.Core.PluginFramework
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AuxiliaryExtensionAttribute : Attribute
	{
		public virtual void Validate(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
		}
	}
}
