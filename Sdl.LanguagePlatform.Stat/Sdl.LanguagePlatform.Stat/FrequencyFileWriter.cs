using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class FrequencyFileWriter
	{
		private readonly List<int> _counts;

		private readonly DataFileInfo _info;

		public int this[int key]
		{
			get
			{
				return _counts[key];
			}
			set
			{
				_counts[key] = value;
			}
		}

		public FrequencyFileWriter(DataLocation location, CultureInfo culture)
		{
			_info = location.FindComponent(DataFileType.FrequencyCountsFile, culture);
			_counts = new List<int>();
		}

		public void Save()
		{
			using (IntegerFileWriter integerFileWriter = new IntegerFileWriter(_info.FileName))
			{
				integerFileWriter.Create();
				foreach (int count in _counts)
				{
					integerFileWriter.Write(count);
				}
				integerFileWriter.Close();
			}
		}

		public void Inc(int key)
		{
			while (key >= _counts.Count)
			{
				_counts.Add(0);
			}
			_counts[key]++;
		}
	}
}
