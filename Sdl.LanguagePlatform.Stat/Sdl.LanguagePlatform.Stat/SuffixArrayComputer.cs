using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	internal class SuffixArrayComputer
	{
		public static List<Posting> Compute(DataLocation location, CultureInfo culture)
		{
			TokenFileReader tokenFileReader = new TokenFileReader(location, culture);
			List<IntSegment> corpus = new List<IntSegment>();
			List<Posting> list = new List<Posting>();
			tokenFileReader.Open();
			Posting item = default(Posting);
			for (int i = 0; i < tokenFileReader.Segments; i++)
			{
				IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
				corpus.Add(segmentAt);
				for (int j = 0; j < segmentAt.Count; j++)
				{
					item.Position = j;
					item.Segment = i;
					list.Add(item);
				}
			}
			tokenFileReader.Close();
			list.Sort(delegate(Posting a, Posting b)
			{
				if (a.Segment == b.Segment && a.Position == b.Position)
				{
					return 0;
				}
				int count = corpus[a.Segment].Count;
				int count2 = corpus[b.Segment].Count;
				int num = a.Position;
				int num2 = b.Position;
				int num3 = 0;
				while (num3 == 0 && num < count && num2 < count2)
				{
					num3 = corpus[a.Segment][num] - corpus[b.Segment][num2];
					num++;
					num2++;
				}
				if (num3 == 0)
				{
					if (num < count)
					{
						num3 = 1;
					}
					else if (num2 < count2)
					{
						num3 = -1;
					}
				}
				if (num3 == 0)
				{
					num3 = a.Segment - b.Segment;
				}
				return (num3 != 0 || a.Position == b.Position) ? num3 : (a.Position - b.Position);
			});
			string componentFileName = location.GetComponentFileName(DataFileType.SuffixArray, culture);
			IntegerFileWriter integerFileWriter = new IntegerFileWriter(componentFileName);
			integerFileWriter.Create();
			for (int k = 0; k < list.Count; k++)
			{
				integerFileWriter.Write(list[k].Segment);
				integerFileWriter.Write(list[k].Position);
			}
			integerFileWriter.Close();
			return list;
		}

		public static List<Posting> Load(DataLocation location, CultureInfo culture)
		{
			string componentFileName = location.GetComponentFileName(DataFileType.SuffixArray, culture);
			return Load(componentFileName);
		}

		public static List<Posting> Load(string fileName)
		{
			using (IntegerFileReader integerFileReader = new IntegerFileReader(fileName))
			{
				integerFileReader.Open();
				if (integerFileReader.Items % 2 != 0)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				List<Posting> list = new List<Posting>();
				int num = 0;
				Posting item = default(Posting);
				while (num < integerFileReader.Items)
				{
					item.Segment = integerFileReader[num++];
					item.Position = integerFileReader[num++];
					list.Add(item);
				}
				integerFileReader.Close();
				return list;
			}
		}
	}
}
