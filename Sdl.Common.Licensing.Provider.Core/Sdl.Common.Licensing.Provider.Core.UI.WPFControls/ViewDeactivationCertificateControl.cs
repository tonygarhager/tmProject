using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	internal class ViewDeactivationCertificateControl : LicensePageControlWPF, IComponentConnector
	{
		internal Button OkButton;

		internal Button HelpButton;

		internal TextBox DeactivationCertificateTextBox;

		internal Button CopyButton;

		internal Image CopyImage;

		private bool _contentLoaded;

		private string DeactivationCertificate
		{
			get
			{
				return DeactivationCertificateTextBox.Text;
			}
			set
			{
				DeactivationCertificateTextBox.Text = value;
			}
		}

		public ViewDeactivationCertificateControl(LicenseManagerView licenseManagerView)
			: base(licenseManagerView)
		{
			InitializeComponent();
			base.MainTitle = StringResources.ViewDeactivationCertificateControl_MainTitle;
			base.SubTitle = StringResources.ViewDeactivationCertificateControl_SubTitle;
			CopyImage.Source = BitmapToImageSource(ImageResources.copy);
		}

		public override void RefreshDisplay()
		{
			base.RefreshDisplay();
			DeactivationCertificate = base.LicenseManager.LicensingProvider.Configuration.Registry.DeactivationCode;
		}

		private void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			ClipboardAccess.SetClipboardText(DeactivationCertificate);
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManagerView.ShowStatusPage();
		}

		private void HelpButton_Click(object sender, RoutedEventArgs e)
		{
			base.LicenseManager.ShowContextHelp(LicensingHelpIDs.ViewDeactivationCertificate);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Sdl.Common.Licensing.Provider.Core;component/ui/wpfcontrols/viewdeactivationcertificatecontrol.xaml", UriKind.Relative);
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
				OkButton = (Button)target;
				OkButton.Click += OkButton_Click;
				break;
			case 2:
				HelpButton = (Button)target;
				HelpButton.Click += HelpButton_Click;
				break;
			case 3:
				DeactivationCertificateTextBox = (TextBox)target;
				break;
			case 4:
				CopyButton = (Button)target;
				CopyButton.Click += CopyButton_Click;
				break;
			case 5:
				CopyImage = (Image)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
