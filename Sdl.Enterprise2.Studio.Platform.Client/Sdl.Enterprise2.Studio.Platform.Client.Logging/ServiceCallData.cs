using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Sdl.Enterprise2.Studio.Platform.Client.Logging
{
	public class ServiceCallData
	{
		private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

		public Dictionary<string, string> Values => _values;

		public XElement ToXml()
		{
			return new XElement("ServiceCallData", Values.Select(delegate(KeyValuePair<string, string> v)
			{
				XName name = "Data";
				object[] array = new object[2];
				XName name2 = "Name";
				KeyValuePair<string, string> keyValuePair = v;
				array[0] = new XElement(name2, keyValuePair.Key);
				XName name3 = "Value";
				keyValuePair = v;
				array[1] = new XElement(name3, keyValuePair.Value);
				return new XElement(name, array);
			}));
		}

		public XPathNavigator ToTraceData()
		{
			return ToXml().CreateNavigator();
		}
	}
}
