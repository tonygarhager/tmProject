using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class FilterDefinitionReader : GenericStreamReader
	{
		private readonly List<FileTypeDefinitionId> _fileTypeDefIds = new List<FileTypeDefinitionId>();

		private readonly List<FileTypeDefinitionId> _fileTypeDefIdsForCurrentFile = new List<FileTypeDefinitionId>();

		public FileTypeDefinitionId[] GetFilterDefinitionIds(string xliffInputFilePath, string encryptionKey)
		{
			_fileTypeDefIds.Clear();
			ProcessStream(xliffInputFilePath, encryptionKey);
			return _fileTypeDefIds.ToArray();
		}

		public override bool OnStartFile(List<XmlAttribute> fileAttributes)
		{
			_fileTypeDefIdsForCurrentFile.Clear();
			return true;
		}

		public override void OnFileTypeInfo(XmlElement fileTypeInfo)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(fileTypeInfo.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("sdl", "http://sdl.com/FileTypes/SdlXliff/1.0");
			XmlElement xmlElement = fileTypeInfo.SelectSingleNode(SdlXliffNames.Prefixed("filetype-id", xmlNamespaceManager), xmlNamespaceManager) as XmlElement;
			if (xmlElement != null)
			{
				_fileTypeDefIdsForCurrentFile.Add(new FileTypeDefinitionId(xmlElement.InnerText));
			}
		}

		public override void OnEndFile()
		{
			if (_fileTypeDefIdsForCurrentFile.Count > 0)
			{
				_fileTypeDefIds.AddRange(_fileTypeDefIdsForCurrentFile);
				return;
			}
			if (base.MessageReporter != null)
			{
				base.MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Error, StringResources.CorruptFile_FileTypeIdNotFound, null);
			}
			_fileTypeDefIds.Add(new FileTypeDefinitionId("Unknown File Type ID"));
		}
	}
}
