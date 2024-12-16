using System.Text;
using System.Xml;

namespace Sdl.Core.Settings.Implementation
{
	internal class RemoveControlCharactersXmlWriter : XmlWriter
	{
		private readonly XmlWriter _inner;

		public override WriteState WriteState => _inner.WriteState;

		public RemoveControlCharactersXmlWriter(XmlWriter inner)
		{
			_inner = inner;
		}

		public override void Close()
		{
			_inner.Close();
		}

		public override void Flush()
		{
			_inner.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return _inner.LookupPrefix(ns);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			_inner.WriteBase64(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			_inner.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			_inner.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			_inner.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			_inner.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			_inner.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteEndAttribute()
		{
			_inner.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			_inner.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			_inner.WriteFullEndElement();
		}

		public override void WriteEntityRef(string name)
		{
			_inner.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			_inner.WriteFullEndElement();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			_inner.WriteProcessingInstruction(name, text);
		}

		public override void WriteRaw(string data)
		{
			_inner.WriteRaw(data);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			_inner.WriteRaw(buffer, index, count);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			_inner.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteStartDocument(bool standalone)
		{
			_inner.WriteStartDocument(standalone);
		}

		public override void WriteStartDocument()
		{
			_inner.WriteStartDocument();
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			_inner.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteString(string text)
		{
			_inner.WriteString(RemoveControlCharacters(text));
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			_inner.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteWhitespace(string ws)
		{
			_inner.WriteWhitespace(ws);
		}

		private static string RemoveControlCharacters(string inString)
		{
			if (inString == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in inString)
			{
				if (!char.IsControl(c))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
