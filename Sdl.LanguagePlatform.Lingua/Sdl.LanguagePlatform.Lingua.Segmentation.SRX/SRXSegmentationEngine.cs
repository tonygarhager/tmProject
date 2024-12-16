using Sdl.Core.LanguageProcessing.Segmentation;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.Lingua.Segmentation.SRX
{
	public class SRXSegmentationEngine : SegmentationEngine
	{
		private struct CompiledRule
		{
			public bool IsBreakRule;

			public Regex BeforeBreak;

			public bool BeforeBreakMatchEof;

			public Regex AfterBreak;

			public bool AfterBreakMatchEof;
		}

		private readonly LanguageRule _rules;

		private List<CompiledRule> _compiledRules;

		private SRXSegmentationEngine(LanguageRule languageRules, CultureInfo culture)
			: base(culture)
		{
			_Culture = culture;
			_rules = (languageRules ?? throw new ArgumentNullException("languageRules"));
			CompileRules();
		}

		private void CompileRules()
		{
			_compiledRules = new List<CompiledRule>();
			foreach (Rule rule in _rules.Rules)
			{
				CompiledRule item = default(CompiledRule);
				item.AfterBreakMatchEof = (item.BeforeBreakMatchEof = false);
				try
				{
					if (!string.IsNullOrEmpty(rule.BeforeBreak))
					{
						item.BeforeBreak = new Regex(TranslateRx(rule.BeforeBreak), RegexOptions.CultureInvariant);
					}
				}
				catch (Exception)
				{
					throw new LanguagePlatformException(ErrorCode.SegmentationInvalidRule, rule.ToString());
				}
				try
				{
					if (!string.IsNullOrEmpty(rule.AfterBreak))
					{
						item.AfterBreak = new Regex(TranslateRx(rule.AfterBreak), RegexOptions.CultureInvariant);
					}
				}
				catch (Exception)
				{
					throw new LanguagePlatformException(ErrorCode.SegmentationInvalidRule, rule.ToString());
				}
				item.IsBreakRule = rule.IsBreak;
				_compiledRules.Add(item);
			}
		}

		private static string TranslateRx(string rx)
		{
			return rx;
		}

		public override Chunk GetNextChunk(string text, int startIndex, bool assumeEof, bool followedByWordBreakTag)
		{
			if (string.IsNullOrEmpty(text) || startIndex >= text.Length)
			{
				return null;
			}
			int neutralPrefixLength = GetNeutralPrefixLength(text, startIndex);
			if (neutralPrefixLength > 0)
			{
				return new Chunk(startIndex, neutralPrefixLength, ChunkType.Whitespace, SegmentationMethod.Whitespace);
			}
			List<int> breakPositions = new List<int>();
			foreach (CompiledRule compiledRule in _compiledRules)
			{
				if (compiledRule.IsBreakRule)
				{
					if (compiledRule.BeforeBreak == null)
					{
						MatchCollection matchCollection = compiledRule.AfterBreak.Matches(text, startIndex);
						foreach (Match item in matchCollection)
						{
							breakPositions.Add(item.Index + item.Length);
						}
						if ((assumeEof | followedByWordBreakTag) && compiledRule.AfterBreakMatchEof)
						{
							breakPositions.Add(text.Length);
						}
					}
					else
					{
						MatchCollection matchCollection2 = compiledRule.BeforeBreak.Matches(text, startIndex);
						foreach (Match item2 in matchCollection2)
						{
							int num = item2.Index + item2.Length;
							bool flag = compiledRule.AfterBreak == null;
							if (!flag)
							{
								Match match3 = compiledRule.AfterBreak.Match(text, num);
								if (match3.Index == num)
								{
									flag = true;
								}
								else if (num == text.Length && compiledRule.AfterBreakMatchEof)
								{
									flag = true;
								}
							}
							if (flag)
							{
								breakPositions.Add(num);
							}
						}
					}
				}
			}
			breakPositions.Sort();
			int bpi = 0;
			while (bpi < breakPositions.Count)
			{
				if ((bpi <= 0 || breakPositions[bpi] != breakPositions[bpi - 1]) && !_compiledRules.Any((CompiledRule r) => !r.IsBreakRule && RuleMatchesPosition(text, breakPositions[bpi], r)))
				{
					return new Chunk(startIndex, breakPositions[bpi] - startIndex, ChunkType.BreakAfter, SegmentationMethod.Rule);
				}
				int num2 = ++bpi;
			}
			if (!assumeEof)
			{
				return null;
			}
			return new Chunk(startIndex, text.Length - startIndex, ChunkType.BreakAfter, SegmentationMethod.EndOfInput);
		}

		private static bool RuleMatchesPosition(string input, int position, CompiledRule rule)
		{
			Regex beforeBreak = rule.BeforeBreak;
			if (beforeBreak == null)
			{
				if (rule.AfterBreak == null)
				{
					return true;
				}
				foreach (Match item in rule.AfterBreak.Matches(input))
				{
					if (item.Index + item.Length == position + 1)
					{
						return true;
					}
				}
				if ((position == 0 || position == input.Length) && rule.AfterBreakMatchEof)
				{
					return true;
				}
			}
			else
			{
				foreach (Match item2 in rule.BeforeBreak.Matches(input))
				{
					if (item2.Index + item2.Length == position)
					{
						if (rule.AfterBreak == null)
						{
							return true;
						}
						if (position == input.Length && rule.AfterBreakMatchEof)
						{
							return true;
						}
						Match match3 = rule.AfterBreak.Match(input, position);
						if (match3.Index == position)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static SRXSegmentationEngine Create(string srxFileName, CultureInfo culture)
		{
			return Create(SRXReader.Read(srxFileName), culture);
		}

		public static SRXSegmentationEngine Create(SRX srxInstance, CultureInfo culture)
		{
			LanguageRule languageRule = SRXReader.FindRules(srxInstance, culture);
			if (languageRule == null)
			{
				throw new LanguagePlatformException(ErrorCode.SegmentationNoRulesForLanguage, culture.Name);
			}
			return new SRXSegmentationEngine(languageRule, culture);
		}
	}
}
