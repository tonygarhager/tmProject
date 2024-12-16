using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	internal class FrequencyFileReader2 : IntegerFileReader
	{
		public FrequencyFileReader2(DataLocation2 location, CultureInfo culture)
			: base(location.FindComponent(DataFileType.FrequencyCountsFile, culture).FileName, -1)
		{
		}
	}
}
