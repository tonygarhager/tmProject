namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	public interface IPermissionCheck
	{
		bool HasPermission(string permission);
	}
}
