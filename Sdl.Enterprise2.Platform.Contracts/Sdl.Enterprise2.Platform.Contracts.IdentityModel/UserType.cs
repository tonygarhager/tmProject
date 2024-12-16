using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[DataContract(Name = "UserType", Namespace = "http://sdl.com/identity/2012")]
	public enum UserType
	{
		[EnumMember]
		SDLUser,
		[EnumMember]
		WindowsUser,
		[EnumMember]
		CustomUser,
		[EnumMember]
		IdpUser
	}
}
