namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents information about the status of a translation provider.
	/// </summary>
	public class ProviderStatusInfo
	{
		/// <summary>
		/// Gets or sets a flag whether the provider is available or not.
		/// </summary>
		public bool Available
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a status message (may be null).
		/// </summary>
		public string StatusMessage
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance with the specified values.
		/// </summary>
		/// <param name="available">The availability status</param>
		/// <param name="statusMessage">The status message</param>
		public ProviderStatusInfo(bool available, string statusMessage)
		{
			Available = available;
			StatusMessage = statusMessage;
		}
	}
}
