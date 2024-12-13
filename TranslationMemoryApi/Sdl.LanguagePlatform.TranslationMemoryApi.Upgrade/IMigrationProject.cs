using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// A  migration project encapsulates functionality to migrate one or more legacy translation memories to one or more
	/// new translation memories. Use this object as follows:
	/// <list type="ordered">
	/// <item>Add all legacy translation memories to the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject.InputTranslationMemories" /> collection. A legacy translation memory can
	/// contain multiple language directions.</item>
	/// <item>Then populate the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject.OutputTranslationMemories" /> collection by adding groups of one or more
	/// bilingual input translation memories with compatible language directions to it. A new translation meory will be created for every
	/// output translation memory in the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject.OutputTranslationMemories" /> collection.</item>
	/// <item>Make sure the properties of the output translation memory are all set (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.Setup" />).</item>
	/// <item>Specify a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITranslationMemoryCreator" /> object for each of the output translation memories (see<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.TranslationMemoryCreator" />.</item>
	/// <item>Perform the migration process by either calling <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject.ProcessAll(System.EventHandler{Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ProgressEventArgs})" />, or by first exporting all input translation memories indiviually (see
	///  <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.ExportTmxFile(System.EventHandler{Sdl.LanguagePlatform.TranslationMemoryApi.BatchExportedEventArgs})" />) and then creating all output translation memoies (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.Process(System.EventHandler{Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ProgressEventArgs})" />).</item>
	/// <item>The newly created translation memories are available through the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.TranslationMemory" /> property.</item>
	/// </list>
	/// </summary>
	public interface IMigrationProject
	{
		/// <summary>
		/// The collection of input legacy translation memories.
		/// </summary>
		IInputTranslationMemoryCollection InputTranslationMemories
		{
			get;
		}

		/// <summary>
		/// The collection of output translation memories. A new translation memory will be created for every item in this collection.
		/// </summary>
		IOutputTranslationMemoryCollection OutputTranslationMemories
		{
			get;
		}

		/// <summary>
		/// Performs the full migration process by exporting all input translation memories and subsequently creating all
		/// output translation memories.
		/// </summary>
		/// <remarks>This method can be called multiple times and will pick up the migration where it failed or was cancelled previously.</remarks>
		/// <param name="progressEventHandler">An event handler through which to report progress events and through which the migration process can be cancelled.</param>
		void ProcessAll(EventHandler<ProgressEventArgs> progressEventHandler);
	}
}
