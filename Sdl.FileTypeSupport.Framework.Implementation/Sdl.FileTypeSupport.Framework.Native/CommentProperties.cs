using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class CommentProperties : CollectionBase, ICommentProperties, ICloneable, ISupportsPersistenceId
	{
		private static XmlSerializer _XmlSerializer = CreateXmlSerializer();

		[NonSerialized]
		private int _persistenceId;

		public Comment this[int index] => (Comment)base.InnerList[index];

		public IEnumerable<IComment> Comments => base.InnerList.Cast<IComment>();

		public string Xml
		{
			get
			{
				return SerializeToString();
			}
			set
			{
				SetFromOther(LoadFromString(value));
			}
		}

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

		public CommentProperties()
		{
		}

		protected CommentProperties(CommentProperties other)
		{
			SetFromOther(other);
		}

		private void SetFromOther(CommentProperties other)
		{
			Clear();
			foreach (Comment item in other)
			{
				Add((Comment)item.Clone());
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			CommentProperties commentProperties = (CommentProperties)obj;
			if (base.Count != commentProperties.Count)
			{
				return false;
			}
			for (int i = 0; i < base.Count; i++)
			{
				Comment comment = this[i];
				Comment comment2 = commentProperties[i];
				if (comment == null != (comment2 == null))
				{
					return false;
				}
				if (comment != null && !comment.Equals(comment2))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.Count;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (current != null)
					{
						num ^= current.GetHashCode();
					}
				}
				return num;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public override string ToString()
		{
			return Xml;
		}

		public IComment GetItem(int index)
		{
			return (Comment)base.InnerList[index];
		}

		private void AssignVersion(Comment comment)
		{
			int num = 0;
			foreach (IComment inner in base.InnerList)
			{
				if (!string.IsNullOrEmpty(inner.Version))
				{
					int val = int.Parse(inner.Version.Split('.')[0], CultureInfo.InvariantCulture);
					num = Math.Max(num, val);
				}
			}
			comment.Version = (num + 1).ToString() + ".0";
		}

		public void Add(Comment comment)
		{
			AssignVersion(comment);
			base.InnerList.Add(comment);
		}

		public void Add(string text, string user, DateTime date, Severity severity)
		{
			Comment comment = new Comment();
			comment.Text = text;
			comment.Author = user;
			comment.Date = date;
			comment.Severity = severity;
			Add(comment);
		}

		public void Add(CommentProperties comments)
		{
			foreach (Comment comment in comments)
			{
				Add(comment);
			}
		}

		public void Remove(Comment comment)
		{
			int num = 0;
			while (true)
			{
				if (num < base.InnerList.Count)
				{
					if (base.InnerList[num].Equals(comment))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			RemoveAt(num);
		}

		public static CommentProperties LoadFromString(string xml)
		{
			StringReader textReader = new StringReader(xml);
			return (CommentProperties)_XmlSerializer.Deserialize(textReader);
		}

		internal string SerializeToString()
		{
			XmlAttributes xmlAttributes = new XmlAttributes();
			StringWriter stringWriter = new StringWriter();
			XmlStringWriter xmlWriter = new XmlStringWriter(stringWriter);
			XmlSerializerNamespaces namespaces = CreateEmptyNamespace();
			_XmlSerializer.Serialize(xmlWriter, this, namespaces);
			return stringWriter.GetStringBuilder().ToString();
		}

		private static XmlSerializer CreateXmlSerializer()
		{
			try
			{
				XmlRootAttribute root = new XmlRootAttribute("Comments");
				return new XmlSerializer(typeof(CommentProperties), root);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private static XmlSerializerNamespaces CreateEmptyNamespace()
		{
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add("", "");
			return xmlSerializerNamespaces;
		}

		public void Add(IComment comment)
		{
			Add((Comment)comment);
		}

		public void AddComments(ICommentProperties comments)
		{
			Add((CommentProperties)comments);
		}

		public void Delete(IComment comment)
		{
			Remove((Comment)comment);
		}

		public object Clone()
		{
			return new CommentProperties(this);
		}
	}
}
