using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class InputTranslationMemoryCollection : ItemCollection<IInputTranslationMemory>, IInputTranslationMemoryCollection, ICollection<IInputTranslationMemory>, IEnumerable<IInputTranslationMemory>, IEnumerable
	{
		public IInputTranslationMemory Add(ILegacyTranslationMemory legacyTm)
		{
			if (legacyTm == null)
			{
				throw new ArgumentNullException("legacyTm");
			}
			InputTranslationMemory inputTranslationMemory = new InputTranslationMemory(legacyTm);
			Add(inputTranslationMemory);
			return inputTranslationMemory;
		}
	}
}
