namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Permissions for language resources in a TM.
	/// </summary>
	public static class TranslationMemoryLanguageResourcePermissions
	{
		/// <summary>
		/// Can create a language resource like abbreviation, stop-word lists, etc.
		/// </summary>
		public const string AddResource = "tmlangresource.add";

		/// <summary>
		/// Can read a language resource like abbreviation, stop-word lists, etc.
		/// </summary>
		public const string ReadResource = "tmlangresource.view";

		/// <summary>
		/// Can update language resource like abbreviation, stop-word lists, etc.
		/// </summary>
		public const string WriteResource = "tmlangresource.edit";

		/// <summary>
		/// Can delete the resource.
		/// </summary>
		public const string DeleteResource = "tmlangresource.delete";
	}
}
