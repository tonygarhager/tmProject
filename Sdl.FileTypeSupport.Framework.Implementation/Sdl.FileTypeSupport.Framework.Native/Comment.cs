using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	[XmlType("Comment")]
	public class Comment : AbstractMetaDataContainer, IComment, IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		protected string _text;

		protected string _user;

		protected string _version;

		protected DateTime _date;

		protected Severity _severity;

		public static DateTime noDateValue = new DateTime(1899, 12, 30);

		[NonSerialized]
		private int _persistenceId;

		[XmlAttribute("severity")]
		public Severity Severity
		{
			get
			{
				return _severity;
			}
			set
			{
				_severity = value;
			}
		}

		[XmlIgnore]
		public bool SeveritySpecified
		{
			get
			{
				return _severity != Severity.Undefined;
			}
			set
			{
			}
		}

		[XmlText]
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		[XmlAttribute("user")]
		public string Author
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
			}
		}

		[XmlAttribute("date")]
		public DateTime Date
		{
			get
			{
				return _date;
			}
			set
			{
				_date = value;
			}
		}

		[XmlIgnore]
		public bool DateSpecified
		{
			get
			{
				return _date != noDateValue;
			}
			set
			{
			}
		}

		[XmlAttribute("version")]
		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
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

		public Comment()
		{
			_text = "";
			_user = "";
			_version = "";
			_severity = Severity.Undefined;
			_date = noDateValue;
		}

		public Comment(IComment other)
		{
			_text = other.Text;
			_user = other.Author;
			_version = other.Version;
			_date = other.Date;
			_severity = other.Severity;
			ReplaceMetaDataWithCloneOf(((Comment)other)._MetaData);
		}

		public Comment(string user, string version, string text)
		{
			_text = text;
			_user = user;
			_version = version;
			_severity = Severity.Undefined;
		}

		protected Comment(Comment other)
		{
			_text = other._text;
			_user = other._user;
			_version = other._version;
			_date = other._date;
			_severity = other._severity;
			ReplaceMetaDataWithCloneOf(other._MetaData);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Comment comment = obj as Comment;
			if (!base.Equals((object)comment))
			{
				return false;
			}
			if (_text != comment._text)
			{
				return false;
			}
			if (_user != comment._user)
			{
				return false;
			}
			if (_version != comment._version)
			{
				return false;
			}
			if (!_date.Equals(comment._date))
			{
				return false;
			}
			if (!_severity.Equals(comment._severity))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_text != null) ? _text.GetHashCode() : 0) ^ ((_user != null) ? _user.GetHashCode() : 0) ^ _date.GetHashCode() ^ ((_version != null) ? _version.GetHashCode() : 0) ^ _severity.GetHashCode();
		}

		public virtual object Clone()
		{
			return new Comment(this);
		}
	}
}
