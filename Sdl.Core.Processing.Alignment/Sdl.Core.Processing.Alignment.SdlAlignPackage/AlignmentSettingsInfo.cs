using Sdl.Core.Processing.Alignment.Common;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	public class AlignmentSettingsInfo
	{
		public string Version
		{
			get;
			set;
		}

		public string LeftDocumentLanguage
		{
			get;
			set;
		}

		public string RightDocumentLanguage
		{
			get;
			set;
		}

		public AlignmentMode AlignmentMode
		{
			get;
			set;
		}

		public string TmPath
		{
			get;
			set;
		}

		public byte MinimumAlignmentQuality
		{
			get;
			set;
		}

		public string ProjectId
		{
			get;
			set;
		}
	}
}
