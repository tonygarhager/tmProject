using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class SignificantAlignableContentPair : IAlignableContentPair
	{
		private readonly List<Token> _sourceTokens = new List<Token>();

		private readonly List<Token> _targetTokens = new List<Token>();

		private readonly LiftAlignedSpanPairSet _significantAlignedSpanPairSet;

		public LiftAlignedSpanPairSet AlignmentData
		{
			get
			{
				return _significantAlignedSpanPairSet;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public AlignableContentPairId Id
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public List<Token> SourceTokens => _sourceTokens;

		public List<Token> TargetTokens => _targetTokens;

		public DateTime? TranslationModelDate
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public SignificantAlignableContentPair(IAlignableContentPair pair, LiftAlignedSpanPairSet significantAlignedSpanPairSet, List<short> significantSourceTokenIndices, List<short> significantTargetTokenIndices)
		{
			foreach (short significantSourceTokenIndex in significantSourceTokenIndices)
			{
				_sourceTokens.Add(pair.SourceTokens[significantSourceTokenIndex]);
			}
			foreach (short significantTargetTokenIndex in significantTargetTokenIndices)
			{
				_targetTokens.Add(pair.TargetTokens[significantTargetTokenIndex]);
			}
			_significantAlignedSpanPairSet = significantAlignedSpanPairSet;
		}
	}
}
