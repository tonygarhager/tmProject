namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a factory object used to create a file-based SDL Trados Studio 2009 translation memory.
	/// </summary>
	public interface IFileBasedTranslationMemoryCreator : ITranslationMemoryCreator
	{
		/// <summary>
		/// Gets or sets the directory where the translation memory should be created.
		/// </summary>
		string TranslationMemoryDirectory
		{
			get;
			set;
		}
	}
}
