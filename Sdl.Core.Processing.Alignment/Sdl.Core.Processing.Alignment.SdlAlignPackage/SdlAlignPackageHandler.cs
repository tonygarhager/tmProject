using Sdl.Core.Processing.Alignment.Core;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	public class SdlAlignPackageHandler : AbstractSdlAlignPackageHandler
	{
		internal SdlAlignPackageHandler()
		{
			Alignments = new List<AlignmentData>();
		}

		internal SdlAlignPackageHandler(IFileTypeManager fileTypeManager, AlignmentSettingsInfo alignmentSettingsInfo = null, List<AlignmentData> alignments = null, IParagraphUnit paragraphUnit = null)
			: base(fileTypeManager, alignmentSettingsInfo, alignments, paragraphUnit)
		{
		}

		public SdlAlignPackage ReadPackage(string sdlAlignPath, bool deleteTempExtractionFolder = true)
		{
			string text = AlignmentHelper.CreateRandomFolderName();
			try
			{
				Directory.CreateDirectory(text);
				return ReadSdlAlignPackage(sdlAlignPath, text);
			}
			finally
			{
				if (deleteTempExtractionFolder && Directory.Exists(text))
				{
					Directory.Delete(text, recursive: true);
				}
			}
		}
	}
}
