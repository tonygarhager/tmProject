using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class InlineEndTagContentItem : AbstractContentItem
	{
		private IEndTagProperties _Properties;

		public IEndTagProperties Properties
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

		public InlineEndTagContentItem(IEndTagProperties properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			_Properties = properties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			InlineEndTagContentItem inlineEndTagContentItem = (InlineEndTagContentItem)obj;
			if (_Properties != null)
			{
				return _Properties.Equals(inlineEndTagContentItem._Properties);
			}
			return _Properties == inlineEndTagContentItem._Properties;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (_Properties == null)
			{
				return base.ToString();
			}
			return _Properties.ToString();
		}

		public override void Invoke(IAbstractNativeContentHandler output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.InlineEndTag(_Properties);
		}
	}
}
