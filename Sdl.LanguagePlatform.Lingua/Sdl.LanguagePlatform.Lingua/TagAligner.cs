using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua.Alignment;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class TagAligner
	{
		public static TagAssociations AlignPairedTags(IList<Token> sourceTokens, IList<Token> targetTokens, SimilarityMatrix similarityMatrix)
		{
			TagPairs tagPairs = FindPairedTags(sourceTokens);
			if (tagPairs == null || tagPairs.Count == 0)
			{
				return null;
			}
			TagPairs tagPairs2 = FindPairedTags(targetTokens);
			if (tagPairs2 == null || tagPairs2.Count == 0)
			{
				return null;
			}
			TagAssociations tagAssociations = new TagAssociations();
			BitArray bitArray = new BitArray(tagPairs.Count);
			BitArray bitArray2 = new BitArray(tagPairs2.Count);
			if (tagPairs.Count > 0 && tagPairs2.Count > 0)
			{
				int[,] array = ComputeTagAssociationScores(similarityMatrix, tagPairs, tagPairs2, useEndPositions: true);
				if (array == null)
				{
					return null;
				}
				while (true)
				{
					int num = int.MinValue;
					int num2 = -1;
					int num3 = -1;
					bool flag = false;
					for (int i = 0; i < tagPairs.Count; i++)
					{
						if (bitArray[i])
						{
							continue;
						}
						for (int j = 0; j < tagPairs2.Count; j++)
						{
							if (!bitArray2[j])
							{
								if (array[i, j] > num)
								{
									num = array[i, j];
									num2 = i;
									num3 = j;
									flag = true;
								}
								else if (array[i, j] == num)
								{
									flag = false;
								}
							}
						}
					}
					if (num2 < 0)
					{
						break;
					}
					tagAssociations.Add(tagPairs[num2], tagPairs2[num3], EditOperation.Change);
					bitArray[num2] = true;
					bitArray2[num3] = true;
				}
			}
			for (int k = 0; k < tagPairs.Count; k++)
			{
				if (!bitArray[k])
				{
					tagAssociations.Add(tagPairs[k], null);
				}
			}
			for (int l = 0; l < tagPairs2.Count; l++)
			{
				if (!bitArray2[l])
				{
					tagAssociations.Add(null, tagPairs2[l]);
				}
			}
			return tagAssociations;
		}

		public static TagPairs FindPairedTags(IList<Token> tokens)
		{
			TagPairs tagPairs = null;
			for (int i = 0; i < tokens.Count; i++)
			{
				TagToken tagToken = tokens[i] as TagToken;
				if (tagToken == null || tagToken.Tag.Type != TagType.Start)
				{
					continue;
				}
				for (int j = i + 1; j < tokens.Count; j++)
				{
					TagToken tagToken2 = tokens[j] as TagToken;
					if (tagToken2 != null && tagToken2.Tag.Type == TagType.End && tagToken2.Tag.Anchor == tagToken.Tag.Anchor)
					{
						if (tagPairs == null)
						{
							tagPairs = new TagPairs();
						}
						tagPairs.Add(i, j, tagToken.Tag.Anchor);
						break;
					}
				}
			}
			return tagPairs;
		}

		private static int[,] ComputeTagAssociationScores(SimilarityMatrix similarityMatrix, TagPairs srcPairedTags, TagPairs trgPairedTags, bool useEndPositions)
		{
			int[,] array = new int[srcPairedTags.Count, trgPairedTags.Count];
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			TokenIndexLcsScoreProvider scorer = new TokenIndexLcsScoreProvider(similarityMatrix, 0.75, maySkip: true);
			for (int i = 0; i < similarityMatrix.SourceTokens.Count; i++)
			{
				list.Add(i);
			}
			for (int j = 0; j < similarityMatrix.TargetTokens.Count; j++)
			{
				list2.Add(j);
			}
			SequenceAlignmentComputer<int> sequenceAlignmentComputer = new SequenceAlignmentComputer<int>(list, list2, scorer, null, 1, 1);
			for (int num = srcPairedTags.Count - 1; num >= 0; num--)
			{
				PairedTag pairedTag = srcPairedTags[num];
				int num2 = useEndPositions ? pairedTag.End : pairedTag.Start;
				for (int num3 = trgPairedTags.Count - 1; num3 >= 0; num3--)
				{
					PairedTag pairedTag2 = trgPairedTags[num3];
					int num4 = useEndPositions ? pairedTag2.End : pairedTag2.Start;
					List<AlignedSubstring> list3 = sequenceAlignmentComputer.Compute(num2, num4);
					if (list3 != null && list3.Count > 0)
					{
						int num5 = list3[0].Score - (num2 - list3[0].Score) - (num4 - list3[0].Score);
						int num6;
						if (useEndPositions)
						{
							num6 = 0;
						}
						else
						{
							int tagSpan = GetTagSpan(pairedTag);
							int tagSpan2 = GetTagSpan(pairedTag2);
							num6 = Math.Abs(tagSpan - tagSpan2) / 2;
						}
						array[num, num3] = num5 - num6;
					}
				}
			}
			return array;
		}

		private static int GetTagSpan(PairedTag pt)
		{
			return pt.End - pt.Start - 1;
		}
	}
}
