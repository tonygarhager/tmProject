using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IFieldIdentifierMappingsCollection : ICollection<IFieldIdentifierMappings>, IEnumerable<IFieldIdentifierMappings>, IEnumerable
	{
		IDictionary<FieldIdentifier, FieldIdentifier> GetFieldIdentifierMappings(IInputTranslationMemory translationMemory);

		IFieldIdentifierMappings GetInputFieldIdentifierMappings(IInputTranslationMemory translationMemory);
	}
}
