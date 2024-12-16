using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class FieldDefinitions : IEnumerable<Field>, IEnumerable, IFieldDefinitions
	{
		private Dictionary<string, Field> _Fields;

		[DataMember]
		public ICollection<Field> Fields
		{
			get
			{
				EnsureFieldsInitialized();
				return _Fields.Values;
			}
			set
			{
				_Fields = ((value == null) ? new Dictionary<string, Field>() : value.ToDictionary((Field f) => f.Name, StringComparer.OrdinalIgnoreCase));
			}
		}

		public Field this[string name] => Lookup(name);

		public Field this[Guid guid] => Lookup(guid);

		public int Count
		{
			get
			{
				Dictionary<string, Field> fields = _Fields;
				if (fields == null)
				{
					return 0;
				}
				return fields.Count;
			}
		}

		public FieldDefinitions()
			: this((ICollection<Field>)null)
		{
		}

		public FieldDefinitions(FieldDefinitions other)
			: this(other.Fields)
		{
		}

		public FieldDefinitions(ICollection<Field> fields)
		{
			fields = (from y in fields?.GroupBy((Field x) => x.Name)
				select y.First()).ToList();
			_Fields = ((fields == null) ? new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase) : fields.ToDictionary((Field f) => f.Name, StringComparer.OrdinalIgnoreCase));
		}

		public Field Lookup(string name)
		{
			if (_Fields == null)
			{
				return null;
			}
			if (!_Fields.TryGetValue(name, out Field value))
			{
				return null;
			}
			return value;
		}

		public Field Lookup(Guid guid)
		{
			if (_Fields == null)
			{
				return null;
			}
			foreach (Field value in _Fields.Values)
			{
				if (value.ResourceId.Guid == guid)
				{
					return value;
				}
			}
			return null;
		}

		public IField LookupIField(Guid guid)
		{
			return Lookup(guid);
		}

		public IField LookupIField(string name)
		{
			return Lookup(name);
		}

		public Field Add(string fieldName, FieldValueType fieldType)
		{
			if (Exists(fieldName))
			{
				throw new LanguagePlatformException(ErrorCode.TMFieldAlreadyExists, fieldName);
			}
			EnsureFieldsInitialized();
			Field field = (fieldType != FieldValueType.SinglePicklist && fieldType != FieldValueType.MultiplePicklist) ? new Field(fieldName, fieldType) : new PicklistField(fieldName, fieldType);
			_Fields.Add(fieldName, field);
			return field;
		}

		public bool Exists(string name)
		{
			return Lookup(name) != null;
		}

		public bool Exists(Field f)
		{
			if (f == null)
			{
				throw new ArgumentNullException();
			}
			return Lookup(f.Name) != null;
		}

		public List<Field> ToList()
		{
			EnsureFieldsInitialized();
			List<Field> list = new List<Field>();
			foreach (Field value in _Fields.Values)
			{
				list.Add(value.Duplicate());
			}
			return list;
		}

		public void Add(Field f)
		{
			if (Exists(f.Name))
			{
				throw new LanguagePlatformException(ErrorCode.TMFieldAlreadyExists, f.Name);
			}
			EnsureFieldsInitialized();
			_Fields.Add(f.Name, f);
		}

		public bool Remove(Field f)
		{
			if (_Fields != null)
			{
				return _Fields.Remove(f.Name);
			}
			return false;
		}

		public bool Remove(string fieldName)
		{
			if (_Fields != null)
			{
				return _Fields.Remove(fieldName);
			}
			return false;
		}

		public IEnumerator<Field> GetEnumerator()
		{
			EnsureFieldsInitialized();
			return _Fields.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			EnsureFieldsInitialized();
			return _Fields.Values.GetEnumerator();
		}

		public void Clear()
		{
			_Fields?.Clear();
		}

		public bool Contains(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				throw new ArgumentNullException("fieldName");
			}
			if (_Fields != null)
			{
				return _Fields.ContainsKey(fieldName);
			}
			return false;
		}

		private void EnsureFieldsInitialized()
		{
			if (_Fields == null)
			{
				_Fields = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
			}
		}
	}
}
