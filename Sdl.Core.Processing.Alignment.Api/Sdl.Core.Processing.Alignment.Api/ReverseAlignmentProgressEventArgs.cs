using System;

namespace Sdl.Core.Processing.Alignment.Api
{
	public class ReverseAlignmentProgressEventArgs : EventArgs
	{
		public AlignmentFilePair FilePair
		{
			get;
			private set;
		}

		public byte Progress
		{
			get;
			private set;
		}

		public ReverseAlignmentPhase Phase
		{
			get;
			private set;
		}

		public ReverseAlignmentProgressEventArgs()
		{
		}

		public ReverseAlignmentProgressEventArgs(ReverseAlignmentPhase alignmentPhase, byte progress, params object[] info)
		{
			if (progress > 100)
			{
				progress = 100;
			}
			if (info != null && info[0] != null)
			{
				try
				{
					FilePair = (AlignmentFilePair)info[0];
				}
				catch
				{
					throw new Exception("AlignmentFilePair");
				}
			}
			Progress = progress;
			Phase = alignmentPhase;
		}

		public override string ToString()
		{
			return string.Format("ReverseAlignment progress: {3} - {0}%, file pair \"{1}\", \"{2}\"", Progress, FilePair.LeftInputFilename ?? "(null)", FilePair.RightInputFilename ?? "(null)", Phase);
		}
	}
}
