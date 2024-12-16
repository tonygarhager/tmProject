using Sdl.Core.PluginFramework;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[ExtensionPointInfo("Views", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ViewAttribute : ExtensionAttribute, ILocationAware
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

		public Type LocationByType
		{
			get;
			set;
		}

		public uint ZIndex
		{
			get;
			set;
		}

		public bool IsSeparator
		{
			get;
			set;
		}

		public bool AllowViewParts
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public ViewAttribute()
		{
			Shortcut = Keys.None;
			AllowViewParts = false;
		}

		public ViewAttribute(string id, string name, string description, Type locationByType, string helpTopic, Keys shorcut = Keys.None, bool allowViewParts = false)
			: base(id, name, description)
		{
			LocationByType = locationByType;
			HelpTopic = helpTopic;
			Shortcut = shorcut;
			AllowViewParts = allowViewParts;
		}
	}
}
