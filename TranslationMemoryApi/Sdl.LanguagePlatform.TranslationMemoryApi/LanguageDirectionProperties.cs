using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the additional related objects that can be retrieved when retrieving one or more language directions.
	/// </summary>
	[Flags]
	public enum LanguageDirectionProperties
	{
		/// <summary>
		/// Retrieve just the language direction, but no additional objects.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Retrive the container for this language direction
		/// </summary>
		Container = 0x1,
		/// <summary>
		/// Retrive the fields associated with the language direction.
		/// </summary>
		Fields = 0x2,
		/// <summary>
		/// Retrieve the language resources associated with the language direction.
		/// </summary>
		LanguageResources = 0x4,
		/// <summary>
		/// Retrieve the import operations associated with this language direction
		/// </summary>
		AssociatedImports = 0x8,
		/// <summary>
		/// Retrieve the export operations associated with this language direction
		/// </summary>
		AssociatedExports = 0x10,
		/// <summary>
		/// Retrieve all the related objects of the language direction.
		/// </summary>
		All = 0x1E
	}
}
