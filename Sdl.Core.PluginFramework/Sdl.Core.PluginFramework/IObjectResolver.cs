using System;
using System.Xml.Linq;

namespace Sdl.Core.PluginFramework
{
	public interface IObjectResolver
	{
		bool CanResolve(Type objectType);

		object CreateObject(Type objectType, XElement attributeElement = null);
	}
}
