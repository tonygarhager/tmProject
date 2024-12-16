using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.TranslationMemory;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "Export")]
	[Entity(Schema = "etm", Table = "Export", PrimaryKey = "ExportId")]
	public class ExportEntity : ImportExportEntity
	{
		[DataMember]
		[EntityColumn(ColumnName = "StatProcessed")]
		public int? Processed
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatExported")]
		public int? Exported
		{
			get;
			set;
		}

		public string FilterExpressionXml
		{
			get
			{
				if (FilterExpression == null)
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter output = new StringWriter(stringBuilder))
				{
					using (XmlWriter writer = XmlWriter.Create(output))
					{
						new DataContractSerializer(typeof(FilterExpression)).WriteObject(writer, FilterExpression);
					}
				}
				return stringBuilder.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					FilterExpression = null;
				}
				else
				{
					using (StringReader input = new StringReader(value))
					{
						using (XmlReader reader = XmlReader.Create(input))
						{
							DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(FilterExpression));
							FilterExpression = (FilterExpression)dataContractSerializer.ReadObject(reader);
						}
					}
				}
			}
		}

		[DataMember]
		[IgnoreEntityMember]
		public FilterExpression FilterExpression
		{
			get;
			set;
		}
	}
}
