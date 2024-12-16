using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_ga : Stemmer
	{
		private int I_p2;

		private int I_p1;

		private int I_pV;

		private static string g_v = "aeiouáéíóú";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_ga()
		{
			a_0 = new Among[24]
			{
				new Among("b'", -1, 1),
				new Among("bh", -1, 4),
				new Among("bhf", 1, 2),
				new Among("bp", -1, 8),
				new Among("ch", -1, 5),
				new Among("d'", -1, 1),
				new Among("d'fh", 5, 2),
				new Among("dh", -1, 6),
				new Among("dt", -1, 9),
				new Among("fh", -1, 2),
				new Among("gc", -1, 5),
				new Among("gh", -1, 7),
				new Among("h-", -1, 1),
				new Among("m'", -1, 1),
				new Among("mb", -1, 4),
				new Among("mh", -1, 10),
				new Among("n-", -1, 1),
				new Among("nd", -1, 6),
				new Among("ng", -1, 7),
				new Among("ph", -1, 8),
				new Among("sh", -1, 3),
				new Among("t-", -1, 1),
				new Among("th", -1, 9),
				new Among("ts", -1, 3)
			};
			a_1 = new Among[16]
			{
				new Among("íochta", -1, 1),
				new Among("aíochta", 0, 1),
				new Among("ire", -1, 2),
				new Among("aire", 2, 2),
				new Among("abh", -1, 1),
				new Among("eabh", 4, 1),
				new Among("ibh", -1, 1),
				new Among("aibh", 6, 1),
				new Among("amh", -1, 1),
				new Among("eamh", 8, 1),
				new Among("imh", -1, 1),
				new Among("aimh", 10, 1),
				new Among("íocht", -1, 1),
				new Among("aíocht", 12, 1),
				new Among("irí", -1, 2),
				new Among("airí", 14, 2)
			};
			a_2 = new Among[25]
			{
				new Among("óideacha", -1, 6),
				new Among("patacha", -1, 5),
				new Among("achta", -1, 1),
				new Among("arcachta", 2, 2),
				new Among("eachta", 2, 1),
				new Among("grafaíochta", -1, 4),
				new Among("paite", -1, 5),
				new Among("ach", -1, 1),
				new Among("each", 7, 1),
				new Among("óideach", 8, 6),
				new Among("gineach", 8, 3),
				new Among("patach", 7, 5),
				new Among("grafaíoch", -1, 4),
				new Among("pataigh", -1, 5),
				new Among("óidigh", -1, 6),
				new Among("achtúil", -1, 1),
				new Among("eachtúil", 15, 1),
				new Among("gineas", -1, 3),
				new Among("ginis", -1, 3),
				new Among("acht", -1, 1),
				new Among("arcacht", 19, 2),
				new Among("eacht", 19, 1),
				new Among("grafaíocht", -1, 4),
				new Among("arcachtaí", -1, 2),
				new Among("grafaíochtaí", -1, 4)
			};
			a_3 = new Among[12]
			{
				new Among("imid", -1, 1),
				new Among("aimid", 0, 1),
				new Among("ímid", -1, 1),
				new Among("aímid", 2, 1),
				new Among("adh", -1, 2),
				new Among("eadh", 4, 2),
				new Among("faidh", -1, 1),
				new Among("fidh", -1, 1),
				new Among("áil", -1, 2),
				new Among("ain", -1, 2),
				new Among("tear", -1, 2),
				new Among("tar", -1, 2)
			};
		}

		private bool r_mark_regions()
		{
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int num = out_grouping(g_v, 97, 250, repeat: true);
			if (num >= 0)
			{
				base.cursor += num;
				I_pV = base.cursor;
			}
			base.cursor = cursor;
			int cursor2 = base.cursor;
			int num2 = out_grouping(g_v, 97, 250, repeat: true);
			if (num2 >= 0)
			{
				base.cursor += num2;
				int num3 = in_grouping(g_v, 97, 250, repeat: true);
				if (num3 >= 0)
				{
					base.cursor += num3;
					I_p1 = base.cursor;
					int num4 = out_grouping(g_v, 97, 250, repeat: true);
					if (num4 >= 0)
					{
						base.cursor += num4;
						int num5 = in_grouping(g_v, 97, 250, repeat: true);
						if (num5 >= 0)
						{
							base.cursor += num5;
							I_p2 = base.cursor;
						}
					}
				}
			}
			base.cursor = cursor2;
			return true;
		}

		private bool r_initial_morph()
		{
			bra = cursor;
			int num = find_among(a_0);
			if (num == 0)
			{
				return false;
			}
			ket = cursor;
			switch (num)
			{
			case 1:
				slice_del();
				break;
			case 2:
				slice_from("f");
				break;
			case 3:
				slice_from("s");
				break;
			case 4:
				slice_from("b");
				break;
			case 5:
				slice_from("c");
				break;
			case 6:
				slice_from("d");
				break;
			case 7:
				slice_from("g");
				break;
			case 8:
				slice_from("p");
				break;
			case 9:
				slice_from("t");
				break;
			case 10:
				slice_from("m");
				break;
			}
			return true;
		}

		private bool r_RV()
		{
			return I_pV <= cursor;
		}

		private bool r_R1()
		{
			return I_p1 <= cursor;
		}

		private bool r_R2()
		{
			return I_p2 <= cursor;
		}

		private bool r_noun_sfx()
		{
			ket = cursor;
			int num = find_among_b(a_1);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				if (!r_R1())
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_deriv()
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
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				slice_from("arc");
				break;
			case 3:
				slice_from("gin");
				break;
			case 4:
				slice_from("graf");
				break;
			case 5:
				slice_from("paite");
				break;
			case 6:
				slice_from("óid");
				break;
			}
			return true;
		}

		private bool r_verb_sfx()
		{
			ket = cursor;
			int num = find_among_b(a_3);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				if (!r_RV())
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (!r_R1())
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		protected override bool stem()
		{
			int cursor = base.cursor;
			r_initial_morph();
			base.cursor = cursor;
			r_mark_regions();
			limit_backward = base.cursor;
			base.cursor = limit;
			int num = limit - base.cursor;
			r_noun_sfx();
			base.cursor = limit - num;
			int num2 = limit - base.cursor;
			r_deriv();
			base.cursor = limit - num2;
			int num3 = limit - base.cursor;
			r_verb_sfx();
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
					StemmerFactory.Register("ga", () => new Stemmer_ga(), 1735016034);
				}
				_registered = true;
			}
		}
	}
}
