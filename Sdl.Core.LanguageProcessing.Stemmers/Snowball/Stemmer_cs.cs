using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_cs : Stemmer
	{
		private int I_p1;

		private int I_pV;

		private static string g_v = "aeiouyáìéíóúùý";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private readonly Among[] a_5;

		private readonly Among[] a_6;

		private readonly Among[] a_7;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_cs()
		{
			a_0 = new Among[14]
			{
				new Among("ce", -1, 1),
				new Among("ze", -1, 2),
				new Among("¾e", -1, 2),
				new Among("ci", -1, 1),
				new Among("¹ti", -1, 4),
				new Among("èti", -1, 3),
				new Among("zi", -1, 2),
				new Among("¾i", -1, 2),
				new Among("èi", -1, 1),
				new Among("è", -1, 1),
				new Among("¹té", -1, 4),
				new Among("èté", -1, 3),
				new Among("¹tì", -1, 4),
				new Among("ètì", -1, 3)
			};
			a_1 = new Among[3]
			{
				new Among("in", -1, 2),
				new Among("ov", -1, 1),
				new Among("ùv", -1, 1)
			};
			a_2 = new Among[48]
			{
				new Among("a", -1, 1),
				new Among("ama", 0, 1),
				new Among("ata", 0, 1),
				new Among("e", -1, 2),
				new Among("ìte", 3, 2),
				new Among("ech", -1, 2),
				new Among("atech", 5, 1),
				new Among("ich", -1, 2),
				new Among("ách", -1, 1),
				new Among("ích", -1, 2),
				new Among("ých", -1, 1),
				new Among("i", -1, 2),
				new Among("mi", 11, 1),
				new Among("ami", 12, 1),
				new Among("emi", 12, 2),
				new Among("ìmi", 12, 2),
				new Among("ími", 12, 2),
				new Among("ými", 12, 1),
				new Among("ìti", 11, 2),
				new Among("ovi", 11, 1),
				new Among("em", -1, 3),
				new Among("ìtem", 20, 1),
				new Among("ám", -1, 1),
				new Among("ém", -1, 2),
				new Among("ím", -1, 2),
				new Among("atùm", -1, 1),
				new Among("ým", -1, 1),
				new Among("o", -1, 1),
				new Among("iho", 27, 2),
				new Among("ého", 27, 2),
				new Among("ího", 27, 2),
				new Among("es", -1, 2),
				new Among("os", -1, 1),
				new Among("us", -1, 1),
				new Among("at", -1, 1),
				new Among("u", -1, 1),
				new Among("imu", 35, 2),
				new Among("ému", 35, 2),
				new Among("ou", 35, 1),
				new Among("y", -1, 1),
				new Among("aty", 39, 1),
				new Among("á", -1, 1),
				new Among("é", -1, 1),
				new Among("ové", 42, 1),
				new Among("ì", -1, 2),
				new Among("í", -1, 2),
				new Among("ù", -1, 1),
				new Among("ý", -1, 1)
			};
			a_3 = new Among[68]
			{
				new Among("ob", -1, 1),
				new Among("itb", -1, 2),
				new Among("ec", -1, 3),
				new Among("inec", 2, 2),
				new Among("obinec", 3, 1),
				new Among("ovec", 2, 1),
				new Among("ic", -1, 2),
				new Among("enic", 6, 3),
				new Among("och", -1, 1),
				new Among("ásek", -1, 1),
				new Among("nk", -1, 1),
				new Among("isk", -1, 2),
				new Among("ovisk", 11, 1),
				new Among("tk", -1, 1),
				new Among("vk", -1, 1),
				new Among("i¹k", -1, 2),
				new Among("u¹k", -1, 1),
				new Among("èk", -1, 1),
				new Among("ník", -1, 1),
				new Among("ovník", 18, 1),
				new Among("ovík", -1, 1),
				new Among("dl", -1, 1),
				new Among("itel", -1, 2),
				new Among("ul", -1, 1),
				new Among("an", -1, 1),
				new Among("èan", 24, 1),
				new Among("en", -1, 3),
				new Among("in", -1, 2),
				new Among("¹tin", 27, 1),
				new Among("ovin", 27, 1),
				new Among("teln", -1, 1),
				new Among("árn", -1, 1),
				new Among("írn", -1, 6),
				new Among("oun", -1, 1),
				new Among("loun", 33, 1),
				new Among("ovn", -1, 1),
				new Among("yn", -1, 1),
				new Among("kyn", 36, 1),
				new Among("án", -1, 1),
				new Among("ián", 38, 2),
				new Among("èn", -1, 1),
				new Among("ìn", -1, 5),
				new Among("ín", -1, 6),
				new Among("as", -1, 1),
				new Among("it", -1, 2),
				new Among("ot", -1, 1),
				new Among("ist", -1, 2),
				new Among("ost", -1, 1),
				new Among("nost", 47, 1),
				new Among("out", -1, 1),
				new Among("ovi¹t", -1, 1),
				new Among("iv", -1, 2),
				new Among("ov", -1, 1),
				new Among("tv", -1, 1),
				new Among("ctv", 53, 1),
				new Among("stv", 53, 1),
				new Among("ovstv", 55, 1),
				new Among("ovtv", 53, 1),
				new Among("ou¹", -1, 1),
				new Among("aè", -1, 1),
				new Among("áè", -1, 1),
				new Among("oò", -1, 1),
				new Among("áø", -1, 1),
				new Among("káø", 62, 1),
				new Among("ionáø", 62, 2),
				new Among("éø", -1, 4),
				new Among("néø", 65, 1),
				new Among("íø", -1, 6)
			};
			a_4 = new Among[6]
			{
				new Among("c", -1, 1),
				new Among("k", -1, 1),
				new Among("l", -1, 1),
				new Among("n", -1, 1),
				new Among("t", -1, 1),
				new Among("è", -1, 1)
			};
			a_5 = new Among[4]
			{
				new Among("isk", -1, 2),
				new Among("ák", -1, 1),
				new Among("izn", -1, 2),
				new Among("ajzn", -1, 1)
			};
			a_6 = new Among[42]
			{
				new Among("k", -1, 1),
				new Among("ak", 0, 7),
				new Among("ek", 0, 2),
				new Among("anek", 2, 1),
				new Among("enek", 2, 2),
				new Among("inek", 2, 4),
				new Among("onek", 2, 1),
				new Among("unek", 2, 1),
				new Among("ánek", 2, 1),
				new Among("ou¹ek", 2, 1),
				new Among("aèek", 2, 1),
				new Among("eèek", 2, 2),
				new Among("ièek", 2, 4),
				new Among("oèek", 2, 1),
				new Among("uèek", 2, 1),
				new Among("áèek", 2, 1),
				new Among("éèek", 2, 3),
				new Among("íèek", 2, 5),
				new Among("ik", 0, 4),
				new Among("ank", 0, 1),
				new Among("enk", 0, 1),
				new Among("ink", 0, 1),
				new Among("onk", 0, 1),
				new Among("unk", 0, 1),
				new Among("ánk", 0, 1),
				new Among("énk", 0, 1),
				new Among("ínk", 0, 1),
				new Among("ok", 0, 8),
				new Among("átk", 0, 1),
				new Among("uk", 0, 9),
				new Among("u¹k", 0, 1),
				new Among("ák", 0, 6),
				new Among("aèk", 0, 1),
				new Among("eèk", 0, 1),
				new Among("ièk", 0, 1),
				new Among("oèk", 0, 1),
				new Among("uèk", 0, 1),
				new Among("áèk", 0, 1),
				new Among("éèk", 0, 1),
				new Among("íèk", 0, 1),
				new Among("ék", 0, 3),
				new Among("ík", 0, 5)
			};
			a_7 = new Among[2]
			{
				new Among("ej¹", -1, 2),
				new Among("ìj¹", -1, 1)
			};
		}

		private bool r_mark_regions()
		{
			I_pV = limit;
			I_p1 = limit;
			int cursor = base.cursor;
			int num = in_grouping(g_v, 97, 253, repeat: true);
			if (num >= 0)
			{
				base.cursor += num;
				I_pV = base.cursor;
				int num2 = in_grouping(g_v, 97, 253, repeat: true);
				if (num2 >= 0)
				{
					base.cursor += num2;
					int num3 = out_grouping(g_v, 97, 253, repeat: true);
					if (num3 >= 0)
					{
						base.cursor += num3;
						I_p1 = base.cursor;
					}
				}
			}
			base.cursor = cursor;
			return true;
		}

		private bool r_RV()
		{
			return I_pV <= cursor;
		}

		private bool r_R1()
		{
			if (I_p1 > cursor)
			{
				return false;
			}
			return true;
		}

		private bool r_palatalise()
		{
			ket = cursor;
			int num = find_among_b(a_0);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_RV())
			{
				return false;
			}
			switch (num)
			{
			case 1:
				slice_from("k");
				break;
			case 2:
				slice_from("h");
				break;
			case 3:
				slice_from("ck");
				break;
			case 4:
				slice_from("sk");
				break;
			}
			return true;
		}

		private bool r_do_possessive()
		{
			ket = cursor;
			int num = find_among_b(a_1);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_RV())
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
				slice_del();
				int num2 = limit - cursor;
				if (!r_palatalise())
				{
					cursor = limit - num2;
				}
				break;
			}
			}
			return true;
		}

		private bool r_do_case()
		{
			ket = cursor;
			int num = find_among_b(a_2);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
			{
				slice_del();
				int num3 = limit - cursor;
				if (!r_palatalise())
				{
					cursor = limit - num3;
				}
				break;
			}
			case 3:
			{
				slice_from("e");
				int num2 = limit - cursor;
				if (!r_palatalise())
				{
					cursor = limit - num2;
				}
				break;
			}
			}
			return true;
		}

		private bool r_do_derivational()
		{
			ket = cursor;
			int num = find_among_b(a_3);
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
				slice_from("i");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 3:
				slice_from("e");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 4:
				slice_from("é");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 5:
				slice_from("ì");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 6:
				slice_from("í");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			}
			return true;
		}

		private bool r_do_deriv_single()
		{
			ket = cursor;
			if (find_among_b(a_4) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_do_augmentative()
		{
			ket = cursor;
			int num = find_among_b(a_5);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("i");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			}
			return true;
		}

		private bool r_do_diminutive()
		{
			ket = cursor;
			int num = find_among_b(a_6);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("e");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 3:
				slice_from("é");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 4:
				slice_from("i");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 5:
				slice_from("í");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 6:
				slice_from("á");
				break;
			case 7:
				slice_from("a");
				break;
			case 8:
				slice_from("o");
				break;
			case 9:
				slice_from("u");
				break;
			}
			return true;
		}

		private bool r_do_comparative()
		{
			ket = cursor;
			int num = find_among_b(a_7);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				slice_from("ì");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			case 2:
				slice_from("e");
				if (!r_palatalise())
				{
					return false;
				}
				break;
			}
			return true;
		}

		private bool r_do_aggressive()
		{
			int num = limit - cursor;
			r_do_comparative();
			cursor = limit - num;
			int num2 = limit - cursor;
			r_do_diminutive();
			cursor = limit - num2;
			int num3 = limit - cursor;
			r_do_augmentative();
			cursor = limit - num3;
			int num4 = limit - cursor;
			if (!r_do_derivational())
			{
				cursor = limit - num4;
				if (!r_do_deriv_single())
				{
					return false;
				}
			}
			return true;
		}

		protected override bool stem()
		{
			r_mark_regions();
			limit_backward = cursor;
			cursor = limit;
			if (!r_do_case())
			{
				return false;
			}
			if (!r_do_possessive())
			{
				return false;
			}
			if (!r_do_aggressive())
			{
				return false;
			}
			cursor = limit_backward;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("cs", () => new Stemmer_cs(), 233537255);
				}
				_registered = true;
			}
		}
	}
}
