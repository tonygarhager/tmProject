using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Sdl.Enterprise2.Studio.Platform.Client.Communication
{
	internal class CompressionMessageHeader : MessageHeader
	{
		private readonly WSCompressionMode _compressionMode;

		private readonly string _id;

		public override string Name => "Compression";

		public override string Namespace => "http://sdl.com/soap/compression/2010";

		public override bool MustUnderstand => true;

		public WSCompressionMode CompressionMode => _compressionMode;

		public CompressionMessageHeader(WSCompressionMode compressionMode)
		{
			_compressionMode = compressionMode;
		}

		public CompressionMessageHeader(WSCompressionMode compressionMode, string id)
			: this(compressionMode)
		{
			_id = id;
		}

		public CompressionMessageHeader(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			while (!reader.IsStartElement("CompressionMethod", "http://sdl.com/soap/compression/2010"))
			{
				if (!reader.Read())
				{
					return;
				}
			}
			_compressionMode = GetCompressionMode(reader.GetAttribute("Algorithm"));
			_id = reader.GetAttribute("Id");
		}

		protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
		{
			writer.WriteStartElement("sc", "CompressionMethod", "http://sdl.com/soap/compression/2010");
			if (!string.IsNullOrEmpty(_id))
			{
				writer.WriteAttributeString("Id", _id);
			}
			writer.WriteAttributeString("Algorithm", GetAlgorithm(_compressionMode));
			writer.WriteEndElement();
		}

		private static string GetAlgorithm(WSCompressionMode compressionMode)
		{
			switch (compressionMode)
			{
			case WSCompressionMode.GZip:
				return "http://sdl.com/soap/compression/2010/gzip";
			case WSCompressionMode.Deflate:
				return "http://sdl.com/soap/compression/2010/deflate";
			default:
				throw new ArgumentOutOfRangeException("compressionMode");
			}
		}

		private static WSCompressionMode GetCompressionMode(string algorithm)
		{
			if (!(algorithm == "http://sdl.com/soap/compression/2010/gzip"))
			{
				if (algorithm == "http://sdl.com/soap/compression/2010/deflate")
				{
					return WSCompressionMode.Deflate;
				}
				throw new ArgumentOutOfRangeException("algorithm");
			}
			return WSCompressionMode.GZip;
		}
	}
}
