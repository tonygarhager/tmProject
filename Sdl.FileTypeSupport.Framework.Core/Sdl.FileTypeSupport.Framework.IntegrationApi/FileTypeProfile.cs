using Sdl.Core.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[DataContract(Namespace = "http://www.sdl.com/filetypesupport", Name = "FileTypeInformation")]
	public sealed class FileTypeProfile : IExtensibleDataObject
	{
		[CollectionDataContract(Namespace = "http://www.sdl.com/filetypesupport", ItemName = "SilverlightSettingsPageId")]
		private class SilverlightSettingsPageIdsList : List<string>
		{
			public SilverlightSettingsPageIdsList()
			{
			}

			public SilverlightSettingsPageIdsList(IEnumerable<string> ids)
				: base(ids)
			{
			}
		}

		[CollectionDataContract(Namespace = "http://www.sdl.com/filetypesupport", ItemName = "WinFormSettingsPageId")]
		private class WinFormSettingsPageIdsList : List<string>
		{
			public WinFormSettingsPageIdsList()
			{
			}

			public WinFormSettingsPageIdsList(IEnumerable<string> ids)
				: base(ids)
			{
			}
		}

		[DataMember(Name = "Expression", EmitDefaultValue = false)]
		private string _expression;

		[DataMember(Name = "FileTypeDefinitionId", EmitDefaultValue = false)]
		public string FileTypeDefinitionId
		{
			get;
			set;
		}

		[DataMember(Name = "FileTypeName", EmitDefaultValue = false)]
		public string FileTypeName
		{
			get;
			set;
		}

		[DataMember(Name = "FileTypeDocumentName", EmitDefaultValue = false)]
		public string FileTypeDocumentName
		{
			get;
			set;
		}

		[DataMember(Name = "FileTypeDocumentsName", EmitDefaultValue = false)]
		public string FileTypeDocumentsName
		{
			get;
			set;
		}

		[DataMember(Name = "FileDialogWildcardExpression", EmitDefaultValue = false)]
		public string FileDialogWildcardExpression
		{
			get;
			set;
		}

		public Regex Expression
		{
			get;
			set;
		}

		[DataMember(Name = "IsStandardCustomization", EmitDefaultValue = false)]
		public bool IsStandardCustomization
		{
			get;
			set;
		}

		[DataMember(Name = "Description", EmitDefaultValue = false)]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Name = "IconContent", EmitDefaultValue = false)]
		public string IconContent
		{
			get;
			set;
		}

		public ExtensionDataObject ExtensionData
		{
			get;
			set;
		}

		public static FileTypeProfile GetFileTypeProfile(string fileTypeProfileXml)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(FileTypeProfile));
			using (StringReader input = new StringReader(fileTypeProfileXml))
			{
				using (XmlReader reader = XmlReader.Create(input))
				{
					return (FileTypeProfile)dataContractSerializer.ReadObject(reader);
				}
			}
		}

		public static string GetFileTypeProfileXml(FileTypeProfile fileTypeProfile)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(FileTypeProfile));
			using (StringWriter stringWriter = new StringWriter())
			{
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.Indent = true;
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
				{
					dataContractSerializer.WriteObject(xmlWriter, fileTypeProfile);
					xmlWriter.Flush();
					return stringWriter.ToString();
				}
			}
		}

		public static void ApplyProfile(IFileTypeDefinition fileTypeDefinition, FileTypeProfile fileTypeProfile)
		{
			fileTypeDefinition.CustomizationLevel = (fileTypeProfile.IsStandardCustomization ? FileTypeDefinitionCustomizationLevel.CustomizedStandard : FileTypeDefinitionCustomizationLevel.FullyCustomized);
			IFileTypeInformation fileTypeInformation = fileTypeDefinition.FileTypeInformation;
			fileTypeInformation.FileTypeDefinitionId = new FileTypeDefinitionId(fileTypeProfile.FileTypeDefinitionId);
			if (fileTypeProfile.FileTypeName != null)
			{
				fileTypeInformation.FileTypeName = new LocalizableString(fileTypeProfile.FileTypeName);
			}
			if (fileTypeProfile.FileTypeDocumentName != null)
			{
				fileTypeInformation.FileTypeDocumentName = new LocalizableString(fileTypeProfile.FileTypeDocumentName);
			}
			if (fileTypeProfile.FileTypeDocumentsName != null)
			{
				fileTypeInformation.FileTypeDocumentsName = new LocalizableString(fileTypeProfile.FileTypeDocumentsName);
			}
			if (fileTypeProfile.FileDialogWildcardExpression != null)
			{
				fileTypeInformation.FileDialogWildcardExpression = fileTypeProfile.FileDialogWildcardExpression;
			}
			if (fileTypeProfile.Description != null)
			{
				fileTypeInformation.Description = new LocalizableString(fileTypeProfile.Description);
			}
			if (fileTypeProfile.IconContent != null)
			{
				fileTypeInformation.Icon = new IconDescriptor("Embedded.ico", fileTypeProfile.IconContent);
			}
		}

		[OnSerializing]
		private void BeforeSerializing(StreamingContext streamingContext)
		{
			_expression = ((Expression != null) ? Expression.ToString() : null);
		}

		[OnSerialized]
		private void AfterSerializing(StreamingContext streamingContext)
		{
			_expression = null;
		}

		[OnDeserialized]
		private void AfterDeserializing(StreamingContext streamingContext)
		{
			Expression = ((_expression != null) ? new Regex(_expression) : null);
		}
	}
}
