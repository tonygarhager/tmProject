using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Sdl.Core.Api.DataAccess
{
	public static class EntityExtensions
	{
		public static XPathNavigator GetXPathNavigator(this Entity entity)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			try
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(entity.GetType());
				using (StringWriter output = new StringWriter(stringBuilder))
				{
					using (XmlWriter writer = XmlWriter.Create(output))
					{
						dataContractSerializer.WriteObject(writer, entity);
					}
				}
			}
			catch (InvalidDataContractException)
			{
				flag = true;
			}
			catch (SerializationException)
			{
				flag = true;
			}
			catch (QuotaExceededException)
			{
				flag = true;
			}
			if (flag)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("<SerializationError>Data contract serialization failed for {0}. A valid XPathNavigator for the Entity graph could not be constructed.</SerializationError>", entity.GetType().ToString());
			}
			using (StringReader input = new StringReader(stringBuilder.ToString()))
			{
				using (XmlTextReader reader = new XmlTextReader(input))
				{
					XPathDocument xPathDocument = new XPathDocument(reader);
					return xPathDocument.CreateNavigator();
				}
			}
		}
	}
}
