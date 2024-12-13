using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a collection of input translation memories (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory" />) in a migration project (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject" />).
	/// </summary>
	public interface IInputTranslationMemoryCollection : ICollection<IInputTranslationMemory>, IEnumerable<IInputTranslationMemory>, IEnumerable
	{
		/// <summary>
		/// Gets the input translation memory at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The input translation memory.</returns>
		IInputTranslationMemory this[int index]
		{
			get;
		}

		/// <summary>
		/// Adds a legacy translation memory as input to the collection.
		/// </summary>
		/// <param name="legacyTm">A legacy translation memory. See <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IBaseTranslationMemoryMigrationManager" /> for methods to obtain
		/// representations of various types of legacy translation memories.</param>
		/// <returns>A new <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory" /> object, which represents the legacy translation memory 
		/// in the migration project that owns this collection.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if the <paramref name="legacyTm" /> parameter is null.</exception>
		IInputTranslationMemory Add(ILegacyTranslationMemory legacyTm);
	}
}
