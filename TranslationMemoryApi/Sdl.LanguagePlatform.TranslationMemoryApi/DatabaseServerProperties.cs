using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the additional related objects that can be retrieved when retrieving one or more database servers.
	/// </summary>
	[Flags]
	public enum DatabaseServerProperties
	{
		/// <summary>
		/// Retrieve just the database server, but no additional objects.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Retrieve the translation memory containers hosted on the database server.
		/// </summary>
		Containers = 0x1,
		/// <summary>
		/// Retrieve the translation memory containers hosted on the database server and the translation memories present in the containers.
		/// </summary>
		ContainerTranslationMemories = 0x2,
		/// <summary>
		/// Retrieve the translation memory containers hosted on the database server and the translation memories present in the containers.
		/// </summary>
		All = 0x3
	}
}
