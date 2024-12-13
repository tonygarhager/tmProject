using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the additional related objects that can be retrieved when retrieving one or more language resource templates.
	/// </summary>
	[Flags]
	public enum LanguageResourcesTemplateProperties
	{
		/// <summary>
		/// Retrieve just the language resources template, but no additional objects.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Retrieve all the language resources contained in the language resources template. Note that this does not include the actual data of the language resources.
		/// </summary>
		LanguageResources = 0x1,
		/// <summary>
		/// Retrieve the data of the language resources in addition to the language resources themselves. 
		/// See <see cref="F:Sdl.LanguagePlatform.TranslationMemoryApi.LanguageResourcesTemplateProperties.LanguageResources" />.
		/// </summary>
		LanguageResourcesData = 0x2,
		/// <summary>
		/// Retrieve all the translation memories associated with the language resources template.
		/// </summary>
		TranslationMemories = 0x4,
		/// <summary>
		/// Retrieve all the language resources including data contained in and translation memories associated with the language resources template.
		/// </summary>
		All = 0x7
	}
}
