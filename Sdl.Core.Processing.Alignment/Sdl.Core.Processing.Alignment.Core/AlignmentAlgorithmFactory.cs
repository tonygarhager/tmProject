using Sdl.Core.Processing.Alignment.Core.Algorithms;
using System;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentAlgorithmFactory
	{
		public IAlignmentAlgorithm CreateAlignmentAlgorithm(AlignmentAlgorithmSettings algorithmSettings)
		{
			if (algorithmSettings == null)
			{
				throw new ArgumentNullException("algorithmSettings");
			}
			switch (algorithmSettings.AlgorithmType)
			{
			case AlignmentAlgorithmType.IdenticalCultures:
				return new RetrofitAlignmentAlgorithm(algorithmSettings);
			case AlignmentAlgorithmType.Version2:
				return new VersionTwoAlgorithm(algorithmSettings);
			case AlignmentAlgorithmType.Version1:
				return new VersionOneAlgorithm(algorithmSettings);
			case AlignmentAlgorithmType.Fast:
				return new GaleAndChurchAlgorithm(algorithmSettings);
			case AlignmentAlgorithmType.Optimistic:
				return new OptimisticAlgorithm(algorithmSettings);
			case AlignmentAlgorithmType.Accurate:
				return new HunAlignAlgorithm(algorithmSettings, algorithmSettings.BilingualDictionary, algorithmSettings.UpdateBilingualDictionary);
			case AlignmentAlgorithmType.Position:
				return new PositionAlignmentAlgorithm();
			default:
				throw new Exception("Algorithm type not supported");
			}
		}
	}
}
