using System;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicensingDialog
	{
		bool CanActivateAPerpetual
		{
			get;
			set;
		}

		event EventHandler CloseUI;

		event EventHandler LicenseStatusChanging;

		event EventHandler LicenseStatusChanged;
	}
}
