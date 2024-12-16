using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Sdl.Core.Globalization
{
	[Serializable]
	public class Codepage : ICloneable
	{
		private string _name;

		[NonSerialized]
		private Encoding _lazyEncoding;

		[NonSerialized]
		private bool _evaluationAttempted;

		public bool IsValid => !string.IsNullOrEmpty(_name);

		public bool IsSupported
		{
			get
			{
				if (!IsValid)
				{
					return false;
				}
				if (_lazyEncoding == null)
				{
					EvaluateEncoding();
				}
				return _lazyEncoding != null;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				_lazyEncoding = null;
				_evaluationAttempted = false;
			}
		}

		[XmlIgnore]
		public Encoding Encoding
		{
			get
			{
				if (!string.IsNullOrEmpty(_name) && _lazyEncoding == null)
				{
					EvaluateEncoding();
					if (_lazyEncoding == null)
					{
						throw new UnsupportedCodepageException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedCodepageExceptionMessage, new object[1]
						{
							_name
						}));
					}
				}
				return _lazyEncoding;
			}
			set
			{
				_name = value?.WebName;
				_lazyEncoding = value;
				_evaluationAttempted = false;
			}
		}

		public Codepage(Encoding encoding)
		{
			if (encoding != null)
			{
				_name = encoding.WebName;
				_evaluationAttempted = true;
			}
			_lazyEncoding = encoding;
		}

		public Codepage(string codepageName)
		{
			_name = codepageName;
		}

		public Codepage()
		{
		}

		protected Codepage(Codepage other)
		{
			_name = other._name;
			if (other._lazyEncoding != null)
			{
				_lazyEncoding = (Encoding)other._lazyEncoding.Clone();
			}
			_evaluationAttempted = other._evaluationAttempted;
		}

		private void EvaluateEncoding()
		{
			if (!_evaluationAttempted)
			{
				try
				{
					_lazyEncoding = Encoding.GetEncoding(Name);
				}
				catch (ArgumentException)
				{
					_lazyEncoding = null;
				}
				_evaluationAttempted = true;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Codepage codepage = (Codepage)obj;
			if (!_name.Equals(codepage._name))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (_name?.GetHashCode() ?? 0);
		}

		public override string ToString()
		{
			return _name ?? string.Empty;
		}

		public object Clone()
		{
			return new Codepage(this);
		}
	}
}
