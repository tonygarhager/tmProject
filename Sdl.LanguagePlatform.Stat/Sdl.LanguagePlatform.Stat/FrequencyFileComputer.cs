using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	internal class FrequencyFileComputer
	{
		private readonly DataLocation _location;

		private readonly CultureInfo _culture;

		public FrequencyFileComputer(DataLocation location, CultureInfo culture)
		{
			_location = location;
			_culture = culture;
		}

		public void Compute()
		{
			using (TokenFileReader tokenFileReader = new TokenFileReader(_location, _culture))
			{
				FrequencyFileWriter frequencyFileWriter = new FrequencyFileWriter(_location, _culture);
				tokenFileReader.Open();
				for (int i = 0; i < tokenFileReader.Segments; i++)
				{
					IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
					for (int j = 0; j < segmentAt.Count; j++)
					{
						frequencyFileWriter.Inc(segmentAt[j]);
					}
				}
				tokenFileReader.Close();
				frequencyFileWriter.Save();
			}
		}
	}
}
