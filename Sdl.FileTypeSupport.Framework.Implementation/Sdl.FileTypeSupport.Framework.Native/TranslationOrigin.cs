using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class TranslationOrigin : AbstractMetaDataContainer, ITranslationOrigin, IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		private string _OriginType;

		private string _OriginSystem;

		private ITranslationOrigin _OriginBeforeAdaptation;

		private byte _MatchPercent;

		private bool _IsStructureContextMatch;

		private bool _IsSIDContextMatch;

		private TextContextMatchLevel _TextContextMatchLevel;

		private RepetitionId _RepetitionTableId;

		[NonSerialized]
		private int _persistenceId;

		public string OriginType
		{
			get
			{
				return _OriginType;
			}
			set
			{
				_OriginType = value;
			}
		}

		public string OriginSystem
		{
			get
			{
				return _OriginSystem;
			}
			set
			{
				_OriginSystem = value;
			}
		}

		public ITranslationOrigin OriginBeforeAdaptation
		{
			get
			{
				return _OriginBeforeAdaptation;
			}
			set
			{
				_OriginBeforeAdaptation = value;
			}
		}

		public byte MatchPercent
		{
			get
			{
				return _MatchPercent;
			}
			set
			{
				_MatchPercent = value;
			}
		}

		public RepetitionId RepetitionTableId
		{
			get
			{
				return _RepetitionTableId;
			}
			set
			{
				_RepetitionTableId = value;
			}
		}

		public bool IsRepeated => _RepetitionTableId.Id != null;

		public bool IsStructureContextMatch
		{
			get
			{
				return _IsStructureContextMatch;
			}
			set
			{
				_IsStructureContextMatch = value;
			}
		}

		public TextContextMatchLevel TextContextMatchLevel
		{
			get
			{
				return _TextContextMatchLevel;
			}
			set
			{
				_TextContextMatchLevel = value;
			}
		}

		public bool IsSIDContextMatch
		{
			get
			{
				return _IsSIDContextMatch;
			}
			set
			{
				_IsSIDContextMatch = value;
			}
		}

		public string OriginalTranslationHash
		{
			get
			{
				return GetMetaData("SDL:OriginalTranslationHash");
			}
			set
			{
				SetMetaData("SDL:OriginalTranslationHash", value);
			}
		}

		public string this[string key]
		{
			get
			{
				return GetMetaData(key);
			}
			set
			{
				SetMetaData(key, value);
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

		public TranslationOrigin()
		{
		}

		protected TranslationOrigin(TranslationOrigin other)
			: base(other)
		{
			_OriginType = other._OriginType;
			_OriginSystem = other._OriginSystem;
			_OriginBeforeAdaptation = other._OriginBeforeAdaptation;
			_MatchPercent = other._MatchPercent;
			_IsStructureContextMatch = other._IsStructureContextMatch;
			_TextContextMatchLevel = other._TextContextMatchLevel;
			_RepetitionTableId = other._RepetitionTableId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			TranslationOrigin translationOrigin = (TranslationOrigin)obj;
			if (!base.Equals((object)translationOrigin))
			{
				return false;
			}
			if (_OriginType != translationOrigin._OriginType)
			{
				return false;
			}
			if (_OriginSystem != translationOrigin.OriginSystem)
			{
				return false;
			}
			if (_OriginBeforeAdaptation == null != (translationOrigin._OriginBeforeAdaptation == null))
			{
				return false;
			}
			if (_OriginBeforeAdaptation != null && !_OriginBeforeAdaptation.Equals(translationOrigin._OriginBeforeAdaptation))
			{
				return false;
			}
			if (_MatchPercent != translationOrigin._MatchPercent)
			{
				return false;
			}
			if (_IsStructureContextMatch != translationOrigin._IsStructureContextMatch)
			{
				return false;
			}
			if (_TextContextMatchLevel != translationOrigin._TextContextMatchLevel)
			{
				return false;
			}
			if (_RepetitionTableId != translationOrigin._RepetitionTableId)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_OriginType != null) ? _OriginType.GetHashCode() : 0) ^ ((_OriginSystem != null) ? _OriginSystem.GetHashCode() : 0) ^ ((_OriginBeforeAdaptation != null) ? _OriginBeforeAdaptation.GetHashCode() : 0) ^ _MatchPercent.GetHashCode() ^ _IsStructureContextMatch.GetHashCode() ^ _TextContextMatchLevel.GetHashCode() ^ _RepetitionTableId.GetHashCode();
		}

		public object Clone()
		{
			return new TranslationOrigin(this);
		}
	}
}
