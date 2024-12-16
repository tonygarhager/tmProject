using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class TranslationUnit : DbObject, IComparer<TranslationUnit>
	{
		private DateTime _creationDate;

		private DateTime _changeDate;

		private DateTime _lastUsedDate;

		private List<AttributeValue> _attributes;

		public Segment Source
		{
			get;
			set;
		}

		public Segment Target
		{
			get;
			set;
		}

		public DateTime CreationDate
		{
			get
			{
				return _creationDate;
			}
			set
			{
				_creationDate = DateTimeUtilities.Normalize(value);
			}
		}

		public string CreationUser
		{
			get;
			set;
		}

		public DateTime ChangeDate
		{
			get
			{
				return _changeDate;
			}
			set
			{
				_changeDate = DateTimeUtilities.Normalize(value);
			}
		}

		public int Flags
		{
			get;
			set;
		}

		public string ChangeUser
		{
			get;
			set;
		}

		public DateTime LastUsedDate
		{
			get
			{
				return _lastUsedDate;
			}
			set
			{
				_lastUsedDate = DateTimeUtilities.Normalize(value);
			}
		}

		public string LastUsedUser
		{
			get;
			set;
		}

		public int UsageCounter
		{
			get;
			set;
		}

		public List<AttributeValue> Attributes
		{
			get
			{
				return _attributes ?? (_attributes = new List<AttributeValue>());
			}
			set
			{
				_attributes = value;
			}
		}

		public TuContexts Contexts
		{
			get;
			set;
		}

		public TuIdContexts IdContexts
		{
			get;
			set;
		}

		public byte[] AlignmentData
		{
			get;
			set;
		}

		public DateTime? AlignModelDate
		{
			get;
			set;
		}

		public DateTime? InsertDate
		{
			get;
			set;
		}

		public int SerializationVersion
		{
			get;
			set;
		}

		public TranslationUnitOrigin Origin
		{
			get;
			set;
		}

		public ConfirmationLevel ConfirmationLevel
		{
			get;
			set;
		}

		public TranslationUnitFormat Format
		{
			get;
			set;
		}

		public int TranslationMemoryId
		{
			get;
			set;
		}

		public byte[] SourceTokenData
		{
			get;
			set;
		}

		public byte[] TargetTokenData
		{
			get;
			set;
		}

		public bool AddContext(TuContext context)
		{
			if (Contexts == null)
			{
				Contexts = new TuContexts();
			}
			return Contexts.Add(context);
		}

		public bool AddIdContext(string idcontext)
		{
			if (IdContexts == null)
			{
				IdContexts = new TuIdContexts();
			}
			return IdContexts.Add(idcontext);
		}

		public bool AddContext(long leftSource, long leftTarget)
		{
			return AddContext(new TuContext(leftSource, leftTarget));
		}

		private bool DatesInUtc()
		{
			if (_creationDate.Kind == DateTimeKind.Utc && _changeDate.Kind == DateTimeKind.Utc)
			{
				return _lastUsedDate.Kind == DateTimeKind.Utc;
			}
			return false;
		}

		public TranslationUnit(int tmId, Guid guid, Segment source, Segment target, DateTime crd, string cru, DateTime chd, string chu, DateTime lud, string luu, int uc, int flags, byte[] source_token_data, byte[] target_token_data, byte[] alignmentData, DateTime? lastAlignDate, DateTime? insertDate, int serializationVersion)
			: base(0, guid)
		{
			TranslationMemoryId = tmId;
			Source = source;
			Target = target;
			_creationDate = crd;
			CreationUser = cru;
			_changeDate = chd;
			ChangeUser = chu;
			_lastUsedDate = lud;
			LastUsedUser = luu;
			UsageCounter = uc;
			Flags = flags;
			SourceTokenData = source_token_data;
			TargetTokenData = target_token_data;
			AlignmentData = alignmentData;
			AlignModelDate = lastAlignDate;
			InsertDate = insertDate;
			SerializationVersion = serializationVersion;
		}

		public TranslationUnit(int tmId, Guid guid, Segment source, Segment target, DateTime crd, string cru, DateTime chd, string chu, DateTime lud, string luu, int uc, int flags, byte[] source_token_data, byte[] target_token_data, byte[] alignmentData, DateTime? lastAlignDate, DateTime? insertDate, int serializationVersion, TranslationUnitFormat format, TranslationUnitOrigin origin, ConfirmationLevel confirmationLevel)
			: this(tmId, guid, source, target, crd, cru, chd, chu, lud, luu, uc, flags, source_token_data, target_token_data, alignmentData, lastAlignDate, insertDate, serializationVersion)
		{
			Format = format;
			Origin = origin;
			ConfirmationLevel = confirmationLevel;
		}

		internal TranslationUnit(int id, Guid guid, int tmId, Segment source, Segment target, DateTime crd, string cru, DateTime chd, string chu, DateTime lud, string luu, int uc, int flags, byte[] source_token_data, byte[] target_token_data, byte[] alignmentData, DateTime? lastAlignDate, DateTime? insertDate, int serializationVersion)
			: this(tmId, guid, source, target, crd, cru, chd, chu, lud, luu, uc, flags, source_token_data, target_token_data, alignmentData, lastAlignDate, insertDate, serializationVersion)
		{
			base.Id = id;
		}

		internal TranslationUnit(int id, Guid guid, int tmId, Segment source, Segment target, DateTime crd, string cru, DateTime chd, string chu, DateTime lud, string luu, int uc, int flags, byte[] source_token_data, byte[] target_token_data, byte[] alignmentData, DateTime? lastAlignDate, DateTime? insertDate, int serializationVersion, TranslationUnitFormat format, TranslationUnitOrigin origin, ConfirmationLevel confirmationLevel)
			: this(tmId, guid, source, target, crd, cru, chd, chu, lud, luu, uc, flags, source_token_data, target_token_data, alignmentData, lastAlignDate, insertDate, serializationVersion, format, origin, confirmationLevel)
		{
			base.Id = id;
		}

		public int Compare(TranslationUnit x, TranslationUnit y)
		{
			return x.Id - y.Id;
		}
	}
}
