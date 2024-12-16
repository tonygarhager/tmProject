namespace Sdl.Common.Licensing.Provider.Core
{
	public interface INetworkLicenseFeature : ILicenseFeature
	{
		void CheckOut();

		void CheckIn();

		void CommuterCheckOut(int duration);

		void CommuterCheckIn();

		bool IsLoggedIn();
	}
}
