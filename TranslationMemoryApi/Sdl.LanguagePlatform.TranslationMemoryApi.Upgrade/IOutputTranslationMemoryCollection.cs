using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a collection of output translation memories (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory" />) in a migration project (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject" />).
	/// </summary>
	public interface IOutputTranslationMemoryCollection : ICollection<IOutputTranslationMemory>, IEnumerable<IOutputTranslationMemory>, IEnumerable
	{
		/// <summary>
		/// Gets the output translation memory at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The output translation memory at the specified index.</returns>
		IOutputTranslationMemory this[int index]
		{
			get;
		}

		/// <summary>
		/// Adds a new output translation memory to the collection. You can add input language 
		/// directions to its <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.InputLanguageDirections" /> collection.
		/// </summary>
		/// <returns>A representation of an output translation memory.</returns>
		IOutputTranslationMemory Add();

		/// <summary>
		/// Adds an output translation memory for a bilingual legacy input translation memory and 
		/// optionally populates the setup information of the output translation memory (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.Setup" />)
		/// based of the setup of the input translation memory (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.Setup" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyLanguageDirectionData.AvailableLanguageResources" />).
		/// </summary>
		/// <param name="inputTranslationMemory">The language direction data, representing the content of a legacy TM to be migrated to the output TM.</param>
		/// <param name="autoPopulateOutputTranslationMemorySetup">Whether to automatically popluate the setup information of the output
		/// translation memory based on the setup information from the input translation memory.</param>
		/// <returns>An <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory" /> object, containing the specified input language direction data.</returns>
		/// <exception cref="T:System.ArgumentException">Thrown if the input translation memory has more than one language direction.</exception>
		IOutputTranslationMemory Add(IInputTranslationMemory inputTranslationMemory, bool autoPopulateOutputTranslationMemorySetup);

		/// <summary>
		/// Adds an output translation memory for a number of bilingual legacy input translation 
		/// memories and optionally populates the setup information of the output translation memory 
		/// (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.Setup" />)
		/// based of the setup of the input translation memories (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.Setup" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyLanguageDirectionData.AvailableLanguageResources" />).
		/// </summary>
		/// <param name="inputTranslationMemory">The language directions, representing the bilingual content of a number of legacy TMs to be migrated to the output TM.</param>
		/// <param name="autoPopulateOutputTranslationMemorySetup">Whether to automatically popluate the setup information of the output
		/// translation memory based on the merged setup information from the input translation memories.</param>
		/// <returns>An <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory" /> object, containing the specified input language directions.</returns>
		/// <exception cref="T:System.ArgumentException">Thrown if one of the input translation memories has more than one language direction.</exception>
		IOutputTranslationMemory Add(IEnumerable<IInputTranslationMemory> inputTranslationMemory, bool autoPopulateOutputTranslationMemorySetup);
	}
}
