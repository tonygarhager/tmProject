using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public abstract class AbstractRevisionBase : AbstractMarkerWithContent, IRevisionMarker, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, IAbstractMarker, IAbstractMarkupData, ICloneable
	{
		private IRevisionProperties _Properties;

		public IRevisionProperties Properties
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

		public AbstractRevisionBase()
		{
		}

		protected AbstractRevisionBase(AbstractRevisionBase other)
			: base(other)
		{
			_Properties = other._Properties;
		}

		protected AbstractRevisionBase(AbstractRevisionBase other, int splitBeforeItemIndex)
			: base(other, splitBeforeItemIndex)
		{
			_Properties = other._Properties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AbstractRevisionBase abstractRevisionBase = (AbstractRevisionBase)obj;
			if (!base.Equals((object)abstractRevisionBase))
			{
				return false;
			}
			if (_Properties == null != (abstractRevisionBase._Properties == null))
			{
				return false;
			}
			if (_Properties != null && !_Properties.Equals(abstractRevisionBase._Properties))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0);
		}

		public abstract override void AcceptVisitor(IMarkupDataVisitor visitor);

		public abstract override IAbstractMarkupDataContainer Split(int splitBeforeItemIndex);

		public abstract override object Clone();
	}
}
