using System;
using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface IProductLicense : IDisposable
	{
		string Id
		{
			get;
		}

		int LicenseType
		{
			get;
		}

		ILicensingProvider Provider
		{
			get;
		}

		LicenseMode Mode
		{
			get;
		}

		LicenseModeDetails ModeDetail
		{
			get;
		}

		LicenseStatus Status
		{
			get;
		}

		DateTime? ExpirationDate
		{
			get;
		}

		List<string> FeaturesToCheckout
		{
			get;
		}

		string CurrentEditionToCheckout
		{
			get;
			set;
		}

		bool IsLoggedIn
		{
			get;
		}

		bool IsLocal
		{
			get;
		}

		bool IsBorrowed
		{
			get;
		}

		bool IsTerminalService
		{
			get;
		}

		bool HasFeature(string id);

		bool IsFeatureCheckedOut(string id);

		ILicenseFeature GetFeature(string id);

		IList<ILicenseFeature> GetFeatures();

		void GetFeaturesToCheckout(string currentEdition = null);

		void CheckOut();

		void CheckIn();

		void CommuterCheckOut(int duration);

		void CommuterCheckIn();

		void BorrowLicenseSeat(string approvalCode);

		dynamic GetProperty(string name);

		bool IsServerRunning();

		void Authorize();

		void NotAuthorized();

		new int GetHashCode();
	}
}
