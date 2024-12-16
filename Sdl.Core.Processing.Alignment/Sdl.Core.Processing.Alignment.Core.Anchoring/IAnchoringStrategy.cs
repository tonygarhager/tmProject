using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.Anchoring
{
	internal interface IAnchoringStrategy
	{
		IList<AlignmentData> GetAnchors(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements);
	}
}
