using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Processing.Alignment
{
	public class DocumentAlignmentData : IXmlSerializable, ICloneable
	{
		private const string LeftSegmentIdXmlName = "LeftSegmentId";

		private const string RightSegmentIdXmlName = "RightSegmentId";

		private const string QualityXmlName = "Quality";

		private const string CostXmlName = "Cost";

		private const string AlignmentTypeXmlAttributeName = "AlignmentType";

		private const string ConfirmedXmlName = "Confirmed";

		private readonly IList<DocumentSegmentId> _leftSegmentIds = new List<DocumentSegmentId>();

		private readonly IList<DocumentSegmentId> _rightSegmentIds = new List<DocumentSegmentId>();

		private AlignmentQuality _quality = AlignmentQuality.Average;

		private double _alignmentCost;

		private Dictionary<string, AlignmentType> _mapstringToEnum;

		public IList<DocumentSegmentId> LeftSegmentIds => _leftSegmentIds;

		public IList<DocumentSegmentId> RightSegmentIds => _rightSegmentIds;

		public AlignmentQuality Quality
		{
			get
			{
				return _quality;
			}
			set
			{
				_quality = value;
			}
		}

		public bool Confirmed
		{
			get;
			set;
		}

		public AlignmentType AlignmentType
		{
			get;
			set;
		}

		public double AlignmentCost
		{
			get
			{
				return _alignmentCost;
			}
			set
			{
				_alignmentCost = value;
			}
		}

		private Dictionary<string, AlignmentType> mapstringToEnum
		{
			get
			{
				Dictionary<string, AlignmentType> dictionary = _mapstringToEnum;
				if (dictionary == null)
				{
					Dictionary<string, AlignmentType> obj = new Dictionary<string, AlignmentType>
					{
						{
							"Invalid",
							AlignmentType.Invalid
						},
						{
							"Alignment01",
							AlignmentType.Alignment01
						},
						{
							"Alignment10",
							AlignmentType.Alignment10
						},
						{
							"Alignment11",
							AlignmentType.Alignment11
						},
						{
							"Alignment12",
							AlignmentType.Alignment12
						},
						{
							"Alignment13",
							AlignmentType.Alignment13
						},
						{
							"Alignment21",
							AlignmentType.Alignment21
						},
						{
							"Alignment22",
							AlignmentType.Alignment22
						},
						{
							"Alignment22C",
							AlignmentType.Alignment22C
						},
						{
							"Alignment31",
							AlignmentType.Alignment31
						},
						{
							"AlignmentN1",
							AlignmentType.AlignmentN1
						},
						{
							"Alignment1N",
							AlignmentType.Alignment1N
						}
					};
					Dictionary<string, AlignmentType> dictionary2 = obj;
					_mapstringToEnum = obj;
					dictionary = dictionary2;
				}
				return dictionary;
			}
		}

		public DocumentAlignmentData()
		{
			Confirmed = false;
		}

		public DocumentAlignmentData(XmlReader reader)
		{
			ReadXml(reader);
		}

		public DocumentAlignmentData(IList<DocumentSegmentId> leftSegmentIds, IList<DocumentSegmentId> rightSegmentIds, AlignmentQuality quality, bool confirmed, double alignmentCost = 0.0)
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
			_quality = quality;
			_alignmentCost = alignmentCost;
			Confirmed = confirmed;
		}

		public override bool Equals(object obj)
		{
			DocumentAlignmentData documentAlignmentData = obj as DocumentAlignmentData;
			if (documentAlignmentData != null && EqualsSegmentIds(documentAlignmentData.LeftSegmentIds, LeftSegmentIds))
			{
				return EqualsSegmentIds(documentAlignmentData.RightSegmentIds, RightSegmentIds);
			}
			return false;
		}

		private bool EqualsSegmentIds(IList<DocumentSegmentId> segmentIds0, IList<DocumentSegmentId> segmentIds1)
		{
			if (segmentIds0.Count != segmentIds1.Count)
			{
				return false;
			}
			for (int i = 0; i < segmentIds0.Count; i++)
			{
				if (!object.Equals(segmentIds0[i], segmentIds1[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 31;
			for (int i = 0; i < LeftSegmentIds.Count; i++)
			{
				num += (i + 47) * LeftSegmentIds[i].GetHashCode();
			}
			for (int j = 0; j < RightSegmentIds.Count; j++)
			{
				num += (j + 171) * RightSegmentIds[j].GetHashCode();
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
				foreach (DocumentSegmentId leftSegmentId in _leftSegmentIds)
				{
					stringBuilder.Append(leftSegmentId + " ");
				}
			}
			else
			{
				stringBuilder.Append("   ");
			}
			stringBuilder.Append("| ");
			if (_rightSegmentIds.Count > 0)
			{
				foreach (DocumentSegmentId rightSegmentId in _rightSegmentIds)
				{
					stringBuilder.Append(rightSegmentId + " ");
				}
			}
			else
			{
				stringBuilder.Append("   ");
			}
			stringBuilder.AppendFormat(": {0} ({1}confirmed)", _quality, Confirmed ? string.Empty : "not ");
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
			if (reader.HasAttributes)
			{
				string attribute = reader.GetAttribute("AlignmentType");
				AlignmentType = ((attribute != null) ? mapstringToEnum[attribute] : AlignmentType.Invalid);
			}
			reader.ReadStartElement();
			while (reader.NodeType == XmlNodeType.Element)
			{
				switch (reader.Name)
				{
				case "LeftSegmentId":
					_leftSegmentIds.Add(new DocumentSegmentId(reader));
					break;
				case "RightSegmentId":
					_rightSegmentIds.Add(new DocumentSegmentId(reader));
					break;
				case "Quality":
				{
					string value2 = reader.ReadElementContentAsString("Quality", "");
					_quality = (AlignmentQuality)Enum.Parse(typeof(AlignmentQuality), value2);
					break;
				}
				case "Cost":
				{
					double num = _alignmentCost = reader.ReadElementContentAsDouble("Cost", "");
					break;
				}
				case "Confirmed":
				{
					string value = reader.ReadElementContentAsString("Confirmed", "");
					Confirmed = bool.Parse(value);
					break;
				}
				default:
					reader.Read();
					break;
				}
			}
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			foreach (DocumentSegmentId leftSegmentId in _leftSegmentIds)
			{
				writer.WriteStartElement("LeftSegmentId");
				leftSegmentId.WriteXml(writer);
				writer.WriteEndElement();
			}
			foreach (DocumentSegmentId rightSegmentId in _rightSegmentIds)
			{
				writer.WriteStartElement("RightSegmentId");
				rightSegmentId.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteElementString("Quality", "", _quality.ToString());
			if (Confirmed)
			{
				writer.WriteElementString("Confirmed", "", Confirmed.ToString().ToLower());
			}
			writer.WriteElementString("Cost", "", _alignmentCost.ToString(CultureInfo.InvariantCulture));
		}

		public object Clone()
		{
			return new DocumentAlignmentData(new List<DocumentSegmentId>(_leftSegmentIds), new List<DocumentSegmentId>(_rightSegmentIds), _quality, Confirmed, _alignmentCost);
		}
	}
}
