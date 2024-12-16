using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class BilingualPhraseComputer2
	{
		public class Settings
		{
			public int MaximumPhraseLength = 8;

			public double MinimumLikelihood = 0.01;

			public bool Incremental;
		}

		private const int MinTokenFrequency = 2;

		private readonly IBilingualPhraseCounter2 _phrases;

		private readonly Action<int, int, int, int> _recordPhraseAction;

		private readonly Settings _settings;

		private readonly int _srcPunctuationId;

		private readonly int _trgPunctuationId;

		private readonly List<int> _srcStopwords;

		private readonly List<int> _trgStopwords;

		private readonly List<int> _srcPlaceables;

		private readonly List<int> _trgPlaceables;

		private readonly int[] _srcFrequencies;

		private readonly int[] _trgFrequencies;

		private bool[] _isSourceAllowedAtBorder;

		private bool[] _srcMayInclude;

		private bool[] _isTargetAllowedAtBorder;

		private bool[] _mayExtendTargetTo;

		public BilingualPhraseComputer2(CultureInfo srcCulture, CultureInfo trgCulture, IBilingualPhraseCounter2 phrases, Action<int, int, int, int> recordPhraseAction, VocabularyFile2 srcVocab, VocabularyFile2 trgVocab)
			: this(srcCulture, trgCulture, new Settings(), phrases, recordPhraseAction, srcVocab, trgVocab)
		{
		}

		public BilingualPhraseComputer2(CultureInfo srcCulture, CultureInfo trgCulture, Settings settings, IBilingualPhraseCounter2 phrases, Action<int, int, int, int> recordPhraseAction, VocabularyFile2 srcVocab, VocabularyFile2 trgVocab)
		{
			_phrases = phrases;
			_recordPhraseAction = recordPhraseAction;
			IResourceDataAccessor resourceDataAccessor = new ResourceFileResourceAccessor();
			_settings = (settings ?? new Settings());
			_srcPunctuationId = srcVocab.Lookup("{{PCT}}");
			_srcPlaceables = new List<int>();
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
					_srcPlaceables.Add(item);
				}
			}
			if (resourceDataAccessor.GetResourceStatus(srcCulture, LanguageResourceType.Stopwords, fallback: true) != ResourceStatus.NotAvailable)
			{
				_srcStopwords = srcVocab.GetStopwordIDs(srcCulture, resourceDataAccessor);
			}
			_trgPunctuationId = trgVocab.Lookup("{{PCT}}");
			_trgPlaceables = new List<int>();
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
					_trgPlaceables.Add(item2);
				}
			}
			if (resourceDataAccessor.GetResourceStatus(trgCulture, LanguageResourceType.Stopwords, fallback: true) != ResourceStatus.NotAvailable)
			{
				_trgStopwords = trgVocab.GetStopwordIDs(trgCulture, resourceDataAccessor);
			}
			_srcFrequencies = null;
			_trgFrequencies = null;
		}

		private void ComputeArrays(AlignmentTable at, IntSegment srcSegment, IntSegment trgSegment)
		{
			_isSourceAllowedAtBorder = new bool[srcSegment.Count];
			_srcMayInclude = new bool[srcSegment.Count];
			for (int i = 0; i < srcSegment.Count; i++)
			{
				int num = srcSegment[i];
				_isSourceAllowedAtBorder[i] = IsSourceAllowedAtBorder(num);
				_srcMayInclude[i] = SourceMayInclude(srcSegment, i, num);
			}
			_isTargetAllowedAtBorder = new bool[trgSegment.Count];
			_mayExtendTargetTo = new bool[trgSegment.Count];
			for (int j = 0; j < trgSegment.Count; j++)
			{
				int num2 = trgSegment[j];
				_isTargetAllowedAtBorder[j] = IsTargetAllowedAtBorder(num2);
				_mayExtendTargetTo[j] = MayExtendTargetTo(at, trgSegment, j, num2);
			}
		}

		public void Compute(IntSegment srcSegment, IntSegment trgSegment, AlignmentTable at, List<BilingualPhrase> phrases)
		{
			if (srcSegment.Count != at.SourceSegmentLength || trgSegment.Count != at.TargetSegmentLength)
			{
				throw new LanguagePlatformException(ErrorCode.CorruptData, "Alignment table dimensions don't fit segment lengths");
			}
			ComputeArrays(at, srcSegment, trgSegment);
			for (int i = 0; i < srcSegment.Count; i++)
			{
				if (!_isSourceAllowedAtBorder[i])
				{
					continue;
				}
				for (int j = i; j < srcSegment.Count && _srcMayInclude[j]; j++)
				{
					if (!_isSourceAllowedAtBorder[j])
					{
						continue;
					}
					at.GetSourceToTargetAlignedRange(i, j, out int trgStart, out int trgInto, out int srcHoles);
					if (trgStart < 0 || trgInto < 0)
					{
						continue;
					}
					at.GetTargetToSourceAlignedRange(trgStart, trgInto, out int srcStart, out int srcInto, out srcHoles);
					if (srcStart != i || srcInto != j)
					{
						continue;
					}
					int num = j - i + 1;
					int num2 = trgStart;
					while (num2 >= 0 && (num2 >= trgStart || _mayExtendTargetTo[num2]))
					{
						if (_isTargetAllowedAtBorder[num2])
						{
							for (int k = trgInto; k < trgSegment.Count && (k <= trgInto || _mayExtendTargetTo[k]); k++)
							{
								if (!_isTargetAllowedAtBorder[k])
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
								if (num > _settings.MaximumPhraseLength || num3 > _settings.MaximumPhraseLength)
								{
									flag = true;
								}
								if (num == srcSegment.Count && num3 == trgSegment.Count)
								{
									flag = true;
								}
								if (!flag)
								{
									List<int> list = new List<int>();
									List<int> list2 = new List<int>();
									for (int l = i; l <= j; l++)
									{
										list.Add(srcSegment[l]);
									}
									for (int m = num2; m <= k; m++)
									{
										list2.Add(trgSegment[m]);
									}
									_phrases?.CountBilingualPhrase(list, list2);
									_recordPhraseAction?.Invoke(i, j - i + 1, num2, k - num2 + 1);
								}
							}
						}
						num2--;
					}
				}
			}
		}

		private static bool IsStopword(List<int> list, int v)
		{
			if (list == null)
			{
				return false;
			}
			return list.BinarySearch(v) >= 0;
		}

		private bool IsSourceAllowedAtBorder(int s)
		{
			if (s == _srcPunctuationId)
			{
				return false;
			}
			if (_srcPlaceables.Contains(s))
			{
				return false;
			}
			if (IsStopword(_srcStopwords, s))
			{
				return false;
			}
			if (!_settings.Incremental)
			{
				if (_srcFrequencies != null)
				{
					return _srcFrequencies[s] >= 2;
				}
				return true;
			}
			if (_srcFrequencies != null && s >= _srcFrequencies.Length)
			{
				return false;
			}
			if (_srcFrequencies != null)
			{
				return _srcFrequencies[s] >= 2;
			}
			return true;
		}

		private bool IsTargetAllowedAtBorder(int t)
		{
			if (t == _trgPunctuationId)
			{
				return false;
			}
			if (_trgPlaceables.Contains(t))
			{
				return false;
			}
			if (IsStopword(_trgStopwords, t))
			{
				return false;
			}
			if (!_settings.Incremental)
			{
				if (_trgFrequencies != null)
				{
					return _trgFrequencies[t] >= 2;
				}
				return true;
			}
			if (_trgFrequencies != null && t >= _trgFrequencies.Length)
			{
				return false;
			}
			if (_trgFrequencies != null)
			{
				return _trgFrequencies[t] >= 2;
			}
			return true;
		}

		private static bool IsPlaceableOrPunctuation(int punctionId, ICollection<int> placeables, int w)
		{
			if (w != punctionId)
			{
				return placeables.Contains(w);
			}
			return true;
		}

		private bool SourceMayInclude(IntSegment segment, int position, int w)
		{
			if (position < 0 || position >= segment.Count)
			{
				return false;
			}
			if (IsPlaceableOrPunctuation(_srcPunctuationId, _srcPlaceables, w))
			{
				return false;
			}
			if (!_settings.Incremental)
			{
				if (_srcFrequencies != null)
				{
					return _srcFrequencies[segment[position]] >= 2;
				}
				return true;
			}
			if (_srcFrequencies != null && segment[position] >= _srcFrequencies.Length)
			{
				return false;
			}
			if (_srcFrequencies != null)
			{
				return _srcFrequencies[segment[position]] >= 2;
			}
			return true;
		}

		private bool MayExtendTargetTo(AlignmentTable at, IntSegment segment, int position, int w)
		{
			if (position < 0 || position >= segment.Count)
			{
				return false;
			}
			if (IsPlaceableOrPunctuation(_trgPunctuationId, _trgPlaceables, w))
			{
				return false;
			}
			if (!at.IsTargetAligned(position))
			{
				return _isTargetAllowedAtBorder[position];
			}
			return false;
		}
	}
}
