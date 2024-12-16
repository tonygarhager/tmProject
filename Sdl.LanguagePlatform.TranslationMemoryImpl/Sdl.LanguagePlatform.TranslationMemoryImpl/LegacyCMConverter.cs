using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class LegacyCMConverter
	{
		private readonly EmbeddedResourceDataAccessor _oldResourceAccessor;

		private const string ProcessedParm = "JACMConversion";

		internal static bool SkipCmConversionChecks;

		public LegacyCMConverter()
		{
			_oldResourceAccessor = new EmbeddedResourceDataAccessor(Assembly.GetExecutingAssembly(), "Sdl.LanguagePlatform.NLP.oldresources");
		}

		private static long GetSegmentHash(AbstractAnnotatedSegment s, AnnotatedTranslationMemory tm)
		{
			if (!tm.Tm.UsesLegacyHashes)
			{
				return s.StrictHash;
			}
			return s.Hash;
		}

		public bool ConvertCMInfo(AnnotatedTranslationMemory tm, List<LanguageResource> tmResources, ResourceManager resourceManager, CallContext context, IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken, bool? skipCMConversionChecks)
		{
			FileContainer fileContainer = context.Container as FileContainer;
			if (!skipCMConversionChecks.HasValue)
			{
				skipCMConversionChecks = SkipCmConversionChecks;
			}
			string text = null;
			if (fileContainer != null && !skipCMConversionChecks.Value)
			{
				if (!string.IsNullOrEmpty(context.Storage.GetParameter("JACMConversion")))
				{
					return false;
				}
				string path = fileContainer.Path;
				text = new SQLiteConnectionStringBuilder
				{
					DataSource = path.Replace('\\', '/')
				}.ToString();
				if (!tm.Tm.CanReportReindexRequired || !tm.Tm.UsesLegacyHashes)
				{
					throw new Exception("The converter should only be used on 8.09 schemas");
				}
			}
			TranslationMemoryProgress translationMemoryProgress = new TranslationMemoryProgress();
			translationMemoryProgress.MaxCount = resourceManager.GetTuCount(tm.Tm.ResourceId);
			translationMemoryProgress.CurrentStage = 1;
			translationMemoryProgress.TotalStages = 2;
			bool flag = NeedsConverting(tm.Tm.LanguageDirection.SourceCulture);
			bool flag2 = NeedsConverting(tm.Tm.LanguageDirection.TargetCulture);
			if (!flag && !flag2)
			{
				return false;
			}
			flag = true;
			flag2 = true;
			CompositeResourceDataAccessor compositeResourceDataAccessor = new CompositeResourceDataAccessor(addDefaultAccessor: false);
			compositeResourceDataAccessor.Add(_oldResourceAccessor);
			if (tmResources.Count > 0)
			{
				CachedResourceDataAccessor racc = new CachedResourceDataAccessor(tmResources);
				compositeResourceDataAccessor.Insert(0, racc);
			}
			LanguageResources resources = new LanguageResources(tm.Tm.LanguageDirection.SourceCulture, compositeResourceDataAccessor);
			LanguageResources resources2 = new LanguageResources(tm.Tm.LanguageDirection.TargetCulture, compositeResourceDataAccessor);
			LanguageTools oldtools = new LanguageTools(resources, tm.Tm.Recognizers, tm.Tm.TokenizerFlags, useAlternateStemmers: false, normalizeCharWidths: false);
			LanguageTools oldtools2 = new LanguageTools(resources2, tm.Tm.Recognizers, tm.Tm.TokenizerFlags, useAlternateStemmers: false, normalizeCharWidths: false);
			TokenizerSetup tokenizerSetup = TokenizerSetupFactory.Create(tm.Tm.LanguageDirection.SourceCulture);
			tokenizerSetup.CreateWhitespaceTokens = true;
			tokenizerSetup.BuiltinRecognizers = tm.Tm.Recognizers;
			tokenizerSetup.TokenizerFlags = tm.Tm.TokenizerFlags;
			Tokenizer tokenizer = new Tokenizer(tokenizerSetup);
			TokenizerSetup tokenizerSetup2 = TokenizerSetupFactory.Create(tm.Tm.LanguageDirection.TargetCulture);
			tokenizerSetup2.CreateWhitespaceTokens = true;
			tokenizerSetup2.BuiltinRecognizers = tm.Tm.Recognizers;
			tokenizerSetup2.TokenizerFlags = tm.Tm.TokenizerFlags;
			Tokenizer tokenizer2 = new Tokenizer(tokenizerSetup2);
			Dictionary<long, long> dictionary = new Dictionary<long, long>();
			Dictionary<long, long> dictionary2 = new Dictionary<long, long>();
			RegularIterator iter = new RegularIterator();
			long num = 0L;
			int num2 = 0;
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return false;
				}
				if (num2 >= 100)
				{
					progress?.Report(translationMemoryProgress);
					num2 -= 100;
				}
				List<TranslationUnit> translationUnits = resourceManager.GetTranslationUnits(tm.Tm.ResourceId, iter);
				if (translationUnits == null || translationUnits.Count == 0)
				{
					break;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TranslationUnit item in translationUnits)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(item.ResourceId.Id);
				}
				Dictionary<long, TuHashInfo> dictionary3 = new Dictionary<long, TuHashInfo>();
				if (text != null)
				{
					using (SQLiteConnection sQLiteConnection = new SQLiteConnection(text))
					{
						sQLiteConnection.Open();
						using (SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT id, source_hash, target_hash from translation_units where id in (" + stringBuilder?.ToString() + ")", sQLiteConnection))
						{
							using (SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
							{
								while (sQLiteDataReader.Read())
								{
									TuHashInfo tuHashInfo = new TuHashInfo();
									long @int = sQLiteDataReader.GetInt64(0);
									tuHashInfo.SourceHash = sQLiteDataReader.GetInt64(1);
									tuHashInfo.TargetHash = sQLiteDataReader.GetInt64(2);
									dictionary3.Add(@int, tuHashInfo);
								}
							}
						}
					}
				}
				foreach (TranslationUnit item2 in translationUnits)
				{
					if (flag)
					{
						if (!GetOldTokensAndHash(tokenizer, oldtools, item2.SourceSegment, out long hash))
						{
							continue;
						}
						long segmentHash = GetSegmentHash(new AnnotatedSegment(tm, item2.SourceSegment, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: false), tm);
						if (text != null && segmentHash != dictionary3[item2.ResourceId.Id].SourceHash)
						{
							continue;
						}
						if (!dictionary.ContainsKey(hash))
						{
							dictionary.Add(hash, segmentHash);
						}
					}
					if (flag2 && GetOldTokensAndHash(tokenizer2, oldtools2, item2.TargetSegment, out long hash2))
					{
						long segmentHash2 = GetSegmentHash(new AnnotatedSegment(tm, item2.TargetSegment, isTargetSegment: true, keepTokens: false, keepPeripheralWhitespace: false), tm);
						if ((text == null || segmentHash2 == dictionary3[item2.ResourceId.Id].TargetHash) && !dictionary2.ContainsKey(hash2))
						{
							dictionary2.Add(hash2, segmentHash2);
						}
					}
				}
				num += translationUnits.Count;
				translationMemoryProgress.Count += translationUnits.Count;
				num2 += translationUnits.Count;
			}
			iter = new RegularIterator();
			num = 0L;
			translationMemoryProgress.CurrentStage = 2;
			translationMemoryProgress.Count = 0;
			num2 = 0;
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return false;
				}
				if (num2 >= 100)
				{
					progress?.Report(translationMemoryProgress);
					num2 -= 100;
				}
				List<TranslationUnit> translationUnits2 = resourceManager.GetTranslationUnits(tm.Tm.ResourceId, iter);
				if (translationUnits2 == null || translationUnits2.Count == 0)
				{
					break;
				}
				num += translationUnits2.Count;
				translationMemoryProgress.Count += translationUnits2.Count;
				num2 += translationUnits2.Count;
				List<TranslationUnit> list = new List<TranslationUnit>();
				foreach (TranslationUnit item3 in translationUnits2)
				{
					if (item3.Contexts?.Values != null && item3.Contexts.Values.Count != 0)
					{
						TuContexts tuContexts = new TuContexts();
						bool flag3 = false;
						foreach (TuContext value3 in item3.Contexts.Values)
						{
							tuContexts.Add(value3);
							if (value3.Context1 != 0L || value3.Context2 != 0L)
							{
								TuContext tuContext = new TuContext(value3.Context1, value3.Context2);
								bool flag4 = false;
								if (dictionary.TryGetValue(value3.Context1, out long value))
								{
									tuContext.Context1 = value;
									flag3 = true;
									flag4 = true;
								}
								if (dictionary2.TryGetValue(value3.Context2, out long value2))
								{
									tuContext.Context2 = value2;
									flag3 = true;
									flag4 = true;
								}
								if (flag4)
								{
									tuContexts.Add(tuContext);
								}
							}
						}
						if (flag3)
						{
							item3.Contexts = tuContexts;
						}
						list.Add(item3);
					}
				}
				resourceManager.UpdateTranslationUnitsInternal(tm.Tm.ResourceId, list, null, null, setMetadata: false);
				Console.WriteLine(num);
			}
			context.Storage.SetParameter("JACMConversion", "true");
			return true;
		}

		private static bool GetOldTokensAndHash(Tokenizer tokenizer, LanguageTools oldtools, Segment segment, out long hash)
		{
			hash = 0L;
			List<Token> tokens = tokenizer.GetTokens(segment, allowTokenBundles: false, enhancedAsian: false);
			if (tokens == null)
			{
				return false;
			}
			segment.Tokens = tokens;
			List<SegmentRange> positionTokenAssociation = null;
			string s = oldtools.ComputeIdentityString(segment, LanguageTools.TokenToFeatureMappingMode.Stem, ref positionTokenAssociation);
			if (!CultureInfoExtensions.UseBlankAsWordSeparator(segment.Culture))
			{
				s = ComputeIdentityStringOldJAZH(segment, LanguageTools.TokenToFeatureMappingMode.Stem, ref positionTokenAssociation);
			}
			hash = Hash.GetHashCodeLong(s);
			return true;
		}

		private static bool NeedsConverting(CultureInfo ci)
		{
			switch (ci.TwoLetterISOLanguageName.ToLowerInvariant())
			{
			case "ja":
			case "zh":
				return true;
			default:
				return false;
			}
		}

		public static string ComputeIdentityStringOldJAZH(Segment segment, LanguageTools.TokenToFeatureMappingMode mode, ref List<SegmentRange> positionTokenAssociation)
		{
			bool flag = positionTokenAssociation != null;
			if (flag)
			{
				positionTokenAssociation.Clear();
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token token in segment.Tokens)
			{
				string text = null;
				bool flag2 = true;
				switch (token.Type)
				{
				case TokenType.Word:
				case TokenType.Uri:
				{
					SimpleToken simpleToken2 = token as SimpleToken;
					if (simpleToken2 != null)
					{
						text = ((mode != 0) ? simpleToken2.Text.ToLowerInvariant() : (simpleToken2.Stem ?? simpleToken2.Text.ToLowerInvariant()));
					}
					break;
				}
				case TokenType.OtherTextPlaceable:
				{
					SimpleToken simpleToken = token as SimpleToken;
					if (simpleToken != null)
					{
						text = ((!simpleToken.IsSubstitutable) ? ((mode != 0) ? simpleToken.Text.ToLowerInvariant() : (simpleToken.Stem ?? simpleToken.Text.ToLowerInvariant())) : ((mode == LanguageTools.TokenToFeatureMappingMode.Stem) ? new string((char)(61696 + token.Type), 1) : token.Text.ToLowerInvariant()));
					}
					break;
				}
				case TokenType.CharSequence:
					text = token.Text.ToLowerInvariant();
					flag2 = false;
					break;
				case TokenType.Abbreviation:
					text = token.Text.ToLowerInvariant();
					break;
				case TokenType.Date:
				case TokenType.Time:
				case TokenType.Variable:
				case TokenType.Number:
				case TokenType.Measurement:
				case TokenType.Acronym:
				case TokenType.UserDefined:
				case TokenType.AlphaNumeric:
					text = ((mode == LanguageTools.TokenToFeatureMappingMode.Stem) ? new string((char)(61696 + token.Type), 1) : token.Text.ToLowerInvariant());
					break;
				case TokenType.GeneralPunctuation:
				case TokenType.OpeningPunctuation:
				case TokenType.ClosingPunctuation:
				case TokenType.Whitespace:
				case TokenType.Tag:
					continue;
				}
				if (text != null)
				{
					stringBuilder.Append(text);
					if (flag)
					{
						for (int i = 0; i < text.Length; i++)
						{
							if (flag2)
							{
								positionTokenAssociation.Add(token.Span);
							}
							else
							{
								positionTokenAssociation.Add(new SegmentRange(token.Span.From.Index, token.Span.From.Position + i, token.Span.From.Position + i));
							}
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
