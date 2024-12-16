using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	internal class FrequencyFileReader : IntegerFileReader
	{
		public FrequencyFileReader(DataLocation location, CultureInfo culture)
			: base(location.FindComponent(DataFileType.FrequencyCountsFile, culture).FileName, -1)
		{
		}
	}
}
