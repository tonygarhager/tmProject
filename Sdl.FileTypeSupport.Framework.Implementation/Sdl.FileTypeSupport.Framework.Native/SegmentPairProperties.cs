using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class SegmentPairProperties : ISegmentPairProperties, ICloneable, ISupportsPersistenceId
	{
		private SegmentId _Id;

		private ITranslationOrigin _TranslationOrigin;

		private ConfirmationLevel _ConfirmationLevel;

		private bool _IsLocked;

		[NonSerialized]
		private int _persistenceId;

		public SegmentId Id
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

		public ITranslationOrigin TranslationOrigin
		{
			get
			{
				return _TranslationOrigin;
			}
			set
			{
				_TranslationOrigin = value;
			}
		}

		public ConfirmationLevel ConfirmationLevel
		{
			get
			{
				return _ConfirmationLevel;
			}
			set
			{
				_ConfirmationLevel = value;
			}
		}

		public bool IsLocked
		{
			get
			{
				return _IsLocked;
			}
			set
			{
				_IsLocked = value;
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

		public SegmentPairProperties()
		{
		}

		protected SegmentPairProperties(SegmentPairProperties other)
		{
			_Id = other._Id;
			if (other._TranslationOrigin != null)
			{
				_TranslationOrigin = (ITranslationOrigin)other._TranslationOrigin.Clone();
			}
			_ConfirmationLevel = other._ConfirmationLevel;
			_IsLocked = other._IsLocked;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SegmentPairProperties segmentPairProperties = (SegmentPairProperties)obj;
			if (_Id != segmentPairProperties._Id)
			{
				return false;
			}
			if (_TranslationOrigin == null != (segmentPairProperties._TranslationOrigin == null))
			{
				return false;
			}
			if (_TranslationOrigin != null && !_TranslationOrigin.Equals(segmentPairProperties._TranslationOrigin))
			{
				return false;
			}
			if (_ConfirmationLevel != segmentPairProperties._ConfirmationLevel)
			{
				return false;
			}
			if (_IsLocked != segmentPairProperties._IsLocked)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _Id.GetHashCode() ^ _ConfirmationLevel.GetHashCode() ^ ((_TranslationOrigin != null) ? _TranslationOrigin.GetHashCode() : 0) ^ _IsLocked.GetHashCode();
		}

		public object Clone()
		{
			return new SegmentPairProperties(this);
		}
	}
}
