using Sdl.Core.Api.DataAccess;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "LicensingRestrictions")]
	[Entity(Schema = "etm", Table = "LicensingRestrictionsCurrentState", PrimaryKey = "LicensingRestrictionsCurrentStateId")]
	public class LicensingRestrictionsEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private long? _unitCount;

		private string _stateLabel;

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
		[EntityColumn(ColumnName = "UnitCount")]
		public long? UnitCount
		{
			get
			{
				return _unitCount;
			}
			set
			{
				if (_unitCount != value)
				{
					_unitCount = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn(ColumnName = "StateLabel")]
		public string StateLabel
		{
			get
			{
				return _stateLabel;
			}
			set
			{
				if (!(_stateLabel == value))
				{
					_stateLabel = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[IgnoreEntityMember]
		public long MaxUnitCount
		{
			get;
			set;
		}

		[DataMember]
		[IgnoreEntityMember]
		public int MaxConcurrentUsers
		{
			get;
			set;
		}

		[DataMember]
		[IgnoreEntityMember]
		public int ConcurrentUsers
		{
			get;
			set;
		}

		[DataMember]
		[IgnoreEntityMember]
		public string FeatureName
		{
			get;
			set;
		}
	}
}
