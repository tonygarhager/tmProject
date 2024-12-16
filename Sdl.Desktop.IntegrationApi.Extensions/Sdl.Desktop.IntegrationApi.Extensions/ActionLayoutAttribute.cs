using System;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ActionLayoutAttribute : AbstractLayoutAttribute
	{
		public ActionLayoutAttribute(Type locationByType, uint zIndex = 0u, DisplayType displayType = DisplayType.Default, string name = "", bool isSeparator = false)
		{
			base.LocationByType = locationByType;
			base.DisplayType = displayType;
			base.ZIndex = zIndex;
			base.Name = name;
			base.IsSeparator = isSeparator;
		}
	}
}
