using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface IAlignerService
	{
		int Align(AlignerDefinition alignerDefinition, Dictionary<AlignableCorpusId, List<AlignableContentPairId>> pairIdsByCorpusId);
	}
}
