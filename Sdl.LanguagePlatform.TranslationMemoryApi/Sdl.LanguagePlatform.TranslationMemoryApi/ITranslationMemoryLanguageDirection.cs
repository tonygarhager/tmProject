using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemory.EditScripts;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationMemoryLanguageDirection : ITranslationProviderLanguageDirection
	{
		new ITranslationMemory TranslationProvider
		{
			get;
		}

		int GetTranslationUnitCount();

		bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId);

		int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds);

		bool DeleteTranslationUnit(PersistentObjectToken translationUnitId);

		int DeleteAllTranslationUnits();

		int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds);

		int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator);

		int EditTranslationUnits(EditScript editScript, EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds);

		int EditTranslationUnitsWithIterator(EditScript editScript, EditUpdateMode updateMode, ref RegularIterator iterator);

		TranslationUnit[] PreviewEditTranslationUnitsWithIterator(EditScript editScript, ref RegularIterator iterator);

		TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator);

		TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId);

		TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator);

		ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask);

		bool ReindexTranslationUnits(ref RegularIterator iterator);
	}
}
