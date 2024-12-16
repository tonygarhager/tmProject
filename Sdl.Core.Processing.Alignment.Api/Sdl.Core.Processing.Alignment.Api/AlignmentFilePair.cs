using System;

namespace Sdl.Core.Processing.Alignment.Api
{
	public class AlignmentFilePair
	{
		public string LeftInputFilename
		{
			get;
			private set;
		}

		public string RightInputFilename
		{
			get;
			private set;
		}

		public string OutputFilename
		{
			get;
			private set;
		}

		public AlignmentFilePair(string leftInputFilename, string rightInputFilename, string outputFilename = "")
		{
			if (leftInputFilename == null)
			{
				throw new ArgumentNullException("leftInputFilename");
			}
			if (rightInputFilename == null)
			{
				throw new ArgumentNullException("rightInputFilename");
			}
			if (outputFilename == null)
			{
				throw new ArgumentNullException("outputFilename");
			}
			LeftInputFilename = leftInputFilename;
			RightInputFilename = rightInputFilename;
			OutputFilename = outputFilename;
		}

		public override bool Equals(object obj)
		{
			AlignmentFilePair alignmentFilePair;
			if (obj == null || (alignmentFilePair = (obj as AlignmentFilePair)) == null)
			{
				return false;
			}
			if (string.Equals(LeftInputFilename, alignmentFilePair.LeftInputFilename, StringComparison.Ordinal) && string.Equals(RightInputFilename, alignmentFilePair.RightInputFilename, StringComparison.Ordinal))
			{
				return string.Equals(OutputFilename, alignmentFilePair.OutputFilename, StringComparison.Ordinal);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (LeftInputFilename.GetHashCode() * 983) ^ (RightInputFilename.GetHashCode() * 569) ^ OutputFilename.GetHashCode();
		}
	}
}
