namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Permissions for language resources in a TM.
	/// </summary>
	public static class TranslationMemoryFieldsPermissions
	{
		/// <summary>
		/// Can read a fields template
		/// </summary>
		public const string ReadResource = "tmfields.view";

		/// <summary>
		/// Can add a fields template
		/// </summary>
		public const string AddResource = "tmfields.add";

		/// <summary>
		/// Can update a fields template
		/// </summary>
		public const string WriteResource = "tmfields.edit";

		/// <summary>
		/// Can delete a fields template
		/// </summary>
		public const string DeleteResource = "tmfields.delete";
	}
}
