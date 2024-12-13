namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Translation unit permissions.
	/// </summary>
	public static class TranslationUnitPermissions
	{
		/// <summary>
		/// User can add or update TUs
		/// </summary>
		public const string WriteTU = "tm.writetu";

		/// <summary>
		/// User can read TUs
		/// </summary>
		public const string ReadTU = "tm.readtu";

		/// <summary>
		/// User can delete TUs
		/// </summary>
		public const string DeleteTU = "tm.deletetu";

		/// <summary>
		/// Can apply a batch edit script to modify some or all TUs in the TM
		/// </summary>
		public const string BatchEditTU = "tm.batchedittu";

		/// <summary>
		/// Can apply a batch delete script to delete all TUs in the TM matching a particular filter expression.
		/// </summary>
		public const string BatchDeleteTU = "tm.batchdeletetu";

		/// <summary>
		/// Can import TUs into the TM
		/// </summary>
		public const string ImportTU = "tm.importtu";

		/// <summary>
		/// Can export TUs from the TM
		/// </summary>
		public const string ExportTU = "tm.exporttu";

		/// <summary>
		/// Can export TUs from the TM
		/// </summary>
		public const string ReindexTU = "tm.reindextu";
	}
}
