using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the additional related objects that can be retrieved when retrieving one or more translation memories.
	/// </summary>
	[Flags]
	public enum TranslationMemoryProperties
	{
		/// <summary>
		/// Retrieve just the translation memory, but no additional objects.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Retrieve the language directions.
		/// </summary>
		LanguageDirections = 0x1,
		/// <summary>
		/// Retrieve the fields of the translation memory or the fields of the fields template if the translation memory
		/// is associated with a fields template.
		/// </summary>
		Fields = 0x2,
		/// <summary>
		/// Retrieve the language resources of the translation memory or the language resources of the language resources template if the translation memory
		/// is associated with a language resources template. Note that this does not include the actual data of the language resource.
		/// </summary>
		LanguageResources = 0x4,
		/// <summary>
		/// Retrieve the data of the language resources in addition to the language resources of the translation memory. 
		/// See <see cref="F:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryProperties.LanguageResources" />.
		/// </summary>
		LanguageResourceData = 0x8,
		/// <summary>
		/// Retrieve the container that contains the translation memory.
		/// </summary>
		Container = 0x10,
		/// <summary>
		/// Retrieve the current scheduled oprations for the translation memory.
		/// </summary>
		ScheduledOperations = 0x20,
		/// <summary>
		/// Retrieve all the related objects of the translation memory.
		/// </summary>
		All = 0x3F
	}
}
