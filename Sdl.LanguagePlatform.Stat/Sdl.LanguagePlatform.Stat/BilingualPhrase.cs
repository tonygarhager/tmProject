using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class BilingualPhrase : IComparable<BilingualPhrase>, ICountable
	{
		public int Id;

		public Phrase Source;

		public Phrase Target;

		public int Count
		{
			get;
			set;
		}

		public BilingualPhrase(Phrase s, Phrase t)
		{
			Source = s;
			Target = t;
			Id = 0;
			Count = 0;
		}

		public void Write(BinaryWriter wtr)
		{
			wtr.Write(Id);
			wtr.Write(Count);
			Source.Write(wtr);
			Target.Write(wtr);
		}

		public static BilingualPhrase Read(BinaryReader rdr)
		{
			int id = rdr.ReadInt32();
			int count = rdr.ReadInt32();
			Phrase s = Phrase.Read(rdr);
			Phrase t = Phrase.Read(rdr);
			return new BilingualPhrase(s, t)
			{
				Count = count,
				Id = id
			};
		}

		public int CompareTo(BilingualPhrase other)
		{
			int num = Source.CompareTo(other.Source);
			if (num == 0)
			{
				num = Target.CompareTo(other.Target);
			}
			return num;
		}
	}
}
