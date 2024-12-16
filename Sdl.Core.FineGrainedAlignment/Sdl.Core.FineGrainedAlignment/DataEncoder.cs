using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class DataEncoder
	{
		private readonly CultureInfo _srcCulture;

		private readonly CultureInfo _trgCulture;

		private LanguageTools _targetTools;

		private CompositeResourceDataAccessor _LanguageResourceAccessor;

		private LanguageResources _targetResources;

		private LanguageResources _sourceResources;

		private LanguageTools _sourceTools;

		private readonly bool _stemming;

		private readonly bool _forTrainedModel;

		private CompositeResourceDataAccessor Accessor
		{
			get
			{
				if (_LanguageResourceAccessor == null)
				{
					_LanguageResourceAccessor = new CompositeResourceDataAccessor(addDefaultAccessor: true);
				}
				return _LanguageResourceAccessor;
			}
		}

		private LanguageResources SourceResources
		{
			get
			{
				if (_sourceResources == null)
				{
					_sourceResources = new LanguageResources(_srcCulture, Accessor);
				}
				return _sourceResources;
			}
		}

		private LanguageResources TargetResources
		{
			get
			{
				if (_targetResources == null)
				{
					_targetResources = new LanguageResources(_trgCulture, Accessor);
				}
				return _targetResources;
			}
		}

		protected LanguageTools SourceTools
		{
			get
			{
				if (_sourceTools == null)
				{
					_sourceTools = new LanguageTools(SourceResources, BuiltinRecognizers.RecognizeAll, TokenizerFlags.DefaultFlags, useAlternateStemmers: true, normalizeCharWidths: true);
				}
				return _sourceTools;
			}
		}

		protected LanguageTools TargetTools
		{
			get
			{
				if (_targetTools == null)
				{
					_targetTools = new LanguageTools(TargetResources, BuiltinRecognizers.RecognizeAll, TokenizerFlags.DefaultFlags, useAlternateStemmers: true, normalizeCharWidths: true);
				}
				return _targetTools;
			}
		}

		internal DataEncoder(CultureInfo srcCulture, CultureInfo trgCulture, bool forTrainedModel, bool stemming)
		{
			if (srcCulture == null)
			{
				throw new ArgumentNullException("srcCulture");
			}
			if (trgCulture == null)
			{
				throw new ArgumentNullException("trgCulture");
			}
			_srcCulture = srcCulture;
			_trgCulture = trgCulture;
			_forTrainedModel = forTrainedModel;
			_stemming = stemming;
		}

		internal void GetUniqueTokenStrings(List<Token> tokens, HashSet<string> uniqueStrings, bool forTraining, bool target)
		{
			EnsureStemmed(tokens, target);
			foreach (Token token in tokens)
			{
				string tokenString = GetTokenString(token, forTraining);
				if (!string.IsNullOrEmpty(tokenString))
				{
					uniqueStrings.Add(tokenString);
				}
			}
		}

		internal void GetTokenStrings(List<Token> tokens, List<string> strings, bool forTraining, bool target)
		{
			EnsureStemmed(tokens, target);
			foreach (Token token in tokens)
			{
				string tokenString = GetTokenString(token, forTraining);
				if (!string.IsNullOrEmpty(tokenString))
				{
					strings.Add(tokenString);
				}
			}
		}

		private void EnsureStemmed(List<Token> tokens, bool target)
		{
			if (_stemming)
			{
				Segment segment = new Segment(target ? _trgCulture : _srcCulture);
				segment.Tokens = tokens;
				if (target)
				{
					TargetTools.Stem(segment);
				}
				else
				{
					SourceTools.Stem(segment);
				}
			}
		}

		private string GetTokenString(Token t, bool forTraining)
		{
			if (t == null)
			{
				return null;
			}
			switch (t.Type)
			{
			case TokenType.Word:
				if (_stemming)
				{
					string text = (t as SimpleToken)?.Stem;
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
				return t.Text;
			case TokenType.Abbreviation:
			case TokenType.CharSequence:
			case TokenType.Variable:
			case TokenType.Acronym:
			case TokenType.Uri:
			case TokenType.OtherTextPlaceable:
			case TokenType.AlphaNumeric:
				return t.Text;
			case TokenType.Date:
				if (!forTraining)
				{
					return "{{DAT}}";
				}
				return null;
			case TokenType.Measurement:
				if (!forTraining)
				{
					return "{{MSR}}";
				}
				return null;
			case TokenType.Number:
				if (!forTraining)
				{
					return "{{NUM}}";
				}
				return null;
			case TokenType.Time:
				if (!forTraining)
				{
					return "{{TIM}}";
				}
				return null;
			case TokenType.GeneralPunctuation:
			case TokenType.OpeningPunctuation:
			case TokenType.ClosingPunctuation:
				if (!forTraining)
				{
					return "{{PCT}}";
				}
				return t.Text;
			default:
				return null;
			}
		}

		public bool Encode(IAlignableContentPair pair, DbVocabularyFile srcVocab, DbVocabularyFile trgVocab, List<short> srcTokenIndicesUsed, List<short> trgTokenIndicesUsed, out IntSegment srcIntSegment, out IntSegment trgIntSegment, List<bool> srcStopWords, List<bool> trgStopWords, List<int> srcFreq, List<int> trgFreq)
		{
			return Encode(pair, srcVocab, trgVocab, srcTokenIndicesUsed, trgTokenIndicesUsed, out srcIntSegment, out trgIntSegment, incremental: true, srcStopWords, trgStopWords, srcFreq, trgFreq);
		}

		public bool Encode(IAlignableContentPair pair, DbVocabularyFile srcVocab, DbVocabularyFile trgVocab, List<short> srcTokenIndicesUsed, List<short> trgTokenIndicesUsed, out IntSegment srcIntSegment, out IntSegment trgIntSegment, bool incremental, List<bool> srcStopWords, List<bool> trgStopWords, List<int> srcFreq, List<int> trgFreq)
		{
			if (pair.SourceTokens == null)
			{
				throw new Exception("Untokenized input");
			}
			if (pair.TargetTokens == null)
			{
				throw new Exception("Untokenized input");
			}
			srcIntSegment = null;
			trgIntSegment = null;
			if (pair.SourceTokens.Count == 0 || pair.TargetTokens.Count == 0)
			{
				return false;
			}
			if (_stemming)
			{
				EnsureStemmed(pair.SourceTokens, target: false);
				EnsureStemmed(pair.TargetTokens, target: true);
			}
			srcIntSegment = GetIntSegment(srcVocab, pair.SourceTokens, srcTokenIndicesUsed, incremental, _forTrainedModel);
			trgIntSegment = GetIntSegment(trgVocab, pair.TargetTokens, trgTokenIndicesUsed, incremental, _forTrainedModel);
			if (srcIntSegment.Count == 0 || trgIntSegment.Count == 0)
			{
				return false;
			}
			if (srcStopWords != null)
			{
				foreach (short item3 in srcTokenIndicesUsed)
				{
					SimpleToken simpleToken = pair.SourceTokens[item3] as SimpleToken;
					bool item = false;
					if (simpleToken != null)
					{
						item = (simpleToken.IsStopword || simpleToken.IsPunctuation);
					}
					srcStopWords.Add(item);
				}
			}
			if (trgStopWords != null)
			{
				foreach (short item4 in trgTokenIndicesUsed)
				{
					SimpleToken simpleToken2 = pair.TargetTokens[item4] as SimpleToken;
					bool item2 = false;
					if (simpleToken2 != null)
					{
						item2 = (simpleToken2.IsStopword || simpleToken2.IsPunctuation);
					}
					trgStopWords.Add(item2);
				}
			}
			return true;
		}

		public bool Encode(IAlignableContentPair pair, bool incremental, DbVocabularyFile srcVocab, DbVocabularyFile trgVocab, TokenFileWriter2 srcSentences, TokenFileWriter2 trgSentences, StreamWriter srcSentenceRawTokensWriter, StreamWriter trgSentenceRawTokensWriter, IOccurrenceCounter srcFreq, IOccurrenceCounter trgFreq)
		{
			List<short> list = new List<short>();
			List<short> list2 = new List<short>();
			if (!Encode(pair, srcVocab, trgVocab, list, list2, out IntSegment srcIntSegment, out IntSegment trgIntSegment, incremental: false, null, null, null, null))
			{
				return false;
			}
			srcSentences.Write(srcIntSegment);
			trgSentences.Write(trgIntSegment);
			srcSentenceRawTokensWriter?.WriteLine(ListToLine(list));
			trgSentenceRawTokensWriter?.WriteLine(ListToLine(list2));
			if (srcFreq != null)
			{
				IntSegment intSegment = new IntSegment(srcIntSegment.Elements);
				intSegment.Uniq();
				foreach (int element in intSegment.Elements)
				{
					srcFreq.Inc(element);
				}
			}
			if (trgFreq != null)
			{
				IntSegment intSegment2 = new IntSegment(trgIntSegment.Elements);
				intSegment2.Uniq();
				foreach (int element2 in intSegment2.Elements)
				{
					trgFreq.Inc(element2);
				}
			}
			return true;
		}

		private IntSegment GetIntSegment(DbVocabularyFile v, List<Token> tokens, List<short> indices, bool incremental, bool forTrainedModel)
		{
			IntSegment intSegment = new IntSegment();
			HashSet<int> hashSet = new HashSet<int>();
			for (short num = 0; num < tokens.Count; num = (short)(num + 1))
			{
				Token t = tokens[num];
				string tokenString = GetTokenString(t, forTrainedModel);
				if (tokenString != null)
				{
					int num2 = 0;
					num2 = ((!incremental) ? v.LookupOrAdd(tokenString) : v.Lookup(tokenString));
					intSegment.Elements.Add(num2);
					indices?.Add(num);
					if (num2 != -1)
					{
						if (!hashSet.Contains(num2))
						{
							v.IncFrequency(num2);
						}
						hashSet.Add(num2);
					}
				}
			}
			return intSegment;
		}

		internal static string ListToLine(List<short> list)
		{
			return string.Join(",", list);
		}

		internal static List<short> LineToList(string line)
		{
			List<short> list = new List<short>();
			string[] array = line.Split(',');
			string[] array2 = array;
			foreach (string s in array2)
			{
				list.Add(short.Parse(s));
			}
			return list;
		}
	}
}
