using Sdl.Core.Processing.Alignment.Tokenization;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.Anchoring
{
	internal abstract class AbstractAnchoringStrategy : IAnchoringStrategy
	{
		public TokensContainer TokensContainer
		{
			get;
			set;
		}

		public abstract IList<AlignmentData> GetAnchors(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements);
	}
}
