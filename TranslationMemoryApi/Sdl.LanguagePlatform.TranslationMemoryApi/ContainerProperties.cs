using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the additional related objects that can be retrieved when retrieving one or more translation memory containers.
	/// </summary>
	[Flags]
	public enum ContainerProperties
	{
		/// <summary>
		/// Retrieve just the translation memeory container, but no additional objects.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Retrieve the database server to which the container belongs.
		/// </summary>
		DatabaseServer = 0x1,
		/// <summary>
		/// Retrieve the translation memories present in the container.
		/// </summary>
		TranslationMemories = 0x2,
		/// <summary>
		/// Retrieve the database server to which the container belongs and the translation memories present in the container.
		/// </summary>
		All = 0x3
	}
}
