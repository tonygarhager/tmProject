using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.IO.TMX
{
	public class TMXStartOfInputEvent : StartOfInputEvent
	{
		private header _Header;

		private TranslationUnitFormat _Flavor;

		private FieldDefinitions _Fields;

		private BuiltinRecognizers _BuiltinRecognizers;

		private string _TMName;

		private TokenizerFlags _TokenizerFlags = TokenizerFlags.DefaultFlags;

		private WordCountFlags _WordCountFlags;

		private bool _UsesIdContextMatch;

		private TextContextMatchType _TextContextMatchType = TextContextMatchType.PrecedingSourceAndTarget;

		private bool _IncludesContextContent;

		public header Header => _Header;

		public FieldDefinitions Fields
		{
			get
			{
				return _Fields;
			}
			set
			{
				_Fields = value;
			}
		}

		public BuiltinRecognizers BuiltinRecognizers
		{
			get
			{
				return _BuiltinRecognizers;
			}
			set
			{
				_BuiltinRecognizers = value;
			}
		}

		public string TMName
		{
			get
			{
				return _TMName;
			}
			set
			{
				_TMName = value;
			}
		}

		public TokenizerFlags TokenizerFlags
		{
			get
			{
				return _TokenizerFlags;
			}
			set
			{
				_TokenizerFlags = value;
			}
		}

		public WordCountFlags WordCountFlags
		{
			get
			{
				return _WordCountFlags;
			}
			set
			{
				_WordCountFlags = value;
			}
		}

		public bool UsesIdContextMatch
		{
			get
			{
				return _UsesIdContextMatch;
			}
			set
			{
				_UsesIdContextMatch = value;
			}
		}

		public bool IncludesContextContent
		{
			get
			{
				return _IncludesContextContent;
			}
			set
			{
				_IncludesContextContent = value;
			}
		}

		public TextContextMatchType TextContextMatchType
		{
			get
			{
				return _TextContextMatchType;
			}
			set
			{
				_TextContextMatchType = value;
			}
		}

		public TMXStartOfInputEvent()
		{
			_Header = null;
			_Flavor = TranslationUnitFormat.SDLTradosStudio2009;
			_Fields = null;
		}

		public TMXStartOfInputEvent(string fileName, header hdr, TranslationUnitFormat flavor)
		{
			_Header = hdr;
			_Flavor = flavor;
			if (string.IsNullOrEmpty(hdr.creationdate) || !TMXConversions.TryTMXToDateTime(hdr.creationdate, out _CreationDate))
			{
				_CreationDate = DateTime.Now.ToUniversalTime();
			}
			base.SourceCultureName = hdr.srclang;
		}
	}
}
