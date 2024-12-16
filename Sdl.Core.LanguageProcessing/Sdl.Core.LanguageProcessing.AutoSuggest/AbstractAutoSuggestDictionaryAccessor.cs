using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.Core.LanguageProcessing.AutoSuggest
{
	internal abstract class AbstractAutoSuggestDictionaryAccessor : IAutoSuggestDictionaryAccessor, IDisposable
	{
		protected class CliticsInformation
		{
			public bool UsesClitics;

			public HashSet<string> LeadingClitics;

			public HashSet<string> TrailingClitics;
		}

		protected const int MinimumNonMergedWordLength = 4;

		protected static readonly HashSet<string> _SpecialTokenIDs = new HashSet<string>(new string[5]
		{
			"{{DAT}}",
			"{{MSR}}",
			"{{NUM}}",
			"{{TIM}}",
			"{{PCT}}"
		});

		protected string[] _specialTokenIDs;

		protected string _specialTokensExpression;

		private readonly Dictionary<string, Tokenizer> _tokenizerCache = new Dictionary<string, Tokenizer>();

		protected CliticsInformation SourceCliticsInformation;

		protected CliticsInformation TargetCliticsInformation;

		protected CultureInfo _sourceCulture;

		protected CultureInfo _targetCulture;

		protected string[] SpecialTokenIDs
		{
			get
			{
				if (_specialTokenIDs != null)
				{
					return _specialTokenIDs;
				}
				_specialTokenIDs = new string[_SpecialTokenIDs.Count];
				_SpecialTokenIDs.CopyTo(_specialTokenIDs);
				return _specialTokenIDs;
			}
		}

		protected string SpecialTokensExpression
		{
			get
			{
				if (_specialTokensExpression != null)
				{
					return _specialTokensExpression;
				}
				int num = SpecialTokenIDs.Length - 1;
				StringBuilder stringBuilder = new StringBuilder("(");
				for (int i = 0; i < num - 1; i++)
				{
					stringBuilder.Append(SpecialTokenIDs[i]);
					stringBuilder.Append("|");
				}
				stringBuilder.Append(SpecialTokenIDs[num]);
				stringBuilder.Append(")");
				_specialTokensExpression = stringBuilder.ToString();
				return _specialTokensExpression;
			}
		}

		public CultureInfo SourceCulture => _sourceCulture;

		public CultureInfo TargetCulture => _targetCulture;

		protected AbstractAutoSuggestDictionaryAccessor(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			_sourceCulture = sourceCulture;
			_targetCulture = targetCulture;
		}

		public IList<Segment> ComputeSuggestions(string sourceText)
		{
			Tokenizer standardTokenizer = GetStandardTokenizer(_sourceCulture);
			Segment segment = new Segment(_sourceCulture);
			segment.Add(sourceText);
			segment.Tokens = standardTokenizer.Tokenize(segment);
			return ComputeSuggestions(segment);
		}

		public IList<Segment> ComputeSuggestions(Segment sourceText)
		{
			IList<Segment> list = new List<Segment>();
			PhraseMappingPairs mappings = GetMappings(sourceText);
			foreach (PhraseMappingPair item in mappings)
			{
				if (!list.Contains(item.Target))
				{
					list.Add(item.Target);
				}
			}
			return list;
		}

		protected abstract void EnsureOpenConnection();

		protected abstract void AddMappings(IList<string> words, IList<string> mergedWords, PhraseMappingPairs mappings, bool hasSpecialTokens, IList<Token> sourceTokens);

		public PhraseMappingPairs GetMappings(string sourceText)
		{
			Segment segment = new Segment(_sourceCulture);
			segment.Add(sourceText);
			Tokenizer standardTokenizer = GetStandardTokenizer(_sourceCulture);
			segment.Tokens = standardTokenizer.Tokenize(segment);
			return GetMappings(segment);
		}

		public PhraseMappingPairs GetMappings(Segment sourceText)
		{
			PhraseMappingPairs phraseMappingPairs = new PhraseMappingPairs();
			EnsureOpenConnection();
			bool hasSpecialTokens;
			IList<string> words = GetWords(sourceText, out hasSpecialTokens);
			if (words.Count <= 0)
			{
				return phraseMappingPairs;
			}
			IList<string> mergedWords = GetMergedWords(words);
			AddMappings(words, mergedWords, phraseMappingPairs, hasSpecialTokens, sourceText.Tokens);
			return phraseMappingPairs;
		}

		private void EnsureTokenizedSegment(Segment segment, CultureInfo culture)
		{
			if (segment.Tokens == null)
			{
				Tokenizer standardTokenizer = GetStandardTokenizer(culture);
				segment.Tokens = standardTokenizer.Tokenize(segment);
			}
		}

		private Segment GetTokenizedSegment(CultureInfo culture, string text)
		{
			Segment segment = new Segment(culture);
			segment.Add(text);
			EnsureTokenizedSegment(segment, culture);
			return segment;
		}

		public PhraseMappingPairs GetCommonPhrases(Segment sourceSegment, Segment targetSegment, out int droppedPhrasePairs)
		{
			if (sourceSegment == null)
			{
				throw new ArgumentNullException("sourceSegment");
			}
			if (targetSegment == null)
			{
				throw new ArgumentNullException("targetSegment");
			}
			droppedPhrasePairs = 0;
			EnsureTokenizedSegment(sourceSegment, _sourceCulture);
			EnsureTokenizedSegment(targetSegment, _targetCulture);
			PhraseMappingPairs mappings = GetMappings(sourceSegment);
			if (mappings == null)
			{
				return null;
			}
			for (int num = mappings.Count - 1; num >= 0; num--)
			{
				PhraseMappingPair phraseMappingPair = mappings[num];
				List<int> list = Utilities.IndexOf(phraseMappingPair.Target.Tokens, targetSegment.Tokens);
				if (list == null || list.Count == 0)
				{
					droppedPhrasePairs++;
					mappings.RemoveAt(num);
				}
			}
			return mappings;
		}

		public PhraseMappingPairs GetCommonPhrases(string sourceText, string targetText, out int droppedPhrasePairs)
		{
			if (sourceText == null)
			{
				throw new ArgumentNullException("sourceText");
			}
			if (targetText == null)
			{
				throw new ArgumentNullException("targetText");
			}
			droppedPhrasePairs = 0;
			Segment tokenizedSegment = GetTokenizedSegment(_sourceCulture, sourceText);
			Segment tokenizedSegment2 = GetTokenizedSegment(_targetCulture, targetText);
			return GetCommonPhrases(tokenizedSegment, tokenizedSegment2, out droppedPhrasePairs);
		}

		public double GetCoverage(string sourceText, string targetText)
		{
			if (sourceText == null)
			{
				throw new ArgumentNullException("sourceText");
			}
			if (targetText == null)
			{
				throw new ArgumentNullException("targetText");
			}
			Segment tokenizedSegment = GetTokenizedSegment(_sourceCulture, sourceText);
			Segment tokenizedSegment2 = GetTokenizedSegment(_targetCulture, targetText);
			return GetCoverage(tokenizedSegment, tokenizedSegment2);
		}

		public double GetCoverage(Segment sourceSegment, Segment targetSegment)
		{
			if (sourceSegment == null)
			{
				throw new ArgumentNullException("sourceSegment");
			}
			if (targetSegment == null)
			{
				throw new ArgumentNullException("targetSegment");
			}
			EnsureTokenizedSegment(sourceSegment, _sourceCulture);
			EnsureTokenizedSegment(targetSegment, _targetCulture);
			int droppedPhrasePairs;
			PhraseMappingPairs commonPhrases = GetCommonPhrases(sourceSegment, targetSegment, out droppedPhrasePairs);
			if (commonPhrases == null || sourceSegment.Tokens == null || targetSegment.Tokens == null || sourceSegment.Tokens.Count == 0 || targetSegment.Tokens.Count == 0)
			{
				return 0.0;
			}
			BitArray bitArray = new BitArray(sourceSegment.Tokens.Count);
			BitArray bitArray2 = new BitArray(targetSegment.Tokens.Count);
			MarkPhrases(commonPhrases, sourceSegment, bitArray, inTarget: false);
			MarkPhrases(commonPhrases, targetSegment, bitArray2, inTarget: true);
			int num = CountBits(bitArray, flag: true);
			int num2 = CountBits(bitArray2, flag: true);
			return (double)(num + num2) / (double)(bitArray.Count + bitArray2.Count);
		}

		private static int CountBits(BitArray bitArray, bool flag)
		{
			int num = 0;
			for (int i = 0; i < bitArray.Count; i++)
			{
				if (bitArray[i] == flag)
				{
					num++;
				}
			}
			return num;
		}

		private static void MarkPhrases(PhraseMappingPairs phrasePairs, Segment segment, BitArray markers, bool inTarget)
		{
			foreach (PhraseMappingPair phrasePair in phrasePairs)
			{
				Segment segment2 = inTarget ? phrasePair.Target : phrasePair.Source;
				List<int> list = Utilities.IndexOf(segment2.Tokens, segment.Tokens);
				if (list != null && list.Count >= 0)
				{
					int count = segment2.Tokens.Count;
					foreach (int item in list)
					{
						for (int i = 0; i < count; i++)
						{
							markers[item + i] = true;
						}
					}
				}
			}
		}

		private IList<string> GetWords(Segment sourceText, out bool hasSpecialTokens)
		{
			IList<string> list = new List<string>();
			hasSpecialTokens = false;
			if (sourceText.Tokens == null && _sourceCulture != null)
			{
				Tokenizer standardTokenizer = GetStandardTokenizer(_sourceCulture);
				if (standardTokenizer != null)
				{
					sourceText.Tokens = standardTokenizer.Tokenize(sourceText);
				}
			}
			if (sourceText.Tokens == null)
			{
				return list;
			}
			foreach (Token token in sourceText.Tokens)
			{
				string tokenString = GetTokenString(token);
				list.Add(tokenString);
				if (tokenString != null && IsSpecial(token))
				{
					hasSpecialTokens = true;
				}
			}
			return list;
		}

		private static string GetTokenString(Token t)
		{
			if (t == null)
			{
				return null;
			}
			switch (t.Type)
			{
			case TokenType.Word:
			case TokenType.Abbreviation:
			case TokenType.CharSequence:
			case TokenType.Variable:
			case TokenType.Acronym:
			case TokenType.Uri:
			case TokenType.OtherTextPlaceable:
				return t.Text;
			case TokenType.Date:
				return "{{DAT}}";
			case TokenType.Measurement:
				return "{{MSR}}";
			case TokenType.Number:
				return "{{NUM}}";
			case TokenType.Time:
				return "{{TIM}}";
			case TokenType.GeneralPunctuation:
			case TokenType.OpeningPunctuation:
			case TokenType.ClosingPunctuation:
				return "{{PCT}}";
			default:
				return null;
			}
		}

		private Tokenizer GetStandardTokenizer(CultureInfo cultureInfo)
		{
			lock (_tokenizerCache)
			{
				string key = cultureInfo.Name.ToLower();
				if (_tokenizerCache.TryGetValue(key, out Tokenizer value))
				{
					return value;
				}
				TokenizerSetup setup = TokenizerSetupFactory.Create(cultureInfo, BuiltinRecognizers.RecognizeDates | BuiltinRecognizers.RecognizeTimes | BuiltinRecognizers.RecognizeNumbers | BuiltinRecognizers.RecognizeMeasurements, TokenizerFlags.BreakOnHyphen | TokenizerFlags.BreakOnApostrophe);
				value = new Tokenizer(setup);
				_tokenizerCache[key] = value;
				return value;
			}
		}

		private static IList<string> GetMergedWords(IList<string> words)
		{
			IList<string> list = new List<string>();
			for (int i = 0; i < words.Count - 1; i++)
			{
				if (words[i] != null && words[i].Length < 4)
				{
					list.Add(words[i] + " " + words[i + 1]);
				}
			}
			return list;
		}

		protected Segment CreateSegment(CultureInfo culture, string databaseString, CliticsInformation cliticsInformation)
		{
			List<string> list = databaseString.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			if (cliticsInformation != null && cliticsInformation.UsesClitics)
			{
				MeltClitics(list, cliticsInformation);
			}
			List<Token> list2 = new List<Token>();
			StringBuilder stringBuilder = new StringBuilder();
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				string current;
				Token token;
				for (; enumerator.MoveNext(); token.Span = new SegmentRange(0, stringBuilder.Length, stringBuilder.Length + current.Length - 1), stringBuilder.Append(current), list2.Add(token))
				{
					current = enumerator.Current;
					if (IsSpecial(current))
					{
						if (current != null)
						{
							if (current == "{{DAT}}")
							{
								token = new SimpleToken(current, TokenType.Date);
								continue;
							}
							if (current == "{{MSR}}")
							{
								token = new SimpleToken(current, TokenType.Measurement);
								continue;
							}
							if (current == "{{NUM}}")
							{
								token = new SimpleToken(current, TokenType.Number);
								continue;
							}
							if (current == "{{TIM}}")
							{
								token = new SimpleToken(current, TokenType.Time);
								continue;
							}
							if (current == "{{PCT}}")
							{
								token = new SimpleToken(current, TokenType.GeneralPunctuation);
								continue;
							}
						}
						throw new Exception("Invalid special token class");
					}
					token = new SimpleToken(current, TokenType.Word);
				}
			}
			Segment segment = new Segment(culture);
			segment.Add(stringBuilder.ToString());
			segment.Tokens = list2;
			return segment;
		}

		protected List<Pair<Segment>> SubstituteSpecialTokens(IList<Token> docSegmentTokens, Segment sourcePhrase, Segment targetPhrase)
		{
			IList<Token> tokens = sourcePhrase.Tokens;
			IList<Token> tokens2 = targetPhrase.Tokens;
			if (tokens == null || tokens.Count == 0 || tokens2 == null || tokens2.Count == 0)
			{
				return null;
			}
			List<Pair<int>> list = AlignSpecialTokens(tokens, tokens2);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<int> list2 = Utilities.IndexOf(tokens, docSegmentTokens);
			if (list2 == null || list2.Count == 0)
			{
				return null;
			}
			List<Pair<Segment>> list3 = null;
			foreach (int item in list2)
			{
				List<Token> list4 = new List<Token>(tokens);
				List<Token> list5 = new List<Token>(tokens2);
				bool[] array = null;
				bool[] array2 = null;
				int i = 0;
				while (i < tokens.Count)
				{
					Pair<int> pair = list.FirstOrDefault((Pair<int> x) => x.Left == i);
					if (pair != null)
					{
						int num = item + i;
						Token token = docSegmentTokens[num];
						list4[i] = token;
						if (token is ILocalizableToken)
						{
							ILocalizableToken localizableToken = token.Duplicate() as ILocalizableToken;
							if (localizableToken != null)
							{
								localizableToken.Localize(_targetCulture, null);
								list5[pair.Right] = (localizableToken as Token);
							}
						}
						else
						{
							if (num > 0 && docSegmentTokens[num - 1].Span.Into.Position + 1 >= token.Span.From.Position)
							{
								if (array == null)
								{
									array = new bool[tokens2.Count];
								}
								array[pair.Right] = true;
							}
							if (num + 1 < docSegmentTokens.Count && token.Span.Into.Position + 1 >= docSegmentTokens[num + 1].Span.From.Position)
							{
								if (array2 == null)
								{
									array2 = new bool[tokens2.Count];
								}
								array2[pair.Right] = true;
							}
							list5[pair.Right] = token;
						}
					}
					int num2 = ++i;
				}
				if (list3 == null)
				{
					list3 = new List<Pair<Segment>>();
				}
				Segment left = CreateFromTokens(_sourceCulture, list4);
				Segment right = Join(_targetCulture, list5, array, array2);
				list3.Add(new Pair<Segment>(left, right));
			}
			return list3;
		}

		private static Segment CreateFromTokens(CultureInfo culture, List<Token> tokens)
		{
			Segment segment = new Segment(culture)
			{
				Tokens = tokens
			};
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token token in tokens)
			{
				token.Span = new SegmentRange(0, stringBuilder.Length, stringBuilder.Length + token.Text.Length - 1);
				stringBuilder.Append(token.Text);
			}
			segment.Add(stringBuilder.ToString());
			return segment;
		}

		private static Segment Join(CultureInfo culture, List<Token> tokens, IList<bool> dropPrecedingWhitespace, IList<bool> dropFollowingWhitespace)
		{
			if (tokens == null || tokens.Count == 0)
			{
				return null;
			}
			if (dropPrecedingWhitespace == null && dropFollowingWhitespace == null)
			{
				return CreateFromTokens(culture, tokens);
			}
			if ((dropPrecedingWhitespace != null && dropPrecedingWhitespace.Count != tokens.Count) || (dropFollowingWhitespace != null && dropFollowingWhitespace.Count != tokens.Count))
			{
				return CreateFromTokens(culture, tokens);
			}
			Segment segment = new Segment(culture);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < tokens.Count; i++)
			{
				bool flag = dropPrecedingWhitespace != null && dropPrecedingWhitespace[i];
				bool flag2 = dropFollowingWhitespace != null && i > 0 && dropFollowingWhitespace[i - 1];
				if (!(flag | flag2))
				{
					stringBuilder.Append(" ");
				}
				tokens[i].Span = new SegmentRange(0, stringBuilder.Length, stringBuilder.Length + tokens[i].Text.Length - 1);
				stringBuilder.Append(tokens[i].Text);
			}
			segment.Add(stringBuilder.ToString());
			segment.Tokens = tokens;
			return segment;
		}

		private static List<Pair<int>> AlignSpecialTokens(IList<Token> src, IList<Token> trg)
		{
			BitArray bitArray = new BitArray(trg.Count);
			bool flag = false;
			for (int i = 0; i < trg.Count; i++)
			{
				bitArray[i] = IsSpecial(trg[i]);
				if (bitArray[i])
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			List<Pair<int>> list = null;
			for (int j = 0; j < src.Count; j++)
			{
				if (!IsSpecial(src[j]))
				{
					continue;
				}
				bool flag2 = false;
				for (int k = 0; k < trg.Count; k++)
				{
					if (flag2)
					{
						break;
					}
					if (bitArray[k] && src[j].Type == trg[k].Type)
					{
						if (list == null)
						{
							list = new List<Pair<int>>();
						}
						list.Add(new Pair<int>(j, k));
						bitArray[k] = false;
						flag2 = true;
					}
				}
			}
			return list;
		}

		private static bool IsSpecial(Token s)
		{
			return _SpecialTokenIDs.Contains(s.Text);
		}

		private static bool IsSpecial(string s)
		{
			return _SpecialTokenIDs.Contains(s);
		}

		protected string GetEncodedLiteralWord(string word)
		{
			return "'" + word.Replace("'", "''") + "'";
		}

		private static void MeltClitics(List<string> tokens, CliticsInformation cliticsInformation)
		{
			if (tokens == null || tokens.Count == 0 || cliticsInformation == null || !cliticsInformation.UsesClitics)
			{
				return;
			}
			bool flag = false;
			int count = tokens.Count;
			for (int i = 0; i < count; i++)
			{
				string text = tokens[i];
				if (text == null)
				{
					continue;
				}
				bool flag2 = cliticsInformation.LeadingClitics != null && cliticsInformation.LeadingClitics.Contains(text);
				bool flag3 = cliticsInformation.TrailingClitics != null && cliticsInformation.TrailingClitics.Contains(text);
				string str;
				if (flag2)
				{
					if (i < count - 1 && (str = tokens[i + 1]) != null)
					{
						tokens[i] = text + str;
						tokens[i + 1] = null;
						flag = true;
					}
				}
				else if (flag3 && i > 0 && (str = tokens[i - 1]) != null)
				{
					tokens[i] = str + text;
					tokens[i - 1] = null;
					flag = true;
				}
			}
			if (flag)
			{
				tokens.RemoveAll((string x) => x == null);
			}
		}

		public abstract void Dispose();
	}
}
