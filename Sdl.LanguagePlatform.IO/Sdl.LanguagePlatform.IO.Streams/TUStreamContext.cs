using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.IO.Streams
{
	public class TUStreamContext
	{
		private FieldDefinitions _Fields;

		private CultureInfo _SourceCulture;

		private CultureInfo _TargetCulture;

		private bool _CheckMatchingSublanguages;

		private bool _MayAddNewFields;

		public FieldDefinitions FieldDefinitions => _Fields;

		public CultureInfo SourceCulture => _SourceCulture;

		public CultureInfo TargetCulture => _TargetCulture;

		public bool CheckMatchingSublanguages
		{
			get
			{
				return _CheckMatchingSublanguages;
			}
			set
			{
				_CheckMatchingSublanguages = value;
			}
		}

		public bool MayAddNewFields
		{
			get
			{
				return _MayAddNewFields;
			}
			set
			{
				_MayAddNewFields = value;
			}
		}

		public TUStreamContext()
			: this(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture, null)
		{
		}

		public TUStreamContext(LanguagePair languageDirection)
			: this(languageDirection.SourceCulture, languageDirection.TargetCulture, null)
		{
		}

		public TUStreamContext(LanguagePair languageDirection, FieldDefinitions fields)
			: this(languageDirection.SourceCulture, languageDirection.TargetCulture, fields)
		{
		}

		public TUStreamContext(CultureInfo sourceCulture, CultureInfo targetCulture)
			: this(sourceCulture, targetCulture, null)
		{
		}

		public TUStreamContext(CultureInfo sourceCulture, CultureInfo targetCulture, FieldDefinitions fields)
		{
			if (sourceCulture == null || targetCulture == null)
			{
				throw new ArgumentNullException("source or target culture can't be null");
			}
			_SourceCulture = sourceCulture;
			_TargetCulture = targetCulture;
			if (fields == null)
			{
				_Fields = new FieldDefinitions();
			}
			else
			{
				_Fields = new FieldDefinitions(fields);
			}
			_MayAddNewFields = true;
		}
	}
}
