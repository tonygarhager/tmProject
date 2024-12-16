namespace Sdl.Core.Processing.Alignment.Api
{
	public enum AlignmentPhase
	{
		Start = 0,
		ReadLeftFile = 20,
		ReadRightFile = 40,
		MergingFiles = 50,
		Aligning = 90,
		PostAlignmentProcessing = 100,
		Finish = 100
	}
}
