using Sdl.Core.Processing.Alignment.Api;
using Sdl.Core.Processing.Alignment.SdlAlignPackage;
using Sdl.Core.TM.ImportExport;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationMemoryImporter : Importer
	{
		private ImportEntity _tmServiceImportEntity;

		public ITranslationMemoryLanguageDirection TranslationMemoryLanguageDirection
		{
			get;
		}

		public new event EventHandler<BatchImportedEventArgs> BatchImported;

		public TranslationMemoryImporter()
		{
			base.BatchImported += Base_BatchImported;
		}

		public TranslationMemoryImporter(ITranslationMemoryLanguageDirection languageDirection)
			: this()
		{
			TranslationMemoryLanguageDirection = languageDirection;
		}

		private void Base_BatchImported(object sender, Sdl.Core.TM.ImportExport.BatchImportedEventArgs e)
		{
			EventHandler<BatchImportedEventArgs> batchImported = this.BatchImported;
			if (batchImported != null)
			{
				BatchImportedEventArgs batchImportedEventArgs = new BatchImportedEventArgs(e.Statistics);
				batchImported(this, batchImportedEventArgs);
				e.Cancel = batchImportedEventArgs.Cancel;
			}
		}

		public new void Import(string fileName)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException(fileInfo.FullName);
			}
			ServerBasedTranslationMemoryLanguageDirection serverBasedTranslationMemoryLanguageDirection = TranslationMemoryLanguageDirection as ServerBasedTranslationMemoryLanguageDirection;
			if (serverBasedTranslationMemoryLanguageDirection != null)
			{
				ServerBasedTranslationMemoryVersion serverVersion = serverBasedTranslationMemoryLanguageDirection.TranslationProviderServer.GetServerVersion();
				if (IsTmServiceSupported(serverVersion) && IsSupportedByTmServiceImport(fileName))
				{
					TmServiceImport(serverBasedTranslationMemoryLanguageDirection, fileName);
					return;
				}
			}
			string a = Path.GetExtension(fileInfo.Name).TrimStart('.').ToLower();
			if (TranslationMemoryLanguageDirection == null)
			{
				throw new InvalidOperationException("TranslationMemoryLanguageDirection cannot be null.");
			}
			TranslationMemoryLanguageDirectionImportExport importDestination = new TranslationMemoryLanguageDirectionImportExport(TranslationMemoryLanguageDirection);
			if (a == "sdlalign")
			{
				ImportAlignFile(fileName);
			}
			else
			{
				try
				{
					Import(fileName, importDestination);
				}
				catch (LanguageMismatchException ex)
				{
					throw new Exception(string.Format(StringResources.Importwizard_NoLanguagePairMatch, ex.OriginSourceLanguage.Name, ex.OriginTargetLanguage.Name, ex.DestinationSourceLanguage.Name, ex.DestinationTargetLanguage.Name));
				}
			}
			serverBasedTranslationMemoryLanguageDirection?.UpdateCachedTranslationUnitCount();
		}

		private void TmServiceImport(ServerBasedTranslationMemoryLanguageDirection ld, string fileName)
		{
			if (!IsTmServiceSupported(ld.TranslationProviderServer.GetServerVersion()))
			{
				throw new ArgumentException("Only supported by TMService.");
			}
			if (!IsSupportedByTmServiceImport(fileName))
			{
				throw new ArgumentException("The file type is not supported by TMService import.");
			}
			_tmServiceImportEntity = ld.Import(base.ImportSettings, fileName);
		}

		public ServerImportStatus GetServerImportStatus()
		{
			if (_tmServiceImportEntity == null)
			{
				throw new ArgumentException("No TM Service import running");
			}
			ServerBasedTranslationMemoryLanguageDirection serverBasedTranslationMemoryLanguageDirection = TranslationMemoryLanguageDirection as ServerBasedTranslationMemoryLanguageDirection;
			if (serverBasedTranslationMemoryLanguageDirection == null)
			{
				throw new ArgumentException("Only server-based TMs support TM Service imports");
			}
			ImportEntity importEntity = null;// serverBasedTranslationMemoryLanguageDirection.TranslationProviderServer?.Service?.GetTranslationMemoryImportById(_tmServiceImportEntity.Id);
            if (importEntity == null)
			{
				throw new ArgumentException("Unknown server import");
			}
			base.Statistics = new ImportStatistics
			{
				TotalRead = importEntity.Read.GetValueOrDefault(),
				Errors = importEntity.Errors.GetValueOrDefault(),
				MergedTranslationUnits = importEntity.Merged.GetValueOrDefault(),
				AddedTranslationUnits = importEntity.Added.GetValueOrDefault(),
				DiscardedTranslationUnits = importEntity.Discarded.GetValueOrDefault(),
				OverwrittenTranslationUnits = importEntity.Overwritten.GetValueOrDefault(),
				BadTranslationUnits = importEntity.Bad.GetValueOrDefault()
			};
			return (ServerImportStatus)importEntity.ScheduledOperation.Status;
		}

		public static bool IsTmServiceSupported(ServerBasedTranslationMemoryVersion? serverBasedTranslationMemoryVersion)
		{
			if (serverBasedTranslationMemoryVersion.HasValue)
			{
				if (serverBasedTranslationMemoryVersion != ServerBasedTranslationMemoryVersion.OnPremiseRest)
				{
					return serverBasedTranslationMemoryVersion == ServerBasedTranslationMemoryVersion.Cloud;
				}
				return true;
			}
			return false;
		}

		public static bool IsSupportedByTmServiceImport(string file)
		{
			string[] source = new string[4]
			{
				".tmx",
				".tmx.gz",
				".sdltm",
				".zip"
			};
			return source.Any((string ext) => file.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase));
		}

		private void ImportAlignFile(string fileName)
		{
			bool isDocumentImport = base.ImportSettings.IsDocumentImport;
			base.ImportSettings.IsDocumentImport = false;
			AlignmentTMBilingualProcessor alignmentTMBilingualProcessor = new AlignmentTMBilingualProcessor(TranslationMemoryLanguageDirection.TranslationProvider, new LanguagePair(TranslationMemoryLanguageDirection.SourceLanguage, TranslationMemoryLanguageDirection.TargetLanguage), base.ImportSettings);
			alignmentTMBilingualProcessor.Initialize();
			alignmentTMBilingualProcessor.AlignSourcePath = Path.GetFileName(fileName);
			alignmentTMBilingualProcessor.AlignTargetPath = Path.GetFileName(fileName);
			IFileTypeManager fileTypeManager = base.FileTypeManager ?? GetDefaultFileTypeManager();
			AlignmentService alignmentService = new AlignmentService(fileTypeManager);
			SdlAlignPackage sdlAlignPackage = alignmentService.ImportSdlAlignPackage(fileName, new List<IBilingualContentProcessor>
			{
				alignmentTMBilingualProcessor
			}, deleteTempExtractionFolder: false);
			IEnumerable<ImportResult> source = alignmentTMBilingualProcessor.AddTranslationUnitsInBundels();
			base.ImportSettings.IsDocumentImport = isDocumentImport;
			base.Statistics = new ImportStatistics
			{
				TotalRead = sdlAlignPackage.Alignments.Count,
				Errors = source.Count((ImportResult r) => r.ErrorCode != ErrorCode.OK),
				MergedTranslationUnits = source.Count((ImportResult r) => r.Action == Sdl.LanguagePlatform.TranslationMemory.Action.Merge),
				AddedTranslationUnits = source.Count((ImportResult r) => r.Action == Sdl.LanguagePlatform.TranslationMemory.Action.Add),
				DiscardedTranslationUnits = source.Count((ImportResult r) => r.Action == Sdl.LanguagePlatform.TranslationMemory.Action.Discard),
				OverwrittenTranslationUnits = source.Count((ImportResult r) => r.Action == Sdl.LanguagePlatform.TranslationMemory.Action.Overwrite),
				BadTranslationUnits = source.Count((ImportResult r) => r.Action == Sdl.LanguagePlatform.TranslationMemory.Action.Error)
			};
		}
	}
}
