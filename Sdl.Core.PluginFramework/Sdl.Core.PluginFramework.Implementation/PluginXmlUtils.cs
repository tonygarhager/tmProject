using System.Globalization;
using System.Xml.Linq;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal static class PluginXmlUtils
	{
		public static string GetRequiredAttribute(Plugin plugin, XElement element, string attributeName)
		{
			XAttribute xAttribute = element.Attribute(XName.Get(attributeName, string.Empty));
			if (xAttribute == null || string.IsNullOrEmpty(xAttribute.Value))
			{
				throw new PluginFrameworkException(string.Format(CultureInfo.InvariantCulture, StringResources.Plugin_RequiredAttributeMissing, new object[3]
				{
					attributeName,
					element.Name,
					plugin.Descriptor.Name
				}));
			}
			return xAttribute.Value;
		}
	}
}
