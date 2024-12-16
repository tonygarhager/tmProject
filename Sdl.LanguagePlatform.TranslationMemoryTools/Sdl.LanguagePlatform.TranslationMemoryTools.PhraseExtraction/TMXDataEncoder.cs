using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.IO.TMX;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryTools.PhraseExtraction
{
	public class TMXDataEncoder
	{
		public class Settings
		{
			public bool ComputePlainFile;

			public bool ComputeTokenFrequencies;

			public int MaxTUs;

			public int MaxSegmentTokenCount;

			public bool IgnoreRegionCodeMismatch;
		}

		private class TokenRestorer
		{
			private readonly bool _isJa;

			public List<Token> Tokens
			{
				get;
			} = new List<Token>();


			public TokenRestorer(CultureInfo ci)
			{
				_isJa = (string.CompareOrdinal("ja", ci.Name.Substring(0, 2).ToLower()) == 0);
			}

			public void ProcessTokens(List<Token> tokens)
			{
				Tokens.Clear();
				tokens.ForEach(AddToken);
			}

			private static bool IsJoinable(Token t)
			{
				TokenType type = t.Type;
				if ((uint)(type - 1) <= 2u || type == TokenType.Acronym || type == TokenType.OtherTextPlaceable)
				{
					return true;
				}
				return false;
			}

			private static bool IsJoiningApostrophe(Token t)
			{
				if (t.Type != TokenType.GeneralPunctuation || t.Text.Length != 1)
				{
					return false;
				}
				return CharacterProperties.IsApostrophe(t.Text[0]);
			}

			private void AddToken(Token t)
			{
				if (_isJa && t.Type == TokenType.GeneralPunctuation && Tokens.Count > 0 && t.Text.Length == 1 && IsJaLongVowelMarker(t.Text[0]))
				{
					SimpleToken item = new SimpleToken(Tokens[Tokens.Count - 1].Text + t.Text);
					Tokens.RemoveAt(Tokens.Count - 1);
					Tokens.Add(item);
				}
				else if (Tokens.Count < 2)
				{
					Tokens.Add(t);
				}
				else if (!IsJoiningApostrophe(Tokens[Tokens.Count - 1]) || !IsJoinable(Tokens[Tokens.Count - 2]) || !IsJoinable(t))
				{
					Tokens.Add(t);
				}
				else
				{
					SimpleToken item2 = new SimpleToken(Tokens[Tokens.Count - 2].Text + Tokens[Tokens.Count - 1].Text + t.Text);
					Tokens.RemoveAt(Tokens.Count - 1);
					Tokens.RemoveAt(Tokens.Count - 1);
					Tokens.Add(item2);
				}
			}
		}

		public EventHandler<ProgressEventArgs> Progress;

		private const int ReportProgressPeriod = 100;

		private Tokenizer _sourceTokenizer;

		private Tokenizer _targetTokenizer;

		public LanguagePair LanguageDirection
		{
			get;
			private set;
		}

		private void SetupCultures(LanguagePair ld)
		{
			if (ld.SourceCulture == null)
			{
				throw new ArgumentNullException("SourceCulture");
			}
			if (ld.TargetCulture == null)
			{
				throw new ArgumentNullException("TargetCulture");
			}
			LanguageDirection = ld;
			_sourceTokenizer = GetStandardTokenizer(ld.SourceCulture, createWhitespaceTokens: true);
			_targetTokenizer = GetStandardTokenizer(ld.TargetCulture, createWhitespaceTokens: true);
			new LanguageResources(ld.TargetCulture);
		}

		internal static bool IsJaLongVowelMarker(char c)
		{
			if (c != '\u30fc')
			{
				return c == '\uff70';
			}
			return true;
		}

		public DataLocation Encode(string tmxFile, string outputLocationOverride, TextWriter logStream, Settings settings)
		{
			return Encode(tmxFile, null, null, outputLocationOverride, logStream, settings);
		}

		public DataLocation Encode(IList<string> tmxFiles, string outputLocationOverride, TextWriter logStream, Settings settings)
		{
			return Encode(tmxFiles, null, null, outputLocationOverride, logStream, settings);
		}

		public DataLocation Encode(string tmxFile, CultureInfo sourceCulture, CultureInfo targetCulture, string outputLocationOverride, TextWriter logStream)
		{
			return Encode(tmxFile, sourceCulture, targetCulture, outputLocationOverride, logStream, new Settings());
		}

		public DataLocation Encode(IList<string> tmxFiles, CultureInfo sourceCulture, CultureInfo targetCulture, string outputLocationOverride, TextWriter logStream)
		{
			return Encode(tmxFiles, sourceCulture, targetCulture, outputLocationOverride, logStream, new Settings());
		}

		public DataLocation Encode(string tmxFile, CultureInfo sourceCulture, CultureInfo targetCulture, string outputLocationOverride, TextWriter logStream, Settings settings)
		{
			List<string> tmxFiles = new List<string>
			{
				tmxFile
			};
			return Encode(tmxFiles, sourceCulture, targetCulture, outputLocationOverride, logStream, settings);
		}

		public DataLocation Encode(IList<string> tmxFiles, CultureInfo sourceCulture, CultureInfo targetCulture, string outputLocationOverride, TextWriter logStream, Settings settings)
		{
			if (tmxFiles == null || tmxFiles.Count == 0)
			{
				throw new ArgumentException("No input files specified");
			}
			if (tmxFiles.Any((string x) => x == null))
			{
				throw new ArgumentException("At least one of the specified input files is null");
			}
			if (tmxFiles.Any((string x) => !File.Exists(x)))
			{
				throw new ArgumentException("At least one of the specified input files does not exist");
			}
			string text = tmxFiles[0];
			FileInfo fileInfo = new FileInfo(text);
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException(text);
			}
			if (settings == null)
			{
				settings = new Settings();
			}
			LanguagePair ld = (sourceCulture == null || targetCulture == null) ? GetLanguageDirection(text) : new LanguagePair(sourceCulture, targetCulture);
			SetupCultures(ld);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
			if (fileInfo.Extension.Equals(".gz", StringComparison.OrdinalIgnoreCase))
			{
				fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
			}
			if (string.IsNullOrEmpty(outputLocationOverride))
			{
				outputLocationOverride = fileInfo.DirectoryName;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(outputLocationOverride + Path.DirectorySeparatorChar.ToString() + fileNameWithoutExtension);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
				directoryInfo.Refresh();
			}
			DataLocation dataLocation = new DataLocation(directoryInfo.FullName);
			VocabularyFile vocabularyFile = new VocabularyFile(dataLocation, LanguageDirection.SourceCulture);
			VocabularyFile vocabularyFile2 = new VocabularyFile(dataLocation, LanguageDirection.TargetCulture);
			TokenFileWriter tokenFileWriter = new TokenFileWriter(dataLocation, LanguageDirection.SourceCulture);
			tokenFileWriter.Create();
			TokenFileWriter tokenFileWriter2 = new TokenFileWriter(dataLocation, LanguageDirection.TargetCulture);
			tokenFileWriter2.Create();
			PlainFileWriter plainFileWriter = null;
			PlainFileWriter plainFileWriter2 = null;
			if (settings.ComputePlainFile)
			{
				plainFileWriter = new PlainFileWriter(dataLocation, LanguageDirection.SourceCulture);
				plainFileWriter.Create();
				plainFileWriter2 = new PlainFileWriter(dataLocation, LanguageDirection.TargetCulture);
				plainFileWriter2.Create();
			}
			FrequencyFileWriter frequencyFileWriter = null;
			FrequencyFileWriter frequencyFileWriter2 = null;
			if (settings.ComputeTokenFrequencies)
			{
				frequencyFileWriter = new FrequencyFileWriter(dataLocation, LanguageDirection.SourceCulture);
				frequencyFileWriter2 = new FrequencyFileWriter(dataLocation, LanguageDirection.TargetCulture);
			}
			int num = 0;
			int num2 = 0;
			bool flag = false;
			TokenRestorer tokenRestorer = new TokenRestorer(LanguageDirection.SourceCulture);
			TokenRestorer tokenRestorer2 = new TokenRestorer(LanguageDirection.TargetCulture);
			foreach (string tmxFile in tmxFiles)
			{
				if (flag)
				{
					break;
				}
				using (TMXReader tMXReader = new TMXReader(tmxFile, new TMXReaderSettings(new TUStreamContext(LanguageDirection), validateAgainstSchema: false, resolveNeutralCultures: true, plainText: true)))
				{
					tMXReader.Settings.Context.MayAddNewFields = false;
					tMXReader.Settings.PlainText = true;
					Event @event;
					while ((@event = tMXReader.Read()) != null && !flag)
					{
						TUEvent tUEvent = @event as TUEvent;
						if (tUEvent != null)
						{
							num2++;
							if (num2 % 100 == 0)
							{
								OnProgress(ProgressStage.Encoding, num2);
							}
							if (tUEvent.TranslationUnit.SourceSegment.Elements.Count != 1 || tUEvent.TranslationUnit.TargetSegment.Elements.Count != 1)
							{
								num++;
							}
							else if ((!object.Equals(tUEvent.TranslationUnit.SourceSegment.Culture, LanguageDirection.SourceCulture) || !object.Equals(tUEvent.TranslationUnit.TargetSegment.Culture, LanguageDirection.TargetCulture)) && (!settings.IgnoreRegionCodeMismatch || !CultureInfoExtensions.AreCompatible(tUEvent.TranslationUnit.SourceSegment.Culture, LanguageDirection.SourceCulture) || !CultureInfoExtensions.AreCompatible(tUEvent.TranslationUnit.TargetSegment.Culture, LanguageDirection.TargetCulture)))
							{
								num++;
							}
							else
							{
								tUEvent.TranslationUnit.SourceSegment.Tokens = _sourceTokenizer.GetTokens(tUEvent.TranslationUnit.SourceSegment, enhancedAsian: true);
								tUEvent.TranslationUnit.TargetSegment.Tokens = _targetTokenizer.GetTokens(tUEvent.TranslationUnit.TargetSegment, enhancedAsian: true);
								if (tUEvent.TranslationUnit.SourceSegment.Tokens.Count == 0 || tUEvent.TranslationUnit.TargetSegment.Tokens.Count == 0)
								{
									num++;
								}
								else if (settings.MaxSegmentTokenCount > 0 && (tUEvent.TranslationUnit.SourceSegment.Tokens.Count > settings.MaxSegmentTokenCount || tUEvent.TranslationUnit.TargetSegment.Tokens.Count > settings.MaxSegmentTokenCount))
								{
									num++;
								}
								else
								{
									if (settings.ComputePlainFile)
									{
										plainFileWriter.Write(tUEvent.TranslationUnit.SourceSegment.ToPlain());
										plainFileWriter2.Write(tUEvent.TranslationUnit.TargetSegment.ToPlain());
									}
									tokenRestorer.ProcessTokens(tUEvent.TranslationUnit.SourceSegment.Tokens);
									tokenRestorer2.ProcessTokens(tUEvent.TranslationUnit.TargetSegment.Tokens);
									IntSegment intSegment = GetIntSegment(vocabularyFile, tokenRestorer.Tokens.FindAll((Token t) => t.Type != TokenType.Whitespace));
									IntSegment intSegment2 = GetIntSegment(vocabularyFile2, tokenRestorer2.Tokens.FindAll((Token t) => t.Type != TokenType.Whitespace));
									if (intSegment.Count == 0 || intSegment2.Count == 0)
									{
										num++;
									}
									else
									{
										tokenFileWriter.Write(intSegment);
										tokenFileWriter2.Write(intSegment2);
										if (settings.ComputeTokenFrequencies)
										{
											foreach (int element in intSegment.Elements)
											{
												frequencyFileWriter.Inc(element);
											}
											foreach (int element2 in intSegment2.Elements)
											{
												frequencyFileWriter2.Inc(element2);
											}
										}
										if (num2 % 1000 == 0 && logStream != null)
										{
											logStream.WriteLine($"Encoding {tmxFile}: {num2} TUs read, {num2 - num} processed, {num} skipped");
											logStream.Flush();
										}
										if (settings.MaxTUs > 0 && num2 >= settings.MaxTUs)
										{
											flag = true;
										}
									}
								}
							}
						}
					}
				}
			}
			vocabularyFile.Save();
			vocabularyFile2.Save();
			tokenFileWriter.Close();
			tokenFileWriter2.Close();
			if (settings.ComputePlainFile)
			{
				plainFileWriter.Close();
				plainFileWriter2.Close();
			}
			if (settings.ComputeTokenFrequencies)
			{
				frequencyFileWriter.Save();
				frequencyFileWriter2.Save();
			}
			if (logStream == null)
			{
				return dataLocation;
			}
			logStream.WriteLine($"Finished encoding {text}: {num2} TUs read, {num2 - num} processed, {num} skipped");
			logStream.Flush();
			return dataLocation;
		}

		private static LanguagePair GetLanguageDirection(string tmxFile)
		{
			CultureInfo headerSourceLanguage;
			LanguagePair languageDirection = TMXTools.GetLanguageDirection(tmxFile, out headerSourceLanguage);
			if (languageDirection?.SourceCulture == null || languageDirection.TargetCulture == null)
			{
				throw new LanguagePlatformException(ErrorCode.TMXCannotDetermineLanguageDirection);
			}
			return languageDirection;
		}

		public static Tokenizer GetStandardTokenizer(CultureInfo culture, bool createWhitespaceTokens)
		{
			TokenizerFlags tokenizerFlags = TokenizerFlags.DefaultFlags;
			tokenizerFlags &= ~TokenizerFlags.BreakOnHyphen;
			tokenizerFlags &= ~TokenizerFlags.BreakOnDash;
			tokenizerFlags &= ~TokenizerFlags.BreakOnApostrophe;
			TokenizerSetup tokenizerSetup = TokenizerSetupFactory.Create(culture, BuiltinRecognizers.RecognizeDates | BuiltinRecognizers.RecognizeTimes | BuiltinRecognizers.RecognizeNumbers | BuiltinRecognizers.RecognizeMeasurements, tokenizerFlags);
			tokenizerSetup.CreateWhitespaceTokens = createWhitespaceTokens;
			return new Tokenizer(tokenizerSetup);
		}

		private static IntSegment GetIntSegment(VocabularyFile v, IEnumerable<Token> tokens)
		{
			IntSegment intSegment = new IntSegment();
			foreach (Token token in tokens)
			{
				string tokenString = GetTokenString(token);
				if (tokenString != null)
				{
					int item = v.LookupOrAdd(tokenString);
					intSegment.Elements.Add(item);
				}
			}
			return intSegment;
		}

		public static string GetTokenString(Token t)
		{
			if (t == null)
			{
				return null;
			}
			switch (t.Type)
			{
			case TokenType.Word:
			case TokenType.Abbreviation:
			case TokenType.CharSequence:
			case TokenType.Acronym:
			case TokenType.Uri:
			case TokenType.OtherTextPlaceable:
				return t.Text;
			case TokenType.Date:
				return "{{DAT}}";
			case TokenType.Measurement:
				return "{{MSR}}";
			case TokenType.Number:
				return "{{NUM}}";
			case TokenType.Time:
				return "{{TIM}}";
			case TokenType.GeneralPunctuation:
			case TokenType.OpeningPunctuation:
			case TokenType.ClosingPunctuation:
				return "{{PCT}}";
			case TokenType.Variable:
				return "{{VAR}}";
			default:
				return null;
			}
		}

		private void OnProgress(ProgressStage progressStage, int progressNumber)
		{
			if (Progress != null)
			{
				ProgressEventArgs e = new ProgressEventArgs(progressStage, progressNumber);
				Progress(this, e);
			}
		}
	}
}
