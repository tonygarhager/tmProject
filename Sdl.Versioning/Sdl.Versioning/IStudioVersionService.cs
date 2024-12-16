using System;
using System.Collections.Generic;

namespace Sdl.Versioning
{
	public interface IStudioVersionService
	{
		List<StudioVersion> GetInstalledStudioVersions();

		StudioVersion GetStudioVersion();

		bool StudioVersionSuported(Version minVersion, Version maxVersion, Version studioVersion);
	}
}
