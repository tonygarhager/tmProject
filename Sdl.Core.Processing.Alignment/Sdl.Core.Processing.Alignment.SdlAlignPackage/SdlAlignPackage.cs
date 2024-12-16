using Sdl.Core.Processing.Alignment.Core;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	public class SdlAlignPackage
	{
		public AlignmentSettingsInfo AlignmentSettingsInfo
		{
			get;
			set;
		}

		public List<AlignmentData> Alignments
		{
			get;
			set;
		}

		public IParagraphUnit ParagraphUnit
		{
			get;
			set;
		}

		public string AlignedBilingualFilePath
		{
			get;
			set;
		}
	}
}
