using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class TokenSerialization
	{
		private enum SerializationVersion
		{
			Version1 = 1,
			Version1Compact
		}

		private enum TokenClass
		{
			DateTimeToken = 1,
			NumberToken,
			MeasureToken,
			SimpleToken,
			TagToken,
			GenericPlaceableToken
		}

		private class TokenSerializer : ISegmentElementVisitor
		{
			private readonly BinaryWriter _writer;

			private readonly Segment _segment;

			private Token _previousToken;

			private bool _tokenHasStandardPlacement;

			private bool _tokenIsSingleChar;

			private readonly bool _compactSerialization = true;

			public bool UnableToSerialize
			{
				get;
				private set;
			}

			public TokenSerializer(BinaryWriter writer, Segment segment, bool compactSerialization)
			{
				_writer = (writer ?? throw new ArgumentNullException("writer"));
				_segment = (segment ?? throw new ArgumentNullException("segment"));
				_compactSerialization = compactSerialization;
				foreach (Token token in segment.Tokens)
				{
					if (token.Span.From.Index != token.Span.Into.Index)
					{
						UnableToSerialize = true;
						throw new Exception("Span.From.Index != Span.To.Index");
					}
					if (token.Span.From.Index >= segment.Elements.Count)
					{
						throw new Exception("t.Span.From.Index >= segment.Elements.Count");
					}
				}
			}

			public void Serialize()
			{
				if (_compactSerialization)
				{
					_writer.Write((byte)2);
				}
				else
				{
					_writer.Write((byte)1);
				}
				WriteIntAsShort(_segment.Tokens.Count);
				_previousToken = null;
				foreach (Token token in _segment.Tokens)
				{
					int num = (int)token.Type;
					if (!_TokenTypeToTokenClassMap.TryGetValue(token.Type, out TokenClass _))
					{
						throw new Exception("Unknown TokenType during serialization: " + token.Type.ToString());
					}
					_tokenIsSingleChar = false;
					if (token.Text.Length == 1)
					{
						num |= TokenTypeSingleCharFlag;
						_tokenIsSingleChar = true;
					}
					_tokenHasStandardPlacement = false;
					if (token.Type == TokenType.Word && ((token as SimpleToken) ?? throw new Exception("TokenType was Word but object was not SimpleToken: " + token.Text)).IsStopword)
					{
						num |= 0x40;
					}
					if (_previousToken != null && token.Span.From.Index == _previousToken.Span.From.Index && token.Span.From.Position == _previousToken.Span.From.Position + _previousToken.Span.Length)
					{
						num |= 0x80;
						_tokenHasStandardPlacement = true;
					}
					_writer.Write((byte)num);
					token.AcceptSegmentElementVisitor(this);
					_previousToken = token;
					if (UnableToSerialize)
					{
						break;
					}
				}
				if (!UnableToSerialize)
				{
					WriteAdditionalData(_segment.Tokens);
				}
			}

			internal static List<T> GetTokensOfType<T>(List<Token> tokens) where T : Token
			{
				List<T> typedTokens = (List<T>)(object)new List<T>();
				tokens.ForEach(delegate(Token x)
				{
					T val = x as T;
					if (val != null)
					{
						((List<T>)(object)typedTokens).Add(val);
					}
				});
				return (List<T>)(object)typedTokens;
			}

			private void WriteAdditionalData(List<Token> tokens)
			{
				List<MeasureToken> tokensOfType = GetTokensOfType<MeasureToken>(tokens);
				List<DateTimeToken> tokensOfType2 = GetTokensOfType<DateTimeToken>(tokens);
				foreach (MeasureToken item in tokensOfType)
				{
					WriteStringOrNull(item.CustomCategory);
				}
				foreach (DateTimeToken item2 in tokensOfType2)
				{
					WriteStringOrNull(item2.FormatString);
				}
			}

			public void VisitText(Text text)
			{
				throw new Exception("Unexpected Text in SaveTokens");
			}

			public void VisitTag(Tag tag)
			{
				throw new Exception("Unexpected Tag in SaveTokens");
			}

			public void VisitDateTimeToken(DateTimeToken token)
			{
				WriteIntAsByte((int)token.DateTimePatternType);
				_writer.Write(token.Value.ToBinary());
				WriteToken(token);
			}

			public void VisitNumberToken(NumberToken token)
			{
				WriteNumberToken(token);
			}

			public void VisitMeasureToken(MeasureToken token)
			{
				WriteIntAsByte((int)token.Unit);
				_writer.Write(token.UnitSeparator);
				WriteStringOrNull(token.UnitString);
				WriteNumberToken(token);
			}

			public void VisitSimpleToken(SimpleToken token)
			{
				GenericPlaceableToken genericPlaceableToken = token as GenericPlaceableToken;
				if (genericPlaceableToken != null)
				{
					if (genericPlaceableToken.TokenClass == null)
					{
						throw new Exception("gpt.TokenClass is null");
					}
					_writer.Write(genericPlaceableToken.TokenClass);
					_writer.Write(genericPlaceableToken.IsSubstitutable);
				}
				WriteToken(token);
			}

			public void VisitTagToken(TagToken token)
			{
				WriteIntAsByte(token.Span.From.Index);
			}

			private void WriteStringOrNull(string s)
			{
				if (s == null)
				{
					_writer.Write(value: false);
					return;
				}
				_writer.Write(value: true);
				_writer.Write(s);
			}

			private bool WriteIntAsSbyte(int i)
			{
				if (!_compactSerialization)
				{
					_writer.Write(i);
					return true;
				}
				if (i > 127 || i < -128)
				{
					UnableToSerialize = true;
					return false;
				}
				_writer.Write((sbyte)i);
				return true;
			}

			private bool WriteIntAsByte(int i)
			{
				if (!_compactSerialization)
				{
					_writer.Write(i);
					return true;
				}
				if (i > 255 || i < 0)
				{
					UnableToSerialize = true;
					return false;
				}
				_writer.Write((byte)i);
				return true;
			}

			private bool WriteIntAsShort(int i)
			{
				if (!_compactSerialization)
				{
					_writer.Write(i);
					return true;
				}
				if (i > 32767 || i < -32768)
				{
					UnableToSerialize = true;
					return false;
				}
				_writer.Write((short)i);
				return true;
			}

			private void WriteNumberToken(NumberToken token)
			{
				WriteIntAsSbyte((int)token.Sign);
				WriteStringOrNull(token.RawSign);
				WriteIntAsSbyte((int)token.DecimalSeparator);
				WriteIntAsSbyte((int)token.GroupSeparator);
				_writer.Write(token.AlternateGroupSeparator);
				_writer.Write(token.AlternateDecimalSeparator);
				WriteStringOrNull(token.RawFractionalDigits);
				WriteStringOrNull(token.RawDecimalDigits);
				WriteToken(token);
			}

			private void WriteToken(Token t)
			{
				WriteSpanInfo(t.Span.From, t.Span.Into);
			}

			private void WriteSpanInfo(SegmentPosition from, SegmentPosition into)
			{
				if (!_tokenHasStandardPlacement)
				{
					WriteIntAsByte(from.Index);
				}
				int i = into.Position - from.Position + 1;
				if (!_tokenHasStandardPlacement)
				{
					WriteIntAsShort(from.Position);
				}
				if (!_tokenIsSingleChar)
				{
					WriteIntAsByte(i);
				}
			}
		}

		private class TokenDeserializer
		{
			private readonly Segment _segment;

			private readonly BinaryReader _reader;

			private bool _compactSerialization;

			private int _elementIndexAdjustment;

			public TokenDeserializer(BinaryReader reader, Segment segment)
			{
				_reader = (reader ?? throw new ArgumentNullException("reader"));
				_segment = (segment ?? throw new ArgumentNullException("segment"));
			}

			public List<Token> Deserialize()
			{
				SerializationVersion serializationVersion = (SerializationVersion)_reader.ReadByte();
				_compactSerialization = true;
				switch (serializationVersion)
				{
				case SerializationVersion.Version1:
					_compactSerialization = false;
					break;
				default:
					throw new Exception("Unsupported SerializationVersion: " + serializationVersion.ToString());
				case SerializationVersion.Version1Compact:
					break;
				}
				List<Token> list = new List<Token>();
				int num = ReadIntAsShort();
				_elementIndexAdjustment = 0;
				for (int i = 0; i < num; i++)
				{
					Token previousToken = (i > 0) ? list[i - 1] : null;
					bool tokenHasStandardPlacement = false;
					bool tokenIsSingleChar = false;
					int num2 = _reader.ReadByte();
					bool isStopword = false;
					if ((num2 & 0x40) > 0)
					{
						isStopword = true;
						num2 -= 64;
					}
					if ((num2 & TokenTypeSingleCharFlag) > 0)
					{
						num2 -= TokenTypeSingleCharFlag;
						tokenIsSingleChar = true;
					}
					if ((num2 & 0x80) > 0)
					{
						num2 -= 128;
						tokenHasStandardPlacement = true;
					}
					TokenType tokenType = (TokenType)num2;
					if (!_TokenTypeToTokenClassMap.TryGetValue(tokenType, out TokenClass value))
					{
						throw new Exception("Unknown TokenType during deserialization: " + tokenType.ToString());
					}
					switch (value)
					{
					case TokenClass.DateTimeToken:
						list.Add(ReadDateTimeToken(tokenType, tokenHasStandardPlacement, previousToken, tokenIsSingleChar));
						break;
					case TokenClass.NumberToken:
						list.Add(ReadNumberToken(tokenType, tokenHasStandardPlacement, previousToken, tokenIsSingleChar));
						break;
					case TokenClass.MeasureToken:
						list.Add(ReadMeasureToken(tokenType, tokenHasStandardPlacement, previousToken, tokenIsSingleChar));
						break;
					case TokenClass.TagToken:
						list.Add(ReadTagToken(tokenType));
						break;
					case TokenClass.SimpleToken:
					{
						SimpleToken simpleToken = ReadSimpleToken(isGenericPlaceable: false, tokenType, tokenHasStandardPlacement, previousToken, tokenIsSingleChar);
						simpleToken.IsStopword = isStopword;
						list.Add(simpleToken);
						break;
					}
					case TokenClass.GenericPlaceableToken:
						list.Add(ReadSimpleToken(isGenericPlaceable: true, tokenType, tokenHasStandardPlacement, previousToken, tokenIsSingleChar));
						break;
					default:
						throw new Exception("Unexpected TokenClass in LoadTokens: " + value.ToString());
					}
				}
				ReadAdditionalDataIfPresent(list);
				foreach (Token item in list)
				{
					item.Culture = _segment.Culture;
				}
				return list;
			}

			private void ReadAdditionalDataIfPresent(List<Token> tokens)
			{
				ReadMeasureCategoriesAndDateTimeFormatsIfPresent(tokens);
			}

			private void ReadMeasureCategoriesAndDateTimeFormatsIfPresent(List<Token> tokens)
			{
				if (_reader.BaseStream.Position != _reader.BaseStream.Length)
				{
					List<MeasureToken> tokensOfType = TokenSerializer.GetTokensOfType<MeasureToken>(tokens);
					List<DateTimeToken> tokensOfType2 = TokenSerializer.GetTokensOfType<DateTimeToken>(tokens);
					foreach (MeasureToken item in tokensOfType)
					{
						item.CustomCategory = ReadStringOrNull();
					}
					foreach (DateTimeToken item2 in tokensOfType2)
					{
						item2.FormatString = ReadStringOrNull();
					}
				}
			}

			private DateTimeToken ReadDateTimeToken(TokenType tokenType, bool tokenHasStandardPlacement, Token previousToken, bool tokenIsSingleChar)
			{
				DateTimePatternType type = (DateTimePatternType)ReadIntAsByte();
				DateTime dateTime = DateTime.FromBinary(_reader.ReadInt64());
				SimpleToken simpleToken = new SimpleToken();
				ReadToken(simpleToken, tokenHasStandardPlacement, previousToken, tokenIsSingleChar);
				DateTimeToken dateTimeToken = new DateTimeToken(simpleToken.Text, dateTime, type);
				CopyToken(simpleToken, dateTimeToken);
				dateTimeToken.Type = tokenType;
				return dateTimeToken;
			}

			private MeasureToken ReadMeasureToken(TokenType tokenType, bool tokenHasStandardPlacement, Token previousToken, bool tokenIsSingleChar)
			{
				Unit unit = (Unit)ReadIntAsByte();
				char unitSeparator = _reader.ReadChar();
				string unitString = ReadStringOrNull();
				NumberToken numberToken = ReadNumberToken(TokenType.Number, tokenHasStandardPlacement, previousToken, tokenIsSingleChar);
				MeasureToken measureToken = new MeasureToken(numberToken.Text, numberToken, unit, unitString, unitSeparator);
				CopyToken(numberToken, measureToken);
				measureToken.Type = tokenType;
				return measureToken;
			}

			private NumberToken ReadNumberToken(TokenType tokenType, bool tokenHasStandardPlacement, Token previousToken, bool tokenIsSingleChar)
			{
				Sign sign = (Sign)ReadIntAsByte();
				string rawSign = ReadStringOrNull();
				NumericSeparator decimalSeparator = (NumericSeparator)ReadIntAsSByte();
				NumericSeparator groupSeparator = (NumericSeparator)ReadIntAsSByte();
				char alternateGroupSeparator = _reader.ReadChar();
				char alternateDecimalSeparator = _reader.ReadChar();
				string rawFractionalDigits = ReadStringOrNull();
				string rawDecimalDigits = ReadStringOrNull();
				SimpleToken simpleToken = new SimpleToken();
				ReadToken(simpleToken, tokenHasStandardPlacement, previousToken, tokenIsSingleChar);
				NumberToken numberToken = new NumberToken(simpleToken.Text, groupSeparator, decimalSeparator, alternateGroupSeparator, alternateDecimalSeparator, sign, rawSign, rawDecimalDigits, rawFractionalDigits);
				CopyToken(simpleToken, numberToken);
				numberToken.Type = tokenType;
				return numberToken;
			}

			private TagToken ReadTagToken(TokenType tokenType)
			{
				TagToken tagToken = new TagToken();
				int num = ReadIntAsByte();
				num -= _elementIndexAdjustment;
				tagToken.Tag = ReadTag(num);
				tagToken.Span = new SegmentRange(num, 0, 0);
				tagToken.Text = tagToken.Tag.ToString();
				tagToken.Type = tokenType;
				return tagToken;
			}

			private static void CopyToken(Token source, Token target)
			{
				target.Span = source.Span.Duplicate();
			}

			private Tag ReadTag(int segElmIndex)
			{
				if (_segment.Elements.Count <= segElmIndex)
				{
					throw new Exception("Segment content incorrect: could not have produced the serialized Tag token");
				}
				return _segment.Elements[segElmIndex] as Tag;
			}

			private SimpleToken ReadSimpleToken(bool isGenericPlaceable, TokenType tokenType, bool tokenHasStandardPlacement, Token previousToken, bool tokenIsSingleChar)
			{
				SimpleToken simpleToken = new SimpleToken();
				if (isGenericPlaceable)
				{
					string tokenClass = _reader.ReadString();
					bool isSubstitutable = _reader.ReadBoolean();
					simpleToken = new GenericPlaceableToken(string.Empty, tokenClass, isSubstitutable);
				}
				ReadToken(simpleToken, tokenHasStandardPlacement, previousToken, tokenIsSingleChar);
				simpleToken.Type = tokenType;
				return simpleToken;
			}

			private void ReadToken(Token t, bool tokenHasStandardPlacement, Token previousToken, bool tokenIsSingleChar)
			{
				ReadTextAndSpan(t, tokenHasStandardPlacement, previousToken, tokenIsSingleChar);
			}

			private void ReadTextAndSpan(Token token, bool tokenHasStandardPlacement, Token previousToken, bool tokenIsSingleChar)
			{
				int num;
				int run;
				if (tokenHasStandardPlacement)
				{
					run = previousToken.Span.From.Index;
					num = previousToken.Span.From.Position + previousToken.Span.Length;
				}
				else
				{
					run = ReadIntAsByte();
					run -= _elementIndexAdjustment;
					num = ReadIntAsShort();
				}
				int num2 = tokenIsSingleChar ? 1 : ReadIntAsByte();
				token.Span = new SegmentRange(run, num, num + num2 - 1);
				if (previousToken != null && !(token is TagToken) && !(previousToken is TagToken) && token.Span.From.Index == previousToken.Span.From.Index + 1)
				{
					Text text = _segment.Elements[previousToken.Span.From.Index] as Text;
					if (text != null)
					{
						int num3 = previousToken.Span.From.Position + previousToken.Span.Length;
						if (text.Value.Length >= num3 + num2)
						{
							num = num3;
							token.Span = new SegmentRange(previousToken.Span.From.Index, num, num + num2 - 1);
							_elementIndexAdjustment++;
						}
					}
				}
				if (_segment.Elements.Count <= token.Span.From.Index)
				{
					throw new Exception("Segment content incorrect: could not have produced the serialized Token");
				}
				token.Text = _segment.Elements[token.Span.From.Index].ToString().Substring(num, num2);
			}

			private int ReadIntAsByte()
			{
				if (_compactSerialization)
				{
					return _reader.ReadByte();
				}
				return _reader.ReadInt32();
			}

			private int ReadIntAsSByte()
			{
				if (_compactSerialization)
				{
					return _reader.ReadSByte();
				}
				return _reader.ReadInt32();
			}

			private int ReadIntAsShort()
			{
				if (_compactSerialization)
				{
					return _reader.ReadInt16();
				}
				return _reader.ReadInt32();
			}

			private string ReadStringOrNull()
			{
				if (_reader.ReadBoolean())
				{
					return _reader.ReadString();
				}
				return null;
			}
		}

		private const int MaximumGuaranteedSerializedTokenCount = 200;

		private const int MaximumDataSize = 2047;

		internal static bool UseTokenSerialization = true;

		private static readonly Dictionary<TokenType, TokenClass> _TokenTypeToTokenClassMap = new Dictionary<TokenType, TokenClass>
		{
			{
				TokenType.Word,
				TokenClass.SimpleToken
			},
			{
				TokenType.Whitespace,
				TokenClass.SimpleToken
			},
			{
				TokenType.Measurement,
				TokenClass.MeasureToken
			},
			{
				TokenType.GeneralPunctuation,
				TokenClass.SimpleToken
			},
			{
				TokenType.OpeningPunctuation,
				TokenClass.SimpleToken
			},
			{
				TokenType.ClosingPunctuation,
				TokenClass.SimpleToken
			},
			{
				TokenType.Number,
				TokenClass.NumberToken
			},
			{
				TokenType.Tag,
				TokenClass.TagToken
			},
			{
				TokenType.AlphaNumeric,
				TokenClass.SimpleToken
			},
			{
				TokenType.Time,
				TokenClass.DateTimeToken
			},
			{
				TokenType.Abbreviation,
				TokenClass.SimpleToken
			},
			{
				TokenType.Acronym,
				TokenClass.SimpleToken
			},
			{
				TokenType.Variable,
				TokenClass.SimpleToken
			},
			{
				TokenType.UserDefined,
				TokenClass.SimpleToken
			},
			{
				TokenType.Uri,
				TokenClass.SimpleToken
			},
			{
				TokenType.Date,
				TokenClass.DateTimeToken
			},
			{
				TokenType.OtherTextPlaceable,
				TokenClass.GenericPlaceableToken
			},
			{
				TokenType.CharSequence,
				TokenClass.SimpleToken
			}
		};

		internal static readonly int TokenTypeSingleCharFlag = 32;

		private const int TokenTypeStopWordFlag = 64;

		private const int TokenTypeStandardPlacementFlag = 128;

		private static void CheckResults(Segment segment, byte[] result)
		{
			List<Token> list = LoadTokens(result, segment);
			for (int i = 0; i < list.Count; i++)
			{
				_ = (list[i] is DateTimeToken);
				_ = segment.Tokens[i];
				_ = (list[i] is NumberToken);
				_ = segment.Tokens[i];
				_ = (list[i] is MeasureToken);
				_ = segment.Tokens[i];
				_ = (list[i] is TagToken);
				_ = segment.Tokens[i];
				_ = (list[i] is SimpleToken);
				_ = segment.Tokens[i];
			}
		}

		[Conditional("DEBUG")]
		private static void MyAssert(bool test)
		{
			if (!test)
			{
				throw new Exception("Serialization debug check failure");
			}
		}

		public static byte[] SaveTokens(Segment segment)
		{
			if (!UseTokenSerialization)
			{
				return null;
			}
			if (segment == null)
			{
				throw new ArgumentNullException("segment");
			}
			if (segment.Tokens == null)
			{
				return null;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					TokenSerializer tokenSerializer = new TokenSerializer(binaryWriter, segment, compactSerialization: true);
					tokenSerializer.Serialize();
					if (tokenSerializer.UnableToSerialize)
					{
						binaryWriter.Seek(0, SeekOrigin.Begin);
						TokenSerializer tokenSerializer2 = new TokenSerializer(binaryWriter, segment, compactSerialization: false);
						tokenSerializer2.Serialize();
						if (tokenSerializer2.UnableToSerialize)
						{
							throw new Exception("Unable to serialize tokens even when not using compact serialization");
						}
					}
					binaryWriter.Close();
					memoryStream.Close();
					byte[] array = memoryStream.ToArray();
					if (array.Length > 2047 && segment.Tokens.Count > 200)
					{
						return null;
					}
					CheckResults(segment, array);
					return array;
				}
			}
		}

		public static List<Token> LoadTokens(byte[] data, Segment segment)
		{
			if (!UseTokenSerialization)
			{
				return null;
			}
			if (data == null)
			{
				return null;
			}
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(input))
				{
					return LoadTokens(reader, segment);
				}
			}
		}

		private static List<Token> LoadTokens(BinaryReader reader, Segment segment)
		{
			return new TokenDeserializer(reader, segment).Deserialize();
		}
	}
}
