using Sdl.Core.LanguageProcessing;
using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Stemming;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	public class LanguageTools
	{
		public enum TokenToFeatureMappingMode
		{
			Stem,
			Token
		}

		private IStemmer _stemmer;

		private Tokenizer _tokenizer;

		private readonly LanguageResources _resources;

		private readonly BuiltinRecognizers _recognizers;

		private readonly TokenizerFlags _tokenizerFlags;

		private readonly bool _useAlternateStemmers;

		private readonly bool _normalizeCharWidths;

		private IStemmer Stemmer
		{
			get
			{
				if (_stemmer != null)
				{
					return _stemmer;
				}
				if (_useAlternateStemmers)
				{
					RuleBasedStemmer ruleBasedStemmer = new RuleBasedStemmer(_resources);
					SnowballWrapper snowballWrapper = SnowballWrapper.Create(_resources.Culture, ruleBasedStemmer.StripsDiacritics);
					if (snowballWrapper != null)
					{
						_stemmer = new CachingStemmer(snowballWrapper);
						return _stemmer;
					}
				}
				_stemmer = new CachingStemmer(new RuleBasedStemmer(_resources, _useAlternateStemmers));
				return _stemmer;
			}
		}

		private Tokenizer Tokenizer
		{
			get
			{
				if (_tokenizer != null)
				{
					return _tokenizer;
				}
				TokenizerSetup tokenizerSetup = new TokenizerSetup
				{
					Culture = _resources.Culture,
					CreateWhitespaceTokens = true
				};
				tokenizerSetup.BreakOnWhitespace = CultureInfoExtensions.UseBlankAsWordSeparator(tokenizerSetup.Culture);
				tokenizerSetup.BuiltinRecognizers = _recognizers;
				tokenizerSetup.TokenizerFlags = _tokenizerFlags;
				TokenizerParameters parameters = new TokenizerParameters(tokenizerSetup, _resources.ResourceDataAccessor);
				_tokenizer = new Tokenizer(parameters);
				return _tokenizer;
			}
		}

		public string StoplistSignature => _resources.StoplistSignature;

		public string TokenizerSignature => Tokenizer.Signature;

		public string StemmerSignature => Stemmer.Signature;

		public string AbbreviationsSignature => _resources.AbbreviationsSignature;

		public LanguageTools(LanguageResources resources, BuiltinRecognizers recognizers, TokenizerFlags flags)
		{
			_resources = (resources ?? throw new ArgumentNullException("resources"));
			_recognizers = recognizers;
			_tokenizerFlags = flags;
		}

		public LanguageTools(LanguageResources resources, BuiltinRecognizers recognizers, TokenizerFlags flags, bool useAlternateStemmers, bool normalizeCharWidths)
			: this(resources, recognizers, flags)
		{
			_useAlternateStemmers = useAlternateStemmers;
			_normalizeCharWidths = (normalizeCharWidths && CultureInfoExtensions.UseFullWidth(resources.Culture));
		}

		public LanguageTools(LanguageResources resources, BuiltinRecognizers recognizers)
			: this(resources, recognizers, TokenizerFlags.DefaultFlags)
		{
		}

		public TokenizationContext GetTokenizationContext()
		{
			return Tokenizer.GetTokenizationContext();
		}

		public void EnsureTokenizedSegment(Segment segment)
		{
			EnsureTokenizedSegment(segment, forceRetokenization: false, allowTokenBundles: false);
		}

		public void EnsureTokenizedSegment(Segment segment, bool forceRetokenization, bool allowTokenBundles)
		{
			if (forceRetokenization || segment.Tokens == null)
			{
				segment.Tokens = Tokenizer.Tokenize(segment, allowTokenBundles);
			}
		}

		public void Stem(Segment s)
		{
			EnsureTokenizedSegment(s);
			foreach (Token token in s.Tokens)
			{
				TokenType type = token.Type;
				if (type == TokenType.Word || type == TokenType.CharSequence || (type == TokenType.Acronym && _useAlternateStemmers))
				{
					SimpleToken simpleToken = token as SimpleToken;
					if (simpleToken != null && simpleToken.Stem == null && !token.Text.Any(char.IsDigit))
					{
						if (token.Type == TokenType.CharSequence)
						{
							simpleToken.Stem = simpleToken.Text;
						}
						else
						{
							simpleToken.Stem = Stemmer.Stem(simpleToken.Text);
							if (_normalizeCharWidths)
							{
								simpleToken.Stem = StringUtilities.HalfWidthToFullWidth2(simpleToken.Stem);
							}
						}
					}
				}
			}
		}

		public bool IsNonblankLanguage()
		{
			return !CultureInfoExtensions.UseBlankAsWordSeparator(_resources.Culture);
		}

		public string ComputeIdentityString(Segment segment, TokenToFeatureMappingMode mode, ref List<SegmentRange> positionTokenAssociation)
		{
			bool flag = AdvancedTokenization.TokenizesToWords(segment.Culture);
			EnsureTokenizedSegment(segment);
			if (mode == TokenToFeatureMappingMode.Stem)
			{
				Stem(segment);
			}
			bool flag2 = positionTokenAssociation != null;
			if (flag2)
			{
				positionTokenAssociation.Clear();
			}
			StringBuilder stringBuilder;
			if (flag)
			{
				stringBuilder = new StringBuilder("|");
				if (flag2)
				{
					positionTokenAssociation.Add(null);
				}
			}
			else
			{
				stringBuilder = new StringBuilder();
			}
			foreach (Token token in segment.Tokens)
			{
				string text = null;
				bool flag3 = true;
				switch (token.Type)
				{
				case TokenType.Word:
				case TokenType.Uri:
				{
					SimpleToken simpleToken = token as SimpleToken;
					if (simpleToken != null)
					{
						text = ((mode != 0) ? simpleToken.Text.ToLowerInvariant() : (simpleToken.Stem ?? simpleToken.Text.ToLowerInvariant()));
					}
					break;
				}
				case TokenType.OtherTextPlaceable:
				{
					SimpleToken simpleToken2 = token as SimpleToken;
					if (simpleToken2 != null)
					{
						text = ((!simpleToken2.IsSubstitutable) ? ((mode != 0) ? simpleToken2.Text.ToLowerInvariant() : (simpleToken2.Stem ?? simpleToken2.Text.ToLowerInvariant())) : ((mode == TokenToFeatureMappingMode.Stem) ? new string((char)(61696 + token.Type), 1) : token.Text.ToLowerInvariant()));
					}
					break;
				}
				case TokenType.CharSequence:
					text = token.Text.ToLowerInvariant();
					flag3 = false;
					break;
				case TokenType.Abbreviation:
					text = token.Text.ToLowerInvariant();
					break;
				case TokenType.Date:
				case TokenType.Time:
				case TokenType.Variable:
				case TokenType.Number:
				case TokenType.Measurement:
				case TokenType.Acronym:
				case TokenType.AlphaNumeric:
					text = ((mode == TokenToFeatureMappingMode.Stem) ? new string((char)(61696 + token.Type), 1) : token.Text.ToLowerInvariant());
					break;
				case TokenType.UserDefined:
					text = ((mode == TokenToFeatureMappingMode.Stem) ? new string((char)(61696 + token.Type), 1) : token.Text.ToLowerInvariant());
					break;
				case TokenType.GeneralPunctuation:
				case TokenType.OpeningPunctuation:
				case TokenType.ClosingPunctuation:
				case TokenType.Whitespace:
				case TokenType.Tag:
					continue;
				}
				if (text != null)
				{
					stringBuilder.Append(text);
					if (flag2)
					{
						for (int i = 0; i < text.Length; i++)
						{
							if (flag3)
							{
								positionTokenAssociation.Add(token.Span);
							}
							else
							{
								positionTokenAssociation.Add(new SegmentRange(token.Span.From.Index, token.Span.From.Position + i, token.Span.From.Position + i));
							}
						}
					}
					if (flag)
					{
						stringBuilder.Append("|");
						if (flag2)
						{
							positionTokenAssociation.Add(null);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		public List<int> ComputeCharFeatureVector(FeatureVectorType fvt, Segment segment, int n, bool unique, ref List<SegmentRange> featureRanges)
		{
			List<Pair<int>> featurePositionAssociation = null;
			List<SegmentRange> positionTokenAssociation = null;
			if (featureRanges != null && !unique)
			{
				featurePositionAssociation = new List<Pair<int>>();
				positionTokenAssociation = new List<SegmentRange>();
				featureRanges.Clear();
			}
			string s;
			switch (fvt)
			{
			case FeatureVectorType.ForTranslationMemory:
				s = ComputeIdentityString(segment, TokenToFeatureMappingMode.Stem, ref positionTokenAssociation);
				break;
			case FeatureVectorType.ForConcordance:
				s = ComputeIdentityString(segment, TokenToFeatureMappingMode.Token, ref positionTokenAssociation);
				break;
			default:
				throw new Exception("Unexpected case");
			}
			List<int> result = ComputeCharFeatureVector(s, n, unique, ref featurePositionAssociation);
			if (positionTokenAssociation == null)
			{
				return result;
			}
			foreach (Pair<int> item2 in featurePositionAssociation)
			{
				SegmentRange item = positionTokenAssociation[item2.Left];
				featureRanges.Add(item);
			}
			return result;
		}

		public List<int> ComputeCharFeatureVector(string s, int n, bool unique, ref List<Pair<int>> featurePositionAssociation)
		{
			List<int> list = new List<int>();
			bool flag = false;
			if (featurePositionAssociation != null)
			{
				featurePositionAssociation.Clear();
				flag = !unique;
			}
			if (string.IsNullOrEmpty(s))
			{
				list.Add(0);
			}
			else if (s.Length <= n)
			{
				list.Add(Hash.GetHashCodeInt(s));
				if (flag)
				{
					featurePositionAssociation.Add(new Pair<int>(0, s.Length - 1));
				}
			}
			else
			{
				for (int i = 0; i <= s.Length - n; i++)
				{
					int hashCodeInt = Hash.GetHashCodeInt(s.Substring(i, n));
					if (!unique || !list.Contains(hashCodeInt))
					{
						list.Add(hashCodeInt);
						if (flag)
						{
							featurePositionAssociation.Add(new Pair<int>(i, i + n - 1));
						}
					}
				}
			}
			if (unique)
			{
				list.Sort();
			}
			return list;
		}

		public List<int> ComputeTokenFeatureVector(Segment segment, bool includeFrequent, bool unique, ref List<SegmentRange> featureToRangeMapping)
		{
			bool flag = featureToRangeMapping != null && !unique;
			List<int> list = new List<int>();
			if (flag && featureToRangeMapping.Count > 0)
			{
				featureToRangeMapping.Clear();
			}
			foreach (Token token in segment.Tokens)
			{
				int num = 0;
				switch (token.Type)
				{
				case TokenType.Word:
				case TokenType.Abbreviation:
				case TokenType.Acronym:
				case TokenType.Uri:
				case TokenType.OtherTextPlaceable:
				{
					SimpleToken simpleToken = token as SimpleToken;
					if (simpleToken != null && (includeFrequent || !IsStopword(simpleToken.Text)))
					{
						num = Hash.GetHashCodeInt(simpleToken.Stem ?? simpleToken.Text.ToLowerInvariant());
					}
					break;
				}
				case TokenType.CharSequence:
				{
					List<Pair<int>> featurePositionAssociation = flag ? new List<Pair<int>>() : null;
					int num2 = 0;
					foreach (int item in ComputeCharFeatureVector(token.Text, 3, unique, ref featurePositionAssociation))
					{
						if (item != 0 && (!unique || !list.Contains(item)))
						{
							list.Add(item);
							if (flag)
							{
								int num3 = token.Span.From.Position + num2;
								featureToRangeMapping.Add(new SegmentRange(token.Span.From.Index, num3, num3));
							}
						}
						num2++;
					}
					break;
				}
				case TokenType.Date:
				case TokenType.Time:
				case TokenType.Variable:
				case TokenType.Number:
				case TokenType.Measurement:
				case TokenType.UserDefined:
				case TokenType.AlphaNumeric:
					num = (int)token.Type;
					break;
				case TokenType.GeneralPunctuation:
				case TokenType.OpeningPunctuation:
				case TokenType.ClosingPunctuation:
				case TokenType.Whitespace:
				case TokenType.Tag:
					continue;
				}
				if (num != 0 && (!unique || !list.Contains(num)))
				{
					list.Add(num);
					if (flag)
					{
						featureToRangeMapping.Add(token.Span);
					}
				}
			}
			if (unique)
			{
				list.Sort();
			}
			return list;
		}

		public bool IsStopword(string s)
		{
			return _resources.IsStopword(s.ToLowerInvariant());
		}
	}
}
