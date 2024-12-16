using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class PicklistField : Field
	{
		[DataMember]
		private PicklistItems _Items;

		public override IList<string> PicklistItemNames => Picklist.Select((PicklistItem picklistItem) => picklistItem.Name).ToList();

		public PicklistItems Picklist => _Items;

		public PicklistField()
		{
			_Items = new PicklistItems();
		}

		public PicklistField(string name, FieldValueType t, ICollection<PicklistItem> picklistValues)
			: base(name, t)
		{
			if (t != FieldValueType.SinglePicklist && t != FieldValueType.MultiplePicklist)
			{
				throw new LanguagePlatformException(ErrorCode.DAFieldTypesInconsistent);
			}
			_Items = new PicklistItems();
			if (picklistValues != null)
			{
				foreach (PicklistItem picklistValue in picklistValues)
				{
					_Items.Add(new PicklistItem(picklistValue));
				}
			}
		}

		public PicklistField(string name, FieldValueType t, ICollection<string> picklistValues)
			: base(name, t)
		{
			if (t != FieldValueType.SinglePicklist && t != FieldValueType.MultiplePicklist)
			{
				throw new LanguagePlatformException(ErrorCode.DAFieldTypesInconsistent);
			}
			_Items = new PicklistItems();
			if (picklistValues != null)
			{
				foreach (string picklistValue in picklistValues)
				{
					_Items.Add(picklistValue);
				}
			}
		}

		public PicklistField(string name, FieldValueType t)
			: this(name, t, (ICollection<string>)null)
		{
		}

		public PicklistField(PicklistField other)
			: base(other)
		{
			_Items = new PicklistItems(other._Items);
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
			PicklistField picklistField = obj as PicklistField;
			if (picklistField == null)
			{
				return false;
			}
			if (base.ValueType != picklistField.ValueType)
			{
				return false;
			}
			if (base.Name.Equals(picklistField.Name, StringComparison.OrdinalIgnoreCase))
			{
				return _Items.Equals(picklistField._Items);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public string PicklistToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (PicklistItem item in _Items)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(item.Name);
			}
			return stringBuilder.ToString();
		}

		public void PicklistFromString(string values)
		{
			string[] array = values.Split(new char[1]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (!_Items.Contains(text))
				{
					_Items.Add(text);
				}
			}
		}

		public override object Clone()
		{
			return Duplicate();
		}

		public override Field Duplicate()
		{
			return new PicklistField(this);
		}
	}
}
