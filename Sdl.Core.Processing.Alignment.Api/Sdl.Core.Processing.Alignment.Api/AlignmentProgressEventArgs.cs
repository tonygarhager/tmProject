using System;

namespace Sdl.Core.Processing.Alignment.Api
{
	public class AlignmentProgressEventArgs : EventArgs
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

		public AlignmentPhase Phase
		{
			get;
			private set;
		}

		public AlignmentProgressEventArgs()
		{
		}

		public AlignmentProgressEventArgs(AlignmentPhase alignmentPhase, byte progress, params object[] info)
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
			return string.Format("Alignment progress: {3} - {0}%, file pair \"{1}\", \"{2}\"", Progress, FilePair.LeftInputFilename ?? "(null)", FilePair.RightInputFilename ?? "(null)", Phase);
		}
	}
}
