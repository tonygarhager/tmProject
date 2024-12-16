using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IOutputTranslationMemoryCollection : ICollection<IOutputTranslationMemory>, IEnumerable<IOutputTranslationMemory>, IEnumerable
	{
		IOutputTranslationMemory this[int index]
		{
			get;
		}

		IOutputTranslationMemory Add();

		IOutputTranslationMemory Add(IInputTranslationMemory inputTranslationMemory, bool autoPopulateOutputTranslationMemorySetup);

		IOutputTranslationMemory Add(IEnumerable<IInputTranslationMemory> inputTranslationMemory, bool autoPopulateOutputTranslationMemorySetup);
	}
}
