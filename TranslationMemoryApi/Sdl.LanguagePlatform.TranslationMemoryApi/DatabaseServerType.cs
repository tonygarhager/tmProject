namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the supported types of database server that can be used to host server-based translation memory containers.
	/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.DatabaseServer.ServerType" />.
	/// </summary>
	public enum DatabaseServerType
	{
		/// <summary>
		/// Unknown database server type.
		/// </summary>
		Unknown,
		/// <summary>
		/// Microsoft SQL database server.
		/// </summary>
		SqlServer,
		/// <summary>
		/// Oracle database server.
		/// </summary>
		Oracle
	}
}
