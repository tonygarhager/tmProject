using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_nl : Stemmer
	{
		private int I_p2;

		private int I_p1;

		private bool B_e_found;

		private static string g_v = "aeiouyè";

		private static string g_v_I = "aeiouyèI";

		private static string g_v_j = "aeiouyèj";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private readonly Among[] a_5;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_nl()
		{
			a_0 = new Among[11]
			{
				new Among("", -1, 6),
				new Among("á", 0, 1),
				new Among("ä", 0, 1),
				new Among("é", 0, 2),
				new Among("ë", 0, 2),
				new Among("í", 0, 3),
				new Among("ï", 0, 3),
				new Among("ó", 0, 4),
				new Among("ö", 0, 4),
				new Among("ú", 0, 5),
				new Among("ü", 0, 5)
			};
			a_1 = new Among[3]
			{
				new Among("", -1, 3),
				new Among("I", 0, 2),
				new Among("Y", 0, 1)
			};
			a_2 = new Among[3]
			{
				new Among("dd", -1, -1),
				new Among("kk", -1, -1),
				new Among("tt", -1, -1)
			};
			a_3 = new Among[5]
			{
				new Among("ene", -1, 2),
				new Among("se", -1, 3),
				new Among("en", -1, 2),
				new Among("heden", 2, 1),
				new Among("s", -1, 3)
			};
			a_4 = new Among[6]
			{
				new Among("end", -1, 1),
				new Among("ig", -1, 2),
				new Among("ing", -1, 1),
				new Among("lijk", -1, 3),
				new Among("baar", -1, 4),
				new Among("bar", -1, 5)
			};
			a_5 = new Among[4]
			{
				new Among("aa", -1, -1),
				new Among("ee", -1, -1),
				new Among("oo", -1, -1),
				new Among("uu", -1, -1)
			};
		}

		private bool r_prelude()
		{
			int cursor = base.cursor;
			int cursor2;
			while (true)
			{
				cursor2 = base.cursor;
				bra = base.cursor;
				int num = find_among(a_0);
				if (num == 0)
				{
					break;
				}
				ket = base.cursor;
				switch (num)
				{
				default:
					continue;
				case 1:
					slice_from("a");
					continue;
				case 2:
					slice_from("e");
					continue;
				case 3:
					slice_from("i");
					continue;
				case 4:
					slice_from("o");
					continue;
				case 5:
					slice_from("u");
					continue;
				case 6:
					break;
				}
				if (base.cursor >= limit)
				{
					break;
				}
				base.cursor++;
			}
			base.cursor = cursor2;
			base.cursor = cursor;
			int cursor3 = base.cursor;
			bra = base.cursor;
			if (!eq_s("y"))
			{
				base.cursor = cursor3;
			}
			else
			{
				ket = base.cursor;
				slice_from("Y");
			}
			while (true)
			{
				int cursor4 = base.cursor;
				int cursor5;
				while (true)
				{
					cursor5 = base.cursor;
					if (in_grouping(g_v, 97, 232, repeat: false) == 0)
					{
						bra = base.cursor;
						int cursor6 = base.cursor;
						if (eq_s("i"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 232, repeat: false) == 0)
							{
								slice_from("I");
								break;
							}
						}
						base.cursor = cursor6;
						if (eq_s("y"))
						{
							ket = base.cursor;
							slice_from("Y");
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
					return true;
				}
				base.cursor = cursor5;
			}
		}

		private bool r_mark_regions()
		{
			I_p1 = limit;
			I_p2 = limit;
			int num = out_grouping(g_v, 97, 232, repeat: true);
			if (num < 0)
			{
				return false;
			}
			cursor += num;
			int num2 = in_grouping(g_v, 97, 232, repeat: true);
			if (num2 < 0)
			{
				return false;
			}
			cursor += num2;
			I_p1 = cursor;
			if (I_p1 < 3)
			{
				I_p1 = 3;
			}
			int num3 = out_grouping(g_v, 97, 232, repeat: true);
			if (num3 < 0)
			{
				return false;
			}
			cursor += num3;
			int num4 = in_grouping(g_v, 97, 232, repeat: true);
			if (num4 < 0)
			{
				return false;
			}
			cursor += num4;
			I_p2 = cursor;
			return true;
		}

		private bool r_postlude()
		{
			int cursor;
			while (true)
			{
				cursor = base.cursor;
				bra = base.cursor;
				int num = find_among(a_1);
				if (num == 0)
				{
					break;
				}
				ket = base.cursor;
				switch (num)
				{
				default:
					continue;
				case 1:
					slice_from("y");
					continue;
				case 2:
					slice_from("i");
					continue;
				case 3:
					break;
				}
				if (base.cursor >= limit)
				{
					break;
				}
				base.cursor++;
			}
			base.cursor = cursor;
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

		private bool r_undouble()
		{
			int num = limit - cursor;
			if (find_among_b(a_2) == 0)
			{
				return false;
			}
			cursor = limit - num;
			ket = cursor;
			if (cursor <= limit_backward)
			{
				return false;
			}
			cursor--;
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_e_ending()
		{
			B_e_found = false;
			ket = cursor;
			if (!eq_s_b("e"))
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			int num = limit - cursor;
			if (out_grouping_b(g_v, 97, 232, repeat: false) != 0)
			{
				return false;
			}
			cursor = limit - num;
			slice_del();
			B_e_found = true;
			return r_undouble();
		}

		private bool r_en_ending()
		{
			if (!r_R1())
			{
				return false;
			}
			int num = limit - cursor;
			if (out_grouping_b(g_v, 97, 232, repeat: false) != 0)
			{
				return false;
			}
			cursor = limit - num;
			int num2 = limit - cursor;
			if (eq_s_b("gem"))
			{
				return false;
			}
			cursor = limit - num2;
			slice_del();
			return r_undouble();
		}

		private bool r_standard_suffix()
		{
			int num = limit - cursor;
			ket = cursor;
			int num2 = find_among_b(a_3);
			if (num2 != 0)
			{
				bra = cursor;
				switch (num2)
				{
				case 1:
					if (r_R1())
					{
						slice_from("heid");
					}
					break;
				case 2:
					if (r_en_ending())
					{
					}
					break;
				case 3:
					if (r_R1() && out_grouping_b(g_v_j, 97, 232, repeat: false) == 0)
					{
						slice_del();
					}
					break;
				}
			}
			cursor = limit - num;
			int num3 = limit - cursor;
			r_e_ending();
			cursor = limit - num3;
			int num4 = limit - cursor;
			ket = cursor;
			if (eq_s_b("heid"))
			{
				bra = cursor;
				if (r_R2())
				{
					int num5 = limit - cursor;
					if (!eq_s_b("c"))
					{
						cursor = limit - num5;
						slice_del();
						ket = cursor;
						if (eq_s_b("en"))
						{
							bra = cursor;
							r_en_ending();
						}
					}
				}
			}
			cursor = limit - num4;
			int num6 = limit - cursor;
			ket = cursor;
			num2 = find_among_b(a_4);
			if (num2 != 0)
			{
				bra = cursor;
				switch (num2)
				{
				case 1:
				{
					if (!r_R2())
					{
						break;
					}
					slice_del();
					int num7 = limit - cursor;
					ket = cursor;
					if (eq_s_b("ig"))
					{
						bra = cursor;
						if (r_R2())
						{
							int num8 = limit - cursor;
							if (!eq_s_b("e"))
							{
								cursor = limit - num8;
								slice_del();
								break;
							}
						}
					}
					cursor = limit - num7;
					if (r_undouble())
					{
					}
					break;
				}
				case 2:
					if (r_R2())
					{
						int num9 = limit - cursor;
						if (!eq_s_b("e"))
						{
							cursor = limit - num9;
							slice_del();
						}
					}
					break;
				case 3:
					if (r_R2())
					{
						slice_del();
						if (r_e_ending())
						{
						}
					}
					break;
				case 4:
					if (r_R2())
					{
						slice_del();
					}
					break;
				case 5:
					if (r_R2() && B_e_found)
					{
						slice_del();
					}
					break;
				}
			}
			cursor = limit - num6;
			int num10 = limit - cursor;
			if (out_grouping_b(g_v_I, 73, 232, repeat: false) == 0)
			{
				int num11 = limit - cursor;
				if (find_among_b(a_5) != 0 && out_grouping_b(g_v, 97, 232, repeat: false) == 0)
				{
					cursor = limit - num11;
					ket = cursor;
					if (cursor > limit_backward)
					{
						cursor--;
						bra = cursor;
						slice_del();
					}
				}
			}
			cursor = limit - num10;
			return true;
		}

		protected override bool stem()
		{
			int cursor = base.cursor;
			r_prelude();
			base.cursor = cursor;
			int cursor2 = base.cursor;
			r_mark_regions();
			base.cursor = cursor2;
			limit_backward = base.cursor;
			base.cursor = limit;
			r_standard_suffix();
			base.cursor = limit_backward;
			int cursor3 = base.cursor;
			r_postlude();
			base.cursor = cursor3;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("nl", () => new Stemmer_nl(), 1543086763);
				}
				_registered = true;
			}
		}
	}
}
