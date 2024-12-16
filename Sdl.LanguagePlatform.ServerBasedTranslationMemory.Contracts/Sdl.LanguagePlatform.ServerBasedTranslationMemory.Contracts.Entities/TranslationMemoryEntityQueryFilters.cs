namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	public class TranslationMemoryEntityQueryFilters
	{
		public ContainerEntity[] Containers
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

		public FieldGroupTemplateEntity[] FieldTemplates
		{
			get;
			set;
		}

		public LanguageResourceTemplateEntity[] LanguageResourceTemplates
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
