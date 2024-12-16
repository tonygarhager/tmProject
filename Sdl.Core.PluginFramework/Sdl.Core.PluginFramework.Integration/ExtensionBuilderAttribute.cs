using System;

namespace Sdl.Core.PluginFramework.Integration
{
	[ExtensionPointInfo("ExtensionBuilders", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class ExtensionBuilderAttribute : ExtensionAttribute
	{
		public Type AttributeType
		{
			get;
			set;
		}

		public Type InstanceType
		{
			get;
			set;
		}

		public ExtensionBuilderAttribute(Type extensionAttributeType, Type instanceType)
		{
			AttributeType = extensionAttributeType;
			InstanceType = instanceType;
		}
	}
}
