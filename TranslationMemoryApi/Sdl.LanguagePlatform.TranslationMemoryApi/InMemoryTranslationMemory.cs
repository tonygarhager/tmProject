using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a bilingual in-memory translation memory.
	/// </summary>
	public class InMemoryTranslationMemory : AbstractLocalTranslationMemory
	{
		/// <summary>
		/// Always returns "OK".
		/// </summary>
		public override ProviderStatusInfo StatusInfo => new ProviderStatusInfo(available: true, "OK");

		/// <summary>
		/// Creates a new bilingual in-memory translation memory.
		/// </summary>
		/// <param name="name">The translation memory name.</param>
		/// <param name="sourceLanguage">A region-qualified culture, representing the source language.</param>
		/// <param name="targetLanguage">A region-qualified culture, representing the target language.</param>
		/// <param name="indexes">The set of fuzzy indexes that should be created in this translation memory.</param>
		/// <param name="recognizers">Recognizer settings.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> is null or empty.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown when <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> are not region-qualified cultures.</exception>
		public InMemoryTranslationMemory(string name, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers)
			: base(new InMemoryTranslationMemoryDescriptor(name, sourceLanguage, targetLanguage, indexes, recognizers))
		{
		}

		/// <summary>
		/// Always returns <code>true</code>.
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		public override bool HasPermission(string permission)
		{
			return true;
		}

		/// <summary>
		/// Refreshes the current status information.
		/// </summary>
		public override void RefreshStatusInfo()
		{
		}
	}
}
