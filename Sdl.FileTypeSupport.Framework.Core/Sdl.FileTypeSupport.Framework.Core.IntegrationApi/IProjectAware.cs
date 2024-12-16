using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.IntegrationApi
{
	public interface IProjectAware
	{
		IDictionary<string, string> ProjectProperties
		{
			get;
			set;
		}
	}
}
