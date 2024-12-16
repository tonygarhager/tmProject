using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class LongestCommonSubsequenceComputer<T>
	{
		public static List<Pair<int>> ComputeLongestCommonSubsequence(IList<T> sequenceA, IList<T> sequenceB)
		{
			if (sequenceA == null || sequenceA.Count == 0 || sequenceB == null || sequenceB.Count == 0)
			{
				return null;
			}
			int count = sequenceA.Count;
			int count2 = sequenceB.Count;
			int[,] array = new int[count, count2];
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < count2; j++)
				{
					if (sequenceA[i].Equals(sequenceB[j]))
					{
						if (i == 0 || j == 0)
						{
							array[i, j] = 1;
						}
						else
						{
							array[i, j] = 1 + array[i - 1, j - 1];
						}
					}
					else if (i == 0)
					{
						if (j == 0)
						{
							array[i, j] = 0;
						}
						else
						{
							array[i, j] = Math.Max(0, array[i, j - 1]);
						}
					}
					else if (j == 0)
					{
						array[i, j] = Math.Max(array[i - 1, j], 0);
					}
					else
					{
						array[i, j] = Math.Max(array[i - 1, j], array[i, j - 1]);
					}
				}
			}
			if (array[count - 1, count2 - 1] == 0)
			{
				return null;
			}
			List<Pair<int>> list = new List<Pair<int>>();
			int num = count - 1;
			int num2 = count2 - 1;
			while (num >= 0 && num2 >= 0)
			{
				if (sequenceA[num].Equals(sequenceB[num2]))
				{
					list.Insert(0, new Pair<int>(num, num2));
					num--;
					num2--;
				}
				else if (num == 0)
				{
					num2--;
				}
				else if (num2 == 0)
				{
					num--;
				}
				else if (array[num, num2 - 1] >= array[num - 1, num2])
				{
					num2--;
				}
				else
				{
					num--;
				}
			}
			return list;
		}

		public static List<AlignedSubstring> ComputeLongestCommonSubstring(IList<T> sequenceA, IList<T> sequenceB)
		{
			if (sequenceA == null || sequenceA.Count == 0 || sequenceB == null || sequenceB.Count == 0)
			{
				return null;
			}
			List<AlignedSubstring> list = new List<AlignedSubstring>();
			int count = sequenceA.Count;
			int count2 = sequenceB.Count;
			int[,] array = new int[count, count2];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < count2; j++)
				{
					if (sequenceA[i].Equals(sequenceB[j]))
					{
						if (i == 0 || j == 0)
						{
							array[i, j] = 1;
						}
						else
						{
							array[i, j] = 1 + array[i - 1, j - 1];
						}
						if (array[i, j] > num)
						{
							num = array[i, j];
							list.Clear();
						}
						if (array[i, j] == num)
						{
							list.Add(new AlignedSubstring(i - num + 1, num, j - num + 1, num));
						}
					}
				}
			}
			if (list.Count > 0)
			{
				list.Sort((AlignedSubstring x, AlignedSubstring y) => y.Source.Length - x.Source.Length);
			}
			return list;
		}

		public static List<AlignedSubstring> ComputeLongestCommonSubstringCoverage(IList<T> sequenceA, IList<T> sequenceB)
		{
			if (sequenceA == null || sequenceA.Count == 0 || sequenceB == null || sequenceB.Count == 0)
			{
				return null;
			}
			return SequenceAlignmentComputer<T>.ComputeCoverage(sequenceA, sequenceB, new SimpleLCSScoreProvider<T>(), null);
		}
	}
}
