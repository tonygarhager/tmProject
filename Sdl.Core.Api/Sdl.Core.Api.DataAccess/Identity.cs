using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Api.DataAccess
{
	public class Identity : IXmlSerializable, IEquatable<Identity>
	{
		private object _value;

		public object Value => _value;

		public int IntValue
		{
			get
			{
				return (int)_value;
			}
			set
			{
				_value = value;
			}
		}

		public Identity()
		{
		}

		public Identity(object value)
		{
			if (value is int || value is Guid)
			{
				_value = value;
				return;
			}
			if (value is decimal)
			{
				_value = Convert.ToInt32(value);
				return;
			}
			if (value is long)
			{
				_value = Convert.ToInt32(value);
				return;
			}
			if (value is string)
			{
				_value = new Guid((string)value);
				return;
			}
			if (value is byte[])
			{
				_value = new Guid((byte[])value);
				return;
			}
			throw new InvalidOperationException(string.Format("id type '{0}' not supported", (value != null) ? value.GetType().ToString() : "NULL"));
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			while (reader.Read())
			{
				if (reader.IsStartElement("Identity"))
				{
					Type type = Type.GetType(reader.GetAttribute("Type"), throwOnError: true, ignoreCase: true);
					DataContractSerializer dataContractSerializer = new DataContractSerializer(type);
					reader.Read();
					_value = dataContractSerializer.ReadObject(reader.ReadSubtree());
				}
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (Value != null)
			{
				Type type = Value.GetType();
				writer.WriteStartElement("Identity");
				writer.WriteAttributeString("Type", type.AssemblyQualifiedName);
				DataContractSerializer dataContractSerializer = new DataContractSerializer(type);
				dataContractSerializer.WriteObject(writer, Value);
				writer.WriteEndElement();
			}
		}

		public override string ToString()
		{
			if (Value != null)
			{
				if (Value is int)
				{
					return Value.ToString();
				}
				return "'" + Value.ToString() + "'";
			}
			return null;
		}

		public override int GetHashCode()
		{
			if (Value == null)
			{
				return base.GetHashCode();
			}
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return ((IEquatable<Identity>)this).Equals((Identity)obj);
		}

		public bool Equals(Identity other)
		{
			if (other == null)
			{
				return false;
			}
			if (other.Value == null && Value == null)
			{
				return true;
			}
			if ((other.Value != null && Value == null) || (other.Value == null && Value != null))
			{
				return false;
			}
			return Value.Equals(other.Value);
		}
	}
}
