namespace Sdl.Core.Processing.Alignment.Api
{
	public enum ImportSdlRetrofitPhase
	{
		Start = 0,
		ExtractFiles = 20,
		ReadSettings = 25,
		ReadAlignments = 30,
		ReadContents = 45,
		ProcessData = 55,
		ReadMapping = 70,
		ReadTarget = 85,
		Finish = 100
	}
}
