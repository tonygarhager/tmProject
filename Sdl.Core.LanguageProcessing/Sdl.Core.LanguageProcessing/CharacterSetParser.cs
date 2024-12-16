using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing
{
	public class CharacterSetParser
	{
		private class ParsePosition
		{
			public int Position
			{
				get;
				private set;
			}

			public ParsePosition(int i)
			{
				Position = i;
			}

			public void Advance()
			{
				int num = ++Position;
			}

			public void Advance(int offset)
			{
				Position += offset;
			}
		}

		private readonly string _input;

		private readonly int _inputLength;

		private readonly ParsePosition _position;

		public static CharacterSet Parse(string input, ref int p)
		{
			ParsePosition parsePosition = new ParsePosition(p);
			CharacterSetParser characterSetParser = new CharacterSetParser(input, parsePosition);
			CharacterSet result = characterSetParser.Parse();
			p = parsePosition.Position;
			return result;
		}

		private CharacterSetParser(string input, ParsePosition position)
		{
			_input = input;
			_inputLength = input.Length;
			_position = position;
		}

		private CharacterSet Parse()
		{
			CharacterSet characterSet = new CharacterSet();
			Expect('[');
			if (LookingAt() == '^')
			{
				characterSet.Negated = true;
				_position.Advance();
			}
			int num = 0;
			while (num != 99)
			{
				char c = LookingAt();
				switch (num)
				{
				case 0:
					switch (c)
					{
					case '[':
						_position.Advance();
						if (LookingAt() == ':')
						{
							_position.Advance();
							num = 1;
						}
						else if (LookingAt() != 0)
						{
							characterSet.Add('[');
						}
						break;
					case '\0':
						throw new LanguageProcessingException(ErrorMessages.EMSG_TokenizerInvalidCharacterSet);
					case ']':
						_position.Advance();
						num = 99;
						break;
					default:
					{
						char c2 = ScanChar();
						if (LookingAt() == '-')
						{
							_position.Advance();
							if (LookingAt() == ']')
							{
								characterSet.Add(c2);
								characterSet.Add('-');
								_position.Advance();
								num = 99;
							}
							else
							{
								char upper = ScanChar();
								characterSet.Add(c2, upper);
							}
						}
						else
						{
							characterSet.Add(c2);
							num = 0;
						}
						break;
					}
					}
					break;
				case 1:
				{
					if (LookingAt() == '\0')
					{
						throw new LanguageProcessingException(ErrorMessages.EMSG_TokenizerInvalidCharacterSet);
					}
					StringBuilder stringBuilder = new StringBuilder();
					while (char.IsLetter(LookingAt()))
					{
						stringBuilder.Append(LookingAt());
						_position.Advance();
					}
					if (stringBuilder.Length != 0)
					{
						UnicodeCategory? unicodeCategoryFromName;
						UnicodeCategory? unicodeCategory = unicodeCategoryFromName = CharacterProperties.GetUnicodeCategoryFromName(stringBuilder.ToString().ToLower());
						if (unicodeCategory.HasValue)
						{
							characterSet.Add(unicodeCategoryFromName.Value);
							Expect(':');
							Expect(']');
							num = 0;
							break;
						}
					}
					throw new LanguageProcessingException(ErrorMessages.EMSG_TokenizerInvalidCharacterSet);
				}
				default:
					throw new LanguageProcessingException(ErrorMessages.EMSG_TokenizerInvalidCharacterSet);
				}
			}
			return characterSet;
		}

		private void Expect(char c)
		{
			if (_position.Position >= _inputLength || _input[_position.Position] != c)
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_TokenizerInvalidCharacterSet);
			}
			if (c <= '\0')
			{
				throw new ArgumentOutOfRangeException("c");
			}
			_position.Advance();
		}

		private char ScanChar()
		{
			char c = LookingAt();
			switch (c)
			{
			case 'U':
			case 'u':
			{
				int hexValue;
				int hexValue2;
				int hexValue3;
				int hexValue4;
				if (_position.Position + 5 < _inputLength && _input[_position.Position + 1] == '+' && (hexValue = GetHexValue(_input[_position.Position + 2])) >= 0 && (hexValue2 = GetHexValue(_input[_position.Position + 3])) >= 0 && (hexValue3 = GetHexValue(_input[_position.Position + 4])) >= 0 && (hexValue4 = GetHexValue(_input[_position.Position + 5])) >= 0)
				{
					_position.Advance(5);
					return (char)(hexValue * 4096 + hexValue2 * 256 + hexValue3 * 16 + hexValue4);
				}
				_position.Advance();
				return c;
			}
			case '\\':
				if (_position.Position + 1 < _inputLength)
				{
					_position.Advance(2);
					return _input[_position.Position - 1];
				}
				break;
			}
			_position.Advance();
			return c;
		}

		private static int GetHexValue(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - 48;
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - 97 + 10;
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - 65 + 10;
			}
			return -1;
		}

		private char LookingAt(int offset = 0)
		{
			if (_position.Position + offset < _inputLength)
			{
				return _input[_position.Position + offset];
			}
			return '\0';
		}
	}
}
