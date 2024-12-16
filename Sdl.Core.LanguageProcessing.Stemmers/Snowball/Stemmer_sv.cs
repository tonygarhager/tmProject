using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_sv : Stemmer
	{
		private int I_x;

		private int I_p1;

		private static string g_v = "aeiouyäåö";

		private static string g_s_ending = "bcdfghjklmnoprtvy";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_sv()
		{
			a_0 = new Among[37]
			{
				new Among("a", -1, 1),
				new Among("arna", 0, 1),
				new Among("erna", 0, 1),
				new Among("heterna", 2, 1),
				new Among("orna", 0, 1),
				new Among("ad", -1, 1),
				new Among("e", -1, 1),
				new Among("ade", 6, 1),
				new Among("ande", 6, 1),
				new Among("arne", 6, 1),
				new Among("are", 6, 1),
				new Among("aste", 6, 1),
				new Among("en", -1, 1),
				new Among("anden", 12, 1),
				new Among("aren", 12, 1),
				new Among("heten", 12, 1),
				new Among("ern", -1, 1),
				new Among("ar", -1, 1),
				new Among("er", -1, 1),
				new Among("heter", 18, 1),
				new Among("or", -1, 1),
				new Among("s", -1, 2),
				new Among("as", 21, 1),
				new Among("arnas", 22, 1),
				new Among("ernas", 22, 1),
				new Among("ornas", 22, 1),
				new Among("es", 21, 1),
				new Among("ades", 26, 1),
				new Among("andes", 26, 1),
				new Among("ens", 21, 1),
				new Among("arens", 29, 1),
				new Among("hetens", 29, 1),
				new Among("erns", 21, 1),
				new Among("at", -1, 1),
				new Among("andet", -1, 1),
				new Among("het", -1, 1),
				new Among("ast", -1, 1)
			};
			a_1 = new Among[7]
			{
				new Among("dd", -1, -1),
				new Among("gd", -1, -1),
				new Among("nn", -1, -1),
				new Among("dt", -1, -1),
				new Among("gt", -1, -1),
				new Among("kt", -1, -1),
				new Among("tt", -1, -1)
			};
			a_2 = new Among[5]
			{
				new Among("ig", -1, 1),
				new Among("lig", 0, 1),
				new Among("els", -1, 1),
				new Among("fullt", -1, 3),
				new Among("löst", -1, 2)
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
			if (out_grouping(g_v, 97, 246, repeat: true) < 0)
			{
				return false;
			}
			int num2 = in_grouping(g_v, 97, 246, repeat: true);
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
				if (in_grouping_b(g_s_ending, 98, 121, repeat: false) != 0)
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
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			int num = limit - cursor;
			if (find_among_b(a_1) == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			cursor = limit - num;
			ket = cursor;
			if (cursor <= base.limit_backward)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			cursor--;
			bra = cursor;
			slice_del();
			base.limit_backward = limit_backward;
			return true;
		}

		private bool r_other_suffix()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			int num = find_among_b(a_2);
			if (num == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("lös");
				break;
			case 3:
				slice_from("full");
				break;
			}
			base.limit_backward = limit_backward;
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
			base.cursor = limit_backward;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("sv", () => new Stemmer_sv(), 1151181130);
				}
				_registered = true;
			}
		}
	}
}
