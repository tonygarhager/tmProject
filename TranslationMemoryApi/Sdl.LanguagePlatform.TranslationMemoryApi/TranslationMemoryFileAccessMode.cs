namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// The possible "access modes" for a password protected file-based TM.
	/// A password can be set for each access mode.
	/// </summary>
	public enum TranslationMemoryFileAccessMode
	{
		/// <summary>
		/// Administrator-level access
		/// </summary>
		Administrator,
		/// <summary>
		/// Maintenance-level access
		/// </summary>
		Maintenance,
		/// <summary>
		/// Read-write access
		/// </summary>
		ReadWrite,
		/// <summary>
		/// Read-only access
		/// </summary>
		ReadOnly
	}
}
