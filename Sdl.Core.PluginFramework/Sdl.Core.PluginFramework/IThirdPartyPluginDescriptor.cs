using System;
using System.Collections.Generic;

namespace Sdl.Core.PluginFramework
{
	public interface IThirdPartyPluginDescriptor : IPluginDescriptor
	{
		string ThirdPartyManifestFilePath
		{
			get;
		}

		string Author
		{
			get;
		}

		string Description
		{
			get;
		}

		string PlugInName
		{
			get;
		}

		Version Version
		{
			get;
		}

		bool Validated
		{
			get;
		}

		[Obsolete("Use InvalidSdlAssemblyReferences to obtain list of invalid SDL API assembly references")]
		List<string> InvalidAssemblies
		{
			get;
		}

		List<InvalidSdlAssemblyReference> InvalidSdlAssemblyReferences
		{
			get;
		}
	}
}
