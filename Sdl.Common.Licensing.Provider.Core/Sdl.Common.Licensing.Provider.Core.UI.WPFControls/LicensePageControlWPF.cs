using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	public class LicensePageControlWPF : UserControl, IUIControl
	{
		public LicenseManagerView LicenseManagerView
		{
			get;
			private set;
		}

		public ICommand CloseCommand
		{
			get;
			set;
		}

		protected IProductLicense CurrentLicense => LicenseManagerView.LicenseManager.ProductLicense;

		public LicenseManager LicenseManager => LicenseManagerView.LicenseManager;

		public LicenseModeUI StatusPageMode
		{
			get
			{
				if (CurrentLicense == null)
				{
					return LicenseModeUI.NoLicense;
				}
				switch (CurrentLicense.Status)
				{
				case LicenseStatus.Authorized:
					if (!CurrentLicense.IsLocal && !CurrentLicense.IsLoggedIn)
					{
						return LicenseModeUI.NoLicense;
					}
					if (LicenseForm.LicensingFormConfiguration == null || LicenseForm.LicensingFormConfiguration.IsProductAllowed == null || LicenseForm.LicensingFormConfiguration.IsProductAllowed(CurrentLicense, null))
					{
						if (CurrentLicense.Mode == LicenseMode.Permanent)
						{
							return LicenseModeUI.FullLicense;
						}
						if (CurrentLicense.Mode == LicenseMode.Demo)
						{
							return LicenseModeUI.Trial;
						}
						if (CurrentLicense.Mode == LicenseMode.Lease)
						{
							return LicenseModeUI.Lease;
						}
					}
					return LicenseModeUI.NoLicense;
				case LicenseStatus.DemoExpired:
					return LicenseModeUI.TrialExpired;
				case LicenseStatus.LeaseExpired:
					return LicenseModeUI.LeaseExpired;
				case LicenseStatus.ExportExpired:
					return LicenseModeUI.NoLicense;
				case LicenseStatus.NotAuthorized:
					return LicenseModeUI.NoLicense;
				case LicenseStatus.UnknownError:
					return LicenseModeUI.NoLicense;
				default:
					return LicenseModeUI.NoLicense;
				}
			}
		}

		public ILicensingForm LicenseForm
		{
			get;
			protected set;
		}

		public string MainTitle
		{
			get;
			set;
		}

		public string SubTitle
		{
			get;
			set;
		}

		public LicensePageControlWPF()
		{
		}

		public LicensePageControlWPF(LicenseManagerView licenseManagerView)
			: this()
		{
			LicenseManagerView = licenseManagerView;
			CloseCommand = licenseManagerView.CloseCommand;
		}

		public ImageSource BitmapToImageSource(Bitmap bitmapImage)
		{
			Icon value = ImageUtils.BitmapToIcon(bitmapImage);
			IconToImageSourceConverter iconToImageSourceConverter = new IconToImageSourceConverter();
			return iconToImageSourceConverter.Convert(value, null, null, null) as ImageSource;
		}

		public virtual void RefreshDisplay()
		{
		}

		public virtual void Initialize(ILicensingForm licenseForm)
		{
			LicenseForm = licenseForm;
			if (licenseForm.LicensingFormConfiguration != null && string.IsNullOrEmpty(MainTitle))
			{
				MainTitle = StringResources.ProductStatusActivation;
			}
			if (licenseForm.LicensingFormConfiguration != null && string.IsNullOrEmpty(SubTitle))
			{
				SubTitle = licenseForm.LicensingFormConfiguration.StatusPageSubtitle;
			}
		}

		public void Close()
		{
			LicenseManagerView.Close();
		}

		public virtual void ExecuteEscapeCommand()
		{
		}

		public virtual void ExecuteEnterCommand()
		{
		}
	}
}
