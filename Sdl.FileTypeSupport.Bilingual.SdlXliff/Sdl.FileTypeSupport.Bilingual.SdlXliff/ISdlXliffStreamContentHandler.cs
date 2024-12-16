using System.Collections.Generic;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public interface ISdlXliffStreamContentHandler
	{
		void OnDocInfo(XmlElement docInfo);

		bool OnStartFile(List<XmlAttribute> fileAttributes);

		void OnStartFileHeader();

		void OnFileInfo(XmlElement fileInfo);

		void OnTagDefinition(XmlElement tagDefinition);

		void OnContextDefinition(XmlElement contextDefinition);

		void OnNodeDefinition(XmlElement nodeDefinition);

		void OnFormattingDefinition(XmlElement formattingDefintion);

		void OnFileTypeInfo(XmlElement fileTypeInfo);

		void OnCommentReference(XmlElement commentReference);

		void OnExternalFile(XmlElement externalFile);

		void OnInternalFile(XmlElement internalFile);

		void OnDependencyFiles(XmlElement dependencyFiles);

		void OnEndFileHeader();

		void OnGroup(XmlElement group);

		void OnTranslationUnit(XmlElement translationUnit);

		void OnBinaryUnit(XmlElement binaryUnit);

		void OnEndFile();
	}
}
