using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_en : Stemmer
	{
		private bool B_Y_found;

		private int I_p2;

		private int I_p1;

		private static string g_v = "aeiouy";

		private static string g_v_WXY = "aeiouywxY";

		private static string g_valid_LI = "cdeghkmnrt";

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

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_en()
		{
			a_0 = new Among[3]
			{
				new Among("arsen", -1, -1),
				new Among("commun", -1, -1),
				new Among("gener", -1, -1)
			};
			a_1 = new Among[3]
			{
				new Among("'", -1, 1),
				new Among("'s'", 0, 1),
				new Among("'s", -1, 1)
			};
			a_2 = new Among[6]
			{
				new Among("ied", -1, 2),
				new Among("s", -1, 3),
				new Among("ies", 1, 2),
				new Among("sses", 1, 1),
				new Among("ss", 1, -1),
				new Among("us", 1, -1)
			};
			a_3 = new Among[13]
			{
				new Among("", -1, 3),
				new Among("bb", 0, 2),
				new Among("dd", 0, 2),
				new Among("ff", 0, 2),
				new Among("gg", 0, 2),
				new Among("bl", 0, 1),
				new Among("mm", 0, 2),
				new Among("nn", 0, 2),
				new Among("pp", 0, 2),
				new Among("rr", 0, 2),
				new Among("at", 0, 1),
				new Among("tt", 0, 2),
				new Among("iz", 0, 1)
			};
			a_4 = new Among[6]
			{
				new Among("ed", -1, 2),
				new Among("eed", 0, 1),
				new Among("ing", -1, 2),
				new Among("edly", -1, 2),
				new Among("eedly", 3, 1),
				new Among("ingly", -1, 2)
			};
			a_5 = new Among[24]
			{
				new Among("anci", -1, 3),
				new Among("enci", -1, 2),
				new Among("ogi", -1, 13),
				new Among("li", -1, 15),
				new Among("bli", 3, 12),
				new Among("abli", 4, 4),
				new Among("alli", 3, 8),
				new Among("fulli", 3, 9),
				new Among("lessli", 3, 14),
				new Among("ousli", 3, 10),
				new Among("entli", 3, 5),
				new Among("aliti", -1, 8),
				new Among("biliti", -1, 12),
				new Among("iviti", -1, 11),
				new Among("tional", -1, 1),
				new Among("ational", 14, 7),
				new Among("alism", -1, 8),
				new Among("ation", -1, 7),
				new Among("ization", 17, 6),
				new Among("izer", -1, 6),
				new Among("ator", -1, 7),
				new Among("iveness", -1, 11),
				new Among("fulness", -1, 9),
				new Among("ousness", -1, 10)
			};
			a_6 = new Among[9]
			{
				new Among("icate", -1, 4),
				new Among("ative", -1, 6),
				new Among("alize", -1, 3),
				new Among("iciti", -1, 4),
				new Among("ical", -1, 4),
				new Among("tional", -1, 1),
				new Among("ational", 5, 2),
				new Among("ful", -1, 5),
				new Among("ness", -1, 5)
			};
			a_7 = new Among[18]
			{
				new Among("ic", -1, 1),
				new Among("ance", -1, 1),
				new Among("ence", -1, 1),
				new Among("able", -1, 1),
				new Among("ible", -1, 1),
				new Among("ate", -1, 1),
				new Among("ive", -1, 1),
				new Among("ize", -1, 1),
				new Among("iti", -1, 1),
				new Among("al", -1, 1),
				new Among("ism", -1, 1),
				new Among("ion", -1, 2),
				new Among("er", -1, 1),
				new Among("ous", -1, 1),
				new Among("ant", -1, 1),
				new Among("ent", -1, 1),
				new Among("ment", 15, 1),
				new Among("ement", 16, 1)
			};
			a_8 = new Among[2]
			{
				new Among("e", -1, 1),
				new Among("l", -1, 2)
			};
			a_9 = new Among[8]
			{
				new Among("succeed", -1, -1),
				new Among("proceed", -1, -1),
				new Among("exceed", -1, -1),
				new Among("canning", -1, -1),
				new Among("inning", -1, -1),
				new Among("earring", -1, -1),
				new Among("herring", -1, -1),
				new Among("outing", -1, -1)
			};
			a_10 = new Among[18]
			{
				new Among("andes", -1, -1),
				new Among("atlas", -1, -1),
				new Among("bias", -1, -1),
				new Among("cosmos", -1, -1),
				new Among("dying", -1, 3),
				new Among("early", -1, 9),
				new Among("gently", -1, 7),
				new Among("howe", -1, -1),
				new Among("idly", -1, 6),
				new Among("lying", -1, 4),
				new Among("news", -1, -1),
				new Among("only", -1, 10),
				new Among("singly", -1, 11),
				new Among("skies", -1, 2),
				new Among("skis", -1, 1),
				new Among("sky", -1, -1),
				new Among("tying", -1, 5),
				new Among("ugly", -1, 8)
			};
		}

		private bool r_prelude()
		{
			B_Y_found = false;
			int cursor = base.cursor;
			bra = base.cursor;
			if (eq_s("'"))
			{
				ket = base.cursor;
				slice_del();
			}
			base.cursor = cursor;
			int cursor2 = base.cursor;
			bra = base.cursor;
			if (eq_s("y"))
			{
				ket = base.cursor;
				slice_from("Y");
				B_Y_found = true;
			}
			base.cursor = cursor2;
			int cursor3 = base.cursor;
			while (true)
			{
				int cursor4 = base.cursor;
				int cursor5;
				while (true)
				{
					cursor5 = base.cursor;
					if (in_grouping(g_v, 97, 121, repeat: false) == 0)
					{
						bra = base.cursor;
						if (eq_s("y"))
						{
							break;
						}
					}
					base.cursor = cursor5;
					if (base.cursor < limit)
					{
						base.cursor++;
						continue;
					}
					base.cursor = cursor4;
					base.cursor = cursor3;
					return true;
				}
				ket = base.cursor;
				base.cursor = cursor5;
				slice_from("Y");
				B_Y_found = true;
			}
		}

		private bool r_mark_regions()
		{
			I_p1 = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int cursor2 = base.cursor;
			if (find_among(a_0) != 0)
			{
				goto IL_0084;
			}
			base.cursor = cursor2;
			int num = out_grouping(g_v, 97, 121, repeat: true);
			if (num >= 0)
			{
				base.cursor += num;
				int num2 = in_grouping(g_v, 97, 121, repeat: true);
				if (num2 >= 0)
				{
					base.cursor += num2;
					goto IL_0084;
				}
			}
			goto IL_00e8;
			IL_00e8:
			base.cursor = cursor;
			return true;
			IL_0084:
			I_p1 = base.cursor;
			int num3 = out_grouping(g_v, 97, 121, repeat: true);
			if (num3 >= 0)
			{
				base.cursor += num3;
				int num4 = in_grouping(g_v, 97, 121, repeat: true);
				if (num4 >= 0)
				{
					base.cursor += num4;
					I_p2 = base.cursor;
				}
			}
			goto IL_00e8;
		}

		private bool r_shortv()
		{
			int num = limit - cursor;
			if (out_grouping_b(g_v_WXY, 89, 121, repeat: false) != 0 || in_grouping_b(g_v, 97, 121, repeat: false) != 0 || out_grouping_b(g_v, 97, 121, repeat: false) != 0)
			{
				cursor = limit - num;
				if (out_grouping_b(g_v, 97, 121, repeat: false) != 0)
				{
					return false;
				}
				if (in_grouping_b(g_v, 97, 121, repeat: false) != 0)
				{
					return false;
				}
				if (cursor > limit_backward)
				{
					return false;
				}
			}
			return true;
		}

		private bool r_R1()
		{
			return I_p1 <= cursor;
		}

		private bool r_R2()
		{
			return I_p2 <= cursor;
		}

		private bool r_Step_1a()
		{
			int num = limit - cursor;
			ket = cursor;
			if (find_among_b(a_1) == 0)
			{
				cursor = limit - num;
			}
			else
			{
				bra = cursor;
				slice_del();
			}
			ket = cursor;
			int num2 = find_among_b(a_2);
			if (num2 == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num2)
			{
			case 1:
				slice_from("ss");
				break;
			case 2:
			{
				int num4 = limit - cursor;
				int num5 = cursor - 2;
				if (limit_backward <= num5 && num5 <= limit)
				{
					cursor = num5;
					slice_from("i");
				}
				else
				{
					cursor = limit - num4;
					slice_from("ie");
				}
				break;
			}
			case 3:
			{
				if (cursor <= limit_backward)
				{
					return false;
				}
				cursor--;
				int num3 = out_grouping_b(g_v, 97, 121, repeat: true);
				if (num3 < 0)
				{
					return false;
				}
				cursor -= num3;
				slice_del();
				break;
			}
			}
			return true;
		}

		private bool r_Step_1b()
		{
			ket = base.cursor;
			int num = find_among_b(a_4);
			if (num == 0)
			{
				return false;
			}
			bra = base.cursor;
			switch (num)
			{
			case 1:
				if (!r_R1())
				{
					return false;
				}
				slice_from("ee");
				break;
			case 2:
			{
				int num2 = limit - base.cursor;
				int num3 = out_grouping_b(g_v, 97, 121, repeat: true);
				if (num3 < 0)
				{
					return false;
				}
				base.cursor -= num3;
				base.cursor = limit - num2;
				slice_del();
				int num4 = limit - base.cursor;
				num = find_among_b(a_3);
				if (num == 0)
				{
					return false;
				}
				base.cursor = limit - num4;
				switch (num)
				{
				case 1:
				{
					int cursor2 = base.cursor;
					insert(base.cursor, base.cursor, "e");
					base.cursor = cursor2;
					break;
				}
				case 2:
					ket = base.cursor;
					if (base.cursor <= limit_backward)
					{
						return false;
					}
					base.cursor--;
					bra = base.cursor;
					slice_del();
					break;
				case 3:
				{
					if (base.cursor != I_p1)
					{
						return false;
					}
					int num5 = limit - base.cursor;
					if (!r_shortv())
					{
						return false;
					}
					base.cursor = limit - num5;
					int cursor = base.cursor;
					insert(base.cursor, base.cursor, "e");
					base.cursor = cursor;
					break;
				}
				}
				break;
			}
			}
			return true;
		}

		private bool r_Step_1c()
		{
			ket = cursor;
			int num = limit - cursor;
			if (!eq_s_b("y"))
			{
				cursor = limit - num;
				if (!eq_s_b("Y"))
				{
					return false;
				}
			}
			bra = cursor;
			if (out_grouping_b(g_v, 97, 121, repeat: false) != 0)
			{
				return false;
			}
			if (cursor <= limit_backward)
			{
				return false;
			}
			slice_from("i");
			return true;
		}

		private bool r_Step_2()
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
				slice_from("tion");
				break;
			case 2:
				slice_from("ence");
				break;
			case 3:
				slice_from("ance");
				break;
			case 4:
				slice_from("able");
				break;
			case 5:
				slice_from("ent");
				break;
			case 6:
				slice_from("ize");
				break;
			case 7:
				slice_from("ate");
				break;
			case 8:
				slice_from("al");
				break;
			case 9:
				slice_from("ful");
				break;
			case 10:
				slice_from("ous");
				break;
			case 11:
				slice_from("ive");
				break;
			case 12:
				slice_from("ble");
				break;
			case 13:
				if (!eq_s_b("l"))
				{
					return false;
				}
				slice_from("og");
				break;
			case 14:
				slice_from("less");
				break;
			case 15:
				if (in_grouping_b(g_valid_LI, 99, 116, repeat: false) != 0)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_Step_3()
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
				slice_from("tion");
				break;
			case 2:
				slice_from("ate");
				break;
			case 3:
				slice_from("al");
				break;
			case 4:
				slice_from("ic");
				break;
			case 5:
				slice_del();
				break;
			case 6:
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_Step_4()
		{
			ket = cursor;
			int num = find_among_b(a_7);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R2())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
			{
				int num2 = limit - cursor;
				if (!eq_s_b("s"))
				{
					cursor = limit - num2;
					if (!eq_s_b("t"))
					{
						return false;
					}
				}
				slice_del();
				break;
			}
			}
			return true;
		}

		private bool r_Step_5()
		{
			ket = cursor;
			int num = find_among_b(a_8);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
			{
				int num2 = limit - cursor;
				if (!r_R2())
				{
					cursor = limit - num2;
					if (!r_R1())
					{
						return false;
					}
					int num3 = limit - cursor;
					if (r_shortv())
					{
						return false;
					}
					cursor = limit - num3;
				}
				slice_del();
				break;
			}
			case 2:
				if (!r_R2())
				{
					return false;
				}
				if (!eq_s_b("l"))
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_exception2()
		{
			ket = cursor;
			if (find_among_b(a_9) == 0)
			{
				return false;
			}
			bra = cursor;
			if (cursor > limit_backward)
			{
				return false;
			}
			return true;
		}

		private bool r_exception1()
		{
			bra = cursor;
			int num = find_among(a_10);
			if (num == 0)
			{
				return false;
			}
			ket = cursor;
			if (cursor < limit)
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_from("ski");
				break;
			case 2:
				slice_from("sky");
				break;
			case 3:
				slice_from("die");
				break;
			case 4:
				slice_from("lie");
				break;
			case 5:
				slice_from("tie");
				break;
			case 6:
				slice_from("idl");
				break;
			case 7:
				slice_from("gentl");
				break;
			case 8:
				slice_from("ugli");
				break;
			case 9:
				slice_from("earli");
				break;
			case 10:
				slice_from("onli");
				break;
			case 11:
				slice_from("singl");
				break;
			}
			return true;
		}

		private bool r_postlude()
		{
			if (!B_Y_found)
			{
				return false;
			}
			while (true)
			{
				int cursor = base.cursor;
				int cursor2;
				while (true)
				{
					cursor2 = base.cursor;
					bra = base.cursor;
					if (eq_s("Y"))
					{
						break;
					}
					base.cursor = cursor2;
					if (base.cursor < limit)
					{
						base.cursor++;
						continue;
					}
					base.cursor = cursor;
					return true;
				}
				ket = base.cursor;
				base.cursor = cursor2;
				slice_from("y");
			}
		}

		protected override bool stem()
		{
			int cursor = base.cursor;
			if (!r_exception1())
			{
				base.cursor = cursor;
				int cursor2 = base.cursor;
				int num = base.cursor + 3;
				if (0 <= num && num <= limit)
				{
					base.cursor = num;
					base.cursor = cursor;
					r_prelude();
					r_mark_regions();
					limit_backward = base.cursor;
					base.cursor = limit;
					int num2 = limit - base.cursor;
					r_Step_1a();
					base.cursor = limit - num2;
					int num3 = limit - base.cursor;
					if (!r_exception2())
					{
						base.cursor = limit - num3;
						int num4 = limit - base.cursor;
						r_Step_1b();
						base.cursor = limit - num4;
						int num5 = limit - base.cursor;
						r_Step_1c();
						base.cursor = limit - num5;
						int num6 = limit - base.cursor;
						r_Step_2();
						base.cursor = limit - num6;
						int num7 = limit - base.cursor;
						r_Step_3();
						base.cursor = limit - num7;
						int num8 = limit - base.cursor;
						r_Step_4();
						base.cursor = limit - num8;
						int num9 = limit - base.cursor;
						r_Step_5();
						base.cursor = limit - num9;
					}
					base.cursor = limit_backward;
					int cursor3 = base.cursor;
					r_postlude();
					base.cursor = cursor3;
				}
				else
				{
					base.cursor = cursor2;
				}
			}
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("en", () => new Stemmer_en(), -2075786655);
				}
				_registered = true;
			}
		}
	}
}
