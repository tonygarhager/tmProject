using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class LockedContent : AbstractDataContent, ILockedContent, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		private ILockedContentProperties _Properties;

		private ILockedContainer _Content;

		public ILockedContainer Content
		{
			get
			{
				return _Content;
			}
			set
			{
				_Content = value;
			}
		}

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

		public LockedContent()
		{
			_Content = new LockedContainer();
			_Content.LockedContent = this;
		}

		protected LockedContent(LockedContent other)
			: base(other)
		{
			if (other._Properties != null)
			{
				_Properties = (ILockedContentProperties)other._Properties.Clone();
			}
			if (other._Content != null)
			{
				_Content = (ILockedContainer)other._Content.Clone();
				_Content.LockedContent = this;
			}
		}

		public LockedContent(params IAbstractMarkupData[] markupData)
		{
			_Content = new LockedContainer();
			_Content.LockedContent = this;
			ReadMarkupData(markupData);
		}

		private void ReadMarkupData(IList<IAbstractMarkupData> markupDataList)
		{
			foreach (IAbstractMarkupData markupData in markupDataList)
			{
				_Content.Add(markupData);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			LockedContent lockedContent = (LockedContent)obj;
			if (!base.Equals((object)lockedContent))
			{
				return false;
			}
			if (_Properties == null != (lockedContent._Properties == null))
			{
				return false;
			}
			if (_Properties != null && !_Properties.Equals(lockedContent._Properties))
			{
				return false;
			}
			if (_Content == null != (lockedContent._Content == null))
			{
				return false;
			}
			if (_Content != null && !_Content.Equals(lockedContent._Content))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0) ^ ((_Content != null) ? _Content.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return _Content.ToString();
		}

		public override object Clone()
		{
			return new LockedContent(this);
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitLockedContent(this);
		}
	}
}
