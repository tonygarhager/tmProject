using System;
using System.IO;
using System.Xml.Serialization;

namespace Sdl.Core.FineGrainedAlignment.Core
{
	public class LiftSpan
	{
		[XmlAttribute]
		public short StartIndex;

		[XmlAttribute]
		public short Length;

		public short EndIndex => (short)(StartIndex + Length - 1);

		public LiftSpan()
		{
		}

		public LiftSpan(LiftSpan otherSpan)
			: this(otherSpan.StartIndex, otherSpan.Length)
		{
		}

		public LiftSpan(short startIndex, short length)
		{
			StartIndex = startIndex;
			Length = length;
		}

		public static LiftSpan Load(BinaryReader reader, bool compactSzn)
		{
			LiftSpan liftSpan = new LiftSpan();
			if (compactSzn)
			{
				liftSpan.StartIndex = reader.ReadByte();
				liftSpan.Length = reader.ReadByte();
			}
			else
			{
				liftSpan.StartIndex = reader.ReadInt16();
				liftSpan.Length = reader.ReadInt16();
			}
			return liftSpan;
		}

		public void Save(BinaryWriter writer, bool compactSzn)
		{
			long num = 32767L;
			if (compactSzn)
			{
				num = 255L;
			}
			if (StartIndex > num)
			{
				throw new Exception("LiftSpan - invalid StartIndex: " + StartIndex.ToString());
			}
			if (Length > num)
			{
				throw new Exception("LiftSpan - invalid Length: " + Length.ToString());
			}
			if (compactSzn)
			{
				writer.Write((byte)StartIndex);
				writer.Write((byte)Length);
			}
			else
			{
				writer.Write(StartIndex);
				writer.Write(Length);
			}
		}

		public bool SpanEquals(LiftSpan otherSpan)
		{
			if (otherSpan.StartIndex == StartIndex)
			{
				return otherSpan.Length == Length;
			}
			return false;
		}

		public void Validate()
		{
			if (StartIndex < 0)
			{
				throw new Exception("Invalid LiftSpan: StartIndex < 0");
			}
			if (Length < 1)
			{
				throw new Exception("Invalid LiftSpan: Length < 1");
			}
			if (StartIndex + Length > 32767)
			{
				throw new Exception("Invalid LiftSpan: StartIndex + Length = " + (StartIndex + Length).ToString());
			}
		}

		public bool Covers(LiftSpan otherSpan, int maxExcess)
		{
			return Covers(otherSpan.StartIndex, otherSpan.Length, maxExcess);
		}

		public bool Covers(int otherStartIndex, int otherLength = 1, int maxExcess = 0)
		{
			if (otherLength < 1)
			{
				throw new ArgumentException("otherLength");
			}
			if (otherStartIndex >= StartIndex - maxExcess && otherStartIndex + otherLength <= StartIndex + Length + maxExcess)
			{
				return true;
			}
			return false;
		}

		public bool Overlaps(LiftSpan otherSpan)
		{
			if (otherSpan.StartIndex >= StartIndex)
			{
				return otherSpan.StartIndex <= StartIndex + Length - 1;
			}
			return StartIndex <= otherSpan.StartIndex + otherSpan.Length - 1;
		}

		public int Overlap(LiftSpan otherSpan)
		{
			int num = Math.Max(StartIndex, otherSpan.StartIndex);
			int num2 = Math.Min(EndIndex, otherSpan.EndIndex);
			if (num > num2)
			{
				return 0;
			}
			return num2 - num + 1;
		}

		public override string ToString()
		{
			return "(" + StartIndex.ToString() + "-" + EndIndex.ToString() + ")";
		}

		public LiftSpan Intersection(LiftSpan otherSpan)
		{
			if (!Overlaps(otherSpan))
			{
				return null;
			}
			LiftSpan liftSpan = new LiftSpan();
			liftSpan.StartIndex = Math.Max(StartIndex, otherSpan.StartIndex);
			liftSpan.Length = (short)(Math.Min(EndIndex, otherSpan.EndIndex) - liftSpan.StartIndex + 1);
			return liftSpan;
		}

		public LiftSpan Union(LiftSpan otherSpan)
		{
			if (!Overlaps(otherSpan))
			{
				return null;
			}
			LiftSpan liftSpan = new LiftSpan();
			liftSpan.StartIndex = Math.Min(StartIndex, otherSpan.StartIndex);
			liftSpan.Length = (short)(Math.Max(EndIndex, otherSpan.EndIndex) - liftSpan.StartIndex + 1);
			return liftSpan;
		}
	}
}
