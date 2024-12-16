using Sdl.Core.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class LanguageReader : GenericStreamReader
	{
		public Language SourceLanguage
		{
			get;
			private set;
		}

		public Language TargetLanguage
		{
			get;
			private set;
		}

		public string XliffOrignalFilePath
		{
			get;
			set;
		}

		public override bool OnStartFile(List<XmlAttribute> fileAttributes)
		{
			XmlAttribute srcLanguage = fileAttributes.FirstOrDefault((XmlAttribute attr) => attr.LocalName == "source-language");
			ProcessSourceLanguageAttribute(srcLanguage);
			XmlAttribute tgtLanguage = fileAttributes.FirstOrDefault((XmlAttribute attr) => attr.LocalName == "target-language");
			ProcessTargetLanguageAttribute(tgtLanguage);
			return true;
		}

		private void ProcessSourceLanguageAttribute(XmlAttribute srcLanguage)
		{
			if (!string.IsNullOrEmpty(srcLanguage?.Value))
			{
				Language language = new Language(srcLanguage.Value);
				if (SourceLanguage != null && SourceLanguage.IsValid && !SourceLanguage.Equals(language))
				{
					throw new XliffParseException(string.Format(StringResources.CorruptFile_FileAndDocumentSourceLangaugeDiffers, string.IsNullOrEmpty(XliffOrignalFilePath) ? string.Empty : XliffOrignalFilePath, language, SourceLanguage));
				}
				SourceLanguage = language;
			}
		}

		private void ProcessTargetLanguageAttribute(XmlAttribute tgtLanguage)
		{
			if (!string.IsNullOrEmpty(tgtLanguage?.Value))
			{
				Language language = new Language(tgtLanguage.Value);
				if (TargetLanguage != null && TargetLanguage.IsValid && !TargetLanguage.Equals(language))
				{
					throw new XliffParseException(string.Format(StringResources.CorruptFile_FileAndDocumentTargetLanguageDiffers, string.IsNullOrEmpty(XliffOrignalFilePath) ? string.Empty : XliffOrignalFilePath, language, TargetLanguage));
				}
				TargetLanguage = language;
			}
		}
	}
}
