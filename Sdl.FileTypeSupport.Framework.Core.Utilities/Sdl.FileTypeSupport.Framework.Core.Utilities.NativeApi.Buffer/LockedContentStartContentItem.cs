using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class LockedContentStartContentItem : AbstractContentItem
	{
		private ILockedContentProperties _Properties;

		public ILockedContentProperties Properties
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

		public LockedContentStartContentItem(ILockedContentProperties properties)
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
			LockedContentStartContentItem lockedContentStartContentItem = (LockedContentStartContentItem)obj;
			if (_Properties != null)
			{
				return _Properties.Equals(lockedContentStartContentItem._Properties);
			}
			return _Properties == lockedContentStartContentItem._Properties;
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
			output.LockedContentStart(_Properties);
		}
	}
}
