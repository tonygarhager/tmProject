using Sdl.Common.Licensing.Provider.Core.Exceptions;
using Sdl.Common.Licensing.Provider.Core.UI.Commands;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	internal class OfflineActivationControl : LicensePageControlWPF, IUIControl, IComponentConnector
	{
		private bool _canActivate = false;

		internal KeyBinding EscapeCommand;

		internal KeyBinding EnterCommand;

		internal Button CancelButton;

		internal Button HelpButton;

		internal TextBox ActivationCodeTextBox;

		internal TextBox InstallationIDTextBox;

		internal TextBox ActivationCertificateTextBox;

		internal Button AlternativeActivationButton;

		internal Button PurchaseLicenseButton;

		internal Button ActivateButton;

		internal Image ActivateImage;

		internal Button ActivationCodePasteButton;

		internal Image ActivationCodePasteImage;

		internal Button InstallationIDCopyButton;

		internal Image InstallationIDCopyImage;

		internal Button ActivationCertificatePasteButton;

		internal Image ActivationCertificatePasteImage;

		private bool _contentLoaded;

		private string ActivationCode
		{
			get
			{
				return ActivationCodeTextBox.Text;
			}
			set
			{
				ActivationCodeTextBox.Text = value;
			}
		}

		private string InstallationId
		{
			get
			{
				return InstallationIDTextBox.Text;
			}
			set
			{
				InstallationIDTextBox.Text = value;
			}
		}

		private string ActivationCertificate
		{
			get
			{
				return ActivationCertificateTextBox.Text;
			}
			set
			{
				ActivationCertificateTextBox.Text = value;
			}
		}

		private bool CanActivate => _canActivate && !string.IsNullOrEmpty(ActivationCertificate);

		public OfflineActivationControl()
		{
			InitializeComponent();
		}

		public OfflineActivationControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.OfflineActivationControl_MainTitle;
			base.SubTitle = StringResources.OfflineActivationControl_SubTitle;
			ActivateImage.Source = BitmapToImageSource(ImageResources.key_activate_24x24);
			ActivationCodePasteImage.Source = BitmapToImageSource(ImageResources.paste);
			InstallationIDCopyImage.Source = BitmapToImageSource(ImageResources.copy);
			ActivationCertificatePasteImage.Source = BitmapToImageSource(ImageResources.paste);
			EscapeCommand.Command = new EscapeCommand(this);
			EnterCommand.CommandParameter = CanActivate;
			EnterCommand.Command = new EnterCommand(this);
		}

		public override void RefreshDisplay()
		{
			base.RefreshDisplay();
			ActivationCodeTextBox.Focus();
			ActivateButton.IsEnabled = false;
			ActivationCode = string.Empty;
			ActivationCertificate = string.Empty;
		}

		private void HelpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.OfflineActivation);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			ExecuteEscapeCommand();
		}

		public override void ExecuteEscapeCommand()
		{
			base.LicenseManagerView.ShowStatusPage();
		}

		public override void ExecuteEnterCommand()
		{
			Activate();
		}

		private void AlternativeActivationButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowAlternativeActivationOptionsPage();
		}

		private void PurchaseLicenseButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(base.LicenseManager.PurchaseLinkUrl))
			{
				WebLinkHelper.DisplayBrowser(base.LicenseManager.PurchaseLinkUrl);
			}
		}

		private void ActivateButton_Click(object sender, RoutedEventArgs e)
		{
			Activate();
		}

		private void Activate()
		{
			string activationCode = ActivationCode.Trim();
			try
			{
				base.LicenseManager.OfflineActivate(activationCode, ActivationCertificate);
				base.LicenseManagerView.ShowStatusPage();
				base.LicenseManagerView.OnLicenseStatusChanging();
			}
			catch (Exception ex)
			{
				MessageLog.DefaultLog.Error("OfflineActivationControl.Activate", ex);
				LicensingProviderException ex2 = ex as LicensingProviderException;
				if (ex2 != null && ex2.ErrorCode == 512)
				{
					Helpers.ShowError(StringResources.OfflineActivationControl_ActivationFailed_General);
				}
				else
				{
					Helpers.ShowException(StringResources.OfflineActivationControl_ActivationFailed, ex);
				}
				base.LicenseManager.ClearCachedLicense();
			}
		}

		private void ActivationCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			InstallationId = string.Empty;
			string activationCode = ActivationCode;
			if (Helpers.ValidateLicenseCode(ref activationCode))
			{
				InstallationId = base.LicenseManager.GetInstallationId(activationCode);
			}
			_canActivate = !string.IsNullOrEmpty(InstallationId);
		}

		private void ActivationCodePasteButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationCode = string.Empty;
			ActivationCode += Helpers.TrimPasteStringToMaxLength(ClipboardAccess.GetClipboardText(), ActivationCodeTextBox.MaxLength);
		}

		private void InstallationIDCopyButton_Click(object sender, RoutedEventArgs e)
		{
			ClipboardAccess.SetClipboardText(InstallationId);
		}

		private void ActivationCertificatePasteButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationCertificate = ClipboardAccess.GetClipboardText();
		}

		private void ActivationCertificateTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ActivateButton.IsEnabled = CanActivate;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/offlineactivationcontrol.xaml", UriKind.Relative);
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
				EscapeCommand = (KeyBinding)target;
				break;
			case 2:
				EnterCommand = (KeyBinding)target;
				break;
			case 3:
				CancelButton = (Button)target;
				CancelButton.Click += CancelButton_Click;
				break;
			case 4:
				HelpButton = (Button)target;
				HelpButton.Click += HelpButton_Click;
				break;
			case 5:
				ActivationCodeTextBox = (TextBox)target;
				ActivationCodeTextBox.TextChanged += ActivationCodeTextBox_TextChanged;
				break;
			case 6:
				InstallationIDTextBox = (TextBox)target;
				break;
			case 7:
				ActivationCertificateTextBox = (TextBox)target;
				ActivationCertificateTextBox.TextChanged += ActivationCertificateTextBox_TextChanged;
				break;
			case 8:
				AlternativeActivationButton = (Button)target;
				AlternativeActivationButton.Click += AlternativeActivationButton_Click;
				break;
			case 9:
				PurchaseLicenseButton = (Button)target;
				PurchaseLicenseButton.Click += PurchaseLicenseButton_Click;
				break;
			case 10:
				ActivateButton = (Button)target;
				ActivateButton.Click += ActivateButton_Click;
				break;
			case 11:
				ActivateImage = (Image)target;
				break;
			case 12:
				ActivationCodePasteButton = (Button)target;
				ActivationCodePasteButton.Click += ActivationCodePasteButton_Click;
				break;
			case 13:
				ActivationCodePasteImage = (Image)target;
				break;
			case 14:
				InstallationIDCopyButton = (Button)target;
				InstallationIDCopyButton.Click += InstallationIDCopyButton_Click;
				break;
			case 15:
				InstallationIDCopyImage = (Image)target;
				break;
			case 16:
				ActivationCertificatePasteButton = (Button)target;
				ActivationCertificatePasteButton.Click += ActivationCertificatePasteButton_Click;
				break;
			case 17:
				ActivationCertificatePasteImage = (Image)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
