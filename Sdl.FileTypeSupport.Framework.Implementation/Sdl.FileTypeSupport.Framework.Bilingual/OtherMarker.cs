using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class OtherMarker : AbstractMarkerWithContent, IOtherMarker, IAbstractMarker, IAbstractMarkupData, ICloneable, ISupportsUniqueId, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable
	{
		private string _Id;

		private string _MarkerType;

		public string Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		public string MarkerType
		{
			get
			{
				return _MarkerType;
			}
			set
			{
				_MarkerType = value;
			}
		}

		public OtherMarker()
		{
		}

		protected OtherMarker(OtherMarker other)
			: base(other)
		{
			_Id = other._Id;
			_MarkerType = other._MarkerType;
		}

		protected OtherMarker(OtherMarker other, int splitBeforeItemIndex)
			: base(other, splitBeforeItemIndex)
		{
			_Id = other._Id;
			_MarkerType = other._MarkerType;
		}

		public OtherMarker(params IAbstractMarkupData[] markupData)
		{
			ReadMarkupData(markupData);
		}

		private void ReadMarkupData(IList<IAbstractMarkupData> markupDataList)
		{
			foreach (IAbstractMarkupData markupData in markupDataList)
			{
				Add(markupData);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			OtherMarker otherMarker = (OtherMarker)obj;
			if (!base.Equals((object)otherMarker))
			{
				return false;
			}
			if (_Id != otherMarker._Id)
			{
				return false;
			}
			if (_MarkerType != otherMarker._MarkerType)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Id != null) ? _Id.GetHashCode() : 0) ^ ((_MarkerType != null) ? _MarkerType.GetHashCode() : 0);
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitOtherMarker(this);
		}

		public override object Clone()
		{
			return new OtherMarker(this);
		}

		public override IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			return new OtherMarker(this, splitBeforeItemIndex);
		}
	}
}
