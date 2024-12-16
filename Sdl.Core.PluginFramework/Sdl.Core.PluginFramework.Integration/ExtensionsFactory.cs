using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Integration
{
	public sealed class ExtensionsFactory
	{
		public static IEnumerable<TResult> Create<TAttribute, TResult>(ExtensionArguments arguments = null, bool useCache = true) where TAttribute : ExtensionAttribute where TResult : class
		{
			return ExtensionBuilderFactory.Instance.Build<TAttribute, TResult>(arguments, useCache);
		}

		public static TResult Create<TResult>(IExtension extension, ExtensionArguments arguments = null, bool useCache = true) where TResult : class
		{
			return ExtensionBuilderFactory.Instance.Build<TResult>(extension, arguments, useCache);
		}
	}
}
