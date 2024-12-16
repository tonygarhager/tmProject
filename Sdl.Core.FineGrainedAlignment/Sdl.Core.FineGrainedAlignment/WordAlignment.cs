namespace Sdl.Core.FineGrainedAlignment
{
	internal class WordAlignment
	{
		public int otherWordIndex;

		public double confidence;

		public AlignmentDirection direction;

		public WordAlignment(AlignmentDirection dir)
		{
			direction = dir;
		}
	}
}
