using Sdl.Common.Licensing.Provider.Core.UI.WPFControls;
using System;
using System.Windows.Input;

namespace Sdl.Common.Licensing.Provider.Core.UI.Commands
{
	internal class EnterCommand : ICommand
	{
		private LicensePageControlWPF _control;

		public event EventHandler CanExecuteChanged;

		public EnterCommand(LicensePageControlWPF control)
		{
			_control = control;
		}

		public bool CanExecute(object parameter)
		{
			return !(parameter is bool) || (bool)parameter;
		}

		public void Execute(object parameter)
		{
			_control.ExecuteEnterCommand();
		}
	}
}
