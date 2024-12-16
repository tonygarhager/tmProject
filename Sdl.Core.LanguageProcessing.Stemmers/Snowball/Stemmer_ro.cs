using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_ro : Stemmer
	{
		private bool B_standard_suffix_removed;

		private int I_p2;

		private int I_p1;

		private int I_pV;

		private static string g_v = "aeiouâîă";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private readonly Among[] a_5;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_ro()
		{
			a_0 = new Among[3]
			{
				new Among("", -1, 3),
				new Among("I", 0, 1),
				new Among("U", 0, 2)
			};
			a_1 = new Among[16]
			{
				new Among("ea", -1, 3),
				new Among("aţia", -1, 7),
				new Among("aua", -1, 2),
				new Among("iua", -1, 4),
				new Among("aţie", -1, 7),
				new Among("ele", -1, 3),
				new Among("ile", -1, 5),
				new Among("iile", 6, 4),
				new Among("iei", -1, 4),
				new Among("atei", -1, 6),
				new Among("ii", -1, 4),
				new Among("ului", -1, 1),
				new Among("ul", -1, 1),
				new Among("elor", -1, 3),
				new Among("ilor", -1, 4),
				new Among("iilor", 14, 4)
			};
			a_2 = new Among[46]
			{
				new Among("icala", -1, 4),
				new Among("iciva", -1, 4),
				new Among("ativa", -1, 5),
				new Among("itiva", -1, 6),
				new Among("icale", -1, 4),
				new Among("aţiune", -1, 5),
				new Among("iţiune", -1, 6),
				new Among("atoare", -1, 5),
				new Among("itoare", -1, 6),
				new Among("ătoare", -1, 5),
				new Among("icitate", -1, 4),
				new Among("abilitate", -1, 1),
				new Among("ibilitate", -1, 2),
				new Among("ivitate", -1, 3),
				new Among("icive", -1, 4),
				new Among("ative", -1, 5),
				new Among("itive", -1, 6),
				new Among("icali", -1, 4),
				new Among("atori", -1, 5),
				new Among("icatori", 18, 4),
				new Among("itori", -1, 6),
				new Among("ători", -1, 5),
				new Among("icitati", -1, 4),
				new Among("abilitati", -1, 1),
				new Among("ivitati", -1, 3),
				new Among("icivi", -1, 4),
				new Among("ativi", -1, 5),
				new Among("itivi", -1, 6),
				new Among("icităi", -1, 4),
				new Among("abilităi", -1, 1),
				new Among("ivităi", -1, 3),
				new Among("icităţi", -1, 4),
				new Among("abilităţi", -1, 1),
				new Among("ivităţi", -1, 3),
				new Among("ical", -1, 4),
				new Among("ator", -1, 5),
				new Among("icator", 35, 4),
				new Among("itor", -1, 6),
				new Among("ător", -1, 5),
				new Among("iciv", -1, 4),
				new Among("ativ", -1, 5),
				new Among("itiv", -1, 6),
				new Among("icală", -1, 4),
				new Among("icivă", -1, 4),
				new Among("ativă", -1, 5),
				new Among("itivă", -1, 6)
			};
			a_3 = new Among[62]
			{
				new Among("ica", -1, 1),
				new Among("abila", -1, 1),
				new Among("ibila", -1, 1),
				new Among("oasa", -1, 1),
				new Among("ata", -1, 1),
				new Among("ita", -1, 1),
				new Among("anta", -1, 1),
				new Among("ista", -1, 3),
				new Among("uta", -1, 1),
				new Among("iva", -1, 1),
				new Among("ic", -1, 1),
				new Among("ice", -1, 1),
				new Among("abile", -1, 1),
				new Among("ibile", -1, 1),
				new Among("isme", -1, 3),
				new Among("iune", -1, 2),
				new Among("oase", -1, 1),
				new Among("ate", -1, 1),
				new Among("itate", 17, 1),
				new Among("ite", -1, 1),
				new Among("ante", -1, 1),
				new Among("iste", -1, 3),
				new Among("ute", -1, 1),
				new Among("ive", -1, 1),
				new Among("ici", -1, 1),
				new Among("abili", -1, 1),
				new Among("ibili", -1, 1),
				new Among("iuni", -1, 2),
				new Among("atori", -1, 1),
				new Among("osi", -1, 1),
				new Among("ati", -1, 1),
				new Among("itati", 30, 1),
				new Among("iti", -1, 1),
				new Among("anti", -1, 1),
				new Among("isti", -1, 3),
				new Among("uti", -1, 1),
				new Among("işti", -1, 3),
				new Among("ivi", -1, 1),
				new Among("ităi", -1, 1),
				new Among("oşi", -1, 1),
				new Among("ităţi", -1, 1),
				new Among("abil", -1, 1),
				new Among("ibil", -1, 1),
				new Among("ism", -1, 3),
				new Among("ator", -1, 1),
				new Among("os", -1, 1),
				new Among("at", -1, 1),
				new Among("it", -1, 1),
				new Among("ant", -1, 1),
				new Among("ist", -1, 3),
				new Among("ut", -1, 1),
				new Among("iv", -1, 1),
				new Among("ică", -1, 1),
				new Among("abilă", -1, 1),
				new Among("ibilă", -1, 1),
				new Among("oasă", -1, 1),
				new Among("ată", -1, 1),
				new Among("ită", -1, 1),
				new Among("antă", -1, 1),
				new Among("istă", -1, 3),
				new Among("ută", -1, 1),
				new Among("ivă", -1, 1)
			};
			a_4 = new Among[94]
			{
				new Among("ea", -1, 1),
				new Among("ia", -1, 1),
				new Among("esc", -1, 1),
				new Among("ăsc", -1, 1),
				new Among("ind", -1, 1),
				new Among("ând", -1, 1),
				new Among("are", -1, 1),
				new Among("ere", -1, 1),
				new Among("ire", -1, 1),
				new Among("âre", -1, 1),
				new Among("se", -1, 2),
				new Among("ase", 10, 1),
				new Among("sese", 10, 2),
				new Among("ise", 10, 1),
				new Among("use", 10, 1),
				new Among("âse", 10, 1),
				new Among("eşte", -1, 1),
				new Among("ăşte", -1, 1),
				new Among("eze", -1, 1),
				new Among("ai", -1, 1),
				new Among("eai", 19, 1),
				new Among("iai", 19, 1),
				new Among("sei", -1, 2),
				new Among("eşti", -1, 1),
				new Among("ăşti", -1, 1),
				new Among("ui", -1, 1),
				new Among("ezi", -1, 1),
				new Among("âi", -1, 1),
				new Among("aşi", -1, 1),
				new Among("seşi", -1, 2),
				new Among("aseşi", 29, 1),
				new Among("seseşi", 29, 2),
				new Among("iseşi", 29, 1),
				new Among("useşi", 29, 1),
				new Among("âseşi", 29, 1),
				new Among("işi", -1, 1),
				new Among("uşi", -1, 1),
				new Among("âşi", -1, 1),
				new Among("aţi", -1, 2),
				new Among("eaţi", 38, 1),
				new Among("iaţi", 38, 1),
				new Among("eţi", -1, 2),
				new Among("iţi", -1, 2),
				new Among("âţi", -1, 2),
				new Among("arăţi", -1, 1),
				new Among("serăţi", -1, 2),
				new Among("aserăţi", 45, 1),
				new Among("seserăţi", 45, 2),
				new Among("iserăţi", 45, 1),
				new Among("userăţi", 45, 1),
				new Among("âserăţi", 45, 1),
				new Among("irăţi", -1, 1),
				new Among("urăţi", -1, 1),
				new Among("ârăţi", -1, 1),
				new Among("am", -1, 1),
				new Among("eam", 54, 1),
				new Among("iam", 54, 1),
				new Among("em", -1, 2),
				new Among("asem", 57, 1),
				new Among("sesem", 57, 2),
				new Among("isem", 57, 1),
				new Among("usem", 57, 1),
				new Among("âsem", 57, 1),
				new Among("im", -1, 2),
				new Among("âm", -1, 2),
				new Among("ăm", -1, 2),
				new Among("arăm", 65, 1),
				new Among("serăm", 65, 2),
				new Among("aserăm", 67, 1),
				new Among("seserăm", 67, 2),
				new Among("iserăm", 67, 1),
				new Among("userăm", 67, 1),
				new Among("âserăm", 67, 1),
				new Among("irăm", 65, 1),
				new Among("urăm", 65, 1),
				new Among("ârăm", 65, 1),
				new Among("au", -1, 1),
				new Among("eau", 76, 1),
				new Among("iau", 76, 1),
				new Among("indu", -1, 1),
				new Among("ându", -1, 1),
				new Among("ez", -1, 1),
				new Among("ească", -1, 1),
				new Among("ară", -1, 1),
				new Among("seră", -1, 2),
				new Among("aseră", 84, 1),
				new Among("seseră", 84, 2),
				new Among("iseră", 84, 1),
				new Among("useră", 84, 1),
				new Among("âseră", 84, 1),
				new Among("iră", -1, 1),
				new Among("ură", -1, 1),
				new Among("âră", -1, 1),
				new Among("ează", -1, 1)
			};
			a_5 = new Among[5]
			{
				new Among("a", -1, 1),
				new Among("e", -1, 1),
				new Among("ie", 1, 1),
				new Among("i", -1, 1),
				new Among("ă", -1, 1)
			};
		}

		private bool r_prelude()
		{
			while (true)
			{
				int cursor = base.cursor;
				int cursor2;
				while (true)
				{
					cursor2 = base.cursor;
					if (in_grouping(g_v, 97, 259, repeat: false) == 0)
					{
						bra = base.cursor;
						int cursor3 = base.cursor;
						if (eq_s("u"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 259, repeat: false) == 0)
							{
								slice_from("U");
								break;
							}
						}
						base.cursor = cursor3;
						if (eq_s("i"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 259, repeat: false) == 0)
							{
								slice_from("I");
								break;
							}
						}
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
				base.cursor = cursor2;
			}
		}

		private bool r_mark_regions()
		{
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int cursor2 = base.cursor;
			if (in_grouping(g_v, 97, 259, repeat: false) == 0)
			{
				int cursor3 = base.cursor;
				if (out_grouping(g_v, 97, 259, repeat: false) == 0)
				{
					int num = out_grouping(g_v, 97, 259, repeat: true);
					if (num >= 0)
					{
						base.cursor += num;
						goto IL_017b;
					}
				}
				base.cursor = cursor3;
				if (in_grouping(g_v, 97, 259, repeat: false) == 0)
				{
					int num2 = in_grouping(g_v, 97, 259, repeat: true);
					if (num2 >= 0)
					{
						base.cursor += num2;
						goto IL_017b;
					}
				}
			}
			base.cursor = cursor2;
			if (out_grouping(g_v, 97, 259, repeat: false) == 0)
			{
				int cursor4 = base.cursor;
				if (out_grouping(g_v, 97, 259, repeat: false) == 0)
				{
					int num3 = out_grouping(g_v, 97, 259, repeat: true);
					if (num3 >= 0)
					{
						base.cursor += num3;
						goto IL_017b;
					}
				}
				base.cursor = cursor4;
				if (in_grouping(g_v, 97, 259, repeat: false) == 0 && base.cursor < limit)
				{
					base.cursor++;
					goto IL_017b;
				}
			}
			goto IL_0187;
			IL_0187:
			base.cursor = cursor;
			int cursor5 = base.cursor;
			int num4 = out_grouping(g_v, 97, 259, repeat: true);
			if (num4 >= 0)
			{
				base.cursor += num4;
				int num5 = in_grouping(g_v, 97, 259, repeat: true);
				if (num5 >= 0)
				{
					base.cursor += num5;
					I_p1 = base.cursor;
					int num6 = out_grouping(g_v, 97, 259, repeat: true);
					if (num6 >= 0)
					{
						base.cursor += num6;
						int num7 = in_grouping(g_v, 97, 259, repeat: true);
						if (num7 >= 0)
						{
							base.cursor += num7;
							I_p2 = base.cursor;
						}
					}
				}
			}
			base.cursor = cursor5;
			return true;
			IL_017b:
			I_pV = base.cursor;
			goto IL_0187;
		}

		private bool r_postlude()
		{
			int cursor;
			while (true)
			{
				cursor = base.cursor;
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
					slice_from("i");
					continue;
				case 2:
					slice_from("u");
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

		private bool r_step_0()
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
				slice_del();
				break;
			case 2:
				slice_from("a");
				break;
			case 3:
				slice_from("e");
				break;
			case 4:
				slice_from("i");
				break;
			case 5:
			{
				int num2 = limit - cursor;
				if (eq_s_b("ab"))
				{
					return false;
				}
				cursor = limit - num2;
				slice_from("i");
				break;
			}
			case 6:
				slice_from("at");
				break;
			case 7:
				slice_from("aţi");
				break;
			}
			return true;
		}

		private bool r_combo_suffix()
		{
			int num = limit - cursor;
			ket = cursor;
			int num2 = find_among_b(a_2);
			if (num2 == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R1())
			{
				return false;
			}
			switch (num2)
			{
			case 1:
				slice_from("abil");
				break;
			case 2:
				slice_from("ibil");
				break;
			case 3:
				slice_from("iv");
				break;
			case 4:
				slice_from("ic");
				break;
			case 5:
				slice_from("at");
				break;
			case 6:
				slice_from("it");
				break;
			}
			B_standard_suffix_removed = true;
			cursor = limit - num;
			return true;
		}

		private bool r_standard_suffix()
		{
			B_standard_suffix_removed = false;
			int num;
			do
			{
				num = limit - cursor;
			}
			while (r_combo_suffix());
			cursor = limit - num;
			ket = cursor;
			int num2 = find_among_b(a_3);
			if (num2 == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R2())
			{
				return false;
			}
			switch (num2)
			{
			case 1:
				slice_del();
				break;
			case 2:
				if (!eq_s_b("ţ"))
				{
					return false;
				}
				bra = cursor;
				slice_from("t");
				break;
			case 3:
				slice_from("ist");
				break;
			}
			B_standard_suffix_removed = true;
			return true;
		}

		private bool r_verb_suffix()
		{
			if (cursor < I_pV)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_pV;
			ket = cursor;
			int num = find_among_b(a_4);
			if (num == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
			{
				int num2 = limit - cursor;
				if (out_grouping_b(g_v, 97, 259, repeat: false) != 0)
				{
					cursor = limit - num2;
					if (!eq_s_b("u"))
					{
						base.limit_backward = limit_backward;
						return false;
					}
				}
				slice_del();
				break;
			}
			case 2:
				slice_del();
				break;
			}
			base.limit_backward = limit_backward;
			return true;
		}

		private bool r_vowel_suffix()
		{
			ket = cursor;
			if (find_among_b(a_5) == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_RV())
			{
				return false;
			}
			slice_del();
			return true;
		}

		protected override bool stem()
		{
			int cursor = base.cursor;
			r_prelude();
			base.cursor = cursor;
			r_mark_regions();
			limit_backward = base.cursor;
			base.cursor = limit;
			int num = limit - base.cursor;
			r_step_0();
			base.cursor = limit - num;
			int num2 = limit - base.cursor;
			r_standard_suffix();
			base.cursor = limit - num2;
			int num3 = limit - base.cursor;
			int num4 = limit - base.cursor;
			if (!B_standard_suffix_removed)
			{
				base.cursor = limit - num4;
				r_verb_suffix();
			}
			base.cursor = limit - num3;
			int num5 = limit - base.cursor;
			r_vowel_suffix();
			base.cursor = limit - num5;
			base.cursor = limit_backward;
			int cursor2 = base.cursor;
			r_postlude();
			base.cursor = cursor2;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("ro", () => new Stemmer_ro(), 1195973253);
				}
				_registered = true;
			}
		}
	}
}
