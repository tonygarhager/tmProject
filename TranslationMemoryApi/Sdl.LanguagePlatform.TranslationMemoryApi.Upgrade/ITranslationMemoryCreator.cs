namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents an object that can create translation memories. This is used to create output translation memories (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.TranslationMemoryCreator" />).
	/// </summary>
	public interface ITranslationMemoryCreator
	{
		/// <summary>
		/// Gets a friendly description of the output translation memory, which can be used to display to the user.
		/// </summary>
		string DisplayName
		{
			get;
		}

		/// <summary>
		/// MaximumNameLength represents the maximum name length that is supported.
		/// </summary>
		int MaximumNameLength
		{
			get;
		}

		/// <summary>
		/// MaximumCopyrightLength represents the maximum copyright length that is supported.
		/// </summary>
		int MaximumCopyrightLength
		{
			get;
		}

		/// <summary>
		/// MaximumDescriptionLength represents the maximum description length that is supported.
		/// </summary>
		int MaximumDescriptionLength
		{
			get;
		}

		/// <summary>
		/// Creates a new empty translation memory based on the specified setup information.
		/// </summary>
		/// <param name="setup">The setup information.</param>
		/// <returns>The newly created translation memory.</returns>
		ITranslationMemory CreateEmptyTranslationMemory(ITranslationMemorySetupOptions setup);

		/// <summary>
		/// Determines whether the translation memory creator is valid. It returns false and a user-friendly localised error message if it is not.
		/// </summary>
		/// <param name="errorMessage">An error message</param>
		/// <returns>whether translation memory creator valid</returns>
		bool IsValid(out string errorMessage);

		/// <summary>
		/// Determines whether the translation memory name is valid. It returns false and a user-friendly localised error message if it is not.
		/// </summary>
		/// <param name="translationMemoryName">translation memory name</param>
		/// <param name="errorMessage">error message</param>
		/// <returns>whether the translation memory name is valid</returns>
		bool IsValidName(string translationMemoryName, out string errorMessage);

		/// <summary>
		/// Determines whether the translation memory copyright is valid. It returns false and a user-friendly localised error message if it is not.
		/// </summary>
		/// <param name="translationMemoryCopyright">translation memory copyright</param>
		/// <param name="errorMessage">error message</param>
		/// <returns>whether the translation memory copyright is valid</returns>
		bool IsValidCopyright(string translationMemoryCopyright, out string errorMessage);

		/// <summary>
		/// Determines whether the translation memory description is valid. It returns false and a user-friendly localised error message if it is not.
		/// </summary>
		/// <param name="translationMemoryDescription">translation memory description</param>
		/// <param name="errorMessage">error message</param>
		/// <returns>whether the translation memory description is valid</returns>
		bool IsValidDescription(string translationMemoryDescription, out string errorMessage);
	}
}
