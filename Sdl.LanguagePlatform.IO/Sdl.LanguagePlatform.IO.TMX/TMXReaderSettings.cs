using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.IO.TMX
{
	public class TMXReaderSettings
	{
		private bool _ValidateAgainstSchema;

		private TUStreamContext _Context;

		private bool _ResolveNeutralCultures;

		private bool _SkipUnknownCultures;

		private bool _PlainText;

		private ImportSettings.ImportTUProcessingMode _CleanupMode;

		private IDictionary<FieldIdentifier, FieldIdentifier> _FieldIdentifierMappings;

		public bool ValidateAgainstSchema
		{
			get
			{
				return _ValidateAgainstSchema;
			}
			set
			{
				_ValidateAgainstSchema = value;
			}
		}

		public TUStreamContext Context => _Context;

		public bool ResolveNeutralCultures
		{
			get
			{
				return _ResolveNeutralCultures;
			}
			set
			{
				_ResolveNeutralCultures = value;
			}
		}

		public bool SkipUnknownCultures
		{
			get
			{
				return _SkipUnknownCultures;
			}
			set
			{
				_SkipUnknownCultures = value;
			}
		}

		public bool PlainText
		{
			get
			{
				return _PlainText;
			}
			set
			{
				_PlainText = value;
			}
		}

		public ImportSettings.ImportTUProcessingMode CleanupMode
		{
			get
			{
				return _CleanupMode;
			}
			set
			{
				_CleanupMode = value;
			}
		}

		public IDictionary<FieldIdentifier, FieldIdentifier> FieldIdentifierMappings
		{
			get
			{
				return _FieldIdentifierMappings;
			}
			set
			{
				_FieldIdentifierMappings = value;
			}
		}

		public TMXReaderSettings()
			: this(null, validateAgainstSchema: true, resolveNeutralCultures: true, plainText: false)
		{
		}

		public TMXReaderSettings(TUStreamContext context)
			: this(context, validateAgainstSchema: true, resolveNeutralCultures: true, plainText: false)
		{
		}

		public TMXReaderSettings(TUStreamContext context, bool validateAgainstSchema, bool resolveNeutralCultures, bool plainText)
		{
			_Context = (context ?? new TUStreamContext());
			_ValidateAgainstSchema = validateAgainstSchema;
			_ResolveNeutralCultures = resolveNeutralCultures;
			_PlainText = plainText;
			_SkipUnknownCultures = false;
			_CleanupMode = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly;
		}
	}
}
