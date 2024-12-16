using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class AttributeValue
	{
		private FieldValueType _type;

		public int DeclarationId
		{
			get;
			set;
		}

		public string AttributeName
		{
			get;
		}

		public FieldValueType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public object Value
		{
			get;
		}

		public AttributeValue(AttributeDeclaration attribute, object value)
			: this(attribute.Id, attribute.Name, attribute.Type, value)
		{
		}

		internal AttributeValue(int attributeId, string name, FieldValueType type, object value)
		{
			DeclarationId = attributeId;
			AttributeName = name;
			_type = type;
			switch (_type)
			{
			case FieldValueType.SingleString:
				Value = (string)value;
				break;
			case FieldValueType.MultipleString:
				Value = (string[])value;
				break;
			case FieldValueType.SinglePicklist:
				Value = (int)value;
				break;
			case FieldValueType.MultiplePicklist:
				Value = (int[])value;
				break;
			case FieldValueType.DateTime:
			{
				DateTime dateTime = (DateTime)value;
				Value = ((dateTime.Kind == DateTimeKind.Unspecified) ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime);
				break;
			}
			case FieldValueType.Integer:
				Value = (int)value;
				break;
			default:
				throw new ArgumentException("Unknown attribute type.", "_type");
			}
		}
	}
}
