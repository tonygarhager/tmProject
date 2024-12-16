using System;

namespace Sdl.Core.FineGrainedAlignment
{
	public class TranslationModelProgressEventArgs : EventArgs
	{
		private readonly TranslationModelProgressStage _ProgressStage;

		private readonly int _ProgressNumber;

		private readonly int _ProgressLimit;

		public TranslationModelProgressStage ProgressStage => _ProgressStage;

		public int ProgressNumber => _ProgressNumber;

		public int ProgressLimit => _ProgressLimit;

		public bool Cancel
		{
			get;
			set;
		}

		public TranslationModelProgressEventArgs(TranslationModelProgressStage progressStage, int progressNumber, int progressLimit)
		{
			_ProgressStage = progressStage;
			_ProgressNumber = progressNumber;
			_ProgressLimit = progressLimit;
		}
	}
}
