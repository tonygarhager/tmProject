using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class InputLanguagePairCollection : ItemCollection<IInputLanguageDirectionData>, IInputLanguageDirectionDataCollection, ICollection<IInputLanguageDirectionData>, IEnumerable<IInputLanguageDirectionData>, IEnumerable
	{
		public IInputLanguageDirectionData Add(IInputTranslationMemory translationMemory, ILegacyLanguageDirectionData languagePair)
		{
			InputLanguagePair inputLanguagePair = new InputLanguagePair(translationMemory, languagePair);
			Add(inputLanguagePair);
			return inputLanguagePair;
		}

		public void Move(int fromIndex, int toIndex)
		{
			IInputLanguageDirectionData item = base[fromIndex];
			RemoveAt(fromIndex);
			Insert(toIndex, item);
		}
	}
}
