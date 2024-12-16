using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_hu : Stemmer
	{
		private int I_p1;

		private static string g_v = "aeiouáéíóöőúüű";

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

		private readonly Among[] a_10;

		private readonly Among[] a_11;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_hu()
		{
			a_0 = new Among[8]
			{
				new Among("cs", -1, -1),
				new Among("dzs", -1, -1),
				new Among("gy", -1, -1),
				new Among("ly", -1, -1),
				new Among("ny", -1, -1),
				new Among("sz", -1, -1),
				new Among("ty", -1, -1),
				new Among("zs", -1, -1)
			};
			a_1 = new Among[2]
			{
				new Among("á", -1, 1),
				new Among("é", -1, 2)
			};
			a_2 = new Among[23]
			{
				new Among("bb", -1, -1),
				new Among("cc", -1, -1),
				new Among("dd", -1, -1),
				new Among("ff", -1, -1),
				new Among("gg", -1, -1),
				new Among("jj", -1, -1),
				new Among("kk", -1, -1),
				new Among("ll", -1, -1),
				new Among("mm", -1, -1),
				new Among("nn", -1, -1),
				new Among("pp", -1, -1),
				new Among("rr", -1, -1),
				new Among("ccs", -1, -1),
				new Among("ss", -1, -1),
				new Among("zzs", -1, -1),
				new Among("tt", -1, -1),
				new Among("vv", -1, -1),
				new Among("ggy", -1, -1),
				new Among("lly", -1, -1),
				new Among("nny", -1, -1),
				new Among("tty", -1, -1),
				new Among("ssz", -1, -1),
				new Among("zz", -1, -1)
			};
			a_3 = new Among[2]
			{
				new Among("al", -1, 1),
				new Among("el", -1, 1)
			};
			a_4 = new Among[44]
			{
				new Among("ba", -1, -1),
				new Among("ra", -1, -1),
				new Among("be", -1, -1),
				new Among("re", -1, -1),
				new Among("ig", -1, -1),
				new Among("nak", -1, -1),
				new Among("nek", -1, -1),
				new Among("val", -1, -1),
				new Among("vel", -1, -1),
				new Among("ul", -1, -1),
				new Among("nál", -1, -1),
				new Among("nél", -1, -1),
				new Among("ból", -1, -1),
				new Among("ról", -1, -1),
				new Among("tól", -1, -1),
				new Among("ül", -1, -1),
				new Among("ből", -1, -1),
				new Among("ről", -1, -1),
				new Among("től", -1, -1),
				new Among("n", -1, -1),
				new Among("an", 19, -1),
				new Among("ban", 20, -1),
				new Among("en", 19, -1),
				new Among("ben", 22, -1),
				new Among("képpen", 22, -1),
				new Among("on", 19, -1),
				new Among("ön", 19, -1),
				new Among("képp", -1, -1),
				new Among("kor", -1, -1),
				new Among("t", -1, -1),
				new Among("at", 29, -1),
				new Among("et", 29, -1),
				new Among("ként", 29, -1),
				new Among("anként", 32, -1),
				new Among("enként", 32, -1),
				new Among("onként", 32, -1),
				new Among("ot", 29, -1),
				new Among("ért", 29, -1),
				new Among("öt", 29, -1),
				new Among("hez", -1, -1),
				new Among("hoz", -1, -1),
				new Among("höz", -1, -1),
				new Among("vá", -1, -1),
				new Among("vé", -1, -1)
			};
			a_5 = new Among[3]
			{
				new Among("án", -1, 2),
				new Among("én", -1, 1),
				new Among("ánként", -1, 2)
			};
			a_6 = new Among[6]
			{
				new Among("stul", -1, 1),
				new Among("astul", 0, 1),
				new Among("ástul", 0, 2),
				new Among("stül", -1, 1),
				new Among("estül", 3, 1),
				new Among("éstül", 3, 3)
			};
			a_7 = new Among[2]
			{
				new Among("á", -1, 1),
				new Among("é", -1, 1)
			};
			a_8 = new Among[7]
			{
				new Among("k", -1, 3),
				new Among("ak", 0, 3),
				new Among("ek", 0, 3),
				new Among("ok", 0, 3),
				new Among("ák", 0, 1),
				new Among("ék", 0, 2),
				new Among("ök", 0, 3)
			};
			a_9 = new Among[12]
			{
				new Among("éi", -1, 1),
				new Among("áéi", 0, 3),
				new Among("ééi", 0, 2),
				new Among("é", -1, 1),
				new Among("ké", 3, 1),
				new Among("aké", 4, 1),
				new Among("eké", 4, 1),
				new Among("oké", 4, 1),
				new Among("áké", 4, 3),
				new Among("éké", 4, 2),
				new Among("öké", 4, 1),
				new Among("éé", 3, 2)
			};
			a_10 = new Among[31]
			{
				new Among("a", -1, 1),
				new Among("ja", 0, 1),
				new Among("d", -1, 1),
				new Among("ad", 2, 1),
				new Among("ed", 2, 1),
				new Among("od", 2, 1),
				new Among("ád", 2, 2),
				new Among("éd", 2, 3),
				new Among("öd", 2, 1),
				new Among("e", -1, 1),
				new Among("je", 9, 1),
				new Among("nk", -1, 1),
				new Among("unk", 11, 1),
				new Among("ánk", 11, 2),
				new Among("énk", 11, 3),
				new Among("ünk", 11, 1),
				new Among("uk", -1, 1),
				new Among("juk", 16, 1),
				new Among("ájuk", 17, 2),
				new Among("ük", -1, 1),
				new Among("jük", 19, 1),
				new Among("éjük", 20, 3),
				new Among("m", -1, 1),
				new Among("am", 22, 1),
				new Among("em", 22, 1),
				new Among("om", 22, 1),
				new Among("ám", 22, 2),
				new Among("ém", 22, 3),
				new Among("o", -1, 1),
				new Among("á", -1, 2),
				new Among("é", -1, 3)
			};
			a_11 = new Among[42]
			{
				new Among("id", -1, 1),
				new Among("aid", 0, 1),
				new Among("jaid", 1, 1),
				new Among("eid", 0, 1),
				new Among("jeid", 3, 1),
				new Among("áid", 0, 2),
				new Among("éid", 0, 3),
				new Among("i", -1, 1),
				new Among("ai", 7, 1),
				new Among("jai", 8, 1),
				new Among("ei", 7, 1),
				new Among("jei", 10, 1),
				new Among("ái", 7, 2),
				new Among("éi", 7, 3),
				new Among("itek", -1, 1),
				new Among("eitek", 14, 1),
				new Among("jeitek", 15, 1),
				new Among("éitek", 14, 3),
				new Among("ik", -1, 1),
				new Among("aik", 18, 1),
				new Among("jaik", 19, 1),
				new Among("eik", 18, 1),
				new Among("jeik", 21, 1),
				new Among("áik", 18, 2),
				new Among("éik", 18, 3),
				new Among("ink", -1, 1),
				new Among("aink", 25, 1),
				new Among("jaink", 26, 1),
				new Among("eink", 25, 1),
				new Among("jeink", 28, 1),
				new Among("áink", 25, 2),
				new Among("éink", 25, 3),
				new Among("aitok", -1, 1),
				new Among("jaitok", 32, 1),
				new Among("áitok", -1, 2),
				new Among("im", -1, 1),
				new Among("aim", 35, 1),
				new Among("jaim", 36, 1),
				new Among("eim", 35, 1),
				new Among("jeim", 38, 1),
				new Among("áim", 35, 2),
				new Among("éim", 35, 3)
			};
		}

		private bool r_mark_regions()
		{
			I_p1 = limit;
			int cursor = base.cursor;
			if (in_grouping(g_v, 97, 369, repeat: false) != 0 || in_grouping(g_v, 97, 369, repeat: true) < 0)
			{
				goto IL_0084;
			}
			int cursor2 = base.cursor;
			if (find_among(a_0) == 0)
			{
				base.cursor = cursor2;
				if (base.cursor >= limit)
				{
					goto IL_0084;
				}
				base.cursor++;
			}
			I_p1 = base.cursor;
			goto IL_00d6;
			IL_0084:
			base.cursor = cursor;
			if (out_grouping(g_v, 97, 369, repeat: false) != 0)
			{
				return false;
			}
			int num = out_grouping(g_v, 97, 369, repeat: true);
			if (num < 0)
			{
				return false;
			}
			base.cursor += num;
			I_p1 = base.cursor;
			goto IL_00d6;
			IL_00d6:
			return true;
		}

		private bool r_R1()
		{
			return I_p1 <= cursor;
		}

		private bool r_v_ending()
		{
			ket = cursor;
			int num = find_among_b(a_1);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_from("a");
				break;
			case 2:
				slice_from("e");
				break;
			}
			return true;
		}

		private bool r_double()
		{
			int num = limit - cursor;
			if (find_among_b(a_2) == 0)
			{
				return false;
			}
			cursor = limit - num;
			return true;
		}

		private bool r_undouble()
		{
			if (cursor <= limit_backward)
			{
				return false;
			}
			cursor--;
			ket = cursor;
			int num = cursor - 1;
			if (limit_backward > num || num > limit)
			{
				return false;
			}
			cursor = num;
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_instrum()
		{
			ket = cursor;
			if (find_among_b(a_3) == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			if (!r_double())
			{
				return false;
			}
			slice_del();
			return r_undouble();
		}

		private bool r_case()
		{
			ket = cursor;
			if (find_among_b(a_4) == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			slice_del();
			return r_v_ending();
		}

		private bool r_case_special()
		{
			ket = cursor;
			int num = find_among_b(a_5);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_from("e");
				break;
			case 2:
				slice_from("a");
				break;
			}
			return true;
		}

		private bool r_case_other()
		{
			ket = cursor;
			int num = find_among_b(a_6);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("a");
				break;
			case 3:
				slice_from("e");
				break;
			}
			return true;
		}

		private bool r_factive()
		{
			ket = cursor;
			if (find_among_b(a_7) == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			if (!r_double())
			{
				return false;
			}
			slice_del();
			return r_undouble();
		}

		private bool r_plural()
		{
			ket = cursor;
			int num = find_among_b(a_8);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_from("a");
				break;
			case 2:
				slice_from("e");
				break;
			case 3:
				slice_del();
				break;
			}
			return true;
		}

		private bool r_owned()
		{
			ket = cursor;
			int num = find_among_b(a_9);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("e");
				break;
			case 3:
				slice_from("a");
				break;
			}
			return true;
		}

		private bool r_sing_owner()
		{
			ket = cursor;
			int num = find_among_b(a_10);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("a");
				break;
			case 3:
				slice_from("e");
				break;
			}
			return true;
		}

		private bool r_plur_owner()
		{
			ket = cursor;
			int num = find_among_b(a_11);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("a");
				break;
			case 3:
				slice_from("e");
				break;
			}
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
			r_instrum();
			base.cursor = limit - num;
			int num2 = limit - base.cursor;
			r_case();
			base.cursor = limit - num2;
			int num3 = limit - base.cursor;
			r_case_special();
			base.cursor = limit - num3;
			int num4 = limit - base.cursor;
			r_case_other();
			base.cursor = limit - num4;
			int num5 = limit - base.cursor;
			r_factive();
			base.cursor = limit - num5;
			int num6 = limit - base.cursor;
			r_owned();
			base.cursor = limit - num6;
			int num7 = limit - base.cursor;
			r_sing_owner();
			base.cursor = limit - num7;
			int num8 = limit - base.cursor;
			r_plur_owner();
			base.cursor = limit - num8;
			int num9 = limit - base.cursor;
			r_plural();
			base.cursor = limit - num9;
			base.cursor = limit_backward;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("hu", () => new Stemmer_hu(), 1867141815);
				}
				_registered = true;
			}
		}
	}
}
