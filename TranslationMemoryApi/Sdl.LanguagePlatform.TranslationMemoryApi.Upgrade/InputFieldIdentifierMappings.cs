using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class InputFieldIdentifierMappings : IFieldIdentifierMappings
	{
		private readonly IInputTranslationMemory _translationMemory;

		private readonly IDictionary<FieldIdentifier, FieldIdentifier> _fieldIdentifierMappings;

		public IInputTranslationMemory TranslationMemory => _translationMemory;

		public IDictionary<FieldIdentifier, FieldIdentifier> FieldIdentifierMappings => _fieldIdentifierMappings;

		public InputFieldIdentifierMappings(IInputTranslationMemory translationMemory, IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings)
		{
			_translationMemory = translationMemory;
			_fieldIdentifierMappings = fieldIdentifierMappings;
		}
	}
}
