using Sdl.Core.Processing.Alignment.Common;
using System;
using System.IO;

namespace Sdl.Core.Processing.Alignment.Api
{
	public class AlignmentResults
	{
		public AlignmentFilePair FilePair
		{
			get;
			private set;
		}

		public Exception LastError
		{
			get;
			private set;
		}

		public TimeSpan Duration
		{
			get;
			private set;
		}

		public AlignmentStatistics AlignmentStatistics
		{
			get;
			private set;
		}

		public AlignmentResults(AlignmentFilePair filePair, AlignmentStatistics alignmentStatistics, TimeSpan duration)
		{
			FilePair = filePair;
			AlignmentStatistics = alignmentStatistics;
			Duration = duration;
		}

		public AlignmentResults(AlignmentFilePair filePair, Exception lastError)
			: this(filePair, null, new TimeSpan(0L))
		{
			LastError = lastError;
		}

		public override string ToString()
		{
			return $"filepair[{Path.GetFileName(FilePair.LeftInputFilename)},{Path.GetFileName(FilePair.RightInputFilename)}] => alignmentCount={AlignmentStatistics.TotalAlignmentsCount} / duration = {Duration}";
		}
	}
}
