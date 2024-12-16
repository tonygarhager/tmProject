namespace Sdl.Core.Processing.Alignment.Api
{
	public enum ReverseAlignmentPhase
	{
		Start = 0,
		ReadXliffTarget = 20,
		ReadUpdatedTarget = 40,
		MergingTargets = 50,
		ReverseAligning = 90,
		PostReverseAlignmentProcessing = 100,
		Finish = 100
	}
}
