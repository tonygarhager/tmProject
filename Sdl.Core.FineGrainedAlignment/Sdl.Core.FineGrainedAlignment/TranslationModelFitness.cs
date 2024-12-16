using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public class TranslationModelFitness
	{
		public Dictionary<string, int> SourceOOVTokenCounts
		{
			get;
			set;
		}

		public Dictionary<string, int> SourceIVTokenCounts
		{
			get;
			set;
		}

		public Dictionary<string, int> TargetOOVTokenCounts
		{
			get;
			set;
		}

		public Dictionary<string, int> TargetIVTokenCounts
		{
			get;
			set;
		}
	}
}
