using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a mapping between field names of a translation memory. This is typically used to list
	/// field names which should be renamed, retyped, or deleted during migration.
	/// </summary>
	public interface IFieldIdentifierMappings
	{
		/// <summary>
		/// Gets the input translation memory.
		/// </summary>
		IInputTranslationMemory TranslationMemory
		{
			get;
		}

		/// <summary>
		/// Represents the field identifier mappings (old field identifier, new field identifier).
		/// <para>
		/// Field identifier mapping maps the input (existing) field identifier to an output (target) 
		/// field identifier, allowing invalid or duplicated field names in legacy translation memories 
		/// to be renamed to valid field names and imported into an output translation memory.
		/// </para>
		/// </summary>
		IDictionary<FieldIdentifier, FieldIdentifier> FieldIdentifierMappings
		{
			get;
		}
	}
}
