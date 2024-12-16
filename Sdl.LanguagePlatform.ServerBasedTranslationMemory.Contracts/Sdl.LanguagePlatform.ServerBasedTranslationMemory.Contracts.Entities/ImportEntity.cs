using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.TranslationMemory;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "Import")]
	[Entity(Schema = "etm", Table = "Import", PrimaryKey = "ImportId")]
	public class ImportEntity : ImportExportEntity
	{
		[DataMember]
		[EntityColumn(ColumnName = "StatRead")]
		public int? Read
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatRawTU")]
		public int? RawTU
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatTotal")]
		public int? Imported
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatAdded")]
		public int? Added
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatMerged")]
		public int? Merged
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatErrors")]
		public int? Errors
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatOverwritten")]
		public int? Overwritten
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatDiscarded")]
		public int? Discarded
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatBad")]
		public int? Bad
		{
			get;
			set;
		}

		[DataMember]
		public string SourceFile
		{
			get;
			set;
		}

		public string ImportSettingsXml
		{
			get
			{
				if (ImportSettings == null)
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter output = new StringWriter(stringBuilder))
				{
					using (XmlWriter writer = XmlWriter.Create(output))
					{
						new DataContractSerializer(typeof(ImportSettings)).WriteObject(writer, ImportSettings);
					}
				}
				return stringBuilder.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					ImportSettings = null;
				}
				else
				{
					using (StringReader input = new StringReader(value))
					{
						using (XmlReader reader = XmlReader.Create(input))
						{
							DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(ImportSettings));
							ImportSettings = (ImportSettings)dataContractSerializer.ReadObject(reader);
						}
					}
				}
			}
		}

		[DataMember]
		[IgnoreEntityMember]
		public ImportSettings ImportSettings
		{
			get;
			set;
		}

		public ImportEntity ShallowCopy()
		{
			return MemberwiseClone() as ImportEntity;
		}

		public ImportEntity ShallowCopy(LanguageDirectionEntity newParent)
		{
			ImportEntity importEntity = ShallowCopy();
			importEntity.LanguageDirection = new EntityReference<LanguageDirectionEntity>(newParent);
			return importEntity;
		}
	}
}
