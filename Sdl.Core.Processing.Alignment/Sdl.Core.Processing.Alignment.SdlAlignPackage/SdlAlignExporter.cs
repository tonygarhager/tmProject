using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	internal class SdlAlignExporter : AbstractBilingualContentProcessor
	{
		protected readonly AlignmentSettings AlignmentSettings;

		protected readonly List<AlignmentData> Alignments;

		protected readonly IFileTypeManager FileTypeManager;

		private readonly string _sdlAlignPackagePath;

		protected internal SdlAlignExporter(List<AlignmentData> alignments, string sdlAlignPackagePath, IFileTypeManager fileTypeManager)
		{
			if (alignments == null)
			{
				throw new ArgumentNullException("alignments");
			}
			if (string.IsNullOrEmpty(sdlAlignPackagePath))
			{
				throw new ArgumentNullException("sdlAlignPackagePath");
			}
			if (fileTypeManager == null)
			{
				throw new ArgumentNullException("fileTypeManager");
			}
			string directoryName = Path.GetDirectoryName(sdlAlignPackagePath);
			if (string.IsNullOrEmpty(sdlAlignPackagePath))
			{
				throw new ArgumentNullException("sdlAlignPackagePath");
			}
			if (directoryName != null && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			_sdlAlignPackagePath = sdlAlignPackagePath;
			Alignments = alignments;
			FileTypeManager = fileTypeManager;
		}

		public SdlAlignExporter(AlignmentSettings alignmentSettings, List<AlignmentData> alignments, string sdlAlignPackagePath, IFileTypeManager fileTypeManager)
			: this(alignments, sdlAlignPackagePath, fileTypeManager)
		{
			if (alignmentSettings == null)
			{
				throw new ArgumentNullException("alignmentSettings");
			}
			AlignmentSettings = alignmentSettings;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			ISdlAlignPackageHandler sdlAlignPackageHandler = GetSdlAlignPackageHandler(paragraphUnit);
			sdlAlignPackageHandler.Save(_sdlAlignPackagePath);
			base.ProcessParagraphUnit(paragraphUnit);
		}

		protected virtual ISdlAlignPackageHandler GetSdlAlignPackageHandler(IParagraphUnit paragraphUnit)
		{
			SdlAlignPackageHandler sdlAlignPackageHandler = new SdlAlignPackageHandler(FileTypeManager, new AlignmentSettingsInfo
			{
				LeftDocumentLanguage = AlignmentSettings.LeftCulture.Name,
				RightDocumentLanguage = AlignmentSettings.RightCulture.Name,
				AlignmentMode = AlignmentSettings.AlignmentMode,
				Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
				TmPath = AlignmentSettings.TmPath,
				MinimumAlignmentQuality = AlignmentSettings.MinimumAlignmentQuality
			}, Alignments, paragraphUnit);
			sdlAlignPackageHandler.SerializeAlignmentsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.SerializeAlignmentsProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlAlignPackageHandler.SerializeContentProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.SerializeContentProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlAlignPackageHandler.SerializeSettingsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.SerializeSettingsProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlAlignPackageHandler.CreateNewPackageProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.CreateNewPackageProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			return sdlAlignPackageHandler;
		}
	}
}
