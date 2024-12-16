namespace Sdl.LanguagePlatform.TranslationMemory
{
	public interface ITypedKeyValueContainer
	{
		FieldValue GetValue(string fieldName);

		FieldValue GetValue(string fieldName, FieldValueType t);

		FieldValueType GetType(string fieldName);
	}
}
