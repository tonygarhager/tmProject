using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a language resources template, which is a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.LanguageResourceBundleCollection" />
	/// that can be stored and managed individually. 
	/// </summary>
	/// <remarks>There are two implementations of this interface: <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedLanguageResourcesTemplate" /> and <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate" />.</remarks>
	public interface ILanguageResourcesTemplate
	{
		/// <summary>
		/// Gets or sets the name of the language resources template.
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the description of the language resources template.
		/// </summary>
		string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the collection of language resource bundles associated with this language resource template.
		/// </summary>
		LanguageResourceBundleCollection LanguageResourceBundles
		{
			get;
		}

		/// <summary>
		/// Gets or sets the recognizers which are enabled for this language resource template. 
		/// </summary>
		BuiltinRecognizers? Recognizers
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the flags affecting word count behaviour for this language resource template.
		/// </summary>
		WordCountFlags? WordCountFlags
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the flags affecting tokenizer behaviour for this language resource template.
		/// </summary>
		TokenizerFlags? TokenizerFlags
		{
			get;
			set;
		}
	}
}
