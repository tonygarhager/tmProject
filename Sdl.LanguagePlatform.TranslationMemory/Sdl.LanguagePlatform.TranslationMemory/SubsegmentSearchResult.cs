using System;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SubsegmentSearchResult : SearchResult
	{
		private int _repetitions;

		private string _cachedTranslationProposalString;

		[DataMember]
		public short MatchTokenIndex
		{
			get;
		}

		[DataMember]
		public short MatchTokenCount
		{
			get;
		}

		[DataMember]
		public short TranslationTokenIndex
		{
			get;
		}

		[DataMember]
		public short TranslationTokenCount
		{
			get;
		}

		[DataMember]
		public SubsegmentMatchType MatchType
		{
			get;
		}

		public short QueryTokenIndex
		{
			get;
		}

		public short QueryTokenCount
		{
			get;
		}

		public int Repetitions
		{
			get
			{
				return _repetitions;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("Repetitions");
				}
				_repetitions = value;
			}
		}

		public string TranslationFeatureString
		{
			get;
		}

		public float Confidence
		{
			get;
		}

		public string CachedTranslationProposalString
		{
			get
			{
				if (_cachedTranslationProposalString != null)
				{
					return _cachedTranslationProposalString;
				}
				_cachedTranslationProposalString = string.Empty;
				if (base.TranslationProposal?.TargetSegment == null)
				{
					return _cachedTranslationProposalString;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < TranslationTokenCount; i++)
				{
					stringBuilder.Append(base.TranslationProposal.TargetSegment.Tokens[i + TranslationTokenIndex]);
				}
				_cachedTranslationProposalString = stringBuilder.ToString();
				return _cachedTranslationProposalString;
			}
		}

		public SubsegmentSearchResult(TranslationUnit tmTu, short matchTokenIndex, short matchTokenCount, short queryTokenIndex, short queryTokenCount, short translationTokenIndex, short translationTokenCount, SubsegmentMatchType matchType, string translationFeatureString, float confidence)
			: base(tmTu)
		{
			QueryTokenIndex = queryTokenIndex;
			QueryTokenCount = queryTokenCount;
			MatchTokenIndex = matchTokenIndex;
			MatchTokenCount = matchTokenCount;
			TranslationTokenIndex = translationTokenIndex;
			TranslationTokenCount = translationTokenCount;
			MatchType = matchType;
			TranslationFeatureString = translationFeatureString;
			Confidence = confidence;
			_repetitions = 1;
		}
	}
}
