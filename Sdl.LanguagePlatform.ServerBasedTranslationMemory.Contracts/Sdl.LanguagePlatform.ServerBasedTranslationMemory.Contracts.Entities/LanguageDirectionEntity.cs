using Sdl.Core.Api.DataAccess;
using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "LanguageDirection")]
	[Entity(Schema = "etm", Table = "LanguageDirection", PrimaryKey = "LanguageDirectionId")]
	public class LanguageDirectionEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject extensionDataObject;

		private Guid? _uniqueId;

		private int? _physicalTmId;

		private string _sourceCultureName;

		private string _targetCultureName;

		private DateTime? _lastRecomputeDate;

		private int? _lastRecomputeSize;

		private int? _tuCount;

		private EntityReference<TranslationMemoryEntity> translationMemory = new EntityReference<TranslationMemoryEntity>();

		private EntityCollection<ImportEntity> imports = new EntityCollection<ImportEntity>();

		private EntityCollection<ExportEntity> exports = new EntityCollection<ExportEntity>();

		[IgnoreEntityMember]
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return extensionDataObject;
			}
			set
			{
				extensionDataObject = value;
			}
		}

		[EntityColumn]
		[DataMember]
		public Guid? UniqueId
		{
			get
			{
				return _uniqueId;
			}
			set
			{
				if (!(_uniqueId == value))
				{
					_uniqueId = value;
					MarkAsDirty();
				}
			}
		}

		[IgnoreEntityMember]
		[DataMember]
		public string ParentResourceGroupName
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public string ParentResourceGroupPath
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public string ParentResourceGroupDescription
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public string[] LinkedResourceGroupPaths
		{
			get;
			set;
		}

		[IgnoreEntityMember]
		[DataMember]
		public PermissionCollection Permissions
		{
			get;
			set;
		}

		[EntityColumn]
		[DataMember]
		public int? PhysicalTmId
		{
			get
			{
				return _physicalTmId;
			}
			set
			{
				if (_physicalTmId != value)
				{
					_physicalTmId = value;
					MarkAsDirty();
				}
			}
		}

		[IgnoreDataMember]
		[IgnoreEntityMember]
		public PersistentObjectToken PhysicalTmToken
		{
			get
			{
				if (UniqueId.HasValue && PhysicalTmId.HasValue)
				{
					return new PersistentObjectToken(PhysicalTmId.Value, UniqueId.Value);
				}
				return null;
			}
		}

		[EntityColumn]
		[DataMember]
		public string SourceCultureName
		{
			get
			{
				return _sourceCultureName;
			}
			set
			{
				if (!(_sourceCultureName == value))
				{
					_sourceCultureName = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string TargetCultureName
		{
			get
			{
				return _targetCultureName;
			}
			set
			{
				if (!(_targetCultureName == value))
				{
					_targetCultureName = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public DateTime? LastRecomputeDate
		{
			get
			{
				return _lastRecomputeDate;
			}
			set
			{
				if (!(_lastRecomputeDate == value))
				{
					_lastRecomputeDate = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public int? LastRecomputeSize
		{
			get
			{
				return _lastRecomputeSize;
			}
			set
			{
				if (_lastRecomputeSize != value)
				{
					_lastRecomputeSize = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public int? TuCount
		{
			get
			{
				return _tuCount;
			}
			set
			{
				if (_tuCount != value)
				{
					_tuCount = value;
					MarkAsDirty();
				}
			}
		}

		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "TranslationMemoryId")]
		[DataMember]
		public EntityReference<TranslationMemoryEntity> TranslationMemory
		{
			get
			{
				return translationMemory;
			}
			set
			{
				translationMemory = value;
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "LanguageDirectionId")]
		public EntityCollection<ImportEntity> Imports
		{
			get
			{
				return imports;
			}
			set
			{
				imports = value;
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "LanguageDirectionId")]
		public EntityCollection<ExportEntity> Exports
		{
			get
			{
				return exports;
			}
			set
			{
				exports = value;
			}
		}

		[IgnoreDataMember]
		public LanguageDirectionIdentity LanguageDirectionIdentity
		{
			get
			{
				LanguageDirectionIdentity result = default(LanguageDirectionIdentity);
				result.Identity = base.Id;
				result.TranslationMemoryId = ((TranslationMemory.ForeignKey != null) ? ((Guid)TranslationMemory.ForeignKey.Value) : Guid.Empty);
				result.Source = SourceCultureName;
				result.Target = TargetCultureName;
				return result;
			}
		}

		public LanguageDirectionEntity Clone(TranslationMemoryEntity newParent)
		{
			LanguageDirectionEntity languageDirectionEntity = MemberwiseClone() as LanguageDirectionEntity;
			if (languageDirectionEntity == null)
			{
				return null;
			}
			languageDirectionEntity.TranslationMemory = new EntityReference<TranslationMemoryEntity>(newParent);
			languageDirectionEntity.Imports = new EntityCollection<ImportEntity>();
			foreach (ImportEntity import in Imports)
			{
				languageDirectionEntity.Imports.Add(import.ShallowCopy(languageDirectionEntity));
			}
			return languageDirectionEntity;
		}
	}
}
