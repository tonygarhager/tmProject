using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "Role", Namespace = "http://sdl.com/identity/2011", IsReference = true)]
	public class Role2011 : Role2009
	{
		[DataMember(Order = 3)]
		public bool Protected
		{
			get;
			set;
		}
	}
}
