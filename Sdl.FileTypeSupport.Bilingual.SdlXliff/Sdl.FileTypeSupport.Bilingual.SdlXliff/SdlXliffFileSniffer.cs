using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Bilingual.SdlXliff.Util;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.IO;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class SdlXliffFileSniffer : INativeFileSniffer
	{
		private bool _disableVersionCheck;

		private bool _reportUnsupportedOlderVersions = true;

		public bool DisableVersionCheck
		{
			get
			{
				return _disableVersionCheck;
			}
			set
			{
				_disableVersionCheck = value;
			}
		}

		public bool ReportUnsupportedOlderVersions
		{
			get
			{
				return _reportUnsupportedOlderVersions;
			}
			set
			{
				_reportUnsupportedOlderVersions = value;
			}
		}

		public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
		{
			SniffInfo sniffInfo = new SniffInfo();
			sniffInfo.IsSupported = true;
			try
			{
				using (FileStream fileStream = File.OpenRead(nativeFilePath))
				{
					if (!CryptographicHelper.IsEncryptedSdlXliff(fileStream))
					{
						fileStream.Seek(0L, SeekOrigin.Begin);
						using (XmlReader xmlReader = XmlReader.Create(fileStream))
						{
							while (xmlReader.Read())
							{
								if (xmlReader.IsStartElement())
								{
									string attribute = xmlReader.GetAttribute("version", "http://sdl.com/FileTypes/SdlXliff/1.0");
									if (xmlReader.ReadToDescendant("file"))
									{
										SetSourceAndTargetLanguages(xmlReader.GetAttribute("source-language"), xmlReader.GetAttribute("target-language"), sniffInfo);
									}
									if (xmlReader.ReadToDescendant("file-info", "http://sdl.com/FileTypes/SdlXliff/1.0"))
									{
										CheckVersionCompatibility(attribute, messageReporter, sniffInfo);
										sniffInfo.DetectedEncoding.Second = DetectionLevel.Certain;
										sniffInfo.SuggestedTargetEncoding = EncodingCategory.NotApplicable;
									}
									return sniffInfo;
								}
							}
							return sniffInfo;
						}
					}
					sniffInfo.SetMetaData("SDL:SDLXLIFF-HEADER", "true");
					return sniffInfo;
				}
			}
			catch (Exception ex)
			{
				sniffInfo.IsSupported = false;
				if (messageReporter == null)
				{
					return sniffInfo;
				}
				ReportMessage(messageReporter, string.Format(StringResources.SniffException, nativeFilePath, ex.Message), ErrorLevel.Warning);
				return sniffInfo;
			}
		}

		private void SetSourceAndTargetLanguages(string xliffSourceLang, string xliffTargetLang, SniffInfo info)
		{
			if (!string.IsNullOrEmpty(xliffSourceLang))
			{
				Language first = new Language(xliffSourceLang);
				info.DetectedSourceLanguage = new Pair<Language, DetectionLevel>(first, DetectionLevel.Certain);
			}
			if (!string.IsNullOrEmpty(xliffTargetLang))
			{
				Language first2 = new Language(xliffTargetLang);
				info.DetectedTargetLanguage = new Pair<Language, DetectionLevel>(first2, DetectionLevel.Certain);
			}
		}

		private bool CheckVersionCompatibility(string sdlXliffVersion, INativeTextLocationMessageReporter messageReporter, SniffInfo info)
		{
			info.IsSupported = false;
			if (DisableVersionCheck)
			{
				info.IsSupported = true;
				return true;
			}
			if (string.IsNullOrEmpty(sdlXliffVersion))
			{
				ReportMessage(messageReporter, StringResources.NoSdlXliffVersion, ErrorLevel.Note);
				return false;
			}
			if (!SdlXliffVersions.ParseVersionString(sdlXliffVersion, out int majorVersion, out int minorVersion))
			{
				ReportMessage(messageReporter, string.Format(StringResources.InvalidSdlXliffVersion, sdlXliffVersion, SdlXliffVersions.CurrentVersionString), ErrorLevel.Warning);
				return false;
			}
			if (majorVersion > 1)
			{
				ReportMessage(messageReporter, string.Format(StringResources.HigherMajorVersion, sdlXliffVersion, SdlXliffVersions.CurrentVersionString), ErrorLevel.Note);
				return false;
			}
			if (majorVersion < 1)
			{
				if (ReportUnsupportedOlderVersions)
				{
					ReportMessage(messageReporter, string.Format(StringResources.LowerMajorVersion, sdlXliffVersion, SdlXliffVersions.CurrentVersionString), ErrorLevel.Note);
				}
				return false;
			}
			if (minorVersion > 0)
			{
				ReportMessage(messageReporter, string.Format(StringResources.HigherMinorVersion, sdlXliffVersion, SdlXliffVersions.CurrentVersionString), ErrorLevel.Warning);
			}
			info.IsSupported = true;
			info.SetMetaData("OriginalVersionNumber", sdlXliffVersion);
			return true;
		}

		private void ReportMessage(INativeTextLocationMessageReporter messageReporter, string message, ErrorLevel level)
		{
			messageReporter?.ReportMessage(this, StringResources.SdlXliffFileSnifferName, level, message, null);
		}
	}
}
