using Sdl.LanguagePlatform.Stat;
using Sdl.LanguagePlatform.Stat.WordAlignment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Core.FineGrainedAlignment
{
	public class WordAlignmentComputer3
	{
		private readonly IScoreProvider2 _ScoreProvider;

		private readonly VocabularyFile3 _SrcVocabulary;

		private readonly VocabularyFile3 _TrgVocabulary;

		private readonly ICombineMetrics _CombineMetrics;

		private readonly SpecialTokenIDs _SrcSpecialTokenIDs;

		private readonly SpecialTokenIDs _TrgSpecialTokenIDs;

		public static readonly int MAX_SEGMENT_LENGTH = 600;

		public WordAlignmentComputer3(IScoreProvider2 scoreProvider, VocabularyFile3 vocabSrc, VocabularyFile3 vocabTrg)
		{
			if (scoreProvider == null)
			{
				throw new ArgumentNullException("scoreProvider");
			}
			_ScoreProvider = scoreProvider;
			_SrcVocabulary = vocabSrc;
			_TrgVocabulary = vocabTrg;
			_SrcSpecialTokenIDs = _SrcVocabulary.SpecialTokenIDs;
			_TrgSpecialTokenIDs = _TrgVocabulary.SpecialTokenIDs;
			_CombineMetrics = new SimpleMinMetrics();
		}

		internal AlignmentTable ComputeAlignment(IntSegment srcSegment, IntSegment trgSegment, Dictionary<int, int> givenAlignments, double givenAlignmentAssociation, out List<ExtendableBilingualPhrase> phrases)
		{
			int count = srcSegment.Count;
			int count2 = trgSegment.Count;
			double[,] array = new double[count, count2];
			for (int i = 0; i < count; i++)
			{
				int num = srcSegment[i];
				bool flag = _SrcSpecialTokenIDs.IsSpecial(num);
				if (givenAlignments.ContainsKey(i))
				{
					continue;
				}
				for (int j = 0; j < count2; j++)
				{
					int num2 = trgSegment[j];
					bool flag2 = _TrgSpecialTokenIDs.IsSpecial(num2);
					if (num < 0 || num2 < 0)
					{
						array[i, j] = 0.0;
					}
					else if (flag | flag2)
					{
						if (num == _SrcSpecialTokenIDs.PCT || num2 == _TrgSpecialTokenIDs.PCT)
						{
							array[i, j] = _ScoreProvider.GetScore(num, num2);
						}
						else if (flag != flag2)
						{
							array[i, j] = 0.0;
						}
						else if (_SrcSpecialTokenIDs.GetSpecialTokenType(num) != _TrgSpecialTokenIDs.GetSpecialTokenType(num2))
						{
							array[i, j] = 0.0;
						}
						else
						{
							array[i, j] = _ScoreProvider.GetScore(num, num2);
						}
					}
					else
					{
						array[i, j] = _ScoreProvider.GetScore(num, num2);
					}
				}
			}
			foreach (KeyValuePair<int, int> givenAlignment in givenAlignments)
			{
				array[givenAlignment.Key, givenAlignment.Value] = givenAlignmentAssociation;
			}
			AlignmentTable result = new AlignmentTable(count, count2, array);
			phrases = ExtendPhrases(result, array, srcSegment, trgSegment, null);
			return result;
		}

		private static bool AnyPhraseContainsSource(List<ExtendableBilingualPhrase> phrases, int srcIndex)
		{
			return phrases.Any((ExtendableBilingualPhrase phrase) => srcIndex >= phrase.FromSrcIndex && srcIndex <= phrase.IntoSrcIndex);
		}

		private static bool AnyPhraseContainsTarget(List<ExtendableBilingualPhrase> phrases, int trgIndex)
		{
			return phrases.Any((ExtendableBilingualPhrase phrase) => trgIndex >= phrase.FromTrgIndex && trgIndex <= phrase.IntoTrgIndex);
		}

		private void ComputeExtensionCandidates(AlignmentTable result, List<ExtendableBilingualPhrase> phrases, int phraseIndex, IntSegment srcSegment, IntSegment trgSegment, double[,] associations, TextWriter logStream)
		{
			ExtendableBilingualPhrase extendableBilingualPhrase = phrases[phraseIndex];
			extendableBilingualPhrase.ExtensionPoints = new List<ExtensionPoint>();
			bool flag = extendableBilingualPhrase.FromSrcIndex > 0 && !AnyPhraseContainsSource(phrases, extendableBilingualPhrase.FromSrcIndex - 1);
			bool flag2 = extendableBilingualPhrase.IntoSrcIndex + 1 < srcSegment.Count && !AnyPhraseContainsSource(phrases, extendableBilingualPhrase.IntoSrcIndex + 1);
			bool flag3 = extendableBilingualPhrase.FromTrgIndex > 0 && !AnyPhraseContainsTarget(phrases, extendableBilingualPhrase.FromTrgIndex - 1);
			bool flag4 = extendableBilingualPhrase.IntoTrgIndex + 1 < trgSegment.Count && !AnyPhraseContainsTarget(phrases, extendableBilingualPhrase.IntoTrgIndex + 1);
			if (!(flag | flag2 | flag4 | flag3))
			{
				return;
			}
			for (int i = flag ? (extendableBilingualPhrase.FromSrcIndex - 1) : extendableBilingualPhrase.FromSrcIndex; i <= (flag2 ? (extendableBilingualPhrase.IntoSrcIndex + 1) : extendableBilingualPhrase.IntoSrcIndex); i++)
			{
				bool flag5 = i >= extendableBilingualPhrase.FromSrcIndex && i <= extendableBilingualPhrase.IntoSrcIndex;
				for (int j = flag3 ? (extendableBilingualPhrase.FromTrgIndex - 1) : extendableBilingualPhrase.FromTrgIndex; j <= (flag4 ? (extendableBilingualPhrase.IntoTrgIndex + 1) : extendableBilingualPhrase.IntoTrgIndex); j++)
				{
					bool flag6 = j >= extendableBilingualPhrase.FromTrgIndex && j <= extendableBilingualPhrase.IntoTrgIndex;
					if ((flag5 && flag6) || result[i, j] || associations[i, j] == 0.0)
					{
						continue;
					}
					bool flag7 = i == extendableBilingualPhrase.FromSrcIndex - 1;
					bool flag8 = i == extendableBilingualPhrase.IntoSrcIndex + 1;
					bool flag9 = j == extendableBilingualPhrase.FromTrgIndex - 1;
					bool flag10 = j == extendableBilingualPhrase.IntoTrgIndex + 1;
					bool flag11 = !flag5 && !flag6;
					bool flag12 = false;
					if (flag7)
					{
						for (int k = j - 1; k <= j + 1; k++)
						{
							if (flag12)
							{
								break;
							}
							flag12 = (k >= 0 && k < trgSegment.Count && result[i + 1, k]);
						}
					}
					if (flag8 && !flag12)
					{
						for (int l = j - 1; l <= j + 1; l++)
						{
							if (flag12)
							{
								break;
							}
							flag12 = (l >= 0 && l < trgSegment.Count && result[i - 1, l]);
						}
					}
					if (flag9 && !flag12)
					{
						for (int m = i - 1; m <= i + 1; m++)
						{
							if (flag12)
							{
								break;
							}
							flag12 = (m >= 0 && m < srcSegment.Count && result[m, j + 1]);
						}
					}
					if (flag10 && !flag12)
					{
						for (int n = i - 1; n <= i + 1; n++)
						{
							if (flag12)
							{
								break;
							}
							flag12 = (n >= 0 && n < srcSegment.Count && result[n, j - 1]);
						}
					}
					if (!flag12)
					{
						continue;
					}
					if (!flag11)
					{
						if (i >= extendableBilingualPhrase.FromSrcIndex && i <= extendableBilingualPhrase.IntoSrcIndex)
						{
							int num = 0;
							if (j > extendableBilingualPhrase.IntoTrgIndex)
							{
								int num2 = extendableBilingualPhrase.IntoTrgIndex;
								while (num2 >= extendableBilingualPhrase.FromTrgIndex && flag12)
								{
									if (!result[i, num2])
									{
										num++;
									}
									else if (num > 0)
									{
										flag12 = false;
									}
									num2--;
								}
							}
							else if (j < extendableBilingualPhrase.FromTrgIndex)
							{
								for (int num3 = extendableBilingualPhrase.FromTrgIndex; num3 <= extendableBilingualPhrase.IntoTrgIndex; num3++)
								{
									if (!flag12)
									{
										break;
									}
									if (!result[i, num3])
									{
										num++;
									}
									else if (num > 0)
									{
										flag12 = false;
									}
								}
							}
						}
						if (flag12 && j >= extendableBilingualPhrase.FromTrgIndex && j <= extendableBilingualPhrase.IntoTrgIndex)
						{
							int num4 = 0;
							if (i > extendableBilingualPhrase.IntoSrcIndex)
							{
								int num5 = extendableBilingualPhrase.IntoSrcIndex;
								while (num5 >= extendableBilingualPhrase.FromSrcIndex && flag12)
								{
									if (!result[num5, j])
									{
										num4++;
									}
									else if (num4 > 0)
									{
										flag12 = false;
									}
									num5--;
								}
							}
							else if (i < extendableBilingualPhrase.FromSrcIndex)
							{
								for (int num6 = extendableBilingualPhrase.FromSrcIndex; num6 <= extendableBilingualPhrase.IntoSrcIndex; num6++)
								{
									if (!flag12)
									{
										break;
									}
									if (!result[num6, j])
									{
										num4++;
									}
									else if (num4 > 0)
									{
										flag12 = false;
									}
								}
							}
						}
					}
					if (flag12)
					{
						extendableBilingualPhrase.ExtensionPoints.Add(new ExtensionPoint(i, j));
					}
				}
			}
		}

		private void InvalidateOverlappingExtensionPoints(List<ExtendableBilingualPhrase> phrases, int currentPhrase)
		{
			if (phrases == null || phrases.Count <= 1)
			{
				return;
			}
			ExtendableBilingualPhrase xp = phrases[currentPhrase];
			for (int i = 0; i < phrases.Count; i++)
			{
				if (i != currentPhrase && phrases[i].ExtensionPoints != null && phrases[i].ExtensionPoints.Count > 0 && xp.HasAdjacentEdge(phrases[i]))
				{
					phrases[i].ExtensionPoints.RemoveAll((ExtensionPoint x) => (x.I >= xp.FromSrcIndex && x.I <= xp.IntoSrcIndex) || (x.J >= xp.FromTrgIndex && x.J <= xp.IntoTrgIndex));
				}
			}
		}

		private List<ExtendableBilingualPhrase> ExtendPhrases(AlignmentTable result, double[,] associations, IntSegment srcSegment, IntSegment trgSegment, TextWriter logStream)
		{
			if (srcSegment.Count > MAX_SEGMENT_LENGTH || trgSegment.Count > MAX_SEGMENT_LENGTH)
			{
				return null;
			}
			if (logStream != null)
			{
				logStream.Write("Src Segment:\t");
				logStream.WriteLine();
				logStream.Write("Trg Segment:\t");
				logStream.WriteLine();
				logStream.WriteLine();
				logStream.WriteLine("Associations:");
				logStream.WriteLine();
				logStream.Flush();
			}
			List<ExtendableBilingualPhrase> list = FindSeedPoints(result, associations);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<ExtendableBilingualPhrase> list2 = new List<ExtendableBilingualPhrase>();
			foreach (ExtendableBilingualPhrase item2 in list)
			{
				ExtendableBilingualPhrase item = new ExtendableBilingualPhrase(item2);
				list2.Add(item);
			}
			bool flag = false;
			do
			{
				double num = 0.0;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				int num5 = -1;
				flag = false;
				for (int i = 0; i < list.Count; i++)
				{
					ExtendableBilingualPhrase extendableBilingualPhrase = list[i];
					if (extendableBilingualPhrase.ExtensionPoints == null)
					{
						ComputeExtensionCandidates(result, list, i, srcSegment, trgSegment, associations, logStream);
					}
					foreach (ExtensionPoint extensionPoint in extendableBilingualPhrase.ExtensionPoints)
					{
						int i2 = extensionPoint.I;
						int j = extensionPoint.J;
						double combinedGain = _CombineMetrics.GetCombinedGain(extendableBilingualPhrase, associations[i2, j]);
						if (combinedGain > num)
						{
							num = combinedGain;
							num3 = i2;
							num4 = j;
							num2 = i;
							num5 = -1;
						}
					}
					for (int k = i + 1; k < list.Count; k++)
					{
						BilingualPhrase bilingualPhrase = list[k];
						if (extendableBilingualPhrase.IsAdjacent(bilingualPhrase))
						{
							double combinedGain2 = _CombineMetrics.GetCombinedGain(extendableBilingualPhrase, bilingualPhrase);
							if (combinedGain2 > num)
							{
								num3 = -1;
								num4 = -1;
								num2 = i;
								num5 = k;
								num = combinedGain2;
							}
						}
					}
				}
				if (num2 < 0)
				{
					continue;
				}
				if (num5 < 0)
				{
					if (logStream != null)
					{
						logStream.WriteLine("Point {0}/{1} extends [{2}-{3}, {4}-{5}]", num3, num4, list[num2].FromSrcIndex, list[num2].IntoSrcIndex, list[num2].FromTrgIndex, list[num2].IntoTrgIndex);
						logStream.Flush();
					}
					result[num3, num4] = true;
					list[num2].Extend(num3, num4);
					list[num2].Association = num;
					list[num2].ClearExtensionPoints();
					list2.Add(new ExtendableBilingualPhrase(list[num2]));
					InvalidateOverlappingExtensionPoints(list, num2);
				}
				else
				{
					if (logStream != null)
					{
						logStream.WriteLine("Phrase merge: [{0}-{1}, {2}-{3}] and [{4}-{5}, {6}-{7}]", list[num2].FromSrcIndex, list[num2].IntoSrcIndex, list[num2].FromTrgIndex, list[num2].IntoTrgIndex, list[num5].FromSrcIndex, list[num5].IntoSrcIndex, list[num5].FromTrgIndex, list[num5].IntoTrgIndex);
						logStream.Flush();
					}
					list[num2].Extend(list[num5]);
					list[num2].Association = num;
					list[num2].ClearExtensionPoints();
					list2.Add(new ExtendableBilingualPhrase(list[num2]));
					list.RemoveAt(num5);
					if (num5 < num2)
					{
						InvalidateOverlappingExtensionPoints(list, num2 - 1);
					}
					else
					{
						InvalidateOverlappingExtensionPoints(list, num2);
					}
				}
				flag = true;
			}
			while (flag);
			return list2;
		}

		private static List<ExtendableBilingualPhrase> FindSeedPoints(AlignmentTable alignments, double[,] associations)
		{
			List<ExtendableBilingualPhrase> list = new List<ExtendableBilingualPhrase>();
			int num = associations.GetUpperBound(0) + 1;
			int num2 = associations.GetUpperBound(1) + 1;
			for (int i = 0; i < num; i++)
			{
				if (alignments.IsSourceAligned(i))
				{
					continue;
				}
				int num3 = -1;
				double num4 = 0.0;
				bool flag = false;
				for (int j = 0; j < num2; j++)
				{
					if (!alignments.IsTargetAligned(j))
					{
						if (associations[i, j] > num4)
						{
							num4 = associations[i, j];
							num3 = j;
							flag = false;
						}
						else if (associations[i, j] == num4)
						{
							flag = true;
						}
					}
				}
				if (num3 < 0 || flag)
				{
					continue;
				}
				bool flag2 = true;
				for (int k = 0; k < num; k++)
				{
					if (!flag2)
					{
						break;
					}
					if (k != i && !alignments.IsSourceAligned(k) && associations[k, num3] >= num4)
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					ExtendableBilingualPhrase item = new ExtendableBilingualPhrase(i, num3, associations[i, num3]);
					list.Add(item);
					alignments[i, num3] = true;
				}
			}
			return list;
		}
	}
}
