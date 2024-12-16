using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class FieldIdentifier
	{
		[DataMember]
		public FieldValueType FieldValueType
		{
			get;
			set;
		}

		[DataMember]
		public string FieldName
		{
			get;
			set;
		}

		public FieldIdentifier()
		{
		}

		public FieldIdentifier(FieldValueType fieldValueType, string fieldName)
		{
			FieldValueType = fieldValueType;
			FieldName = fieldName;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			FieldIdentifier fieldIdentifier = obj as FieldIdentifier;
			if (fieldIdentifier != null && object.Equals(fieldIdentifier.FieldValueType, FieldValueType))
			{
				return object.Equals(fieldIdentifier.FieldName, FieldName);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 71 + 391 * FieldValueType.GetHashCode() + 1077 * (FieldName ?? "").GetHashCode();
		}
	}
}
