using Sdl.Core.Api.DataAccess;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "TranslationSequenceItem")]
	[Entity(Schema = "etm", Table = "TranslationSequenceItem", PrimaryKey = "TranslationSequenceItemId")]
	public class TranslationSequenceItemEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private Guid? _uniqueId;

		private string _uri;

		private string _state;

		private int? _penalty;

		private bool? _performUpdate;

		private bool? _performNormalSearch;

		private bool? _performConcordanceSearch;

		private bool? _enabled;

		private int? _sequenceIndex;

		private EntityReference<TranslationSequenceEntity> _translationSequence = new EntityReference<TranslationSequenceEntity>();

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

		[EntityColumn]
		[DataMember]
		public string Uri
		{
			get
			{
				return _uri;
			}
			set
			{
				if (!(_uri == value))
				{
					_uri = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string State
		{
			get
			{
				return _state;
			}
			set
			{
				if (!(_state == value))
				{
					_state = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public int? Penalty
		{
			get
			{
				return _penalty;
			}
			set
			{
				if (_penalty != value)
				{
					_penalty = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public bool? PerformUpdate
		{
			get
			{
				return _performUpdate;
			}
			set
			{
				if (_performUpdate != value)
				{
					_performUpdate = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public bool? PerformNormalSearch
		{
			get
			{
				return _performNormalSearch;
			}
			set
			{
				if (_performNormalSearch != value)
				{
					_performNormalSearch = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public bool? PerformConcordanceSearch
		{
			get
			{
				return _performConcordanceSearch;
			}
			set
			{
				if (_performConcordanceSearch != value)
				{
					_performConcordanceSearch = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public bool? Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public int? SequenceIndex
		{
			get
			{
				return _sequenceIndex;
			}
			set
			{
				if (_sequenceIndex != value)
				{
					_sequenceIndex = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "TranslationSequenceId")]
		public EntityReference<TranslationSequenceEntity> TranslationSequence
		{
			get
			{
				return _translationSequence;
			}
			set
			{
				_translationSequence = value;
			}
		}
	}
}
