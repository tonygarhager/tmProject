using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Processing.Alignment.Core
{
	public class MappingDocumentSegmentId : IXmlSerializable
	{
		private const string MergedSegmentIdXmlName = "MergedSegmentId";

		private const string OriginalParagraphIdXmlName = "OriginalParagraphId";

		private const string OriginalSegmentIdXmlName = "OriginalSegmentId";

		public ParagraphUnitId OriginalParagraphUnitId
		{
			get;
			private set;
		}

		public SegmentId OriginalSegmentId
		{
			get;
			private set;
		}

		public SegmentId MergedSegmentId
		{
			get;
			private set;
		}

		public MappingDocumentSegmentId(XmlReader reader)
		{
			ReadXml(reader);
		}

		public MappingDocumentSegmentId(ParagraphUnitId paragraphUnitId, SegmentId segmentId, SegmentId mergedSegmentId)
		{
			OriginalParagraphUnitId = paragraphUnitId;
			OriginalSegmentId = segmentId;
			MergedSegmentId = mergedSegmentId;
		}

		public override bool Equals(object obj)
		{
			MappingDocumentSegmentId mappingDocumentSegmentId = obj as MappingDocumentSegmentId;
			if (mappingDocumentSegmentId != null && object.Equals(mappingDocumentSegmentId.OriginalParagraphUnitId, OriginalParagraphUnitId) && object.Equals(mappingDocumentSegmentId.OriginalSegmentId, OriginalSegmentId))
			{
				return object.Equals(mappingDocumentSegmentId.MergedSegmentId, MergedSegmentId);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 7 + 13 * OriginalParagraphUnitId.GetHashCode() + 751 * OriginalSegmentId.GetHashCode();
		}

		public override string ToString()
		{
			return $"{MergedSegmentId} =>[{OriginalParagraphUnitId},{OriginalSegmentId}]";
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			string id = reader.ReadElementContentAsString("MergedSegmentId", "");
			MergedSegmentId = new SegmentId(id);
			string id2 = reader.ReadElementContentAsString("OriginalParagraphId", "");
			OriginalParagraphUnitId = new ParagraphUnitId(id2);
			string id3 = reader.ReadElementContentAsString("OriginalSegmentId", "");
			OriginalSegmentId = new SegmentId(id3);
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("MergedSegmentId", "", MergedSegmentId.ToString());
			writer.WriteElementString("OriginalParagraphId", "", OriginalParagraphUnitId.ToString());
			writer.WriteElementString("OriginalSegmentId", "", OriginalSegmentId.ToString());
		}
	}
}
