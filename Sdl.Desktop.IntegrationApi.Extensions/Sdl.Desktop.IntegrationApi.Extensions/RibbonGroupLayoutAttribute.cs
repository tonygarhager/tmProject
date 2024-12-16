using System;
using System.ComponentModel;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class RibbonGroupLayoutAttribute : AbstractLayoutAttribute
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RibbonGroupLayoutAttribute()
		{
		}

		public RibbonGroupLayoutAttribute(Type locationType)
			: base(locationType)
		{
		}
	}
}
