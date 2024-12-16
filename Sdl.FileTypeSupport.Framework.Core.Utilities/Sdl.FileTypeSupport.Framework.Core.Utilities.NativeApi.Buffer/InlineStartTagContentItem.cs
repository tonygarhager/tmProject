using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class InlineStartTagContentItem : AbstractContentItem
	{
		private IStartTagProperties _Properties;

		public IStartTagProperties Properties
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

		public InlineStartTagContentItem(IStartTagProperties properties)
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
			InlineStartTagContentItem inlineStartTagContentItem = (InlineStartTagContentItem)obj;
			if (_Properties != null)
			{
				return _Properties.Equals(inlineStartTagContentItem._Properties);
			}
			return _Properties == inlineStartTagContentItem._Properties;
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
			output.InlineStartTag(_Properties);
		}
	}
}
