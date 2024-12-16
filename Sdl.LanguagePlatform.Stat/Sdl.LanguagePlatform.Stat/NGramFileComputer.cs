using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class NGramFileComputer
	{
		private readonly DataLocation _location;

		private readonly CultureInfo _culture;

		private readonly int _n;

		public NGramFileComputer(DataLocation location, CultureInfo culture, int n)
		{
			_location = (location ?? throw new ArgumentNullException("location"));
			_culture = (culture ?? throw new ArgumentNullException("culture"));
			_n = n;
			if (_n != 2 && _n != 3)
			{
				throw new ArgumentException($"ngram width {_n} not supported");
			}
			_location.ExpectComponent(DataFileType.TokenFile, _culture);
		}

		public void Compute()
		{
			NGramComputer nGramComputer = new NGramComputer(_n, _location.Directory.FullName, "ng" + _n.ToString() + _culture.TwoLetterISOLanguageName);
			using (TokenFileReader tokenFileReader = new TokenFileReader(_location, _culture))
			{
				tokenFileReader.Open();
				for (int i = 0; i < tokenFileReader.Segments; i++)
				{
					IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
					CountNGrams(nGramComputer, segmentAt);
				}
			}
			DataFileType t;
			switch (_n)
			{
			case 3:
				t = DataFileType.NGram3CountsFile;
				break;
			case 2:
				t = DataFileType.NGram2CountsFile;
				break;
			default:
				throw new InvalidOperationException();
			}
			string componentFileName = _location.GetComponentFileName(t, _culture);
			nGramComputer.FinishAndSave(0, componentFileName);
		}

		internal static void CountNGrams(NGramComputer computer, IntSegment s)
		{
			int n = computer.N;
			int[] array = new int[n];
			int count = s.Count;
			for (int i = 1 - n; i < count; i++)
			{
				for (int j = 0; j < n; j++)
				{
					int num = i + j;
					array[j] = ((num < 0) ? (-1) : ((num >= count) ? (-1) : s[num]));
				}
				computer.Count(array);
			}
		}
	}
}
