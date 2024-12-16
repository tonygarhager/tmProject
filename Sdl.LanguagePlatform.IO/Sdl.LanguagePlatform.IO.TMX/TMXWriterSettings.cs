using Sdl.LanguagePlatform.TranslationMemory;
using System.Text;

namespace Sdl.LanguagePlatform.IO.TMX
{
	public class TMXWriterSettings
	{
		private Encoding _Encoding;

		private TranslationUnitFormat _TargetFormat;

		private bool _PlainText;

		private bool _ReplaceInvalidCharacters;

		private char _ReplacementChar;

		public Encoding Encoding => _Encoding;

		public TranslationUnitFormat TargetFormat
		{
			get
			{
				return _TargetFormat;
			}
			set
			{
				_TargetFormat = value;
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

		public bool ReplaceInvalidCharacters
		{
			get
			{
				return _ReplaceInvalidCharacters;
			}
			set
			{
				_ReplaceInvalidCharacters = value;
			}
		}

		public char ReplacementCharacter
		{
			get
			{
				return _ReplacementChar;
			}
			set
			{
				_ReplacementChar = value;
			}
		}

		public TMXWriterSettings()
			: this(Encoding.UTF8)
		{
		}

		public TMXWriterSettings(Encoding encoding)
		{
			_Encoding = encoding;
			_TargetFormat = TranslationUnitFormat.SDLTradosStudio2009;
			_PlainText = false;
			_ReplaceInvalidCharacters = false;
		}
	}
}
