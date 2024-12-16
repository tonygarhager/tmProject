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
	internal class LicenseStatusControl : LicensePageControlWPF, IComponentConnector
	{
		private ImageSource _imageLicenced;

		private ImageSource _imageLicensedWarning;

		private ImageSource _imageTimerWarning;

		private ImageSource _imageUnlicenced;

		private ImageSource _imageTrial;

		private ImageSource _activateKeys;

		private ImageSource _deactivateKeys;

		private const int TrialProgressBarMaxDays = 30;

		private bool _isActivateButton;

		internal Button SdlAccountButton;

		internal Button CancelButton;

		internal Button HelpButton;

		internal Image LicenseImage;

		internal TextBlock ActiveLicenseDescriptionTextBlock;

		internal ProgressBar TrialRemainingBar;

		internal TextBlock TrialRemainingTextBlock;

		internal Button ShowActivationCodeButton;

		internal Button DeactivateOfflineButton;

		internal Button ViewDeactivationCertificateButton;

		internal TextBlock FirstWarningTextBlock;

		internal TextBlock SecondWarningTextBlock;

		internal StackPanel MainButtonsStackPanel;

		internal Button ActivateDeactivateButton;

		internal Image ActivateDeactivateButtonImage;

		internal Label ActivateDeactivateButtonText;

		internal Button PurchaseLicenseButton;

		internal Image PurchaseLicenseButtonImage;

		private bool _contentLoaded;

		public LicenseStatusControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			_imageLicenced = BitmapToImageSource(ImageResources.Shield_tick);
			_imageLicensedWarning = BitmapToImageSource(ImageResources.Shield_warn);
			_imageTimerWarning = BitmapToImageSource(ImageResources.timer_warn);
			_imageUnlicenced = BitmapToImageSource(ImageResources.Shield_cross);
			_imageTrial = BitmapToImageSource(ImageResources.timer);
			_activateKeys = BitmapToImageSource(ImageResources.key_activate_24x24);
			_deactivateKeys = BitmapToImageSource(ImageResources.key_deactivate_24x24);
			PurchaseLicenseButtonImage.Source = BitmapToImageSource(ImageResources.Purchase_license);
		}

		private void DisplayErrorStatus(Exception exception)
		{
			ViewDeactivationCertificateButton.Visibility = Visibility.Collapsed;
			PurchaseLicenseButton.Visibility = Visibility.Collapsed;
			LicenseImage.Source = BitmapToImageSource(ImageResources.paste);
			if (exception is LicensingProviderException)
			{
				LicensingProviderException ex = exception as LicensingProviderException;
			}
			else
			{
				SetActivateButton(isVisible: false);
				DeactivateOfflineButton.Visibility = Visibility.Visible;
			}
			ShowActivationCodeButton.Visibility = ((!ShowLicenseCodeLink()) ? Visibility.Collapsed : Visibility.Visible);
			SetTrialInfoVisibility(isVisible: false);
			if (exception == null)
			{
				ActiveLicenseDescriptionTextBlock.Text = StringResources.LicenseStatusControl_LicError;
				return;
			}
			SetActiveLicenseDescription(exception);
			if (base.LicenseManager.UnlockEnabled)
			{
				ActivateOverrideMode();
			}
		}

		private void SetActiveLicenseDescription(Exception exception)
		{
			if (base.LicenseManager.ProductLicense != null)
			{
				ActiveLicenseDescriptionTextBlock.Text = string.Format(StringResources.LicenseStatusControl_LicErrorDetails, exception.Message);
			}
			else
			{
				ActiveLicenseDescriptionTextBlock.Text = string.Format(StringResources.LicenseStatusControl_LicErrorDetailsRetry, exception.Message);
			}
		}

		public override void RefreshDisplay()
		{
			base.RefreshDisplay();
			if (base.CurrentLicense == null)
			{
				RefreshDisplayForNoCurrentLicense();
			}
			else
			{
				RefreshDisplayForCurrentLicense();
			}
		}

		private void RefreshDisplayForCurrentLicense()
		{
			if (base.CurrentLicense.IsTerminalService && base.CurrentLicense.Status == LicenseStatus.NotAuthorized)
			{
				ShowRemoteDesktopMessage();
				return;
			}
			ActiveLicenseDescriptionTextBlock.Text = base.LicenseManager.GetLicenseInfo();
			ViewDeactivationCertificateButton.Visibility = ((!ShowOldDeactivationCertificate()) ? Visibility.Collapsed : Visibility.Visible);
			bool flag = base.StatusPageMode == LicenseModeUI.NoLicense || base.StatusPageMode == LicenseModeUI.TrialExpired || base.StatusPageMode == LicenseModeUI.LeaseExpired;
			bool flag2 = base.StatusPageMode == LicenseModeUI.Trial;
			CancelButton.Content = (flag2 ? StringResources.StatusControl_ContinueTrial : (flag ? StringResources.StatusControl_Close : StringResources.StatusControl_Ok));
			RefreshDisplayBasedOnLicenseStatus();
		}

		private void RefreshDisplayBasedOnLicenseStatus()
		{
			switch (base.StatusPageMode)
			{
			case LicenseModeUI.NoLicense:
				RefreshDisplayForLicenseStatus();
				return;
			case LicenseModeUI.FullLicense:
				RefreshDisplayForFullLicense();
				return;
			case LicenseModeUI.Trial:
				RefreshDisplayForTrialLicense();
				return;
			case LicenseModeUI.TrialExpired:
				RefreshDisplayForTrialExpired();
				return;
			case LicenseModeUI.Lease:
				RefreshDisplayForLeaseLicense();
				return;
			case LicenseModeUI.LeaseExpired:
				RefreshDisplayForLeaseExpired();
				return;
			}
			SetActivateVisibility(isVisible: true);
			SetTrialInfoVisibility(isVisible: false);
			ShowPreviousVersionWarning(showWarning: false);
		}

		private void RefreshDisplayForLeaseExpired()
		{
			LicenseImage.Source = _imageTrial;
			SetActivateVisibility(isVisible: true);
			SetTrialInfoVisibility(isVisible: false);
			ShowPreviousVersionWarning(showWarning: false);
		}

		private void RefreshDisplayForLeaseLicense()
		{
			LicenseImage.Source = _imageTrial;
			SetActivateVisibility(isVisible: false);
			SetTrialInfoVisibility(isVisible: true);
			ShowPreviousVersionWarning(showWarning: false);
			if (!base.LicenseManager.CallIsLicenseValidForMachine(out string shortStatus, out string _))
			{
				LicenseImage.Source = _imageTimerWarning;
				TextBlock activeLicenseDescriptionTextBlock = ActiveLicenseDescriptionTextBlock;
				activeLicenseDescriptionTextBlock.Text = activeLicenseDescriptionTextBlock.Text + "\n" + shortStatus;
			}
		}

		private void RefreshDisplayForTrialExpired()
		{
			LicenseImage.Source = _imageTrial;
			SetActivateVisibility(isVisible: true);
			SetTrialInfoVisibility(isVisible: false);
			ShowPreviousVersionWarning(showWarning: false);
		}

		private void RefreshDisplayForTrialLicense()
		{
			LicenseImage.Source = _imageTrial;
			SetActivateVisibility(isVisible: true);
			SetTrialInfoVisibility(isVisible: true);
			ShowPreviousVersionWarning(showWarning: false);
		}

		private void RefreshDisplayForFullLicense()
		{
			LicenseImage.Source = _imageLicenced;
			SetActivateVisibility(isVisible: false);
			SetTrialInfoVisibility(isVisible: false);
			ShowPreviousVersionWarning(showWarning: false);
			if (!base.LicenseManager.CallIsLicenseValidForMachine(out string shortStatus, out string _))
			{
				LicenseImage.Source = _imageLicensedWarning;
				TextBlock activeLicenseDescriptionTextBlock = ActiveLicenseDescriptionTextBlock;
				activeLicenseDescriptionTextBlock.Text = activeLicenseDescriptionTextBlock.Text + "\n" + shortStatus;
			}
			if (base.LicenseManager.UnlockEnabled)
			{
				ActivateOverrideMode();
			}
		}

		private void RefreshDisplayForLicenseStatus()
		{
			LicenseImage.Source = _imageUnlicenced;
			if (base.CurrentLicense.Status == LicenseStatus.Authorized)
			{
				SetActivateVisibility(isVisible: false);
				ShowPreviousVersionWarning(showWarning: true);
			}
			else
			{
				SetActivateVisibility(isVisible: true);
				ShowPreviousVersionWarning(showWarning: false);
			}
			SetTrialInfoVisibility(isVisible: false);
		}

		private void RefreshDisplayForNoCurrentLicense()
		{
			try
			{
				base.LicenseManager.GetLicense();
				if (base.CurrentLicense != null)
				{
					base.LicenseManagerView.ShowStatusPage();
				}
				else
				{
					DisplayErrorStatus(null);
				}
			}
			catch (Exception exception)
			{
				DisplayErrorStatus(exception);
			}
		}

		private void ShowEulaPenaltiesWarning()
		{
			ShowWarningMessage(showWarning: true, StringResources.LicenseStatusControl_EulaPenaltiesWarning1, StringResources.LicenseStatusControl_EulaPenaltiesWarning2);
		}

		private void ShowPreviousVersionWarning(bool showWarning)
		{
			ShowWarningMessage(showWarning, StringResources.LicenseStatusControl_PreviousVersionWarning1, StringResources.LicenseStatusControl_PreviousVersionWarning2);
		}

		private void ShowWarningMessage(bool showWarning, string warningMsg1, string warningMsg2)
		{
			if (showWarning || FirstWarningTextBlock.Visibility != Visibility.Collapsed)
			{
				FirstWarningTextBlock.Visibility = ((!showWarning) ? Visibility.Collapsed : Visibility.Visible);
				SecondWarningTextBlock.Visibility = ((!showWarning) ? Visibility.Collapsed : Visibility.Visible);
				if (showWarning)
				{
					FirstWarningTextBlock.Text = ((!string.IsNullOrEmpty(warningMsg1)) ? warningMsg1 : "");
					SecondWarningTextBlock.Text = ((!string.IsNullOrEmpty(warningMsg2)) ? warningMsg2 : "");
				}
			}
		}

		private void ShowRemoteDesktopMessage()
		{
			LicenseImage.Source = _imageLicensedWarning;
			SetTrialInfoVisibility(isVisible: false);
			ActivateDeactivateButton.Visibility = Visibility.Collapsed;
			ActiveLicenseDescriptionTextBlock.Text = StringResources.RemoteDesktopMessage;
			ViewDeactivationCertificateButton.Visibility = Visibility.Collapsed;
			ShowActivationCodeButton.Visibility = Visibility.Collapsed;
			DeactivateOfflineButton.Visibility = Visibility.Collapsed;
			CancelButton.Content = StringResources.StatusControl_Close;
		}

		private void SetActivateVisibility(bool isVisible)
		{
			_isActivateButton = isVisible;
			SetActivateButton(isVisible);
			PurchaseLicenseButton.Visibility = ((!isVisible) ? Visibility.Collapsed : Visibility.Visible);
			DeactivateOfflineButton.Visibility = (isVisible ? Visibility.Collapsed : Visibility.Visible);
			ShowActivationCodeButton.Visibility = (isVisible ? Visibility.Collapsed : Visibility.Visible);
		}

		private void ActivateOverrideMode()
		{
			_isActivateButton = false;
			ActivateDeactivateButton.Visibility = Visibility.Collapsed;
			PurchaseLicenseButton.Visibility = Visibility.Collapsed;
			DeactivateOfflineButton.Visibility = Visibility.Collapsed;
			ShowActivationCodeButton.Visibility = Visibility.Collapsed;
			ShowEulaPenaltiesWarning();
		}

		private void SetActivateButton(bool isVisible)
		{
			if (isVisible)
			{
				MainButtonsStackPanel.Margin = new Thickness(0.0, 15.0, 0.0, 0.0);
				ActivateDeactivateButtonText.Content = StringResources.LicenseStatusControl_Activate;
				ActivateDeactivateButtonImage.Source = _activateKeys;
				ActivateDeactivateButton.IsEnabled = base.LicenseManagerView.CanActivateAPerpetual;
				if (base.StatusPageMode == LicenseModeUI.Trial)
				{
					CancelButton.IsEnabled = base.LicenseManagerView.CanActivateAPerpetual;
				}
				else
				{
					CancelButton.IsEnabled = true;
				}
			}
			else
			{
				MainButtonsStackPanel.Margin = new Thickness(0.0, 40.0, 0.0, 0.0);
				ActivateDeactivateButtonText.Content = StringResources.LicenseStatusControl_Deactivate;
				ActivateDeactivateButtonImage.Source = _deactivateKeys;
			}
		}

		private void SetTrialInfoVisibility(bool isVisible)
		{
			TrialRemainingBar.Visibility = ((!isVisible) ? Visibility.Collapsed : Visibility.Visible);
			TrialRemainingTextBlock.Visibility = ((!isVisible) ? Visibility.Collapsed : Visibility.Visible);
			PurchaseLicenseButton.Visibility = ((!isVisible) ? Visibility.Collapsed : Visibility.Visible);
			if (isVisible)
			{
				SetTrialRemainingTimeInfo();
			}
		}

		private void SetTrialRemainingTimeInfo()
		{
			int daysRemainingOnLicense = Helpers.GetDaysRemainingOnLicense(base.CurrentLicense);
			TrialRemainingTextBlock.Text = string.Format(StringResources.LicenseStatusControl_DaysRemaining, daysRemainingOnLicense);
			int num = (daysRemainingOnLicense <= 30) ? daysRemainingOnLicense : 30;
			TrialRemainingBar.Maximum = 30.0;
			TrialRemainingBar.Value = num;
			if (num <= 7 || base.CurrentLicense.Status != 0)
			{
				TrialRemainingTextBlock.Foreground = new SolidColorBrush(Colors.Red);
			}
			else
			{
				TrialRemainingTextBlock.Foreground = new SolidColorBrush(Colors.Green);
			}
		}

		private bool ShowOldDeactivationCertificate()
		{
			return !string.IsNullOrEmpty(base.LicenseManager.LicensingProvider.Configuration.Registry.DeactivationCode);
		}

		private bool ShowLicenseCodeLink()
		{
			return !string.IsNullOrEmpty(base.LicenseManager.LicensingProvider.Configuration.Registry.LicenseCode);
		}

		private void ShowActivationCodeButton_Click(object sender, RoutedEventArgs e)
		{
			Helpers.ShowActivationCodeDialog(base.LicenseManager.LicensingProvider.Configuration.Registry.LicenseCode);
		}

		private void DeactivateOfflineButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowOfflineDeactivationPage();
		}

		private void ViewDeactivationCertificateButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowViewDeactivationCertificatePage();
		}

		private void ActivateDeactivateButton_Click(object sender, RoutedEventArgs e)
		{
			if (_isActivateButton)
			{
				if (base.LicenseManager.EnableServerLicensing)
				{
					if (OSVersion.isTerminalServicesInstalled())
					{
						base.LicenseManagerView.ShowLicenseServerConfigControl();
					}
					else
					{
						base.LicenseManagerView.ShowOnlineActivationPage();
					}
				}
				else
				{
					base.LicenseManagerView.ShowOnlineActivationPage();
				}
			}
			else
			{
				base.LicenseManagerView.PerformOnlineDeactivation(showConfirmationQuestion: true);
			}
		}

		private void PurchaseLicenseButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(base.LicenseManager.PurchaseLinkUrl))
			{
				WebLinkHelper.DisplayBrowser(base.LicenseManager.PurchaseLinkUrl);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void HelpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.LicenseStatusScreen);
		}

		private void SdlAccountButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(base.LicenseManager.MyAccountLinkUrl))
			{
				WebLinkHelper.DisplayBrowser(base.LicenseManager.MyAccountLinkUrl);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/licensestatuscontrol.xaml", UriKind.Relative);
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
				SdlAccountButton = (Button)target;
				SdlAccountButton.Click += SdlAccountButton_Click;
				break;
			case 2:
				CancelButton = (Button)target;
				CancelButton.Click += CancelButton_Click;
				break;
			case 3:
				HelpButton = (Button)target;
				HelpButton.Click += HelpButton_Click;
				break;
			case 4:
				LicenseImage = (Image)target;
				break;
			case 5:
				ActiveLicenseDescriptionTextBlock = (TextBlock)target;
				break;
			case 6:
				TrialRemainingBar = (ProgressBar)target;
				break;
			case 7:
				TrialRemainingTextBlock = (TextBlock)target;
				break;
			case 8:
				ShowActivationCodeButton = (Button)target;
				ShowActivationCodeButton.Click += ShowActivationCodeButton_Click;
				break;
			case 9:
				DeactivateOfflineButton = (Button)target;
				DeactivateOfflineButton.Click += DeactivateOfflineButton_Click;
				break;
			case 10:
				ViewDeactivationCertificateButton = (Button)target;
				ViewDeactivationCertificateButton.Click += ViewDeactivationCertificateButton_Click;
				break;
			case 11:
				FirstWarningTextBlock = (TextBlock)target;
				break;
			case 12:
				SecondWarningTextBlock = (TextBlock)target;
				break;
			case 13:
				MainButtonsStackPanel = (StackPanel)target;
				break;
			case 14:
				ActivateDeactivateButton = (Button)target;
				ActivateDeactivateButton.Click += ActivateDeactivateButton_Click;
				break;
			case 15:
				ActivateDeactivateButtonImage = (Image)target;
				break;
			case 16:
				ActivateDeactivateButtonText = (Label)target;
				break;
			case 17:
				PurchaseLicenseButton = (Button)target;
				PurchaseLicenseButton.Click += PurchaseLicenseButton_Click;
				break;
			case 18:
				PurchaseLicenseButtonImage = (Image)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
