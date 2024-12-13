using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// IAdvancedContextTranslationMemoryLanguageDirection interface
	/// </summary>
	public interface IAdvancedContextTranslationMemoryLanguageDirection
	{
		/// <summary>
		/// Identical to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemoryLanguageDirection.GetTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.RegularIterator@)" /> except that the implementation will
		/// attempt to populate the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment1" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment2" /> properties for
		/// any items in the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationUnit.Contexts" /> collection of a tu
		/// </summary>
		/// <param name="iterator"></param>
		/// <returns></returns>
		/// <remarks>Implementations must ensure <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment1" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TuContext.Segment2" /> are tokenized and that their <see cref="P:Sdl.LanguagePlatform.Core.Segment.Tokens" /> collections are returned to the client.</remarks>
		TranslationUnit[] GetTranslationUnitsWithContextContent(ref RegularIterator iterator);
	}
}
