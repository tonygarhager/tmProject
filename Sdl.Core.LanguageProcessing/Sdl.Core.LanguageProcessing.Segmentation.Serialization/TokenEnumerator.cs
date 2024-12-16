using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Segmentation.Serialization
{
	internal class TokenEnumerator
	{
		private readonly IList<Token> _tokens;

		private int _position = -1;

		public Token Current
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

		private static IList<Token> GetTokens(string contentString)
		{
			IList<Token> list = new List<Token>();
			int i = 0;
			bool flag = true;
			while (i < contentString.Length)
			{
				if (contentString[i] == '<')
				{
					i++;
					switch (contentString[i])
					{
					case '/':
						switch (contentString[i + 1])
						{
						case 't':
						{
							EndTagPairToken item11 = new EndTagPairToken();
							list.Add(item11);
							if (contentString.Substring(i + 1).StartsWith("tuh") || contentString.Substring(i + 1).StartsWith("tmh") || contentString.Substring(i + 1).StartsWith("tth"))
							{
								i++;
							}
							i += 4;
							break;
						}
						case 'l':
						{
							EndLockedToken item10 = new EndLockedToken();
							list.Add(item10);
							i += 3;
							break;
						}
						case 'r':
						{
							EndRevisionMarkerToken item9 = new EndRevisionMarkerToken();
							list.Add(item9);
							i += 4;
							break;
						}
						case 'c':
						{
							EndComentMarker item8 = new EndComentMarker();
							list.Add(item8);
							i += 3;
							break;
						}
						}
						break;
					case 't':
					{
						i++;
						char c = contentString[i];
						if (c == 'e' && i + 3 < contentString.Length && contentString[i + 1] == 'x' && contentString[i + 2] == 't')
						{
							i++;
							int num = i + 3;
							int num2 = contentString.IndexOf("</text>", num, StringComparison.Ordinal);
							TextToken item6 = new TextToken(contentString.Substring(num, num2 - num));
							list.Add(item6);
							i += num2 - num - 1 + "ext></text>".Length;
							break;
						}
						SegmentationHint segmentationHint2 = GetSegmentationHint(c);
						bool isWordStop2 = IsWordStop(c);
						bool flag2 = i + 1 < contentString.Length && contentString[i + 1] == 'h';
						StartTagPairToken item7 = new StartTagPairToken(segmentationHint2, isWordStop2, flag2);
						list.Add(item7);
						if (flag2)
						{
							i++;
						}
						i += 2;
						break;
					}
					case 'c':
					{
						i++;
						StartComentMarker item5 = new StartComentMarker();
						list.Add(item5);
						i++;
						break;
					}
					case 'l':
					{
						StartLockedToken item4 = new StartLockedToken();
						list.Add(item4);
						i += 2;
						break;
					}
					case 'r':
					{
						i++;
						int revisionMarkerTokenId = int.Parse(contentString[i].ToString());
						StartRevisionMarkerToken item3 = new StartRevisionMarkerToken(revisionMarkerTokenId);
						list.Add(item3);
						i += 2;
						break;
					}
					case 'p':
					{
						i++;
						char controlChar = contentString[i];
						SegmentationHint segmentationHint = GetSegmentationHint(controlChar);
						bool isWordStop = IsWordStop(controlChar);
						PlaceholderTagToken item2 = new PlaceholderTagToken(segmentationHint, isWordStop);
						list.Add(item2);
						i += 3;
						break;
					}
					case '0':
					{
						i++;
						TextToken item = new TextToken(string.Empty);
						list.Add(item);
						i++;
						break;
					}
					default:
						flag = false;
						break;
					}
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (!flag)
					{
						stringBuilder.Append("<");
						flag = true;
					}
					for (; i < contentString.Length && contentString[i] != '<'; i++)
					{
						stringBuilder.Append(contentString[i]);
					}
					TextToken item12 = new TextToken(stringBuilder.ToString());
					list.Add(item12);
				}
			}
			if (contentString == string.Empty)
			{
				list.Add(new TextToken(string.Empty));
			}
			return list;
		}

		private static SegmentationHint GetSegmentationHint(char controlChar)
		{
			switch (controlChar)
			{
			case 'e':
				return SegmentationHint.Exclude;
			case 'i':
				return SegmentationHint.Include;
			case 't':
				return SegmentationHint.IncludeWithText;
			case 'm':
				return SegmentationHint.MayExclude;
			case 'w':
				return SegmentationHint.Include;
			default:
				return SegmentationHint.MayExclude;
			}
		}

		private static bool IsWordStop(char controlChar)
		{
			return controlChar == 'w';
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
			if (_position > -1)
			{
				return _position < _tokens.Count;
			}
			return false;
		}
	}
}
