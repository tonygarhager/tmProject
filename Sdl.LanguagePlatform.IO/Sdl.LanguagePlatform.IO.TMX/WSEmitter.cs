using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.Lingua.Locales;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;
using System.Xml;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class WSEmitter : AbstractEmitter
	{
		public WSEmitter(XmlWriter output, TMXWriterSettings writerSettings)
			: base(output, writerSettings)
		{
			_TargetLocaleSystem = LocaleInfoSet.GetLocaleInfoSet(LocaleSource.SDLX);
		}

		public override void EmitExtendedHeader(StartOfInputEvent soi)
		{
		}

		public override void EmitHeaderFormat()
		{
			_Output.WriteAttributeString("o-tmf", "Idiom TM v9.3.0");
			_Output.WriteAttributeString("datatype", "PlainText");
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
			if (!string.IsNullOrEmpty(value))
			{
				_Output.WriteStartElement("prop");
				_Output.WriteAttributeString("type", "x-idiom-tm-uda-" + name);
				_Output.WriteString(value);
				_Output.WriteEndElement();
			}
		}

		public override void EmitOrigin(TranslationUnitOrigin origin)
		{
		}

		public override void EmitFormat(TranslationUnitFormat format)
		{
		}

		public override void EmitConfirmationLevel(ConfirmationLevel level)
		{
		}

		public override void EmitContext(TuContext tuc)
		{
		}

		public override void EmitLastUsedByField(string fieldValue)
		{
		}

		public override void EmitIdContext(string idContext)
		{
		}
	}
}
