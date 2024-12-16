using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.PluginFramework.Integration
{
	internal class ExtensionBuilderCatalog
	{
		private struct PairAttributeResultType
		{
			public readonly Type AttributeType;

			public readonly Type ResultType;

			public static PairAttributeResultType CreateKey<TResult>(Type extensionAttributeType)
			{
				return new PairAttributeResultType(extensionAttributeType, typeof(TResult));
			}

			public PairAttributeResultType(Type attributeType, Type resultType)
			{
				AttributeType = attributeType;
				ResultType = resultType;
			}
		}

		private static readonly ExtensionBuilderCatalog _instance = new ExtensionBuilderCatalog();

		private readonly Dictionary<PairAttributeResultType, IExtensionBuilder> _builders = new Dictionary<PairAttributeResultType, IExtensionBuilder>();

		public static ExtensionBuilderCatalog Instance => _instance;

		private ExtensionBuilderCatalog()
		{
			foreach (IExtension extension in PluginManager.DefaultPluginRegistry.GetExtensionPoint<ExtensionBuilderAttribute>().Extensions)
			{
				ExtensionBuilderAttribute extensionBuilderAttribute = (ExtensionBuilderAttribute)extension.ExtensionAttribute;
				PairAttributeResultType key = new PairAttributeResultType(extensionBuilderAttribute.AttributeType, extensionBuilderAttribute.InstanceType);
				IExtensionBuilder extensionBuilder = extension.CreateInstance() as IExtensionBuilder;
				if (extensionBuilder != null)
				{
					_builders.Add(key, extensionBuilder);
				}
			}
		}

		public IExtensionBuilder FirstOrDefault<TResult>(IExtension extension)
		{
			Type type = extension.ExtensionAttribute.GetType();
			if (_builders.TryGetValue(PairAttributeResultType.CreateKey<TResult>(type), out IExtensionBuilder value))
			{
				return value;
			}
			List<KeyValuePair<PairAttributeResultType, IExtensionBuilder>> list = _builders.Where((KeyValuePair<PairAttributeResultType, IExtensionBuilder> a) => typeof(TResult).IsAssignableFrom(a.Key.ResultType)).ToList();
			if (list.Count > 1)
			{
				throw new AmbiguousMatchException($"There are multiple extension adaptors with the same assignable ResultType {typeof(TResult)}");
			}
			if (list.Count == 1)
			{
				return list.First().Value;
			}
			return null;
		}
	}
}
