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
	internal class AlternativeActivationOptionsControl : LicensePageControlWPF, IUIControl, IComponentConnector
	{
		internal KeyBinding EscapeCommand;

		internal Button BackButton;

		internal Button HelpButton;

		internal Button OfflineActivationButton;

		internal Button LicenseServerButton;

		internal Button ConnectionSettingsButton;

		private bool _contentLoaded;

		public AlternativeActivationOptionsControl()
		{
			InitializeComponent();
			EscapeCommand.Command = new EscapeCommand(this);
		}

		public AlternativeActivationOptionsControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.AlternativeActivationOptionsControl_MainTitle;
			base.SubTitle = StringResources.AlternativeActivationOptionsControl_SubTitle;
			LicenseServerButton.IsEnabled = base.LicenseManager.EnableServerLicensing;
			if (base.LicenseManager.EnableServerLicensing && OSVersion.isTerminalServicesInstalled())
			{
				OfflineActivationButton.IsEnabled = false;
				OfflineActivationButton.ToolTip = StringResources.LicenseServerConfigControl_InvalidWithTerminalServices;
				LicenseServerButton.Focus();
			}
		}

		private void OfflineActivationButton_Click(object sender, RoutedEventArgs e)
		{
			if (Helpers.MultipleInstancesRunning)
			{
				base.LicenseManagerView.ShowMultipleInstanceRunningInformationMessage();
			}
			else
			{
				base.LicenseManagerView.ShowOfflineActivationPage();
			}
		}

		private void ConnectionSettingsButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowConnectionSettingsControlPage();
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			ExecuteEscapeCommand();
		}

		public override void ExecuteEscapeCommand()
		{
			base.LicenseManagerView.ShowStatusPage();
		}

		private void HelpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.AlternativeActivationOptions);
		}

		private void LicenseServerButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowLicenseServerConfigControl();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/alternativeactivationoptionscontrol.xaml", UriKind.Relative);
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
				BackButton = (Button)target;
				BackButton.Click += BackButton_Click;
				break;
			case 3:
				HelpButton = (Button)target;
				HelpButton.Click += HelpButton_Click;
				break;
			case 4:
				OfflineActivationButton = (Button)target;
				OfflineActivationButton.Click += OfflineActivationButton_Click;
				break;
			case 5:
				LicenseServerButton = (Button)target;
				LicenseServerButton.Click += LicenseServerButton_Click;
				break;
			case 6:
				ConnectionSettingsButton = (Button)target;
				ConnectionSettingsButton.Click += ConnectionSettingsButton_Click;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
