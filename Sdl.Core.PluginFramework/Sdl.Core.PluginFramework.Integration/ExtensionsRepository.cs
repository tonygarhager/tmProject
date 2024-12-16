using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.PluginFramework.Integration
{
	public sealed class ExtensionsRepository
	{
		private static readonly ExtensionsRepository _instance = new ExtensionsRepository();

		private readonly List<ExtensionStoreItem> _extensions = new List<ExtensionStoreItem>();

		public static ExtensionsRepository Instance => _instance;

		private ExtensionsRepository()
		{
		}

		public ExtensionStoreItem FirstOrDefault(Func<ExtensionStoreItem, bool> predicate)
		{
			return _extensions.FirstOrDefault(predicate);
		}

		public ExtensionStoreItem FirstOrDefault(object extensionInstanceReference)
		{
			return FirstOrDefault((ExtensionStoreItem e) => e.References.FirstOrDefault((ValuePairTypeInstance r) => r.Instance == extensionInstanceReference) != null);
		}

		public TResult GetAssignableInstanceOrDefault<TResult>(Type extensionAttributeType, Type extensionType) where TResult : class
		{
			ExtensionStoreItem extensionStoreItem = FirstOrDefault((ExtensionStoreItem e) => e.AttributeType == extensionAttributeType && e.ExtensionType == extensionType);
			if (extensionStoreItem != null)
			{
				ValuePairTypeInstance valuePairTypeInstance = extensionStoreItem.References.FirstOrDefault((ValuePairTypeInstance r) => r.ResultType == typeof(TResult));
				if (valuePairTypeInstance != null)
				{
					return valuePairTypeInstance.Instance as TResult;
				}
				List<ValuePairTypeInstance> list = extensionStoreItem.References.Where((ValuePairTypeInstance c) => c.ResultType as TResult != null).ToList();
				if (list.Count > 1)
				{
					throw new AmbiguousMatchException($"There are multiple extension instances using attribute {extensionAttributeType.Name} which are assignable to type {typeof(TResult)}.");
				}
				valuePairTypeInstance = list.FirstOrDefault();
				if (valuePairTypeInstance == null)
				{
					return null;
				}
				return valuePairTypeInstance.Instance as TResult;
			}
			return null;
		}

		public TResult GetAssignableInstanceOrDefault<TAttribute, TExtensionType, TResult>() where TAttribute : ExtensionAttribute where TResult : class
		{
			return GetAssignableInstanceOrDefault<TResult>(typeof(TAttribute), typeof(TExtensionType));
		}

		public TResult GetAssignableInstanceOrDefault<TResult>(IExtension extension) where TResult : class
		{
			return GetAssignableInstanceOrDefault<TResult>(extension.ExtensionAttribute.GetType(), extension.ExtensionType);
		}

		public TResult AddExtension<TResult>(IExtension extension, ExtensionArguments arguments, Func<IExtension, object, ExtensionArguments, object> instanceCreator) where TResult : class
		{
			Type extensionAttributeType = extension.ExtensionAttribute.GetType();
			ExtensionStoreItem extensionStoreItem = _extensions.FirstOrDefault((ExtensionStoreItem e) => e.AttributeType == extensionAttributeType && e.ExtensionType == extension.ExtensionType);
			if (extensionStoreItem != null)
			{
				TResult assignableInstanceOrDefault = GetAssignableInstanceOrDefault<TResult>(extension);
				if (assignableInstanceOrDefault != null)
				{
					return assignableInstanceOrDefault;
				}
				assignableInstanceOrDefault = (TResult)instanceCreator(extension, extensionStoreItem.DefaultInstance, arguments);
				extensionStoreItem.References.Add(ValuePairTypeInstance.Create<TResult>(assignableInstanceOrDefault));
				return assignableInstanceOrDefault;
			}
			_extensions.Add(new ExtensionStoreItem(extension));
			return AddExtension<TResult>(extension, arguments, instanceCreator);
		}
	}
}
