using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class Segment : AbstractMarkerWithContent, ISegment, IAbstractMarker, IAbstractMarkupData, ICloneable, ISupportsUniqueId, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable
	{
		private ISegmentPairProperties _Properties;

		public override bool CanBeSplit => false;

		public ISegmentPairProperties Properties
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

		public IParagraphUnit ParentParagraphUnit => base.ParentParagraph?.Parent;

		public bool IsReadOnly
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public Segment()
		{
		}

		protected Segment(Segment other)
			: base(other)
		{
			_Properties = other._Properties;
		}

		public Segment(params IAbstractMarkupData[] markupData)
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
			Segment segment = (Segment)obj;
			if (!base.Equals((object)segment))
			{
				return false;
			}
			if (_Properties == null != (segment._Properties == null))
			{
				return false;
			}
			if (_Properties != null && !_Properties.Equals(segment._Properties))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0);
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitSegment(this);
		}

		public override object Clone()
		{
			return new Segment(this);
		}

		public override IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			throw new NotSplittableException(StringResources.Segment_NoSplitError);
		}

		public new IEnumerator GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
