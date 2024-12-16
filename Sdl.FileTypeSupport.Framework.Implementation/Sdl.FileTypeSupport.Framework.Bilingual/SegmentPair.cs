using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class SegmentPair : ISegmentPair, ISupportsPersistenceId
	{
		private ISegment _Source;

		private ISegment _Target;

		[NonSerialized]
		private int _persistenceId;

		public ISegment Source
		{
			get
			{
				return _Source;
			}
			set
			{
				_Source = value;
			}
		}

		public ISegment Target
		{
			get
			{
				return _Target;
			}
			set
			{
				_Target = value;
			}
		}

		public ISegmentPairProperties Properties
		{
			get
			{
				if (_Source == null)
				{
					return null;
				}
				return _Source.Properties;
			}
			set
			{
				if (_Source != null)
				{
					_Source.Properties = value;
				}
				if (_Target != null)
				{
					_Target.Properties = value;
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

		public SegmentPair()
		{
		}

		public SegmentPair(ISegment source, ISegment target)
		{
			_Source = source;
			_Target = target;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SegmentPair segmentPair = (SegmentPair)obj;
			if (_Source == null != (segmentPair._Source == null))
			{
				return false;
			}
			if (_Source != null && !_Source.Equals(segmentPair._Source))
			{
				return false;
			}
			if (_Target == null != (segmentPair._Target == null))
			{
				return false;
			}
			if (_Target != null && !_Target.Equals(segmentPair._Target))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_Source != null) ? _Source.GetHashCode() : 0) ^ ((_Target != null) ? _Target.GetHashCode() : 0);
		}
	}
}
