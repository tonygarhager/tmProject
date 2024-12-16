using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi;
using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class BcmToFileConverter : IDisposable
	{
		private readonly BcmReader _reader;

		private string _tempDependencyFolder = string.Empty;

		public ContentRestriction ContentRestriction
		{
			get
			{
				return _reader.ContentRestriction;
			}
			set
			{
				_reader.ContentRestriction = value;
			}
		}

		public IFileTypeManager FileTypeManager
		{
			get
			{
				return _reader.FileTypeManager.Value;
			}
			set
			{
				_reader.FileTypeManager = new Lazy<IFileTypeManager>(() => value);
			}
		}

		public string TargetEncoding
		{
			get
			{
				return _reader.TargetEncoding;
			}
			set
			{
				_reader.TargetEncoding = value;
			}
		}

		public event EventHandler ParagraphUnitProcessing
		{
			add
			{
				_reader.ParagraphUnitProcessing += value;
			}
			remove
			{
				_reader.ParagraphUnitProcessing -= value;
			}
		}

		public event EventHandler ParagraphUnitProcessed
		{
			add
			{
				_reader.ParagraphUnitProcessed += value;
			}
			remove
			{
				_reader.ParagraphUnitProcessed += value;
			}
		}

		public BcmToFileConverter(Document document, string outputFilePath)
		{
			_reader = new BcmReader(document, outputFilePath);
		}

		public BcmToFileConverter(string jsonBcmDocument, string outputFilePath)
		{
			Document document = JsonConvert.DeserializeObject<Document>(jsonBcmDocument);
			_reader = new BcmReader(document, outputFilePath);
		}

		public BcmToFileConverter(Document document, string outputFilePath, CommonDelegates.GetGeneratorId getGeneratorIdCallback)
		{
			_reader = new BcmReader(document, outputFilePath, getGeneratorIdCallback);
		}

		public BcmToFileConverter(Document document, string sourceFilePath, string outputFilePath, List<DependencyFile> dependencyFiles, bool saveAsSdlXliff = false)
		{
			bool flag = outputFilePath.Trim().EndsWith(".sdlxliff", StringComparison.InvariantCultureIgnoreCase);
			if (saveAsSdlXliff && !flag)
			{
				throw new ArgumentException("Incorrect file extension when generating SdlXliff files - file extension must be .sdlxliff");
			}
			if (!saveAsSdlXliff && flag)
			{
				throw new ArgumentException("Incorrect file extension when generating native files - file extension may not be .sdlxliff");
			}
			_tempDependencyFolder = Path.Combine(Path.GetTempPath(), DependencyFileHelper.MakeShortGuid(8));
			PrepareDependencyFiles(document, sourceFilePath, dependencyFiles);
			_reader = new BcmReader(document, outputFilePath);
		}

		public void Convert()
		{
			_reader.Convert();
			if (!string.IsNullOrEmpty(_tempDependencyFolder) && Directory.Exists(_tempDependencyFolder))
			{
				Directory.Delete(_tempDependencyFolder, recursive: true);
				_tempDependencyFolder = null;
			}
		}

		public void Dispose()
		{
			_reader?.Dispose();
		}

		private string PrepareDependencyFiles(Document doc, string sourceFilePath, List<DependencyFile> dependencyFiles)
		{
			string tempDependencyFolder = _tempDependencyFolder;
			doc.Files[0].DependencyFiles = (dependencyFiles ?? new List<DependencyFile>());
			string fileTypeDefinitionId = doc.Files[0].FileTypeDefinitionId;
			string dependencyFileId = DependencyFileHelper.GetDependencyFileId(fileTypeDefinitionId);
			IEnumerable<DependencyFile> source = dependencyFiles.Where((DependencyFile d) => DependencyFileHelper.GetDependencyFileType(d.Id, fileTypeDefinitionId) == DependencyFileType.PreTweakedSource);
			if (!string.IsNullOrEmpty(sourceFilePath) && !string.IsNullOrEmpty(dependencyFileId) && !source.Any())
			{
				DependencyFile item = new DependencyFile
				{
					Location = sourceFilePath,
					FileName = Path.GetFileName(sourceFilePath),
					Id = dependencyFileId,
					Usage = Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Generation
				};
				doc.Files[0].DependencyFiles.Insert(0, item);
			}
			string text = Path.Combine(tempDependencyFolder, DependencyFileHelper.MakeShortGuid(8));
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			if (!Directory.Exists(tempDependencyFolder))
			{
				Directory.CreateDirectory(tempDependencyFolder);
			}
			foreach (DependencyFile dependencyFile in doc.Files[0].DependencyFiles)
			{
				if (!string.IsNullOrEmpty(dependencyFile.Location))
				{
					string fileName = Path.GetFileName(dependencyFile.FileName);
					if (!string.IsNullOrEmpty(fileName))
					{
						SetDependencyPath(text, fileName, dependencyFile, doc.Files[0].FileTypeDefinitionId);
					}
				}
			}
			return text;
		}

		private void SetDependencyPath(string localDependencyFolder, string depFileName, DependencyFile dependencyFile, string fileTypeDefinitionId)
		{
			string empty = string.Empty;
			if (DependencyFileHelper.GetDependencyFileType(dependencyFile.Id, fileTypeDefinitionId) == DependencyFileType.Source)
			{
				empty = dependencyFile.Location;
			}
			else
			{
				empty = Path.Combine(localDependencyFolder, depFileName);
				string text = Path.Combine(Path.GetTempPath(), DependencyFileHelper.MakeShortGuid(8) + "_" + depFileName + ".zip");
				string text2 = Path.Combine(localDependencyFolder, "Unzip_(" + DependencyFileHelper.MakeShortGuid(8) + ")");
				Directory.CreateDirectory(text2);
				empty = Path.Combine(text2, depFileName);
				byte[] bytes = System.Convert.FromBase64String(dependencyFile.EmbeddedContent);
				System.IO.File.WriteAllBytes(text, bytes);
				ZipArchive zipArchive = ZipFile.OpenRead(text);
				zipArchive.Entries.FirstOrDefault()?.ExtractToFile(empty);
			}
			dependencyFile.FileName = empty;
		}
	}
}
