using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Processing.Alignment.Core
{
	public class DocumentSegmentId : IXmlSerializable
	{
		private const string ParagraphIdXmlName = "ParagraphId";

		private const string SegmentIdXmlName = "SegmentId";

		private const string OrderXmlName = "Order";

		public ParagraphUnitId ParagraphUnitId
		{
			get;
			private set;
		}

		public SegmentId SegmentId
		{
			get;
			private set;
		}

		public int Order
		{
			get;
			private set;
		}

		public DocumentSegmentId(XmlReader reader)
		{
			ReadXml(reader);
		}

		public DocumentSegmentId(ParagraphUnitId paragraphUnitId, SegmentId segmentId, int order)
		{
			ParagraphUnitId = paragraphUnitId;
			SegmentId = segmentId;
			Order = order;
		}

		public override bool Equals(object obj)
		{
			DocumentSegmentId documentSegmentId = obj as DocumentSegmentId;
			if (documentSegmentId != null && object.Equals(documentSegmentId.ParagraphUnitId, ParagraphUnitId))
			{
				return object.Equals(documentSegmentId.SegmentId, SegmentId);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 7 + 13 * ParagraphUnitId.GetHashCode() + 751 * SegmentId.GetHashCode();
		}

		public override string ToString()
		{
			return "(" + ParagraphUnitId + ", " + SegmentId + ")";
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			string id = reader.ReadElementContentAsString("ParagraphId", "");
			ParagraphUnitId = new ParagraphUnitId(id);
			string id2 = reader.ReadElementContentAsString("SegmentId", "");
			SegmentId = new SegmentId(id2);
			Order = reader.ReadElementContentAsInt("Order", "");
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("ParagraphId", "", ParagraphUnitId.ToString());
			writer.WriteElementString("SegmentId", "", SegmentId.ToString());
			writer.WriteElementString("Order", "", Order.ToString(CultureInfo.InvariantCulture));
		}
	}
}
