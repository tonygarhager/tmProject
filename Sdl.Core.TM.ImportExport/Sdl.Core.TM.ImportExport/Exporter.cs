using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.IO.TMX;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Sdl.Core.TM.ImportExport
{
	public class Exporter
	{
		public TranslationUnitFormat TmxExportFormat
		{
			get;
			set;
		}

		public FilterExpression FilterExpression
		{
			get;
			set;
		}

		public event EventHandler<BatchExportedEventArgs> BatchExported;

		private void Export(IExportOrigin exportOrigin, IEventReceiver writer)
		{
			RegularIterator iter = new RegularIterator();
			if (FilterExpression != null)
			{
				iter.Filter = FilterExpression;
			}
			iter.MaxScan = 1000;
			int num = 0;
			TMXStartOfInputEvent e = new TMXStartOfInputEvent
			{
				SourceCultureName = exportOrigin.SourceLanguage.Name,
				TargetCultureName = exportOrigin.TargetLanguage.Name,
				CreationDate = exportOrigin.CreationDate,
				CreationUser = exportOrigin.CreationUserName,
				Fields = exportOrigin.FieldDefinitions,
				TMName = exportOrigin.Name,
				BuiltinRecognizers = exportOrigin.BuiltinRecognizers,
				TokenizerFlags = exportOrigin.TokenizerFlags,
				WordCountFlags = exportOrigin.WordCountFlags,
				UsesIdContextMatch = exportOrigin.UsesIdContextMatch,
				TextContextMatchType = exportOrigin.TextContextMatchType,
				IncludesContextContent = exportOrigin.IncludesContextContent
			};
			writer.Emit(e);
			TranslationUnit[] translationUnits = exportOrigin.GetTranslationUnits(ref iter);
			while (iter.ScannedTranslationUnits > 0)
			{
				num += translationUnits.Length;
				TranslationUnit[] array = translationUnits;
				foreach (TranslationUnit tu in array)
				{
					writer.Emit(new TUEvent(tu));
				}
				if (_onBatchExported(iter.ProcessedTranslationUnits, num) == ImportExportResponse.Cancel)
				{
					break;
				}
				translationUnits = exportOrigin.GetTranslationUnits(ref iter);
			}
			writer.Emit(new EndOfInputEvent());
			_onBatchExported(iter.ProcessedTranslationUnits, num);
		}

		private ImportExportResponse _onBatchExported(int totalProcessed, int totalExported)
		{
			EventHandler<BatchExportedEventArgs> batchExported = this.BatchExported;
			if (batchExported == null)
			{
				return ImportExportResponse.Continue;
			}
			BatchExportedEventArgs batchExportedEventArgs = new BatchExportedEventArgs(totalProcessed, totalExported);
			batchExported(this, batchExportedEventArgs);
			if (!batchExportedEventArgs.Cancel)
			{
				return ImportExportResponse.Continue;
			}
			return ImportExportResponse.Cancel;
		}

		public void Export(IExportOrigin exportOrigin, string fileName, bool overwriteExisting)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			FileInfo fileInfo = new FileInfo(fileName);
			if (fileInfo.Exists)
			{
				if (!overwriteExisting)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Output path {0} exists, and overwriteExisting == False.", new object[1]
					{
						fileInfo.FullName
					}));
				}
				fileInfo.Delete();
			}
			TMXWriterSettings writerSettings = new TMXWriterSettings(Encoding.UTF8)
			{
				TargetFormat = TmxExportFormat
			};
			if (fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
			{
				using (FileStream stream = File.Create(fileName))
				{
					using (GZipStream outStream = new GZipStream(stream, CompressionMode.Compress))
					{
						using (TMXWriter writer = new TMXWriter(outStream, writerSettings))
						{
							Export(exportOrigin, writer);
						}
					}
				}
			}
			else
			{
				using (TMXWriter writer2 = new TMXWriter(fileName, writerSettings))
				{
					Export(exportOrigin, writer2);
				}
			}
		}
	}
}
