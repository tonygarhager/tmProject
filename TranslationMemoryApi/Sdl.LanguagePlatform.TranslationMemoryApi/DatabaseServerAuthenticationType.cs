namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the possible authentication types to use for with <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.DatabaseServer" />.
	/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.DatabaseServer.AuthenticationType" />.
	/// </summary>
	public enum DatabaseServerAuthenticationType
	{
		/// <summary>
		/// Unknown authentication type.
		/// </summary>
		Unknown,
		/// <summary>
		/// Windows authentication. Use the Windows credentials the application server is running as.
		/// </summary>
		Windows,
		/// <summary>
		/// Database authentication. Use the user name and password specified in <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.DatabaseServer.UserName" />
		/// and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.DatabaseServer.Password" />.
		/// </summary>
		Database
	}
}
