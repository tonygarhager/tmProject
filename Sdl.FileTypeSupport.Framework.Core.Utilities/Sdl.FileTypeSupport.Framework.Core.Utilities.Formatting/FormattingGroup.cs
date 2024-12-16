using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Properties;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting
{
	[Serializable]
	public class FormattingGroup : ObservableDictionary<string, IFormattingItem>, IFormattingGroup, IDictionary<string, IFormattingItem>, ICollection<KeyValuePair<string, IFormattingItem>>, IEnumerable<KeyValuePair<string, IFormattingItem>>, IEnumerable, ICloneable, IXmlSerializable, ISupportsPersistenceId
	{
		private const string FORMATTING_LIST_ELEMENT_NAME = "FormattingItemsList";

		private const string FORMATTING_ITEM_ELEMENT_NAME = "FormattingItem";

		private const string FORMATTING_ITEM_TYPE_ATTR_NAME = "Type";

		private const string FORMATTING_ITEM_VALUE_ATTR_NAME = "Value";

		private const string ERROR_COMMENT_FORMATTING_ELEMENT_NAME = "_ErrorCommentFormatting";

		private const string OLD_NAME = "Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting.Formatting";

		[NonSerialized]
		private int _persistenceId;

		[XmlIgnore]
		public int PersistenceId
		{
			get
			{
				return _persistenceId;
			}
			set
			{
				_persistenceId = value;
			}
		}

		public FormattingGroup()
		{
		}

		protected FormattingGroup(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected FormattingGroup(FormattingGroup other)
		{
			foreach (KeyValuePair<string, IFormattingItem> item in other)
			{
				Add((IFormattingItem)item.Value.Clone());
			}
		}

		public static FormattingGroup Create(params object[] formattingItems)
		{
			FormattingGroup formattingGroup = new FormattingGroup();
			foreach (object obj in formattingItems)
			{
				formattingGroup.Add((IFormattingItem)obj);
			}
			return formattingGroup;
		}

		private void AddAllItems(IEnumerable<IFormattingItem> formattingItemDataList)
		{
			foreach (IFormattingItem formattingItemData in formattingItemDataList)
			{
				Add(formattingItemData);
			}
		}

		public void Add(IFormattingItem formatting)
		{
			if (formatting == null)
			{
				throw new ArgumentNullException("formatting");
			}
			Remove(formatting.FormattingName);
			base.Add(formatting.FormattingName, formatting);
		}

		public bool Contains(string formattingName)
		{
			return ContainsKey(formattingName);
		}

		public bool Contains(IFormattingItem formatting)
		{
			if (formatting == null)
			{
				return false;
			}
			if (!ContainsKey(formatting.FormattingName))
			{
				return false;
			}
			return formatting.Equals(base[formatting.FormattingName]);
		}

		public void OverrideWith(IFormattingGroup otherFormatting)
		{
			foreach (KeyValuePair<string, IFormattingItem> item in otherFormatting)
			{
				Add(item.Value);
			}
		}

		public void UnderrideWith(IFormattingGroup otherFormatting)
		{
			foreach (KeyValuePair<string, IFormattingItem> item in otherFormatting)
			{
				if (!ContainsKey(item.Value.FormattingName))
				{
					Add(item.Value);
				}
			}
		}

		public new void Add(string key, IFormattingItem value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (key != value.FormattingName)
			{
				throw new ArgumentException(Resources.KeyMustBeFormattingNameOfValue);
			}
			Add(value);
		}

		public override bool Equals(object obj)
		{
			FormattingGroup formattingGroup = obj as FormattingGroup;
			if (formattingGroup == null)
			{
				return false;
			}
			if (base.Count != formattingGroup.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, IFormattingItem> item in formattingGroup)
			{
				if (!ContainsKey(item.Value.FormattingName))
				{
					return false;
				}
				if (!item.Value.Equals(base[item.Value.FormattingName]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			using (IEnumerator<KeyValuePair<string, IFormattingItem>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num ^= enumerator.Current.Value.GetHashCode();
				}
				return num;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IFormattingItem value in base.Values)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(value.ToString());
			}
			return stringBuilder.ToString();
		}

		public object Clone()
		{
			return new FormattingGroup(this);
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public virtual void ReadXml(XmlReader reader)
		{
			if (!IsCurrentOrPreviousTypeName(reader.LocalName))
			{
				reader.Read();
			}
			if (reader.MoveToContent() != XmlNodeType.Element || !IsCurrentOrPreviousTypeName(reader.LocalName))
			{
				return;
			}
			reader.Read();
			bool flag = false;
			IFormattingItemFactory formattingItemFactory = GetFormattingItemFactory();
			while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "FormattingItem")
			{
				flag = true;
				string text = reader["Type"];
				if (!string.IsNullOrEmpty(text))
				{
					IFormattingItem formatting = formattingItemFactory.CreateFormattingItem(text, reader["Value"]);
					Add(formatting);
				}
				reader.Read();
			}
			if (flag)
			{
				reader.Read();
			}
		}

		protected virtual IFormattingItemFactory GetFormattingItemFactory()
		{
			return new FormattingItemFactory();
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().ToString());
			foreach (IFormattingItem value in base.Values)
			{
				writer.WriteStartElement("FormattingItem");
				writer.WriteAttributeString("Type", value.FormattingName);
				writer.WriteAttributeString("Value", value.StringValue);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		public bool IsCurrentOrPreviousTypeName(string name)
		{
			if (!(name == GetType().ToString()))
			{
				return name == "Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting.Formatting";
			}
			return true;
		}
	}
}
