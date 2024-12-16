using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Sdl.Core.LanguageProcessing.Stemming
{
	public class StemmingRuleSet
	{
		private const int DefaultMinimumWordLength = 3;

		private const int DefaultMinimumStemLength = 2;

		private const int DefaultMinimumStemPercentage = 30;

		private const int DefaultMaximumRuleApplications = 100;

		private readonly List<StemmingRule> _rules;

		public StemmingRule this[int index] => _rules[index];

		public int Count => _rules.Count;

		[XmlIgnore]
		public CultureInfo Culture
		{
			get;
		}

		public int Version
		{
			get;
			private set;
		}

		public int MinimumStemLength
		{
			get;
			set;
		} = 2;


		public int MaximumRuleApplications
		{
			get;
			set;
		} = 100;


		public int MinimumStemPercentage
		{
			get;
			set;
		} = 30;


		public int MinimumWordLength
		{
			get;
			set;
		} = 3;


		public StemmingRuleSet(CultureInfo culture)
		{
			_rules = new List<StemmingRule>();
			Culture = culture;
		}

		public void Add(StemmingRule r)
		{
			VersionStemmingRule versionStemmingRule = r as VersionStemmingRule;
			if (versionStemmingRule != null)
			{
				Version = versionStemmingRule.Version;
				return;
			}
			_rules.Add(r);
			Sort();
		}

		private void Sort()
		{
			_rules.Sort(StemmingRule.Compare);
		}
	}
}
