using System;

namespace Sdl.Desktop.IntegrationApi.Extensions
{
	public interface ILocationAware
	{
		Type LocationByType
		{
			get;
			set;
		}

		uint ZIndex
		{
			get;
			set;
		}

		bool IsSeparator
		{
			get;
			set;
		}
	}
}
