using Sdl.Core.LanguageProcessing.Segmentation;
using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.SdlAlignPackage;
using Sdl.Core.Processing.Processors.Storage;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Api
{
	internal class AlignmentApiHelper
	{
		public void RemoveSdlxliffWriter(IMultiFileConverter converter)
		{
			converter.BilingualDocumentGenerator = null;
		}

		public bool AreParallel(ParagraphUnitStore leftDocument, ParagraphUnitStore rightDocument)
		{
			if (leftDocument.SegmentPairCount == 0 || rightDocument.SegmentPairCount == 0)
			{
				return leftDocument.SegmentPairCount == rightDocument.SegmentPairCount;
			}
			if (leftDocument.NonStructureParagraphUnitCount == rightDocument.NonStructureParagraphUnitCount)
			{
				return Math.Abs(1f - (float)leftDocument.SegmentPairCount / (float)rightDocument.SegmentPairCount) < 0.1f;
			}
			return false;
		}

		public void CreateOutputFolderHiearchy(string fileName)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(fileName);
				DirectoryInfo directory = fileInfo.Directory;
				if (directory != null && !directory.Exists)
				{
					directory.Create();
				}
			}
			catch
			{
			}
		}

		public ParagraphUnitStore ReadLeftDocument(Tuple<IFileTypeManager, CultureInfo, IResourceDataAccessor, ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase>, EventHandler<MessageEventArgs>> fileProcessingInfo, string filename)
		{
			fileProcessingInfo.Deconstruct(out IFileTypeManager item, out CultureInfo item2, out IResourceDataAccessor item3, out ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase> item4, out EventHandler<MessageEventArgs> item5);
			IFileTypeManager fileTypeManager = item;
			CultureInfo sourceLanguage = item2;
			IResourceDataAccessor resourceDataAccessor = item3;
			ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase> progressReporter = item4;
			EventHandler<MessageEventArgs> messageReporter = item5;
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("File not found", filename);
			}
			Segmentor impl = new Segmentor(new Sdl.Core.LanguageProcessing.Segmentation.Settings(), resourceDataAccessor);
			ParagraphUnitExtractor paragraphUnitExtractor = new ParagraphUnitExtractor();
			IMultiFileConverter converter = fileTypeManager.GetConverter(new string[1]
			{
				filename
			}, sourceLanguage, null, delegate(object source, MessageEventArgs args)
			{
				messageReporter?.Invoke(source, args);
			});
			converter.Progress += delegate(object source, BatchProgressEventArgs args)
			{
				byte filePercentComplete = args.FilePercentComplete;
				progressReporter.Report(ReverseAlignmentPhase.ReadXliffTarget, (int)((double)(int)filePercentComplete / 100.0 * 20.0));
			};
			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(impl));
			converter.AddBilingualProcessor(paragraphUnitExtractor);
			converter.Parse();
			return paragraphUnitExtractor.ParagraphUnits;
		}

		public ParagraphUnitStore ReadRightDocument(Tuple<IFileTypeManager, CultureInfo, IResourceDataAccessor, ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase>, EventHandler<MessageEventArgs>> fileProcessingInfo, string rightNativeFilePath, out string outputFile, out string fileId)
		{
			fileProcessingInfo.Deconstruct(out IFileTypeManager item, out CultureInfo item2, out IResourceDataAccessor item3, out ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase> item4, out EventHandler<MessageEventArgs> item5);
			IFileTypeManager fileTypeManager = item;
			CultureInfo sourceLanguage = item2;
			IResourceDataAccessor resourceDataAccessor = item3;
			ProgressReporter<ReverseAlignmentProgressEventArgs, ReverseAlignmentPhase> progressReporter = item4;
			EventHandler<MessageEventArgs> messageReporter = item5;
			string text = AlignmentHelper.CreateRandomFolderName();
			Directory.CreateDirectory(text);
			outputFile = text + "\\" + Path.GetFileNameWithoutExtension(rightNativeFilePath) + ".sdlxliff";
			if (!File.Exists(rightNativeFilePath))
			{
				throw new FileNotFoundException("File not found", rightNativeFilePath);
			}
			Segmentor segmentor = new Segmentor(new Sdl.Core.LanguageProcessing.Segmentation.Settings(), resourceDataAccessor);
			segmentor.Settings.TargetSegmentCreationMode = TargetSegmentCreationMode.None;
			ParagraphUnitExtractor paragraphUnitExtractor = new ParagraphUnitExtractor();
			IMultiFileConverter converterToDefaultBilingual = fileTypeManager.GetConverterToDefaultBilingual(new string[1]
			{
				rightNativeFilePath
			}, outputFile, sourceLanguage, null, delegate(object source, MessageEventArgs args)
			{
				messageReporter?.Invoke(source, args);
			});
			converterToDefaultBilingual.Progress += delegate(object source, BatchProgressEventArgs args)
			{
				byte filePercentComplete = args.FilePercentComplete;
				progressReporter.Report(ReverseAlignmentPhase.ReadUpdatedTarget, 20 + (int)((double)(int)filePercentComplete / 100.0 * 20.0));
			};
			converterToDefaultBilingual.AddBilingualProcessor(new BilingualContentHandlerAdapter(segmentor));
			converterToDefaultBilingual.AddBilingualProcessor(paragraphUnitExtractor);
			converterToDefaultBilingual.Parse();
			fileId = paragraphUnitExtractor.FileProperties.FileConversionProperties.FileTypeDefinitionId.Id;
			return paragraphUnitExtractor.ParagraphUnits;
		}

		public ParagraphUnitStore ReadDocument(IFileTypeManager fileTypeManager, string filename, CultureInfo culture, IResourceDataAccessor resourceDataAccessor, AlignmentFilePosition filePosition, ProgressReporter<AlignmentProgressEventArgs, AlignmentPhase> progressReporter, EventHandler<MessageEventArgs> messageReporter)
		{
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("File not found", filename);
			}
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			Segmentor impl = new Segmentor(new Sdl.Core.LanguageProcessing.Segmentation.Settings(), resourceDataAccessor);
			ParagraphUnitExtractor paragraphUnitExtractor = new ParagraphUnitExtractor();
			IMultiFileConverter converter = fileTypeManager.GetConverter(new string[1]
			{
				filename
			}, culture, null, delegate(object source, MessageEventArgs args)
			{
				messageReporter?.Invoke(source, args);
			});
			converter.Progress += delegate(object source, BatchProgressEventArgs args)
			{
				byte filePercentComplete = args.FilePercentComplete;
				if (filePosition == AlignmentFilePosition.Left)
				{
					progressReporter.Report(AlignmentPhase.ReadLeftFile, (int)((double)(int)filePercentComplete / 100.0 * 20.0));
				}
				else
				{
					progressReporter.Report(AlignmentPhase.ReadRightFile, 20 + (int)((double)(int)filePercentComplete / 100.0 * 20.0));
				}
			};
			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(impl));
			converter.AddBilingualProcessor(paragraphUnitExtractor);
			converter.Parse();
			return paragraphUnitExtractor.ParagraphUnits;
		}

		public ParagraphUnitStore ReadDocument(IFileTypeManager fileTypeManager, string filename, CultureInfo culture, IResourceDataAccessor resourceDataAccessor, AlignmentFilePosition filePosition, ProgressReporter<SdlAlignPackageProgressEventArgs<UpdateSdlxliffPhase>, UpdateSdlxliffPhase> progressReporter, EventHandler<MessageEventArgs> messageReporter)
		{
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException("File not found", filename);
			}
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			Segmentor impl = new Segmentor(new Sdl.Core.LanguageProcessing.Segmentation.Settings(), resourceDataAccessor);
			ParagraphUnitExtractor paragraphUnitExtractor = new ParagraphUnitExtractor();
			IMultiFileConverter converter = fileTypeManager.GetConverter(new string[1]
			{
				filename
			}, culture, null, delegate(object source, MessageEventArgs args)
			{
				messageReporter?.Invoke(source, args);
			});
			converter.Progress += delegate(object source, BatchProgressEventArgs args)
			{
				byte filePercentComplete = args.FilePercentComplete;
				progressReporter.Report(UpdateSdlxliffPhase.ReadSdlxliff, (int)((double)(int)filePercentComplete / 100.0 * 20.0));
			};
			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(impl));
			converter.AddBilingualProcessor(paragraphUnitExtractor);
			converter.Parse();
			return paragraphUnitExtractor.ParagraphUnits;
		}

		public AlignmentAlgorithmSettings CreateAlignmentAlgorithmSettings(AlignmentSettings alignmentSettings, bool areParallel = false)
		{
			AlignmentAlgorithmSettings alignmentAlgorithmSettings = new AlignmentAlgorithmSettings(alignmentSettings.LeftCulture, alignmentSettings.RightCulture)
			{
				ResourceDataAccessor = alignmentSettings.ResourceDataAccessor
			};
			AlignmentMode alignmentMode = alignmentSettings.AlignmentMode;
			if (alignmentMode == AlignmentMode.Automatic)
			{
				alignmentMode = ((!areParallel) ? AlignmentMode.Accurate : AlignmentMode.Optimistic);
			}
			switch (alignmentMode)
			{
			case AlignmentMode.IdenticalCultures:
				alignmentAlgorithmSettings.AlgorithmType = AlignmentAlgorithmType.IdenticalCultures;
				break;
			case AlignmentMode.Version2:
				alignmentAlgorithmSettings.AlgorithmType = AlignmentAlgorithmType.Version2;
				break;
			case AlignmentMode.Version1:
				alignmentAlgorithmSettings.AlgorithmType = AlignmentAlgorithmType.Version1;
				break;
			case AlignmentMode.Fast:
				alignmentAlgorithmSettings.AlgorithmType = AlignmentAlgorithmType.Fast;
				break;
			case AlignmentMode.Optimistic:
				alignmentAlgorithmSettings.AlgorithmType = AlignmentAlgorithmType.Optimistic;
				break;
			case AlignmentMode.Accurate:
				alignmentAlgorithmSettings.AlgorithmType = AlignmentAlgorithmType.Accurate;
				if (alignmentSettings.BilingualDictionary != null)
				{
					alignmentAlgorithmSettings.BilingualDictionary = alignmentSettings.BilingualDictionary;
					alignmentAlgorithmSettings.UpdateBilingualDictionary = (alignmentSettings.UpdateBilingualDictionary && !alignmentAlgorithmSettings.BilingualDictionary.IsReadOnly);
				}
				break;
			case AlignmentMode.Automatic:
				throw new InvalidOperationException("Should be caught above");
			default:
				throw new ArgumentException($"Unknown algorithm mode: {alignmentSettings.AlignmentMode}");
			}
			return alignmentAlgorithmSettings;
		}

		public AlignmentAlgorithmSettings CreateReverseAlignmentAlgorithmSettings(ReverseAlignmentSettings alignmentSettings)
		{
			return new AlignmentAlgorithmSettings(alignmentSettings.RightCulture, alignmentSettings.RightCulture)
			{
				ResourceDataAccessor = alignmentSettings.ResourceDataAccessor,
				AlgorithmType = AlignmentAlgorithmType.IdenticalCultures
			};
		}

		public AlignmentStatistics CalculateStatistics(IEnumerable<AlignmentData> alignments)
		{
			int num = 0;
			int num2 = 0;
			AlignmentData[] array = (alignments as AlignmentData[]) ?? alignments.ToArray();
			int totalAlignmentsCount = array.Length;
			IDictionary<AlignmentQuality, int> dictionary = new Dictionary<AlignmentQuality, int>();
			IDictionary<AlignmentType, int> dictionary2 = new Dictionary<AlignmentType, int>();
			AlignmentData[] array2 = array;
			foreach (AlignmentData alignmentData in array2)
			{
				int count = alignmentData.LeftIds.Count;
				int count2 = alignmentData.RightIds.Count;
				num += count;
				num2 += count2;
				AlignmentQuality quality = alignmentData.Quality;
				if (!dictionary.ContainsKey(quality))
				{
					dictionary[quality] = 0;
				}
				dictionary[quality]++;
				AlignmentType alignmentType = GetAlignmentType(alignmentData);
				if (!dictionary2.ContainsKey(alignmentType))
				{
					dictionary2[alignmentType] = 0;
				}
				dictionary2[alignmentType]++;
			}
			return new AlignmentStatistics(num, num2, totalAlignmentsCount, dictionary, dictionary2);
		}

		private AlignmentType GetAlignmentType(AlignmentData alignment)
		{
			return AlignmentHelper.GetAlignmentType(alignment);
		}

		public AlignmentType GetAlignmentType(int leftSegmentsCount, int rightSegmentsCount)
		{
			return AlignmentHelper.GetAlignmentType(leftSegmentsCount, rightSegmentsCount);
		}

		public IParagraphUnit GetParagraphUnitFromMergedSdlXliff(IFileTypeManager fileTypeManager, string sdlXliffFilePath)
		{
			IMultiFileConverter converterToDefaultBilingual = fileTypeManager.GetConverterToDefaultBilingual(sdlXliffFilePath, string.Empty, null);
			ParagraphUnitExtractor paragraphUnitExtractor = new ParagraphUnitExtractor();
			converterToDefaultBilingual.AddBilingualProcessor(paragraphUnitExtractor);
			converterToDefaultBilingual.BilingualDocumentGenerator = null;
			converterToDefaultBilingual.Parse();
			return paragraphUnitExtractor.ParagraphUnits[0];
		}

		public SdlRetrofitPackage GetReversePackageFromFile(IFileTypeManager fileTypeManager, string packagePath)
		{
			SdlReverseAlignPackageHandler sdlReverseAlignPackageHandler = new SdlReverseAlignPackageHandler(fileTypeManager);
			sdlReverseAlignPackageHandler.ExtractFilesProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ExtractFilesProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.ReadSettingsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadSettingsProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.ReadAlignmentsProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadAlignmentsProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.ReadContentProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadContentProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.ReadMappingFilesProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadMappingFilesProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
			sdlReverseAlignPackageHandler.ReadUpdatedTargetFileProgress = (EventHandler<BatchProgressEventArgs>)Delegate.Combine(sdlReverseAlignPackageHandler.ReadUpdatedTargetFileProgress, (EventHandler<BatchProgressEventArgs>)delegate
			{
			});
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
			return sdlReverseAlignPackageHandler.ReadPackage(packagePath);
		}

		public string GetTempFileUsingFilePath(string xliffFilePath)
		{
			string text = AlignmentHelper.CreateRandomFolderName();
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text + "\\" + Path.GetFileNameWithoutExtension(xliffFilePath) + ".sdlxliff";
		}

		public void ReplaceFile(string victimFile, string overiderFile)
		{
			File.Delete(victimFile);
			File.Copy(overiderFile, victimFile);
			File.Delete(overiderFile);
		}
	}
}
