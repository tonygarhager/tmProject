using System;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class TypeLoaderUtil
	{
		public static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName, throwOnError: false);
			if (type != null)
			{
				return type;
			}
			return Type.GetType(typeName, (AssemblyName name) => AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly z) => z.FullName == name.FullName), null, throwOnError: true);
		}
	}
}
