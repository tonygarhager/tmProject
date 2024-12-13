using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Encapsulates the data associated with cancellable progress events.
	/// </summary>
	public class ProgressEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Gets or sets the percentage.
		/// </summary>
		public int PercentComplete
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a message representing detailed information about the current operation.
		/// </summary>
		public string InfoMessage
		{
			get;
			set;
		}

		/// <summary>
		/// Creates a new ProgressEventArgs object and initializes it with the specified value.
		/// </summary>
		/// <param name="percentComplete">A value between 0 and 100.</param>
		public ProgressEventArgs(int percentComplete)
		{
			PercentComplete = percentComplete;
		}

		/// <summary>
		/// Creates a new ProgressEventArgs object and initializes it with the specified values.
		/// </summary>
		/// <param name="percentComplete">A value between 0 and 100.</param>
		/// <param name="infoMessage">The message, representing detailed information about the current operation.</param>
		public ProgressEventArgs(int percentComplete, string infoMessage)
		{
			PercentComplete = percentComplete;
			InfoMessage = infoMessage;
		}
	}
}
