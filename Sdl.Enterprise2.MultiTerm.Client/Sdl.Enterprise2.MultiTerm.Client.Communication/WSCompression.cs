#define TRACE
using Sdl.Enterprise2.MultiTerm.Client.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	public static class WSCompression
	{
		public static class AttributeNames
		{
			public const string Algorithm = "Algorithm";

			public const string Uri = "URI";

			public const string Id = "Id";
		}

		public static class ElementNames
		{
			public const string Compression = "Compression";

			public const string CompressionMethod = "CompressionMethod";

			public const string CompressedData = "CompressedData";
		}

		public static class Algorithm
		{
			public const string GZip = "http://sdl.com/soap/compression/2010/gzip";

			public const string Deflate = "http://sdl.com/soap/compression/2010/deflate";
		}

		private const int _COMPRESSION_THRESHOLD = 2048;

		public const string Prefix = "sc";

		public const string NamespaceUri = "http://sdl.com/soap/compression/2010";

		public static Message Compress(WSCompressionMode mode, Message message, string messageDescription, out bool compressed, out int orginalBodySize, out int compressedBodySize)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			string text = null;
			string text2 = null;
			string id = null;
			using (XmlReader xmlReader = message.GetReaderAtBodyContents())
			{
				try
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Start, 0, "Serialize " + messageDescription);
					text = xmlReader.ReadOuterXml();
				}
				finally
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Stop, 0, "Serialize " + messageDescription);
				}
			}
			orginalBodySize = text.Length;
			if (text.Length > 2048)
			{
				compressed = true;
				id = string.Format(CultureInfo.InvariantCulture, "sc-{0}", Guid.NewGuid().ToString());
				StringBuilder stringBuilder = new StringBuilder();
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				using (XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
				{
					CompressedData compressedData = new CompressedData(mode, id);
					compressedData.WriteCompressedXml(writer, text);
				}
				text2 = stringBuilder.ToString();
			}
			else
			{
				compressed = false;
				text2 = text;
			}
			compressedBodySize = text2.Length;
			Message message2 = null;
			message2 = Message.CreateMessage(message.Version, null, CreateReader(text2));
			message2.Headers.CopyHeadersFrom(message);
			foreach (string key in message.Properties.Keys)
			{
				message2.Properties.Add(key, message.Properties[key]);
			}
			if (text.Length > 2048)
			{
				message2.Headers.Add(new CompressionMessageHeader(mode, id));
			}
			return message2;
		}

		public static Message Decompress(Message compressedMessage)
		{
			try
			{
				if (compressedMessage == null)
				{
					throw new ArgumentNullException("compressedMessage");
				}
				int num = compressedMessage.Headers.FindHeader("Compression", "http://sdl.com/soap/compression/2010");
				if (num == -1)
				{
					return compressedMessage;
				}
				CompressedData compressedData = null;
				CompressionMessageHeader compressionMessageHeader = new CompressionMessageHeader(compressedMessage.Headers.GetReaderAtHeader(num));
				using (XmlReader xmlReader = compressedMessage.GetReaderAtBodyContents())
				{
					while (xmlReader.MoveToContent() == XmlNodeType.Element && (xmlReader.NodeType != XmlNodeType.Element || !CompressedData.IsValidElement(xmlReader)))
					{
						xmlReader.Read();
					}
					compressedData = new CompressedData(compressionMessageHeader.CompressionMode, xmlReader);
				}
				compressedMessage.Headers.RemoveAt(num);
				Message message = null;
				message = Message.CreateMessage(compressedMessage.Version, null, CreateReader(compressedData.Data));
				message.Headers.CopyHeadersFrom(compressedMessage);
				foreach (string key in compressedMessage.Properties.Keys)
				{
					message.Properties.Add(key, compressedMessage.Properties[key]);
				}
				return message;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private static XmlReader CreateReader(string messageBody)
		{
			return new XmlTextReader(new StringReader(messageBody));
		}
	}
}
