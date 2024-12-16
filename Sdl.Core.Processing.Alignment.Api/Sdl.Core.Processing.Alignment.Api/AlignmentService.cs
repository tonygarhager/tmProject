using Sdl.Core.LanguageProcessing.Segmentation;
using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Extensions;
using Sdl.Core.Processing.Alignment.ReverseAlignment;
using Sdl.Core.Processing.Alignment.SdlAlignPackage;
using Sdl.Core.Processing.Processors.Storage;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Api
{
	public class AlignmentService
	{
		private readonly IFileTypeManager _fileTypeManager;

		private readonly AlignmentApiHelper _alignmentApiHelper;

		public AlignmentService(IFileTypeManager fileTypeManager)
		{
			_fileTypeManager = fileTypeManager;
			_fileTypeManager.SettingsBundle = (_fileTypeManager.SettingsBundle ?? SettingsUtil.CreateSettingsBundle(null));
			_alignmentApiHelper = new AlignmentApiHelper();
		}

		public AlignmentResults Align(string leftNativeFilePath, string rightNativeFilePath, string outputFilePath, AlignmentFileFormat outputFileFormat, AlignmentSettings alignmentSettings, List<IBilingualContentProcessor> bilingualProcessors = null, EventHandler<AlignmentProgressEventArgs> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			if (bilingualProcessors == null)
			{
				bilingualProcessors = new List<IBilingualContentProcessor>();
			}
			DateTime now = DateTime.Now;
			AlignmentFilePair alignmentFilePair = new AlignmentFilePair(leftNativeFilePath, rightNativeFilePath, outputFilePath);
			ProgressReporter<AlignmentProgressEventArgs, AlignmentPhase> progressReporter = new ProgressReporter<AlignmentProgressEventArgs, AlignmentPhase>(this, alignmentFilePair)
			{
				Handler = progressHandler
			};
			progressReporter.Report(AlignmentPhase.Start, 0);
			using (ParagraphUnitStore leftDocument = _alignmentApiHelper.ReadDocument(_fileTypeManager, leftNativeFilePath, alignmentSettings.LeftCulture, alignmentSettings.ResourceDataAccessor, AlignmentFilePosition.Left, progressReporter, messageReporter))
			{
				using (ParagraphUnitStore rightDocument = _alignmentApiHelper.ReadDocument(_fileTypeManager, rightNativeFilePath, alignmentSettings.RightCulture, alignmentSettings.ResourceDataAccessor, AlignmentFilePosition.Right, progressReporter, messageReporter))
				{
					bool areParallel = _alignmentApiHelper.AreParallel(leftDocument, rightDocument);
					AlignmentAlgorithmSettings algorithmSettings = _alignmentApiHelper.CreateAlignmentAlgorithmSettings(alignmentSettings, areParallel);
					MergeParagraphParser mergeParagraphParser = new MergeParagraphParser(leftDocument, rightDocument, alignmentSettings.LeftCulture, alignmentSettings.RightCulture);
					if (progressHandler != null)
					{
						mergeParagraphParser.ProgressHandler = delegate(object sender, ProgressEventArgs args)
						{
							int progress2 = 40 + (int)((double)(int)args.ProgressValue / 100.0 * 10.0);
							progressReporter.Report(AlignmentPhase.MergingFiles, progress2);
						};
					}
					AlignmentProcessor alignmentProcessor = new AlignmentProcessor(algorithmSettings);
					AlignedProcessor processor = new AlignedProcessor(alignmentProcessor, alignmentSettings.MinimumAlignmentQuality);
					if (progressHandler != null)
					{
						alignmentProcessor.OnCurrentParagraphUnitProgress += delegate(object sender, ProgressEventArgs progressEventArgs)
						{
							int progress = 50 + (int)((double)(int)progressEventArgs.ProgressValue / 100.0 * 40.0);
							progressReporter.Report(AlignmentPhase.Aligning, progress);
						};
					}
					_alignmentApiHelper.CreateOutputFolderHiearchy(outputFilePath);
					IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(mergeParagraphParser, outputFilePath);
					converterToDefaultBilingual.AddBilingualProcessor(alignmentProcessor);
					if (outputFileFormat == AlignmentFileFormat.SdlAlign)
					{
						SdlAlignExporter processor2 = new SdlAlignExporter(alignmentSettings, alignmentProcessor.Alignments, outputFilePath, _fileTypeManager);
						converterToDefaultBilingual.AddBilingualProcessor(processor2);
						_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
					}
					converterToDefaultBilingual.AddBilingualProcessor(processor);
					if (outputFileFormat == AlignmentFileFormat.Tmx)
					{
						converterToDefaultBilingual.AddBilingualProcessor(new TMXEmitter(outputFilePath));
						_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
					}
					if (outputFileFormat == AlignmentFileFormat.None)
					{
						_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
					}
					foreach (IBilingualContentProcessor bilingualProcessor in bilingualProcessors)
					{
						converterToDefaultBilingual.AddBilingualProcessor(bilingualProcessor);
					}
					converterToDefaultBilingual.Parse();
					AlignmentStatistics alignmentStatistics = _alignmentApiHelper.CalculateStatistics(alignmentProcessor.Alignments);
					progressReporter.Report(AlignmentPhase.PostAlignmentProcessing, 100);
					return new AlignmentResults(alignmentFilePair, alignmentStatistics, DateTime.Now - now);
				}
			}
		}

		public AlignmentResults ReverseAlign(string leftxliffFilePath, string rightNativeFilePath, string outputFilePath, AlignmentFileFormat outputFileFormat, ReverseAlignmentSettings alignmentSettings, List<IBilingualContentProcessor> bilingualProcessors = null, EventHandler<ReverseAlignmentProgressEventArgs> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			if (bilingualProcessors == null)
			{
				bilingualProcessors = new List<IBilingualContentProcessor>();
			}
			if (alignmentSettings.MinimumAlignmentQuality == 0)
			{
				alignmentSettings.MinimumAlignmentQuality = 70;
			}
			DateTime now = DateTime.Now;
			AlignmentFilePair alignmentFilePair = new AlignmentFilePair(leftxliffFilePath, rightNativeFilePath, outputFilePath);
			ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase> progressReporter = new ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase>(this, alignmentFilePair)
			{
				Handler = progressHandler
			};
			progressReporter.Report(ReverseAlignmentPhase.Start, 0);
			Tuple<IFileTypeManager, CultureInfo, IResourceDataAccessor, ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase>, EventHandler<MessageEventArgs>> fileProcessingInfo = FileProcessingDetails(alignmentSettings, alignmentSettings.LeftCulture, progressReporter, messageReporter);
			Tuple<IFileTypeManager, CultureInfo, IResourceDataAccessor, ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase>, EventHandler<MessageEventArgs>> fileProcessingInfo2 = FileProcessingDetails(alignmentSettings, alignmentSettings.RightCulture, progressReporter, messageReporter);
			using (ParagraphUnitStore leftDocument = _alignmentApiHelper.ReadLeftDocument(fileProcessingInfo, leftxliffFilePath))
			{
				string outputFile;
				string fileId;
				using (ParagraphUnitStore rightDocument = _alignmentApiHelper.ReadRightDocument(fileProcessingInfo2, rightNativeFilePath, out outputFile, out fileId))
				{
					AlignmentAlgorithmSettings algorithmSettings = _alignmentApiHelper.CreateReverseAlignmentAlgorithmSettings(alignmentSettings);
					MergeParagraphParser mergeParagraphParser = new MergeParagraphParser(leftDocument, rightDocument, alignmentSettings.RightCulture, alignmentSettings.RightCulture, outputFile, fileId);
					if (progressHandler != null)
					{
						mergeParagraphParser.ProgressHandler = delegate(object sender, ProgressEventArgs args)
						{
							int progress2 = 40 + (int)((double)(int)args.ProgressValue / 100.0 * 10.0);
							progressReporter.Report(ReverseAlignmentPhase.MergingTargets, progress2);
						};
					}
					RetrofitAlignmentProcessor retrofitAlignmentProcessor = new RetrofitAlignmentProcessor(algorithmSettings, mergeParagraphParser, alignmentSettings.MinimumAlignmentQuality);
					AlignedProcessor processor = new AlignedProcessor(retrofitAlignmentProcessor, alignmentSettings.MinimumAlignmentQuality);
					if (progressHandler != null)
					{
						retrofitAlignmentProcessor.OnCurrentParagraphUnitProgress += delegate(object sender, ProgressEventArgs progressEventArgs)
						{
							int progress = 50 + (int)((double)(int)progressEventArgs.ProgressValue / 100.0 * 40.0);
							progressReporter.Report(ReverseAlignmentPhase.ReverseAligning, progress);
						};
					}
					_alignmentApiHelper.CreateOutputFolderHiearchy(outputFilePath);
					IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(mergeParagraphParser, outputFilePath);
					converterToDefaultBilingual.AddBilingualProcessor(retrofitAlignmentProcessor);
					if (outputFileFormat == AlignmentFileFormat.SdlAlign)
					{
						SdlReverseAlignExporter processor2 = new SdlReverseAlignExporter(alignmentSettings, retrofitAlignmentProcessor.Alignments, outputFilePath, _fileTypeManager, mergeParagraphParser);
						converterToDefaultBilingual.AddBilingualProcessor(processor2);
						_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
					}
					converterToDefaultBilingual.AddBilingualProcessor(processor);
					if (outputFileFormat == AlignmentFileFormat.Tmx)
					{
						converterToDefaultBilingual.AddBilingualProcessor(new TMXEmitter(outputFilePath));
						_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
					}
					if (outputFileFormat == AlignmentFileFormat.None)
					{
						_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
					}
					foreach (IBilingualContentProcessor bilingualProcessor in bilingualProcessors)
					{
						converterToDefaultBilingual.AddBilingualProcessor(bilingualProcessor);
					}
					converterToDefaultBilingual.Parse();
					AlignmentStatistics alignmentStatistics = _alignmentApiHelper.CalculateStatistics(retrofitAlignmentProcessor.Alignments);
					progressReporter.Report(ReverseAlignmentPhase.PostReverseAlignmentProcessing, 100);
					return new AlignmentResults(alignmentFilePair, alignmentStatistics, DateTime.Now - now);
				}
			}
		}

		private Tuple<IFileTypeManager, CultureInfo, IResourceDataAccessor, ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase>, EventHandler<MessageEventArgs>> FileProcessingDetails(ReverseAlignmentSettings alignmentSettings, CultureInfo culture, ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase> progressHandler, EventHandler<MessageEventArgs> messageReporter)
		{
			return new Tuple<IFileTypeManager, CultureInfo, IResourceDataAccessor, ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase>, EventHandler<MessageEventArgs>>(_fileTypeManager, culture, alignmentSettings.ResourceDataAccessor, progressHandler, messageReporter);
		}

		public IList<AlignmentResults> Align(IList<AlignmentFilePair> filePairs, AlignmentFileFormat outputFileFormat, AlignmentSettings alignmentSettings, EventHandler<AlignmentProgressEventArgs> alignmentProgressHandler, EventHandler<MessageEventArgs> messageReporter = null)
		{
			List<AlignmentResults> list = new List<AlignmentResults>();
			foreach (AlignmentFilePair filePair in filePairs)
			{
				try
				{
					AlignmentResults item = Align(filePair.LeftInputFilename, filePair.RightInputFilename, filePair.OutputFilename, outputFileFormat, alignmentSettings, null, alignmentProgressHandler, messageReporter);
					list.Add(item);
				}
				catch (Exception lastError)
				{
					AlignmentResults item2 = new AlignmentResults(filePair, lastError);
					list.Add(item2);
				}
			}
			return list;
		}

		public Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage ImportSdlAlignPackage(string sdlAlignPackagePath, List<IBilingualContentProcessor> bilingualProcessors = null, bool deleteTempExtractionFolder = true, EventHandler<SdlAlignPackageProgressEventArgs<ImportSdlAlignPhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			ProgressReporter<SdlAlignPackageProgressEventArgs<ImportSdlAlignPhase>, ImportSdlAlignPhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<ImportSdlAlignPhase>, ImportSdlAlignPhase>(this, sdlAlignPackagePath)
			{
				Handler = progressHandler
			};
			progressReporter.Report(ImportSdlAlignPhase.Start, 0);
			Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage sdlAlignPackage = null;
			try
			{
				SdlAlignPackageHandler sdlAlignPackageHandler = new SdlAlignPackageHandler(_fileTypeManager);
				sdlAlignPackageHandler.ExtractFilesProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.ExtractFilesProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlAlignPhase.ExtractFiles, 20);
				});
				sdlAlignPackageHandler.ReadSettingsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.ReadSettingsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlAlignPhase.ReadSettings, 25);
				});
				sdlAlignPackageHandler.ReadAlignmentsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.ReadAlignmentsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlAlignPhase.ReadAlignments, 30);
				});
				sdlAlignPackageHandler.ReadContentProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.ReadContentProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlAlignPhase.ReadContents, 50);
				});
				sdlAlignPackage = sdlAlignPackageHandler.ReadPackage(sdlAlignPackagePath, deleteTempExtractionFolder);
				progressReporter.Report(ImportSdlAlignPhase.ProcessData, 75);
				if (bilingualProcessors != null)
				{
					ParagraphUnitParser paragraphUnitParser = new ParagraphUnitParser(sdlAlignPackage.AlignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), sdlAlignPackage.AlignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName());
					paragraphUnitParser.AddParagraphUnit(sdlAlignPackage.ParagraphUnit);
					IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(paragraphUnitParser, sdlAlignPackagePath);
					AlignmentSettings alignmentSettings = new AlignmentSettings(sdlAlignPackage.AlignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), sdlAlignPackage.AlignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName())
					{
						MinimumAlignmentQuality = sdlAlignPackage.AlignmentSettingsInfo.MinimumAlignmentQuality
					};
					AlignedProcessor processor = new AlignedProcessor(sdlAlignPackage.Alignments, alignmentSettings.MinimumAlignmentQuality);
					converterToDefaultBilingual.AddBilingualProcessor(processor);
					foreach (IBilingualContentProcessor bilingualProcessor in bilingualProcessors)
					{
						converterToDefaultBilingual.AddBilingualProcessor(bilingualProcessor);
					}
					converterToDefaultBilingual.BilingualDocumentGenerator = null;
					converterToDefaultBilingual.Parse();
				}
			}
			catch (UserCancelledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				messageReporter?.Invoke(this, new MessageEventArgs(sdlAlignPackagePath, "Alignment.Api.ImportSdlAlignPackage", ErrorLevel.Error, ex2.Message, null, null));
			}
			progressReporter.Report(ImportSdlAlignPhase.Finish, 100);
			return sdlAlignPackage;
		}

		public SdlRetrofitPackage ImportSdlRetrofitPackage(string sdlAlignPackagePath, List<IBilingualContentProcessor> bilingualProcessors = null, bool deleteTempExtractionFolder = true, EventHandler<SdlAlignPackageProgressEventArgs<ImportSdlRetrofitPhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			ProgressReporter<SdlAlignPackageProgressEventArgs<ImportSdlRetrofitPhase>, ImportSdlRetrofitPhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<ImportSdlRetrofitPhase>, ImportSdlRetrofitPhase>(this, sdlAlignPackagePath)
			{
				Handler = progressHandler
			};
			progressReporter.Report(ImportSdlRetrofitPhase.Start, 0);
			SdlRetrofitPackage sdlRetrofitPackage = null;
			try
			{
				SdlReverseAlignPackageHandler sdlReverseAlignPackageHandler = new SdlReverseAlignPackageHandler(_fileTypeManager);
				sdlReverseAlignPackageHandler.ExtractFilesProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ExtractFilesProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlRetrofitPhase.ExtractFiles, 20);
				});
				sdlReverseAlignPackageHandler.ReadSettingsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadSettingsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlRetrofitPhase.ReadSettings, 25);
				});
				sdlReverseAlignPackageHandler.ReadAlignmentsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadAlignmentsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlRetrofitPhase.ReadAlignments, 30);
				});
				sdlReverseAlignPackageHandler.ReadContentProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadContentProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlRetrofitPhase.ReadContents, 45);
				});
				sdlReverseAlignPackageHandler.ReadMappingFilesProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadMappingFilesProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlRetrofitPhase.ReadMapping, 70);
				});
				sdlReverseAlignPackageHandler.ReadUpdatedTargetFileProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadUpdatedTargetFileProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(ImportSdlRetrofitPhase.ReadTarget, 85);
				});
				sdlRetrofitPackage = sdlReverseAlignPackageHandler.ReadPackage(sdlAlignPackagePath, deleteTempExtractionFolder);
				progressReporter.Report(ImportSdlRetrofitPhase.ProcessData, 55);
				if (bilingualProcessors != null)
				{
					ParagraphUnitParser paragraphUnitParser = new ParagraphUnitParser(sdlRetrofitPackage.AlignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), sdlRetrofitPackage.AlignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName());
					paragraphUnitParser.AddParagraphUnit(sdlRetrofitPackage.ParagraphUnit);
					IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(paragraphUnitParser, sdlAlignPackagePath);
					AlignedProcessor processor = new AlignedProcessor(sdlRetrofitPackage.Alignments, sdlRetrofitPackage.AlignmentSettingsInfo.MinimumAlignmentQuality);
					converterToDefaultBilingual.AddBilingualProcessor(processor);
					foreach (IBilingualContentProcessor bilingualProcessor in bilingualProcessors)
					{
						converterToDefaultBilingual.AddBilingualProcessor(bilingualProcessor);
					}
					converterToDefaultBilingual.BilingualDocumentGenerator = null;
					converterToDefaultBilingual.Parse();
				}
			}
			catch (UserCancelledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				messageReporter?.Invoke(this, new MessageEventArgs(sdlAlignPackagePath, "Alignment.Api.ImportSdlRetrofitPackage", ErrorLevel.Error, ex2.Message, null, null));
			}
			progressReporter.Report(ImportSdlRetrofitPhase.Finish, 100);
			return sdlRetrofitPackage;
		}

		public bool UpdateSdlAlignPackage(Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage existingPackage, string outputSdlAlignPath, ContentSource contentSource = ContentSource.SdlXliffFile, EventHandler<SdlAlignPackageProgressEventArgs<UpdateSdlAlignPhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlAlignPhase>, UpdateSdlAlignPhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlAlignPhase>, UpdateSdlAlignPhase>(this, outputSdlAlignPath);
			ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlAlignPhase>, UpdateSdlAlignPhase> progressReporter2 = progressReporter;
			progressReporter2.Handler = (EventHandler<SdlAlignPackageProgressEventArgs<UpdateSdlAlignPhase>>)Delegate.Combine(progressReporter2.Handler, progressHandler);
			progressReporter.Report(UpdateSdlAlignPhase.Start, 0);
			try
			{
				SdlAlignPackageHandler sdlAlignPackageHandler = new SdlAlignPackageHandler(_fileTypeManager);
				sdlAlignPackageHandler.SerializeAlignmentsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.SerializeAlignmentsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(UpdateSdlAlignPhase.SerializeAlignments, 30);
				});
				sdlAlignPackageHandler.SerializeSettingsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.SerializeSettingsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(UpdateSdlAlignPhase.SerializeSettings, 50);
				});
				sdlAlignPackageHandler.CreateNewPackageProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.CreateNewPackageProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(UpdateSdlAlignPhase.CreateNewPackage, 80);
				});
				sdlAlignPackageHandler.SerializeContentProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlAlignPackageHandler.SerializeContentProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
				});
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (contentSource == ContentSource.ParagraphUnit)
				{
					existingPackage.AlignedBilingualFilePath = string.Empty;
				}
				sdlAlignPackageHandler.WriteSdlAlignPackage(existingPackage, outputSdlAlignPath);
				if (empty2 != string.Empty)
				{
					File.Delete(empty2);
				}
				if (empty != string.Empty)
				{
					Directory.Delete(empty);
				}
			}
			catch (UserCancelledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				messageReporter?.Invoke(this, new MessageEventArgs(outputSdlAlignPath, "UpdateSdlAlignPackage", ErrorLevel.Error, ex2.Message, null, null));
			}
			progressReporter.Report(UpdateSdlAlignPhase.Finish, 100);
			return true;
		}

		public bool UpdateSdlRetrofitPackage(SdlRetrofitPackage existingPackage, string outputSdlAlignPath, ContentSource contentSource = ContentSource.SdlXliffFile, EventHandler<SdlAlignPackageProgressEventArgs<UpdateSdlRetrofitPackagePhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlRetrofitPackagePhase>, UpdateSdlRetrofitPackagePhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlRetrofitPackagePhase>, UpdateSdlRetrofitPackagePhase>(this, outputSdlAlignPath);
			ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlRetrofitPackagePhase>, UpdateSdlRetrofitPackagePhase> progressReporter2 = progressReporter;
			progressReporter2.Handler = (EventHandler<SdlAlignPackageProgressEventArgs<UpdateSdlRetrofitPackagePhase>>)Delegate.Combine(progressReporter2.Handler, progressHandler);
			progressReporter.Report(UpdateSdlRetrofitPackagePhase.Start, 0);
			try
			{
				SdlReverseAlignPackageHandler sdlReverseAlignPackageHandler = new SdlReverseAlignPackageHandler();
				sdlReverseAlignPackageHandler.SerializeAlignmentsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.SerializeAlignmentsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(UpdateSdlRetrofitPackagePhase.SerializeAlignments, 30);
				});
				sdlReverseAlignPackageHandler.SerializeSettingsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.SerializeSettingsProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(UpdateSdlRetrofitPackagePhase.SerializeSettings, 50);
				});
				sdlReverseAlignPackageHandler.CreateNewPackageProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.CreateNewPackageProgress, (EventHandler<BatchProgressEventArgs>)delegate
				{
					progressReporter.Report(UpdateSdlRetrofitPackagePhase.CreateNewPackage, 80);
				});
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (contentSource == ContentSource.ParagraphUnit)
				{
					existingPackage.AlignedBilingualFilePath = string.Empty;
				}
				sdlReverseAlignPackageHandler.WritePackage(existingPackage, outputSdlAlignPath);
				if (empty2 != string.Empty)
				{
					File.Delete(empty2);
				}
				if (empty != string.Empty)
				{
					Directory.Delete(empty);
				}
			}
			catch (UserCancelledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				messageReporter?.Invoke(this, new MessageEventArgs(outputSdlAlignPath, "UpdateSdlRetrofitPackage", ErrorLevel.Error, ex2.Message, null, null));
			}
			progressReporter.Report(UpdateSdlRetrofitPackagePhase.Finish, 100);
			return true;
		}

		public Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage RealignSdlAlignPackage(Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage existingPackage, EventHandler<SdlAlignPackageProgressEventArgs<RealignSdlAlignPhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			ProgressReporter<SdlAlignPackageProgressEventArgs<RealignSdlAlignPhase>, RealignSdlAlignPhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<RealignSdlAlignPhase>, RealignSdlAlignPhase>(this);
			ProgressReporter<SdlAlignPackageProgressEventArgs<RealignSdlAlignPhase>, RealignSdlAlignPhase> progressReporter2 = progressReporter;
			progressReporter2.Handler = (EventHandler<SdlAlignPackageProgressEventArgs<RealignSdlAlignPhase>>)Delegate.Combine(progressReporter2.Handler, progressHandler);
			progressReporter.Report(RealignSdlAlignPhase.Start, 0);
			Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage sdlAlignPackage = null;
			try
			{
				ParagraphUnitParser paragraphUnitParser = new ParagraphUnitParser(existingPackage.AlignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), existingPackage.AlignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName());
				paragraphUnitParser.AddParagraphUnit(existingPackage.ParagraphUnit);
				AlignmentAlgorithmSettings algorithmSettings = _alignmentApiHelper.CreateAlignmentAlgorithmSettings(existingPackage.AlignmentSettingsInfo.ConvertToAlignmentSettings());
				AlignmentProcessor alignmentProcessor = new AlignmentProcessor(algorithmSettings)
				{
					ConfirmedAlignments = existingPackage.Alignments.Where((AlignmentData x) => x.Confirmed).ToList()
				};
				if (progressHandler != null)
				{
					alignmentProcessor.OnCurrentParagraphUnitProgress += delegate(object sender, ProgressEventArgs progressEventArgs)
					{
						int progress = (int)((double)(int)progressEventArgs.ProgressValue / 100.0 * 80.0);
						progressReporter.Report(RealignSdlAlignPhase.Realign, progress);
					};
				}
				IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(paragraphUnitParser, string.Empty);
				converterToDefaultBilingual.AddBilingualProcessor(alignmentProcessor);
				_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
				converterToDefaultBilingual.Parse();
				progressReporter.Report(RealignSdlAlignPhase.Finish, 100);
				sdlAlignPackage = existingPackage;
				sdlAlignPackage.Alignments = alignmentProcessor.Alignments;
				return sdlAlignPackage;
			}
			catch (UserCancelledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (messageReporter == null)
				{
					return sdlAlignPackage;
				}
				messageReporter(this, new MessageEventArgs(string.Empty, "RealignSdlAlignPackage", ErrorLevel.Error, ex2.Message, null, null));
				return sdlAlignPackage;
			}
		}

		public bool ProcessSdlAlignPackage(Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage existingPackage, ContentSource contentSource = ContentSource.SdlXliffFile, List<IBilingualContentProcessor> bilingualProcessors = null, EventHandler<SdlAlignPackageProgressEventArgs<ProcessSdlAlignPhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			if (bilingualProcessors == null || bilingualProcessors.Count == 0)
			{
				return false;
			}
			ProgressReporter<SdlAlignPackageProgressEventArgs<ProcessSdlAlignPhase>, ProcessSdlAlignPhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<ProcessSdlAlignPhase>, ProcessSdlAlignPhase>(this);
			progressReporter.Handler = (EventHandler<SdlAlignPackageProgressEventArgs<ProcessSdlAlignPhase>>)Delegate.Combine(progressReporter.Handler, progressHandler);
			progressReporter.Report(ProcessSdlAlignPhase.Start, 0);
			try
			{
				ParagraphUnitParser paragraphUnitParser = new ParagraphUnitParser(existingPackage.AlignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), existingPackage.AlignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName());
				IParagraphUnit paragraphUnit = (contentSource != ContentSource.ParagraphUnit) ? _alignmentApiHelper.GetParagraphUnitFromMergedSdlXliff(_fileTypeManager, existingPackage.AlignedBilingualFilePath) : existingPackage.ParagraphUnit;
				paragraphUnitParser.AddParagraphUnit(paragraphUnit);
				AlignedProcessor processor = new AlignedProcessor(existingPackage.Alignments, existingPackage.AlignmentSettingsInfo.ConvertToAlignmentSettings().MinimumAlignmentQuality);
				IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(paragraphUnitParser, string.Empty);
				converterToDefaultBilingual.AddBilingualProcessor(processor);
				foreach (IBilingualContentProcessor bilingualProcessor in bilingualProcessors)
				{
					converterToDefaultBilingual.AddBilingualProcessor(bilingualProcessor);
				}
				_alignmentApiHelper.RemoveSdlxliffWriter(converterToDefaultBilingual);
				progressReporter.Report(ProcessSdlAlignPhase.ProcessingPackage, 50);
				converterToDefaultBilingual.Parse();
				progressReporter.Report(ProcessSdlAlignPhase.Finish, 100);
			}
			catch (UserCancelledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				messageReporter?.Invoke(this, new MessageEventArgs(string.Empty, "ProcessSdlAlignPackage", ErrorLevel.Error, ex2.Message, null, null));
			}
			return true;
		}

		public bool GenerateAlignedSdlXliff(Sdl.Core.Processing.Alignment.SdlAlignPackage.SdlAlignPackage existingPackage, string sdlxliffPath, EventHandler<SdlAlignPackageProgressEventArgs<GenerateAlignedSdlxliffPhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null)
		{
			ProgressReporter<SdlAlignPackageProgressEventArgs<GenerateAlignedSdlxliffPhase>, GenerateAlignedSdlxliffPhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<GenerateAlignedSdlxliffPhase>, GenerateAlignedSdlxliffPhase>(this);
			progressReporter.Handler = (EventHandler<SdlAlignPackageProgressEventArgs<GenerateAlignedSdlxliffPhase>>)Delegate.Combine(progressReporter.Handler, progressHandler);
			progressReporter.Report(GenerateAlignedSdlxliffPhase.Start, 0);
			try
			{
				ParagraphUnitParser paragraphUnitParser = new ParagraphUnitParser(existingPackage.AlignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), existingPackage.AlignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName());
				paragraphUnitParser.AddParagraphUnit(existingPackage.ParagraphUnit);
				AlignedProcessor processor = new AlignedProcessor(existingPackage.Alignments, existingPackage.AlignmentSettingsInfo.ConvertToAlignmentSettings().MinimumAlignmentQuality);
				progressReporter.Report(GenerateAlignedSdlxliffPhase.ProcessingContent, 40);
				IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(paragraphUnitParser, sdlxliffPath);
				progressReporter.Report(GenerateAlignedSdlxliffPhase.Aligning, 70);
				converterToDefaultBilingual.AddBilingualProcessor(processor);
				converterToDefaultBilingual.Parse();
				progressReporter.Report(GenerateAlignedSdlxliffPhase.Finish, 100);
			}
			catch (UserCancelledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				messageReporter?.Invoke(this, new MessageEventArgs(string.Empty, "GenerateAlignedSdlXliff", ErrorLevel.Error, ex2.Message, null, null));
			}
			return true;
		}

		public ReverseAlignmentUpdateResults UpdateXliffWithReversedAlignedPackage(string xliffFilePath, string packagePath, CultureInfo culture, IResourceDataAccessor resourceDataAccessor, List<IBilingualContentProcessor> bilingualProcessors = null, EventHandler<SdlAlignPackageProgressEventArgs<UpdateSdlxliffPhase>> progressHandler = null, EventHandler<MessageEventArgs> messageReporter = null, RetrofitUpdateSettings settings = null)
		{
			ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlxliffPhase>, UpdateSdlxliffPhase> progressReporter = new ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlxliffPhase>, UpdateSdlxliffPhase>(this);
			progressReporter.Handler = (EventHandler<SdlAlignPackageProgressEventArgs<UpdateSdlxliffPhase>>)Delegate.Combine(progressReporter.Handler, progressHandler);
			progressReporter.Report(UpdateSdlxliffPhase.Start, 0);
			ParagraphUnitStore xliffPuStore = _alignmentApiHelper.ReadDocument(_fileTypeManager, xliffFilePath, culture, resourceDataAccessor, AlignmentFilePosition.Left, progressReporter, messageReporter);
			string tempFileUsingFilePath = _alignmentApiHelper.GetTempFileUsingFilePath(xliffFilePath);
			Segmentor impl = new Segmentor(new Sdl.Core.LanguageProcessing.Segmentation.Settings(), resourceDataAccessor);
			progressReporter.Report(UpdateSdlxliffPhase.ReadAlignPackage, 40);
			SdlRetrofitPackage reversePackageFromFile = _alignmentApiHelper.GetReversePackageFromFile(_fileTypeManager, packagePath);
			RetrofitAlignmentDataComparer comparer = new RetrofitAlignmentDataComparer();
			reversePackageFromFile.Alignments.Sort(comparer);
			RetrofitTrackChangesProcessor retrofitTrackChangesProcessor = new RetrofitTrackChangesProcessor(reversePackageFromFile, xliffPuStore, settings);
			IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(xliffFilePath, tempFileUsingFilePath, null);
			converterToDefaultBilingual.AddBilingualProcessor(new BilingualContentHandlerAdapter(impl));
			converterToDefaultBilingual.AddBilingualProcessor(retrofitTrackChangesProcessor);
			if (bilingualProcessors != null)
			{
				foreach (IBilingualContentProcessor bilingualProcessor in bilingualProcessors)
				{
					converterToDefaultBilingual.AddBilingualProcessor(bilingualProcessor);
				}
			}
			progressReporter.Report(UpdateSdlxliffPhase.ReverseAligning, 70);
			converterToDefaultBilingual.Parse();
			_alignmentApiHelper.ReplaceFile(xliffFilePath, tempFileUsingFilePath);
			progressReporter.Report(UpdateSdlxliffPhase.Finish, 100);
			return new ReverseAlignmentUpdateResults(retrofitTrackChangesProcessor.UpdatedSegmentsCount, retrofitTrackChangesProcessor.UpdatedSegmentErrorCount);
		}
	}
}
