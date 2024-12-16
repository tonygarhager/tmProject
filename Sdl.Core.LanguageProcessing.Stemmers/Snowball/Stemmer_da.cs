using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;
using System.Text;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_da : Stemmer
	{
		private int I_x;

		private int I_p1;

		private StringBuilder S_ch = new StringBuilder();

		private static string g_c = "bcdfghjklmnpqrstvwxz";

		private static string g_v = "aeiouyæåø";

		private static string g_s_ending = "abcdfghjklmnoprtvyzå";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_da()
		{
			a_0 = new Among[32]
			{
				new Among("hed", -1, 1),
				new Among("ethed", 0, 1),
				new Among("ered", -1, 1),
				new Among("e", -1, 1),
				new Among("erede", 3, 1),
				new Among("ende", 3, 1),
				new Among("erende", 5, 1),
				new Among("ene", 3, 1),
				new Among("erne", 3, 1),
				new Among("ere", 3, 1),
				new Among("en", -1, 1),
				new Among("heden", 10, 1),
				new Among("eren", 10, 1),
				new Among("er", -1, 1),
				new Among("heder", 13, 1),
				new Among("erer", 13, 1),
				new Among("s", -1, 2),
				new Among("heds", 16, 1),
				new Among("es", 16, 1),
				new Among("endes", 18, 1),
				new Among("erendes", 19, 1),
				new Among("enes", 18, 1),
				new Among("ernes", 18, 1),
				new Among("eres", 18, 1),
				new Among("ens", 16, 1),
				new Among("hedens", 24, 1),
				new Among("erens", 24, 1),
				new Among("ers", 16, 1),
				new Among("ets", 16, 1),
				new Among("erets", 28, 1),
				new Among("et", -1, 1),
				new Among("eret", 30, 1)
			};
			a_1 = new Among[4]
			{
				new Among("gd", -1, -1),
				new Among("dt", -1, -1),
				new Among("gt", -1, -1),
				new Among("kt", -1, -1)
			};
			a_2 = new Among[5]
			{
				new Among("ig", -1, 1),
				new Among("lig", 0, 1),
				new Among("elig", 1, 1),
				new Among("els", -1, 1),
				new Among("løst", -1, 2)
			};
		}

		private bool r_mark_regions()
		{
			I_p1 = limit;
			int cursor = base.cursor;
			int num = base.cursor + 3;
			if (0 > num || num > limit)
			{
				return false;
			}
			base.cursor = num;
			I_x = base.cursor;
			base.cursor = cursor;
			if (out_grouping(g_v, 97, 248, repeat: true) < 0)
			{
				return false;
			}
			int num2 = in_grouping(g_v, 97, 248, repeat: true);
			if (num2 < 0)
			{
				return false;
			}
			base.cursor += num2;
			I_p1 = base.cursor;
			if (I_p1 < I_x)
			{
				I_p1 = I_x;
			}
			return true;
		}

		private bool r_main_suffix()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			int num = find_among_b(a_0);
			if (num == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			base.limit_backward = limit_backward;
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				if (in_grouping_b(g_s_ending, 97, 229, repeat: false) != 0)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_consonant_pair()
		{
			int num = limit - cursor;
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			if (find_among_b(a_1) == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			base.limit_backward = limit_backward;
			cursor = limit - num;
			if (cursor <= base.limit_backward)
			{
				return false;
			}
			cursor--;
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_other_suffix()
		{
			int num = limit - cursor;
			ket = cursor;
			if (eq_s_b("st"))
			{
				bra = cursor;
				if (eq_s_b("ig"))
				{
					slice_del();
				}
			}
			cursor = limit - num;
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			int num2 = find_among_b(a_2);
			if (num2 == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			base.limit_backward = limit_backward;
			switch (num2)
			{
			case 1:
			{
				slice_del();
				int num3 = limit - cursor;
				r_consonant_pair();
				cursor = limit - num3;
				break;
			}
			case 2:
				slice_from("løs");
				break;
			}
			return true;
		}

		private bool r_undouble()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			if (in_grouping_b(g_c, 98, 122, repeat: false) != 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			slice_to(S_ch);
			base.limit_backward = limit_backward;
			if (!eq_s_b(S_ch))
			{
				return false;
			}
			slice_del();
			return true;
		}

		protected override bool stem()
		{
			int cursor = base.cursor;
			r_mark_regions();
			base.cursor = cursor;
			limit_backward = base.cursor;
			base.cursor = limit;
			int num = limit - base.cursor;
			r_main_suffix();
			base.cursor = limit - num;
			int num2 = limit - base.cursor;
			r_consonant_pair();
			base.cursor = limit - num2;
			int num3 = limit - base.cursor;
			r_other_suffix();
			base.cursor = limit - num3;
			int num4 = limit - base.cursor;
			r_undouble();
			base.cursor = limit - num4;
			base.cursor = limit_backward;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("da", () => new Stemmer_da(), 86921364);
				}
				_registered = true;
			}
		}
	}
}
