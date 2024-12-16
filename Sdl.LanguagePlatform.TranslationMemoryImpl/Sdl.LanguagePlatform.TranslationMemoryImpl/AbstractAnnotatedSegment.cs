using Sdl.Core.LanguageProcessing;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.Lingua.TermRecognition;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal abstract class AbstractAnnotatedSegment : IAnnotatedSegment
	{
		protected Segment _Segment;

		private List<int> _tmFeatureVector;

		private List<int> _concordanceFeatureVector;

		private string _identityString;

		private string _strictIdentityString;

		private const int ConcordanceNGramSize = 2;

		private readonly object _linguaToolsLock = new object();

		private const uint FnvPrime = 16777619u;

		private const uint FnvOffsetBasis = 2166136261u;

		internal static bool IgnoreWhitespaceInStrictIdentityString;

		protected LanguageTools LinguaLanguageTools => GetLinguaLanguageTools();

		public Segment Segment => _Segment;

		public string TrimmedPrefix
		{
			get;
		}

		public string TrimmedSuffix
		{
			get;
		}

		public List<int> TmFeatureVector
		{
			get
			{
				if (_tmFeatureVector != null)
				{
					return _tmFeatureVector;
				}
				List<SegmentRange> featureToRangeMapping = null;
				_tmFeatureVector = ComputeFeatureVector(FeatureVectorType.ForTranslationMemory, sortAndUnique: true, ref featureToRangeMapping);
				return _tmFeatureVector;
			}
		}

		public List<int> ConcordanceFeatureVector
		{
			get
			{
				if (_concordanceFeatureVector != null)
				{
					return _concordanceFeatureVector;
				}
				List<SegmentRange> featureToRangeMapping = null;
				_concordanceFeatureVector = ComputeFeatureVector(FeatureVectorType.ForConcordance, sortAndUnique: true, ref featureToRangeMapping);
				return _concordanceFeatureVector;
			}
		}

		private string IdentityString
		{
			get
			{
				if (_identityString != null)
				{
					return _identityString;
				}
				lock (_linguaToolsLock)
				{
					List<SegmentRange> positionTokenAssociation = null;
					_identityString = LinguaLanguageTools.ComputeIdentityString(_Segment, LanguageTools.TokenToFeatureMappingMode.Stem, ref positionTokenAssociation);
				}
				return _identityString;
			}
		}

		private string StrictIdentityString
		{
			get
			{
				if (_strictIdentityString != null)
				{
					return _strictIdentityString;
				}
				lock (_linguaToolsLock)
				{
					_strictIdentityString = ComputeStrictIdentityString(_Segment);
				}
				return _strictIdentityString;
			}
		}

		public long Hash => Sdl.LanguagePlatform.Lingua.Hash.GetHashCodeLong(IdentityString);

		public long StrictHash => GetStrictHash(StrictIdentityString);

		public AbstractAnnotatedSegment(Segment s, bool keepTokens, bool keepPeripheralWhitespace)
		{
			if (!keepPeripheralWhitespace)
			{
				TrimmedPrefix = s.TrimStart();
				TrimmedSuffix = s.TrimEnd();
				if (TrimmedPrefix == null)
				{
					_ = TrimmedSuffix;
				}
			}
			if (!keepTokens)
			{
				s.Tokens = null;
			}
			_Segment = s;
		}

		protected abstract LanguageTools GetLinguaLanguageTools();

		public List<FeatureToRangeMapping> ComputeWordItemVector()
		{
			List<SegmentRange> featureToRangeMapping = new List<SegmentRange>();
			List<int> features = ComputeFeatureVector(FeatureVectorType.ForTranslationMemory, sortAndUnique: false, ref featureToRangeMapping);
			return featureToRangeMapping.Select((SegmentRange value, int index) => new FeatureToRangeMapping(features[index], value)).ToList();
		}

		public List<int> ComputeCharacterItemVector(ref List<SegmentRange> featureToRangeMapping)
		{
			return ComputeFeatureVector(FeatureVectorType.ForConcordance, sortAndUnique: false, ref featureToRangeMapping);
		}

		private List<int> ComputeFeatureVector(FeatureVectorType type, bool sortAndUnique, ref List<SegmentRange> featureToRangeMapping)
		{
			List<int> list = null;
			LinguaLanguageTools.Stem(_Segment);
			bool flag = AdvancedTokenization.TokenizesToWords(_Segment.Culture);
			switch (type)
			{
			case FeatureVectorType.ForTranslationMemory:
				list = (flag ? LinguaLanguageTools.ComputeTokenFeatureVector(_Segment, includeFrequent: true, sortAndUnique, ref featureToRangeMapping) : LinguaLanguageTools.ComputeCharFeatureVector(type, _Segment, 3, sortAndUnique, ref featureToRangeMapping));
				break;
			case FeatureVectorType.ForConcordance:
				list = (flag ? LinguaLanguageTools.ComputeCharFeatureVector(type, _Segment, 2, sortAndUnique, ref featureToRangeMapping) : LinguaLanguageTools.ComputeCharFeatureVector(type, _Segment, 1, sortAndUnique, ref featureToRangeMapping));
				break;
			}
			if (sortAndUnique)
			{
				list?.Sort();
			}
			if (featureToRangeMapping != null)
			{
			}
			return list;
		}

		private static long GetStrictHash(string s)
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

		private static uint JenkinsHash(byte[] bytes)
		{
			uint num = 0u;
			foreach (byte b in bytes)
			{
				num += b;
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}

		private string ComputeStrictIdentityString(Segment s)
		{
			LinguaLanguageTools.EnsureTokenizedSegment(s);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token token in s.Tokens)
			{
				string text = null;
				switch (token.Type)
				{
				case TokenType.Whitespace:
					if (IgnoreWhitespaceInStrictIdentityString)
					{
						break;
					}
					goto case TokenType.Unknown;
				case TokenType.Unknown:
				case TokenType.Word:
				case TokenType.Abbreviation:
				case TokenType.CharSequence:
				case TokenType.GeneralPunctuation:
				case TokenType.OpeningPunctuation:
				case TokenType.ClosingPunctuation:
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
	}
}
