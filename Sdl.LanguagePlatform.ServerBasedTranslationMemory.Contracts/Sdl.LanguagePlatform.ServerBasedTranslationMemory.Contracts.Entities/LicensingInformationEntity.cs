using Sdl.Core.Api.DataAccess;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "LicensingInformation")]
	public class LicensingInformationEntity : IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

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
		public long CurrentUnitCount
		{
			get;
			set;
		}

		[DataMember]
		[IgnoreEntityMember]
		public int CurrentConcurrentUsers
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
