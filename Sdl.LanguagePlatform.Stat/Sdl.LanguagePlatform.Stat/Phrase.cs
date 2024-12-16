using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class Phrase : IComparable<Phrase>, ICountable
	{
		public int[] Keys
		{
			get;
		}

		public int this[int idx] => Keys[idx];

		public int ID
		{
			get;
			set;
		}

		public int Count
		{
			get;
			set;
		}

		public int Length => Keys.Length;

		public Phrase(IList<int> keys)
		{
			if (keys == null || keys.Count == 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			Keys = new int[keys.Count];
			for (int i = 0; i < keys.Count; i++)
			{
				Keys[i] = keys[i];
			}
		}

		public void Dump(VocabularyFile v, TextWriter wtr)
		{
			for (int i = 0; i < Length; i++)
			{
				if (i > 0)
				{
					wtr.Write(' ');
				}
				wtr.Write(v.Lookup(Keys[i]));
			}
			wtr.WriteLine(" (id={0}, f={1}, l={2})", ID, Count, Keys.Length);
		}

		public void Write(BinaryWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(ID);
			writer.Write(Count);
			writer.Write(Keys.Length);
			int[] keys = Keys;
			foreach (int value in keys)
			{
				writer.Write(value);
			}
		}

		public static Phrase Read(BinaryReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			int iD = reader.ReadInt32();
			int count = reader.ReadInt32();
			int num = reader.ReadInt32();
			List<int> list = new List<int>();
			for (int i = 0; i < num; i++)
			{
				list.Add(reader.ReadInt32());
			}
			return new Phrase(list)
			{
				ID = iD,
				Count = count
			};
		}

		public int CompareTo(Phrase other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			int num = Math.Min(Length, other.Length);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (num2 != 0)
				{
					break;
				}
				num2 = Keys[i] - other.Keys[i];
			}
			if (num2 != 0)
			{
				return num2;
			}
			return Length - other.Length;
		}
	}
}
