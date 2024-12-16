using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class ParagraphUnit : IParagraphUnit, ICloneable, ISupportsPersistenceId
	{
		private IParagraphUnitProperties _Properties = new ParagraphUnitProperties();

		private IParagraph _Source;

		private IParagraph _Target;

		[NonSerialized]
		private int _persistenceId;

		public IParagraphUnitProperties Properties
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

		public IParagraph Source
		{
			get
			{
				return _Source;
			}
			set
			{
				IParagraph source = _Source;
				_Source = value;
				if (_Source != null)
				{
					_Source.Parent = this;
				}
				if (source != null)
				{
					source.Parent = null;
				}
			}
		}

		public IParagraph Target
		{
			get
			{
				return _Target;
			}
			set
			{
				IParagraph target = _Target;
				_Target = value;
				if (_Target != null)
				{
					_Target.Parent = this;
				}
				if (target != null)
				{
					target.Parent = null;
				}
			}
		}

		public bool IsStructure
		{
			get
			{
				if (_Properties == null)
				{
					return false;
				}
				return (_Properties.LockType & LockTypeFlags.Structure) != 0;
			}
		}

		public IEnumerable<ISegmentPair> SegmentPairs
		{
			get
			{
				foreach (Location location in _Source.Locations)
				{
					ISegment segment = location.ItemAtLocation as ISegment;
					if (segment != null)
					{
						ISegment targetSegment = GetTargetSegment(segment.Properties.Id);
						yield return new SegmentPair(segment, targetSegment);
					}
				}
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

		public ParagraphUnit()
		{
			_Source = new Paragraph();
			_Source.Parent = this;
			_Target = new Paragraph();
			_Target.Parent = this;
		}

		protected ParagraphUnit(ParagraphUnit other)
			: this()
		{
			_Properties = other._Properties;
			Source = (Paragraph)other.Source.Clone();
			Target = (Paragraph)other.Target.Clone();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			ParagraphUnit paragraphUnit = (ParagraphUnit)obj;
			if (_Properties == null != (paragraphUnit._Properties == null))
			{
				return false;
			}
			if (_Properties != null && !_Properties.Equals(paragraphUnit._Properties))
			{
				return false;
			}
			if (_Source == null != (paragraphUnit._Source == null))
			{
				return false;
			}
			if (_Source != null && !_Source.Equals(paragraphUnit._Source))
			{
				return false;
			}
			if (_Target == null != (paragraphUnit._Target == null))
			{
				return false;
			}
			if (_Target != null && !_Target.Equals(paragraphUnit._Target))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_Properties != null) ? _Properties.GetHashCode() : 0) ^ ((_Source != null) ? _Source.GetHashCode() : 0) ^ ((_Target != null) ? _Target.GetHashCode() : 0);
		}

		public static ISegment FindSegment(SegmentId id, IAbstractMarkupDataContainer collection)
		{
			return collection.Find(delegate(IAbstractMarkupData item)
			{
				ISegment segment = item as ISegment;
				return segment != null && segment.Properties != null && segment.Properties.Id == id;
			}) as ISegment;
		}

		public static Location FindSegmentLocation(SegmentId id, IAbstractMarkupDataContainer collection)
		{
			return collection.Find(delegate(Location location)
			{
				ISegment segment = location.ItemAtLocation as ISegment;
				return segment != null && segment.Properties != null && segment.Properties.Id == id;
			});
		}

		public Location GetTargetSegmentLocation(SegmentId id)
		{
			return FindSegmentLocation(id, _Target);
		}

		public Location GetSourceSegmentLocation(SegmentId id)
		{
			return FindSegmentLocation(id, _Source);
		}

		public ISegment GetSourceSegment(SegmentId id)
		{
			return FindSegment(id, _Source);
		}

		public ISegment GetTargetSegment(SegmentId id)
		{
			return FindSegment(id, _Target);
		}

		public ISegmentPair GetSegmentPair(SegmentId id)
		{
			ISegment segment = FindSegment(id, _Source);
			if (segment == null)
			{
				return null;
			}
			return new SegmentPair(segment, FindSegment(id, _Target));
		}

		public object Clone()
		{
			return new ParagraphUnit(this);
		}
	}
}
