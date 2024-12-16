using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public interface ITranslationUnitFilteringStrategy
	{
		List<TranslationUnit> GetTusFiltered(PersistentObjectToken translationMemoryId, RegularIterator iter, FieldDefinitions fieldDefinitions, bool includeContextContent, TextContextMatchType textContextMatchType, CultureInfo sourceCulture, CultureInfo targetCulture, bool usesIdContextMatch);

		List<PersistentObjectToken> DeleteTusFiltered(PersistentObjectToken tmId, RegularIterator iter);
	}
}
