using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.ReverseAlignment;
using Sdl.Core.Processing.Processors.Storage;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	public class SdlRetrofitPackage
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

		public AlignmentToOriginalMapper ReverseMapper
		{
			get;
			set;
		}

		public string ReverseAlignmentMappingsFilePath
		{
			get;
			set;
		}

		public ParagraphUnitStore UpdatedTargetPUs
		{
			get;
			set;
		}

		public string UpdatedTargetBilingualFilePath
		{
			get;
			set;
		}
	}
}
