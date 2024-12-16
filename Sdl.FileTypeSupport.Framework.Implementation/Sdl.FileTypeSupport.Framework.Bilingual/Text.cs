using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class Text : AbstractDataContent, IText, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		private ITextProperties _Properties;

		public ITextProperties Properties
		{
			get
			{
				return _Properties;
			}
			set
			{
				_Properties = value;
			}
		}

		public Text()
		{
		}

		protected Text(Text other)
			: base(other)
		{
			if (other._Properties != null)
			{
				_Properties = (ITextProperties)other._Properties.Clone();
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Text text = (Text)obj;
			if (text._Properties == null != (_Properties == null))
			{
				return false;
			}
			if (_Properties != null && !_Properties.Equals(text._Properties))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (_Properties != null)
			{
				return _Properties.ToString();
			}
			return string.Empty;
		}

		public override object Clone()
		{
			return new Text(this);
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitText(this);
		}

		public IText Split(int fromIndex)
		{
			if (Properties == null)
			{
				throw new FileTypeSupportException(StringResources.Text_NoPropertiesError);
			}
			if (Properties.Text == null)
			{
				throw new FileTypeSupportException(StringResources.Text_NullError);
			}
			if (fromIndex < 0 || fromIndex > Properties.Text.Length)
			{
				throw new ArgumentOutOfRangeException("fromIndex");
			}
			IText text = (IText)Clone();
			text.Properties.Text = Properties.Text.Remove(0, fromIndex);
			if (fromIndex < Properties.Text.Length)
			{
				Properties.Text = Properties.Text.Remove(fromIndex);
			}
			return text;
		}
	}
}
