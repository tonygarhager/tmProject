using System;
using System.Windows.Input;

namespace Sdl.Common.Licensing.Provider.Core.UI.Commands
{
	public class DiagnosticPageCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			LicenseManagerView licenseManagerView = parameter as LicenseManagerView;
			return !licenseManagerView.IsDiagnosticPageDisabled;
		}

		public void Execute(object parameter)
		{
			LicenseManagerView licenseManagerView = parameter as LicenseManagerView;
			licenseManagerView.ShowDiagnosticPage();
		}
	}
}
