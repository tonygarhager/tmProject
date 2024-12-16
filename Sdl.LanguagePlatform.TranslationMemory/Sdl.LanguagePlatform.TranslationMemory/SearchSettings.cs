using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SearchSettings
	{
		public static readonly int MinscoreLowerbound = 30;

		private int _minScore;

		private int _maxResults;

		[DataMember]
		public SearchMode Mode
		{
			get;
			set;
		}

		[DataMember]
		public MachineTranslationLookupMode MachineTranslationLookup
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDocumentSearch
		{
			get;
			set;
		}

		[DataMember]
		public List<Penalty> Penalties
		{
			get;
			set;
		}

		[DataMember]
		public bool CheckMatchingSublanguages
		{
			get;
			set;
		}

		[DataMember]
		public bool AdvancedTokenizationLegacyScoring
		{
			get;
			set;
		}

		[DataMember]
		public List<Filter> Filters
		{
			get;
			set;
		}

		[DataMember]
		public AutoLocalizationSettings AutoLocalizationSettings
		{
			get;
			set;
		}

		[DataMember]
		public FilterExpression HardFilter
		{
			get;
			set;
		}

		[DataMember]
		public SortSpecification SortSpecification
		{
			get;
			set;
		}

		[DataMember]
		public bool ComputeTranslationProposal
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentStructureContext
		{
			get;
			set;
		}

		[DataMember]
		public int MinScore
		{
			get
			{
				return _minScore;
			}
			set
			{
				_minScore = Math.Max(MinscoreLowerbound, value);
			}
		}

		[DataMember]
		public int MaxResults
		{
			get
			{
				return _maxResults;
			}
			set
			{
				_maxResults = Math.Max(1, value);
			}
		}

		[DataMember]
		public AdaptiveMachineTranslationLookupMode AdaptiveMachineTranslationLookupMode
		{
			get;
			set;
		}

		public bool IsConcordanceSearch
		{
			get
			{
				if (Mode != SearchMode.ConcordanceSearch)
				{
					return Mode == SearchMode.TargetConcordanceSearch;
				}
				return true;
			}
		}

		public SearchSettings()
		{
			ComputeTranslationProposal = false;
			IsDocumentSearch = false;
			Mode = SearchMode.NormalSearch;
			MachineTranslationLookup = MachineTranslationLookupMode.WhenNoTranslationMemoryMatch;
			AdaptiveMachineTranslationLookupMode = AdaptiveMachineTranslationLookupMode.Translation;
			_minScore = 70;
			_maxResults = 10;
			CheckMatchingSublanguages = false;
			AdvancedTokenizationLegacyScoring = false;
		}

		public Penalty FindPenalty(PenaltyType pt)
		{
			return Penalties?.FirstOrDefault((Penalty p) => p.PenaltyType == pt);
		}

		public void AddPenalty(PenaltyType pt, int malus)
		{
			Penalty penalty = FindPenalty(pt);
			if (penalty == null)
			{
				penalty = new Penalty(pt, malus);
				if (Penalties == null)
				{
					Penalties = new List<Penalty>();
				}
				Penalties.Add(penalty);
			}
		}

		public void RemovePenalty(PenaltyType pt)
		{
			if (Penalties != null && Penalties.Count > 0)
			{
				RemoveAll(Penalties, (Penalty p) => p.PenaltyType == pt);
			}
		}

		private static void RemoveAll<T>(IList<T> elements, Func<T, bool> d)
		{
			int num = 0;
			while (num < elements.Count)
			{
				if (d(elements[num]))
				{
					elements.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}

		public void AddFilter(Filter filter)
		{
			if (Filters == null)
			{
				Filters = new List<Filter>();
			}
			Filters.Add(filter);
		}

		public bool RemoveFilter(string filterName)
		{
			int num = FindFilterPosition(filterName);
			if (num < 0)
			{
				return false;
			}
			Filters.RemoveAt(num);
			return true;
		}

		public Filter FindFilter(string filterName)
		{
			int num = FindFilterPosition(filterName);
			if (num < 0)
			{
				return null;
			}
			return Filters[num];
		}

		private int FindFilterPosition(string filterName)
		{
			if (Filters == null || Filters.Count == 0)
			{
				return -1;
			}
			for (int i = 0; i < Filters.Count; i++)
			{
				if (Filters[i].Name.Equals(filterName, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
