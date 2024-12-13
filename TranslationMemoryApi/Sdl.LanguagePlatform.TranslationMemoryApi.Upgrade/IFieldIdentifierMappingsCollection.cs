using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a collection of field identifier mappings.
	/// </summary>
	public interface IFieldIdentifierMappingsCollection : ICollection<IFieldIdentifierMappings>, IEnumerable<IFieldIdentifierMappings>, IEnumerable
	{
		/// <summary>
		/// Gets the field identifier mappings for the given translation memory.
		/// </summary>
		/// <param name="translationMemory">The translation memory</param>
		/// <returns>The field identifier mappings for the specified translation memory, or null if no
		/// mapping is available for the TM.</returns>
		IDictionary<FieldIdentifier, FieldIdentifier> GetFieldIdentifierMappings(IInputTranslationMemory translationMemory);

		/// <summary>
		/// Gets the field identifier mappings for the given translation memory.
		/// </summary>
		/// <param name="translationMemory">The translation memory</param>
		/// <returns>The <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IFieldIdentifierMappings" /> for the specified translation memory, or null if no
		/// mapping is available for the TM.</returns>
		IFieldIdentifierMappings GetInputFieldIdentifierMappings(IInputTranslationMemory translationMemory);
	}
}
