using System;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	public class TranslationMemoryEntityQuery
	{
		public int Index
		{
			get;
			set;
		}

		public int Size
		{
			get;
			set;
		}

		public string[] Relationships
		{
			get;
			set;
		}

		public bool IncludeLanguageResourceData
		{
			get;
			set;
		}

		public bool IncludeScheduledOperations
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public string ResourceGroupPath
		{
			get;
			set;
		}

		public bool IncludeChildResourceGroups
		{
			get;
			set;
		}

		public string[] ContainerNames
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

		public Guid[] FieldTemplateIds
		{
			get;
			set;
		}

		public Guid[] LanguageResourceIds
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

		public string OwnerId
		{
			get;
			set;
		}

		public TranslationMemoryEntityQuery()
		{
			Relationships = new string[0];
			ContainerNames = new string[0];
			SourceLanguageCodes = new string[0];
			TargetLanguageCodes = new string[0];
			FieldTemplateIds = new Guid[0];
			LanguageResourceIds = new Guid[0];
			ResourceGroupPath = "/";
			OwnerId = null;
			IncludeChildResourceGroups = true;
		}
	}
}