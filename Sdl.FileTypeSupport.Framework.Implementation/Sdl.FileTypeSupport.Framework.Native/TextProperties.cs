using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class TextProperties : ITextProperties, ICloneable, ISupportsPersistenceId
	{
		private string _Text;

		[NonSerialized]
		private int _persistenceId;

		public string Text
		{
			get
			{
				return _Text;
			}
			set
			{
				_Text = value;
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

		public TextProperties()
		{
		}

		protected TextProperties(TextProperties other)
		{
			_Text = other._Text;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			TextProperties textProperties = (TextProperties)obj;
			if (_Text != textProperties._Text)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (_Text == null)
			{
				return 0;
			}
			return _Text.GetHashCode();
		}

		public override string ToString()
		{
			return _Text;
		}

		public virtual object Clone()
		{
			return new TextProperties(this);
		}
	}
}
