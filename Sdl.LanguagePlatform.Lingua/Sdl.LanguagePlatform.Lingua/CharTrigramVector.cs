using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Lingua
{
	public class CharTrigramVector
	{
		private const int Magic = 200703270;

		private List<long> _keys;

		private List<double> _values;

		public bool IsNormalized
		{
			get;
			private set;
		}

		public CharTrigramVector()
		{
			_keys = new List<long>();
			_values = new List<double>();
		}

		public CharTrigramVector(CharTrigramVector other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			_keys = new List<long>(other._keys);
			_values = new List<double>(other._values);
			IsNormalized = other.IsNormalized;
		}

		public void Inc(char c1, char c2, char c3)
		{
			long item = (short)c1 * 4096 * 4096 + (short)c2 * 4096 * (short)c3;
			int num = _keys.BinarySearch(item);
			if (num >= 0)
			{
				_values[num] += 1.0;
			}
			else
			{
				_keys.Insert(~num, item);
				_values.Insert(~num, 1.0);
			}
			IsNormalized = false;
		}

		public void Cutoff(double threshold)
		{
			bool flag = false;
			for (int num = _values.Count - 1; num >= 0; num--)
			{
				if (_values[num] < threshold)
				{
					_keys.RemoveAt(num);
					_values.RemoveAt(num);
					flag = true;
				}
			}
			if (flag && IsNormalized)
			{
				IsNormalized = false;
				Normalize();
			}
		}

		public void Topmost(int n)
		{
			if (_keys.Count >= n)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < _keys.Count; i++)
				{
					list.Add(i);
				}
				list.Sort((int k1, int k2) => Math.Sign(_values[k2] - _values[k1]));
				list.RemoveRange(n, _keys.Count - n);
				double num = _values[list[n - 1]];
				list.Sort();
				List<long> list2 = new List<long>();
				List<double> list3 = new List<double>();
				foreach (int item in list)
				{
					list2.Add(_keys[item]);
					list3.Add(_values[item]);
				}
				_keys = list2;
				_values = list3;
				if (IsNormalized)
				{
					IsNormalized = false;
					Normalize();
				}
			}
		}

		public void Normalize()
		{
			if (IsNormalized || _keys.Count == 0)
			{
				return;
			}
			double num = Length();
			if (num == 0.0)
			{
				IsNormalized = true;
				return;
			}
			for (int i = 0; i < _values.Count; i++)
			{
				_values[i] /= num;
			}
			IsNormalized = true;
		}

		public double DotProduct(CharTrigramVector other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			double num = 0.0;
			List<long> keys = _keys;
			List<long> keys2 = other._keys;
			int num2 = 0;
			int num3 = 0;
			while (num2 < keys.Count && num3 < keys2.Count)
			{
				if (keys[num2] < keys2[num3])
				{
					num2++;
					continue;
				}
				if (keys[num2] > keys2[num3])
				{
					num3++;
					continue;
				}
				num += _values[num2] * other._values[num3];
				num2++;
				num3++;
			}
			return num;
		}

		public double Length()
		{
			if (IsNormalized)
			{
				return 1.0;
			}
			double num = 0.0;
			foreach (double value in _values)
			{
				num += value * value;
			}
			return Math.Sqrt(num);
		}

		public double Angle(CharTrigramVector other)
		{
			double num = Length();
			double num2 = other.Length();
			if (num == 0.0 || num2 == 0.0)
			{
				return 0.0;
			}
			return DotProduct(other) / (num * num2);
		}

		public void Save(string fileName)
		{
			using (Stream stream = File.Create(fileName))
			{
				Save(stream);
				stream.Close();
			}
		}

		public void Save(Stream str)
		{
			BinaryWriter binaryWriter = new BinaryWriter(str);
			binaryWriter.Write(200703270);
			binaryWriter.Write(_keys.Count);
			binaryWriter.Write(IsNormalized);
			long num = 0L;
			for (int i = 0; i < _keys.Count; i++)
			{
				_ = 0;
				num = _keys[i];
				binaryWriter.Write(_keys[i]);
				binaryWriter.Write(_values[i]);
			}
			binaryWriter.Flush();
		}

		public static CharTrigramVector Load(string fileName)
		{
			using (Stream stream = File.OpenRead(fileName))
			{
				return Load(stream);
			}
		}

		public static CharTrigramVector Load(Stream stream)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (binaryReader.ReadInt32() != 200703270)
			{
				throw new Exception("Invalid input data");
			}
			int num = binaryReader.ReadInt32();
			if (num < 0)
			{
				throw new Exception("Invalid input data");
			}
			CharTrigramVector charTrigramVector = new CharTrigramVector
			{
				IsNormalized = binaryReader.ReadBoolean()
			};
			long num2 = 0L;
			for (int i = 0; i < num; i++)
			{
				long num3 = binaryReader.ReadInt64();
				double item = binaryReader.ReadDouble();
				_ = 0;
				num2 = num3;
				charTrigramVector._keys.Add(num3);
				charTrigramVector._values.Add(item);
			}
			return charTrigramVector;
		}
	}
}
