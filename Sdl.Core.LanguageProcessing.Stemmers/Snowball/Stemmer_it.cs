using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_it : Stemmer
	{
		private int I_p2;

		private int I_p1;

		private int I_pV;

		private static string g_v = "aeiouàèìòù";

		private static string g_AEIO = "aeioàèìò";

		private static string g_CG = "cg";

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

		public Stemmer_it()
		{
			a_0 = new Among[7]
			{
				new Among("", -1, 7),
				new Among("qu", 0, 6),
				new Among("á", 0, 1),
				new Among("é", 0, 2),
				new Among("í", 0, 3),
				new Among("ó", 0, 4),
				new Among("ú", 0, 5)
			};
			a_1 = new Among[3]
			{
				new Among("", -1, 3),
				new Among("I", 0, 1),
				new Among("U", 0, 2)
			};
			a_2 = new Among[37]
			{
				new Among("la", -1, -1),
				new Among("cela", 0, -1),
				new Among("gliela", 0, -1),
				new Among("mela", 0, -1),
				new Among("tela", 0, -1),
				new Among("vela", 0, -1),
				new Among("le", -1, -1),
				new Among("cele", 6, -1),
				new Among("gliele", 6, -1),
				new Among("mele", 6, -1),
				new Among("tele", 6, -1),
				new Among("vele", 6, -1),
				new Among("ne", -1, -1),
				new Among("cene", 12, -1),
				new Among("gliene", 12, -1),
				new Among("mene", 12, -1),
				new Among("sene", 12, -1),
				new Among("tene", 12, -1),
				new Among("vene", 12, -1),
				new Among("ci", -1, -1),
				new Among("li", -1, -1),
				new Among("celi", 20, -1),
				new Among("glieli", 20, -1),
				new Among("meli", 20, -1),
				new Among("teli", 20, -1),
				new Among("veli", 20, -1),
				new Among("gli", 20, -1),
				new Among("mi", -1, -1),
				new Among("si", -1, -1),
				new Among("ti", -1, -1),
				new Among("vi", -1, -1),
				new Among("lo", -1, -1),
				new Among("celo", 31, -1),
				new Among("glielo", 31, -1),
				new Among("melo", 31, -1),
				new Among("telo", 31, -1),
				new Among("velo", 31, -1)
			};
			a_3 = new Among[5]
			{
				new Among("ando", -1, 1),
				new Among("endo", -1, 1),
				new Among("ar", -1, 2),
				new Among("er", -1, 2),
				new Among("ir", -1, 2)
			};
			a_4 = new Among[4]
			{
				new Among("ic", -1, -1),
				new Among("abil", -1, -1),
				new Among("os", -1, -1),
				new Among("iv", -1, 1)
			};
			a_5 = new Among[3]
			{
				new Among("ic", -1, 1),
				new Among("abil", -1, 1),
				new Among("iv", -1, 1)
			};
			a_6 = new Among[51]
			{
				new Among("ica", -1, 1),
				new Among("logia", -1, 3),
				new Among("osa", -1, 1),
				new Among("ista", -1, 1),
				new Among("iva", -1, 9),
				new Among("anza", -1, 1),
				new Among("enza", -1, 5),
				new Among("ice", -1, 1),
				new Among("atrice", 7, 1),
				new Among("iche", -1, 1),
				new Among("logie", -1, 3),
				new Among("abile", -1, 1),
				new Among("ibile", -1, 1),
				new Among("usione", -1, 4),
				new Among("azione", -1, 2),
				new Among("uzione", -1, 4),
				new Among("atore", -1, 2),
				new Among("ose", -1, 1),
				new Among("ante", -1, 1),
				new Among("mente", -1, 1),
				new Among("amente", 19, 7),
				new Among("iste", -1, 1),
				new Among("ive", -1, 9),
				new Among("anze", -1, 1),
				new Among("enze", -1, 5),
				new Among("ici", -1, 1),
				new Among("atrici", 25, 1),
				new Among("ichi", -1, 1),
				new Among("abili", -1, 1),
				new Among("ibili", -1, 1),
				new Among("ismi", -1, 1),
				new Among("usioni", -1, 4),
				new Among("azioni", -1, 2),
				new Among("uzioni", -1, 4),
				new Among("atori", -1, 2),
				new Among("osi", -1, 1),
				new Among("anti", -1, 1),
				new Among("amenti", -1, 6),
				new Among("imenti", -1, 6),
				new Among("isti", -1, 1),
				new Among("ivi", -1, 9),
				new Among("ico", -1, 1),
				new Among("ismo", -1, 1),
				new Among("oso", -1, 1),
				new Among("amento", -1, 6),
				new Among("imento", -1, 6),
				new Among("ivo", -1, 9),
				new Among("ità", -1, 8),
				new Among("istà", -1, 1),
				new Among("istè", -1, 1),
				new Among("istì", -1, 1)
			};
			a_7 = new Among[87]
			{
				new Among("isca", -1, 1),
				new Among("enda", -1, 1),
				new Among("ata", -1, 1),
				new Among("ita", -1, 1),
				new Among("uta", -1, 1),
				new Among("ava", -1, 1),
				new Among("eva", -1, 1),
				new Among("iva", -1, 1),
				new Among("erebbe", -1, 1),
				new Among("irebbe", -1, 1),
				new Among("isce", -1, 1),
				new Among("ende", -1, 1),
				new Among("are", -1, 1),
				new Among("ere", -1, 1),
				new Among("ire", -1, 1),
				new Among("asse", -1, 1),
				new Among("ate", -1, 1),
				new Among("avate", 16, 1),
				new Among("evate", 16, 1),
				new Among("ivate", 16, 1),
				new Among("ete", -1, 1),
				new Among("erete", 20, 1),
				new Among("irete", 20, 1),
				new Among("ite", -1, 1),
				new Among("ereste", -1, 1),
				new Among("ireste", -1, 1),
				new Among("ute", -1, 1),
				new Among("erai", -1, 1),
				new Among("irai", -1, 1),
				new Among("isci", -1, 1),
				new Among("endi", -1, 1),
				new Among("erei", -1, 1),
				new Among("irei", -1, 1),
				new Among("assi", -1, 1),
				new Among("ati", -1, 1),
				new Among("iti", -1, 1),
				new Among("eresti", -1, 1),
				new Among("iresti", -1, 1),
				new Among("uti", -1, 1),
				new Among("avi", -1, 1),
				new Among("evi", -1, 1),
				new Among("ivi", -1, 1),
				new Among("isco", -1, 1),
				new Among("ando", -1, 1),
				new Among("endo", -1, 1),
				new Among("Yamo", -1, 1),
				new Among("iamo", -1, 1),
				new Among("avamo", -1, 1),
				new Among("evamo", -1, 1),
				new Among("ivamo", -1, 1),
				new Among("eremo", -1, 1),
				new Among("iremo", -1, 1),
				new Among("assimo", -1, 1),
				new Among("ammo", -1, 1),
				new Among("emmo", -1, 1),
				new Among("eremmo", 54, 1),
				new Among("iremmo", 54, 1),
				new Among("immo", -1, 1),
				new Among("ano", -1, 1),
				new Among("iscano", 58, 1),
				new Among("avano", 58, 1),
				new Among("evano", 58, 1),
				new Among("ivano", 58, 1),
				new Among("eranno", -1, 1),
				new Among("iranno", -1, 1),
				new Among("ono", -1, 1),
				new Among("iscono", 65, 1),
				new Among("arono", 65, 1),
				new Among("erono", 65, 1),
				new Among("irono", 65, 1),
				new Among("erebbero", -1, 1),
				new Among("irebbero", -1, 1),
				new Among("assero", -1, 1),
				new Among("essero", -1, 1),
				new Among("issero", -1, 1),
				new Among("ato", -1, 1),
				new Among("ito", -1, 1),
				new Among("uto", -1, 1),
				new Among("avo", -1, 1),
				new Among("evo", -1, 1),
				new Among("ivo", -1, 1),
				new Among("ar", -1, 1),
				new Among("ir", -1, 1),
				new Among("erà", -1, 1),
				new Among("irà", -1, 1),
				new Among("erò", -1, 1),
				new Among("irò", -1, 1)
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
					slice_from("à");
					continue;
				case 2:
					slice_from("è");
					continue;
				case 3:
					slice_from("ì");
					continue;
				case 4:
					slice_from("ò");
					continue;
				case 5:
					slice_from("ù");
					continue;
				case 6:
					slice_from("qU");
					continue;
				case 7:
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
			while (true)
			{
				int cursor3 = base.cursor;
				int cursor4;
				while (true)
				{
					cursor4 = base.cursor;
					if (in_grouping(g_v, 97, 249, repeat: false) == 0)
					{
						bra = base.cursor;
						int cursor5 = base.cursor;
						if (eq_s("u"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 249, repeat: false) == 0)
							{
								slice_from("U");
								break;
							}
						}
						base.cursor = cursor5;
						if (eq_s("i"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 249, repeat: false) == 0)
							{
								slice_from("I");
								break;
							}
						}
					}
					base.cursor = cursor4;
					if (base.cursor < limit)
					{
						base.cursor++;
						continue;
					}
					base.cursor = cursor3;
					return true;
				}
				base.cursor = cursor4;
			}
		}

		private bool r_mark_regions()
		{
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int cursor2 = base.cursor;
			if (in_grouping(g_v, 97, 249, repeat: false) == 0)
			{
				int cursor3 = base.cursor;
				if (out_grouping(g_v, 97, 249, repeat: false) == 0)
				{
					int num = out_grouping(g_v, 97, 249, repeat: true);
					if (num >= 0)
					{
						base.cursor += num;
						goto IL_017b;
					}
				}
				base.cursor = cursor3;
				if (in_grouping(g_v, 97, 249, repeat: false) == 0)
				{
					int num2 = in_grouping(g_v, 97, 249, repeat: true);
					if (num2 >= 0)
					{
						base.cursor += num2;
						goto IL_017b;
					}
				}
			}
			base.cursor = cursor2;
			if (out_grouping(g_v, 97, 249, repeat: false) == 0)
			{
				int cursor4 = base.cursor;
				if (out_grouping(g_v, 97, 249, repeat: false) == 0)
				{
					int num3 = out_grouping(g_v, 97, 249, repeat: true);
					if (num3 >= 0)
					{
						base.cursor += num3;
						goto IL_017b;
					}
				}
				base.cursor = cursor4;
				if (in_grouping(g_v, 97, 249, repeat: false) == 0 && base.cursor < limit)
				{
					base.cursor++;
					goto IL_017b;
				}
			}
			goto IL_0187;
			IL_0187:
			base.cursor = cursor;
			int cursor5 = base.cursor;
			int num4 = out_grouping(g_v, 97, 249, repeat: true);
			if (num4 >= 0)
			{
				base.cursor += num4;
				int num5 = in_grouping(g_v, 97, 249, repeat: true);
				if (num5 >= 0)
				{
					base.cursor += num5;
					I_p1 = base.cursor;
					int num6 = out_grouping(g_v, 97, 249, repeat: true);
					if (num6 >= 0)
					{
						base.cursor += num6;
						int num7 = in_grouping(g_v, 97, 249, repeat: true);
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
			if (I_p2 > cursor)
			{
				return false;
			}
			return true;
		}

		private bool r_attached_pronoun()
		{
			ket = cursor;
			if (find_among_b(a_2) == 0)
			{
				return false;
			}
			bra = cursor;
			int num = find_among_b(a_3);
			if (num == 0)
			{
				return false;
			}
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
				slice_from("e");
				break;
			}
			return true;
		}

		private bool r_standard_suffix()
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
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				break;
			case 2:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num5 = limit - cursor;
				ket = cursor;
				if (!eq_s_b("ic"))
				{
					cursor = limit - num5;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num5;
				}
				else
				{
					slice_del();
				}
				break;
			}
			case 3:
				if (!r_R2())
				{
					return false;
				}
				slice_from("log");
				break;
			case 4:
				if (!r_R2())
				{
					return false;
				}
				slice_from("u");
				break;
			case 5:
				if (!r_R2())
				{
					return false;
				}
				slice_from("ente");
				break;
			case 6:
				if (!r_RV())
				{
					return false;
				}
				slice_del();
				break;
			case 7:
			{
				if (!r_R1())
				{
					return false;
				}
				slice_del();
				int num3 = limit - cursor;
				ket = cursor;
				num = find_among_b(a_4);
				if (num == 0)
				{
					cursor = limit - num3;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num3;
					break;
				}
				slice_del();
				if (num != 1)
				{
					break;
				}
				ket = cursor;
				if (!eq_s_b("at"))
				{
					cursor = limit - num3;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num3;
				}
				else
				{
					slice_del();
				}
				break;
			}
			case 8:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num4 = limit - cursor;
				ket = cursor;
				if (find_among_b(a_5) == 0)
				{
					cursor = limit - num4;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num4;
				}
				else
				{
					slice_del();
				}
				break;
			}
			case 9:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num2 = limit - cursor;
				ket = cursor;
				if (!eq_s_b("at"))
				{
					cursor = limit - num2;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num2;
					break;
				}
				slice_del();
				ket = cursor;
				if (!eq_s_b("ic"))
				{
					cursor = limit - num2;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num2;
				}
				else
				{
					slice_del();
				}
				break;
			}
			}
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
			if (find_among_b(a_7) == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			slice_del();
			base.limit_backward = limit_backward;
			return true;
		}

		private bool r_vowel_suffix()
		{
			int num = limit - cursor;
			ket = cursor;
			if (in_grouping_b(g_AEIO, 97, 242, repeat: false) != 0)
			{
				cursor = limit - num;
			}
			else
			{
				bra = cursor;
				if (!r_RV())
				{
					cursor = limit - num;
				}
				else
				{
					slice_del();
					ket = cursor;
					if (!eq_s_b("i"))
					{
						cursor = limit - num;
					}
					else
					{
						bra = cursor;
						if (!r_RV())
						{
							cursor = limit - num;
						}
						else
						{
							slice_del();
						}
					}
				}
			}
			int num2 = limit - cursor;
			ket = cursor;
			if (!eq_s_b("h"))
			{
				cursor = limit - num2;
			}
			else
			{
				bra = cursor;
				if (in_grouping_b(g_CG, 99, 103, repeat: false) != 0)
				{
					cursor = limit - num2;
				}
				else if (!r_RV())
				{
					cursor = limit - num2;
				}
				else
				{
					slice_del();
				}
			}
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
			r_attached_pronoun();
			base.cursor = limit - num;
			int num2 = limit - base.cursor;
			int num3 = limit - base.cursor;
			if (!r_standard_suffix())
			{
				base.cursor = limit - num3;
				r_verb_suffix();
			}
			base.cursor = limit - num2;
			int num4 = limit - base.cursor;
			r_vowel_suffix();
			base.cursor = limit - num4;
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
					StemmerFactory.Register("it", () => new Stemmer_it(), -815420804);
				}
				_registered = true;
			}
		}
	}
}
