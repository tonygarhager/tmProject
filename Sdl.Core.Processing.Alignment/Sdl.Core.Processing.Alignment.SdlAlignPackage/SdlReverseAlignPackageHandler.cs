using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.ReverseAlignment;
using Sdl.Core.Processing.Processors.Storage;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	public class SdlReverseAlignPackageHandler : AbstractSdlAlignPackageHandler
	{
		private const string ReverseAlignmentMappingFile = "ReverseAlignmentMappings.xml";

		private AlignmentToOriginalMapper _reverseMapper;

		private string _retrofitUpdateTargetFilePath;

		private string _retrofitMappingFilePath;

		public EventHandler<BatchProgressEventArgs> ReadMappingFilesProgress;

		public EventHandler<BatchProgressEventArgs> ReadUpdatedTargetFileProgress;

		public EventHandler<BatchProgressEventArgs> SerializeMappingFilesPProgress;

		public SdlReverseAlignPackageHandler()
		{
		}

		public SdlReverseAlignPackageHandler(IFileTypeManager fileTypeManager)
			: base(fileTypeManager)
		{
		}

		public SdlReverseAlignPackageHandler(IFileTypeManager fileTypeManager, AlignmentSettingsInfo alignmentSettingsInfo, List<AlignmentData> alignments, IParagraphUnit paragraphUnit, AlignmentToOriginalMapper reverseMapper, string retrofitUpdateTargetFilePath)
			: base(fileTypeManager, alignmentSettingsInfo, alignments, paragraphUnit)
		{
			_reverseMapper = reverseMapper;
			_retrofitUpdateTargetFilePath = retrofitUpdateTargetFilePath;
		}

		public SdlRetrofitPackage ReadPackage(string sdlAlignPath, bool deleteTempExtractionFolder = true)
		{
			string text = AlignmentHelper.CreateRandomFolderName();
			Directory.CreateDirectory(text);
			return ReadRetrofitPackage(sdlAlignPath, text);
		}

		public void WritePackage(SdlRetrofitPackage existingPackage, string sdlAlignPath)
		{
			string text = AlignmentHelper.CreateRandomFolderName();
			try
			{
				Directory.CreateDirectory(text);
				ParagraphUnit = existingPackage.ParagraphUnit;
				Alignments = existingPackage.Alignments;
				AlignmentSettingsInfo = existingPackage.AlignmentSettingsInfo;
				_retrofitUpdateTargetFilePath = existingPackage.UpdatedTargetBilingualFilePath;
				_retrofitMappingFilePath = existingPackage.ReverseAlignmentMappingsFilePath;
				RetrofitAlignmentDataComparer comparer = new RetrofitAlignmentDataComparer();
				Alignments.Sort(comparer);
				List<string> list = SerializeFiles(sdlAlignPath, text, existingPackage.AlignedBilingualFilePath);
				string text2 = sdlAlignPath;
				if (File.Exists(sdlAlignPath))
				{
					text2 = Path.Combine(Path.GetDirectoryName(sdlAlignPath), Path.GetFileNameWithoutExtension(sdlAlignPath) + "_updated.sdlAlign");
				}
				Zip(text2, list.ToArray());
				CreateNewPackageProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
				if (sdlAlignPath != text2)
				{
					File.Copy(text2, sdlAlignPath, overwrite: true);
					File.Delete(text2);
				}
			}
			finally
			{
				if (Directory.Exists(text))
				{
					Directory.Delete(text, recursive: true);
				}
			}
		}

		protected override List<string> SerializeFiles(string sdlAlignPath, string outputDir, string serializedBillingualPath)
		{
			List<string> list = base.SerializeFiles(sdlAlignPath, outputDir, serializedBillingualPath);
			if (string.IsNullOrEmpty(_retrofitMappingFilePath))
			{
				_retrofitMappingFilePath = SerializeMappingFile(_reverseMapper, outputDir);
				SerializeMappingFilesPProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			}
			string text = Path.Combine(outputDir, "UpdatedTarget.sdlXliff");
			File.Copy(_retrofitUpdateTargetFilePath, text, overwrite: true);
			list.Add(_retrofitMappingFilePath);
			list.Add(text);
			return list;
		}

		private static string SerializeMappingFile(AlignmentToOriginalMapper reverseMapper, string outputDir)
		{
			string text = Path.Combine(outputDir, "ReverseAlignmentMappings.xml");
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AlignmentToOriginalMapper));
			using (FileStream stream = File.OpenWrite(text))
			{
				xmlSerializer.Serialize(stream, reverseMapper);
				return text;
			}
		}

		private SdlRetrofitPackage ReadRetrofitPackage(string sdlAlignPath, string tempDir)
		{
			SdlAlignPackage package = ReadSdlAlignPackage(sdlAlignPath, tempDir);
			SdlRetrofitPackage sdlRetrofitPackage = AlignPackageToRetrofitPackage(package);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AlignmentToOriginalMapper));
			sdlRetrofitPackage.ReverseAlignmentMappingsFilePath = tempDir + '\\' + "ReverseAlignmentMappings.xml";
			sdlRetrofitPackage.UpdatedTargetBilingualFilePath = tempDir + '\\' + "UpdatedTarget.sdlXliff";
			using (FileStream stream = File.OpenRead(sdlRetrofitPackage.ReverseAlignmentMappingsFilePath))
			{
				_reverseMapper = (AlignmentToOriginalMapper)xmlSerializer.Deserialize(stream);
			}
			ReadMappingFilesProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			sdlRetrofitPackage.ReverseMapper = _reverseMapper;
			ParagraphUnitExtractor paragraphUnitExtractor = GetParagraphUnitExtractor(sdlRetrofitPackage.UpdatedTargetBilingualFilePath);
			sdlRetrofitPackage.UpdatedTargetPUs = paragraphUnitExtractor.ParagraphUnits;
			ReadUpdatedTargetFileProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			return sdlRetrofitPackage;
		}

		private static SdlRetrofitPackage AlignPackageToRetrofitPackage(SdlAlignPackage package)
		{
			return new SdlRetrofitPackage
			{
				AlignmentSettingsInfo = package.AlignmentSettingsInfo,
				Alignments = package.Alignments,
				AlignedBilingualFilePath = package.AlignedBilingualFilePath,
				ParagraphUnit = package.ParagraphUnit
			};
		}
	}
}
