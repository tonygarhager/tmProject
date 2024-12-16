using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	internal class DiagnosticPageControl : LicensePageControlWPF, IComponentConnector
	{
		internal Button CopyButton;

		internal Image CopyImage;

		internal TextBox ResultTextBox;

		internal Button RunButton;

		internal Image RunImage;

		internal Button BackButton;

		private bool _contentLoaded;

		public DiagnosticPageControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.Diagnostics_MainTitle;
			base.SubTitle = StringResources.Diagnostics_SubTitle;
			CopyImage.Source = BitmapToImageSource(ImageResources.copy);
			RunImage.Source = BitmapToImageSource(ImageResources.server_ok_24x24);
		}

		public void RunDiagnostic()
		{
			ResultTextBox.Text += string.Format(StringResources.LicensingDiagnostics_Running, DateTime.Now.ToString(), Environment.NewLine);
			IProductLicense productLicense = base.LicenseManagerView.LicenseManager.ProductLicense;
			ResultTextBox.Text += base.LicenseManager.LicensingProvider.GetDiagnosticInfo(productLicense);
		}

		private void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			ClipboardAccess.SetClipboardText(ResultTextBox.Text);
		}

		private void RunButton_Click(object sender, RoutedEventArgs e)
		{
			RunDiagnostic();
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowStatusPage();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/diagnosticpagecontrol.xaml", UriKind.Relative);
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
				CopyButton = (Button)target;
				CopyButton.Click += CopyButton_Click;
				break;
			case 2:
				CopyImage = (Image)target;
				break;
			case 3:
				ResultTextBox = (TextBox)target;
				break;
			case 4:
				RunButton = (Button)target;
				RunButton.Click += RunButton_Click;
				break;
			case 5:
				RunImage = (Image)target;
				break;
			case 6:
				BackButton = (Button)target;
				BackButton.Click += BackButton_Click;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
