using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.Lingua.Locales;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class WorkbenchEmitter : AbstractEmitter
	{
		public WorkbenchEmitter(XmlWriter output, TMXWriterSettings writerSettings)
			: base(output, writerSettings)
		{
			_TargetLocaleSystem = LocaleInfoSet.GetLocaleInfoSet(LocaleSource.Workbench);
		}

		public override void EmitExtendedHeader(StartOfInputEvent soi)
		{
		}

		public override void EmitHeaderFormat()
		{
			_Output.WriteAttributeString("o-tmf", "TW4Win 2.0 Format");
			_Output.WriteAttributeString("datatype", "rtf");
		}

		public override void EmitField(FieldValue field)
		{
			string text = null;
			StringBuilder stringBuilder = null;
			bool flag = false;
			switch (field.ValueType)
			{
			case FieldValueType.SingleString:
				text = ((SingleStringFieldValue)field).Value;
				break;
			case FieldValueType.MultipleString:
				stringBuilder = new StringBuilder();
				flag = true;
				foreach (string value in ((MultipleStringFieldValue)field).Values)
				{
					if (!string.IsNullOrEmpty(value))
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(value);
					}
				}
				text = stringBuilder.ToString();
				break;
			case FieldValueType.DateTime:
				text = ((DateTimeFieldValue)field).Value.ToString("r");
				break;
			case FieldValueType.SinglePicklist:
				text = ((SinglePicklistFieldValue)field).Value.Name;
				break;
			case FieldValueType.MultiplePicklist:
				stringBuilder = new StringBuilder();
				flag = true;
				foreach (PicklistItem value2 in ((MultiplePicklistFieldValue)field).Values)
				{
					if (value2 != null && value2.Name != null)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(value2.Name);
					}
				}
				text = stringBuilder.ToString();
				break;
			case FieldValueType.Integer:
				text = ((IntFieldValue)field).Value.ToString(CultureInfo.InvariantCulture);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				_Output.WriteStartElement("prop");
				if (field.ValueType == FieldValueType.SinglePicklist || field.ValueType == FieldValueType.MultiplePicklist)
				{
					_Output.WriteAttributeString("type", "Att::" + field.Name);
				}
				else
				{
					_Output.WriteAttributeString("type", "Txt::" + field.Name);
				}
				_Output.WriteString(text);
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
