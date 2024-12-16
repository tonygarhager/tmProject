using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Licensing
{
	[DataContract(Name = "LicensedFeatureInfo", Namespace = "http://sdl.com/enterprise/platform/2011", IsReference = true)]
	public class LicensedFeatureInfo : LicensedFeature
	{
		[DataMember]
		public int AllocatedCount
		{
			get;
			set;
		}

		public LicensedFeatureInfo()
		{
		}

		public LicensedFeatureInfo(string featureName)
			: base(featureName)
		{
		}

		public LicensedFeatureInfo(string featureName, int maxCount, int allocatedCount, bool isCal = false)
			: base(featureName, maxCount, isCal)
		{
			AllocatedCount = allocatedCount;
		}
	}
}
