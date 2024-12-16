using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer
{
	[Serializable]
	public class LocationMarkContentItem : AbstractContentItem
	{
		private LocationMarkerId _markerId;

		public LocationMarkerId MarkerId
		{
			get
			{
				return _markerId;
			}
			set
			{
				_markerId = value;
			}
		}

		public LocationMarkContentItem(LocationMarkerId markerId)
		{
			_markerId = markerId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			LocationMarkContentItem locationMarkContentItem = (LocationMarkContentItem)obj;
			if (_markerId != null)
			{
				return _markerId.Equals(locationMarkContentItem._markerId);
			}
			return _markerId == locationMarkContentItem._markerId;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_markerId != null) ? _markerId.GetHashCode() : 0);
		}

		public override void Invoke(IAbstractNativeContentHandler output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.LocationMark(_markerId);
		}
	}
}
