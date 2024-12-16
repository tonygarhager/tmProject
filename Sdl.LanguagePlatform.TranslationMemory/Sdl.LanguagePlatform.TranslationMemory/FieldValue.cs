using Sdl.LanguagePlatform.Core;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	[KnownType(typeof(SingleStringFieldValue))]
	[KnownType(typeof(MultipleStringFieldValue))]
	[KnownType(typeof(SinglePicklistFieldValue))]
	[KnownType(typeof(MultiplePicklistFieldValue))]
	[KnownType(typeof(DateTimeFieldValue))]
	[KnownType(typeof(IntFieldValue))]
	public abstract class FieldValue
	{
		private string _name;

		[DataMember]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (!Field.IsValidName(value))
				{
					throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName);
				}
				_name = value;
			}
		}

		[DataMember]
		public abstract FieldValueType ValueType
		{
			get;
			set;
		}

		public FieldValue()
		{
		}

		public FieldValue(string name)
		{
			Name = name;
		}

		public abstract bool Merge(FieldValue rhs);

		public abstract bool Add(FieldValue rhs);

		public abstract bool Substract(FieldValue rhs);

		public abstract FieldValue Duplicate();

		public abstract void Parse(string s);

		public abstract bool Add(string s);

		public abstract void Clear();

		public abstract string GetValueString();
	}
}
