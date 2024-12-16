using System;
using System.Xml;

namespace Sdl.Enterprise2.Studio.Platform.Client.Communication
{
	internal class CompressedData
	{
		private readonly WSCompressionMode _compressionMode;

		private readonly string _id;

		private string _data;

		public string Data => _data;

		public CompressedData(WSCompressionMode compressionMode)
		{
			_compressionMode = compressionMode;
		}

		public CompressedData(WSCompressionMode compressionMode, string id)
			: this(compressionMode)
		{
			_id = id;
		}

		public CompressedData(WSCompressionMode compressionMode, XmlReader elementReader)
			: this(compressionMode)
		{
			ReadXml(elementReader);
		}

		public static bool IsValidElement(XmlReader reader)
		{
			if (reader.LocalName == "CompressedData")
			{
				return reader.NamespaceURI == "http://sdl.com/soap/compression/2010";
			}
			return false;
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (!IsValidElement(reader))
			{
				throw new ArgumentException("Invalid element");
			}
			_data = _base64DecodeAndDecompress(_compressionMode, reader);
			reader.Read();
		}

		public void WriteCompressedXml(XmlWriter writer, string data)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			string text = _compressAndBase64Encode(_compressionMode, data);
			writer.WriteStartElement("sc", "CompressedData", "http://sdl.com/soap/compression/2010");
			if (!string.IsNullOrEmpty(_id))
			{
				writer.WriteAttributeString("Id", _id);
			}
			writer.WriteString(text);
			writer.WriteEndElement();
		}

		private static string _compressAndBase64Encode(WSCompressionMode mode, string data)
		{
			return Convert.ToBase64String(new Compressor(mode).CompressString(data));
		}

		private static string _base64DecodeAndDecompress(WSCompressionMode mode, XmlReader reader)
		{
			return new Compressor(mode).DecompressElement(reader);
		}
	}
}
