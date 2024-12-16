namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationMemoryQueryFilters
	{
		public TranslationMemoryContainer[] Containers
		{
			get;
			set;
		}

		public string[] SourceLanguageCodes
		{
			get;
			set;
		}

		public string[] TargetLanguageCodes
		{
			get;
			set;
		}

		public ServerBasedFieldsTemplate[] FieldTemplates
		{
			get;
			set;
		}

		public ServerBasedLanguageResourcesTemplate[] LanguageResourceTemplates
		{
			get;
			set;
		}

		public bool IsProject
		{
			get;
			set;
		}

		public bool IsMain
		{
			get;
			set;
		}
	}
}
