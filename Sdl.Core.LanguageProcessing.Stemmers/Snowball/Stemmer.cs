#define TRACE
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Snowball
{
	public abstract class Stemmer : Env
	{
		public StringBuilder Buffer => current;

		public string Current
		{
			get
			{
				return current.ToString();
			}
			set
			{
				setBufferContents(value);
			}
		}

		protected Stemmer()
		{
			current = new StringBuilder();
			setBufferContents("");
		}

		protected abstract bool stem();

		public bool Stem()
		{
			return stem();
		}

		public string Stem(string word)
		{
			setBufferContents(word);
			stem();
			return current.ToString();
		}

		private void setBufferContents(string value)
		{
			current.Clear();
			current.Insert(0, value);
			cursor = 0;
			limit = current.Length;
			limit_backward = 0;
			bra = cursor;
			ket = limit;
		}

		protected int in_grouping(string s, int min, int max, bool repeat)
		{
			do
			{
				if (cursor >= limit)
				{
					return -1;
				}
				char c = current[cursor];
				if (c > max || c < min)
				{
					return 1;
				}
				if (!s.Contains(c))
				{
					return 1;
				}
				cursor++;
			}
			while (repeat);
			return 0;
		}

		protected int in_grouping_b(string s, int min, int max, bool repeat)
		{
			do
			{
				if (cursor <= limit_backward)
				{
					return -1;
				}
				char c = current[cursor - 1];
				if (c > max || c < min)
				{
					return 1;
				}
				if (!s.Contains(c))
				{
					return 1;
				}
				cursor--;
			}
			while (repeat);
			return 0;
		}

		protected int out_grouping(string s, int min, int max, bool repeat)
		{
			do
			{
				if (cursor >= limit)
				{
					return -1;
				}
				char c = current[cursor];
				if (c > max || c < min)
				{
					cursor++;
					continue;
				}
				if (!s.Contains(c))
				{
					cursor++;
					continue;
				}
				return 1;
			}
			while (repeat);
			return 0;
		}

		protected int out_grouping_b(string s, int min, int max, bool repeat)
		{
			do
			{
				if (cursor <= limit_backward)
				{
					return -1;
				}
				char c = current[cursor - 1];
				if (c > max || c < min)
				{
					cursor--;
					continue;
				}
				if (s.Contains(c))
				{
					return 1;
				}
				cursor--;
			}
			while (repeat);
			return 0;
		}

		protected bool eq_s(string s)
		{
			if (limit - cursor < s.Length)
			{
				return false;
			}
			for (int i = 0; i != s.Length; i++)
			{
				if (current[cursor + i] != s[i])
				{
					return false;
				}
			}
			cursor += s.Length;
			return true;
		}

		protected bool eq_s_b(string s)
		{
			if (cursor - limit_backward < s.Length)
			{
				return false;
			}
			for (int i = 0; i != s.Length; i++)
			{
				if (current[cursor - s.Length + i] != s[i])
				{
					return false;
				}
			}
			cursor -= s.Length;
			return true;
		}

		protected bool eq_s_b(StringBuilder s)
		{
			if (cursor - limit_backward < s.Length)
			{
				return false;
			}
			for (int i = 0; i != s.Length; i++)
			{
				if (current[cursor - s.Length + i] != s[i])
				{
					return false;
				}
			}
			cursor -= s.Length;
			return true;
		}

		protected int find_among(Among[] v)
		{
			int num = 0;
			int num2 = v.Length;
			int cursor = base.cursor;
			int limit = base.limit;
			int num3 = 0;
			int num4 = 0;
			bool flag = false;
			while (true)
			{
				int num5 = num + (num2 - num >> 1);
				int num6 = 0;
				int num7 = (num3 < num4) ? num3 : num4;
				Among among = v[num5];
				for (int i = num7; i < among.SearchString.Length; i++)
				{
					if (cursor + num7 == limit)
					{
						num6 = -1;
						break;
					}
					num6 = current[cursor + num7] - among.SearchString[i];
					if (num6 != 0)
					{
						break;
					}
					num7++;
				}
				if (num6 < 0)
				{
					num2 = num5;
					num4 = num7;
				}
				else
				{
					num = num5;
					num3 = num7;
				}
				if (num2 - num <= 1)
				{
					if (num > 0 || num2 == num || flag)
					{
						break;
					}
					flag = true;
				}
			}
			do
			{
				Among among2 = v[num];
				if (num3 >= among2.SearchString.Length)
				{
					base.cursor = cursor + among2.SearchString.Length;
					if (among2.Action == null)
					{
						return among2.Result;
					}
					bool flag2 = among2.Action();
					base.cursor = cursor + among2.SearchString.Length;
					if (flag2)
					{
						return among2.Result;
					}
				}
				num = among2.MatchIndex;
			}
			while (num >= 0);
			return 0;
		}

		protected int find_among_b(Among[] v)
		{
			int num = 0;
			int num2 = v.Length;
			int cursor = base.cursor;
			int limit_backward = base.limit_backward;
			int num3 = 0;
			int num4 = 0;
			bool flag = false;
			while (true)
			{
				int num5 = num + (num2 - num >> 1);
				int num6 = 0;
				int num7 = (num3 < num4) ? num3 : num4;
				Among among = v[num5];
				for (int num8 = among.SearchString.Length - 1 - num7; num8 >= 0; num8--)
				{
					if (cursor - num7 == limit_backward)
					{
						num6 = -1;
						break;
					}
					num6 = current[cursor - 1 - num7] - among.SearchString[num8];
					if (num6 != 0)
					{
						break;
					}
					num7++;
				}
				if (num6 < 0)
				{
					num2 = num5;
					num4 = num7;
				}
				else
				{
					num = num5;
					num3 = num7;
				}
				if (num2 - num <= 1)
				{
					if (num > 0 || num2 == num || flag)
					{
						break;
					}
					flag = true;
				}
			}
			do
			{
				Among among2 = v[num];
				if (num3 >= among2.SearchString.Length)
				{
					base.cursor = cursor - among2.SearchString.Length;
					if (among2.Action == null)
					{
						return among2.Result;
					}
					bool flag2 = among2.Action();
					base.cursor = cursor - among2.SearchString.Length;
					if (flag2)
					{
						return among2.Result;
					}
				}
				num = among2.MatchIndex;
			}
			while (num >= 0);
			return 0;
		}

		protected int replace_s(int c_bra, int c_ket, string s)
		{
			int num = s.Length - (c_ket - c_bra);
			Replace(current, c_bra, c_ket, s);
			limit += num;
			if (cursor >= c_ket)
			{
				cursor += num;
			}
			else if (cursor > c_bra)
			{
				cursor = c_bra;
			}
			return num;
		}

		protected void slice_check()
		{
			if (bra < 0 || bra > ket || ket > limit || limit > current.Length)
			{
				Trace.WriteLine("faulty slice operation");
			}
		}

		protected void slice_from(string s)
		{
			slice_check();
			replace_s(bra, ket, s);
		}

		protected void slice_del()
		{
			slice_from("");
		}

		protected void insert(int c_bra, int c_ket, string s)
		{
			int num = replace_s(c_bra, c_ket, s);
			if (c_bra <= bra)
			{
				bra += num;
			}
			if (c_bra <= ket)
			{
				ket += num;
			}
		}

		protected void insert(int c_bra, int c_ket, StringBuilder s)
		{
			int num = replace_s(c_bra, c_ket, s.ToString());
			if (c_bra <= bra)
			{
				bra += num;
			}
			if (c_bra <= ket)
			{
				ket += num;
			}
		}

		protected void slice_to(StringBuilder s)
		{
			slice_check();
			Replace(s, 0, s.Length, current.ToString(bra, ket - bra));
		}

		protected void assign_to(StringBuilder s)
		{
			Replace(s, 0, s.Length, current.ToString(0, limit));
		}

		public static StringBuilder Replace(StringBuilder sb, int index, int length, string text)
		{
			sb.Remove(index, length - index);
			sb.Insert(index, text);
			return sb;
		}
	}
}
