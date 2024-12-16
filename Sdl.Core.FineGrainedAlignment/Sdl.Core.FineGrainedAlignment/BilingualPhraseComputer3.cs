using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Stat;
using Sdl.LanguagePlatform.Stat.WordAlignment;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public class BilingualPhraseComputer3
	{
		public class Settings
		{
			public int MaximumPhraseLength = 8;

			public double MinimumLikelihood = 0.01;

			public bool Incremental;
		}

		private static readonly bool AllowPunctuationInPeripheralPositions;

		private static readonly bool AllowStopwordsInPeripheralPositions;

		private static readonly bool AllowPlaceablesInPeripheralPositions;

		private const int MIN_TOKEN_FREQUENCY = 2;

		private readonly IBilingualPhraseCounter2 _Phrases;

		private readonly CultureInfo _SrcCulture;

		private readonly CultureInfo _TrgCulture;

		private readonly Settings _Settings;

		private readonly int _srcPunctuationId = -1;

		private readonly int _trgPunctuationId = -1;

		private readonly List<int> _SrcStopwords;

		private readonly List<int> _TrgStopwords;

		private readonly List<int> _SrcPlaceables;

		private readonly List<int> _TrgPlaceables;

		private readonly int[] _SrcFrequencies;

		private readonly int[] _TrgFrequencies;

		private bool[] _IsSourceAllowedAtBorder;

		private bool[] _SrcMayInclude;

		private bool[] _IsTargetAllowedAtBorder;

		private bool[] _MayExtendTargetTo;

		public BilingualPhraseComputer3(CultureInfo srcCulture, CultureInfo trgCulture, IBilingualPhraseCounter2 phrases, Action<int, int, int, int> recordPhraseAction, VocabularyFile3 srcVocab, VocabularyFile3 trgVocab)
			: this(srcCulture, trgCulture, new Settings(), phrases, srcVocab, trgVocab)
		{
		}

		public BilingualPhraseComputer3(CultureInfo srcCulture, CultureInfo trgCulture, Settings settings, IBilingualPhraseCounter2 phrases, VocabularyFile3 srcVocab, VocabularyFile3 trgVocab)
		{
			_Phrases = phrases;
			_SrcCulture = srcCulture;
			_TrgCulture = trgCulture;
			IResourceDataAccessor resourceDataAccessor = new ResourceFileResourceAccessor();
			_Settings = (settings ?? new Settings());
			_srcPunctuationId = srcVocab.Lookup("{{PCT}}");
			_SrcPlaceables = new List<int>();
			string[] array = new string[3]
			{
				"{{NUM}}",
				"{{DAT}}",
				"{{MSR}}"
			};
			foreach (string s in array)
			{
				int item;
				if ((item = srcVocab.Lookup(s)) >= 0)
				{
					_SrcPlaceables.Add(item);
				}
			}
			if (resourceDataAccessor.GetResourceStatus(_SrcCulture, LanguageResourceType.Stopwords, fallback: true) != ResourceStatus.NotAvailable)
			{
				_SrcStopwords = srcVocab.GetStopwordIDs(_SrcCulture, resourceDataAccessor);
			}
			_trgPunctuationId = trgVocab.Lookup("{{PCT}}");
			_TrgPlaceables = new List<int>();
			string[] array2 = new string[3]
			{
				"{{NUM}}",
				"{{DAT}}",
				"{{MSR}}"
			};
			foreach (string s2 in array2)
			{
				int item2;
				if ((item2 = trgVocab.Lookup(s2)) >= 0)
				{
					_TrgPlaceables.Add(item2);
				}
			}
			if (resourceDataAccessor.GetResourceStatus(_TrgCulture, LanguageResourceType.Stopwords, fallback: true) != ResourceStatus.NotAvailable)
			{
				_TrgStopwords = trgVocab.GetStopwordIDs(_TrgCulture, resourceDataAccessor);
			}
			_SrcFrequencies = null;
			_TrgFrequencies = null;
		}

		private void ComputeArrays(AlignmentTable at, IntSegment srcSegment, IntSegment trgSegment)
		{
			_IsSourceAllowedAtBorder = new bool[srcSegment.Count];
			_SrcMayInclude = new bool[srcSegment.Count];
			for (int i = 0; i < srcSegment.Count; i++)
			{
				int num = srcSegment[i];
				_IsSourceAllowedAtBorder[i] = IsSourceAllowedAtBorder(num);
				_SrcMayInclude[i] = SourceMayInclude(at, srcSegment, i, num);
			}
			_IsTargetAllowedAtBorder = new bool[trgSegment.Count];
			_MayExtendTargetTo = new bool[trgSegment.Count];
			for (int j = 0; j < trgSegment.Count; j++)
			{
				int num2 = trgSegment[j];
				_IsTargetAllowedAtBorder[j] = IsTargetAllowedAtBorder(num2);
				_MayExtendTargetTo[j] = MayExtendTargetTo(at, trgSegment, j, num2);
			}
		}

		public List<AlignedPhrase> Compute(IntSegment srcSegment, IntSegment trgSegment, AlignmentTable at)
		{
			List<AlignedPhrase> list = new List<AlignedPhrase>();
			if (srcSegment.Count != at.SourceSegmentLength || trgSegment.Count != at.TargetSegmentLength)
			{
				throw new LanguagePlatformException(ErrorCode.CorruptData, "Alignment table dimensions don't fit segment lengths");
			}
			ComputeArrays(at, srcSegment, trgSegment);
			for (int i = 0; i < srcSegment.Count; i++)
			{
				if (!_IsSourceAllowedAtBorder[i])
				{
					continue;
				}
				for (int j = i; j < srcSegment.Count && _SrcMayInclude[j]; j++)
				{
					if (!_IsSourceAllowedAtBorder[j])
					{
						continue;
					}
					int trgStart = -1;
					int trgInto = -1;
					int srcHoles = 0;
					int trgHoles = 0;
					at.GetSourceToTargetAlignedRange(i, j, out trgStart, out trgInto, out srcHoles);
					if (trgStart < 0 || trgInto < 0)
					{
						continue;
					}
					int srcStart = -1;
					int srcInto = -1;
					at.GetTargetToSourceAlignedRange(trgStart, trgInto, out srcStart, out srcInto, out trgHoles);
					if (srcStart != i || srcInto != j)
					{
						continue;
					}
					int num = j - i + 1;
					int num2 = trgStart;
					while (num2 >= 0 && (num2 >= trgStart || _MayExtendTargetTo[num2]))
					{
						if (_IsTargetAllowedAtBorder[num2])
						{
							for (int k = trgInto; k < trgSegment.Count && (k <= trgInto || _MayExtendTargetTo[k]); k++)
							{
								if (!_IsTargetAllowedAtBorder[k])
								{
									continue;
								}
								bool flag = false;
								int num3 = k - num2 + 1;
								if (num == 1)
								{
									_ = 1;
								}
								if (num != srcSegment.Count)
								{
									_ = trgSegment.Count;
								}
								if (num > _Settings.MaximumPhraseLength || num3 > _Settings.MaximumPhraseLength)
								{
									flag = ((byte)((flag ? 1 : 0) | 1) != 0);
								}
								if (num == srcSegment.Count && num3 == trgSegment.Count)
								{
									flag = ((byte)((flag ? 1 : 0) | 1) != 0);
								}
								if (!flag)
								{
									List<int> list2 = new List<int>();
									List<int> list3 = new List<int>();
									for (int l = i; l <= j; l++)
									{
										list2.Add(srcSegment[l]);
									}
									for (int m = num2; m <= k; m++)
									{
										list3.Add(trgSegment[m]);
									}
									if (_Phrases != null)
									{
										_Phrases.CountBilingualPhrase(list2, list3);
									}
									AlignedPhrase item = new AlignedPhrase(i, j - i + 1, num2, k - num2 + 1, -1f);
									list.Add(item);
								}
							}
						}
						num2--;
					}
				}
			}
			return list;
		}

		private bool IsStopword(List<int> list, int v)
		{
			if (list == null)
			{
				return false;
			}
			return list.BinarySearch(v) >= 0;
		}

		private bool IsSourceAllowedAtBorder(int s)
		{
			if (s == _srcPunctuationId && !AllowPunctuationInPeripheralPositions)
			{
				return false;
			}
			if (_SrcPlaceables.Contains(s) && !AllowPlaceablesInPeripheralPositions)
			{
				return false;
			}
			if (!AllowStopwordsInPeripheralPositions && IsStopword(_SrcStopwords, s))
			{
				return false;
			}
			if (_Settings.Incremental && _SrcFrequencies != null && s >= _SrcFrequencies.Length)
			{
				return false;
			}
			if (_SrcFrequencies != null && _SrcFrequencies[s] < 2)
			{
				return false;
			}
			return true;
		}

		private bool IsTargetAllowedAtBorder(int t)
		{
			if (t == _trgPunctuationId && !AllowPunctuationInPeripheralPositions)
			{
				return false;
			}
			if (_TrgPlaceables.Contains(t) && !AllowPlaceablesInPeripheralPositions)
			{
				return false;
			}
			if (!AllowStopwordsInPeripheralPositions && IsStopword(_TrgStopwords, t))
			{
				return false;
			}
			if (_Settings.Incremental && _TrgFrequencies != null && t >= _TrgFrequencies.Length)
			{
				return false;
			}
			if (_TrgFrequencies != null && _TrgFrequencies[t] < 2)
			{
				return false;
			}
			return true;
		}

		private static bool IsPlaceableOrPunctuation(int punctionId, List<int> placeables, int w)
		{
			if (w != punctionId)
			{
				return placeables.Contains(w);
			}
			return true;
		}

		private bool SourceMayInclude(AlignmentTable at, IntSegment segment, int position, int w)
		{
			if (position < 0 || position >= segment.Count)
			{
				return false;
			}
			if (_Settings.Incremental && _SrcFrequencies != null && segment[position] >= _SrcFrequencies.Length)
			{
				return false;
			}
			if (_SrcFrequencies != null && _SrcFrequencies[segment[position]] < 2)
			{
				return false;
			}
			return true;
		}

		private bool MayExtendTargetTo(AlignmentTable at, IntSegment segment, int position, int w)
		{
			if (position < 0 || position >= segment.Count)
			{
				return false;
			}
			if (IsPlaceableOrPunctuation(_trgPunctuationId, _TrgPlaceables, w))
			{
				return false;
			}
			if (!at.IsTargetAligned(position))
			{
				return _IsTargetAllowedAtBorder[position];
			}
			return false;
		}
	}
}
