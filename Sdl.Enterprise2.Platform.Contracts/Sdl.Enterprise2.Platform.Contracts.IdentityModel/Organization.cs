using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "Organization", Namespace = "http://sdl.com/identity/2010")]
	public class Organization : ResourceGroup2011
	{
		public Organization()
		{
			base.ResourceGroupType = "ORG";
		}
	}
}
