using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class BatchProgressEventArgs : EventArgs
	{
		public string FilePath
		{
			get;
			set;
		}

		public int FileNumber
		{
			get;
			set;
		}

		public int TotalFiles
		{
			get;
			set;
		}

		public byte FilePercentComplete
		{
			get;
			set;
		}

		public BatchProgressEventArgs(string filePath, int fileNumber, int totalFiles, byte filePercentComplete)
		{
			FilePath = filePath;
			FileNumber = fileNumber;
			TotalFiles = totalFiles;
			FilePercentComplete = filePercentComplete;
		}

		public override string ToString()
		{
			return $"File {FileNumber + 1} of {TotalFiles}: {FilePercentComplete}%";
		}
	}
}
