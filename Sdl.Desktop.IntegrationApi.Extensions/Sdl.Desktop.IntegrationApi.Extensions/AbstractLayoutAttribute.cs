using Sdl.Core.PluginFramework;
using System;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	public abstract class AbstractLayoutAttribute : AuxiliaryExtensionAttribute, ILocationAware
	{
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

		[PluginResource]
		public string Name
		{
			get;
			set;
		}

		public DisplayType DisplayType
		{
			get;
			set;
		}

		protected AbstractLayoutAttribute()
		{
			Name = string.Empty;
		}

		protected AbstractLayoutAttribute(Type locationType)
		{
			Name = string.Empty;
			LocationByType = locationType;
		}
	}
}
