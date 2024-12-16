using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.IO.TMX
{
	public class TMXReader : IEventReader, IDisposable
	{
		internal static readonly string TMX14Namespace = "http://www.lisa.org/tmx14";

		internal static readonly string PropertyNameRecognizers = "x-Recognizers";

		internal static readonly string PropertyNameTMName = "x-TMName";

		internal static readonly string TextPlaceholderTagID = "x-TextPlaceholder";

		internal static readonly string LockedContentTagID = "x-LockedContent";

		internal static readonly string PropertyNameTokenizerFlags = "x-TokenizerFlags";

		internal static readonly string PropertyNameWordCountFlags = "x-WordCountFlags";

		internal static readonly string PropertyNameTextContextMatchType = "x-TextContextMatchType";

		internal static readonly string PropertyNameUsesIdContextMatch = "x-UsesIdContextMatch";

		internal static readonly string PropertyNameIncludesContextContentMatch = "x-IncludesContextContent";

		private XmlReader _Reader;

		private TMXReaderSettings _Settings;

		private int _State;

		private int _RawTUsRead;

		private Dictionary<CultureInfo, int> _LanguagesSeen;

		private string _ErrorDescription;

		private string _FileName;

		private header _Header;

		private string _TMX14NamespaceAtom;

		private TranslationUnitFormat _Flavor = TranslationUnitFormat.Unknown;

		private StreamReader _OwnedBaseReader;

		private TMXStartOfInputEvent _TMXStartOfInputEvent;

		private Dictionary<Type, XmlSerializer> _TypeSerializers;

		private Queue<TUEvent> _Buffer;

		public TranslationUnitFormat Flavor => _Flavor;

		public Dictionary<CultureInfo, int> EncounteredLanguages => _LanguagesSeen;

		public TMXReaderSettings Settings => _Settings;

		public int RawTUsRead => _RawTUsRead;

		public TMXReader(TextReader reader)
			: this(reader, null)
		{
		}

		public TMXReader(TextReader reader, TMXReaderSettings readerSettings)
		{
			if (readerSettings == null)
			{
				readerSettings = new TMXReaderSettings();
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			_Settings = readerSettings;
			_Reader = CreateReader(reader, _Settings);
			Init();
		}

		public TMXReader(string fileName)
			: this(fileName, null)
		{
		}

		public TMXReader(string fileName, TMXReaderSettings readerSettings)
		{
			if (readerSettings == null)
			{
				readerSettings = new TMXReaderSettings();
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			_Settings = readerSettings;
			_FileName = fileName;
			_Reader = CreateReader(fileName, _Settings);
			Init();
		}

		private void Init()
		{
			_State = 0;
			_RawTUsRead = 0;
			_LanguagesSeen = new Dictionary<CultureInfo, int>();
			_TMX14NamespaceAtom = _Reader.NameTable.Get(TMX14Namespace);
			_Buffer = new Queue<TUEvent>();
			_TypeSerializers = new Dictionary<Type, XmlSerializer>();
		}

		private void SetError(string msg)
		{
			_State = -1;
			_ErrorDescription = ((_Reader == null) ? msg : (msg + " (" + _Reader?.ToString() + ")"));
		}

		private void IncrementLanguageCount(CultureInfo culture)
		{
			if (_LanguagesSeen.TryGetValue(culture, out int value))
			{
				_LanguagesSeen[culture] = value + 1;
			}
			else
			{
				_LanguagesSeen.Add(culture, 1);
			}
		}

		private bool CanHandleItTag()
		{
			if (_Flavor != TranslationUnitFormat.TradosTranslatorsWorkbench)
			{
				return _Flavor == TranslationUnitFormat.SDLX;
			}
			return true;
		}

		public Event Read()
		{
			TMXTUBuilder tMXTUBuilder = null;
			bool flag = false;
			bool flag2 = false;
			bool istagskipped = false;
			bool flag3 = _Settings.PlainText;
			while (true)
			{
				if (_State < 0)
				{
					throw new Exception(_ErrorDescription ?? "Input error");
				}
				if (_Buffer.Count > 0)
				{
					return _Buffer.Dequeue();
				}
				if (_State == 999)
				{
					break;
				}
				bool flag4 = false;
				do
				{
					if (flag)
					{
						flag = false;
					}
					else if (!_Reader.Read())
					{
						return null;
					}
					if (_Reader.NodeType == XmlNodeType.Comment || _Reader.NodeType == XmlNodeType.DocumentType || _Reader.NodeType == XmlNodeType.ProcessingInstruction || (_Reader.NodeType == XmlNodeType.Whitespace && !flag2) || _Reader.NodeType == XmlNodeType.XmlDeclaration)
					{
						flag4 = false;
					}
					else if ((_Reader.NodeType == XmlNodeType.Element || _Reader.NodeType == XmlNodeType.EndElement) && _Reader.NamespaceURI != _TMX14NamespaceAtom)
					{
						_Reader.Skip();
						flag = true;
						flag4 = false;
					}
					else if (IsTMXElement("hi"))
					{
						flag4 = false;
					}
					else if (IsTMXElement("ut"))
					{
						_Reader.Skip();
						flag = true;
						flag4 = false;
					}
					else if (IsTMXElement("it"))
					{
						if (!flag3 && !CanHandleItTag())
						{
							flag3 = true;
						}
						if (flag3)
						{
							_Reader.Skip();
							flag = true;
							flag4 = false;
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag4 = true;
					}
				}
				while (!flag4);
				bool flag5 = _Reader.NodeType == XmlNodeType.Element;
				switch (_State)
				{
				case 0:
					if (IsTMXElement("tmx"))
					{
						if (flag5)
						{
							bool flag8 = false;
							if (_Reader.HasAttributes)
							{
								while (_Reader.MoveToNextAttribute())
								{
									if (!(_Reader.Name == "version"))
									{
										continue;
									}
									flag8 = true;
									if (!_Reader.HasValue || _Reader.Value != "1.4")
									{
										if (_Settings.ValidateAgainstSchema)
										{
											throw new LanguagePlatformException(ErrorCode.TMXUnknownVersion, _Reader.Value);
										}
										_Settings.PlainText = true;
									}
								}
								_Reader.MoveToElement();
							}
							if (!flag8)
							{
								if (_Settings.ValidateAgainstSchema)
								{
									throw new LanguagePlatformException(ErrorCode.TMXMissingVersion);
								}
								_Settings.PlainText = true;
							}
							_State = 1;
						}
						else
						{
							_State = 999;
						}
					}
					else
					{
						SetError("Expected \"tmx\"");
					}
					break;
				case 1:
					if (IsTMXElement("header", XmlNodeType.Element))
					{
						header hdr = _Header = (header)DeserializeSubtree(typeof(header));
						_State = 2;
						_Flavor = DetermineFlavor(hdr);
						_TMXStartOfInputEvent = new TMXStartOfInputEvent(_FileName, hdr, _Flavor);
						if (_Flavor == TranslationUnitFormat.SDLTradosStudio2009)
						{
							ExtractHeaderSettings(_TMXStartOfInputEvent);
							ExtractHeaderFieldDefinitions(_TMXStartOfInputEvent);
						}
						return _TMXStartOfInputEvent;
					}
					SetError("Expected \"header\"");
					break;
				case 2:
					if (IsTMXElement("body", XmlNodeType.Element))
					{
						_State = (_Reader.IsEmptyElement ? 4 : 3);
					}
					else
					{
						SetError("Expected \"body\"");
					}
					break;
				case 3:
					if (IsTMXElement("tu", XmlNodeType.Element))
					{
						tMXTUBuilder = new TMXTUBuilder(_Flavor, _Header, _Settings);
						if (_Reader.HasAttributes)
						{
							while (_Reader.MoveToNextAttribute())
							{
								if (_Reader.HasValue)
								{
									tMXTUBuilder.AddAttribute(_Reader.LocalName, _Reader.Value);
								}
							}
							_Reader.MoveToElement();
						}
						_State = 5;
					}
					else if (IsTMXElement("body", XmlNodeType.EndElement))
					{
						_State = 4;
					}
					else
					{
						SetError("Expected \"/body\"");
					}
					break;
				case 4:
					if (IsTMXElement("tmx", XmlNodeType.EndElement))
					{
						_State = 999;
						return new EndOfInputEvent();
					}
					SetError("Expected \"/tmx\"");
					break;
				case 5:
					if (IsTMXElement("prop", XmlNodeType.Element))
					{
						prop p2 = (prop)DeserializeSubtree(typeof(prop));
						tMXTUBuilder.AddProperty(p2, _TMXStartOfInputEvent);
					}
					else if (IsTMXElement("note", XmlNodeType.Element))
					{
						note note2 = (note)DeserializeSubtree(typeof(note));
						tMXTUBuilder.AddNote(note2.Value);
					}
					else if (IsTMXElement("tuv", XmlNodeType.Element))
					{
						string lang = null;
						if (_Reader.HasAttributes)
						{
							while (_Reader.MoveToNextAttribute())
							{
								if (_Reader.HasValue)
								{
									if (_Reader.LocalName == "lang")
									{
										lang = _Reader.Value;
									}
									else
									{
										tMXTUBuilder.AddAttribute(_Reader.LocalName, _Reader.Value);
									}
								}
							}
							_Reader.MoveToElement();
						}
						tMXTUBuilder.OpenSegment(lang);
						IncrementLanguageCount(tMXTUBuilder.CurrentSegment.Culture);
						_State = 6;
					}
					else if (IsTMXElement("tu", XmlNodeType.EndElement))
					{
						_State = 3;
						_RawTUsRead++;
						foreach (TranslationUnit translationUnit in tMXTUBuilder.TranslationUnits)
						{
							_Buffer.Enqueue(new TUEvent(translationUnit));
						}
						tMXTUBuilder = null;
					}
					else
					{
						SetError("got \"tu\", expecting (note|prop)* tuv+");
					}
					break;
				case 6:
					if (IsTMXElement("prop", XmlNodeType.Element))
					{
						prop p = (prop)DeserializeSubtree(typeof(prop));
						tMXTUBuilder.AddProperty(p, _TMXStartOfInputEvent);
					}
					else if (IsTMXElement("note", XmlNodeType.Element))
					{
						note note = (note)DeserializeSubtree(typeof(note));
						tMXTUBuilder.AddNote(note.Value);
					}
					else if (IsTMXElement("seg", XmlNodeType.Element))
					{
						if (_Reader.IsEmptyElement)
						{
							_State = 8;
							break;
						}
						_State = 7;
						flag2 = true;
					}
					else
					{
						SetError("got \"tu/tuv\", expecting (note|prop)* seg");
					}
					break;
				case 7:
					if ((_Reader.NamespaceURI == _TMX14NamespaceAtom && (_Reader.LocalName == "bpt" || _Reader.LocalName == "ept" || _Reader.LocalName == "ph" || _Reader.LocalName == "it")) & flag5)
					{
						if (_Settings.PlainText)
						{
							if (!_Reader.IsEmptyElement)
							{
								_Reader.Skip();
								istagskipped = true;
								flag = true;
							}
							_State = 7;
							break;
						}
						Tag tag2 = new Tag();
						tag2.Type = GetTagType(_Reader.LocalName);
						bool flag6 = false;
						bool flag7 = _Settings.PlainText;
						if (tag2.Type == TagType.Undefined && _Reader.LocalName.Equals("it", StringComparison.Ordinal))
						{
							flag6 = true;
						}
						if (_Reader.HasAttributes)
						{
							while (_Reader.MoveToNextAttribute())
							{
								if (!_Reader.HasValue)
								{
									continue;
								}
								if (flag6 && _Reader.LocalName.Equals("pos", StringComparison.Ordinal))
								{
									if (_Reader.Value.Equals("begin", StringComparison.Ordinal))
									{
										tag2.Type = TagType.UnmatchedStart;
									}
									else if (_Reader.Value.Equals("end", StringComparison.Ordinal))
									{
										tag2.Type = TagType.UnmatchedEnd;
									}
								}
								else
								{
									AddTagAttribute(tag2, _Reader.LocalName, _Reader.Value);
								}
							}
							_Reader.MoveToElement();
						}
						if (tag2.Type == TagType.Undefined)
						{
							flag7 = true;
						}
						if (!flag7)
						{
							tMXTUBuilder.AddSegmentTag(tag2);
						}
						_State = (_Reader.IsEmptyElement ? 7 : 9);
					}
					else
					{
						if (IsTMXElement("hi"))
						{
							break;
						}
						if (flag5 && (_Reader.LocalName == "sub" || _Reader.LocalName == "seg"))
						{
							_Reader.Skip();
							flag = true;
						}
						else if (IsTMXElement("seg", XmlNodeType.EndElement))
						{
							flag2 = false;
							_State = 8;
						}
						else if (_Reader.NodeType == XmlNodeType.Text || _Reader.NodeType == XmlNodeType.Whitespace || _Reader.NodeType == XmlNodeType.SignificantWhitespace)
						{
							if (_Reader.HasValue)
							{
								tMXTUBuilder.AddSegmentText(_Reader.Value, _Reader.NodeType == XmlNodeType.Whitespace, istagskipped);
								istagskipped = false;
							}
							_State = 7;
						}
						else if (flag5 && _Reader.LocalName == "prop" && _Header != null && string.Equals(_Header.creationtool, "across", StringComparison.OrdinalIgnoreCase))
						{
							_Reader.Skip();
							flag = true;
						}
						else
						{
							SetError("got seg, expected segment data");
						}
					}
					break;
				case 8:
					if (IsTMXElement("tuv", XmlNodeType.EndElement))
					{
						tMXTUBuilder.CloseSegment(_TMXStartOfInputEvent);
						_State = 5;
					}
					else
					{
						SetError("got closing /seg within tuv, expecting /tuv");
					}
					break;
				case 9:
				{
					if (tMXTUBuilder.CurrentSegment == null)
					{
						throw new LanguagePlatformException(ErrorCode.TMXNoSegmentOpen);
					}
					Tag tag = tMXTUBuilder.CurrentSegment.LastElement as Tag;
					if (tag == null)
					{
						throw new LanguagePlatformException(ErrorCode.TMXCannotAddTagData);
					}
					if (_Reader.NodeType == XmlNodeType.Text || _Reader.NodeType == XmlNodeType.Whitespace)
					{
						tMXTUBuilder.AddTagContent(_Reader.Value);
					}
					else if (IsTMXElement("sub", XmlNodeType.Element))
					{
						_Reader.Skip();
						flag = true;
					}
					else if (GetTagType(_Reader.LocalName) == tag.Type && !flag5)
					{
						tMXTUBuilder.CloseTag();
						_State = 7;
					}
					else if (!flag3 && (tag.Type == TagType.UnmatchedStart || tag.Type == TagType.UnmatchedEnd) && IsTMXElement("it", XmlNodeType.EndElement))
					{
						tMXTUBuilder.CloseTag();
						_State = 7;
					}
					else
					{
						SetError("within bpt, ept, it/ut, ph");
					}
					break;
				}
				default:
					throw new LanguagePlatformException(ErrorCode.TMXInternalParserError);
				}
			}
			return null;
		}

		private void ExtractHeaderSettings(TMXStartOfInputEvent soi)
		{
			if (_Header?.Items == null)
			{
				return;
			}
			object[] items = _Header.Items;
			for (int i = 0; i < items.Length; i++)
			{
				prop prop = items[i] as prop;
				if (prop == null || string.IsNullOrEmpty(prop.type))
				{
					continue;
				}
				if (prop.type.Equals(PropertyNameTMName, StringComparison.OrdinalIgnoreCase))
				{
					if (!string.IsNullOrEmpty(prop.Value))
					{
						soi.TMName = prop.Value;
					}
				}
				else if (prop.type.Equals(PropertyNameRecognizers, StringComparison.OrdinalIgnoreCase))
				{
					if (!string.IsNullOrEmpty(prop.Value))
					{
						try
						{
							BuiltinRecognizers builtinRecognizers = (BuiltinRecognizers)Enum.Parse(typeof(BuiltinRecognizers), prop.Value, ignoreCase: true);
							soi.BuiltinRecognizers |= builtinRecognizers;
						}
						catch (Exception)
						{
						}
					}
				}
				else if (prop.type.Equals(PropertyNameTextContextMatchType, StringComparison.OrdinalIgnoreCase))
				{
					if (!string.IsNullOrEmpty(prop.Value))
					{
						try
						{
							TextContextMatchType textContextMatchType2 = soi.TextContextMatchType = (TextContextMatchType)Enum.Parse(typeof(TextContextMatchType), prop.Value, ignoreCase: true);
						}
						catch (Exception)
						{
						}
					}
				}
				else if (prop.type.Equals(PropertyNameUsesIdContextMatch, StringComparison.OrdinalIgnoreCase))
				{
					if (!string.IsNullOrEmpty(prop.Value))
					{
						try
						{
							bool flag2 = soi.UsesIdContextMatch = bool.Parse(prop.Value);
						}
						catch (Exception)
						{
						}
					}
				}
				else if (prop.type.Equals(PropertyNameIncludesContextContentMatch, StringComparison.OrdinalIgnoreCase))
				{
					if (!string.IsNullOrEmpty(prop.Value))
					{
						try
						{
							bool flag4 = soi.IncludesContextContent = bool.Parse(prop.Value);
						}
						catch (Exception)
						{
						}
					}
				}
				else if (prop.type.Equals(PropertyNameTokenizerFlags, StringComparison.OrdinalIgnoreCase))
				{
					if (!string.IsNullOrEmpty(prop.Value))
					{
						try
						{
							TokenizerFlags tokenizerFlags2 = soi.TokenizerFlags = (TokenizerFlags)Enum.Parse(typeof(TokenizerFlags), prop.Value, ignoreCase: true);
						}
						catch (Exception)
						{
						}
					}
				}
				else if (prop.type.Equals(PropertyNameWordCountFlags, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(prop.Value))
				{
					try
					{
						WordCountFlags wordCountFlags2 = soi.WordCountFlags = (WordCountFlags)Enum.Parse(typeof(WordCountFlags), prop.Value, ignoreCase: true);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private void ExtractHeaderFieldDefinitions(TMXStartOfInputEvent soi)
		{
			if (!_Settings.Context.MayAddNewFields || _Header.Items == null)
			{
				return;
			}
			object[] items = _Header.Items;
			for (int i = 0; i < items.Length; i++)
			{
				prop prop = items[i] as prop;
				if (prop == null || string.IsNullOrEmpty(prop.type) || !prop.type.StartsWith("x-", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				string text = prop.type.Substring(2);
				int num = text.IndexOf(':');
				if (num <= 0)
				{
					continue;
				}
				string fieldName = text.Substring(0, num);
				FieldValueType typeFromString = TMXTUBuilder.GetTypeFromString(text.Substring(num + 1));
				FieldIdentifier fieldIdentifier = new FieldIdentifier(typeFromString, fieldName);
				FieldIdentifier fieldIdentifier2 = TMXTUBuilder.MapFieldIdentifier(_Settings.FieldIdentifierMappings, fieldIdentifier);
				if (fieldIdentifier2 == null || string.IsNullOrEmpty(fieldIdentifier2.FieldName))
				{
					continue;
				}
				fieldName = fieldIdentifier2.FieldName;
				typeFromString = fieldIdentifier2.FieldValueType;
				if (typeFromString == FieldValueType.Unknown)
				{
					continue;
				}
				Field field = null;
				if (typeFromString == FieldValueType.SinglePicklist || typeFromString == FieldValueType.MultiplePicklist)
				{
					if (!string.IsNullOrEmpty(prop.Value))
					{
						PicklistField picklistField = new PicklistField(fieldName, typeFromString);
						picklistField.PicklistFromString(prop.Value);
						field = picklistField;
					}
				}
				else
				{
					field = new Field(fieldName, typeFromString);
				}
				if (field != null)
				{
					if (soi.Fields == null)
					{
						soi.Fields = new FieldDefinitions();
					}
					soi.Fields.Add(field);
				}
			}
		}

		private bool IsTMXElement(string name, XmlNodeType t)
		{
			if (IsTMXElement(name))
			{
				return _Reader.NodeType == t;
			}
			return false;
		}

		private bool IsTMXElement(string name)
		{
			if (_Reader.NodeType == XmlNodeType.Element || _Reader.NodeType == XmlNodeType.EndElement)
			{
				return _Reader.LocalName == name;
			}
			return false;
		}

		private TranslationUnitFormat DetermineFlavor(header hdr)
		{
			if (hdr.creationtool == null || hdr.otmf == null)
			{
				return TranslationUnitFormat.Unknown;
			}
			if (hdr.segtype == headerSegtype.sentence && hdr.otmf.Equals("TW4Win 2.0 Format", StringComparison.OrdinalIgnoreCase))
			{
				return TranslationUnitFormat.TradosTranslatorsWorkbench;
			}
			if (hdr.otmf.Equals("sdlxTM", StringComparison.OrdinalIgnoreCase))
			{
				return TranslationUnitFormat.SDLX;
			}
			if (hdr.creationtool.Equals("SDL Language Platform") && hdr.otmf.Equals("SDL TM8 Format"))
			{
				return TranslationUnitFormat.SDLTradosStudio2009;
			}
			if (hdr.creationtool.Equals("Idiom WorldServer") && hdr.otmf.Equals("Idiom TM v9.3.0"))
			{
				return TranslationUnitFormat.IdiomWorldServer;
			}
			return TranslationUnitFormat.Unknown;
		}

		private static TagType GetTagType(string relation)
		{
			if (relation != null)
			{
				if (relation == "bpt")
				{
					return TagType.Start;
				}
				if (relation == "ept")
				{
					return TagType.End;
				}
				if (relation == "ph")
				{
					return TagType.Standalone;
				}
				if (relation == "it" || relation == "ut")
				{
					return TagType.Undefined;
				}
			}
			throw new ArgumentException("Unknown tag relation " + relation);
		}

		private void AddTagAttribute(Tag t, string key, string value)
		{
			if (string.IsNullOrEmpty(value) || key == null || key == "pos")
			{
				return;
			}
			int result2;
			if (!(key == "x"))
			{
				if (!(key == "type"))
				{
					if (!(key == "assoc") && key == "i" && int.TryParse(value, out int result) && t.Anchor == 0 && (t.Type == TagType.Start || t.Type == TagType.End) && result > 0)
					{
						t.Anchor = result;
					}
				}
				else if (_Flavor == TranslationUnitFormat.SDLTradosStudio2009 || _Flavor == TranslationUnitFormat.TradosTranslatorsWorkbench || _Flavor == TranslationUnitFormat.SDLX)
				{
					t.TagID = value;
				}
			}
			else if (int.TryParse(value, out result2) && t.AlignmentAnchor == 0 && (t.Type == TagType.Start || t.Type == TagType.Standalone || t.Type == TagType.UnmatchedEnd || t.Type == TagType.UnmatchedStart || t.Type == TagType.TextPlaceholder || t.Type == TagType.LockedContent) && result2 > 0)
			{
				t.AlignmentAnchor = result2;
			}
		}

		private XmlReader CreateReader(string fileName, TMXReaderSettings settings)
		{
			if (fileName.EndsWith(".gz"))
			{
				GZipStream stream = new GZipStream(File.OpenRead(fileName), CompressionMode.Decompress);
				_OwnedBaseReader = new StreamReader(stream);
				return CreateReader(_OwnedBaseReader, _Settings);
			}
			_OwnedBaseReader = new StreamReader(fileName, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
			return CreateReader(_OwnedBaseReader, _Settings);
		}

		private static XmlReader CreateReader(TextReader reader, TMXReaderSettings readerSettings)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			if (readerSettings.ValidateAgainstSchema)
			{
				xmlReaderSettings.ValidationType = ValidationType.Schema;
			}
			else
			{
				xmlReaderSettings.ValidationType = ValidationType.None;
			}
			xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = false;
			xmlReaderSettings.CheckCharacters = false;
			xmlReaderSettings.XmlResolver = new TMXXmlResolver();
			if (readerSettings.ValidateAgainstSchema)
			{
				XmlReader xmlReader = XmlReader.Create(typeof(TMXReader).Assembly.GetManifestResourceStream(typeof(TMXReader), "tmx14.xsd") ?? throw new LanguagePlatformException(ErrorCode.EmbeddedResourceNotFound, "tmx14.xsd"));
				xmlReaderSettings.Schemas.Add(null, xmlReader);
				xmlReader.Close();
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlReaderSettings.Schemas.NameTable);
			xmlNamespaceManager.AddNamespace(string.Empty, TMX14Namespace);
			XmlParserContext inputContext = new XmlParserContext(null, xmlNamespaceManager, null, XmlSpace.None);
			if (true)
			{
				return XmlReader.Create(new IllegalCharacterFilter(reader), xmlReaderSettings, inputContext);
			}
			return XmlReader.Create(reader, xmlReaderSettings, inputContext);
		}

		private object DeserializeSubtree(Type t)
		{
			XmlReader xmlReader = _Reader.ReadSubtree();
			XmlSerializer value = null;
			if (!_TypeSerializers.TryGetValue(t, out value))
			{
				value = new XmlSerializer(t);
				_TypeSerializers.Add(t, value);
			}
			return value.Deserialize(xmlReader);
		}

		public void Dispose()
		{
			_Reader = null;
			_Header = null;
			_TypeSerializers = null;
			_Buffer = null;
			if (_OwnedBaseReader != null)
			{
				_OwnedBaseReader.Dispose();
				_OwnedBaseReader = null;
			}
		}
	}
}
