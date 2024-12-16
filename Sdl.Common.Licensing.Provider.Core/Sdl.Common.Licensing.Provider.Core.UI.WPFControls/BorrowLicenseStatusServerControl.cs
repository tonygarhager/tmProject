using Sdl.Common.Licensing.Provider.Core.Exceptions;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	internal class BorrowLicenseStatusServerControl : LicensePageControlWPF, IUIControl, IComponentConnector
	{
		private const string ActionButtonSpace = "   ";

		private bool InRetryMode = true;

		internal Button _okCancelButton;

		internal Button _helpButton;

		internal Image _licensePictureBox;

		internal TextBlock _activeLicenseLabel;

		internal TextBlock _activeLicenseDescriptionLabel;

		internal TextBlock _licenseServerLabel;

		internal TextBlock _borrowDaysRemainingLabel;

		internal Button _actionButton;

		internal Image _actionImage;

		internal Label _actionTextBlock;

		private bool _contentLoaded;

		public BorrowLicenseStatusServerControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
		}

		public override void RefreshDisplay()
		{
			_actionButton.Visibility = Visibility.Collapsed;
			if (base.LicenseManager.ProductLicense == null)
			{
				_okCancelButton.Content = StringResources.StatusControl_Close;
				ShowRetrievingMessage();
				RetryToObtainLicense();
				return;
			}
			InRetryMode = false;
			_activeLicenseLabel.Visibility = Visibility.Visible;
			_activeLicenseDescriptionLabel.Text = base.LicenseManager.GetLicenseInfo();
			_licenseServerLabel.Text = string.Format(StringResources.BorrowLicenseStatusServerControl_BorrowedFromServer, base.LicenseManager.LicensingProvider.Configuration.Registry.LicenseServer);
			_actionImage.Source = BitmapToImageSource(ImageResources.server_from_client);
			_actionTextBlock.Content = "   " + StringResources.LicenseStatusServerControl_Return;
			_actionButton.Visibility = Visibility.Visible;
			_okCancelButton.Content = StringResources.StatusControl_Ok;
			if (base.LicenseManager.ProductLicense != null && base.LicenseManager.ProductLicense.Status != 0)
			{
				_licensePictureBox.Source = BitmapToImageSource(ImageResources.Shield_cross);
				_okCancelButton.Content = StringResources.StatusControl_Close;
			}
			else
			{
				_licensePictureBox.Source = BitmapToImageSource(ImageResources.Shield_tick);
				_okCancelButton.Content = StringResources.StatusControl_Ok;
			}
			UpdateExpiryInfo();
		}

		private void LicenseManager_GetLicenseCompleted(object sender, GetLicenseCompletedEventArgs e)
		{
			base.LicenseManager.GetLicenseCompleted -= LicenseManager_GetLicenseCompleted;
			_actionButton.IsEnabled = true;
			if (e.Error != null)
			{
				RetryMode(e.Error);
			}
			else
			{
				RefreshDisplay();
			}
		}

		private void RetryToObtainLicense()
		{
			_actionButton.IsEnabled = false;
			base.LicenseManager.GetLicenseCompleted += LicenseManager_GetLicenseCompleted;
			ShowRetrievingMessage();
			base.LicenseManager.GetLicenseAsync();
		}

		private void ShowRetrievingMessage()
		{
			_activeLicenseLabel.Visibility = Visibility.Collapsed;
			_activeLicenseDescriptionLabel.Text = StringResources.ServerStatusControl_RetrievingLicense;
			_actionImage.Source = BitmapToImageSource(ImageResources.server_into_32x32);
			_actionTextBlock.Content = "   " + StringResources.LicenseStatusServerControl_Retry;
		}

		private void _actionButton_Click(object s, EventArgs e)
		{
			if (!InRetryMode)
			{
				if (Helpers.ShowQuestion(StringResources.BorrowLicenseStatusServerControl_ConfirmReturnLicense, StringResources.BorrowLicenseStatusServerControl_ConfirmReturnLicense_Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					base.LicenseManagerView.IsProcessInProgress = true;
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.DoWork += delegate
					{
						ReturnBorrowedLicense();
					};
					backgroundWorker.RunWorkerCompleted += delegate
					{
						base.LicenseManagerView.IsProcessInProgress = false;
						base.LicenseManagerView.OnLicenseStatusChanging();
						base.LicenseManagerView.ShowStatusPage();
					};
					backgroundWorker.RunWorkerAsync();
				}
			}
			else
			{
				RetryToObtainLicense();
			}
		}

		private void ReturnBorrowedLicense()
		{
			try
			{
				base.LicenseManager.ProductLicense.CommuterCheckIn();
				SetUseCommuterLicenseConfig(useCommuterLicense: false);
				base.LicenseManager.ClearCachedLicense();
				base.LicenseManager.GetLicense();
			}
			catch (LicensingProviderException ex)
			{
				MessageLog.DefaultLog.Error("Error connecting to license server.", ex);
				Helpers.ShowError(ex.ErrorMessage);
				return;
			}
			catch (Exception ex2)
			{
				MessageLog.DefaultLog.Error("Error connecting to license server.", ex2);
				Helpers.ShowException(ex2);
				return;
			}
			Helpers.ShowInformation(StringResources.DesktopLicensing_ReturningBorrowedLicense_Successful, StringResources.DesktopLicensing_ReturningBorrowedLicense_MessageBox_Title);
		}

		private void SetUseCommuterLicenseConfig(bool useCommuterLicense)
		{
			ILicenseRegistryAccess registry = base.LicenseManager.LicensingProvider.Configuration.Registry;
			registry.UseBorrowedLicense = useCommuterLicense;
		}

		private void _okCancelButton_Click(object sender, EventArgs e)
		{
			base.LicenseManagerView.Close();
		}

		private void _helpButton_Click(object sender, EventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.BorrowLicenseStatusScreen);
		}

		internal void RetryMode(Exception exception)
		{
			_activeLicenseDescriptionLabel.Text = "";
			_actionButton.Visibility = Visibility.Visible;
			_actionImage.Source = BitmapToImageSource(ImageResources.server_into_32x32);
			_actionTextBlock.Content = "   " + StringResources.LicenseStatusServerControl_Retry;
			_okCancelButton.Content = StringResources.StatusControl_Close;
			_licensePictureBox.Source = BitmapToImageSource(ImageResources.server_error_48x48);
		}

		private void UpdateExpiryInfo()
		{
			int daysRemainingOnLicense = Helpers.GetDaysRemainingOnLicense(base.CurrentLicense);
			if (daysRemainingOnLicense <= 0 || base.CurrentLicense.Status == LicenseStatus.ExportExpired || base.CurrentLicense.Status != 0)
			{
				_borrowDaysRemainingLabel.Text = string.Format(StringResources.LicenseStatusServerControl_BorrowedLicenseHasExpired);
				_borrowDaysRemainingLabel.Foreground = Brushes.Red;
			}
			else
			{
				_borrowDaysRemainingLabel.Text = string.Format(StringResources.LicenseStatusServerControl_DaysRemaining, daysRemainingOnLicense);
				_borrowDaysRemainingLabel.Foreground = Brushes.Green;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/borrowlicensestatusservercontrol.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				_okCancelButton = (Button)target;
				_okCancelButton.Click += _okCancelButton_Click;
				break;
			case 2:
				_helpButton = (Button)target;
				_helpButton.Click += _helpButton_Click;
				break;
			case 3:
				_licensePictureBox = (Image)target;
				break;
			case 4:
				_activeLicenseLabel = (TextBlock)target;
				break;
			case 5:
				_activeLicenseDescriptionLabel = (TextBlock)target;
				break;
			case 6:
				_licenseServerLabel = (TextBlock)target;
				break;
			case 7:
				_borrowDaysRemainingLabel = (TextBlock)target;
				break;
			case 8:
				_actionButton = (Button)target;
				_actionButton.Click += _actionButton_Click;
				break;
			case 9:
				_actionImage = (Image)target;
				break;
			case 10:
				_actionTextBlock = (Label)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
