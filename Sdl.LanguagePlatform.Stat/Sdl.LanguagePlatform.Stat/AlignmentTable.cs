using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class AlignmentTable
	{
		private readonly BitArray[] _data;

		private readonly BitArray _srcAligned;

		private readonly BitArray _trgAligned;

		public int SourceSegmentLength
		{
			get;
		}

		public int TargetSegmentLength
		{
			get;
		}

		public double[,] Associations
		{
			get;
		}

		public bool this[int srcIdx, int trgIdx]
		{
			get
			{
				return _data[srcIdx][trgIdx];
			}
			set
			{
				_data[srcIdx][trgIdx] = value;
				if (value)
				{
					_srcAligned[srcIdx] = true;
					_trgAligned[trgIdx] = true;
					return;
				}
				_srcAligned[srcIdx] = false;
				_trgAligned[trgIdx] = false;
				for (int i = 0; i < SourceSegmentLength; i++)
				{
					if (_data[i][trgIdx])
					{
						_trgAligned[trgIdx] = true;
						break;
					}
				}
				int num = 0;
				while (true)
				{
					if (num < TargetSegmentLength)
					{
						if (_data[srcIdx][num])
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				_srcAligned[srcIdx] = true;
			}
		}

		public AlignmentTable(int srcSegmentLength, int trgSegmentLength)
			: this(srcSegmentLength, trgSegmentLength, null)
		{
		}

		public AlignmentTable(int srcSegmentLength, int trgSegmentLength, double[,] associations)
		{
			if (srcSegmentLength <= 0 || trgSegmentLength <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			Associations = associations;
			SourceSegmentLength = srcSegmentLength;
			TargetSegmentLength = trgSegmentLength;
			_data = new BitArray[srcSegmentLength];
			for (int i = 0; i < srcSegmentLength; i++)
			{
				_data[i] = new BitArray(trgSegmentLength);
			}
			_srcAligned = new BitArray(srcSegmentLength);
			_trgAligned = new BitArray(trgSegmentLength);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			AlignmentTable alignmentTable = obj as AlignmentTable;
			if (alignmentTable != null && (SourceSegmentLength != alignmentTable.SourceSegmentLength || TargetSegmentLength != alignmentTable.TargetSegmentLength))
			{
				return false;
			}
			for (int i = 0; i < SourceSegmentLength; i++)
			{
				for (int j = 0; j < TargetSegmentLength; j++)
				{
					if (alignmentTable != null && _data[i][j] != alignmentTable._data[i][j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < SourceSegmentLength; i++)
			{
				for (int j = 0; j < TargetSegmentLength; j++)
				{
					num += _data[i][j].GetHashCode();
				}
			}
			return num;
		}

		public bool IsTargetAligned(int trgIdx)
		{
			return _trgAligned[trgIdx];
		}

		public void GetSourceToTargetAlignedRange(int srcStart, int srcInto, out int trgStart, out int trgInto, out int srcHoles)
		{
			if (srcStart < 0)
			{
				throw new ArgumentOutOfRangeException("srcStart");
			}
			if (srcInto < 0)
			{
				throw new ArgumentOutOfRangeException("srcInto");
			}
			if (srcInto < srcStart)
			{
				throw new ArgumentException("srcInto < srcStart not valid");
			}
			trgStart = -1;
			trgInto = -1;
			srcHoles = 0;
			for (int i = srcStart; i <= srcInto; i++)
			{
				bool flag = false;
				for (int j = 0; j < TargetSegmentLength; j++)
				{
					if (_data[i][j])
					{
						if (trgStart < 0 || j < trgStart)
						{
							trgStart = j;
						}
						if (trgInto < 0 || j > trgInto)
						{
							trgInto = j;
						}
						flag = true;
					}
				}
				if (!flag)
				{
					srcHoles++;
				}
			}
		}

		public void GetTargetToSourceAlignedRange(int trgStart, int trgInto, out int srcStart, out int srcInto, out int trgHoles)
		{
			if (trgStart < 0)
			{
				throw new ArgumentOutOfRangeException("trgStart");
			}
			if (trgInto < 0)
			{
				throw new ArgumentOutOfRangeException("trgInto");
			}
			if (trgInto < trgStart)
			{
				throw new ArgumentException("trgInto < trgStart not valid");
			}
			srcStart = -1;
			srcInto = -1;
			trgHoles = 0;
			for (int i = trgStart; i <= trgInto; i++)
			{
				bool flag = false;
				for (int j = 0; j < SourceSegmentLength; j++)
				{
					if (_data[j][i])
					{
						if (srcStart < 0 || j < srcStart)
						{
							srcStart = j;
						}
						if (srcInto < 0 || j > srcInto)
						{
							srcInto = j;
						}
						flag = true;
					}
				}
				if (!flag)
				{
					trgHoles++;
				}
			}
		}

		public void Write(BinaryWriter stream)
		{
			stream.Write(BitConverter.GetBytes(20061220));
			stream.Write(BitConverter.GetBytes(SourceSegmentLength));
			stream.Write(BitConverter.GetBytes(TargetSegmentLength));
			int num = 0;
			for (int i = 0; i < SourceSegmentLength; i++)
			{
				for (int j = 0; j < TargetSegmentLength; j++)
				{
					if (_data[i][j])
					{
						num++;
					}
				}
			}
			stream.Write(BitConverter.GetBytes(num));
			for (int k = 0; k < SourceSegmentLength; k++)
			{
				for (int l = 0; l < TargetSegmentLength; l++)
				{
					if (_data[k][l])
					{
						stream.Write(BitConverter.GetBytes(k));
						stream.Write(BitConverter.GetBytes(l));
					}
				}
			}
		}

		public static AlignmentTable Read(BinaryReader stream)
		{
			int num = stream.ReadInt32();
			if (num != 20061220)
			{
				throw new LanguagePlatformException(ErrorCode.CorruptData);
			}
			int num2 = stream.ReadInt32();
			int num3 = stream.ReadInt32();
			if (num2 <= 0 || num3 <= 0)
			{
				throw new LanguagePlatformException(ErrorCode.CorruptData);
			}
			AlignmentTable alignmentTable = new AlignmentTable(num2, num3);
			int num4 = stream.ReadInt32();
			for (int i = 0; i < num4; i++)
			{
				int num5 = stream.ReadInt32();
				int num6 = stream.ReadInt32();
				if (num5 < 0 || num5 >= num2 || num6 < 0 || num6 >= num3)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				alignmentTable[num5, num6] = true;
			}
			return alignmentTable;
		}

		public void Dump(IntSegment srcSegment, IntSegment trgSegment, VocabularyFile srcVocabulary, VocabularyFile trgVocabulary, TextWriter logStream)
		{
			if (SourceSegmentLength != srcSegment.Count || TargetSegmentLength != trgSegment.Count)
			{
				throw new ArgumentException("AlignmentTable.Dump(): invalid parameters");
			}
			logStream.Write("Src:\t");
			srcSegment.Dump(srcVocabulary, logStream);
			logStream.WriteLine();
			logStream.Write("Trg:\t");
			trgSegment.Dump(trgVocabulary, logStream);
			logStream.WriteLine();
			logStream.WriteLine();
			int num = 0;
			for (int i = 0; i < srcSegment.Count; i++)
			{
				string text = srcVocabulary.Lookup(srcSegment[i]);
				if (text != null)
				{
					num = Math.Max(num, text.Length);
				}
			}
			for (int j = 0; j < num; j++)
			{
				logStream.Write(" ");
			}
			for (int k = 0; k < trgSegment.Count; k++)
			{
				logStream.Write(" | ");
				string text2 = trgVocabulary.Lookup(trgSegment[k]);
				if (text2.Length > 5)
				{
					text2 = text2.Substring(0, 3) + "..";
				}
				logStream.Write(text2);
				int length = text2.Length;
				for (int l = 0; l < 5 - length; l++)
				{
					logStream.Write(" ");
				}
			}
			logStream.WriteLine(" |");
			for (int m = 0; m < num; m++)
			{
				logStream.Write("-");
			}
			for (int n = 0; n < trgSegment.Count; n++)
			{
				logStream.Write("-+-");
				for (int num2 = 0; num2 < 5; num2++)
				{
					logStream.Write("-");
				}
			}
			logStream.WriteLine("-+--");
			for (int num3 = 0; num3 < srcSegment.Count; num3++)
			{
				string text3 = srcVocabulary.Lookup(srcSegment[num3]);
				logStream.Write(text3);
				int num4 = text3?.Length ?? 0;
				for (int num5 = 0; num5 < num - num4; num5++)
				{
					logStream.Write(" ");
				}
				for (int num6 = 0; num6 < trgSegment.Count; num6++)
				{
					logStream.Write(" | ");
					bool flag = this[num3, num6];
					for (int num7 = 0; num7 < 2; num7++)
					{
						logStream.Write(' ');
					}
					logStream.Write(flag ? '*' : ' ');
					for (int num8 = 0; num8 < 2; num8++)
					{
						logStream.Write(' ');
					}
				}
				logStream.WriteLine(" | {0}", IsSourceAligned(num3) ? '*' : '-');
			}
			for (int num9 = 0; num9 < num; num9++)
			{
				logStream.Write("-");
			}
			for (int num10 = 0; num10 < trgSegment.Count; num10++)
			{
				logStream.Write("-+-");
				for (int num11 = 0; num11 < 5; num11++)
				{
					logStream.Write("-");
				}
			}
			logStream.WriteLine("-+--");
			for (int num12 = 0; num12 < num; num12++)
			{
				logStream.Write(" ");
			}
			for (int num13 = 0; num13 < trgSegment.Count; num13++)
			{
				logStream.Write(" | ");
				for (int num14 = 0; num14 < 2; num14++)
				{
					logStream.Write(' ');
				}
				logStream.Write(IsTargetAligned(num13) ? '*' : ' ');
				for (int num15 = 0; num15 < 2; num15++)
				{
					logStream.Write(' ');
				}
			}
			logStream.WriteLine(" |");
			logStream.WriteLine();
			logStream.WriteLine();
		}

		public bool IsSourceAligned(int srcIdx)
		{
			return _srcAligned[srcIdx];
		}
	}
}
