using System;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Stemming
{
	internal class StemmingRuleParser
	{
		private int _ruleP;

		private string _rule;

		private readonly StemmingRuleSet _ruleSet;

		public StemmingRuleParser(StemmingRuleSet ruleset)
		{
			_ruleSet = ruleset;
		}

		public void Add(string rule)
		{
			_rule = rule;
			_ruleP = 0;
			StemmingRule stemmingRule = new StemmingRule();
			int num = 0;
			int length = _rule.Length;
			while (num != 99)
			{
				while (_ruleP < length && char.IsWhiteSpace(_rule, _ruleP))
				{
					_ruleP++;
				}
				switch (num)
				{
				case 0:
					switch (GetIdentifier().ToLowerInvariant())
					{
					case "version":
						num = 15;
						break;
					case "replace":
						num = 6;
						break;
					case "stripdiacritics":
						num = 1;
						stemmingRule.Action = StemmingRule.StemAction.StripDiacritics;
						break;
					case "tolower":
						stemmingRule.Action = StemmingRule.StemAction.MapToLower;
						num = 1;
						break;
					case "deletelastdoublevowels":
						stemmingRule.Action = StemmingRule.StemAction.DeleteLastDoubleVowels;
						num = 1;
						break;
					case "deletelastdoubleconsonants":
						stemmingRule.Action = StemmingRule.StemAction.DeleteLastDoubleConsonants;
						num = 1;
						break;
					case "testonbaseword":
						stemmingRule.Action = StemmingRule.StemAction.TestOnBaseWord;
						num = 1;
						break;
					case "set":
						num = 12;
						break;
					default:
						throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationIllegalKeywordInRule);
					}
					break;
				case 1:
					Expect("priority");
					num = 2;
					break;
				case 2:
					stemmingRule.Priority = GetNumber();
					num = 3;
					break;
				case 3:
					Expect("and");
					num = 4;
					break;
				case 4:
					num = 10;
					switch (GetIdentifier().ToLowerInvariant())
					{
					case "continue":
						stemmingRule.ContinuationOnSuccess = StemmingRule.StemContinuation.Continue;
						break;
					case "restart":
						stemmingRule.ContinuationOnSuccess = StemmingRule.StemContinuation.Restart;
						break;
					case "stop":
						stemmingRule.ContinuationOnSuccess = StemmingRule.StemContinuation.Stop;
						num = 5;
						break;
					default:
						throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationIllegalContinuation);
					}
					stemmingRule.ContinuationOnFail = StemmingRule.StemContinuation.Continue;
					stemmingRule.ContinuationPriority = 0;
					break;
				case 5:
					if (_rule[_ruleP] == ';')
					{
						num = 99;
						break;
					}
					throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationTrailingJunk);
				case 6:
					switch (GetIdentifier().ToLowerInvariant())
					{
					case "prefix":
						stemmingRule.Action = StemmingRule.StemAction.Prefix;
						break;
					case "suffix":
						stemmingRule.Action = StemmingRule.StemAction.Suffix;
						break;
					case "infix":
						stemmingRule.Action = StemmingRule.StemAction.Infix;
						break;
					case "properinfix":
						stemmingRule.Action = StemmingRule.StemAction.ProperInfix;
						break;
					case "circumfix":
						stemmingRule.Action = StemmingRule.StemAction.Circumfix;
						break;
					case "form":
						stemmingRule.Action = StemmingRule.StemAction.Form;
						break;
					case "prefixedinfix":
						stemmingRule.Action = StemmingRule.StemAction.PrefixedInfix;
						break;
					default:
						throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationUnknownRuleType);
					}
					num = 7;
					break;
				case 7:
					stemmingRule.Affix = GetQuotedString();
					num = 8;
					break;
				case 8:
					Expect("with");
					num = 9;
					break;
				case 9:
					stemmingRule.Replacement = GetQuotedString();
					num = 1;
					break;
				case 10:
					if (_rule[_ruleP] == ';')
					{
						num = 5;
						break;
					}
					Expect("at");
					num = 11;
					break;
				case 11:
					stemmingRule.ContinuationPriority = GetNumber();
					num = 5;
					break;
				case 12:
					switch (GetIdentifier().ToLowerInvariant())
					{
					case "minwordlength":
						_ruleSet.MinimumWordLength = GetNumber();
						break;
					case "minstemlength":
						_ruleSet.MinimumStemLength = GetNumber();
						break;
					case "minstempercentage":
						_ruleSet.MinimumStemPercentage = GetNumber();
						break;
					case "maxruleapplications":
						_ruleSet.MaximumRuleApplications = GetNumber();
						break;
					default:
						throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationInvalidVariableName);
					}
					num = 5;
					break;
				case 15:
					stemmingRule = new VersionStemmingRule(GetNumber());
					num = 5;
					break;
				}
			}
			if (stemmingRule.Action != 0)
			{
				_ruleSet.Add(stemmingRule);
			}
		}

		private void Expect(string expectation)
		{
			if (expectation == null)
			{
				throw new ArgumentNullException("expectation");
			}
			string identifier = GetIdentifier();
			if (!identifier.Equals(expectation, StringComparison.OrdinalIgnoreCase))
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationInvalidRule);
			}
		}

		private string GetIdentifier()
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (_ruleP < _rule.Length && char.IsLetter(_rule[_ruleP]))
			{
				stringBuilder.Append(_rule[_ruleP]);
				_ruleP++;
			}
			return stringBuilder.ToString();
		}

		private int GetNumber()
		{
			while (_ruleP < _rule.Length && char.IsWhiteSpace(_rule, _ruleP))
			{
				_ruleP++;
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (_ruleP < _rule.Length && char.IsDigit(_rule[_ruleP]))
			{
				stringBuilder.Append(_rule[_ruleP]);
				_ruleP++;
			}
			if (stringBuilder.Length == 0)
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationInvalidRule);
			}
			return int.Parse(stringBuilder.ToString());
		}

		private string GetQuotedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (_rule[_ruleP] != '"')
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationInvalidRule);
			}
			_ruleP++;
			while (_ruleP < _rule.Length && _rule[_ruleP] != '"')
			{
				stringBuilder.Append(_rule[_ruleP]);
				_ruleP++;
			}
			if (_ruleP >= _rule.Length || _rule[_ruleP] != '"')
			{
				throw new LanguageProcessingException(ErrorMessages.EMSG_SegmentationInvalidRule);
			}
			_ruleP++;
			return stringBuilder.ToString();
		}
	}
}
