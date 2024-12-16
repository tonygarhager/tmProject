using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class FuzzySearcher
	{
		private readonly IFuzzyDataReader _storage;

		public FuzzySearcher(IFuzzyDataReader storage)
		{
			_storage = storage;
		}

		public List<Hit> Search(IntFeatureVector fv, int maxResults, int minScore, int lastKey, ScoringMethod scoringMethod, Predicate<int> validateItemCallback, bool descendingOrder)
		{
			if (fv == null || fv.Count == 0 || maxResults <= 0)
			{
				return null;
			}
			if (minScore <= 0)
			{
				minScore = 0;
			}
			else if (minScore > 100)
			{
				minScore = 100;
			}
			IComparer<Hit> comparer = (!descendingOrder) ? ((IComparer<Hit>)new HitAscendingComparer()) : ((IComparer<Hit>)new HitDescendingComparer());
			List<Hit> list = new List<Hit>();
			int num = 0;
			List<AbstractPostingsIterator> list2 = new List<AbstractPostingsIterator>();
			foreach (int item2 in fv)
			{
				AbstractPostingsIterator iterator = _storage.GetIterator(item2, descendingOrder);
				if (iterator != null && iterator.Count > 0)
				{
					list2.Add(iterator);
					if (num == 0 || iterator.Count > num)
					{
						num = iterator.Count;
					}
				}
			}
			list2.Sort();
			double num2 = (double)minScore / 100.0;
			int num3 = (minScore == 0) ? 1 : (fv.Count * minScore / 100);
			int num4 = fv.Count - num3;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			if (scoringMethod == ScoringMethod.Dice && minScore > 0)
			{
				double num8 = 2.0 * (double)fv.Count * (1.0 - num2);
				num7 = fv.Count - (int)(num8 / (2.0 - num2));
				if (num7 <= 0)
				{
					num7 = 1;
				}
				num6 = fv.Count + (int)(num8 / num2);
			}
			while (list2.Count > 4 && num4 > 0 && list2[0].Count > 1000)
			{
				list2.RemoveAt(0);
				num5++;
				num4--;
			}
			int num9 = 0;
			while (num9 < list2.Count)
			{
				int num10 = descendingOrder ? int.MaxValue : (-1);
				foreach (AbstractPostingsIterator item3 in list2)
				{
					if (!item3.AtEnd)
					{
						int current3 = item3.Current;
						if (descendingOrder)
						{
							if (num10 == int.MaxValue || current3 > num10)
							{
								num10 = current3;
							}
						}
						else if (num10 == -1 || current3 < num10)
						{
							num10 = current3;
						}
					}
				}
				if (num10 == int.MaxValue || num10 == -1)
				{
					return null;
				}
				int num11 = 0;
				foreach (AbstractPostingsIterator item4 in list2)
				{
					if (!item4.AtEnd && item4.Current == num10)
					{
						num11++;
						item4.Next();
						if (item4.AtEnd)
						{
							num9++;
						}
					}
				}
				if ((descendingOrder && num10 > lastKey) || (!descendingOrder && num10 < lastKey))
				{
					continue;
				}
				IntFeatureVector featureVector = _storage.GetFeatureVector(num10);
				if (featureVector == null)
				{
					continue;
				}
				int count = featureVector.Count;
				if (count == 0 || (num6 > 0 && num7 > 0 && (count < num7 || count > num6)) || (validateItemCallback != null && !validateItemCallback(num10)))
				{
					continue;
				}
				if (num5 != 0)
				{
					num11 = featureVector.GetCommonFeatureCount(fv);
				}
				double num12;
				switch (scoringMethod)
				{
				case ScoringMethod.Query:
					num12 = (double)num11 / (double)fv.Count;
					break;
				case ScoringMethod.Result:
					num12 = (double)num11 / (double)count;
					break;
				case ScoringMethod.Dice:
					num12 = 2.0 * (double)num11 / ((double)fv.Count + (double)count);
					break;
				default:
					throw new ArgumentException("scoringMethod");
				}
				int num13 = (int)(100.0 * num12);
				if (num13 < minScore)
				{
					continue;
				}
				Hit item = new Hit(num10, num13);
				int num14 = list.BinarySearch(item, comparer);
				if (~num14 < maxResults)
				{
					list.Insert(~num14, item);
					if (list.Count > maxResults)
					{
						list.RemoveAt(list.Count - 1);
					}
				}
			}
			return list;
		}
	}
}
