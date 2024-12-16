using MahApps.Metro.Controls;
using Sdl.Common.Licensing.Provider.Core.Exceptions;
using Sdl.Common.Licensing.Provider.Core.UI.Commands;
using Sdl.Common.Licensing.Provider.Core.UI.WPFControls;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class LicenseManagerView : UserControl, ILicensingDialog, IComponentConnector
	{
		private readonly List<CustomStatusPage> _customStatusPages = new List<CustomStatusPage>();

		private bool _canActivateAPerpetual = true;

		internal KeyBinding ShortcutCommand;

		internal Border ProgressRingBorder;

		internal ProgressRing ProgressRing;

		internal DockPanel LicenseContentDockPanel;

		internal TextBlock Title;

		internal TextBlock SubTitle;

		internal ContentPresenter ContentControl;

		private bool _contentLoaded;

		public bool IsDiagnosticPageDisabled
		{
			get;
			private set;
		}

		public bool IsProcessInProgress
		{
			get
			{
				return ProgressRing.IsActive;
			}
			set
			{
				ProgressRing.IsActive = value;
				ProgressRingBorder.Visibility = ((!value) ? Visibility.Collapsed : Visibility.Visible);
				LicenseContentDockPanel.Visibility = (value ? Visibility.Collapsed : Visibility.Visible);
			}
		}

		internal LicenseManager LicenseManager
		{
			get;
			private set;
		}

		public ICommand CloseCommand
		{
			get;
			internal set;
		}

		public bool CanActivateAPerpetual
		{
			get
			{
				return _canActivateAPerpetual;
			}
			set
			{
				if (value != _canActivateAPerpetual)
				{
					_canActivateAPerpetual = value;
					ShowStatusPage();
				}
			}
		}

		public event EventHandler CloseUI;

		public event EventHandler LicenseStatusChanging;

		public event EventHandler LicenseStatusChanged;

		public LicenseManagerView()
		{
			InitializeComponent();
			ShortcutCommand.Command = new DiagnosticPageCommand();
			ShortcutCommand.CommandParameter = this;
		}

		internal void Initialize(LicenseManager licenseManager, IList<CustomStatusPage> customStatusPages)
		{
			LicenseManager = licenseManager;
			if (customStatusPages != null)
			{
				AddCustomStatusPages(customStatusPages);
			}
			ShowStatusPage();
		}

		public void OnLicenseStatusChanging()
		{
			this.LicenseStatusChanging?.Invoke(null, null);
			this.LicenseStatusChanged?.Invoke(null, null);
		}

		public void PerformOnlineActivation(string activationCode, Action showPageOnFailure)
		{
			IsProcessInProgress = true;
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs eventArgs)
			{
				try
				{
					LicenseManager.Activate(activationCode);
					eventArgs.Result = true;
				}
				catch (LicensingProviderException exception)
				{
					HandleFileCheckException(StringResources.LicenseManagerForm_ActivationFailed, exception, StringResources.OnlineActivationFailureSubMessage, ShowOfflineActivationPage, showPageOnFailure);
					eventArgs.Result = false;
				}
				catch (Exception ex)
				{
					MessageLog.DefaultLog.Error("PerformOnlineActivation", ex);
					Helpers.ShowException(StringResources.LicenseManagerForm_ActivationFailed, ex);
					showPageOnFailure?.Invoke();
					eventArgs.Result = false;
				}
			};
			backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs eventArgs)
			{
				if ((bool)eventArgs.Result)
				{
					LicenseManager.GetLicense();
					IsProcessInProgress = false;
					StringBuilder stringBuilder = new StringBuilder();
					if (LicenseManager.IsProductAllowed != null && !LicenseManager.IsProductAllowed(LicenseManager.ProductLicense, stringBuilder))
					{
						Helpers.ShowInformation(stringBuilder.ToString(), StringResources.DesktopLicensing_Activation_MessageBox_Title);
						PerformOnlineDeactivation(showConfirmationQuestion: false);
					}
					ShowStatusPage();
				}
				IsProcessInProgress = false;
				OnLicenseStatusChanging();
			};
			backgroundWorker.RunWorkerAsync();
		}

		public void PerformOnlineDeactivation(bool showConfirmationQuestion)
		{
			PerformOnlineDeactivation(showConfirmationQuestion, ShowStatusPage);
			OnLicenseStatusChanging();
		}

		private void PerformOnlineDeactivation(bool showConfirmationQuestion, Action showPageOnFailure)
		{
			if (!showConfirmationQuestion || MessageBoxResult.No != Helpers.ShowQuestion(StringResources.LicenseStatusControl_ConfirmDeactivateProduct, StringResources.LicenseStatusControl_ConfirmDeactivateProduct_Title, MessageBoxButton.YesNo))
			{
				string licenseCode = LicenseManager.LicensingProvider.Configuration.Registry.LicenseCode;
				if (licenseCode == null)
				{
					Helpers.ShowError(StringResources.DesktopLicensing_Deactivation_Error_UnableToObtainActivationCode);
					ShowOnlineDeactivationWithNoCodePage();
				}
				else if (Helpers.MultipleInstancesRunning)
				{
					ShowMultipleInstanceRunningInformationMessage();
				}
				else
				{
					PerformOnlineDeactivation(licenseCode, showPageOnFailure);
				}
			}
		}

		public void PerformOnlineDeactivation(string licenseCode, Action showPageOnFailure)
		{
			IsProcessInProgress = true;
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs eventArgs)
			{
				eventArgs.Result = PerformDeactivation(licenseCode, showPageOnFailure);
			};
			backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs eventArgs)
			{
				if ((bool)eventArgs.Result)
				{
					ShowStatusPage();
				}
				IsProcessInProgress = false;
				OnLicenseStatusChanging();
			};
			backgroundWorker.RunWorkerAsync();
		}

		private bool PerformDeactivation(string licenseCode, Action showPageOnFailure)
		{
			try
			{
				LicenseManager.ReturnLicense();
				LicenseManager.Deactivate(licenseCode);
			}
			catch (LicensingProviderException exception)
			{
				MessageLog.DefaultLog.Error("PerformOnlineDeactivation", exception);
				HandleFileCheckException(StringResources.LicenseManagerForm_DeactivationFailed, exception, StringResources.OnlineDeactivationFailureSubMessage, ShowOfflineDeactivationPage, showPageOnFailure);
				return false;
			}
			catch (Exception ex)
			{
				MessageLog.DefaultLog.Error("PerformOnlineDeactivation", ex);
				Helpers.ShowException(StringResources.LicenseManagerForm_DeactivationFailed, ex);
				showPageOnFailure?.Invoke();
				return false;
			}
			return true;
		}

		private bool IsValidServer(string serverName)
		{
			bool result = true;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(serverName);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public bool ServerExists(string serverName)
		{
			if (serverName.Length == 0)
			{
				return false;
			}
			if (!Helpers.isValidNetBiosName(serverName))
			{
				Helpers.ShowError(StringResources.ServerNameNetBiosError);
				return false;
			}
			if (!IsValidServer(serverName))
			{
				Helpers.ShowError(string.Format(StringResources.LicenseServerConfigControl_Server_Connect_Error, StringResources.NoConnectionTo + serverName), StringResources.LicenseServerActivationTitleBar);
				return false;
			}
			return true;
		}

		public void PerformNetworkActivation(string serverName)
		{
			IsProcessInProgress = true;
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += delegate
			{
				PerformNetworkActivationInternal(serverName);
			};
			backgroundWorker.RunWorkerCompleted += delegate
			{
				IsProcessInProgress = false;
				ShowStatusPage();
				OnLicenseStatusChanging();
			};
			backgroundWorker.RunWorkerAsync();
		}

		private void PerformNetworkActivationInternal(object serverName)
		{
			try
			{
				if (LicenseManager.ProductLicense.IsLoggedIn)
				{
					LicenseManager.ClearCachedLicense();
				}
				LicenseManager.NetworkActivate((string)serverName);
			}
			catch (LicensingProviderException ex)
			{
				MessageLog.DefaultLog.Error("Error connecting to license server.", ex);
				Helpers.ShowError(ex.ErrorMessage);
			}
			catch (Exception ex2)
			{
				MessageLog.DefaultLog.Error("Error connecting to license server.", ex2);
				Helpers.ShowException(ex2);
			}
		}

		private bool HandleFileCheckException(string message, LicensingProviderException exception, string offlineMessage, Action offlineAction, Action defaultFallBackAction)
		{
			MessageBoxButton buttons = MessageBoxButton.OK;
			StringBuilder stringBuilder = new StringBuilder(message);
			if (!string.IsNullOrEmpty(exception.Message))
			{
				stringBuilder.AppendFormat("{0}{1}", Environment.NewLine, exception.Message);
			}
			if (ShouldShowNetworkSettingsOption(exception.ErrorCode))
			{
				stringBuilder.AppendFormat("{1}{1}{0}{1}{2}", StringResources.OnlineActionFailWouldYouLikeToCheckConnectionSettings, Environment.NewLine, offlineMessage);
				buttons = MessageBoxButton.YesNo;
			}
			switch (Helpers.ShowError(stringBuilder.ToString(), buttons))
			{
			case MessageBoxResult.Yes:
			{
				Action method = ShowConnectionSettingsControlPage;
				base.Dispatcher.BeginInvoke(method);
				return true;
			}
			case MessageBoxResult.No:
				base.Dispatcher.BeginInvoke(offlineAction);
				return true;
			default:
				if (defaultFallBackAction != null)
				{
					base.Dispatcher.BeginInvoke(defaultFallBackAction);
				}
				return false;
			}
		}

		private static bool ShouldShowNetworkSettingsOption(long errorCode)
		{
			return false;
		}

		public void ShowStatusPage()
		{
			IsDiagnosticPageDisabled = false;
			if (!LicenseManager.EnableServerLicensing || !LicenseManager.UseLicenseServer)
			{
				try
				{
					LicenseManager.ProductLicense?.CheckOut();
				}
				catch (Exception exception)
				{
					MessageLog.DefaultLog.Error("LicenseManagerForm.ShowStatusPage", exception);
				}
			}
			IProductLicense productLicense = LicenseManager.ProductLicense;
			bool flag = LicenseManager.EnableServerLicensing && LicenseManager.UseLicenseServer;
			if (productLicense != null)
			{
				if (!productLicense.IsBorrowed && flag && productLicense.Status == LicenseStatus.LeaseExpired)
				{
					ShowLicenseStatusServerControl();
					return;
				}
				if (productLicense.Mode == LicenseMode.Exported && (productLicense.Status == LicenseStatus.Authorized || productLicense.Status == LicenseStatus.ExportExpired || productLicense.Status == LicenseStatus.LeaseExpired))
				{
					ShowBorrowLicenseStatusControl();
					return;
				}
				foreach (CustomStatusPage customStatusPage in _customStatusPages)
				{
					if (customStatusPage.ShowPageWhen(productLicense))
					{
						ShowPage(customStatusPage.StatusPage);
						return;
					}
				}
			}
			if (flag && !LicenseManager.UnlockEnabled)
			{
				ShowLicenseStatusServerControl();
			}
			else
			{
				ShowLicenseStatusControl();
			}
		}

		public void ShowOnlineActivationPage()
		{
			InitializeAndShowPage(new OnlineActivationControl(this));
		}

		public void ShowOnlineDeactivationWithNoCodePage()
		{
			InitializeAndShowPage(new OnlineDeactivationControl(this));
		}

		public void Close()
		{
			this.CloseUI?.Invoke(null, null);
			CloseCommand.Execute(null);
		}

		public void ShowOfflineActivationPage()
		{
			InitializeAndShowPage(new OfflineActivationControl(this));
		}

		public void ShowOfflineDeactivationPage()
		{
			if (Helpers.MultipleInstancesRunning)
			{
				ShowMultipleInstanceRunningInformationMessage();
			}
			else
			{
				InitializeAndShowPage(LicenseManager.LicensingControlsProvider.GetOfflineDeactivationControl(this));
			}
		}

		public void ShowAlternativeActivationOptionsPage()
		{
			IsDiagnosticPageDisabled = false;
			InitializeAndShowPage(new AlternativeActivationOptionsControl(this));
		}

		public void ShowViewDeactivationCertificatePage()
		{
			InitializeAndShowPage(new ViewDeactivationCertificateControl(this));
		}

		public void ShowLicenseServerConfigControl()
		{
			IsDiagnosticPageDisabled = true;
			InitializeAndShowPage(new LicenseServerConfigControl(this));
		}

		public void ShowLicenseServerEditionsControl()
		{
			InitializeAndShowPage(LicenseManager.LicensingControlsProvider.GetLicenseServerEditionsControl(this));
		}

		public void ShowLicenseStatusServerControl()
		{
			InitializeAndShowPage(new LicenseStatusServerControl(this));
		}

		public void ShowLicenseStatusControl()
		{
			InitializeAndShowPage(new LicenseStatusControl(this));
		}

		public void ShowBorrowLicenseControl()
		{
			InitializeAndShowPage(LicenseManager.LicensingControlsProvider.GetBorrowLicenseControl(this));
		}

		public void ShowBorrowLicenseStatusControl()
		{
			InitializeAndShowPage(new BorrowLicenseStatusServerControl(this));
		}

		public void ShowDiagnosticPage()
		{
			InitializeAndShowPage(new DiagnosticPageControl(this));
		}

		public void ShowConnectionSettingsControlPage()
		{
			InitializeAndShowPage(new ConnectionSettingsControl(this));
		}

		private void InitializeAndShowPage(IUIControl control)
		{
			InitializeControl(control);
			ShowPage(control);
		}

		internal void AddCustomStatusPages(IList<CustomStatusPage> customStatusPages)
		{
			foreach (CustomStatusPage customStatusPage in customStatusPages)
			{
				AddCustomStatusPage(customStatusPage);
			}
		}

		internal void AddCustomStatusPage(CustomStatusPage customStatusPage)
		{
			_customStatusPages.Add(customStatusPage);
			InitializeControl(customStatusPage.StatusPage);
		}

		private void InitializeControl(IUIControl licensePage)
		{
			if (licensePage is UIElement)
			{
				licensePage.Initialize(LicenseManager.LicensingForm);
			}
		}

		private void ShowPage(IUIControl licensePage)
		{
			licensePage.Initialize(LicenseManager.LicensingForm);
			Title.Text = licensePage.MainTitle;
			SubTitle.Text = licensePage.SubTitle;
			ContentControl.Content = licensePage;
			ContentControl.Focusable = true;
			ContentControl.IsEnabled = true;
			ContentControl.Focus();
			licensePage.RefreshDisplay();
		}

		internal void ActivatePage(LicensePageType pageType)
		{
			switch (pageType)
			{
			case LicensePageType.BorrowLicense:
				ShowBorrowLicenseControl();
				break;
			case LicensePageType.LicenseServerStatus:
				ShowLicenseStatusServerControl();
				break;
			case LicensePageType.LicenseStatus:
				ShowLicenseStatusControl();
				break;
			case LicensePageType.OfflineActivation:
				ShowOfflineActivationPage();
				break;
			case LicensePageType.OfflineDeactivation:
				ShowOfflineDeactivationPage();
				break;
			case LicensePageType.OnlineActivation:
				ShowOnlineActivationPage();
				break;
			case LicensePageType.OnlineDeactivation:
				ShowOnlineDeactivationWithNoCodePage();
				break;
			}
		}

		public void ShowMultipleInstanceRunningInformationMessage()
		{
			Helpers.ShowInformation(StringResources.LicenseStatusControl_MultipleInstancesRunning, StringResources.LicenseStatusControl_ProductDeactivation_Title);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/licensemanagerview.xaml", UriKind.Relative);
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
				ShortcutCommand = (KeyBinding)target;
				break;
			case 2:
				ProgressRingBorder = (Border)target;
				break;
			case 3:
				ProgressRing = (ProgressRing)target;
				break;
			case 4:
				LicenseContentDockPanel = (DockPanel)target;
				break;
			case 5:
				Title = (TextBlock)target;
				break;
			case 6:
				SubTitle = (TextBlock)target;
				break;
			case 7:
				ContentControl = (ContentPresenter)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
