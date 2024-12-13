using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a translation memory.
	/// </summary>
	public interface ITranslationMemory2015 : ITranslationMemory, ITranslationProvider
	{
		/// <summary>
		/// Gets or sets the flags affecting tokenizer behaviour for this TM.
		/// <remarks>Note that changing tokenizer flags may require reindexing.</remarks>
		/// </summary>
		TokenizerFlags TokenizerFlags
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the flags affecting word count behaviour for this TM.
		/// </summary>
		WordCountFlags WordCountFlags
		{
			get;
			set;
		}
	}
}
