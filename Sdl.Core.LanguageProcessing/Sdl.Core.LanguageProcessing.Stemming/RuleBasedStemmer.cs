using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using System;

namespace Sdl.Core.LanguageProcessing.Stemming
{
	public class RuleBasedStemmer : IStemmer
	{
		private readonly StemmingRuleSet _rules;

		private readonly LanguageResources _resources;

		private readonly bool _skipSurrogates;

		public string Signature
		{
			get;
		}

		public bool StripsDiacritics
		{
			get
			{
				if (_rules == null)
				{
					return false;
				}
				StemmingRuleSetIterator stemmingRuleSetIterator = new StemmingRuleSetIterator(_rules);
				stemmingRuleSetIterator.First(0);
				while (stemmingRuleSetIterator.Current != null)
				{
					if (stemmingRuleSetIterator.Current.Action == StemmingRule.StemAction.StripDiacritics)
					{
						return true;
					}
					stemmingRuleSetIterator.Next(0);
				}
				return false;
			}
		}

		public RuleBasedStemmer(LanguageResources resources)
		{
			_resources = resources;
			_rules = resources.StemmingRules;
			Signature = "RuleBased" + ((_rules == null) ? "0" : _rules.Version.ToString());
		}

		public RuleBasedStemmer(LanguageResources resources, bool skipSurrogates)
			: this(resources)
		{
			_skipSurrogates = skipSurrogates;
		}

		public string Stem(string word)
		{
			string word2 = word;
			StemInternal(ref word2);
			return word2;
		}

		private static bool ReplaceAffix(ref string word, StemmingRule.StemAction affixType, string affix, string replacement)
		{
			string text = null;
			string text2 = null;
			int length = affix.Length;
			int length2 = word.Length;
			if (affixType == StemmingRule.StemAction.Circumfix)
			{
				if (length - 1 > length2)
				{
					return false;
				}
				int num = affix.IndexOf('|');
				if (num < 0)
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_StemmerErrorInStemmingRule);
				}
				text = affix.Substring(0, num);
				text2 = affix.Substring(num + 1);
			}
			else if (length > length2)
			{
				return false;
			}
			switch (affixType)
			{
			case StemmingRule.StemAction.Prefix:
				if (!word.StartsWith(affix))
				{
					return false;
				}
				word = word.Remove(0, length);
				if (!string.IsNullOrEmpty(replacement))
				{
					word = replacement + word;
				}
				break;
			case StemmingRule.StemAction.Suffix:
				if (!word.EndsWith(affix))
				{
					return false;
				}
				word = word.Remove(length2 - length, length);
				if (!string.IsNullOrEmpty(replacement))
				{
					word += replacement;
				}
				break;
			case StemmingRule.StemAction.Infix:
			{
				int num2 = word.IndexOf(affix, StringComparison.Ordinal);
				if (num2 < 0)
				{
					return false;
				}
				word = word.Remove(num2, length);
				if (!string.IsNullOrEmpty(replacement))
				{
					word = word.Insert(num2, replacement);
				}
				break;
			}
			case StemmingRule.StemAction.PrefixedInfix:
			{
				int num2 = word.IndexOf(affix, StringComparison.Ordinal);
				if (num2 <= 0)
				{
					return false;
				}
				word = word.Remove(num2, length);
				if (!string.IsNullOrEmpty(replacement))
				{
					word = word.Insert(num2, replacement);
				}
				break;
			}
			case StemmingRule.StemAction.ProperInfix:
			{
				int num2 = word.IndexOf(affix, StringComparison.Ordinal);
				if (num2 < 0 || num2 == 0 || num2 == length2 - length)
				{
					return false;
				}
				word = word.Remove(num2, length);
				if (!string.IsNullOrEmpty(replacement))
				{
					word = word.Insert(num2, replacement);
				}
				break;
			}
			case StemmingRule.StemAction.Circumfix:
				if (word.StartsWith(text ?? throw new InvalidOperationException()) && word.EndsWith(text2))
				{
					word = word.Remove(length2 - text2.Length);
					word = word.Remove(text.Length);
					break;
				}
				return false;
			case StemmingRule.StemAction.Form:
				if (word.Equals(affix))
				{
					word = replacement;
					break;
				}
				return false;
			default:
				throw new ArgumentException("Illegal affix type");
			}
			return true;
		}

		private bool ApplyRule(ref string form, int shortestStemLength, bool specialRulesOnly, StemmingRule rule)
		{
			int num = (!string.IsNullOrEmpty(rule.Affix)) ? rule.Affix.Length : 0;
			int length = form.Length;
			int num2 = (!string.IsNullOrEmpty(rule.Replacement)) ? rule.Replacement.Length : 0;
			switch (rule.Action)
			{
			case StemmingRule.StemAction.MapToLower:
				form = form.ToLowerInvariant();
				return true;
			case StemmingRule.StemAction.StripDiacritics:
				form = CharacterProperties.ToBase(form, _skipSurrogates);
				form = StripPeripheralPunctuation(form);
				return true;
			default:
				if (specialRulesOnly)
				{
					return false;
				}
				switch (rule.Action)
				{
				case StemmingRule.StemAction.TestOnBaseWord:
					return _resources.IsStopword(form);
				case StemmingRule.StemAction.DeleteLastDoubleConsonants:
					if (length > 2)
					{
						for (int num4 = length - 1; num4 > 0; num4--)
						{
							if (form[num4] == form[num4 - 1] && !CharacterProperties.IsVowel(form[num4]))
							{
								form = form.Remove(num4);
								break;
							}
						}
					}
					return true;
				case StemmingRule.StemAction.DeleteLastDoubleVowels:
					if (length > 2)
					{
						for (int num3 = length - 1; num3 > 0; num3--)
						{
							if (form[num3] == form[num3 - 1] && CharacterProperties.IsVowel(form[num3]))
							{
								form = form.Remove(num3);
								break;
							}
						}
					}
					return true;
				default:
					if (length < num)
					{
						return false;
					}
					if (length - num + num2 < shortestStemLength)
					{
						return false;
					}
					return ReplaceAffix(ref form, rule.Action, rule.Affix, rule.Replacement);
				}
			}
		}

		private void BruteForceStem(ref string word)
		{
			word = CharacterProperties.ToBase(word.ToLowerInvariant());
			word = StripPeripheralPunctuation(word);
			int num = word.Length;
			if (_rules != null && _resources.IsStopword(word))
			{
				return;
			}
			for (int num2 = num - 1; num2 > 2; num2--)
			{
				if (word[num2] == word[num2 - 1] && !CharacterProperties.IsVowel(word[num2]))
				{
					word = word.Remove(num2, 1);
					num--;
					break;
				}
			}
			int num3 = Math.Min(num / 3, 3);
			if (num > 3 && (num - num3) % 2 != 0)
			{
				num3++;
			}
			if (num >= 3 && num - num3 < 3)
			{
				num3 = num - 3;
			}
			word = word.Remove(num - num3, num3);
		}

		private static string StripPeripheralPunctuation(string form)
		{
			if (string.IsNullOrEmpty(form))
			{
				return form;
			}
			int i = 0;
			int length;
			for (length = form.Length; i < length && char.IsPunctuation(form, i); i++)
			{
			}
			if (i == length)
			{
				return form;
			}
			int num = length - 1;
			while (num > i && char.IsPunctuation(form, num))
			{
				num--;
			}
			if (i > 0 || num < length - 1)
			{
				return form.Substring(i, num - i + 1);
			}
			return form;
		}

		private void StemInternal(ref string word)
		{
			if (_rules == null || _rules.Count == 0)
			{
				BruteForceStem(ref word);
				return;
			}
			bool flag = true;
			bool specialRulesOnly = word.Length < _rules.MinimumWordLength;
			int num = 0;
			StemmingRuleSetIterator stemmingRuleSetIterator = new StemmingRuleSetIterator(_rules);
			stemmingRuleSetIterator.First(0);
			int shortestStemLength = Math.Max(_rules.MinimumStemLength, word.Length * _rules.MinimumStemPercentage / 100);
			while (flag)
			{
				StemmingRule current = stemmingRuleSetIterator.Current;
				if (num > _rules.MaximumRuleApplications || current == null)
				{
					flag = false;
					continue;
				}
				StemmingRule.StemContinuation stemContinuation;
				if (ApplyRule(ref word, shortestStemLength, specialRulesOnly, current))
				{
					num++;
					stemContinuation = current.ContinuationOnSuccess;
				}
				else
				{
					stemContinuation = current.ContinuationOnFail;
				}
				switch (stemContinuation)
				{
				case StemmingRule.StemContinuation.Continue:
					stemmingRuleSetIterator.Next(current.ContinuationPriority);
					break;
				case StemmingRule.StemContinuation.Restart:
					stemmingRuleSetIterator.First(0);
					break;
				case StemmingRule.StemContinuation.Stop:
					flag = false;
					break;
				default:
					flag = false;
					break;
				}
			}
		}
	}
}
