using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.IO.TMX;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class TmxLegacyTranslationMemory : ITmxLegacyTranslationMemory, ILegacyTranslationMemory
	{
		/// <summary>
		/// A delegate which is called to indicate progress of the TMX Scan.
		/// </summary>
		/// <returns>If the implementation returns true, the further scan will be
		/// cancelled.</returns>
		public delegate bool ScanProgressDelegate(int read);

		private class ScanResult
		{
			public Dictionary<CultureInfo, int> EncounteredLanguages = new Dictionary<CultureInfo, int>();

			public Dictionary<LanguagePair, int> LanguageDirections = new Dictionary<LanguagePair, int>();

			public FieldDefinitions FieldDefinitions = new FieldDefinitions();

			public TMXStartOfInputEvent TMXHeader;

			public int RawTUs;

			public int TotalTUs;
		}

		private readonly string _tmxFilePath;

		public string TmxFilePath => _tmxFilePath;

		public string Url => TmxFilePath;

		public string DisplayName => TmxFilePath;

		public event ScanProgressDelegate OnScanProgress;

		public TmxLegacyTranslationMemory(string tmxFilePath)
		{
			if (string.IsNullOrEmpty(tmxFilePath))
			{
				throw new ArgumentNullException("tmxFilePath");
			}
			_tmxFilePath = tmxFilePath;
		}

		public void Check()
		{
			using (StreamReader streamReader = File.OpenText(_tmxFilePath))
			{
				streamReader.ReadLine();
			}
		}

		public ILegacyTranslationMemorySetup GetSetup()
		{
			TMXReaderSettings tMXReaderSettings = new TMXReaderSettings();
			tMXReaderSettings.ValidateAgainstSchema = false;
			tMXReaderSettings.ResolveNeutralCultures = true;
			ScanResult scanResult = new ScanResult();
			using (TMXReader tMXReader = new TMXReader(_tmxFilePath, tMXReaderSettings))
			{
				bool flag = false;
				int num = 0;
				Event @event;
				while ((@event = tMXReader.Read()) != null && !flag)
				{
					TMXStartOfInputEvent tMXHeader;
					TUEvent tUEvent;
					if ((tMXHeader = (@event as TMXStartOfInputEvent)) != null)
					{
						scanResult.TMXHeader = tMXHeader;
					}
					else if ((tUEvent = (@event as TUEvent)) != null)
					{
						UpdateLanguageDirectionCount(tUEvent.TranslationUnit, scanResult);
						num++;
						if (num % 1000 == 0)
						{
							flag |= RaiseScanProgress(num);
						}
					}
				}
				RaiseScanProgress(num);
				scanResult.EncounteredLanguages = tMXReader.EncounteredLanguages;
				if (tMXReaderSettings.Context.FieldDefinitions != null)
				{
					scanResult.FieldDefinitions = tMXReaderSettings.Context.FieldDefinitions;
				}
				scanResult.RawTUs = tMXReader.RawTUsRead;
				scanResult.TotalTUs = num;
			}
			return GetTMXSetupInfo(scanResult);
		}

		private void UpdateLanguageDirectionCount(TranslationUnit tu, ScanResult result)
		{
			LanguagePair languagePair = new LanguagePair(tu.SourceSegment.Culture, tu.TargetSegment.Culture);
			if (result.LanguageDirections.TryGetValue(languagePair, out int value))
			{
				result.LanguageDirections[languagePair] = value + 1;
			}
			else
			{
				result.LanguageDirections.Add(languagePair, 1);
			}
			languagePair = languagePair.Reverse();
			if (result.LanguageDirections.TryGetValue(languagePair, out value))
			{
				result.LanguageDirections[languagePair] = value + 1;
			}
			else
			{
				result.LanguageDirections.Add(languagePair, 1);
			}
		}

		private bool RaiseScanProgress(int read)
		{
			bool result = false;
			if (this.OnScanProgress != null)
			{
				result = this.OnScanProgress(read);
			}
			return result;
		}

		private ILegacyTranslationMemorySetup GetTMXSetupInfo(ScanResult scanResult)
		{
			LegacyTranslationMemorySetup legacyTranslationMemorySetup = new LegacyTranslationMemorySetup();
			legacyTranslationMemorySetup.Name = Path.GetFileNameWithoutExtension(_tmxFilePath);
			List<ILegacyLanguageDirectionData> list = new List<ILegacyLanguageDirectionData>();
			foreach (KeyValuePair<LanguagePair, int> languageDirection in scanResult.LanguageDirections)
			{
				LegacyLanguagePair legacyLanguagePair = new LegacyLanguagePair(new LegacyLanguage(languageDirection.Key.SourceCulture), new LegacyLanguage(languageDirection.Key.TargetCulture));
				legacyLanguagePair.TranslationUnitCount = languageDirection.Value;
				list.Add(legacyLanguagePair);
			}
			legacyTranslationMemorySetup.LanguageDirections = list.ToArray();
			legacyTranslationMemorySetup.Recognizers = scanResult.TMXHeader.BuiltinRecognizers;
			legacyTranslationMemorySetup.Fields = new List<FieldDefinition>();
			legacyTranslationMemorySetup.UsesIdContextMatch = scanResult.TMXHeader.UsesIdContextMatch;
			legacyTranslationMemorySetup.TextContextMatchType = scanResult.TMXHeader.TextContextMatchType;
			legacyTranslationMemorySetup.TokenizerFlags = scanResult.TMXHeader.TokenizerFlags;
			legacyTranslationMemorySetup.WordCountFlags = scanResult.TMXHeader.WordCountFlags;
			foreach (Field fieldDefinition in scanResult.FieldDefinitions)
			{
				legacyTranslationMemorySetup.Fields.Add(new FieldDefinition(fieldDefinition, isReadOnly: false));
			}
			return legacyTranslationMemorySetup;
		}
	}
}
