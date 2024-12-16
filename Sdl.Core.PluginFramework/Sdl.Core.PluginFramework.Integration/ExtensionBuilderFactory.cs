using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.PluginFramework.Integration
{
	internal sealed class ExtensionBuilderFactory : IExtensionFactory
	{
		private static readonly ExtensionBuilderFactory _instance = new ExtensionBuilderFactory();

		public static ExtensionBuilderFactory Instance => _instance;

		private ExtensionBuilderFactory()
		{
		}

		public IEnumerable<TResult> Build<TAttribute, TResult>(ExtensionArguments arguments = null, bool useCache = true) where TAttribute : ExtensionAttribute where TResult : class
		{
			return PluginManager.DefaultPluginRegistry.GetExtensionPoint<TAttribute>().Extensions.Select((IExtension extension) => Build<TResult>(extension, arguments, useCache)).ToList();
		}

		public TResult Build<TResult>(IExtension extension, ExtensionArguments arguments = null, bool useCache = true) where TResult : class
		{
			if (useCache)
			{
				TResult assignableInstanceOrDefault = ExtensionsRepository.Instance.GetAssignableInstanceOrDefault<TResult>(extension);
				if (assignableInstanceOrDefault != null)
				{
					return assignableInstanceOrDefault;
				}
			}
			IExtensionBuilder extensionBuilder = ExtensionBuilderCatalog.Instance.FirstOrDefault<TResult>(extension);
			if (extensionBuilder != null)
			{
				if (!useCache)
				{
					return extensionBuilder.Build(extension, extension.CreateInstance(), arguments) as TResult;
				}
				return ExtensionsRepository.Instance.AddExtension<TResult>(extension, arguments, extensionBuilder.Build);
			}
			throw new NotSupportedException($"Cannot build an instance for {extension.ExtensionType.Name} to be assignable to type {typeof(TResult)}");
		}
	}
}
