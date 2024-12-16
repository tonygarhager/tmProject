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
	internal class ConnectionSettingsControl : LicensePageControlWPF, IComponentConnector
	{
		private Uri _proxyUri = null;

		internal KeyBinding EscapeCommand;

		internal Button CancelButton;

		internal Button HelpButton;

		internal CheckBox UseProxySettingsCheckBox;

		internal TextBlock ActiveTextBlock;

		internal TextBox AddressTextBox;

		internal TextBlock PortTextBlock;

		internal NumericUpDown PortNumericUpDown;

		internal Button RefreshSettingsButton;

		internal Button TestInternetConnection;

		internal Image TestConnectionImage;

		private bool _contentLoaded;

		public ConnectionSettingsControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.ConnectionSettingsControl_Title;
			base.SubTitle = StringResources.ConnectionSettingsControl_SubTitle;
			EnableProxyPanel(UseProxySettingsCheckBox.IsChecked.Value);
			if (!string.IsNullOrEmpty(base.LicenseManager.ProxyAddress))
			{
				UseProxySettingsCheckBox.IsChecked = true;
				AddressTextBox.Text = base.LicenseManager.ProxyAddress;
				PortNumericUpDown.Value = base.LicenseManager.ProxyPort;
			}
			TestConnectionImage.Source = BitmapToImageSource(ImageResources.earth_connection);
			EscapeCommand.Command = new EscapeCommand(this);
		}

		public override void ExecuteEscapeCommand()
		{
			base.LicenseManagerView.ShowStatusPage();
		}

		private void QueryProxyDetails()
		{
			try
			{
				Uri proxyDetails = NetworkConnectivity.GetProxyDetails();
				if (proxyDetails != null)
				{
					UseProxySettingsCheckBox.IsChecked = true;
					_proxyUri = proxyDetails;
					AddressTextBox.Text = proxyDetails.Host;
					PortNumericUpDown.Value = proxyDetails.Port;
				}
				else
				{
					UseProxySettingsCheckBox.IsChecked = false;
				}
			}
			catch (Exception ex)
			{
				Helpers.ShowException(StringResources.ConnectionSettingsControl_Error_ObtainingProxySettings, ex);
			}
		}

		private void EnableProxyPanel(bool enable)
		{
			TextBox addressTextBox = AddressTextBox;
			NumericUpDown portNumericUpDown = PortNumericUpDown;
			TextBlock activeTextBlock = ActiveTextBlock;
			TextBlock portTextBlock = PortTextBlock;
			bool flag2 = RefreshSettingsButton.IsEnabled = enable;
			bool flag4 = portTextBlock.IsEnabled = flag2;
			bool flag6 = activeTextBlock.IsEnabled = flag4;
			bool isEnabled = portNumericUpDown.IsEnabled = flag6;
			addressTextBox.IsEnabled = isEnabled;
		}

		private bool ValidatePortNumber(NumericUpDown numericUpDown)
		{
			bool flag = true;
			int.TryParse(numericUpDown.Value.ToString(), out int result);
			if (!numericUpDown.Value.HasValue)
			{
				flag = false;
			}
			else if (result < 1 || result > 65535)
			{
				flag = false;
			}
			if (!flag)
			{
				Helpers.ShowInformation(StringResources.ConnectionSettings_ProxyPortRange, StringResources.ConnectionSettings_ProxyPortTitle);
			}
			return flag;
		}

		private void UseProxySettingsCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			EnableProxyPanel(UseProxySettingsCheckBox.IsChecked.Value);
		}

		private void TestInternetConnection_Click(object sender, RoutedEventArgs e)
		{
			string text = AddressTextBox.Text.StartsWith("http://") ? AddressTextBox.Text : $"http://{AddressTextBox.Text}";
			if (!ValidatePortNumber(PortNumericUpDown))
			{
				PortNumericUpDown.SelectAll();
			}
			else
			{
				try
				{
					bool flag = false;
					TestInternetConnection.IsEnabled = false;
					if (!NetworkConnectivity.IsConnectedToInternet())
					{
						Helpers.ShowError(StringResources.ConnectionSettingsControl_Error_NoInternetConnection);
					}
					else if (!NetworkConnectivity.TryToConnectToWebServer(UseProxySettingsCheckBox.IsChecked.Value ? text : null, Convert.ToInt32(UseProxySettingsCheckBox.IsChecked.Value ? PortNumericUpDown.Value : new double?(0.0))))
					{
						Helpers.ShowError(StringResources.ConnectionSettingsControl_InternetConnectionTest_Failed);
					}
					else
					{
						Helpers.ShowInformation(StringResources.ConnectionSettingsControl_InternetConnectionTest_Successful, StringResources.ConnectionSettingsControl_InternetConnectionTest_Title);
						if (UseProxySettingsCheckBox.IsChecked.Value)
						{
							Uri uri = new Uri(text);
							base.LicenseManager.ProxyAddress = uri.Host;
							base.LicenseManager.ProxyPort = Convert.ToInt32(PortNumericUpDown.Value);
						}
						else
						{
							base.LicenseManager.ProxyAddress = null;
							base.LicenseManager.ProxyPort = 0;
						}
					}
				}
				catch (Exception ex)
				{
					Helpers.ShowException(StringResources.ConnectionSettingsControl_FailedToConnectToInternet, ex);
				}
				finally
				{
					TestInternetConnection.IsEnabled = true;
				}
			}
		}

		private void RefreshSettingsButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				RefreshSettingsButton.IsEnabled = false;
				QueryProxyDetails();
			}
			catch (Exception ex)
			{
				Helpers.ShowException(StringResources.ConnectionSettingsControl_Error_ObtainingProxySettings, ex);
			}
			finally
			{
				RefreshSettingsButton.IsEnabled = true;
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowAlternativeActivationOptionsPage();
		}

		private void HelpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.ConnectionSettings);
		}

		private void PortNumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			NumericUpDown numericUpDown = sender as NumericUpDown;
			ValidatePortNumber(numericUpDown);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/connectionsettingscontrol.xaml", UriKind.Relative);
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
				CancelButton = (Button)target;
				CancelButton.Click += CancelButton_Click;
				break;
			case 3:
				HelpButton = (Button)target;
				HelpButton.Click += HelpButton_Click;
				break;
			case 4:
				UseProxySettingsCheckBox = (CheckBox)target;
				UseProxySettingsCheckBox.Checked += UseProxySettingsCheckBox_Checked;
				UseProxySettingsCheckBox.Unchecked += UseProxySettingsCheckBox_Checked;
				break;
			case 5:
				ActiveTextBlock = (TextBlock)target;
				break;
			case 6:
				AddressTextBox = (TextBox)target;
				break;
			case 7:
				PortTextBlock = (TextBlock)target;
				break;
			case 8:
				PortNumericUpDown = (NumericUpDown)target;
				break;
			case 9:
				RefreshSettingsButton = (Button)target;
				RefreshSettingsButton.Click += RefreshSettingsButton_Click;
				break;
			case 10:
				TestInternetConnection = (Button)target;
				TestInternetConnection.Click += TestInternetConnection_Click;
				break;
			case 11:
				TestConnectionImage = (Image)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
