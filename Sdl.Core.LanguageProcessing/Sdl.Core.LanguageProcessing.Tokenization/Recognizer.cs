using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal abstract class Recognizer
	{
		protected TokenType _Type;

		protected int _Priority;

		protected CharacterSet _AdditionalTerminators;

		protected bool _IsFallbackRecognizer;

		protected bool _OverrideFallbackRecognizer;

		protected bool _AutoSubstitutable;

		protected RecognizerSettings _Settings;

		protected CultureInfo _Culture;

		protected Func<string, int, Token, bool> _cultureSpecificTextConstraints;

		public TokenType Type => _Type;

		public bool IsFallbackRecognizer => _IsFallbackRecognizer;

		public bool OverrideFallbackRecognizer
		{
			get
			{
				return _OverrideFallbackRecognizer;
			}
			set
			{
				_OverrideFallbackRecognizer = value;
			}
		}

		public string TokenClassName
		{
			get;
		}

		public string RecognizerName
		{
			get;
		}

		public int Priority => _Priority;

		public bool OnlyIfFollowedByNonwordCharacter
		{
			get;
			set;
		}

		protected CharacterSet AdditionalTerminators
		{
			get
			{
				return _AdditionalTerminators;
			}
			set
			{
				_AdditionalTerminators = value;
			}
		}

		public virtual string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Recognizer0");
			stringBuilder.Append("BOA");
			stringBuilder.Append(_Settings.BreakOnApostrophe);
			stringBuilder.Append("BOD");
			stringBuilder.Append(_Settings.BreakOnDash);
			stringBuilder.Append("BOH");
			stringBuilder.Append(_Settings.BreakOnHyphen);
			stringBuilder.Append(_Type);
			stringBuilder.Append(TokenClassName);
			stringBuilder.Append(_Priority);
			if (_AdditionalTerminators != null)
			{
				stringBuilder.Append(_AdditionalTerminators.Signature());
			}
			stringBuilder.Append("NonWord");
			stringBuilder.Append(OnlyIfFollowedByNonwordCharacter);
			stringBuilder.Append("OverrideFallback");
			stringBuilder.Append(_OverrideFallbackRecognizer);
			return stringBuilder.ToString();
		}

		internal Recognizer(RecognizerSettings settings, TokenType t, int priority, string tokenClassName, string recognizerName, bool autoSubstitutable, CultureInfo culture)
		{
			_Settings = settings;
			_Type = t;
			_Priority = priority;
			TokenClassName = tokenClassName;
			RecognizerName = recognizerName;
			_AdditionalTerminators = null;
			_IsFallbackRecognizer = false;
			_OverrideFallbackRecognizer = false;
			_AutoSubstitutable = autoSubstitutable;
			_Culture = culture;
		}

		public abstract Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength);

		protected bool VerifyContextConstraints(string s, int p, Token t = null)
		{
			if (_cultureSpecificTextConstraints != null)
			{
				return _cultureSpecificTextConstraints(s, p, t);
			}
			return DefaultTextConstraints(s, p);
		}

		internal bool DefaultTextConstraints(string s, int p, bool breakOnCJK = true)
		{
			if (!OnlyIfFollowedByNonwordCharacter || p >= s.Length)
			{
				return true;
			}
			char c = s[p];
			if ((_AdditionalTerminators == null || !_AdditionalTerminators.Contains(c)) && !char.IsWhiteSpace(c) && !char.IsPunctuation(c) && !char.IsSeparator(c) && !CharacterProperties.IsApostrophe(c) && !char.IsSymbol(c))
			{
				if (breakOnCJK)
				{
					return CharacterProperties.IsCJKChar(c);
				}
				return false;
			}
			return true;
		}
	}
}
