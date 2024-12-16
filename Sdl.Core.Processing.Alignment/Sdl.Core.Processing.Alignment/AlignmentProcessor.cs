using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Core.Algorithms;
using Sdl.Core.Processing.Alignment.Core.Anchoring;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment
{
	internal class AlignmentProcessor : AbstractAlignmentProcessor
	{
		public AlignmentProcessor(IAlignmentAlgorithm alignmentAlgorithm, IEnumerable<AbstractAnchoringStrategy> anchoringStrategies, AlignmentAlgorithmSettings algorithmSettings)
		{
			Aligner = new GapAligner(alignmentAlgorithm, anchoringStrategies, algorithmSettings);
		}

		public AlignmentProcessor(AlignmentAlgorithmSettings algorithmSettings)
		{
			AlignmentAlgorithmFactory alignmentAlgorithmFactory = new AlignmentAlgorithmFactory();
			IAlignmentAlgorithm algorithm = alignmentAlgorithmFactory.CreateAlignmentAlgorithm(algorithmSettings);
			IList<AbstractAnchoringStrategy> list = new List<AbstractAnchoringStrategy>();
			bool flag = DocumentStructureAnchorSearch.SupportsAlignmentAlgorithm(algorithmSettings.AlgorithmType);
			bool flag2 = CognateAnchorSearch.SupportsCulture(algorithmSettings.LeftCulture) && CognateAnchorSearch.SupportsCulture(algorithmSettings.RightCulture) && CognateAnchorSearch.SupportsAlignmentAlgorithm(algorithmSettings.AlgorithmType);
			list.Add(ConfirmedAlignmentsAnchorStrategy);
			if (ExactMatchAnchorSearch.SupportsAlignmentAlgorithm(algorithmSettings.AlgorithmType))
			{
				list.Add(new ExactMatchAnchorSearch(algorithmSettings.LeftCulture, algorithmSettings.RightCulture));
			}
			if (flag2)
			{
				list.Add(new CognateAnchorSearch(algorithmSettings.LeftCulture, algorithmSettings.RightCulture));
			}
			if (TokenComparisonAnchorSearch.SupportsAlignmentAlgorithm(algorithmSettings.AlgorithmType))
			{
				list.Add(new TokenComparisonAnchorSearch(algorithmSettings.LeftCulture, algorithmSettings.RightCulture));
			}
			if (flag)
			{
				list.Add(new DocumentStructureAnchorSearch());
			}
			Aligner = new GapAligner(algorithm, list, algorithmSettings);
		}
	}
}
