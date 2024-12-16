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
	internal class OnlineDeactivationControl : LicensePageControlWPF, IComponentConnector
	{
		private bool _canActivate = false;

		internal KeyBinding EscapeCommand;

		internal KeyBinding EnterCommand;

		internal TextBox ActivationCodeTextBox;

		internal Button PasteButton;

		internal Image PasteImage;

		internal Button DeactivateButton;

		internal Image DeactivateImage;

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

		public OnlineDeactivationControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.OnlineDeactivationControl_MainTitle;
			base.SubTitle = StringResources.OnlineDeactivationControl_SubTitle;
			PasteImage.Source = BitmapToImageSource(ImageResources.paste);
			DeactivateImage.Source = BitmapToImageSource(ImageResources.key_deactivate_24x24);
			EscapeCommand.Command = new EscapeCommand(this);
			EnterCommand.Command = new EnterCommand(this);
			EnterCommand.CommandParameter = _canActivate;
		}

		public override void RefreshDisplay()
		{
			base.RefreshDisplay();
			UpdateDeactivateButton();
			ActivationCode = base.LicenseManager.LicensingProvider.Configuration.Registry.LicenseCode;
		}

		private void UpdateDeactivateButton()
		{
			DeactivateButton.IsEnabled = _canActivate;
		}

		private void DeactivateButton_Click(object sender, RoutedEventArgs e)
		{
			string activationCode = ActivationCode;
			if (!Helpers.ValidateLicenseCode(ref activationCode))
			{
				Helpers.ShowError(StringResources.DesktopLicensing_Error_InvalidActivationCode);
			}
			else
			{
				Deactivate(activationCode);
			}
		}

		private void Deactivate(string activationCode)
		{
			base.LicenseManagerView.PerformOnlineDeactivation(activationCode, null);
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
			Deactivate(ActivationCode);
		}

		private void HelpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.OnlineDeactivation);
		}

		private void PasteButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationCode = string.Empty;
			ActivationCode += Helpers.TrimPasteStringToMaxLength(ClipboardAccess.GetClipboardText(), ActivationCodeTextBox.MaxLength);
		}

		private void ActivationCodeTextBox_TextChanged(object sender, RoutedEventArgs e)
		{
			string activationCode = ActivationCode;
			_canActivate = Helpers.ValidateLicenseCode(ref activationCode);
			UpdateDeactivateButton();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/onlinedeactivationcontrol.xaml", UriKind.Relative);
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
				DeactivateButton = (Button)target;
				DeactivateButton.Click += DeactivateButton_Click;
				break;
			case 7:
				DeactivateImage = (Image)target;
				break;
			case 8:
				CancelButton = (Button)target;
				CancelButton.Click += CancelButton_Click;
				break;
			case 9:
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
