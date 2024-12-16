using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_es : Stemmer
	{
		private int I_p2;

		private int I_p1;

		private int I_pV;

		private static string g_v = "aeiouáéíóúü";

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

		public Stemmer_es()
		{
			a_0 = new Among[6]
			{
				new Among("", -1, 6),
				new Among("á", 0, 1),
				new Among("é", 0, 2),
				new Among("í", 0, 3),
				new Among("ó", 0, 4),
				new Among("ú", 0, 5)
			};
			a_1 = new Among[13]
			{
				new Among("la", -1, -1),
				new Among("sela", 0, -1),
				new Among("le", -1, -1),
				new Among("me", -1, -1),
				new Among("se", -1, -1),
				new Among("lo", -1, -1),
				new Among("selo", 5, -1),
				new Among("las", -1, -1),
				new Among("selas", 7, -1),
				new Among("les", -1, -1),
				new Among("los", -1, -1),
				new Among("selos", 10, -1),
				new Among("nos", -1, -1)
			};
			a_2 = new Among[11]
			{
				new Among("ando", -1, 6),
				new Among("iendo", -1, 6),
				new Among("yendo", -1, 7),
				new Among("ándo", -1, 2),
				new Among("iéndo", -1, 1),
				new Among("ar", -1, 6),
				new Among("er", -1, 6),
				new Among("ir", -1, 6),
				new Among("ár", -1, 3),
				new Among("ér", -1, 4),
				new Among("ír", -1, 5)
			};
			a_3 = new Among[4]
			{
				new Among("ic", -1, -1),
				new Among("ad", -1, -1),
				new Among("os", -1, -1),
				new Among("iv", -1, 1)
			};
			a_4 = new Among[3]
			{
				new Among("able", -1, 1),
				new Among("ible", -1, 1),
				new Among("ante", -1, 1)
			};
			a_5 = new Among[3]
			{
				new Among("ic", -1, 1),
				new Among("abil", -1, 1),
				new Among("iv", -1, 1)
			};
			a_6 = new Among[46]
			{
				new Among("ica", -1, 1),
				new Among("ancia", -1, 2),
				new Among("encia", -1, 5),
				new Among("adora", -1, 2),
				new Among("osa", -1, 1),
				new Among("ista", -1, 1),
				new Among("iva", -1, 9),
				new Among("anza", -1, 1),
				new Among("logía", -1, 3),
				new Among("idad", -1, 8),
				new Among("able", -1, 1),
				new Among("ible", -1, 1),
				new Among("ante", -1, 2),
				new Among("mente", -1, 7),
				new Among("amente", 13, 6),
				new Among("ación", -1, 2),
				new Among("ución", -1, 4),
				new Among("ico", -1, 1),
				new Among("ismo", -1, 1),
				new Among("oso", -1, 1),
				new Among("amiento", -1, 1),
				new Among("imiento", -1, 1),
				new Among("ivo", -1, 9),
				new Among("ador", -1, 2),
				new Among("icas", -1, 1),
				new Among("ancias", -1, 2),
				new Among("encias", -1, 5),
				new Among("adoras", -1, 2),
				new Among("osas", -1, 1),
				new Among("istas", -1, 1),
				new Among("ivas", -1, 9),
				new Among("anzas", -1, 1),
				new Among("logías", -1, 3),
				new Among("idades", -1, 8),
				new Among("ables", -1, 1),
				new Among("ibles", -1, 1),
				new Among("aciones", -1, 2),
				new Among("uciones", -1, 4),
				new Among("adores", -1, 2),
				new Among("antes", -1, 2),
				new Among("icos", -1, 1),
				new Among("ismos", -1, 1),
				new Among("osos", -1, 1),
				new Among("amientos", -1, 1),
				new Among("imientos", -1, 1),
				new Among("ivos", -1, 9)
			};
			a_7 = new Among[12]
			{
				new Among("ya", -1, 1),
				new Among("ye", -1, 1),
				new Among("yan", -1, 1),
				new Among("yen", -1, 1),
				new Among("yeron", -1, 1),
				new Among("yendo", -1, 1),
				new Among("yo", -1, 1),
				new Among("yas", -1, 1),
				new Among("yes", -1, 1),
				new Among("yais", -1, 1),
				new Among("yamos", -1, 1),
				new Among("yó", -1, 1)
			};
			a_8 = new Among[96]
			{
				new Among("aba", -1, 2),
				new Among("ada", -1, 2),
				new Among("ida", -1, 2),
				new Among("ara", -1, 2),
				new Among("iera", -1, 2),
				new Among("ía", -1, 2),
				new Among("aría", 5, 2),
				new Among("ería", 5, 2),
				new Among("iría", 5, 2),
				new Among("ad", -1, 2),
				new Among("ed", -1, 2),
				new Among("id", -1, 2),
				new Among("ase", -1, 2),
				new Among("iese", -1, 2),
				new Among("aste", -1, 2),
				new Among("iste", -1, 2),
				new Among("an", -1, 2),
				new Among("aban", 16, 2),
				new Among("aran", 16, 2),
				new Among("ieran", 16, 2),
				new Among("ían", 16, 2),
				new Among("arían", 20, 2),
				new Among("erían", 20, 2),
				new Among("irían", 20, 2),
				new Among("en", -1, 1),
				new Among("asen", 24, 2),
				new Among("iesen", 24, 2),
				new Among("aron", -1, 2),
				new Among("ieron", -1, 2),
				new Among("arán", -1, 2),
				new Among("erán", -1, 2),
				new Among("irán", -1, 2),
				new Among("ado", -1, 2),
				new Among("ido", -1, 2),
				new Among("ando", -1, 2),
				new Among("iendo", -1, 2),
				new Among("ar", -1, 2),
				new Among("er", -1, 2),
				new Among("ir", -1, 2),
				new Among("as", -1, 2),
				new Among("abas", 39, 2),
				new Among("adas", 39, 2),
				new Among("idas", 39, 2),
				new Among("aras", 39, 2),
				new Among("ieras", 39, 2),
				new Among("ías", 39, 2),
				new Among("arías", 45, 2),
				new Among("erías", 45, 2),
				new Among("irías", 45, 2),
				new Among("es", -1, 1),
				new Among("ases", 49, 2),
				new Among("ieses", 49, 2),
				new Among("abais", -1, 2),
				new Among("arais", -1, 2),
				new Among("ierais", -1, 2),
				new Among("íais", -1, 2),
				new Among("aríais", 55, 2),
				new Among("eríais", 55, 2),
				new Among("iríais", 55, 2),
				new Among("aseis", -1, 2),
				new Among("ieseis", -1, 2),
				new Among("asteis", -1, 2),
				new Among("isteis", -1, 2),
				new Among("áis", -1, 2),
				new Among("éis", -1, 1),
				new Among("aréis", 64, 2),
				new Among("eréis", 64, 2),
				new Among("iréis", 64, 2),
				new Among("ados", -1, 2),
				new Among("idos", -1, 2),
				new Among("amos", -1, 2),
				new Among("ábamos", 70, 2),
				new Among("áramos", 70, 2),
				new Among("iéramos", 70, 2),
				new Among("íamos", 70, 2),
				new Among("aríamos", 74, 2),
				new Among("eríamos", 74, 2),
				new Among("iríamos", 74, 2),
				new Among("emos", -1, 1),
				new Among("aremos", 78, 2),
				new Among("eremos", 78, 2),
				new Among("iremos", 78, 2),
				new Among("ásemos", 78, 2),
				new Among("iésemos", 78, 2),
				new Among("imos", -1, 2),
				new Among("arás", -1, 2),
				new Among("erás", -1, 2),
				new Among("irás", -1, 2),
				new Among("ís", -1, 2),
				new Among("ará", -1, 2),
				new Among("erá", -1, 2),
				new Among("irá", -1, 2),
				new Among("aré", -1, 2),
				new Among("eré", -1, 2),
				new Among("iré", -1, 2),
				new Among("ió", -1, 2)
			};
			a_9 = new Among[8]
			{
				new Among("a", -1, 1),
				new Among("e", -1, 2),
				new Among("o", -1, 1),
				new Among("os", -1, 1),
				new Among("á", -1, 1),
				new Among("é", -1, 2),
				new Among("í", -1, 1),
				new Among("ó", -1, 1)
			};
		}

		private bool r_mark_regions()
		{
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int cursor2 = base.cursor;
			if (in_grouping(g_v, 97, 252, repeat: false) == 0)
			{
				int cursor3 = base.cursor;
				if (out_grouping(g_v, 97, 252, repeat: false) == 0)
				{
					int num = out_grouping(g_v, 97, 252, repeat: true);
					if (num >= 0)
					{
						base.cursor += num;
						goto IL_017b;
					}
				}
				base.cursor = cursor3;
				if (in_grouping(g_v, 97, 252, repeat: false) == 0)
				{
					int num2 = in_grouping(g_v, 97, 252, repeat: true);
					if (num2 >= 0)
					{
						base.cursor += num2;
						goto IL_017b;
					}
				}
			}
			base.cursor = cursor2;
			if (out_grouping(g_v, 97, 252, repeat: false) == 0)
			{
				int cursor4 = base.cursor;
				if (out_grouping(g_v, 97, 252, repeat: false) == 0)
				{
					int num3 = out_grouping(g_v, 97, 252, repeat: true);
					if (num3 >= 0)
					{
						base.cursor += num3;
						goto IL_017b;
					}
				}
				base.cursor = cursor4;
				if (in_grouping(g_v, 97, 252, repeat: false) == 0 && base.cursor < limit)
				{
					base.cursor++;
					goto IL_017b;
				}
			}
			goto IL_0187;
			IL_0187:
			base.cursor = cursor;
			int cursor5 = base.cursor;
			int num4 = out_grouping(g_v, 97, 252, repeat: true);
			if (num4 >= 0)
			{
				base.cursor += num4;
				int num5 = in_grouping(g_v, 97, 252, repeat: true);
				if (num5 >= 0)
				{
					base.cursor += num5;
					I_p1 = base.cursor;
					int num6 = out_grouping(g_v, 97, 252, repeat: true);
					if (num6 >= 0)
					{
						base.cursor += num6;
						int num7 = in_grouping(g_v, 97, 252, repeat: true);
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

		private bool r_attached_pronoun()
		{
			ket = cursor;
			if (find_among_b(a_1) == 0)
			{
				return false;
			}
			bra = cursor;
			int num = find_among_b(a_2);
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
				bra = cursor;
				slice_from("iendo");
				break;
			case 2:
				bra = cursor;
				slice_from("ando");
				break;
			case 3:
				bra = cursor;
				slice_from("ar");
				break;
			case 4:
				bra = cursor;
				slice_from("er");
				break;
			case 5:
				bra = cursor;
				slice_from("ir");
				break;
			case 6:
				slice_del();
				break;
			case 7:
				if (!eq_s_b("u"))
				{
					return false;
				}
				slice_del();
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
				int num3 = limit - cursor;
				ket = cursor;
				if (!eq_s_b("ic"))
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
			{
				if (!r_R1())
				{
					return false;
				}
				slice_del();
				int num6 = limit - cursor;
				ket = cursor;
				num = find_among_b(a_3);
				if (num == 0)
				{
					cursor = limit - num6;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num6;
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
					cursor = limit - num6;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num6;
				}
				else
				{
					slice_del();
				}
				break;
			}
			case 7:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num4 = limit - cursor;
				ket = cursor;
				if (find_among_b(a_4) == 0)
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
			case 8:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num5 = limit - cursor;
				ket = cursor;
				if (find_among_b(a_5) == 0)
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

		private bool r_y_verb_suffix()
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
			base.limit_backward = limit_backward;
			if (!eq_s_b("u"))
			{
				return false;
			}
			slice_del();
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
			int num = find_among_b(a_8);
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
				if (!eq_s_b("u"))
				{
					cursor = limit - num2;
				}
				else
				{
					int num3 = limit - cursor;
					if (!eq_s_b("g"))
					{
						cursor = limit - num2;
					}
					else
					{
						cursor = limit - num3;
					}
				}
				bra = cursor;
				slice_del();
				break;
			}
			case 2:
				slice_del();
				break;
			}
			return true;
		}

		private bool r_residual_suffix()
		{
			ket = cursor;
			int num = find_among_b(a_9);
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
			{
				if (!r_RV())
				{
					return false;
				}
				slice_del();
				int num2 = limit - cursor;
				ket = cursor;
				if (!eq_s_b("u"))
				{
					cursor = limit - num2;
					break;
				}
				bra = cursor;
				int num3 = limit - cursor;
				if (!eq_s_b("g"))
				{
					cursor = limit - num2;
					break;
				}
				cursor = limit - num3;
				if (!r_RV())
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

		protected override bool stem()
		{
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
				if (!r_y_verb_suffix())
				{
					base.cursor = limit - num3;
					r_verb_suffix();
				}
			}
			base.cursor = limit - num2;
			int num4 = limit - base.cursor;
			r_residual_suffix();
			base.cursor = limit - num4;
			base.cursor = limit_backward;
			int cursor = base.cursor;
			r_postlude();
			base.cursor = cursor;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("es", () => new Stemmer_es(), -615906116);
				}
				_registered = true;
			}
		}
	}
}
