using System;
using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Integration
{
	public sealed class ExtensionStoreItem
	{
		public readonly List<ValuePairTypeInstance> References = new List<ValuePairTypeInstance>();

		public Type AttributeType
		{
			get;
			set;
		}

		public IExtension Extension
		{
			get;
			set;
		}

		public Type ExtensionType
		{
			get;
			set;
		}

		public object DefaultInstance
		{
			get;
			set;
		}

		internal ExtensionStoreItem(IExtension extension)
		{
			Extension = extension;
			AttributeType = extension.ExtensionAttribute.GetType();
			ExtensionType = extension.ExtensionType;
			DefaultInstance = extension.CreateInstance();
			References.Add(ValuePairTypeInstance.Create(extension.ExtensionType, DefaultInstance));
		}
	}
}
