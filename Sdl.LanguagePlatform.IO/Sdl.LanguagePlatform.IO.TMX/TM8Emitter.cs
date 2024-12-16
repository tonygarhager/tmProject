using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class TM8Emitter : AbstractEmitter
	{
		internal static readonly string OriginFieldName = "x-Origin";

		internal static readonly string ContextFieldName = "x-Context";

		internal static readonly string ContextContentFieldName = "x-ContextContent";

		internal static readonly string FormatFieldName = "x-OriginalFormat";

		internal static readonly string ConfirmationLevelFieldName = "x-ConfirmationLevel";

		internal static readonly string LastUsedByFieldName = "x-LastUsedBy";

		internal static readonly string ContextSeparatorChar = "|";

		internal static readonly string ContextSeparator = " " + ContextSeparatorChar + " ";

		internal static readonly string IdContextFieldName = "x-IdContext";

		public TM8Emitter(XmlWriter output, TMXWriterSettings writerSettings)
			: base(output, writerSettings)
		{
			_TargetLocaleSystem = null;
		}

		public override void EmitHeaderFormat()
		{
			_Output.WriteAttributeString("o-tmf", "SDL TM8 Format");
			_Output.WriteAttributeString("datatype", "xml");
		}

		public override void EmitExtendedHeader(StartOfInputEvent soi)
		{
			if (soi.Properties != null)
			{
				foreach (KeyValuePair<string, string> property in soi.Properties)
				{
					if (!string.IsNullOrEmpty(property.Key) && !string.IsNullOrEmpty(property.Value))
					{
						_Output.WriteStartElement("prop");
						_Output.WriteAttributeString("type", $"x-user-defined:{property.Key}");
						_Output.WriteString(property.Value);
						_Output.WriteEndElement();
					}
				}
			}
			TMXStartOfInputEvent tMXStartOfInputEvent = soi as TMXStartOfInputEvent;
			if (tMXStartOfInputEvent != null)
			{
				if (tMXStartOfInputEvent.Fields != null)
				{
					foreach (Field field in tMXStartOfInputEvent.Fields)
					{
						string value = null;
						if (field.ValueType == FieldValueType.MultiplePicklist || field.ValueType == FieldValueType.SinglePicklist)
						{
							value = ((field as PicklistField) ?? throw new LanguagePlatformException(ErrorCode.TMIncompatibleFieldTypes)).PicklistToString();
						}
						EmitFieldValue(field.Name, field.ValueType, value);
					}
				}
				if (tMXStartOfInputEvent.BuiltinRecognizers != 0)
				{
					_Output.WriteStartElement("prop");
					_Output.WriteAttributeString("type", TMXReader.PropertyNameRecognizers);
					_Output.WriteString(tMXStartOfInputEvent.BuiltinRecognizers.ToString());
					_Output.WriteEndElement();
				}
				if (tMXStartOfInputEvent.TextContextMatchType != TextContextMatchType.PrecedingSourceAndTarget)
				{
					_Output.WriteStartElement("prop");
					_Output.WriteAttributeString("type", TMXReader.PropertyNameTextContextMatchType);
					_Output.WriteString(tMXStartOfInputEvent.TextContextMatchType.ToString());
					_Output.WriteEndElement();
				}
				if (tMXStartOfInputEvent.UsesIdContextMatch)
				{
					_Output.WriteStartElement("prop");
					_Output.WriteAttributeString("type", TMXReader.PropertyNameUsesIdContextMatch);
					_Output.WriteString(tMXStartOfInputEvent.UsesIdContextMatch.ToString());
					_Output.WriteEndElement();
				}
				if (tMXStartOfInputEvent.IncludesContextContent)
				{
					_Output.WriteStartElement("prop");
					_Output.WriteAttributeString("type", TMXReader.PropertyNameIncludesContextContentMatch);
					_Output.WriteString(tMXStartOfInputEvent.IncludesContextContent.ToString());
					_Output.WriteEndElement();
				}
				if (!string.IsNullOrEmpty(tMXStartOfInputEvent.TMName))
				{
					_Output.WriteStartElement("prop");
					_Output.WriteAttributeString("type", TMXReader.PropertyNameTMName);
					_Output.WriteString(tMXStartOfInputEvent.TMName);
					_Output.WriteEndElement();
				}
				_Output.WriteStartElement("prop");
				_Output.WriteAttributeString("type", TMXReader.PropertyNameTokenizerFlags);
				_Output.WriteString(tMXStartOfInputEvent.TokenizerFlags.ToString());
				_Output.WriteEndElement();
				_Output.WriteStartElement("prop");
				_Output.WriteAttributeString("type", TMXReader.PropertyNameWordCountFlags);
				_Output.WriteString(tMXStartOfInputEvent.WordCountFlags.ToString());
				_Output.WriteEndElement();
			}
		}

		public override void EmitField(FieldValue field)
		{
			switch (field.ValueType)
			{
			case FieldValueType.Unknown:
				break;
			case FieldValueType.SingleString:
			{
				SingleStringFieldValue singleStringFieldValue = field as SingleStringFieldValue;
				EmitFieldValue(singleStringFieldValue.Name, singleStringFieldValue.ValueType, singleStringFieldValue.Value);
				break;
			}
			case FieldValueType.MultipleString:
			{
				MultipleStringFieldValue multipleStringFieldValue = field as MultipleStringFieldValue;
				foreach (string value in multipleStringFieldValue.Values)
				{
					EmitFieldValue(multipleStringFieldValue.Name, multipleStringFieldValue.ValueType, value);
				}
				break;
			}
			case FieldValueType.DateTime:
			{
				DateTimeFieldValue dateTimeFieldValue = field as DateTimeFieldValue;
				EmitFieldValue(dateTimeFieldValue.Name, dateTimeFieldValue.ValueType, TMXConversions.DateTimeToTMX(dateTimeFieldValue.Value));
				break;
			}
			case FieldValueType.SinglePicklist:
			{
				SinglePicklistFieldValue singlePicklistFieldValue = field as SinglePicklistFieldValue;
				EmitFieldValue(singlePicklistFieldValue.Name, singlePicklistFieldValue.ValueType, singlePicklistFieldValue.Value.Name);
				break;
			}
			case FieldValueType.MultiplePicklist:
			{
				MultiplePicklistFieldValue multiplePicklistFieldValue = field as MultiplePicklistFieldValue;
				foreach (PicklistItem value2 in multiplePicklistFieldValue.Values)
				{
					EmitFieldValue(multiplePicklistFieldValue.Name, multiplePicklistFieldValue.ValueType, value2.Name);
				}
				break;
			}
			case FieldValueType.Integer:
			{
				IntFieldValue intFieldValue = field as IntFieldValue;
				EmitFieldValue(intFieldValue.Name, intFieldValue.ValueType, intFieldValue.Value.ToString(CultureInfo.InvariantCulture));
				break;
			}
			}
		}

		private void EmitFieldValue(string name, FieldValueType type, string value)
		{
			_Output.WriteStartElement("prop");
			_Output.WriteAttributeString("type", "x-" + name + ":" + TMXTUBuilder.GetTypeName(type));
			if (value == null)
			{
				_Output.WriteString(string.Empty);
			}
			else
			{
				_Output.WriteString(value);
			}
			_Output.WriteEndElement();
		}

		public override void EmitOrigin(TranslationUnitOrigin origin)
		{
			_Output.WriteStartElement("prop");
			_Output.WriteAttributeString("type", OriginFieldName);
			_Output.WriteString(origin.ToString());
			_Output.WriteEndElement();
		}

		public override void EmitFormat(TranslationUnitFormat format)
		{
			_Output.WriteStartElement("prop");
			_Output.WriteAttributeString("type", FormatFieldName);
			_Output.WriteString(format.ToString());
			_Output.WriteEndElement();
		}

		public override void EmitConfirmationLevel(ConfirmationLevel level)
		{
			_Output.WriteStartElement("prop");
			_Output.WriteAttributeString("type", ConfirmationLevelFieldName);
			_Output.WriteString(level.ToString());
			_Output.WriteEndElement();
		}

		public override void EmitIdContext(string idContext)
		{
			_Output.WriteStartElement("prop");
			_Output.WriteAttributeString("type", IdContextFieldName);
			_Output.WriteString(idContext);
			_Output.WriteEndElement();
		}

		public override void EmitContext(TuContext tuc)
		{
			_Output.WriteStartElement("prop");
			_Output.WriteAttributeString("type", ContextFieldName);
			_Output.WriteString(tuc.Context1.ToString(CultureInfo.InvariantCulture) + ", " + tuc.Context2.ToString(CultureInfo.InvariantCulture));
			_Output.WriteEndElement();
			if (tuc.Segment1 != null || tuc.Segment2 != null)
			{
				List<byte> list = new List<byte>();
				List<byte> list2 = new List<byte>();
				string s = string.Empty;
				string s2 = string.Empty;
				if (tuc.Segment1 != null)
				{
					s = SegmentSerializer.Save(tuc.Segment1, list);
				}
				if (tuc.Segment2 != null)
				{
					s2 = SegmentSerializer.Save(tuc.Segment2, list2);
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(EscapeSegmentString(s));
				stringBuilder.Append(ContextSeparator);
				if (list.Count > 0)
				{
					stringBuilder.Append(Convert.ToBase64String(list.ToArray()));
				}
				stringBuilder.Append(ContextSeparator);
				stringBuilder.Append(EscapeSegmentString(s2));
				stringBuilder.Append(ContextSeparator);
				if (list2.Count > 0)
				{
					stringBuilder.Append(Convert.ToBase64String(list2.ToArray()));
				}
				_Output.WriteStartElement("prop");
				_Output.WriteAttributeString("type", ContextContentFieldName);
				_Output.WriteString(stringBuilder.ToString());
				_Output.WriteEndElement();
			}
		}

		public override void EmitLastUsedByField(string fieldValue)
		{
			_Output.WriteStartElement("prop");
			_Output.WriteAttributeString("type", LastUsedByFieldName);
			_Output.WriteString(fieldValue);
			_Output.WriteEndElement();
		}

		private string EscapeSegmentString(string s)
		{
			if (s == null)
			{
				return string.Empty;
			}
			return s.Replace(ContextSeparatorChar, ContextSeparatorChar + ContextSeparatorChar);
		}
	}
}
