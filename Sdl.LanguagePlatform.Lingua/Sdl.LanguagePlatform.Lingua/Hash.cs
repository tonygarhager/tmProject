using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	public class Hash
	{
		private const uint FnvPrime = 16777619u;

		private const uint FnvOffsetBasis = 2166136261u;

		public static int GetHashCodeInt(string s)
		{
			int num = 0;
			foreach (char c in s)
			{
				num = 31 * num + c;
			}
			return num;
		}

		public static long GetHashCodeLong(string s)
		{
			long num = 0L;
			foreach (char c in s)
			{
				num = 31 * num + c;
			}
			return num;
		}

		public static string ComputeStrictIdentityString(Segment s, LanguageTools languageTools)
		{
			languageTools.EnsureTokenizedSegment(s);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token token in s.Tokens)
			{
				string text = null;
				switch (token.Type)
				{
				case TokenType.Unknown:
				case TokenType.Word:
				case TokenType.Abbreviation:
				case TokenType.CharSequence:
				case TokenType.GeneralPunctuation:
				case TokenType.OpeningPunctuation:
				case TokenType.ClosingPunctuation:
				case TokenType.Whitespace:
				case TokenType.Uri:
					text = EscFn(token.Text);
					break;
				case TokenType.OtherTextPlaceable:
				{
					SimpleToken simpleToken = token as SimpleToken;
					if (simpleToken != null)
					{
						text = ((!simpleToken.IsSubstitutable) ? EscFn(token.Text) : ("\\" + new string((char)(61696 + token.Type), 1)));
					}
					break;
				}
				case TokenType.Tag:
					text = "\\" + new string('\uf164', 1);
					break;
				case TokenType.Date:
				case TokenType.Time:
				case TokenType.Variable:
				case TokenType.Number:
				case TokenType.Measurement:
				case TokenType.Acronym:
				case TokenType.UserDefined:
				case TokenType.AlphaNumeric:
					text = "\\" + new string((char)(61696 + token.Type), 1);
					break;
				}
				if (text != null)
				{
					stringBuilder.Append(text);
				}
			}
			return stringBuilder.ToString();
			string EscFn(string x)
			{
				return x.Replace("\\", "\\\\");
			}
		}

		public static long GetStrictHash(string s)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(s);
			uint num = Fnv1A32Hash(bytes, 0, bytes.Length);
			uint num2 = JenkinsHash(bytes);
			num2 = (uint)(num2 & -65536);
			long num3 = num;
			num3 += num2;
			num3 += (s.Length & 0xFFFF);
			if (num3 != 0L && num3 != -1)
			{
				return num3;
			}
			return -2L;
		}

		private static uint Fnv1A32Hash(IReadOnlyList<byte> array, int ibStart, int cbSize)
		{
			uint num = 2166136261u;
			for (int i = ibStart; i < cbSize; i++)
			{
				num ^= array[i];
				num *= 16777619;
			}
			return num;
		}

		private static uint JenkinsHash(IEnumerable<byte> bytes)
		{
			uint num = 0u;
			foreach (byte @byte in bytes)
			{
				num += @byte;
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}
	}
}
