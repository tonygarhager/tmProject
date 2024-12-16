using Sdl.LanguagePlatform.Core;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class XmlSegmentSerializer
	{
		private XmlSerializer _SegmentSerializer;

		internal Sdl.LanguagePlatform.Core.Segment DeserializeSegment(string databaseString)
		{
			if (_SegmentSerializer == null)
			{
				_SegmentSerializer = new XmlSerializer(typeof(Sdl.LanguagePlatform.Core.Segment));
			}
			using (StringReader input = new StringReader(databaseString))
			{
				XmlReaderSettings settings = new XmlReaderSettings
				{
					CheckCharacters = false
				};
				XmlReader xmlReader = XmlReader.Create(input, settings);
				Sdl.LanguagePlatform.Core.Segment segment = _SegmentSerializer.Deserialize(xmlReader) as Sdl.LanguagePlatform.Core.Segment;
				if (segment != null && !segment.IsValid())
				{
					throw new LanguagePlatformException(ErrorCode.DAInvalidSegmentAfterDeserialization);
				}
				return segment;
			}
		}

		public string SerializeSegment(Sdl.LanguagePlatform.Core.Segment segment)
		{
			if (segment == null)
			{
				throw new ArgumentNullException("segment");
			}
			if (!segment.IsValid())
			{
				throw new LanguagePlatformException(ErrorCode.InvalidSegment);
			}
			if (_SegmentSerializer == null)
			{
				_SegmentSerializer = new XmlSerializer(typeof(Sdl.LanguagePlatform.Core.Segment));
			}
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = false,
				NewLineOnAttributes = false,
				OmitXmlDeclaration = true,
				CheckCharacters = false,
				NewLineHandling = NewLineHandling.Entitize
			};
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings);
			_SegmentSerializer.Serialize(xmlWriter, segment);
			xmlWriter.Flush();
			return stringBuilder.ToString();
		}
	}
}
