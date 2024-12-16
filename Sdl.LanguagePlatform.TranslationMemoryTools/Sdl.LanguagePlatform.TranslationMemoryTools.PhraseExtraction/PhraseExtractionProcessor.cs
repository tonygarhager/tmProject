#define TRACE
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Stat;
using Sdl.LanguagePlatform.Stat.WordAlignment;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryTools.PhraseExtraction
{
	public class PhraseExtractionProcessor
	{
		public static readonly string TraceCategory = "Sdl.LanguagePlatform.Stat.PhraseExtraction";

		private DataLocation _location;

		private CultureInfo _sourceCulture;

		private CultureInfo _targetCulture;

		private string _tmxFilePath;

		private readonly TraceSource _traceSource;

		public event EventHandler<ProgressEventArgs> Progress;

		public PhraseExtractionProcessor()
		{
			_traceSource = new TraceSource(TraceCategory, SourceLevels.Warning);
		}

		public void Process(string tmxFilePath, CultureInfo sourceCulture, CultureInfo targetCulture, int maxIterationCount, int maxTUs, string dbFilePath)
		{
			if (string.IsNullOrEmpty(tmxFilePath))
			{
				throw new ArgumentNullException("tmxFilePath");
			}
			if (!File.Exists(tmxFilePath))
			{
				throw new FileNotFoundException("File not found", tmxFilePath);
			}
			_tmxFilePath = tmxFilePath;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				_traceSource.TraceInformation("Start encoding TMX file.");
				EncodeTMX(tmxFilePath, sourceCulture, targetCulture, toGenerateInvertIndice: false, maxTUs);
				_traceSource.TraceInformation("Complete encoding at {0} s.\n", stopwatch.ElapsedMilliseconds / 1000);
				_traceSource.TraceInformation("Compute word associations.");
				ComputeChiSquareLexicalTranslationProb(_location, _sourceCulture, _targetCulture);
				_traceSource.TraceInformation("Complete word associations at {0}s.\n", stopwatch.ElapsedMilliseconds / 1000);
				_traceSource.TraceInformation("Compute phrases.");
				ComputeBilingualPhrases(_location, _sourceCulture, _targetCulture, useChiSquare: true);
				_traceSource.TraceInformation("Complete phrase calculation at {0}s.\n", stopwatch.ElapsedMilliseconds / 1000);
				_traceSource.TraceInformation("Storing the phrases.");
				CreatePhraseDb(_location, dbFilePath);
				_traceSource.TraceInformation("Complete at {0}s.\n", stopwatch.ElapsedMilliseconds / 1000);
				stopwatch.Stop();
			}
			catch (PhraseExtractionCancelException)
			{
			}
			finally
			{
				PurgeAuxilliaryFiles(_location);
			}
		}

		public void SetupDataLocation()
		{
			FileInfo fileInfo = new FileInfo(_tmxFilePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
			if (fileInfo.Extension.Equals(".gz", StringComparison.CurrentCultureIgnoreCase))
			{
				fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName + Path.DirectorySeparatorChar.ToString() + fileNameWithoutExtension);
			if (!directoryInfo.Exists)
			{
				Directory.CreateDirectory(directoryInfo.FullName);
			}
			_location = new DataLocation(directoryInfo.FullName);
		}

		public void EncodeTMX(string inputFileName, CultureInfo sourceCulture, CultureInfo targetCulture, bool toGenerateInvertIndice, int maxTUs)
		{
			OnProgress(ProgressStage.Encoding);
			TMXDataEncoder tMXDataEncoder = new TMXDataEncoder();
			tMXDataEncoder.Progress = (EventHandler<ProgressEventArgs>)Delegate.Combine(tMXDataEncoder.Progress, new EventHandler<ProgressEventArgs>(OnProgress));
			TMXDataEncoder.Settings settings = new TMXDataEncoder.Settings
			{
				ComputeTokenFrequencies = true,
				MaxTUs = maxTUs
			};
			_location = tMXDataEncoder.Encode(inputFileName, sourceCulture, targetCulture, null, null, settings);
			if (toGenerateInvertIndice)
			{
				InvertedFileComputer invertedFileComputer = new InvertedFileComputer();
				invertedFileComputer.Invert(_location, tMXDataEncoder.LanguageDirection.SourceCulture);
				invertedFileComputer.Invert(_location, tMXDataEncoder.LanguageDirection.TargetCulture);
			}
			_sourceCulture = tMXDataEncoder.LanguageDirection.SourceCulture;
			_targetCulture = tMXDataEncoder.LanguageDirection.TargetCulture;
		}

		public void ComputeLexicalTranslationProb(DataLocation location, CultureInfo sourceCulture, CultureInfo targetCulture, int maxIterationCount)
		{
			OnProgress(ProgressStage.Probability);
			new LTPComputer(location, sourceCulture, targetCulture).Compute(0.0001, maxIterationCount);
			new LTPComputer(location, targetCulture, sourceCulture).Compute(0.0001, maxIterationCount);
		}

		public void ComputeChiSquareLexicalTranslationProb(DataLocation location, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			OnProgress(ProgressStage.Probability);
			BilingualChiSquareComputer bilingualChiSquareComputer = new BilingualChiSquareComputer(location, sourceCulture, targetCulture);
			bilingualChiSquareComputer.Progress = (EventHandler<ProgressEventArgs>)Delegate.Combine(bilingualChiSquareComputer.Progress, new EventHandler<ProgressEventArgs>(OnProgress));
			bilingualChiSquareComputer.Compute();
		}

		public void ComputeAlignment(DataLocation location, CultureInfo sourceCulture, CultureInfo targetCulture, bool useChiSquare)
		{
			OnProgress(ProgressStage.WordAlignment);
			ScoreProviderType providerType = ScoreProviderType.LTP;
			if (useChiSquare)
			{
				providerType = ScoreProviderType.Chi2;
			}
			WordAlignmentFileComputer wordAlignmentFileComputer = new WordAlignmentFileComputer(location, sourceCulture, targetCulture, providerType);
			wordAlignmentFileComputer.Progress = (EventHandler<ProgressEventArgs>)Delegate.Combine(wordAlignmentFileComputer.Progress, new EventHandler<ProgressEventArgs>(OnProgress));
			wordAlignmentFileComputer.ComputeAlignments(null, 0, verbose: false);
		}

		public void ComputeBilingualPhrases(DataLocation location, CultureInfo sourceCulture, CultureInfo targetCulture, bool useChiSquare)
		{
			OnProgress(ProgressStage.ComputeBilingual);
			BilingualPhraseFileComputer bilingualPhraseFileComputer = new BilingualPhraseFileComputer(location, sourceCulture, targetCulture);
			bilingualPhraseFileComputer.Progress = (EventHandler<ProgressEventArgs>)Delegate.Combine(bilingualPhraseFileComputer.Progress, new EventHandler<ProgressEventArgs>(OnProgress));
			ScoreProviderType scoreProviderType = ScoreProviderType.LTP;
			if (useChiSquare)
			{
				scoreProviderType = ScoreProviderType.Chi2;
			}
			IScoreProvider scoreProvider;
			switch (scoreProviderType)
			{
			case ScoreProviderType.LTP:
				scoreProvider = new LTPScoreProvider(location, sourceCulture, targetCulture);
				break;
			case ScoreProviderType.Chi2:
				scoreProvider = new ChiSquareScoreProvider(location, sourceCulture, targetCulture);
				break;
			default:
				throw new Exception("Unknown score provider type");
			}
			if (!scoreProvider.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "bilingual association scores");
			}
			bilingualPhraseFileComputer.ComputePhrases(null, 0, verbose: false, scoreProvider);
		}

		public void CreatePhraseDb(DataLocation location, string dbFilePath)
		{
			OnProgress(ProgressStage.StoreBilingual);
			if (location == null)
			{
				SetupDataLocation();
				location = _location;
			}
			FileInfo[] files = location.Directory.GetFiles("*.pm");
			if (File.Exists(dbFilePath))
			{
				File.Delete(dbFilePath);
			}
			DatabaseConverter.Convert(files[0].FullName, dbFilePath);
		}

		public void PurgeAuxilliaryFiles(DataLocation location)
		{
			if (location == null)
			{
				SetupDataLocation();
				location = _location;
			}
			try
			{
				FileInfo[] files = location.Directory.GetFiles();
				for (int i = 0; i < files.Length; i++)
				{
					files[i].Delete();
				}
				location.Directory.Delete();
			}
			catch (IOException)
			{
			}
		}

		private void OnProgress(ProgressStage progressStage, int progressNumber = 0)
		{
			OnProgress(new ProgressEventArgs(progressStage, progressNumber));
		}

		private void OnProgress(ProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		private void OnProgress(object sender, ProgressEventArgs progressEventArgs)
		{
			if (this.Progress != null)
			{
				this.Progress(this, progressEventArgs);
				if (progressEventArgs.Cancel)
				{
					throw new PhraseExtractionCancelException();
				}
			}
		}
	}
}
