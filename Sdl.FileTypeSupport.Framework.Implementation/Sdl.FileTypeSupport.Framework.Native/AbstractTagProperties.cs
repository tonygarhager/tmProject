using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public abstract class AbstractTagProperties : AbstractBasicTagProperties, IAbstractTagProperties, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable
	{
		private List<ISubSegmentProperties> _lazyLocalizableContent;

		private TagId _TagId;

		public List<ISubSegmentProperties> LocalizableContent
		{
			get
			{
				return _lazyLocalizableContent;
			}
			set
			{
				_lazyLocalizableContent = value;
				SortSubSegments();
			}
		}

		IEnumerable<ISubSegmentProperties> IAbstractTagProperties.LocalizableContent
		{
			get
			{
				if (_lazyLocalizableContent != null)
				{
					return _lazyLocalizableContent;
				}
				return new List<ISubSegmentProperties>();
			}
		}

		public TagId TagId
		{
			get
			{
				return _TagId;
			}
			set
			{
				_TagId = value;
			}
		}

		public bool HasLocalizableContent
		{
			get
			{
				if (_lazyLocalizableContent == null)
				{
					return false;
				}
				return _lazyLocalizableContent.Count > 0;
			}
		}

		protected AbstractTagProperties()
		{
		}

		protected AbstractTagProperties(AbstractTagProperties other)
			: base(other)
		{
			if (other._lazyLocalizableContent != null)
			{
				foreach (ISubSegmentProperties item in other._lazyLocalizableContent)
				{
					AddSubSegment((ISubSegmentProperties)item.Clone());
				}
			}
			_TagId = other._TagId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AbstractTagProperties abstractTagProperties = (AbstractTagProperties)obj;
			if (!base.Equals((object)abstractTagProperties))
			{
				return false;
			}
			if (abstractTagProperties.HasLocalizableContent != HasLocalizableContent)
			{
				return false;
			}
			if (HasLocalizableContent)
			{
				if (LocalizableContent.Count != abstractTagProperties.LocalizableContent.Count)
				{
					return false;
				}
				for (int i = 0; i < LocalizableContent.Count; i++)
				{
					if (!LocalizableContent[i].Equals(abstractTagProperties.LocalizableContent[i]))
					{
						return false;
					}
				}
			}
			if (abstractTagProperties.TagId != TagId)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (HasLocalizableContent)
			{
				foreach (ISubSegmentProperties item in _lazyLocalizableContent)
				{
					num ^= item.GetHashCode();
				}
			}
			return base.GetHashCode() ^ num ^ _TagId.GetHashCode();
		}

		public void SortSubSegments()
		{
			if (_lazyLocalizableContent != null)
			{
				_lazyLocalizableContent.Sort((ISubSegmentProperties first, ISubSegmentProperties second) => first.StartOffset.CompareTo(second.StartOffset));
			}
		}

		public void AddSubSegment(ISubSegmentProperties subSegmentInfo)
		{
			if (subSegmentInfo == null)
			{
				throw new ArgumentNullException("subSegmentInfo");
			}
			if (_lazyLocalizableContent == null)
			{
				_lazyLocalizableContent = new List<ISubSegmentProperties>();
			}
			_lazyLocalizableContent.Add(subSegmentInfo);
			SortSubSegments();
		}

		public void AddSubSegments(IEnumerable<ISubSegmentProperties> subSegments)
		{
			if (_lazyLocalizableContent == null)
			{
				_lazyLocalizableContent = new List<ISubSegmentProperties>();
			}
			_lazyLocalizableContent.AddRange(subSegments);
			SortSubSegments();
		}

		public void RemoveSubSegment(ISubSegmentProperties subSegment)
		{
			if (_lazyLocalizableContent != null)
			{
				_lazyLocalizableContent.Remove(subSegment);
			}
		}

		public void ClearSubSegments()
		{
			_lazyLocalizableContent = null;
		}
	}
}
