using Sdl.Core.FineGrainedAlignment;
using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class AlignableTu : IAlignableContentPair
	{
		private readonly AlignableTuId _id;

		public AlignableContentPairId Id => _id;

		public LiftAlignedSpanPairSet AlignmentData
		{
			get
			{
				return TU.AlignmentData;
			}
			set
			{
				TU.AlignmentData = value;
			}
		}

		public DateTime? TranslationModelDate
		{
			get
			{
				return TU.AlignModelDate;
			}
			set
			{
				TU.AlignModelDate = value;
			}
		}

		public List<Token> SourceTokens => TU.SourceSegment.Tokens;

		public List<Token> TargetTokens => TU.TargetSegment.Tokens;

		public TranslationUnit TU
		{
			get;
		}

		public AlignableTu(TranslationUnit tu)
		{
			TU = tu;
			_id = new AlignableTuId
			{
				ResourceId = tu.ResourceId
			};
		}
	}
}
