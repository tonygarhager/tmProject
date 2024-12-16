using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	internal class SdlReverseAlignExporter : SdlAlignExporter
	{
		private readonly MergeParagraphParser _mergeParagraphParser;

		public ReverseAlignmentSettings ReverseAlignmentSettings
		{
			get;
			set;
		}

		public SdlReverseAlignExporter(ReverseAlignmentSettings alignmentSettings, List<AlignmentData> alignments, string sdlAlignPackagePath, IFileTypeManager fileTypeManager, MergeParagraphParser mergeParagraphParser)
			: base(alignments, sdlAlignPackagePath, fileTypeManager)
		{
			_mergeParagraphParser = mergeParagraphParser;
			ReverseAlignmentSettings = alignmentSettings;
		}

		protected override ISdlAlignPackageHandler GetSdlAlignPackageHandler(IParagraphUnit paragraphUnit)
		{
			SdlReverseAlignPackageHandler sdlReverseAlignPackageHandler = new SdlReverseAlignPackageHandler(FileTypeManager, new AlignmentSettingsInfo
			{
				LeftDocumentLanguage = ReverseAlignmentSettings.LeftCulture.Name,
				RightDocumentLanguage = ReverseAlignmentSettings.RightCulture.Name,
				AlignmentMode = AlignmentMode.IdenticalCultures,
				Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
				TmPath = "",
				ProjectId = ReverseAlignmentSettings.ProjectId,
				MinimumAlignmentQuality = ReverseAlignmentSettings.MinimumAlignmentQuality
			}, Alignments, paragraphUnit, _mergeParagraphParser.ReverseMapper, _mergeParagraphParser.OutputFile);
			sdlReverseAlignPackageHandler.SerializeMappingFilesPProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.SerializeMappingFilesPProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.SerializeAlignmentsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.SerializeAlignmentsProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.SerializeContentProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.SerializeContentProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.SerializeSettingsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.SerializeSettingsProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.CreateNewPackageProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.CreateNewPackageProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			return sdlReverseAlignPackageHandler;
		}
	}
}
