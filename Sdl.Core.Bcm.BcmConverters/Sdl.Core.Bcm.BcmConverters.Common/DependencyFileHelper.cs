using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	public static class DependencyFileHelper
	{
		private static Dictionary<string, DependencyFileType> _dependencyFileInfo = new Dictionary<string, DependencyFileType>
		{
			{
				"BilingualExcel.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Email.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.FrameMaker/OriginalFile",
				DependencyFileType.Source
			},
			{
				"FrameMaker.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"input_file",
				DependencyFileType.Source
			},
			{
				"Html.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"ORIGINAL_XML_DEPENDENCYFILEID",
				DependencyFileType.Source
			},
			{
				"http://www.sdl.com/Sdl.FileTypeSupport.Filters.Idml/OriginalFile",
				DependencyFileType.Source
			},
			{
				"http://www.sdl.com/Sdl.FileTypeSupport.Bilingual.Itd/OriginalFile",
				DependencyFileType.Source
			},
			{
				"Json.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Markdown.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Docx.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Pptx.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Xlsx.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"OriginalOpenDocumentTextDocument",
				DependencyFileType.Source
			},
			{
				"Photoshop.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Subtitles.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"http://www.sdl.com/Sdl.FileTypeSupport.Bilingual.Ttx/OriginalTtxFile",
				DependencyFileType.Source
			},
			{
				"Visio.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"WORLDSERVER_ORIGINAL_XLIFF",
				DependencyFileType.Source
			},
			{
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile",
				DependencyFileType.Source
			},
			{
				"Xliff2.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Xml.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"Yaml.DependencyFileId",
				DependencyFileType.Source
			},
			{
				"PreTweak.Docx.DependencyFileId",
				DependencyFileType.PreTweakedSource
			},
			{
				"PreTweak.Xlsx.DependencyFileId",
				DependencyFileType.PreTweakedSource
			},
			{
				"PreTweak.Pptx.DependencyFileId",
				DependencyFileType.PreTweakedSource
			},
			{
				"http://www.sdl.com/Sdl.FileTypeSupport.Bilingual.Ttx/TagSettingsFile",
				DependencyFileType.Other
			}
		};

		private static Dictionary<string, string> _fileTypeDefinitionIdToDependencyFileId = new Dictionary<string, string>
		{
			{
				"Bilingual Excel v 1.0.0.0",
				"BilingualExcel.DependencyFileId"
			},
			{
				"Bilingual Workbench 1.0.0.0",
				"input_file"
			},
			{
				"DOC v 2.0.0.0",
				"PreTweak.Docx.DependencyFileId"
			},
			{
				"Email v 1.0.0.0",
				"Email.DependencyFileId"
			},
			{
				"FrameMaker 8.0 v 2.0.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.FrameMaker/OriginalFile"
			},
			{
				"FrameMaker v 10.0.0",
				"FrameMaker.DependencyFileId"
			},
			{
				"Tab Delimited v 2.0.0.0",
				"input_file"
			},
			{
				"CSV v 2.0.0.0",
				"input_file"
			},
			{
				"Delimited Text v 2.0.0.0",
				"input_file"
			},
			{
				"Html 5 2.0.0.0",
				"Html.DependencyFileId"
			},
			{
				"Html 4 2.0.0.0",
				"Html.DependencyFileId"
			},
			{
				"Html File v 2.0.0.0",
				"Html.DependencyFileId"
			},
			{
				"ICML Filter 1.0.0.0",
				"ORIGINAL_XML_DEPENDENCYFILEID"
			},
			{
				"IDML v 1.0.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Filters.Idml/OriginalFile"
			},
			{
				"Inx 1.0.0.0",
				"input_file"
			},
			{
				"ITD v 1.0.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Bilingual.Itd/OriginalFile"
			},
			{
				"JSON v 1.0.0.0",
				"Json.DependencyFileId"
			},
			{
				"Markdown v 1.0.0.0",
				"Markdown.DependencyFileId"
			},
			{
				"WordprocessingML v. 2",
				"Docx.DependencyFileId"
			},
			{
				"PresentationML v. 1",
				"Pptx.DependencyFileId"
			},
			{
				"SpreadsheetML v. 1",
				"Xlsx.DependencyFileId"
			},
			{
				"Odt 1.0.0.0",
				"OriginalOpenDocumentTextDocument"
			},
			{
				"Odp 1.0.0.0",
				"OriginalOpenDocumentTextDocument"
			},
			{
				"Ods 1.0.0.0",
				"OriginalOpenDocumentTextDocument"
			},
			{
				"PDF v 3.0.0.0",
				"Docx.DependencyFileId"
			},
			{
				"Photoshop v 1.0.0.0",
				"Photoshop.DependencyFileId"
			},
			{
				"PPT v 2.0.0.0",
				"PreTweak.Pptx.DependencyFileId"
			},
			{
				"RTF v 2.0.0.0",
				"PreTweak.Docx.DependencyFileId"
			},
			{
				"Subtitles v 1.0.0.0",
				"Subtitles.DependencyFileId"
			},
			{
				"Ttx",
				"TTX 2.0 v 2.0.0.0 http://www.sdl.com/Sdl.FileTypeSupport.Bilingual.Ttx/OriginalTtxFile"
			},
			{
				"Visio v 1.0.0.0",
				"Visio.DependencyFileId"
			},
			{
				"WsXliff 1.0.0.0",
				"WORLDSERVER_ORIGINAL_XLIFF"
			},
			{
				"XHTML 1.1 v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XLIFF 1.1-1.2 v 2.0.0.0",
				"input_file"
			},
			{
				"MemoQ v 1.0.0.0",
				"input_file"
			},
			{
				"XLIFF 2.0 v 1.0.0.0",
				"Xliff2.DependencyFileId"
			},
			{
				"XLS v 3.0.0.0",
				"PreTweak.Xlsx.DependencyFileId"
			},
			{
				"XML: Author-it 1.2 v 1.0.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML: DITA 1.1 v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML: DITA 1.2 v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML: MadCap 1.2 v 1.0.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"DocBook 4.5 v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML: RESX v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML: ITS 1.0 v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML: Any v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML v 1.2.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML v 1.3.0.0",
				"http://www.sdl.com/Sdl.FileTypeSupport.Native.Xml/OriginalXmlFile"
			},
			{
				"XML: Author-IT 1.2 v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML: DITA 1.3 v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML: DocBook 4.5 v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML: ITS 1.0 v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML: MadCap 1.2 v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML: RESX v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XHTML 1.1 v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML: Any v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"XML with Element Rules v 2.0.0.0",
				"Xml.DependencyFileId"
			},
			{
				"YAML v 1.0.0.0",
				"Yaml.DependencyFileId"
			}
		};

		public static DependencyFileType GetDependencyFileType(string dependencyFileId, string fileTypeDefinitionId)
		{
			if (fileTypeDefinitionId == "PDF v 3.0.0.0" && dependencyFileId == "Docx.DependencyFileId")
			{
				return DependencyFileType.PreTweakedSource;
			}
			if (_dependencyFileInfo.ContainsKey(dependencyFileId))
			{
				return _dependencyFileInfo[dependencyFileId];
			}
			return DependencyFileType.Other;
		}

		public static string GetDependencyFileId(string fileTypeDefinitionId)
		{
			if (_fileTypeDefinitionIdToDependencyFileId.ContainsKey(fileTypeDefinitionId))
			{
				return _fileTypeDefinitionIdToDependencyFileId[fileTypeDefinitionId];
			}
			return null;
		}

		public static string MakeShortGuid(int length)
		{
			return Guid.NewGuid().ToString("N").Substring(0, length);
		}
	}
}
