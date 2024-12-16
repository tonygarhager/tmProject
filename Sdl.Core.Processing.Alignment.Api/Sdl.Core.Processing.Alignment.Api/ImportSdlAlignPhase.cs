namespace Sdl.Core.Processing.Alignment.Api
{
	public enum ImportSdlAlignPhase
	{
		Start = 0,
		ExtractFiles = 20,
		ReadSettings = 25,
		ReadAlignments = 30,
		ReadContents = 50,
		ProcessData = 75,
		Finish = 100
	}
}
