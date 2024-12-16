using System;
using System.Drawing;

namespace Sdl.Desktop.IntegrationApi.Extensions.Internal
{
	public interface IAction
	{
		string Id
		{
			get;
		}

		string Text
		{
			get;
		}

		string ToolTipText
		{
			get;
		}

		Icon Icon
		{
			get;
		}

		bool Enabled
		{
			get;
		}

		bool Available
		{
			get;
		}

		ActionStyle Style
		{
			get;
		}

		DisplayType DisplayType
		{
			get;
		}

		bool Checked
		{
			get;
			set;
		}

		string ActionGroup
		{
			get;
		}

		event EventHandler<ActionPropertyChangedEventArgs> PropertyChanged;

		bool Execute();

		void Initialize();
	}
}
