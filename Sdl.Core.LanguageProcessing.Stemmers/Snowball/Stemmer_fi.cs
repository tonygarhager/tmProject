using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;
using System.Text;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_fi : Stemmer
	{
		private bool B_ending_removed;

		private StringBuilder S_x = new StringBuilder();

		private int I_p2;

		private int I_p1;

		private static string g_AEI = "aäei";

		private static string g_C = "bcdfghjklmnpqrstvwxz";

		private static string g_V1 = "aeiouyäö";

		private static string g_V2 = "aeiouäö";

		private static string g_particle_end = "aeiouyäönt";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private readonly Among[] a_5;

		private readonly Among[] a_6;

		private readonly Among[] a_7;

		private readonly Among[] a_8;

		private readonly Among[] a_9;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_fi()
		{
			a_0 = new Among[10]
			{
				new Among("pa", -1, 1),
				new Among("sti", -1, 2),
				new Among("kaan", -1, 1),
				new Among("han", -1, 1),
				new Among("kin", -1, 1),
				new Among("hän", -1, 1),
				new Among("kään", -1, 1),
				new Among("ko", -1, 1),
				new Among("pä", -1, 1),
				new Among("kö", -1, 1)
			};
			a_1 = new Among[6]
			{
				new Among("lla", -1, -1),
				new Among("na", -1, -1),
				new Among("ssa", -1, -1),
				new Among("ta", -1, -1),
				new Among("lta", 3, -1),
				new Among("sta", 3, -1)
			};
			a_2 = new Among[6]
			{
				new Among("llä", -1, -1),
				new Among("nä", -1, -1),
				new Among("ssä", -1, -1),
				new Among("tä", -1, -1),
				new Among("ltä", 3, -1),
				new Among("stä", 3, -1)
			};
			a_3 = new Among[2]
			{
				new Among("lle", -1, -1),
				new Among("ine", -1, -1)
			};
			a_4 = new Among[9]
			{
				new Among("nsa", -1, 3),
				new Among("mme", -1, 3),
				new Among("nne", -1, 3),
				new Among("ni", -1, 2),
				new Among("si", -1, 1),
				new Among("an", -1, 4),
				new Among("en", -1, 6),
				new Among("än", -1, 5),
				new Among("nsä", -1, 3)
			};
			a_5 = new Among[7]
			{
				new Among("aa", -1, -1),
				new Among("ee", -1, -1),
				new Among("ii", -1, -1),
				new Among("oo", -1, -1),
				new Among("uu", -1, -1),
				new Among("ää", -1, -1),
				new Among("öö", -1, -1)
			};
			a_6 = new Among[30]
			{
				new Among("a", -1, 8),
				new Among("lla", 0, -1),
				new Among("na", 0, -1),
				new Among("ssa", 0, -1),
				new Among("ta", 0, -1),
				new Among("lta", 4, -1),
				new Among("sta", 4, -1),
				new Among("tta", 4, 2),
				new Among("lle", -1, -1),
				new Among("ine", -1, -1),
				new Among("ksi", -1, -1),
				new Among("n", -1, 7),
				new Among("han", 11, 1),
				new Among("den", 11, -1, r_VI),
				new Among("seen", 11, -1, r_LONG),
				new Among("hen", 11, 2),
				new Among("tten", 11, -1, r_VI),
				new Among("hin", 11, 3),
				new Among("siin", 11, -1, r_VI),
				new Among("hon", 11, 4),
				new Among("hän", 11, 5),
				new Among("hön", 11, 6),
				new Among("ä", -1, 8),
				new Among("llä", 22, -1),
				new Among("nä", 22, -1),
				new Among("ssä", 22, -1),
				new Among("tä", 22, -1),
				new Among("ltä", 26, -1),
				new Among("stä", 26, -1),
				new Among("ttä", 26, 2)
			};
			a_7 = new Among[14]
			{
				new Among("eja", -1, -1),
				new Among("mma", -1, 1),
				new Among("imma", 1, -1),
				new Among("mpa", -1, 1),
				new Among("impa", 3, -1),
				new Among("mmi", -1, 1),
				new Among("immi", 5, -1),
				new Among("mpi", -1, 1),
				new Among("impi", 7, -1),
				new Among("ejä", -1, -1),
				new Among("mmä", -1, 1),
				new Among("immä", 10, -1),
				new Among("mpä", -1, 1),
				new Among("impä", 12, -1)
			};
			a_8 = new Among[2]
			{
				new Among("i", -1, -1),
				new Among("j", -1, -1)
			};
			a_9 = new Among[2]
			{
				new Among("mma", -1, 1),
				new Among("imma", 0, -1)
			};
		}

		private bool r_mark_regions()
		{
			I_p1 = limit;
			I_p2 = limit;
			if (out_grouping(g_V1, 97, 246, repeat: true) < 0)
			{
				return false;
			}
			int num = in_grouping(g_V1, 97, 246, repeat: true);
			if (num < 0)
			{
				return false;
			}
			cursor += num;
			I_p1 = cursor;
			if (out_grouping(g_V1, 97, 246, repeat: true) < 0)
			{
				return false;
			}
			int num2 = in_grouping(g_V1, 97, 246, repeat: true);
			if (num2 < 0)
			{
				return false;
			}
			cursor += num2;
			I_p2 = cursor;
			return true;
		}

		private bool r_R2()
		{
			return I_p2 <= cursor;
		}

		private bool r_particle_etc()
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
				if (in_grouping_b(g_particle_end, 97, 246, repeat: false) != 0)
				{
					return false;
				}
				break;
			case 2:
				if (!r_R2())
				{
					return false;
				}
				break;
			}
			slice_del();
			return true;
		}

		private bool r_possessive()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			int num = find_among_b(a_4);
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
			{
				int num2 = limit - cursor;
				if (eq_s_b("k"))
				{
					return false;
				}
				cursor = limit - num2;
				slice_del();
				break;
			}
			case 2:
				slice_del();
				ket = cursor;
				if (!eq_s_b("kse"))
				{
					return false;
				}
				bra = cursor;
				slice_from("ksi");
				break;
			case 3:
				slice_del();
				break;
			case 4:
				if (find_among_b(a_1) == 0)
				{
					return false;
				}
				slice_del();
				break;
			case 5:
				if (find_among_b(a_2) == 0)
				{
					return false;
				}
				slice_del();
				break;
			case 6:
				if (find_among_b(a_3) == 0)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_LONG()
		{
			return find_among_b(a_5) != 0;
		}

		private bool r_VI()
		{
			if (!eq_s_b("i"))
			{
				return false;
			}
			return in_grouping_b(g_V2, 97, 246, repeat: false) == 0;
		}

		private bool r_case_ending()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			int num = find_among_b(a_6);
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
				if (!eq_s_b("a"))
				{
					return false;
				}
				break;
			case 2:
				if (!eq_s_b("e"))
				{
					return false;
				}
				break;
			case 3:
				if (!eq_s_b("i"))
				{
					return false;
				}
				break;
			case 4:
				if (!eq_s_b("o"))
				{
					return false;
				}
				break;
			case 5:
				if (!eq_s_b("ä"))
				{
					return false;
				}
				break;
			case 6:
				if (!eq_s_b("ö"))
				{
					return false;
				}
				break;
			case 7:
			{
				int num2 = limit - cursor;
				int num3 = limit - cursor;
				int num4 = limit - cursor;
				if (!r_LONG())
				{
					cursor = limit - num4;
					if (!eq_s_b("ie"))
					{
						cursor = limit - num2;
						break;
					}
				}
				cursor = limit - num3;
				if (cursor <= base.limit_backward)
				{
					cursor = limit - num2;
					break;
				}
				cursor--;
				bra = cursor;
				break;
			}
			case 8:
				if (in_grouping_b(g_V1, 97, 246, repeat: false) != 0)
				{
					return false;
				}
				if (in_grouping_b(g_C, 98, 122, repeat: false) != 0)
				{
					return false;
				}
				break;
			}
			slice_del();
			B_ending_removed = true;
			return true;
		}

		private bool r_other_endings()
		{
			if (cursor < I_p2)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p2;
			ket = cursor;
			int num = find_among_b(a_7);
			if (num == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			base.limit_backward = limit_backward;
			if (num == 1)
			{
				int num2 = limit - cursor;
				if (eq_s_b("po"))
				{
					return false;
				}
				cursor = limit - num2;
			}
			slice_del();
			return true;
		}

		private bool r_i_plural()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			if (find_among_b(a_8) == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			base.limit_backward = limit_backward;
			slice_del();
			return true;
		}

		private bool r_t_plural()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			ket = cursor;
			if (!eq_s_b("t"))
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			int num = limit - cursor;
			if (in_grouping_b(g_V1, 97, 246, repeat: false) != 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			cursor = limit - num;
			slice_del();
			base.limit_backward = limit_backward;
			if (cursor < I_p2)
			{
				return false;
			}
			int limit_backward2 = base.limit_backward;
			base.limit_backward = I_p2;
			ket = cursor;
			int num2 = find_among_b(a_9);
			if (num2 == 0)
			{
				base.limit_backward = limit_backward2;
				return false;
			}
			bra = cursor;
			base.limit_backward = limit_backward2;
			if (num2 == 1)
			{
				int num3 = limit - cursor;
				if (eq_s_b("po"))
				{
					return false;
				}
				cursor = limit - num3;
			}
			slice_del();
			return true;
		}

		private bool r_tidy()
		{
			if (cursor < I_p1)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_p1;
			int num = limit - cursor;
			int num2 = limit - cursor;
			if (r_LONG())
			{
				cursor = limit - num2;
				ket = cursor;
				if (cursor > base.limit_backward)
				{
					cursor--;
					bra = cursor;
					slice_del();
				}
			}
			cursor = limit - num;
			int num3 = limit - cursor;
			ket = cursor;
			if (in_grouping_b(g_AEI, 97, 228, repeat: false) == 0)
			{
				bra = cursor;
				if (in_grouping_b(g_C, 98, 122, repeat: false) == 0)
				{
					slice_del();
				}
			}
			cursor = limit - num3;
			int num4 = limit - cursor;
			ket = cursor;
			if (eq_s_b("j"))
			{
				bra = cursor;
				int num5 = limit - cursor;
				if (!eq_s_b("o"))
				{
					cursor = limit - num5;
					if (!eq_s_b("u"))
					{
						goto IL_0170;
					}
				}
				slice_del();
			}
			goto IL_0170;
			IL_0170:
			cursor = limit - num4;
			int num6 = limit - cursor;
			ket = cursor;
			if (eq_s_b("o"))
			{
				bra = cursor;
				if (eq_s_b("j"))
				{
					slice_del();
				}
			}
			cursor = limit - num6;
			base.limit_backward = limit_backward;
			if (in_grouping_b(g_V1, 97, 246, repeat: true) < 0)
			{
				return false;
			}
			ket = cursor;
			if (in_grouping_b(g_C, 98, 122, repeat: false) != 0)
			{
				return false;
			}
			bra = cursor;
			slice_to(S_x);
			if (!eq_s_b(S_x))
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
			B_ending_removed = false;
			limit_backward = base.cursor;
			base.cursor = limit;
			int num = limit - base.cursor;
			r_particle_etc();
			base.cursor = limit - num;
			int num2 = limit - base.cursor;
			r_possessive();
			base.cursor = limit - num2;
			int num3 = limit - base.cursor;
			r_case_ending();
			base.cursor = limit - num3;
			int num4 = limit - base.cursor;
			r_other_endings();
			base.cursor = limit - num4;
			if (B_ending_removed)
			{
				int num5 = limit - base.cursor;
				r_i_plural();
				base.cursor = limit - num5;
			}
			else
			{
				int num6 = limit - base.cursor;
				r_t_plural();
				base.cursor = limit - num6;
			}
			int num7 = limit - base.cursor;
			r_tidy();
			base.cursor = limit - num7;
			base.cursor = limit_backward;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("fi", () => new Stemmer_fi(), 1383118328);
				}
				_registered = true;
			}
		}
	}
}
