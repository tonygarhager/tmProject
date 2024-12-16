using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class InlinePlaceholderTagContentItem : AbstractContentItem
	{
		private IPlaceholderTagProperties _Properties;

		public IPlaceholderTagProperties Properties
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

		public InlinePlaceholderTagContentItem(IPlaceholderTagProperties properties)
		{
			_Properties = properties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			InlinePlaceholderTagContentItem inlinePlaceholderTagContentItem = (InlinePlaceholderTagContentItem)obj;
			if (_Properties != null)
			{
				return _Properties.Equals(inlinePlaceholderTagContentItem._Properties);
			}
			return _Properties == inlinePlaceholderTagContentItem._Properties;
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
			output.InlinePlaceholderTag(_Properties);
		}
	}
}
