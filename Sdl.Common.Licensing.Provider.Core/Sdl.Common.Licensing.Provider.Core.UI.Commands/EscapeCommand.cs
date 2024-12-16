using Sdl.Common.Licensing.Provider.Core.UI.WPFControls;
using System;
using System.Windows.Input;

namespace Sdl.Common.Licensing.Provider.Core.UI.Commands
{
	internal class EscapeCommand : ICommand
	{
		private LicensePageControlWPF _control;

		public event EventHandler CanExecuteChanged;

		public EscapeCommand(LicensePageControlWPF control)
		{
			_control = control;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_control.ExecuteEscapeCommand();
		}
	}
}
