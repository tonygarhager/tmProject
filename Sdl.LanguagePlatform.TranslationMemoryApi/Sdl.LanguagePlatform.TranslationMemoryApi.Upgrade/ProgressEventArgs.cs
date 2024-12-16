using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class ProgressEventArgs : CancelEventArgs
	{
		public int PercentComplete
		{
			get;
			set;
		}

		public string InfoMessage
		{
			get;
			set;
		}

		public ProgressEventArgs(int percentComplete)
		{
			PercentComplete = percentComplete;
		}

		public ProgressEventArgs(int percentComplete, string infoMessage)
		{
			PercentComplete = percentComplete;
			InfoMessage = infoMessage;
		}
	}
}
