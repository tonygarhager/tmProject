using Sdl.Core.Api.DataAccess;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "ImportExportBase")]
	public abstract class ImportExportEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private EntityReference<LanguageDirectionEntity> _LanguageDirection = new EntityReference<LanguageDirectionEntity>();

		[IgnoreEntityMember]
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return _extensionDataObject;
			}
			set
			{
				_extensionDataObject = value;
			}
		}

		[DataMember]
		public Guid? UniqueId
		{
			get;
			set;
		}

		[DataMember]
		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "LanguageDirectionId")]
		public EntityReference<LanguageDirectionEntity> LanguageDirection
		{
			get
			{
				return _LanguageDirection;
			}
			set
			{
				_LanguageDirection = value;
			}
		}

		[DataMember]
		[EntityColumn(ColumnName = "StatsUpdated")]
		public DateTime? LastUpdated
		{
			get;
			set;
		}

		[DataMember]
		public int? ChunkSize
		{
			get;
			set;
		}

		[DataMember]
		public bool? ContinueOnError
		{
			get;
			set;
		}

		[DataMember]
		public bool? ExitedOnError
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn(ColumnName = "WorkItemRefrenceUniqueId")]
		public Guid? WorkItemId
		{
			get;
			set;
		}

		[DataMember]
		[IgnoreEntityMember]
		public ScheduledOperationEntity ScheduledOperation
		{
			get;
			set;
		}
	}
}
