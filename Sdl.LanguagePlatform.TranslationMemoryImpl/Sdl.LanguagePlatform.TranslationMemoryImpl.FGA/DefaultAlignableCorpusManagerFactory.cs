using Sdl.Core.FineGrainedAlignment;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	internal class DefaultAlignableCorpusManagerFactory
	{
		internal static IAlignableCorpusManager GetAlignableCorpusManager(CallContext context)
		{
			return new StorageBasedAlignableCorpusManager(context);
		}
	}
}
