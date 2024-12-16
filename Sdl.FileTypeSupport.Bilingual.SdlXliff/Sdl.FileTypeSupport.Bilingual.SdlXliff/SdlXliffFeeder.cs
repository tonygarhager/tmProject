using Sdl.FileTypeSupport.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public sealed class SdlXliffFeeder : IDisposable
	{
		private readonly List<ISdlXliffStreamContentHandler> _contentHandlers = new List<ISdlXliffStreamContentHandler>();

		private WhitespacePreservingXmlTextReader _reader;

		private readonly XmlDocument _xmlDocument = new XmlDocument();

		private bool _isDisposed;

		public long EstimatedCharsRead
		{
			get;
			private set;
		}

		public SdlXliffFeeder(TextReader reader)
		{
			_reader = new WhitespacePreservingXmlTextReader(reader);
			EstimatedCharsRead = 0L;
		}

		public SdlXliffFeeder(Stream stream)
		{
			_reader = new WhitespacePreservingXmlTextReader(stream);
			EstimatedCharsRead = 0L;
		}

		public SdlXliffFeeder(string filePath)
		{
			_reader = new WhitespacePreservingXmlTextReader(filePath);
			EstimatedCharsRead = 0L;
		}

		public void Close()
		{
			_reader?.Close();
		}

		public bool RegisterSubscriber(ISdlXliffStreamContentHandler subscriber)
		{
			if (_contentHandlers.Contains(subscriber))
			{
				return false;
			}
			_contentHandlers.Add(subscriber);
			return true;
		}

		public bool UnregisterSubscriber(ISdlXliffStreamContentHandler subscriber)
		{
			if (!_contentHandlers.Contains(subscriber))
			{
				return false;
			}
			_contentHandlers.Remove(subscriber);
			return true;
		}

		public bool ContinueScanning()
		{
			if (_reader.ReadState == ReadState.Initial)
			{
				_reader.Read();
			}
			while (!_reader.EOF)
			{
				switch (_reader.LocalName)
				{
				case "doc-info":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnDocInfo(GetXmlElement(_reader));
					});
					return true;
				case "file":
					return HandleFile();
				case "header":
					HandleFileHeader();
					return true;
				case "file-info":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnFileInfo(GetXmlElement(_reader));
					});
					return true;
				case "cxt-def":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnContextDefinition(GetXmlElement(_reader));
					});
					return true;
				case "node-def":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnNodeDefinition(GetXmlElement(_reader));
					});
					return true;
				case "tag":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnTagDefinition(GetXmlElement(_reader));
					});
					return true;
				case "fmt-def":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnFormattingDefinition(GetXmlElement(_reader));
					});
					return true;
				case "filetype-info":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnFileTypeInfo(GetXmlElement(_reader));
					});
					return true;
				case "cmt":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnCommentReference(GetXmlElement(_reader));
					});
					return true;
				case "ref-files":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnDependencyFiles(GetXmlElement(_reader));
					});
					return true;
				case "internal-file":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnInternalFile(GetXmlElement(_reader));
					});
					return true;
				case "external-file":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnExternalFile(GetXmlElement(_reader));
					});
					return true;
				case "group":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnGroup(GetXmlElement(_reader));
					});
					return true;
				case "trans-unit":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnTranslationUnit(GetXmlElement(_reader));
					});
					return true;
				case "bin-unit":
					_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
					{
						handler.OnBinaryUnit(GetXmlElement(_reader));
					});
					return true;
				}
				_reader.Read();
			}
			return false;
		}

		private bool HandleFile()
		{
			if (_reader.IsStartElement())
			{
				if (!_contentHandlers.Any((ISdlXliffStreamContentHandler handler) => handler.OnStartFile(GetXmlAttributes(_reader))))
				{
					if (_contentHandlers.Count > 1)
					{
						throw new FileTypeSupportException("Cannot skip element: more than one subscriber registered!");
					}
					_reader.Skip();
					return true;
				}
			}
			else
			{
				if (_reader.NodeType != XmlNodeType.EndElement)
				{
					throw new FileTypeSupportException("Bad type of File node!");
				}
				_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
				{
					handler.OnEndFile();
				});
				_reader.Read();
			}
			return true;
		}

		private void HandleFileHeader()
		{
			if (_reader.IsStartElement())
			{
				_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
				{
					handler.OnStartFileHeader();
				});
			}
			else
			{
				if (_reader.NodeType != XmlNodeType.EndElement)
				{
					throw new FileTypeSupportException("Bad type of File Header node!");
				}
				_contentHandlers.ForEach(delegate(ISdlXliffStreamContentHandler handler)
				{
					handler.OnEndFileHeader();
				});
			}
			_reader.Read();
		}

		public void Dispose()
		{
			if (!_isDisposed && _reader != null)
			{
				_isDisposed = true;
				((IDisposable)_reader).Dispose();
				_reader = null;
			}
		}

		private XmlElement GetXmlElement(XmlReader xmlReader)
		{
			XmlNode xmlNode = _xmlDocument.ReadNode(xmlReader);
			if (xmlNode == null)
			{
				return null;
			}
			EstimatedCharsRead += xmlNode.OuterXml.Length;
			return (xmlNode as XmlElement) ?? throw new FileTypeSupportException("Cannot cast to XmlElement from XmlNode: " + xmlNode.OuterXml);
		}

		private List<XmlAttribute> GetXmlAttributes(XmlReader xmlReader)
		{
			List<XmlAttribute> list = new List<XmlAttribute>();
			for (int i = 0; i < xmlReader.AttributeCount; i++)
			{
				xmlReader.MoveToAttribute(i);
				XmlAttribute xmlAttribute = _xmlDocument.CreateAttribute(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI);
				xmlAttribute.Value = xmlReader.Value;
				list.Add(xmlAttribute);
			}
			xmlReader.MoveToElement();
			xmlReader.Read();
			return list;
		}
	}
}
