using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class SubSegmentReference : ISubSegmentReference, ICloneable, ISupportsPersistenceId
	{
		private ParagraphUnitId _ParagraphUnitId;

		private ISubSegmentProperties _Properties;

		[NonSerialized]
		private int _persistenceId;

		public ISubSegmentProperties Properties
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

		public ParagraphUnitId ParagraphUnitId
		{
			get
			{
				return _ParagraphUnitId;
			}
			set
			{
				_ParagraphUnitId = value;
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

		public SubSegmentReference()
		{
		}

		protected SubSegmentReference(SubSegmentReference other)
		{
			_ParagraphUnitId = other._ParagraphUnitId;
			if (other._Properties != null)
			{
				_Properties = (ISubSegmentProperties)other._Properties.Clone();
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SubSegmentReference subSegmentReference = (SubSegmentReference)obj;
			if (_ParagraphUnitId != subSegmentReference._ParagraphUnitId)
			{
				return false;
			}
			if (_Properties == null != (subSegmentReference._Properties == null))
			{
				return false;
			}
			if (_Properties != null && !_Properties.Equals(subSegmentReference._Properties))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _ParagraphUnitId.GetHashCode() ^ ((_Properties != null) ? _Properties.GetHashCode() : 0);
		}

		public virtual object Clone()
		{
			return new SubSegmentReference(this);
		}
	}
}
