using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class ParagraphUnitProperties : IParagraphUnitProperties, ICloneable, ISupportsPersistenceId
	{
		private ParagraphUnitId _ParagraphUnitId;

		private IContextProperties _Contexts;

		private LockTypeFlags _LockType;

		private ICommentProperties _Comments;

		private SourceCount _SourceCount;

		[NonSerialized]
		private int _persistenceId;

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

		public IContextProperties Contexts
		{
			get
			{
				return _Contexts;
			}
			set
			{
				_Contexts = value;
			}
		}

		public LockTypeFlags LockType
		{
			get
			{
				return _LockType;
			}
			set
			{
				_LockType = value;
			}
		}

		public ICommentProperties Comments
		{
			get
			{
				return _Comments;
			}
			set
			{
				_Comments = value;
			}
		}

		public SourceCount SourceCount
		{
			get
			{
				return _SourceCount;
			}
			set
			{
				_SourceCount = value;
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

		public ParagraphUnitProperties()
		{
		}

		protected ParagraphUnitProperties(ParagraphUnitProperties other)
		{
			_ParagraphUnitId = other._ParagraphUnitId;
			_LockType = other._LockType;
			if (other._Contexts != null)
			{
				_Contexts = (IContextProperties)other._Contexts.Clone();
			}
			if (other._Comments != null)
			{
				_Comments = (ICommentProperties)other._Comments.Clone();
			}
			if (other._SourceCount != null)
			{
				_SourceCount = (SourceCount)other._SourceCount.Clone();
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			ParagraphUnitProperties paragraphUnitProperties = (ParagraphUnitProperties)obj;
			if (_ParagraphUnitId != paragraphUnitProperties._ParagraphUnitId)
			{
				return false;
			}
			if (_Contexts == null != (paragraphUnitProperties.Contexts == null))
			{
				return false;
			}
			if (_Contexts != null && !_Contexts.Equals(paragraphUnitProperties._Contexts))
			{
				return false;
			}
			if (_LockType != paragraphUnitProperties._LockType)
			{
				return false;
			}
			if (_Comments == null != (paragraphUnitProperties._Comments == null))
			{
				return false;
			}
			if (_Comments != null && _Comments.Xml != paragraphUnitProperties._Comments.Xml)
			{
				return false;
			}
			if (_SourceCount == null != (paragraphUnitProperties._SourceCount == null))
			{
				return false;
			}
			if (_SourceCount != null && !_SourceCount.Equals(paragraphUnitProperties._SourceCount))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _ParagraphUnitId.GetHashCode() ^ ((_Contexts != null) ? _Contexts.GetHashCode() : 0) ^ _LockType.GetHashCode() ^ ((_Comments != null) ? _Comments.Xml.GetHashCode() : 0) ^ ((_SourceCount != null) ? _SourceCount.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return _ParagraphUnitId.ToString();
		}

		public object Clone()
		{
			return new ParagraphUnitProperties(this);
		}
	}
}
