using Sdl.Core.PluginFramework;
using System;
using System.ComponentModel;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[ExtensionPointInfo("Actions", ExtensionPointBehavior.Static)]
	[AttributeUsage(AttributeTargets.Class)]
	public class ActionAttribute : AbstractCommandBarItemAttribute
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ActionAttribute()
		{
		}

		public ActionAttribute(string id, Type contextByType)
			: this(id, id + "_Name", id + "_Description", contextByType)
		{
		}

		public ActionAttribute(string id)
			: this(id, id + "_Name", id + "_Description", null)
		{
		}

		public ActionAttribute(string id, string name, string description, Type contextByType)
			: base(id, name, description)
		{
			base.ContextByType = contextByType;
		}

		public ActionAttribute(string id, string name, string description)
			: base(id, name, description)
		{
		}
	}
}
