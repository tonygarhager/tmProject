namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Implements the available translation memory query filters
	/// </summary>
	public class TranslationMemoryQueryFilters
	{
		/// <summary>
		/// Filter by the containers
		/// </summary>
		public TranslationMemoryContainer[] Containers
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by the source languages
		/// </summary>
		public string[] SourceLanguageCodes
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by the source languages
		/// </summary>
		public string[] TargetLanguageCodes
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by template id's
		/// </summary>
		public ServerBasedFieldsTemplate[] FieldTemplates
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by language template id's
		/// </summary>
		public ServerBasedLanguageResourcesTemplate[] LanguageResourceTemplates
		{
			get;
			set;
		}

		/// <summary>
		/// Filter the project translation memories
		/// </summary>
		public bool IsProject
		{
			get;
			set;
		}

		/// <summary>
		/// Filter the main translation memories
		/// </summary>
		public bool IsMain
		{
			get;
			set;
		}
	}
}
