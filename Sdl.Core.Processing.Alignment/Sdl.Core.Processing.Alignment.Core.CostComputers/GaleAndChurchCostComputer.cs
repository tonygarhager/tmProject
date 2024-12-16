using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class GaleAndChurchCostComputer : IAlignmentCostComputer
	{
		private readonly double _expansionFactor = 1.0;

		private readonly AlignmentPenalties _penalties = new AlignmentPenalties();

		public GaleAndChurchCostComputer()
		{
		}

		public GaleAndChurchCostComputer(double expansionFactor)
		{
			_expansionFactor = expansionFactor;
		}

		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			IList<string> list = sourceElements.Select((AlignmentElement item) => item.TextContent).ToList();
			IList<string> list2 = targetElements.Select((AlignmentElement item) => item.TextContent).ToList();
			if (list.Count == 0 && list2.Count == 1)
			{
				return GetNormalizedAlignmentCost(GetDeletionCosts(list2[0]));
			}
			if (list.Count == 1 && list2.Count == 0)
			{
				return GetNormalizedAlignmentCost(GetInsertionCosts(list[0]));
			}
			if (list.Count == 1 && list2.Count == 1)
			{
				return GetNormalizedAlignmentCost(GetSubstitutionCosts(list[0], list2[0]));
			}
			if (list.Count == 1 && list2.Count == 2)
			{
				return GetNormalizedAlignmentCost(GetExpansionCosts(list[0], list2[0], list2[1]));
			}
			if (list.Count == 2 && list2.Count == 1)
			{
				return GetNormalizedAlignmentCost(GetContractionCosts(list[0], list[1], list2[0]));
			}
			if (list.Count == 2 && list2.Count == 2)
			{
				return GetNormalizedAlignmentCost(GetMeldingCosts(list[0], list[1], list2[0], list2[1]));
			}
			throw new NotImplementedException();
		}

		private static AlignmentCost GetNormalizedAlignmentCost(int alignmentCost)
		{
			return (AlignmentCost)((double)Math.Min(alignmentCost, 2500) / 2500.0);
		}

		private int GetDeletionCosts(string t)
		{
			return GetDistanceCost(0, t.Length) + _penalties.DeletionPenalty;
		}

		private int GetInsertionCosts(string s)
		{
			return GetDistanceCost(s.Length, 0) + _penalties.InsertionPenalty;
		}

		private int GetSubstitutionCosts(string s, string t)
		{
			return GetDistanceCost(s.Length, t.Length);
		}

		private int GetExpansionCosts(string s, string t1, string t2)
		{
			return GetDistanceCost(s.Length, t1.Length + t2.Length) + _penalties.ExpansionPenalty;
		}

		private int GetContractionCosts(string s1, string s2, string t)
		{
			return GetDistanceCost(s1.Length + s2.Length, t.Length) + _penalties.ContractionPenalty;
		}

		private int GetMeldingCosts(string s1, string s2, string t1, string t2)
		{
			return GetDistanceCost(s1.Length + s2.Length, t1.Length + t2.Length) + _penalties.MeldingPenalty;
		}

		private int GetDistanceCost(int s, int t)
		{
			if (s == 0 && t == 0)
			{
				return 0;
			}
			double num = (double)s * _expansionFactor - (double)t;
			double num2 = ((double)s + (double)t / _expansionFactor) / 2.0;
			double z = Math.Abs(num / Math.Sqrt(num2 * 6.8));
			double num3 = 2.0 * (1.0 - pnorm(z));
			return (int)((num3 > 0.0) ? (-100.0 * Math.Log(num3)) : 1000000.0);
		}

		private static double pnorm(double z)
		{
			double num = 1.0 / (1.0 + 0.2316419 * z);
			return 1.0 - 0.3989423 * Math.Exp((0.0 - z) * z / 2.0) * ((((1.330274429 * num - 1.821255978) * num + 1.781477937) * num - 0.356563782) * num + 0.31938153) * num;
		}
	}
}
