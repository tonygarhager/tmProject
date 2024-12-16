using ICSharpCode.SharpZipLib.Zip;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Extensions;
using Sdl.Core.Processing.Processors.Storage;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sdl.Core.Processing.Alignment.SdlAlignPackage
{
	public class AbstractSdlAlignPackageHandler : ISdlAlignPackageHandler
	{
		private const string AlignmentSettingsFile = "AlignmentSettings.xml";

		private const string AlignmentDataFile = "AlignmentData.xml";

		private const int BufferSize = 1048576;

		protected const string ReverseAlignmentUpdatedTarget = "UpdatedTarget.sdlXliff";

		private readonly IFileTypeManager _fileTypeManager;

		protected AlignmentSettingsInfo AlignmentSettingsInfo;

		protected IParagraphUnit ParagraphUnit;

		protected string AlignedBilingualFilePath = string.Empty;

		protected List<AlignmentData> Alignments;

		public EventHandler<BatchProgressEventArgs> ExtractFilesProgress;

		public EventHandler<BatchProgressEventArgs> ReadSettingsProgress;

		public EventHandler<BatchProgressEventArgs> ReadAlignmentsProgress;

		public EventHandler<BatchProgressEventArgs> ReadContentProgress;

		public EventHandler<BatchProgressEventArgs> SerializeAlignmentsProgress;

		public EventHandler<BatchProgressEventArgs> SerializeContentProgress;

		public EventHandler<BatchProgressEventArgs> SerializeSettingsProgress;

		public EventHandler<BatchProgressEventArgs> CreateNewPackageProgress;

		internal AbstractSdlAlignPackageHandler()
		{
			Alignments = new List<AlignmentData>();
		}

		protected internal AbstractSdlAlignPackageHandler(IFileTypeManager fileTypeManager, AlignmentSettingsInfo alignmentSettingsInfo = null, List<AlignmentData> alignments = null, IParagraphUnit paragraphUnit = null)
		{
			AlignmentSettingsInfo = alignmentSettingsInfo;
			Alignments = (alignments ?? new List<AlignmentData>());
			_fileTypeManager = fileTypeManager;
			ParagraphUnit = paragraphUnit;
		}

		protected SdlAlignPackage ReadSdlAlignPackage(string sdlAlignPath, string tempDir)
		{
			UnZip(sdlAlignPath, tempDir);
			ExtractFilesProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AlignmentSettingsInfo));
			using (FileStream stream = File.OpenRead(tempDir + "\\AlignmentSettings.xml"))
			{
				AlignmentSettingsInfo = (AlignmentSettingsInfo)xmlSerializer.Deserialize(stream);
			}
			ReadSettingsProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			xmlSerializer = new XmlSerializer(typeof(DocumentAlignmentDataList));
			using (FileStream stream2 = File.OpenRead(tempDir + "\\AlignmentData.xml"))
			{
				DocumentAlignmentDataList documentAlignmentDataList = (DocumentAlignmentDataList)xmlSerializer.Deserialize(stream2);
				foreach (DocumentAlignmentData item in documentAlignmentDataList)
				{
					Alignments.Add(item.ConvertToAlignmentData());
				}
			}
			ReadAlignmentsProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			ParagraphUnitExtractor paragraphUnitExtractor = GetParagraphUnitExtractor(AlignedBilingualFilePath);
			ReadContentProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			ParagraphUnit = paragraphUnitExtractor.ParagraphUnits[0];
			return new SdlAlignPackage
			{
				AlignedBilingualFilePath = AlignedBilingualFilePath,
				Alignments = Alignments,
				AlignmentSettingsInfo = AlignmentSettingsInfo,
				ParagraphUnit = ParagraphUnit
			};
		}

		public void WriteSdlAlignPackage(SdlAlignPackage existingPackage, string sdlAlignPath)
		{
			string text = AlignmentHelper.CreateRandomFolderName();
			try
			{
				Directory.CreateDirectory(text);
				ParagraphUnit = existingPackage.ParagraphUnit;
				Alignments = existingPackage.Alignments;
				AlignmentSettingsInfo = existingPackage.AlignmentSettingsInfo;
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

		public void Save(string sdlAlignPath)
		{
			string text = AlignmentHelper.CreateRandomFolderName();
			try
			{
				Directory.CreateDirectory(text);
				List<string> list = SerializeFiles(sdlAlignPath, text, "");
				Zip(sdlAlignPath, list.ToArray());
				CreateNewPackageProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			}
			finally
			{
				if (Directory.Exists(text))
				{
					Directory.Delete(text, recursive: true);
				}
			}
		}

		private static void EnsureBackwardsCompatibility(ZipOutputStream zipOutputStream)
		{
			zipOutputStream.UseZip64 = UseZip64.Off;
		}

		public void Zip(string archiveName, params string[] files)
		{
			ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;
			byte[] buffer = new byte[1048576];
			using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.OpenWrite(archiveName)))
			{
				EnsureBackwardsCompatibility(zipOutputStream);
				zipOutputStream.SetLevel(9);
				foreach (string path in files)
				{
					zipOutputStream.PutNextEntry(new ZipEntry(Path.GetFileName(path)));
					FileStream fileStream = File.OpenRead(path);
					try
					{
						int num;
						do
						{
							num = fileStream.Read(buffer, 0, 1048576);
							zipOutputStream.Write(buffer, 0, num);
						}
						while (num > 0);
					}
					finally
					{
						fileStream.Close();
					}
				}
				zipOutputStream.Finish();
				zipOutputStream.Close();
			}
		}

		public void UnZip(string archiveName, string destination)
		{
			ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;
			ZipFile zipFile = new ZipFile(archiveName);
			foreach (ZipEntry item in zipFile)
			{
				if (item.IsFile)
				{
					string text = Path.Combine(destination, item.Name);
					if ((Path.GetExtension(item.Name) ?? string.Empty).ToUpperInvariant() == ".SDLXLIFF" && item.Name != "UpdatedTarget.sdlXliff")
					{
						AlignedBilingualFilePath = text;
					}
					using (FileStream fileStream = File.OpenWrite(text))
					{
						byte[] array = new byte[1048576];
						Stream inputStream = zipFile.GetInputStream(item);
						int count;
						while ((count = inputStream.Read(array, 0, array.Length)) > 0)
						{
							fileStream.Write(array, 0, count);
						}
						fileStream.Flush();
						fileStream.Close();
					}
				}
			}
			zipFile.Close();
		}

		protected virtual List<string> SerializeFiles(string sdlAlignPath, string outputDir, string serializedBillingualPath)
		{
			if (string.IsNullOrEmpty(serializedBillingualPath))
			{
				serializedBillingualPath = SerializeAlignedBilingualFilePath(AlignmentSettingsInfo, ParagraphUnit, sdlAlignPath, outputDir);
				SerializeContentProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			}
			string item = SerializeAlignmentFile(Alignments, outputDir);
			SerializeAlignmentsProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			string item2 = SerializeAligmentSettingFile(AlignmentSettingsInfo, outputDir);
			SerializeSettingsProgress(this, new BatchProgressEventArgs(string.Empty, 1, 1, 0));
			return new List<string>
			{
				serializedBillingualPath,
				item,
				item2
			};
		}

		private string SerializeAlignedBilingualFilePath(AlignmentSettingsInfo alignmentSettingsInfo, IParagraphUnit paragraphUnit, string sdlAlignPath, string outputDir)
		{
			ParagraphUnitParser paragraphUnitParser = new ParagraphUnitParser(alignmentSettingsInfo.LeftDocumentLanguage.GetCultureInfoFromName(), alignmentSettingsInfo.RightDocumentLanguage.GetCultureInfoFromName());
			paragraphUnitParser.AddParagraphUnit(paragraphUnit);
			string text = outputDir + "\\" + Path.GetFileNameWithoutExtension(sdlAlignPath) + ".sdlxliff";
			IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(paragraphUnitParser, text);
			converterToDefaultBilingual.Parse();
			return text;
		}

		private string SerializeAlignmentFile(List<AlignmentData> alignments, string outputDir)
		{
			DocumentAlignmentDataList documentAlignmentDataList = new DocumentAlignmentDataList();
			foreach (AlignmentData alignment in alignments)
			{
				documentAlignmentDataList.Add(alignment.ConvertToDocumentAlignmentData());
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DocumentAlignmentDataList));
			using (FileStream stream = File.OpenWrite(Path.Combine(outputDir, "AlignmentData.xml")))
			{
				xmlSerializer.Serialize(stream, documentAlignmentDataList);
			}
			return outputDir + "\\AlignmentData.xml";
		}

		private string SerializeAligmentSettingFile(AlignmentSettingsInfo alignmentSettingsInfo, string outputDir)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AlignmentSettingsInfo));
			using (FileStream stream = File.OpenWrite(Path.Combine(outputDir, "AlignmentSettings.xml")))
			{
				xmlSerializer.Serialize(stream, alignmentSettingsInfo);
			}
			return outputDir + "\\AlignmentSettings.xml";
		}

		protected ParagraphUnitExtractor GetParagraphUnitExtractor(string fileName)
		{
			IMultiFileConverter converterToDefaultBilingual = _fileTypeManager.GetConverterToDefaultBilingual(fileName, string.Empty, null);
			ParagraphUnitExtractor paragraphUnitExtractor = new ParagraphUnitExtractor();
			converterToDefaultBilingual.AddBilingualProcessor(paragraphUnitExtractor);
			converterToDefaultBilingual.BilingualDocumentGenerator = null;
			converterToDefaultBilingual.Parse();
			return paragraphUnitExtractor;
		}
	}
}
