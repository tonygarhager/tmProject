using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Core.Serialization
{
	internal class TokenEnumerator
	{
		private readonly IList<SerializationToken> _tokens;

		private int _position;

		public SerializationToken Current
		{
			get
			{
				if (!Valid())
				{
					throw new InvalidOperationException();
				}
				return _tokens[_position];
			}
		}

		public TokenEnumerator(string contentString)
		{
			_tokens = GetTokens(contentString);
		}

		private static IList<SerializationToken> GetTokens(string contentString)
		{
			IList<SerializationToken> list = new List<SerializationToken>();
			int i = 0;
			while (i < contentString.Length)
			{
				char c = contentString[i];
				if (c == '<')
				{
					if (contentString[i + 1] == '/' || contentString[i + 2] == '>')
					{
						i++;
						char c2 = contentString[i];
						SerializationToken serializationToken = new SerializationToken();
						switch (c2)
						{
						case '/':
							switch (contentString[i + 1])
							{
							case 't':
								serializationToken.TokenType = TokenType.EndTagPairContent;
								break;
							case 'l':
								serializationToken.TokenType = TokenType.EndLockedContent;
								break;
							case 'r':
								serializationToken.TokenType = TokenType.EndRevisionMarker;
								break;
							case 'c':
								serializationToken.TokenType = TokenType.EndCommentMarker;
								break;
							}
							i++;
							break;
						case 't':
							serializationToken.TokenType = TokenType.StartTagPairContent;
							break;
						case 'c':
							serializationToken.TokenType = TokenType.StartCommentMarker;
							break;
						case 'l':
							serializationToken.TokenType = TokenType.StartLockedContent;
							break;
						case 'r':
						{
							char c3 = contentString[i + 1];
							serializationToken.TokenType = ((c3 == 'i') ? TokenType.StartRevisionMarkerInclude : TokenType.StartRevisionMarkerDelete);
							break;
						}
						case 'p':
							serializationToken.TokenType = TokenType.PlaceholderTag;
							i += 2;
							break;
						}
						list.Add(serializationToken);
						i += 2;
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (i++; i < contentString.Length && contentString[i] != '/'; i++)
						{
							stringBuilder.Append(contentString[i]);
						}
						SerializationToken item = new SerializationToken
						{
							TokenType = TokenType.PlaceholderTag,
							Text = stringBuilder.ToString()
						};
						list.Add(item);
						i += 2;
					}
				}
				else
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					for (; i < contentString.Length && contentString[i] != '<'; i++)
					{
						stringBuilder2.Append(contentString[i]);
					}
					SerializationToken item2 = new SerializationToken
					{
						TokenType = TokenType.Text,
						Text = stringBuilder2.ToString()
					};
					list.Add(item2);
				}
			}
			return list;
		}

		public bool MoveNext()
		{
			_position++;
			return Valid();
		}

		public bool MovePrevious()
		{
			_position--;
			return Valid();
		}

		private bool Valid()
		{
			if (_position >= 0)
			{
				return _position < _tokens.Count;
			}
			return false;
		}
	}
}
