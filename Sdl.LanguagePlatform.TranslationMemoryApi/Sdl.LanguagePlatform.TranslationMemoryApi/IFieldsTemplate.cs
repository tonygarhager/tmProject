namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface IFieldsTemplate
	{
		string Description
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		FieldDefinitionCollection FieldDefinitions
		{
			get;
		}
	}
}
