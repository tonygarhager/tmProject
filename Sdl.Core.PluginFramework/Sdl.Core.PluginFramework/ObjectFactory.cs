using System;

namespace Sdl.Core.PluginFramework
{
	public class ObjectFactory
	{
		public static T CreateObject<T>(string typeName) where T : class
		{
			return (Activator.CreateInstance(LoadType(typeName)) as T) ?? throw new InvalidCastException("The object is not of the requested type.");
		}

		private static Type LoadType(string typeName)
		{
			return Type.GetType(typeName, throwOnError: true);
		}
	}
}
