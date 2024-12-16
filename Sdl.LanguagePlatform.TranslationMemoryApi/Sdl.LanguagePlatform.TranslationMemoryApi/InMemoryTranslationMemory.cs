using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class InMemoryTranslationMemory : AbstractLocalTranslationMemory
	{
		public override ProviderStatusInfo StatusInfo => new ProviderStatusInfo(available: true, "OK");

		public InMemoryTranslationMemory(string name, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers)
			: base(new InMemoryTranslationMemoryDescriptor(name, sourceLanguage, targetLanguage, indexes, recognizers))
		{
		}

		public override bool HasPermission(string permission)
		{
			return true;
		}

		public override void RefreshStatusInfo()
		{
		}
	}
}
