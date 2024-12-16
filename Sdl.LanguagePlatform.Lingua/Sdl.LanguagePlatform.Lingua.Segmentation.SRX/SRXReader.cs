using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Sdl.LanguagePlatform.Lingua.Segmentation.SRX
{
	public class SRXReader
	{
		public static bool ValidateSRX(string fileName)
		{
			try
			{
				SRX sRX = Read(fileName);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static SRX Read(string fileName)
		{
			using (XmlTextReader rdr = new XmlTextReader(fileName))
			{
				XmlReader xmlReader = CreateReader(rdr);
				int num = 0;
				SRX sRX = new SRX();
				LanguageRule languageRule = null;
				Rule rule = null;
				MapRule mapRule = null;
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType != XmlNodeType.XmlDeclaration)
					{
						if (num == 999)
						{
							break;
						}
						bool flag = xmlReader.NodeType == XmlNodeType.Element;
						switch (num)
						{
						case 0:
							if (flag && xmlReader.Name == "srx")
							{
								bool flag2 = false;
								if (xmlReader.HasAttributes)
								{
									while (xmlReader.MoveToNextAttribute())
									{
										if (!(xmlReader.Name != "version"))
										{
											flag2 = true;
											if (!xmlReader.HasValue || xmlReader.Value != "1.0")
											{
												throw new LanguagePlatformException(ErrorCode.SegmentationSRXUnsupportedVersion, xmlReader.Value);
											}
										}
									}
									xmlReader.MoveToElement();
								}
								if (!flag2)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXNoVersion);
								}
								num = 1;
							}
							break;
						case 1:
							if (!(xmlReader.Name == "header" && flag))
							{
								throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "expected \"header\"");
							}
							if (xmlReader.HasAttributes)
							{
								while (xmlReader.MoveToNextAttribute())
								{
									if (xmlReader.Name == "segmentsubflows")
									{
										sRX.SegmentSubflows = GetYesNo(xmlReader.Value);
									}
								}
								xmlReader.MoveToElement();
							}
							num = (xmlReader.IsEmptyElement ? 3 : 2);
							break;
						case 2:
							if (xmlReader.Name == "header" && xmlReader.NodeType == XmlNodeType.EndElement)
							{
								num = 3;
							}
							else
							{
								if (!flag || !(xmlReader.Name == "formathandle"))
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "expected \"formathandle\"");
								}
								FormatHandle formatHandle = new FormatHandle();
								bool flag3 = false;
								if (xmlReader.HasAttributes)
								{
									flag3 = true;
									while (xmlReader.MoveToNextAttribute())
									{
										if (xmlReader.Name == "type")
										{
											formatHandle.Type = xmlReader.Value;
										}
										else if (xmlReader.Name == "include")
										{
											formatHandle.Include = GetYesNo(xmlReader.Value);
										}
									}
									xmlReader.MoveToElement();
								}
								if (flag3)
								{
									sRX.FormatHandleRules.Add(formatHandle);
								}
							}
							break;
						case 3:
							if (!flag || !(xmlReader.Name == "body"))
							{
								throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "expected \"body\"");
							}
							num = (xmlReader.IsEmptyElement ? 5 : 4);
							break;
						case 4:
							if (flag && xmlReader.Name == "languagerules")
							{
								num = 6;
							}
							else if (flag && xmlReader.Name == "maprules")
							{
								num = 9;
							}
							else
							{
								if (!(xmlReader.Name == "body") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "error in \"body\"");
								}
								num = 5;
							}
							break;
						case 5:
							if (!(xmlReader.Name == "srx") || xmlReader.NodeType != XmlNodeType.EndElement)
							{
								throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "expected \"/srx\"");
							}
							num = 999;
							break;
						case 6:
							if (flag && xmlReader.Name == "languagerule")
							{
								languageRule = new LanguageRule();
								if (xmlReader.HasAttributes)
								{
									while (xmlReader.MoveToNextAttribute())
									{
										if (xmlReader.Name == "languagerulename")
										{
											languageRule.Name = xmlReader.Value;
										}
									}
									xmlReader.MoveToElement();
								}
								num = 7;
							}
							else
							{
								if (!(xmlReader.Name == "languagerules") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "error in langaugerules");
								}
								num = 4;
							}
							break;
						case 7:
							if (flag && xmlReader.Name == "rule")
							{
								rule = new Rule();
								if (xmlReader.HasAttributes)
								{
									while (xmlReader.MoveToNextAttribute())
									{
										if (xmlReader.Name == "break")
										{
											rule.IsBreak = GetYesNo(xmlReader.Value);
										}
									}
									xmlReader.MoveToElement();
								}
								if (xmlReader.IsEmptyElement)
								{
									languageRule.Rules.Add(rule);
									rule = null;
									num = 7;
								}
								else
								{
									num = 8;
								}
							}
							else
							{
								if (!(xmlReader.Name == "languagerule") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "error in languagerule");
								}
								sRX.LanguageRules.Add(languageRule);
								languageRule = null;
								num = 6;
							}
							break;
						case 8:
							if (flag && xmlReader.Name == "beforebreak")
							{
								num = ((!xmlReader.IsEmptyElement) ? 11 : 8);
							}
							else if (flag && xmlReader.Name == "afterbreak")
							{
								num = ((!xmlReader.IsEmptyElement) ? 12 : 8);
							}
							else
							{
								if (!(xmlReader.Name == "rule") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "error in \"rule\"");
								}
								languageRule.Rules.Add(rule);
								rule = null;
								num = 7;
							}
							break;
						case 9:
							if (flag && xmlReader.Name == "maprule")
							{
								mapRule = new MapRule();
								if (xmlReader.HasAttributes)
								{
									while (xmlReader.MoveToNextAttribute())
									{
										if (xmlReader.Name == "maprulename")
										{
											mapRule.Name = xmlReader.Value;
										}
									}
									xmlReader.MoveToElement();
								}
								num = 10;
							}
							else
							{
								if (!(xmlReader.Name == "maprules") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "error in maprules");
								}
								num = 4;
							}
							break;
						case 10:
							if (flag && xmlReader.Name == "languagemap")
							{
								bool flag4 = false;
								LanguageMap languageMap = new LanguageMap();
								if (xmlReader.HasAttributes)
								{
									flag4 = true;
									while (xmlReader.MoveToNextAttribute())
									{
										switch (xmlReader.Name)
										{
										case "languagepattern":
											languageMap.Pattern = xmlReader.Value;
											break;
										case "languagerulename":
											languageMap.LanguageRuleName = xmlReader.Value;
											break;
										}
									}
									xmlReader.MoveToElement();
								}
								if (flag4)
								{
									mapRule.LanguageMaps.Add(languageMap);
								}
							}
							else
							{
								if (!(xmlReader.Name == "maprule") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "error in maprule");
								}
								sRX.MapRules.Add(mapRule);
								mapRule = null;
								num = 9;
							}
							break;
						case 11:
							if (xmlReader.NodeType == XmlNodeType.Text)
							{
								rule.BeforeBreak = MapICURXToDotNetRX(xmlReader.Value);
							}
							else
							{
								if (!(xmlReader.Name == "beforebreak") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "expected break rule content");
								}
								num = 8;
							}
							break;
						case 12:
							if (xmlReader.NodeType == XmlNodeType.Text)
							{
								rule.AfterBreak = MapICURXToDotNetRX(xmlReader.Value);
							}
							else
							{
								if (!(xmlReader.Name == "afterbreak") || xmlReader.NodeType != XmlNodeType.EndElement)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationSRXParseError, "expected break rule content");
								}
								num = 8;
							}
							break;
						default:
							throw new LanguagePlatformException(ErrorCode.SegmentationSRXInternalError);
						}
					}
				}
				if (num != 999)
				{
					throw new LanguagePlatformException(ErrorCode.SegmentationSRXInvalidDocument);
				}
				return sRX;
			}
		}

		public static string MapICURXToDotNetRX(string icurx)
		{
			if (string.IsNullOrEmpty(icurx))
			{
				return icurx;
			}
			Regex regex = new Regex("\\\\x(?=[0-9a-fA-F]{4})");
			return regex.Replace(icurx, "\\u", -1);
		}

		private static bool GetYesNo(string s)
		{
			return s?.Equals("yes", StringComparison.OrdinalIgnoreCase) ?? false;
		}

		public static LanguageRule FindRules(SRX instance, CultureInfo culture)
		{
			string name = culture.Name;
			if (instance == null)
			{
				throw new ArgumentNullException();
			}
			foreach (MapRule mapRule in instance.MapRules)
			{
				foreach (LanguageMap languageMap in mapRule.LanguageMaps)
				{
					foreach (Match item in Regex.Matches(name, languageMap.Pattern, RegexOptions.IgnoreCase))
					{
						if (item.Length == name.Length)
						{
							foreach (LanguageRule languageRule in instance.LanguageRules)
							{
								if (languageRule.Name.Equals(languageMap.LanguageRuleName, StringComparison.OrdinalIgnoreCase))
								{
									return languageRule;
								}
							}
						}
					}
				}
			}
			return null;
		}

		private static XmlReader CreateReader(XmlReader rdr)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				ValidationType = ValidationType.None,
				DtdProcessing = DtdProcessing.Parse,
				IgnoreProcessingInstructions = true,
				IgnoreComments = true,
				IgnoreWhitespace = true,
				CheckCharacters = false
			};
			return XmlReader.Create(rdr, settings);
		}
	}
}
