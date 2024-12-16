using Sdl.Common.Licensing.Provider.Core.UI.Commands;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	internal class LicenseServerConfigControl : LicensePageControlWPF, IUIControl, IComponentConnector
	{
		internal KeyBinding EscapeCommand;

		internal KeyBinding ReturnCommand;

		internal Button _cancelButton;

		internal Button _helpButton;

		internal TextBlock label1;

		internal TextBox _serverNameTextBox;

		internal Button _buttonPasteCode;

		internal Image PasteImage;

		internal Button _alternativeActivationLink;

		internal Button _connectButton;

		internal Image ActivateImage;

		private bool _contentLoaded;

		public LicenseServerConfigControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.LicenseServerConfigControl_MainTitle;
			base.SubTitle = StringResources.LicenseServerConfigControl_SubTitle;
			PasteImage.Source = BitmapToImageSource(ImageResources.paste);
			ActivateImage.Source = BitmapToImageSource(ImageResources.server_ok_24x24);
			EscapeCommand.Command = new EscapeCommand(this);
			ReturnCommand.Command = new EnterCommand(this);
		}

		public override void RefreshDisplay()
		{
			base.RefreshDisplay();
			string licenseServer = base.LicenseManager.LicenseServer;
			if (OSVersion.isTerminalServicesInstalled())
			{
				Helpers.ShowInformation(StringResources.LicenseServerConfigControl_InvalidWithTerminalServices, null);
			}
			if (!base.LicenseManager.CanUpdateLicenseServerName && string.IsNullOrEmpty(licenseServer))
			{
				Helpers.ShowInformation(StringResources.LicenseServerConfigControl_AdminMessage, null);
			}
			else
			{
				_serverNameTextBox.Text = licenseServer;
			}
			_serverNameTextBox.IsEnabled = base.LicenseManager.CanUpdateLicenseServerName;
			_buttonPasteCode.IsEnabled = base.LicenseManager.CanUpdateLicenseServerName;
		}

		private void _cancelButton_Click(object sender, RoutedEventArgs e)
		{
			ExecuteEscapeCommand();
		}

		public override void ExecuteEscapeCommand()
		{
			base.LicenseManagerView.ShowStatusPage();
		}

		public override void ExecuteEnterCommand()
		{
			base.LicenseManagerView.IsProcessInProgress = true;
			base.LicenseManager.LicensingProvider.Configuration.Registry.CheckedOutEdition = null;
			_connectButton.IsEnabled = false;
			string trimmedServerName = _serverNameTextBox.Text.Trim();
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs eventArgs)
			{
				eventArgs.Result = ConnectToLicenseServer(trimmedServerName);
			};
			backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs eventArgs)
			{
				RunWorkerCompleted(eventArgs.Result, trimmedServerName);
			};
			backgroundWorker.RunWorkerAsync();
		}

		private void _connectButton_Click(object sender, RoutedEventArgs e)
		{
			ExecuteEnterCommand();
		}

		private List<string> ConnectToLicenseServer(string serverName)
		{
			if (!base.LicenseManagerView.ServerExists(serverName))
			{
				return null;
			}
			base.LicenseManager.LicenseServer = serverName;
			List<string> featuresAvailableOnServer = base.LicenseManager.GetFeaturesAvailableOnServer(serverName);
			return featuresAvailableOnServer.Intersect(base.LicenseManager.LicensingProvider.Configuration.AvailableEditions).ToList();
		}

		private void RunWorkerCompleted(object features, string trimmedServerName)
		{
			List<string> list = features as List<string>;
			if (list == null)
			{
				base.LicenseManagerView.IsProcessInProgress = false;
				_connectButton.IsEnabled = true;
				return;
			}
			if (list.Count() > 1)
			{
				base.LicenseManagerView.ShowLicenseServerEditionsControl();
				base.LicenseManagerView.IsProcessInProgress = false;
			}
			else
			{
				if (list.Count() == 1)
				{
					base.LicenseManager.LicensingProvider.Configuration.Registry.CheckedOutEdition = list.First();
				}
				base.LicenseManagerView.PerformNetworkActivation(trimmedServerName);
			}
			_connectButton.IsEnabled = true;
		}

		private void _serverNameTextBox_TextChanged(object sender, EventArgs e)
		{
			if (_serverNameTextBox.Text.Length > 0)
			{
				_connectButton.IsEnabled = true;
			}
			else
			{
				_connectButton.IsEnabled = false;
			}
		}

		private void _buttonPasteCode_Click(object sender, RoutedEventArgs e)
		{
			_serverNameTextBox.Text = ClipboardAccess.GetClipboardText();
		}

		private void _alternativeActivationLink_LinkClicked(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowAlternativeActivationOptionsPage();
		}

		private void _helpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.LicenseServerConfiguration);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/licenseserverconfigcontrol.xaml", UriKind.Relative);
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
				ReturnCommand = (KeyBinding)target;
				break;
			case 3:
				_cancelButton = (Button)target;
				_cancelButton.Click += _cancelButton_Click;
				break;
			case 4:
				_helpButton = (Button)target;
				_helpButton.Click += _helpButton_Click;
				break;
			case 5:
				label1 = (TextBlock)target;
				break;
			case 6:
				_serverNameTextBox = (TextBox)target;
				_serverNameTextBox.TextChanged += _serverNameTextBox_TextChanged;
				break;
			case 7:
				_buttonPasteCode = (Button)target;
				_buttonPasteCode.Click += _buttonPasteCode_Click;
				break;
			case 8:
				PasteImage = (Image)target;
				break;
			case 9:
				_alternativeActivationLink = (Button)target;
				_alternativeActivationLink.Click += _alternativeActivationLink_LinkClicked;
				break;
			case 10:
				_connectButton = (Button)target;
				_connectButton.Click += _connectButton_Click;
				break;
			case 11:
				ActivateImage = (Image)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
