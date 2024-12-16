using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Core.Algorithms;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	internal class RetrofitAlignmentProcessor : AbstractAlignmentProcessor
	{
		public RetrofitAlignmentProcessor(AlignmentAlgorithmSettings algorithmSettings, MergeParagraphParser mergeParagraphParser, byte minimumAlignmentQuality)
		{
			AlignmentAlgorithmFactory alignmentAlgorithmFactory = new AlignmentAlgorithmFactory();
			IAlignmentAlgorithm algorithm = alignmentAlgorithmFactory.CreateAlignmentAlgorithm(algorithmSettings);
			Aligner = new ReverseGapAligner(algorithm, algorithmSettings, mergeParagraphParser, minimumAlignmentQuality);
		}
	}
}
