using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class ScoringResult
	{
		[DataMember]
		public TextContextMatch TextContextMatch
		{
			get;
			set;
		}

		[DataMember]
		public bool IsStructureContextMatch
		{
			get;
			set;
		}

		[DataMember]
		public bool IdContextMatch
		{
			get;
			set;
		}

		[XmlIgnore]
		public int Match
		{
			get
			{
				int num = BaseScore;
				if (AppliedPenalties == null)
				{
					if (num >= 0)
					{
						return num;
					}
					return 0;
				}
				foreach (AppliedPenalty appliedPenalty in AppliedPenalties)
				{
					num -= appliedPenalty.Malus;
				}
				if (num >= 0)
				{
					return num;
				}
				return 0;
			}
		}

		[DataMember]
		public int BaseScore
		{
			get;
			set;
		}

		[DataMember]
		public int ResolvedPlaceables
		{
			get;
			set;
		}

		[DataMember]
		public int TextReplacements
		{
			get;
			set;
		}

		[DataMember]
		public int PlaceableFormatChanges
		{
			get;
			set;
		}

		[DataMember]
		public EditDistance EditDistance
		{
			get;
			set;
		}

		[DataMember]
		public List<SegmentRange> MatchingConcordanceRanges
		{
			get;
			set;
		}

		[DataMember]
		public bool MemoryTagsDeleted
		{
			get;
			set;
		}

		[DataMember]
		public bool TagMismatch
		{
			get;
			set;
		}

		[DataMember]
		public List<AppliedPenalty> AppliedPenalties
		{
			get;
			set;
		}

		[DataMember]
		public bool TargetSegmentDiffers
		{
			get;
			set;
		}

		public bool IsExactMatch => BaseScore >= 100;

		public ScoringResult()
		{
			BaseScore = 0;
			EditDistance = null;
			MemoryTagsDeleted = false;
			TagMismatch = false;
		}

		public AppliedPenalty FindPenalty(PenaltyType pt)
		{
			if (AppliedPenalties == null)
			{
				return null;
			}
			foreach (AppliedPenalty appliedPenalty in AppliedPenalties)
			{
				if (appliedPenalty.Type == pt)
				{
					return appliedPenalty;
				}
			}
			return null;
		}

		public AppliedPenalty FindAppliedFilter(string filterName)
		{
			return AppliedPenalties?.FirstOrDefault((AppliedPenalty t) => t.Type == PenaltyType.FilterPenalty && filterName.Equals(t.FilterName, StringComparison.OrdinalIgnoreCase));
		}

		public void ApplyPenalty(Penalty pt)
		{
			if (pt == null)
			{
				return;
			}
			AppliedPenalty appliedPenalty = FindPenalty(pt.PenaltyType);
			if (appliedPenalty != null)
			{
				if (Penalty.CanApplyMultipleTimes(pt.PenaltyType))
				{
					appliedPenalty.Malus += pt.Malus;
				}
				return;
			}
			if (AppliedPenalties == null)
			{
				AppliedPenalties = new List<AppliedPenalty>();
			}
			AppliedPenalties.Add(new AppliedPenalty(pt.PenaltyType, pt.Malus));
		}

		public void RemovePenalty(PenaltyType pt)
		{
			if (AppliedPenalties != null)
			{
				RemoveAll(AppliedPenalties, (AppliedPenalty x) => x.Type == pt);
			}
		}

		private static void RemoveAll(IList<AppliedPenalty> elements, Func<AppliedPenalty, bool> d)
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

		public void ApplyFilter(string filterName, int malus)
		{
			if (FindAppliedFilter(filterName) == null)
			{
				if (AppliedPenalties == null)
				{
					AppliedPenalties = new List<AppliedPenalty>();
				}
				AppliedPenalties.Add(new AppliedPenalty(filterName, malus));
			}
		}
	}
}
