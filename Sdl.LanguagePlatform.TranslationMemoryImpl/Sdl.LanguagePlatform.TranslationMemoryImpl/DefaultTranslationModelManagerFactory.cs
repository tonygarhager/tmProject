using Sdl.Core.FineGrainedAlignment;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class DefaultTranslationModelManagerFactory
	{
		internal static ITranslationModelManager GetTranslationModelManager(ITranslationModelStorage modelStorage)
		{
			return new TranslationModelManager(modelStorage);
		}
	}
}
