using System;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class PluginDeserializer
	{
		public static object DeserializeAttribute(IExtension extension, XElement attributeElement, IObjectResolver objectResolver)
		{
			Type type = TypeLoaderUtil.GetType(attributeElement.Attribute("type").Value);
			object obj = objectResolver.CreateObject(type, attributeElement);
			foreach (XElement item in attributeElement.Element("properties").Elements("property"))
			{
				string value = item.Attribute("name").Value;
				PropertyInfo property = type.GetProperty(value);
				object value2 = DeserializeValue(item.Value, property.PropertyType);
				property.SetValue(obj, value2, null);
			}
			InitializeLocalizableProperties(extension, obj);
			return obj;
		}

		public static object DeserializeValue(string valueText, Type destinationType)
		{
			if (destinationType.IsEnum)
			{
				return Enum.Parse(destinationType, valueText, ignoreCase: false);
			}
			if (!(destinationType == typeof(Type)))
			{
				return Convert.ChangeType(valueText, destinationType, CultureInfo.InvariantCulture);
			}
			return TypeLoaderUtil.GetType(valueText);
		}

		private static void InitializeLocalizableProperties(IExtension extension, object attribute)
		{
			PropertyInfo[] properties = attribute.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!(propertyInfo.PropertyType == typeof(string)) || propertyInfo.GetCustomAttributes(typeof(PluginResourceAttribute), inherit: true).Length == 0)
				{
					continue;
				}
				string text = (string)propertyInfo.GetValue(attribute, null);
				if (text != null)
				{
					string pluginResource = extension.Plugin.GetPluginResource<string>(text);
					if (pluginResource != null)
					{
						propertyInfo.SetValue(attribute, pluginResource, null);
					}
				}
			}
		}
	}
}
