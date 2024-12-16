using System.Drawing;
using System.Windows.Input;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class LicensingFormConfiguration
	{
		public LicenseInfoProviderDelegate LicenseInfoProvider
		{
			get;
			set;
		}

		public IsLicenseValidForMachineDelegate IsLicenseValidForMachine
		{
			get;
			set;
		}

		public IsProductAllowedDelegate IsProductAllowed
		{
			get;
			set;
		}

		public string StatusPageTitle
		{
			get;
			set;
		}

		public string StatusPageSubtitle
		{
			get;
			set;
		}

		public Icon Icon
		{
			get;
			set;
		}

		public Image TitleBarImage
		{
			get;
			set;
		}

		public string PurchaseLinkUrl
		{
			get;
			set;
		}

		public string MyAccountLinkUrl
		{
			get;
			set;
		}

		public string ResetActivationLinkUrl
		{
			get;
			set;
		} = "http://feedback.sdl.com/go?iv=2gsl95ewsc1jq";


		public ILicensingHelpProvider LicensingHelpProvider
		{
			get;
			set;
		}

		public ICommand CloseCommand
		{
			get;
			set;
		}
	}
}
