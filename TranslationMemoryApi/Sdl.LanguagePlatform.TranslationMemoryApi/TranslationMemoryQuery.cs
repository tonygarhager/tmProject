using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Defines the query to be executed over the translation memories
	/// </summary>
	public class TranslationMemoryQuery
	{
		/// <summary>
		/// O based index of page results
		/// </summary>
		public int Index
		{
			get;
			set;
		}

		/// <summary>
		/// the max number of entities to be returned by the query
		/// </summary>
		public int Size
		{
			get;
			set;
		}

		/// <summary>
		/// The expected properties of the translation memory.
		/// </summary>
		public TranslationMemoryProperties Properties
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by text in name or description
		/// </summary>
		public string Text
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by the organization
		/// </summary>
		public string ResourceGroupPath
		{
			get;
			set;
		}

		/// <summary>
		/// Decide to include sub-organization when filtering by the organiation
		/// </summary>
		public bool IncludeChildResourceGroups
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by the containers
		/// </summary>
		public string[] ContainerNames
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
		public Guid[] FieldTemplateIds
		{
			get;
			set;
		}

		/// <summary>
		/// Filter by language template id's
		/// </summary>
		public Guid[] LanguageResourceIds
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

		/// <summary>
		/// Filter by the ownerId (this is used for GroupShare 2017 for compatibility, from 2017SR1, ResourceGroup is supposed to be used)
		/// </summary>
		public string OwnerId
		{
			get;
			set;
		}

		/// <summary>
		/// Default constructor for TM query
		/// </summary>
		public TranslationMemoryQuery()
		{
			Index = 0;
			Size = 20;
			ContainerNames = new string[0];
			SourceLanguageCodes = new string[0];
			TargetLanguageCodes = new string[0];
			FieldTemplateIds = new Guid[0];
			LanguageResourceIds = new Guid[0];
			ResourceGroupPath = "/";
			IncludeChildResourceGroups = true;
		}
	}
}
