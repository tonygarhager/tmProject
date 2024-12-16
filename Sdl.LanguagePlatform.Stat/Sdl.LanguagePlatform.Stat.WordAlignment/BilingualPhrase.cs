using System;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class BilingualPhrase
	{
		public int FromSrcIndex;

		public int IntoSrcIndex;

		public int FromTrgIndex;

		public int IntoTrgIndex;

		public double Association;

		public int SourceLength => IntoSrcIndex - FromSrcIndex + 1;

		public int TargetLength => IntoTrgIndex - FromTrgIndex + 1;

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
			BilingualPhrase other = (BilingualPhrase)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 31 + FromSrcIndex;
			num = num * 31 + IntoSrcIndex;
			num = num * 31 + FromTrgIndex;
			return num * 31 + IntoTrgIndex;
		}

		public bool Equals(BilingualPhrase other)
		{
			if (other != null && FromSrcIndex == other.FromSrcIndex && IntoSrcIndex == other.IntoSrcIndex && FromTrgIndex == other.FromTrgIndex)
			{
				return IntoTrgIndex == other.IntoTrgIndex;
			}
			return false;
		}

		public BilingualPhrase(BilingualPhrase other)
		{
			FromSrcIndex = other.FromSrcIndex;
			IntoSrcIndex = other.IntoSrcIndex;
			FromTrgIndex = other.FromTrgIndex;
			IntoTrgIndex = other.IntoTrgIndex;
			Association = other.Association;
		}

		public BilingualPhrase(int fromSrc, int intoSrc, int fromTrg, int intoTrg, double association)
		{
			FromSrcIndex = fromSrc;
			IntoSrcIndex = intoSrc;
			FromTrgIndex = fromTrg;
			IntoTrgIndex = intoTrg;
			Association = association;
		}

		public BilingualPhrase(int srcPosition, int trgPosition, double association)
		{
			FromSrcIndex = (IntoSrcIndex = srcPosition);
			FromTrgIndex = (IntoTrgIndex = trgPosition);
			Association = association;
		}

		public BilingualPhrase(BilingualPhrase first, BilingualPhrase second, double jointAssociation)
		{
			FromSrcIndex = Math.Min(first.FromSrcIndex, second.FromSrcIndex);
			IntoSrcIndex = Math.Max(first.IntoSrcIndex, second.IntoSrcIndex);
			FromTrgIndex = Math.Min(first.FromTrgIndex, second.FromTrgIndex);
			IntoTrgIndex = Math.Max(first.IntoTrgIndex, second.IntoTrgIndex);
			Association = jointAssociation;
		}

		public bool IsAdjacent(BilingualPhrase other)
		{
			if (IntoSrcIndex + 1 == other.FromSrcIndex || FromSrcIndex == other.IntoSrcIndex + 1)
			{
				if (IntoTrgIndex + 1 != other.FromTrgIndex)
				{
					return FromTrgIndex == other.IntoTrgIndex + 1;
				}
				return true;
			}
			return false;
		}

		public bool HasAdjacentEdge(BilingualPhrase other)
		{
			if (IntoSrcIndex + 1 != other.FromSrcIndex && FromSrcIndex != other.IntoSrcIndex + 1 && IntoTrgIndex + 1 != other.FromTrgIndex)
			{
				return FromTrgIndex == other.IntoTrgIndex + 1;
			}
			return true;
		}

		public void Extend(int i, int j)
		{
			if (i == FromSrcIndex - 1)
			{
				FromSrcIndex = i;
			}
			else if (i == IntoSrcIndex + 1)
			{
				IntoSrcIndex = i;
			}
			if (j == FromTrgIndex - 1)
			{
				FromTrgIndex = j;
			}
			else if (j == IntoTrgIndex + 1)
			{
				IntoTrgIndex = j;
			}
		}

		public void Extend(BilingualPhrase other)
		{
			FromSrcIndex = Math.Min(FromSrcIndex, other.FromSrcIndex);
			IntoSrcIndex = Math.Max(IntoSrcIndex, other.IntoSrcIndex);
			FromTrgIndex = Math.Min(FromTrgIndex, other.FromTrgIndex);
			IntoTrgIndex = Math.Max(IntoTrgIndex, other.IntoTrgIndex);
		}

		public bool IsTouching(int i, int j)
		{
			bool flag = i == FromSrcIndex - 1 || i == IntoSrcIndex + 1;
			bool flag2 = j == FromTrgIndex - 1 || j == IntoTrgIndex + 1;
			return flag | flag2;
		}

		public bool Contains(int i, int j)
		{
			if (i >= FromSrcIndex && i <= IntoSrcIndex && j >= FromTrgIndex)
			{
				return j <= IntoTrgIndex;
			}
			return false;
		}
	}
}
