using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.Licensing
{
	[DataContract(Name = "LicensedFeature", Namespace = "http://sdl.com/enterprise/platform/2011", IsReference = true)]
	public class LicensedFeature
	{
		[DataMember]
		public string FeatureName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCAL
		{
			get;
			set;
		}

		[DataMember]
		public int? MaxCount
		{
			get;
			set;
		}

		public bool IsCounted => MaxCount.HasValue;

		public LicensedFeatureType FeatureType
		{
			get
			{
				if (IsCounted)
				{
					if (!IsCAL)
					{
						return LicensedFeatureType.LimitCount;
					}
					return LicensedFeatureType.CALCount;
				}
				return LicensedFeatureType.OnOff;
			}
		}

		public LicensedFeature()
		{
		}

		public LicensedFeature(string featureName, int? maxCount = null, bool cal = false)
		{
			FeatureName = featureName;
			MaxCount = maxCount;
			IsCAL = cal;
		}
	}
}
