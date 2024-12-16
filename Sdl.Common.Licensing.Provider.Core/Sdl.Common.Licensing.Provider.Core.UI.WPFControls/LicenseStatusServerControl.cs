using Sdl.Common.Licensing.Provider.Core.Exceptions;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	internal class LicenseStatusServerControl : LicensePageControlWPF, IUIControl, IComponentConnector
	{
		private enum InternalLicenseStatus
		{
			NoLicense,
			LicenseExpired,
			ServerConnected,
			ServerError
		}

		public bool IsInProgress;

		private bool InRetryMode = true;

		internal Button _okCancelButton;

		internal Button _helpButton;

		internal Image _licensePictureBox;

		internal TextBlock label1;

		internal TextBlock _licenseServerLabel;

		internal TextBlock _activeLicenseLabel;

		internal TextBlock _activeLicenseDescriptionLabel;

		internal Button disconnectlinkLabel;

		internal Button _borrowLicenselinkLabel;

		internal Button _changeLicenseSeverLinkLabel;

		internal Button _actionButton;

		internal Image DisconnectImage;

		internal Label DisconnectButton;

		private bool _contentLoaded;

		public LicenseStatusServerControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			_actionButton.ToolTip = StringResources.LicenseServerConfigControl_AdminRequiredTooltip;
		}

		private void SetLicensingConnectionStatus(object productLicense)
		{
			try
			{
				base.LicenseManager.IsLicenseServerRunning = ((productLicense as IProductLicense)?.IsServerRunning() ?? false);
			}
			catch (LicensingProviderException ex)
			{
				if (ex.ErrorCode == 64)
				{
					base.LicenseManager.IsLicenseServerRunning = true;
					HandleGetLicenseError(ex);
				}
			}
			catch (Exception exception)
			{
				base.LicenseManager.IsLicenseServerRunning = false;
				HandleGetLicenseError(exception);
			}
		}

		public override void RefreshDisplay()
		{
			IProductLicense productLicense = null;
			try
			{
				_actionButton.Visibility = Visibility.Collapsed;
				disconnectlinkLabel.Visibility = Visibility.Collapsed;
				_borrowLicenselinkLabel.Visibility = Visibility.Collapsed;
				_licenseServerLabel.Text = base.LicenseManager.LicenseServer;
				_changeLicenseSeverLinkLabel.Visibility = ((!base.LicenseManager.CanUpdateLicenseServerName) ? Visibility.Collapsed : Visibility.Visible);
				productLicense = base.LicenseManager.GetProductLicense();
			}
			catch (Exception exception)
			{
				HandleGetLicenseError(exception);
			}
			if (productLicense == null)
			{
				SetControlsForLicenseStatus(InternalLicenseStatus.NoLicense);
				if (base.LicenseManager.UseLicenseServer)
				{
					disconnectlinkLabel.Visibility = Visibility.Visible;
				}
				return;
			}
			_activeLicenseLabel.Visibility = Visibility.Visible;
			try
			{
				if (!productLicense.IsLocal && !productLicense.IsLoggedIn && productLicense.Status != LicenseStatus.LeaseExpired)
				{
					base.LicenseManager.ClearCachedLicense();
					base.LicenseManager.GetLicense();
				}
				else
				{
					_activeLicenseDescriptionLabel.Text = base.LicenseManager.GetLicenseInfo();
				}
			}
			catch (LicensingProviderException ex)
			{
				HandleGetLicenseError(ex);
				base.LicenseManager.ClearCachedLicense();
				if (ex.ErrorCode == 4)
				{
					base.LicenseManager.ProductLicense.NotAuthorized();
					disconnectlinkLabel.Visibility = Visibility.Visible;
				}
				return;
			}
			try
			{
				base.LicenseManagerView.IsProcessInProgress = true;
				SetLicensingConnectionStatus(productLicense);
			}
			catch (LicensingProviderException ex2)
			{
				MessageLog.DefaultLog.Error("Error connecting to license server.", ex2);
				Helpers.ShowError(ex2.ErrorMessage);
			}
			catch (Exception ex3)
			{
				MessageLog.DefaultLog.Error("Error connecting to license server.", ex3);
				Helpers.ShowException(ex3);
			}
			finally
			{
				base.LicenseManagerView.IsProcessInProgress = false;
			}
			if (!base.LicenseManager.IsLicenseServerRunning)
			{
				_licenseServerLabel.Text += $" [{StringResources.NoConnection}]";
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromRgb(10, 30, 44));
			_licenseServerLabel.Foreground = (base.LicenseManager.IsLicenseServerRunning ? solidColorBrush : Brushes.Red);
			disconnectlinkLabel.Visibility = (base.LicenseManager.IsLicenseServerRunning ? Visibility.Collapsed : Visibility.Visible);
			if (base.StatusPageMode == LicenseModeUI.NoLicense)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (productLicense.Status == LicenseStatus.Authorized && base.LicenseManager.IsProductAllowed != null && !base.LicenseManager.IsProductAllowed(productLicense, stringBuilder))
				{
					_activeLicenseDescriptionLabel.Text = stringBuilder.ToString();
				}
				SetControlsForLicenseStatus(InternalLicenseStatus.ServerError);
				if (!productLicense.IsLocal)
				{
					_changeLicenseSeverLinkLabel.Visibility = Visibility.Visible;
				}
			}
			else if (!base.LicenseManager.IsLicenseServerRunning)
			{
				SetControlsForLicenseStatus(InternalLicenseStatus.ServerError);
			}
			else
			{
				SetControlsForLicenseStatus(InternalLicenseStatus.ServerConnected);
				if (productLicense.Status == LicenseStatus.LeaseExpired)
				{
					SetControlsForLicenseStatus(InternalLicenseStatus.LicenseExpired);
				}
				InRetryMode = false;
			}
		}

		private void LicenseManager_GetLicenseCompleted(object sender, GetLicenseCompletedEventArgs e)
		{
			base.LicenseManager.GetLicenseCompleted -= LicenseManager_GetLicenseCompleted;
			_actionButton.IsEnabled = true;
			_okCancelButton.IsEnabled = true;
			if (e.Error != null)
			{
				HandleGetLicenseError(e.Error);
			}
			else
			{
				RefreshDisplay();
			}
			base.LicenseManagerView.OnLicenseStatusChanging();
		}

		private void RetryToObtainLicense()
		{
			_actionButton.IsEnabled = false;
			_okCancelButton.IsEnabled = false;
			base.LicenseManager.GetLicenseCompleted += LicenseManager_GetLicenseCompleted;
			ShowRetrievingMessage();
			base.LicenseManager.GetLicenseAsync();
		}

		private void ShowRetrievingMessage()
		{
			_activeLicenseLabel.Visibility = Visibility.Collapsed;
			_activeLicenseDescriptionLabel.Text = StringResources.ServerStatusControl_RetrievingLicense;
			DisconnectImage.Source = BitmapToImageSource(ImageResources.server_into_32x32);
			DisconnectButton.Content = "    " + StringResources.LicenseStatusServerControl_Retry;
		}

		private void _actionButton_Click(object sender, RoutedEventArgs e)
		{
			if (!InRetryMode)
			{
				DisconnectFromLicenseServer();
			}
			else
			{
				RetryToObtainLicense();
			}
		}

		private void DisconnectFromLicenseServer()
		{
			base.LicenseManager.UseLicenseServer = false;
			base.LicenseManager.ResetCurrentLicense();
			base.LicenseManagerView.ShowStatusPage();
			base.LicenseManagerView.OnLicenseStatusChanging();
		}

		private void _okCancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void _helpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.LicenseStatusScreen);
		}

		private void _changeLicenseSeverLinkLabel_LinkClicked(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ReturnLicense();
			base.LicenseManagerView.ShowLicenseServerConfigControl();
		}

		private void HandleGetLicenseError(Exception exception)
		{
			MessageLog.DefaultLog.Error(exception);
			LicensingProviderException ex = exception as LicensingProviderException;
			string message = (ex != null) ? ex.ErrorMessage : StringResources.LicenseStatusServerControl_Error_General;
			base.Dispatcher.Invoke(delegate
			{
				disconnectlinkLabel.Visibility = Visibility.Collapsed;
				_activeLicenseDescriptionLabel.Text = message;
				_actionButton.Visibility = Visibility.Visible;
				DisconnectImage.Source = BitmapToImageSource(ImageResources.server_into_32x32);
				DisconnectButton.Content = "    " + StringResources.LicenseStatusServerControl_Retry;
			});
			SetControlsForLicenseStatus((ex != null) ? InternalLicenseStatus.ServerError : InternalLicenseStatus.NoLicense);
			if (ex != null && ex.ErrorCode == 4)
			{
				base.LicenseManager.ProductLicense.NotAuthorized();
				disconnectlinkLabel.Visibility = Visibility.Visible;
			}
		}

		private void disconnectlinkLabel_LinkClicked(object sender, RoutedEventArgs e)
		{
			DisconnectFromLicenseServer();
			base.LicenseManagerView.OnLicenseStatusChanging();
		}

		private void _borrowLicenselinkLabel_LinkClicked(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowBorrowLicenseControl();
		}

		private void SetControlsForLicenseStatus(InternalLicenseStatus status)
		{
			switch (status)
			{
			case InternalLicenseStatus.NoLicense:
				base.Dispatcher.Invoke(delegate
				{
					_okCancelButton.Content = StringResources.StatusControl_Close;
					_licensePictureBox.Source = BitmapToImageSource(ImageResources.server_error_48x48);
					_activeLicenseLabel.Visibility = Visibility.Collapsed;
				});
				break;
			case InternalLicenseStatus.ServerConnected:
				base.Dispatcher.Invoke(delegate
				{
					_okCancelButton.Content = StringResources.StatusControl_Ok;
					_licensePictureBox.Source = BitmapToImageSource(ImageResources.server_ok_48x48);
					_activeLicenseLabel.Visibility = Visibility.Visible;
					DisconnectImage.Source = BitmapToImageSource(ImageResources.server_error_32x32);
					DisconnectButton.Content = StringResources.LicenseStatusServerControl_Disconnect;
					_actionButton.Visibility = Visibility.Visible;
					_actionButton.IsEnabled = base.LicenseManager.CanUpdateLicenseServerName;
					_borrowLicenselinkLabel.Visibility = ((!base.LicenseManager.CanUpdateLicenseServerName) ? Visibility.Collapsed : Visibility.Visible);
				});
				break;
			case InternalLicenseStatus.ServerError:
				base.Dispatcher.Invoke(delegate
				{
					_okCancelButton.Content = StringResources.StatusControl_Close;
					_licensePictureBox.Source = BitmapToImageSource(ImageResources.server_error_48x48);
					_activeLicenseLabel.Visibility = Visibility.Collapsed;
					DisconnectImage.Source = BitmapToImageSource(ImageResources.server_into_32x32);
					DisconnectButton.Content = StringResources.LicenseStatusServerControl_Retry;
					_actionButton.Visibility = Visibility.Visible;
					_borrowLicenselinkLabel.Visibility = Visibility.Collapsed;
					_changeLicenseSeverLinkLabel.Visibility = Visibility.Visible;
				});
				break;
			case InternalLicenseStatus.LicenseExpired:
				base.Dispatcher.Invoke(delegate
				{
					_okCancelButton.Content = StringResources.StatusControl_Close;
					_licensePictureBox.Source = BitmapToImageSource(ImageResources.server_error_48x48);
					_activeLicenseLabel.Visibility = Visibility.Collapsed;
					_activeLicenseDescriptionLabel.Foreground = Brushes.Red;
					_borrowLicenselinkLabel.Visibility = Visibility.Collapsed;
				});
				break;
			}
			IsInProgress = false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/licensestatusservercontrol.xaml", UriKind.Relative);
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
				label1 = (TextBlock)target;
				break;
			case 5:
				_licenseServerLabel = (TextBlock)target;
				break;
			case 6:
				_activeLicenseLabel = (TextBlock)target;
				break;
			case 7:
				_activeLicenseDescriptionLabel = (TextBlock)target;
				break;
			case 8:
				disconnectlinkLabel = (Button)target;
				disconnectlinkLabel.Click += disconnectlinkLabel_LinkClicked;
				break;
			case 9:
				_borrowLicenselinkLabel = (Button)target;
				_borrowLicenselinkLabel.Click += _borrowLicenselinkLabel_LinkClicked;
				break;
			case 10:
				_changeLicenseSeverLinkLabel = (Button)target;
				_changeLicenseSeverLinkLabel.Click += _changeLicenseSeverLinkLabel_LinkClicked;
				break;
			case 11:
				_actionButton = (Button)target;
				_actionButton.Click += _actionButton_Click;
				break;
			case 12:
				DisconnectImage = (Image)target;
				break;
			case 13:
				DisconnectButton = (Label)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
