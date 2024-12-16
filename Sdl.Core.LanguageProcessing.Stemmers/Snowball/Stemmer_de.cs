using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_de : Stemmer
	{
		private int I_x;

		private int I_p2;

		private int I_p1;

		private static string g_v = "aeiouyäöü";

		private static string g_s_ending = "bdfghklmnrt";

		private static string g_st_ending = "bdfghklmnt";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private readonly Among[] a_5;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_de()
		{
			a_0 = new Among[6]
			{
				new Among("", -1, 6),
				new Among("ae", 0, 2),
				new Among("oe", 0, 3),
				new Among("qu", 0, 5),
				new Among("ue", 0, 4),
				new Among("ß", 0, 1)
			};
			a_1 = new Among[6]
			{
				new Among("", -1, 5),
				new Among("U", 0, 2),
				new Among("Y", 0, 1),
				new Among("ä", 0, 3),
				new Among("ö", 0, 4),
				new Among("ü", 0, 2)
			};
			a_2 = new Among[7]
			{
				new Among("e", -1, 2),
				new Among("em", -1, 1),
				new Among("en", -1, 2),
				new Among("ern", -1, 1),
				new Among("er", -1, 1),
				new Among("s", -1, 3),
				new Among("es", 5, 2)
			};
			a_3 = new Among[4]
			{
				new Among("en", -1, 1),
				new Among("er", -1, 1),
				new Among("st", -1, 2),
				new Among("est", 2, 1)
			};
			a_4 = new Among[2]
			{
				new Among("ig", -1, 1),
				new Among("lich", -1, 1)
			};
			a_5 = new Among[8]
			{
				new Among("end", -1, 1),
				new Among("ig", -1, 2),
				new Among("ung", -1, 1),
				new Among("lich", -1, 3),
				new Among("isch", -1, 2),
				new Among("ik", -1, 2),
				new Among("heit", -1, 3),
				new Among("keit", -1, 4)
			};
		}

		private bool r_prelude()
		{
			int cursor = base.cursor;
			while (true)
			{
				int cursor2 = base.cursor;
				int cursor3;
				while (true)
				{
					cursor3 = base.cursor;
					if (in_grouping(g_v, 97, 252, repeat: false) == 0)
					{
						bra = base.cursor;
						int cursor4 = base.cursor;
						if (eq_s("u"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 252, repeat: false) == 0)
							{
								slice_from("U");
								break;
							}
						}
						base.cursor = cursor4;
						if (eq_s("y"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 252, repeat: false) == 0)
							{
								slice_from("Y");
								break;
							}
						}
					}
					base.cursor = cursor3;
					if (base.cursor < limit)
					{
						base.cursor++;
						continue;
					}
					base.cursor = cursor2;
					base.cursor = cursor;
					int cursor5;
					while (true)
					{
						cursor5 = base.cursor;
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
							slice_from("ss");
							continue;
						case 2:
							slice_from("ä");
							continue;
						case 3:
							slice_from("ö");
							continue;
						case 4:
							slice_from("ü");
							continue;
						case 5:
						{
							int num2 = base.cursor + 2;
							if (0 <= num2 && num2 <= limit)
							{
								base.cursor = num2;
								continue;
							}
							break;
						}
						case 6:
							if (base.cursor < limit)
							{
								base.cursor++;
								continue;
							}
							break;
						}
						break;
					}
					base.cursor = cursor5;
					return true;
				}
				base.cursor = cursor3;
			}
		}

		private bool r_mark_regions()
		{
			I_p1 = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int num = base.cursor + 3;
			if (0 > num || num > limit)
			{
				return false;
			}
			base.cursor = num;
			I_x = base.cursor;
			base.cursor = cursor;
			int num2 = out_grouping(g_v, 97, 252, repeat: true);
			if (num2 < 0)
			{
				return false;
			}
			base.cursor += num2;
			int num3 = in_grouping(g_v, 97, 252, repeat: true);
			if (num3 < 0)
			{
				return false;
			}
			base.cursor += num3;
			I_p1 = base.cursor;
			if (I_p1 < I_x)
			{
				I_p1 = I_x;
			}
			int num4 = out_grouping(g_v, 97, 252, repeat: true);
			if (num4 < 0)
			{
				return false;
			}
			base.cursor += num4;
			int num5 = in_grouping(g_v, 97, 252, repeat: true);
			if (num5 < 0)
			{
				return false;
			}
			base.cursor += num5;
			I_p2 = base.cursor;
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
					slice_from("u");
					continue;
				case 3:
					slice_from("a");
					continue;
				case 4:
					slice_from("o");
					continue;
				case 5:
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

		private bool r_standard_suffix()
		{
			int num = limit - cursor;
			ket = cursor;
			int num2 = find_among_b(a_2);
			if (num2 != 0)
			{
				bra = cursor;
				if (r_R1())
				{
					switch (num2)
					{
					case 1:
						slice_del();
						break;
					case 2:
					{
						slice_del();
						int num3 = limit - cursor;
						ket = cursor;
						if (!eq_s_b("s"))
						{
							cursor = limit - num3;
							break;
						}
						bra = cursor;
						if (!eq_s_b("nis"))
						{
							cursor = limit - num3;
						}
						else
						{
							slice_del();
						}
						break;
					}
					case 3:
						if (in_grouping_b(g_s_ending, 98, 116, repeat: false) == 0)
						{
							slice_del();
						}
						break;
					}
				}
			}
			cursor = limit - num;
			int num4 = limit - cursor;
			ket = cursor;
			num2 = find_among_b(a_3);
			if (num2 != 0)
			{
				bra = cursor;
				if (r_R1())
				{
					switch (num2)
					{
					case 1:
						slice_del();
						break;
					case 2:
						if (in_grouping_b(g_st_ending, 98, 116, repeat: false) == 0)
						{
							int num5 = cursor - 3;
							if (limit_backward <= num5 && num5 <= limit)
							{
								cursor = num5;
								slice_del();
							}
						}
						break;
					}
				}
			}
			cursor = limit - num4;
			int num6 = limit - cursor;
			ket = cursor;
			num2 = find_among_b(a_5);
			if (num2 != 0)
			{
				bra = cursor;
				if (r_R2())
				{
					switch (num2)
					{
					case 1:
					{
						slice_del();
						int num8 = limit - cursor;
						ket = cursor;
						if (!eq_s_b("ig"))
						{
							cursor = limit - num8;
							break;
						}
						bra = cursor;
						int num9 = limit - cursor;
						if (eq_s_b("e"))
						{
							cursor = limit - num8;
							break;
						}
						cursor = limit - num9;
						if (!r_R2())
						{
							cursor = limit - num8;
						}
						else
						{
							slice_del();
						}
						break;
					}
					case 2:
					{
						int num12 = limit - cursor;
						if (!eq_s_b("e"))
						{
							cursor = limit - num12;
							slice_del();
						}
						break;
					}
					case 3:
					{
						slice_del();
						int num10 = limit - cursor;
						ket = cursor;
						int num11 = limit - cursor;
						if (!eq_s_b("er"))
						{
							cursor = limit - num11;
							if (!eq_s_b("en"))
							{
								cursor = limit - num10;
								break;
							}
						}
						bra = cursor;
						if (!r_R1())
						{
							cursor = limit - num10;
						}
						else
						{
							slice_del();
						}
						break;
					}
					case 4:
					{
						slice_del();
						int num7 = limit - cursor;
						ket = cursor;
						if (find_among_b(a_4) == 0)
						{
							cursor = limit - num7;
							break;
						}
						bra = cursor;
						if (!r_R2())
						{
							cursor = limit - num7;
						}
						else
						{
							slice_del();
						}
						break;
					}
					}
				}
			}
			cursor = limit - num6;
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
					StemmerFactory.Register("de", () => new Stemmer_de(), -127002293);
				}
				_registered = true;
			}
		}
	}
}
