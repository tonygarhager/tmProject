using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public interface IFileTypeConfigurationAware
	{
		string FileTypeConfigurationId
		{
			get;
			set;
		}

		List<string> SubContentFileTypeConfigurationIds
		{
			get;
			set;
		}
	}
}
