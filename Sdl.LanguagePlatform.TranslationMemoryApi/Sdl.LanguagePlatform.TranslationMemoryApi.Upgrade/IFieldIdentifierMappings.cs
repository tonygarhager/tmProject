using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IFieldIdentifierMappings
	{
		IInputTranslationMemory TranslationMemory
		{
			get;
		}

		IDictionary<FieldIdentifier, FieldIdentifier> FieldIdentifierMappings
		{
			get;
		}
	}
}
