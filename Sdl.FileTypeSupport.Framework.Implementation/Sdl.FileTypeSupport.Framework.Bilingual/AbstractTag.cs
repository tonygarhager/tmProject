using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public abstract class AbstractTag : AbstractDataContent, IAbstractTag, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		private List<ISubSegmentReference> _lazySubSegments;

		private IAbstractTagProperties _TagProperties;

		private IRevisionProperties _RevisionProperties;

		public virtual List<ISubSegmentReference> SubSegments
		{
			get
			{
				return _lazySubSegments;
			}
			set
			{
				_lazySubSegments = value;
			}
		}

		IEnumerable<ISubSegmentReference> IAbstractTag.SubSegments
		{
			get
			{
				List<ISubSegmentReference> list = SubSegments;
				if (list == null)
				{
					list = new List<ISubSegmentReference>();
				}
				return list;
			}
		}

		public virtual IAbstractTagProperties TagProperties
		{
			get
			{
				return _TagProperties;
			}
			protected set
			{
				_TagProperties = value;
			}
		}

		public virtual IRevisionProperties RevisionProperties
		{
			get
			{
				return _RevisionProperties;
			}
			set
			{
				_RevisionProperties = value;
			}
		}

		public bool HasSubSegmentReferences
		{
			get
			{
				if (_lazySubSegments == null)
				{
					return false;
				}
				return _lazySubSegments.Count > 0;
			}
		}

		protected AbstractTag()
		{
		}

		protected AbstractTag(AbstractTag other)
			: base(other)
		{
			if (other._lazySubSegments != null)
			{
				_lazySubSegments = new List<ISubSegmentReference>();
				foreach (ISubSegmentReference lazySubSegment in other._lazySubSegments)
				{
					_lazySubSegments.Add((ISubSegmentReference)lazySubSegment.Clone());
				}
			}
			_TagProperties = other._TagProperties;
			_RevisionProperties = other._RevisionProperties;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AbstractTag abstractTag = (AbstractTag)obj;
			if (!base.Equals((object)abstractTag))
			{
				return false;
			}
			if (_lazySubSegments == null != (abstractTag._lazySubSegments == null))
			{
				return false;
			}
			if (_lazySubSegments != null)
			{
				if (_lazySubSegments.Count != abstractTag._lazySubSegments.Count)
				{
					return false;
				}
				for (int i = 0; i < _lazySubSegments.Count; i++)
				{
					if (!_lazySubSegments[i].Equals(abstractTag._lazySubSegments[i]))
					{
						return false;
					}
				}
			}
			if (_TagProperties == null != (abstractTag._TagProperties == null))
			{
				return false;
			}
			if (_TagProperties != null && !_TagProperties.Equals(abstractTag._TagProperties))
			{
				return false;
			}
			if (_RevisionProperties == null != (abstractTag._RevisionProperties == null))
			{
				return false;
			}
			if (_RevisionProperties != null && !_RevisionProperties.Equals(abstractTag._RevisionProperties))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (_lazySubSegments != null)
			{
				foreach (ISubSegmentReference lazySubSegment in _lazySubSegments)
				{
					num ^= lazySubSegment.GetHashCode();
				}
			}
			return base.GetHashCode() ^ num ^ ((_TagProperties != null) ? _TagProperties.GetHashCode() : 0) ^ ((_RevisionProperties != null) ? _RevisionProperties.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (_TagProperties != null)
			{
				return _TagProperties.ToString();
			}
			return string.Empty;
		}

		public virtual void AddSubSegmentReference(ISubSegmentReference subSegmentReference)
		{
			if (_lazySubSegments == null)
			{
				_lazySubSegments = new List<ISubSegmentReference>();
			}
			_lazySubSegments.Add(subSegmentReference);
		}

		public virtual void AddSubSegmentReferences(IEnumerable<ISubSegmentReference> subSegmentReferences)
		{
			if (_lazySubSegments == null)
			{
				_lazySubSegments = new List<ISubSegmentReference>();
			}
			_lazySubSegments.AddRange(subSegmentReferences);
		}

		public virtual void RemoveSubSegmentReference(ISubSegmentReference subSegmentReference)
		{
			if (_lazySubSegments != null)
			{
				_lazySubSegments.Remove(subSegmentReference);
			}
		}

		public virtual void ClearSubSegmentReferences()
		{
			_lazySubSegments = null;
		}
	}
}
