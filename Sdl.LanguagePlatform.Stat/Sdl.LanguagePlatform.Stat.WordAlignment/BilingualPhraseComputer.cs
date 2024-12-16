using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal class BilingualPhraseComputer
	{
		public class Settings
		{
			public int MaximumPhraseLength = 8;

			public double MinimumLikelihood = 0.01;
		}

		private const int MinTokenFrequency = 2;

		private readonly IBilingualPhraseCounter _phrases;

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

		public BilingualPhraseComputer(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture, IBilingualPhraseCounter phrases)
			: this(location, srcCulture, trgCulture, new Settings(), phrases)
		{
		}

		public BilingualPhraseComputer(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture, Settings settings, IBilingualPhraseCounter phrases)
		{
			_phrases = (phrases ?? throw new ArgumentNullException("phrases"));
			IResourceDataAccessor resourceDataAccessor = new ResourceFileResourceAccessor();
			_settings = (settings ?? new Settings());
			VocabularyFile vocabulary = location.GetVocabulary(srcCulture);
			vocabulary.Load();
			_srcPunctuationId = vocabulary.Lookup("{{PCT}}");
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
				if ((item = vocabulary.Lookup(s)) >= 0)
				{
					_srcPlaceables.Add(item);
				}
			}
			if (resourceDataAccessor.GetResourceStatus(srcCulture, LanguageResourceType.Stopwords, fallback: true) != ResourceStatus.NotAvailable)
			{
				_srcStopwords = vocabulary.GetStopwordIDs(srcCulture, resourceDataAccessor);
			}
			VocabularyFile vocabulary2 = location.GetVocabulary(trgCulture);
			vocabulary2.Load();
			_trgPunctuationId = vocabulary2.Lookup("{{PCT}}");
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
				if ((item2 = vocabulary2.Lookup(s2)) >= 0)
				{
					_trgPlaceables.Add(item2);
				}
			}
			if (resourceDataAccessor.GetResourceStatus(trgCulture, LanguageResourceType.Stopwords, fallback: true) != ResourceStatus.NotAvailable)
			{
				_trgStopwords = vocabulary2.GetStopwordIDs(trgCulture, resourceDataAccessor);
			}
			if (location.HasComponent(DataFileType.FrequencyCountsFile, srcCulture) && location.HasComponent(DataFileType.FrequencyCountsFile, trgCulture))
			{
				using (FrequencyFileReader frequencyFileReader = new FrequencyFileReader(location, srcCulture))
				{
					_srcFrequencies = IntegerFileReader.Load(frequencyFileReader.DataFileName);
				}
				using (FrequencyFileReader frequencyFileReader2 = new FrequencyFileReader(location, trgCulture))
				{
					_trgFrequencies = IntegerFileReader.Load(frequencyFileReader2.DataFileName);
				}
			}
			else
			{
				_srcFrequencies = null;
				_trgFrequencies = null;
			}
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
									_phrases.CountBilingualPhrase(list, list2);
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
			if (s != _srcPunctuationId && !_srcPlaceables.Contains(s) && !IsStopword(_srcStopwords, s))
			{
				if (_srcFrequencies != null)
				{
					return _srcFrequencies[s] >= 2;
				}
				return true;
			}
			return false;
		}

		private bool IsTargetAllowedAtBorder(int t)
		{
			if (t != _trgPunctuationId && !_trgPlaceables.Contains(t) && !IsStopword(_trgStopwords, t))
			{
				if (_trgFrequencies != null)
				{
					return _trgFrequencies[t] >= 2;
				}
				return true;
			}
			return false;
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
