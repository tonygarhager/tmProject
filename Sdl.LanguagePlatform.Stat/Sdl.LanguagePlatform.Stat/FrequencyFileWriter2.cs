using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class FrequencyFileWriter2 : IDisposable, IOccurrenceCounter
	{
		protected List<int> Counts;

		private readonly DataFileInfo2 _info;

		public int this[int key]
		{
			get
			{
				return Counts[key];
			}
			set
			{
				Counts[key] = value;
			}
		}

		public FrequencyFileWriter2(DataLocation2 location, CultureInfo culture)
		{
			_info = location.FindComponent(DataFileType.FrequencyCountsFile, culture);
			Counts = new List<int>();
		}

		public virtual void Save()
		{
			using (IntegerFileWriter integerFileWriter = new IntegerFileWriter(_info.FileName))
			{
				integerFileWriter.Create();
				foreach (int count in Counts)
				{
					integerFileWriter.Write(count);
				}
				integerFileWriter.Close();
			}
		}

		public void Inc(int key)
		{
			while (key >= Counts.Count)
			{
				Counts.Add(0);
			}
			Counts[key]++;
		}

		public void Dispose()
		{
		}
	}
}
