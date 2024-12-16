using Sdl.Core.LanguageProcessing.Segmentation;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core.Resources;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	internal static class DefaultBuilder
	{
		private static IFileTypeManager _fileTypeManager;

		static DefaultBuilder()
		{
		}

		internal static Segmentor GetDefaultSegmentor(IResourceDataAccessor accessor)
		{
			Sdl.Core.LanguageProcessing.Segmentation.Settings settings = new Sdl.Core.LanguageProcessing.Segmentation.Settings
			{
				DontSegmentIfTargetExists = true,
				Mode = Mode.SentenceSegmentation,
				TargetSegmentCreationMode = TargetSegmentCreationMode.CreateEmptyTarget
			};
			return new Segmentor(settings, accessor);
		}

		public static IFileTypeManager GetDefaultFileTypeManager()
		{
			if (_fileTypeManager == null)
			{
				_fileTypeManager = new PocoFilterManager(autoLoadFileTypes: true);
			}
			_fileTypeManager.SettingsBundle = SettingsUtil.CreateSettingsBundle(null);
			return _fileTypeManager;
		}
	}
}
