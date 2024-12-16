using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.SdlAlignPackage;

namespace Sdl.Core.Processing.Alignment.Extensions
{
	internal static class AlignmentSettingsExtensions
	{
		internal static AlignmentSettings ConvertToAlignmentSettings(this AlignmentSettingsInfo alignmentSettingsInfo)
		{
			return new AlignmentSettings(alignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), alignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName())
			{
				AlignmentMode = alignmentSettingsInfo.AlignmentMode,
				MinimumAlignmentQuality = alignmentSettingsInfo.MinimumAlignmentQuality,
				TmPath = alignmentSettingsInfo.TmPath
			};
		}
	}
}
