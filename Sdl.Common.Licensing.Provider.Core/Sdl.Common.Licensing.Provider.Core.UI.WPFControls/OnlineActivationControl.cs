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
	internal class OnlineActivationControl : LicensePageControlWPF, IComponentConnector
	{
		private bool _canActivate = false;

		internal KeyBinding EscapeCommand;

		internal KeyBinding EnterCommand;

		internal TextBox ActivationCodeTextBox;

		internal Button PasteButton;

		internal Image PasteImage;

		internal Button ResetActivationCodeButton;

		internal Button AlternativeActivationButton;

		internal Button ActivateButton;

		internal Image ActivateImage;

		internal Button SDLAccoutButton;

		internal Button CancelButton;

		internal Button HelpButton;

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

		public OnlineActivationControl()
		{
			InitializeComponent();
		}

		public OnlineActivationControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.OnlineActivationControl_MainTitle;
			base.SubTitle = StringResources.OnlineActivationControl_SubTitle;
			PasteImage.Source = BitmapToImageSource(ImageResources.paste);
			ActivateImage.Source = BitmapToImageSource(ImageResources.key_activate_24x24);
			EscapeCommand.Command = new EscapeCommand(this);
			EnterCommand.Command = new EnterCommand(this);
		}

		public override void RefreshDisplay()
		{
			base.RefreshDisplay();
			ActivateButton.IsEnabled = false;
			ActivationCode = string.Empty;
			ActivationCodeTextBox.Focus();
		}

		private void Activate(string activationCode)
		{
			base.LicenseManagerView.PerformOnlineActivation(activationCode, null);
		}

		public override void ExecuteEscapeCommand()
		{
			base.LicenseManagerView.ShowStatusPage();
		}

		public override void ExecuteEnterCommand()
		{
			string activationCode = ActivationCode;
			if (_canActivate && ActivationCode.Length > 0 && Helpers.ValidateLicenseCode(ref activationCode))
			{
				Activate(activationCode);
			}
		}

		private static void DisplayInBrowser(string linkUrl)
		{
			if (!string.IsNullOrEmpty(linkUrl))
			{
				WebLinkHelper.DisplayBrowser(linkUrl);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			ExecuteEscapeCommand();
		}

		private void ActivateButton_Click(object sender, RoutedEventArgs e)
		{
			string activationCode = ActivationCode;
			if (!Helpers.ValidateLicenseCode(ref activationCode))
			{
				Helpers.ShowError(StringResources.DesktopLicensing_Error_InvalidActivationCode);
			}
			else
			{
				Activate(activationCode);
			}
		}

		private void AlternativeActivationButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowAlternativeActivationOptionsPage();
		}

		private void HelpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.OnlineActivation);
		}

		private void PasteButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationCode = string.Empty;
			ActivationCode += Helpers.TrimPasteStringToMaxLength(ClipboardAccess.GetClipboardText(), ActivationCodeTextBox.MaxLength);
		}

		private void SDLAccoutButton_Click(object sender, RoutedEventArgs e)
		{
			DisplayInBrowser(base.LicenseManager.MyAccountLinkUrl);
		}

		private void ActivationCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string activationCode = ActivationCode;
			_canActivate = Helpers.ValidateLicenseCode(ref activationCode);
			ActivateButton.IsEnabled = _canActivate;
		}

		private void ResetActivationCodeButton_Click(object sender, RoutedEventArgs e)
		{
			DisplayInBrowser(base.LicenseManager.ResetActivationLinkUrl);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/onlineactivationcontrol.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
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
				ActivationCodeTextBox = (TextBox)target;
				ActivationCodeTextBox.TextChanged += ActivationCodeTextBox_TextChanged;
				break;
			case 4:
				PasteButton = (Button)target;
				PasteButton.Click += PasteButton_Click;
				break;
			case 5:
				PasteImage = (Image)target;
				break;
			case 6:
				ResetActivationCodeButton = (Button)target;
				ResetActivationCodeButton.Click += ResetActivationCodeButton_Click;
				break;
			case 7:
				AlternativeActivationButton = (Button)target;
				AlternativeActivationButton.Click += AlternativeActivationButton_Click;
				break;
			case 8:
				ActivateButton = (Button)target;
				ActivateButton.Click += ActivateButton_Click;
				break;
			case 9:
				ActivateImage = (Image)target;
				break;
			case 10:
				SDLAccoutButton = (Button)target;
				SDLAccoutButton.Click += SDLAccoutButton_Click;
				break;
			case 11:
				CancelButton = (Button)target;
				CancelButton.Click += CancelButton_Click;
				break;
			case 12:
				HelpButton = (Button)target;
				HelpButton.Click += HelpButton_Click;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
