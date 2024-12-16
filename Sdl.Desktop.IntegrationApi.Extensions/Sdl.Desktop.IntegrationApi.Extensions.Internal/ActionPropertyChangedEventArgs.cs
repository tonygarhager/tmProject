using System;

namespace Sdl.Desktop.IntegrationApi.Extensions.Internal
{
	public sealed class ActionPropertyChangedEventArgs : EventArgs
	{
		public static ActionPropertyChangedEventArgs Text = new ActionPropertyChangedEventArgs(ActionProperties.Text);

		public static ActionPropertyChangedEventArgs ActionGroup = new ActionPropertyChangedEventArgs(ActionProperties.ActionGroup);

		public static ActionPropertyChangedEventArgs All = new ActionPropertyChangedEventArgs(ActionProperties.All);

		public static ActionPropertyChangedEventArgs Available = new ActionPropertyChangedEventArgs(ActionProperties.Available);

		public static ActionPropertyChangedEventArgs Checked = new ActionPropertyChangedEventArgs(ActionProperties.Checked);

		public static ActionPropertyChangedEventArgs Enabled = new ActionPropertyChangedEventArgs(ActionProperties.Enabled);

		public static ActionPropertyChangedEventArgs Icon = new ActionPropertyChangedEventArgs(ActionProperties.Icon);

		public static ActionPropertyChangedEventArgs Shortcuts = new ActionPropertyChangedEventArgs(ActionProperties.Shortcuts);

		public static ActionPropertyChangedEventArgs Style = new ActionPropertyChangedEventArgs(ActionProperties.Style);

		public static ActionPropertyChangedEventArgs ToolTipText = new ActionPropertyChangedEventArgs(ActionProperties.ToolTipText);

		public static ActionPropertyChangedEventArgs ShowText = new ActionPropertyChangedEventArgs(ActionProperties.ShowText);

		public static ActionPropertyChangedEventArgs PopupControl = new ActionPropertyChangedEventArgs(ActionProperties.PopupControl);

		public static ActionPropertyChangedEventArgs Size = new ActionPropertyChangedEventArgs(ActionProperties.Size);

		private ActionProperties _properties;

		public ActionProperties Properties => _properties;

		public ActionPropertyChangedEventArgs(ActionProperties properties)
		{
			_properties = properties;
		}

		public bool ContainsProperty(ActionProperties property)
		{
			return (_properties & property) != 0;
		}
	}
}
