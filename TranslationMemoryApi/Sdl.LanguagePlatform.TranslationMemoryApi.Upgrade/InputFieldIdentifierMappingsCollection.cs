using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class InputFieldIdentifierMappingsCollection : ItemCollection<IFieldIdentifierMappings>, IFieldIdentifierMappingsCollection, ICollection<IFieldIdentifierMappings>, IEnumerable<IFieldIdentifierMappings>, IEnumerable
	{
		public IDictionary<FieldIdentifier, FieldIdentifier> GetFieldIdentifierMappings(IInputTranslationMemory translationMemory)
		{
			return GetInputFieldIdentifierMappings(translationMemory)?.FieldIdentifierMappings;
		}

		public IFieldIdentifierMappings GetInputFieldIdentifierMappings(IInputTranslationMemory translationMemory)
		{
			using (IEnumerator<IFieldIdentifierMappings> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IFieldIdentifierMappings current = enumerator.Current;
					if (current.TranslationMemory == translationMemory)
					{
						return current;
					}
				}
			}
			return null;
		}
	}
}
