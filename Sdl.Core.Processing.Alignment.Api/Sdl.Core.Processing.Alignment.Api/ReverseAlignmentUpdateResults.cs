namespace Sdl.Core.Processing.Alignment.Api
{
	public class ReverseAlignmentUpdateResults
	{
		public int UpdatedSegmentCount
		{
			get;
			private set;
		}

		public int ErrorSegmentCount
		{
			get;
			private set;
		}

		public ReverseAlignmentUpdateResults(int updateSegmentCount, int errorUpdateSegmentsCount)
		{
			UpdatedSegmentCount = updateSegmentCount;
			ErrorSegmentCount = errorUpdateSegmentsCount;
		}

		public override string ToString()
		{
			return $"Success Count={UpdatedSegmentCount}; error count={ErrorSegmentCount}";
		}
	}
}
