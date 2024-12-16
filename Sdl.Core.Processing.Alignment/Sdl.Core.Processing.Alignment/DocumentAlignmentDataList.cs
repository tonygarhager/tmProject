using Sdl.Core.Processing.Alignment.Common;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Processing.Alignment
{
	public class DocumentAlignmentDataList : List<DocumentAlignmentData>, IXmlSerializable
	{
		private const string AlignmentXmlName = "Alignment";

		private const string AlignmentTypeXmlAttributeName = "AlignmentType";

		public DocumentAlignmentDataList()
		{
		}

		public DocumentAlignmentDataList(XmlReader reader)
		{
			ReadXml(reader);
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.NodeType == XmlNodeType.Element)
			{
				if (object.Equals(reader.Name, "Alignment"))
				{
					Add(new DocumentAlignmentData(reader));
				}
			}
			if (!reader.EOF)
			{
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DocumentAlignmentData current = enumerator.Current;
					writer.WriteStartElement("Alignment");
					writer.WriteAttributeString("AlignmentType", current.AlignmentType.ToString());
					current.WriteXml(writer);
					writer.WriteEndElement();
				}
			}
		}

		public AlignmentStatistics CalculateStatistics()
		{
			int num = 0;
			int num2 = 0;
			int count = base.Count;
			IDictionary<AlignmentQuality, int> dictionary = new Dictionary<AlignmentQuality, int>();
			IDictionary<AlignmentType, int> dictionary2 = new Dictionary<AlignmentType, int>();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DocumentAlignmentData current = enumerator.Current;
					int count2 = current.LeftSegmentIds.Count;
					int count3 = current.RightSegmentIds.Count;
					num += count2;
					num2 += count3;
					AlignmentQuality quality = current.Quality;
					if (!dictionary.ContainsKey(quality))
					{
						dictionary[quality] = 0;
					}
					dictionary[quality]++;
					AlignmentType alignmentType = GetAlignmentType(count2, count3);
					if (!dictionary2.ContainsKey(alignmentType))
					{
						dictionary2[alignmentType] = 0;
					}
					dictionary2[alignmentType]++;
				}
			}
			return new AlignmentStatistics(num, num2, count, dictionary, dictionary2);
		}

		private AlignmentType GetAlignmentType(int leftSegmentsCount, int rightSegmentsCount)
		{
			AlignmentType alignmentType = AlignmentHelper.GetAlignmentType(leftSegmentsCount, rightSegmentsCount);
			if (alignmentType == AlignmentType.Invalid)
			{
				throw new Exception("Invalid alignment type; " + leftSegmentsCount + ", " + rightSegmentsCount);
			}
			return alignmentType;
		}
	}
}
