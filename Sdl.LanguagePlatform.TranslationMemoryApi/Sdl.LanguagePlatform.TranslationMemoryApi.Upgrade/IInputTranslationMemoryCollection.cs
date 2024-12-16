using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IInputTranslationMemoryCollection : ICollection<IInputTranslationMemory>, IEnumerable<IInputTranslationMemory>, IEnumerable
	{
		IInputTranslationMemory this[int index]
		{
			get;
		}

		IInputTranslationMemory Add(ILegacyTranslationMemory legacyTm);
	}
}
