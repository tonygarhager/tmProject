using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Licensing
{
	[DataContract(Name = "LicenseInfo", Namespace = "http://sdl.com/enterprise/platform/2011", IsReference = true)]
	public class LicenseInfo
	{
		[DataMember]
		public LicensedFeatureInfo[] LicensedFeatures
		{
			get;
			set;
		}

		[DataMember]
		public bool IsLicensed
		{
			get;
			set;
		}

		[DataMember]
		public bool IsTimeLimited
		{
			get;
			set;
		}

		[DataMember]
		public bool IsTrial
		{
			get;
			set;
		}

		[DataMember]
		public int DaysRemaining
		{
			get;
			set;
		}
	}
}
