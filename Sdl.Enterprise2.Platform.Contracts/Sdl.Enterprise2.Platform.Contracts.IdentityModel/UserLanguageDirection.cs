using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "UserLanguageDirection", Namespace = "http://sdl.com/identity/2012", IsReference = true)]
	public class UserLanguageDirection : IExtensibleDataObject
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

		[DataMember(Name = "Id")]
		public int Id
		{
			get;
			set;
		}

		[DataMember(Name = "SourceLanguageCode")]
		public string SourceLanguageCode
		{
			get;
			set;
		}

		[DataMember(Name = "TargetLanguageCode")]
		public string TargetLanguageCode
		{
			get;
			set;
		}
	}
}
