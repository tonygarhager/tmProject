using System.Drawing;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents information about a translation provider (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider" />) that can be used to display 
	/// this translation provider in a user interface. The translation provider plug-in has to provide an implementation of <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderWinFormsUI" />
	/// which supports getting display information through calling <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderWinFormsUI.GetDisplayInfo(System.Uri,System.String)" />.
	/// </summary>
	public class TranslationProviderDisplayInfo
	{
		/// <summary>
		/// Gets the display name of the translation provider.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets descriptive text that can be shown in a tooltip or similar.
		/// </summary>
		public string TooltipText
		{
			get;
			set;
		}

		/// <summary>
		/// Gets an image that can be used to represent the fact that a certain search result (<see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResult" />)
		/// was generated by this particular translation provider.
		/// </summary>
		public Image SearchResultImage
		{
			get;
			set;
		}

		/// <summary>
		/// Gets an image that can be used to represent this translation provider.
		/// </summary>
		public Icon TranslationProviderIcon
		{
			get;
			set;
		}
	}
}
