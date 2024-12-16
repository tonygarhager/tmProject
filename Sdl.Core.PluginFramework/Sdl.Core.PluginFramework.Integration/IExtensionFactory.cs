using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Integration
{
	internal interface IExtensionFactory
	{
		IEnumerable<TResult> Build<TAttribute, TResult>(ExtensionArguments arguments, bool useCache) where TAttribute : ExtensionAttribute where TResult : class;

		TResult Build<TResult>(IExtension extension, ExtensionArguments arguments, bool useCache) where TResult : class;
	}
}
