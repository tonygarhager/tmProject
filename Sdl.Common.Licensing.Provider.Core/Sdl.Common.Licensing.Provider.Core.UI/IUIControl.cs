namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public interface IUIControl
	{
		ILicensingForm LicenseForm
		{
			get;
		}

		string MainTitle
		{
			get;
			set;
		}

		string SubTitle
		{
			get;
			set;
		}

		void RefreshDisplay();

		void Initialize(ILicensingForm licenseForm);
	}
}
