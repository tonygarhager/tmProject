using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class LocationMarker : AbstractMarkupData, ILocationMarker, IAbstractMarker, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		private LocationMarkerId _MarkerId;

		public LocationMarkerId MarkerId
		{
			get
			{
				return _MarkerId;
			}
			set
			{
				_MarkerId = value;
			}
		}

		public LocationMarker()
		{
			_MarkerId = new LocationMarkerId();
		}

		public LocationMarker(LocationMarkerId id)
		{
			_MarkerId = id;
		}

		public LocationMarker(LocationMarker other)
			: base(other)
		{
			_MarkerId = (LocationMarkerId)other._MarkerId.Clone();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			LocationMarker locationMarker = (LocationMarker)obj;
			if (_MarkerId == null != (locationMarker._MarkerId == null))
			{
				return false;
			}
			if (_MarkerId != null && !_MarkerId.Equals(locationMarker._MarkerId))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (_MarkerId == null)
			{
				return 0;
			}
			return _MarkerId.GetHashCode();
		}

		public override string ToString()
		{
			return string.Empty;
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			visitor.VisitLocationMarker(this);
		}

		public override object Clone()
		{
			return new LocationMarker(this);
		}
	}
}
