using Sdl.Core.PluginFramework.Implementation;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Sdl.Core.PluginFramework
{
	public class DefaultObjectResolver : IObjectResolver
	{
		public bool CanResolve(Type objectType)
		{
			return true;
		}

		public object CreateObject(Type objectType, XElement attributeElement = null)
		{
			if (attributeElement == null)
			{
				return Activator.CreateInstance(objectType);
			}
			List<object> list = new List<object>();
			foreach (XElement item2 in attributeElement.Element("constructorArgs").Elements("arg"))
			{
				string value = item2.Attribute("type").Value;
				string value2 = item2.Value;
				Type type = TypeLoaderUtil.GetType(value);
				object item = PluginDeserializer.DeserializeValue(value2, type);
				list.Add(item);
			}
			return Activator.CreateInstance(objectType, list.ToArray());
		}
	}
}
