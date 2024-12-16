using Sdl.Core.Processing.Alignment.Core;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	public class AlignmentToOriginalMapper : IXmlSerializable, ICloneable
	{
		private const string LeftMappingsXmlName = "LeftMappings";

		private const string RightMappingsIdXmlName = "RightMappings";

		private const string MappingItemIdXmlName = "MappingItem";

		private readonly IDictionary<int, MappingDocumentSegmentId> _leftSegmentIds = new Dictionary<int, MappingDocumentSegmentId>();

		private readonly IDictionary<int, MappingDocumentSegmentId> _rightSegmentIds = new Dictionary<int, MappingDocumentSegmentId>();

		public Dictionary<int, List<int>> LeftToRightConnections
		{
			get;
			set;
		}

		public Dictionary<int, List<int>> RightToLeftConnections
		{
			get;
			set;
		}

		public Dictionary<int, ParagraphUnitIdPair> ParagraphAssociations
		{
			get;
			set;
		}

		public IDictionary<int, MappingDocumentSegmentId> LeftSegmentIds => _leftSegmentIds;

		public IDictionary<int, MappingDocumentSegmentId> RightSegmentIds => _rightSegmentIds;

		public AlignmentToOriginalMapper()
		{
		}

		public AlignmentToOriginalMapper(XmlReader reader)
		{
			ReadXml(reader);
		}

		public AlignmentToOriginalMapper(IDictionary<int, MappingDocumentSegmentId> leftSegmentIds, IDictionary<int, MappingDocumentSegmentId> rightSegmentIds)
		{
			if (leftSegmentIds == null)
			{
				throw new ArgumentNullException("leftSegmentIds");
			}
			if (rightSegmentIds == null)
			{
				throw new ArgumentNullException("rightSegmentIds");
			}
			_leftSegmentIds = leftSegmentIds;
			_rightSegmentIds = rightSegmentIds;
		}

		public override bool Equals(object obj)
		{
			AlignmentToOriginalMapper alignmentToOriginalMapper = obj as AlignmentToOriginalMapper;
			if (alignmentToOriginalMapper != null && EqualsSegmentIds(alignmentToOriginalMapper.LeftSegmentIds, LeftSegmentIds))
			{
				return EqualsSegmentIds(alignmentToOriginalMapper.RightSegmentIds, RightSegmentIds);
			}
			return false;
		}

		private bool EqualsSegmentIds(IDictionary<int, MappingDocumentSegmentId> segmentIds0, IDictionary<int, MappingDocumentSegmentId> segmentIds1)
		{
			if (segmentIds0.Count != segmentIds1.Count)
			{
				return false;
			}
			foreach (int key in segmentIds0.Keys)
			{
				if (!segmentIds1.ContainsKey(key) || !segmentIds0[key].Equals(segmentIds1[key]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 31;
			int result;
			foreach (MappingDocumentSegmentId value in LeftSegmentIds.Values)
			{
				int.TryParse(value.OriginalSegmentId.Id, out result);
				num += (result + 47) * value.GetHashCode();
			}
			foreach (MappingDocumentSegmentId value2 in RightSegmentIds.Values)
			{
				int.TryParse(value2.OriginalSegmentId.Id, out result);
				num += (result + 47) * value2.GetHashCode();
			}
			return num;
		}

		public void Write()
		{
			Console.Out.WriteLine(ToString());
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (_leftSegmentIds.Count > 0)
			{
				foreach (MappingDocumentSegmentId value in _leftSegmentIds.Values)
				{
					stringBuilder.Append(value + " ");
				}
			}
			else
			{
				stringBuilder.Append("   ");
			}
			stringBuilder.Append("| ");
			if (_rightSegmentIds.Count > 0)
			{
				foreach (MappingDocumentSegmentId value2 in _rightSegmentIds.Values)
				{
					stringBuilder.Append(value2 + " ");
				}
			}
			else
			{
				stringBuilder.Append("   ");
			}
			return stringBuilder.ToString();
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			_leftSegmentIds.Clear();
			_rightSegmentIds.Clear();
			reader.ReadStartElement();
			while (reader.NodeType == XmlNodeType.Element)
			{
				switch (reader.Name)
				{
				case "LeftMappings":
					reader.ReadStartElement();
					while (reader.NodeType == XmlNodeType.Element && reader.Name == "MappingItem")
					{
						MappingDocumentSegmentId mappingDocumentSegmentId = new MappingDocumentSegmentId(reader);
						LeftSegmentIds.Add(int.Parse(mappingDocumentSegmentId.MergedSegmentId.Id), mappingDocumentSegmentId);
					}
					reader.ReadEndElement();
					break;
				case "RightMappings":
					reader.ReadStartElement();
					while (reader.NodeType == XmlNodeType.Element && reader.Name == "MappingItem")
					{
						MappingDocumentSegmentId mappingDocumentSegmentId = new MappingDocumentSegmentId(reader);
						RightSegmentIds.Add(int.Parse(mappingDocumentSegmentId.MergedSegmentId.Id), mappingDocumentSegmentId);
					}
					reader.ReadEndElement();
					break;
				}
			}
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("LeftMappings");
			foreach (MappingDocumentSegmentId value in _leftSegmentIds.Values)
			{
				writer.WriteStartElement("MappingItem");
				value.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("RightMappings");
			foreach (MappingDocumentSegmentId value2 in _rightSegmentIds.Values)
			{
				writer.WriteStartElement("MappingItem");
				value2.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		public object Clone()
		{
			return new AlignmentToOriginalMapper(new Dictionary<int, MappingDocumentSegmentId>(_leftSegmentIds), new Dictionary<int, MappingDocumentSegmentId>(_rightSegmentIds));
		}

		private static void AddConnection(Dictionary<int, List<int>> connectionList, int key, int value)
		{
			if (!connectionList.ContainsKey(key))
			{
				connectionList.Add(key, new List<int>
				{
					value
				});
			}
			else
			{
				connectionList[key].Add(value);
			}
		}

		private void UpdateConnections(Dictionary<int, List<int>> leftToRight, Dictionary<int, List<int>> rightToLeft, List<int> leftSegmentIDs, List<int> rightSegmentIDs)
		{
			foreach (int leftSegmentID in leftSegmentIDs)
			{
				foreach (int rightSegmentID in rightSegmentIDs)
				{
					AddConnection(leftToRight, leftSegmentID, rightSegmentID);
					AddConnection(rightToLeft, rightSegmentID, leftSegmentID);
				}
			}
		}

		public void SetParagraphAssociations()
		{
			ParagraphAssociations = new Dictionary<int, ParagraphUnitIdPair>();
			int num = 0;
			int i = 1;
			ParagraphUnitId paragraphUnitId = new ParagraphUnitId("dummyId");
			while (i <= LeftSegmentIds.Count)
			{
				num++;
				ParagraphAssociations.Add(num, new ParagraphUnitIdPair(LeftSegmentIds[i].OriginalParagraphUnitId, paragraphUnitId));
				for (ParagraphUnitId originalParagraphUnitId = LeftSegmentIds[i].OriginalParagraphUnitId; i <= LeftSegmentIds.Count && LeftSegmentIds[i].OriginalParagraphUnitId == originalParagraphUnitId; i++)
				{
				}
			}
			num = 0;
			i = 1;
			while (i <= RightSegmentIds.Count)
			{
				num++;
				if (ParagraphAssociations.ContainsKey(num))
				{
					ParagraphAssociations[num].Right = RightSegmentIds[i].OriginalParagraphUnitId;
				}
				else
				{
					ParagraphAssociations.Add(num, new ParagraphUnitIdPair(paragraphUnitId, RightSegmentIds[i].OriginalParagraphUnitId));
				}
				for (ParagraphUnitId originalParagraphUnitId2 = RightSegmentIds[i].OriginalParagraphUnitId; i <= RightSegmentIds.Count && RightSegmentIds[i].OriginalParagraphUnitId == originalParagraphUnitId2; i++)
				{
				}
			}
		}

		public void SetConnections()
		{
			SetParagraphAssociations();
			int num = 1;
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			LeftToRightConnections = new Dictionary<int, List<int>>();
			RightToLeftConnections = new Dictionary<int, List<int>>();
			int i = 1;
			int j = 1;
			while (num <= ParagraphAssociations.Count)
			{
				ParagraphUnitIdPair paragraphUnitIdPair;
				for (paragraphUnitIdPair = ParagraphAssociations[num]; i <= LeftSegmentIds.Count && LeftSegmentIds[i].OriginalParagraphUnitId == paragraphUnitIdPair.Left; i++)
				{
					list.Add(i);
				}
				for (; j <= RightSegmentIds.Count && RightSegmentIds[j].OriginalParagraphUnitId == paragraphUnitIdPair.Right; j++)
				{
					list2.Add(j);
				}
				UpdateConnections(LeftToRightConnections, RightToLeftConnections, list, list2);
				num++;
				list = new List<int>();
				list2 = new List<int>();
			}
		}
	}
}
