using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the additional related objects that can be retrieved when retrieving one or more fields templates.
	/// </summary>
	[Flags]
	public enum FieldsTemplateProperties
	{
		/// <summary>
		/// Retrieve just the fields template, but no additional objects.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Retrieve all the field definitions contained in the fields template.
		/// </summary>
		Fields = 0x1,
		/// <summary>
		/// Retrieve all the translation memories associated with the fields template.
		/// </summary>
		TranslationMemories = 0x2,
		/// <summary>
		/// Retrieve all the fields contained in and translation memories associated with the fields template.
		/// </summary>
		All = 0x3
	}
}
