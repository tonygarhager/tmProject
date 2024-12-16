using Sdl.Core.PluginFramework;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[ExtensionPointInfo("IntegrationApi - View Parts", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ViewPartAttribute : ExtensionAttribute
	{
		public string HelpTopic
		{
			get;
			set;
		}

		public Keys Shortcut
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public ViewPartAttribute()
		{
			Shortcut = Keys.None;
		}

		public ViewPartAttribute(string id, string name, string description, string helpTopic, Keys shortcut = Keys.None)
			: base(id, name, description)
		{
			HelpTopic = helpTopic;
			Shortcut = shortcut;
		}

		public ViewPartAttribute(string id, string helpTopic, Keys shortcut = Keys.None)
			: this(id, id + "_Name", id + "_Description", helpTopic, shortcut)
		{
		}
	}
}
