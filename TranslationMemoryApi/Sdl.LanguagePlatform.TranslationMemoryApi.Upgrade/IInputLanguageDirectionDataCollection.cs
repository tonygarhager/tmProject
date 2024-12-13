using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a collection of input language data (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputLanguageDirectionData" />), 
	/// which makes up the content that will be imported into a single output translation 
	/// memory (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.InputLanguageDirections" />).
	/// </summary>
	public interface IInputLanguageDirectionDataCollection : ICollection<IInputLanguageDirectionData>, IEnumerable<IInputLanguageDirectionData>, IEnumerable
	{
		/// <summary>
		/// Gets the input language direction data at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The input language direction data object at the specified index.</returns>
		IInputLanguageDirectionData this[int index]
		{
			get;
		}

		/// <summary>
		/// Adds a language direction of a legacy translation memory to the collection.
		/// </summary>
		/// <param name="translationMemory">The legacy translation memory.</param>
		/// <param name="languageDirectionData">One of the available language direction data from the legacy translation memory.</param>
		/// <returns>A new <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputLanguageDirectionData" /> object, representing the language direction data of the
		/// legacy translation memory as input to a single output translation memory within a migration project.</returns>
		IInputLanguageDirectionData Add(IInputTranslationMemory translationMemory, ILegacyLanguageDirectionData languageDirectionData);

		/// <summary>
		/// Moves the input language direction data from <paramref name="fromIndex" /> to <paramref name="toIndex" />.
		/// </summary>
		/// <remarks>The content will be imported into the output translation memory in the order
		/// that input language direction data objects appear in this collection.</remarks>
		/// <param name="fromIndex">The index from which to move the object.</param>
		/// <param name="toIndex">The index to which to move the object.</param>
		void Move(int fromIndex, int toIndex);
	}
}
