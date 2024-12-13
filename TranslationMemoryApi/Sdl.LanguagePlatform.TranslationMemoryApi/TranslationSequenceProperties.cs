using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the additional related objects that can be retrieved when retrieving one or more translation sequences.
	/// </summary>
	[Flags]
	public enum TranslationSequenceProperties
	{
		/// <summary>
		/// Retrieve just the translation sequence, but no additional objects.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Retrieve all the translation sequence items.
		/// </summary>
		Items = 0x1,
		/// <summary>
		/// Retrieve all the translation sequence items.
		/// </summary>
		All = 0x1
	}
}
