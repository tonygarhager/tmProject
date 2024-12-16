using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class InvertedFileComputer
	{
		private struct InvertedCell : IComparable<InvertedCell>
		{
			public int Key;

			public int Segment;

			public int Position;

			public int CompareTo(InvertedCell other)
			{
				if (Key < 0)
				{
					if (other.Key >= 0)
					{
						return 1;
					}
					return 0;
				}
				if (other.Key < 0)
				{
					return -1;
				}
				int num = Key - other.Key;
				if (num == 0)
				{
					num = Segment - other.Segment;
				}
				if (num == 0)
				{
					num = Position - other.Position;
				}
				return num;
			}

			public void Write(BinaryWriter wtr)
			{
				wtr.Write(Key);
				wtr.Write(Segment);
				wtr.Write(Position);
			}

			public void Read(BinaryReader rdr)
			{
				Key = rdr.ReadInt32();
				Segment = rdr.ReadInt32();
				Position = rdr.ReadInt32();
			}
		}

		private class InvertedCellReaderWriter : IBinaryReaderWriter<InvertedCell>
		{
			public bool Read(BinaryReader reader, out InvertedCell item)
			{
				item = default(InvertedCell);
				if (reader.BaseStream.Position >= reader.BaseStream.Length)
				{
					return false;
				}
				item.Read(reader);
				return true;
			}

			public void Write(BinaryWriter writer, InvertedCell item)
			{
				item.Write(writer);
			}
		}

		private const int Bufsize = 1048576;

		private DataLocation _location;

		private CultureInfo _culture;

		private DiskBasedMergeSorter<InvertedCell> _sorter;

		public void Invert(DataLocation location, CultureInfo culture)
		{
			_location = (location ?? throw new ArgumentNullException("location"));
			_culture = (culture ?? throw new ArgumentNullException("culture"));
			_sorter = new DiskBasedMergeSorter<InvertedCell>(new InvertedCellReaderWriter(), unique: true, "ifb", _location.Directory.FullName, 1048576);
			string inputFileName = Generate();
			CreateFinalIndexFiles(inputFileName);
		}

		private string Generate()
		{
			using (TokenFileReader tokenFileReader = new TokenFileReader(_location, _culture))
			{
				if (!tokenFileReader.Exists)
				{
					throw new ArgumentException("The required token file does not exist");
				}
				tokenFileReader.Open();
				InvertedCell item = default(InvertedCell);
				for (int i = 0; i < tokenFileReader.Segments; i++)
				{
					IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
					for (int j = 0; j < segmentAt.Count; j++)
					{
						item.Segment = i;
						item.Position = j;
						item.Key = segmentAt[j];
						_sorter.Add(item);
					}
				}
				tokenFileReader.Close();
				return _sorter.FinishCounting();
			}
		}

		private static void ReadCell(ref InvertedCell c, IntegerFileReader rdr, ref int pos)
		{
			c.Key = rdr[pos++];
			c.Segment = rdr[pos++];
			c.Position = rdr[pos++];
		}

		private void CreateFinalIndexFiles(string inputFileName)
		{
			DataFileInfo dataFileInfo = new DataFileInfo(_location, DataFileType.TokenInvertedFile, _culture, null);
			DataFileInfo dataFileInfo2 = new DataFileInfo(_location, DataFileType.TokenInvertedFileIndex, _culture, null);
			int num = -1;
			int num2 = 0;
			try
			{
				using (IntegerFileWriter integerFileWriter = new IntegerFileWriter(dataFileInfo.FileName))
				{
					using (IntegerFileWriter integerFileWriter2 = new IntegerFileWriter(dataFileInfo2.FileName))
					{
						using (IntegerFileReader integerFileReader = new IntegerFileReader(inputFileName))
						{
							integerFileReader.Open();
							integerFileWriter.Create();
							integerFileWriter2.Create();
							int items = integerFileReader.Items;
							int pos = 0;
							InvertedCell c = default(InvertedCell);
							c.Key = (c.Segment = (c.Position = 0));
							while (pos < items)
							{
								ReadCell(ref c, integerFileReader, ref pos);
								if (c.Key != num)
								{
									integerFileWriter2.Write(num2);
									num = c.Key;
								}
								integerFileWriter.Write(c.Segment);
								integerFileWriter.Write(c.Position);
								num2 += 2;
							}
							integerFileReader.Close();
							integerFileWriter.Close();
							integerFileWriter2.Close();
						}
					}
				}
			}
			finally
			{
				File.Delete(inputFileName);
			}
		}
	}
}
