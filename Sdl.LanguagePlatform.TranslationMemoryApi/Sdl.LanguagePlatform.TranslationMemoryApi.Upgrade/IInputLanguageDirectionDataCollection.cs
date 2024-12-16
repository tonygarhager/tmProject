using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IInputLanguageDirectionDataCollection : ICollection<IInputLanguageDirectionData>, IEnumerable<IInputLanguageDirectionData>, IEnumerable
	{
		IInputLanguageDirectionData this[int index]
		{
			get;
		}

		IInputLanguageDirectionData Add(IInputTranslationMemory translationMemory, ILegacyLanguageDirectionData languageDirectionData);

		void Move(int fromIndex, int toIndex);
	}
}
