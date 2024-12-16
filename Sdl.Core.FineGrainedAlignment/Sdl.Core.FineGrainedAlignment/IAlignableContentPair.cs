using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface IAlignableContentPair
	{
		AlignableContentPairId Id
		{
			get;
		}

		LiftAlignedSpanPairSet AlignmentData
		{
			get;
			set;
		}

		DateTime? TranslationModelDate
		{
			get;
			set;
		}

		List<Token> SourceTokens
		{
			get;
		}

		List<Token> TargetTokens
		{
			get;
		}
	}
}
