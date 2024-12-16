using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class AttributeDeclaration : DbObject
	{
		private string _name;

		private List<PickValue> _picklist;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public FieldValueType Type
		{
			get;
			set;
		}

		public List<PickValue> Picklist
		{
			get
			{
				if (IsPicklistField)
				{
					return _picklist ?? (_picklist = new List<PickValue>());
				}
				throw new ArgumentException("Attribute must have one of the picklist types.", "Type");
			}
			set
			{
				if (IsPicklistField)
				{
					_picklist = value;
					return;
				}
				throw new ArgumentException("Attribute must have one of the picklist types.", "Type");
			}
		}

		public int TMId
		{
			get;
			set;
		}

		public bool IsPicklistField
		{
			get
			{
				if (Type != FieldValueType.SinglePicklist)
				{
					return Type == FieldValueType.MultiplePicklist;
				}
				return true;
			}
		}

		public AttributeDeclaration(string name, Guid guid, FieldValueType type, int tmId)
			: base(0, guid)
		{
			Name = name;
			Type = type;
			TMId = tmId;
		}

		internal AttributeDeclaration(int id, Guid guid, string name, FieldValueType type, int tmId)
			: this(name, guid, type, tmId)
		{
			base.Id = id;
			base.Guid = guid;
		}

		public int GetPicklistValueId(string value)
		{
			if (!IsPicklistField)
			{
				throw new ArgumentException("Attribute must have one of the picklist types.", "Type");
			}
			foreach (PickValue item in Picklist)
			{
				if (value.Equals(item.Value, StringComparison.OrdinalIgnoreCase))
				{
					return item.Id;
				}
			}
			throw new ArgumentException("Invalid picklist value for this attribute.", value);
		}

		public int FindPicklistValueId(string value)
		{
			if (!IsPicklistField)
			{
				return -1;
			}
			foreach (PickValue item in Picklist)
			{
				if (value.Equals(item.Value, StringComparison.OrdinalIgnoreCase))
				{
					return item.Id;
				}
			}
			return -1;
		}

		public void AddPicklistValues(IEnumerable<PickValue> values)
		{
			Picklist.AddRange(values);
		}

		public void AddPickListValue(PickValue value)
		{
			Picklist.Add(value);
		}

		public List<PickValue> GetPicklistValues()
		{
			if (IsPicklistField)
			{
				return Picklist;
			}
			throw new ArgumentException("Attribute must have one of the picklist types.");
		}
	}
}
