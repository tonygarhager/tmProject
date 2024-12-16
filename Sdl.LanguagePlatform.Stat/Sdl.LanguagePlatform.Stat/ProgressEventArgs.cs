using System;

namespace Sdl.LanguagePlatform.Stat
{
	public class ProgressEventArgs : EventArgs
	{
		public ProgressStage ProgressStage
		{
			get;
		}

		public int ProgressNumber
		{
			get;
		}

		public bool Cancel
		{
			get;
			set;
		}

		public ProgressEventArgs(ProgressStage progressStage, int progressNumber)
		{
			ProgressStage = progressStage;
			ProgressNumber = progressNumber;
		}
	}
}
