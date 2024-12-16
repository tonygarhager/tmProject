using Sdl.LanguagePlatform.Lingua;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	internal class SuffixArray : IDisposable
	{
		private readonly DataFileInfo _dataFile;

		private readonly DataFileInfo _frequenciesFile;

		private readonly DataFileInfo _corpusFile;

		private int[] _frequencies;

		private List<Posting> _suffixArray;

		private List<IntSegment> _corpus;

		public bool Exists
		{
			get
			{
				if (_dataFile.Exists && _frequenciesFile.Exists)
				{
					return _corpusFile.Exists;
				}
				return false;
			}
		}

		public SuffixArray(DataLocation location, CultureInfo culture)
		{
			_dataFile = location.FindComponent(DataFileType.SuffixArray, culture);
			_frequenciesFile = location.FindComponent(DataFileType.FrequencyCountsFile, culture);
			_corpusFile = location.FindComponent(DataFileType.TokenFile, culture);
		}

		public void Load()
		{
			if (_frequencies == null)
			{
				_frequencies = IntegerFileReader.Load(_frequenciesFile.FileName);
			}
			if (_suffixArray == null)
			{
				_suffixArray = SuffixArrayComputer.Load(_dataFile.FileName);
			}
			if (_corpus == null)
			{
				_corpus = new List<IntSegment>();
				using (TokenFileReader tokenFileReader = new TokenFileReader(_corpusFile.Location, _corpusFile.SourceCulture))
				{
					tokenFileReader.Open();
					for (int i = 0; i < tokenFileReader.Segments; i++)
					{
						_corpus.Add(tokenFileReader.GetSegmentAt(i));
					}
					tokenFileReader.Close();
				}
			}
		}

		public void FuzzySearch(IList<int> searchSegment)
		{
			if (searchSegment.Count == 0)
			{
				return;
			}
			List<List<Range<int>>> list = new List<List<Range<int>>>
			{
				[0] = Search(searchSegment, 0, 0)
			};
			for (int num = list[0].Count; num > 0; num--)
			{
				Range<int> range = list[0][num - 1];
				for (int i = range.Start; i < range.End; i++)
				{
					Posting posting = _suffixArray[i];
				}
			}
		}

		public List<Range<int>> Search(IList<int> searchSegment)
		{
			return Search(searchSegment, 0, 0);
		}

		public List<Range<int>> Search(IList<int> searchSegment, int startIndex)
		{
			return Search(searchSegment, startIndex, 0);
		}

		public List<Range<int>> Search(IList<int> searchSegment, int startIndex, int maxLength)
		{
			int num = 0;
			int num2 = _suffixArray.Count;
			List<Range<int>> list = new List<Range<int>>();
			if (searchSegment == null || startIndex >= searchSegment.Count || searchSegment[0] < 0)
			{
				return list;
			}
			int num3 = 0;
			if (_frequencies != null)
			{
				for (int i = 0; i < searchSegment[startIndex]; i++)
				{
					num += _frequencies[i];
				}
				num2 = num + _frequencies[searchSegment[startIndex]];
				num3 = 1;
				list.Add(new Range<int>(num, num2));
			}
			bool flag = true;
			while (startIndex + num3 < searchSegment.Count && flag && (maxLength <= 0 || num3 < maxLength))
			{
				int num4 = num;
				int num5 = num2 - 1;
				flag = false;
				int num6 = searchSegment[startIndex + num3];
				if (num6 < 0)
				{
					break;
				}
				while (num4 <= num5 && !flag)
				{
					int num7 = (num4 + num5) / 2;
					Posting posting = _suffixArray[num7];
					if (posting.Position + num3 >= _corpus[posting.Segment].Count)
					{
						num4 = num7 + 1;
						continue;
					}
					int num8 = _corpus[posting.Segment][posting.Position + num3];
					if (num8 > num6)
					{
						num5 = num7 - 1;
						continue;
					}
					if (num8 < num6)
					{
						num4 = num7 + 1;
						continue;
					}
					flag = true;
					int num9 = num;
					int num10 = num2;
					num = num7 - 1;
					if (num >= 0)
					{
						posting = _suffixArray[num];
						while (num >= 0 && num >= num9 && posting.Position + num3 < _corpus[posting.Segment].Count && _corpus[posting.Segment][posting.Position + num3] == num8)
						{
							num--;
							if (num >= 0)
							{
								posting = _suffixArray[num];
							}
						}
					}
					num++;
					num2 = num7 + 1;
					if (num2 >= _suffixArray.Count)
					{
						continue;
					}
					posting = _suffixArray[num2];
					while (num2 < _suffixArray.Count && num2 < num10 && posting.Position + num3 < _corpus[posting.Segment].Count && _corpus[posting.Segment][posting.Position + num3] == num8)
					{
						num2++;
						if (num2 < _suffixArray.Count)
						{
							posting = _suffixArray[num2];
						}
					}
				}
				if (flag)
				{
					list.Add(new Range<int>(num, num2));
					num3++;
				}
			}
			return list;
		}

		public void Dispose()
		{
			_frequencies = null;
			_suffixArray = null;
			_corpus = null;
		}

		public void Test()
		{
			DateTime now = DateTime.Now;
			int num = 0;
			foreach (IntSegment corpu in _corpus)
			{
				List<Range<int>> list = Search(corpu.Elements);
				int count = list.Count;
				if (num > 0 && num % 1000 == 0)
				{
					TimeSpan timeSpan = DateTime.Now - now;
				}
				num++;
			}
		}
	}
}
