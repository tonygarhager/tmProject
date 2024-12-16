using System;

namespace Sdl.Versioning
{
	public class StudioVersion
	{
		public string Version
		{
			get;
			set;
		}

		public string PublicVersion
		{
			get;
			set;
		}

		public string InstallPath
		{
			get;
			set;
		}

		public Version ExecutableVersion
		{
			get;
			set;
		}

		public string PluginSubPath => $"{ExecutableVersion.Major}{Edition}";

		public string Edition
		{
			get;
			set;
		}

		public string StudioDocumentsFolderName
		{
			get;
			set;
		}

		public string ShortVersion
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format($"{PublicVersion} {Edition} - {ExecutableVersion}");
		}
	}
}
