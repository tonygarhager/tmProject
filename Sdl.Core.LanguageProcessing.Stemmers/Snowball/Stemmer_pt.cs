using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_pt : Stemmer
	{
		private int I_p2;

		private int I_p1;

		private int I_pV;

		private static string g_v = "aeiouáéíóúâêô";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private readonly Among[] a_5;

		private readonly Among[] a_6;

		private readonly Among[] a_7;

		private readonly Among[] a_8;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_pt()
		{
			a_0 = new Among[3]
			{
				new Among("", -1, 3),
				new Among("ã", 0, 1),
				new Among("õ", 0, 2)
			};
			a_1 = new Among[3]
			{
				new Among("", -1, 3),
				new Among("a~", 0, 1),
				new Among("o~", 0, 2)
			};
			a_2 = new Among[4]
			{
				new Among("ic", -1, -1),
				new Among("ad", -1, -1),
				new Among("os", -1, -1),
				new Among("iv", -1, 1)
			};
			a_3 = new Among[3]
			{
				new Among("ante", -1, 1),
				new Among("avel", -1, 1),
				new Among("ível", -1, 1)
			};
			a_4 = new Among[3]
			{
				new Among("ic", -1, 1),
				new Among("abil", -1, 1),
				new Among("iv", -1, 1)
			};
			a_5 = new Among[45]
			{
				new Among("ica", -1, 1),
				new Among("ância", -1, 1),
				new Among("ência", -1, 4),
				new Among("logia", -1, 2),
				new Among("ira", -1, 9),
				new Among("adora", -1, 1),
				new Among("osa", -1, 1),
				new Among("ista", -1, 1),
				new Among("iva", -1, 8),
				new Among("eza", -1, 1),
				new Among("idade", -1, 7),
				new Among("ante", -1, 1),
				new Among("mente", -1, 6),
				new Among("amente", 12, 5),
				new Among("ável", -1, 1),
				new Among("ível", -1, 1),
				new Among("ico", -1, 1),
				new Among("ismo", -1, 1),
				new Among("oso", -1, 1),
				new Among("amento", -1, 1),
				new Among("imento", -1, 1),
				new Among("ivo", -1, 8),
				new Among("aça~o", -1, 1),
				new Among("uça~o", -1, 3),
				new Among("ador", -1, 1),
				new Among("icas", -1, 1),
				new Among("ências", -1, 4),
				new Among("logias", -1, 2),
				new Among("iras", -1, 9),
				new Among("adoras", -1, 1),
				new Among("osas", -1, 1),
				new Among("istas", -1, 1),
				new Among("ivas", -1, 8),
				new Among("ezas", -1, 1),
				new Among("idades", -1, 7),
				new Among("adores", -1, 1),
				new Among("antes", -1, 1),
				new Among("aço~es", -1, 1),
				new Among("uço~es", -1, 3),
				new Among("icos", -1, 1),
				new Among("ismos", -1, 1),
				new Among("osos", -1, 1),
				new Among("amentos", -1, 1),
				new Among("imentos", -1, 1),
				new Among("ivos", -1, 8)
			};
			a_6 = new Among[120]
			{
				new Among("ada", -1, 1),
				new Among("ida", -1, 1),
				new Among("ia", -1, 1),
				new Among("aria", 2, 1),
				new Among("eria", 2, 1),
				new Among("iria", 2, 1),
				new Among("ara", -1, 1),
				new Among("era", -1, 1),
				new Among("ira", -1, 1),
				new Among("ava", -1, 1),
				new Among("asse", -1, 1),
				new Among("esse", -1, 1),
				new Among("isse", -1, 1),
				new Among("aste", -1, 1),
				new Among("este", -1, 1),
				new Among("iste", -1, 1),
				new Among("ei", -1, 1),
				new Among("arei", 16, 1),
				new Among("erei", 16, 1),
				new Among("irei", 16, 1),
				new Among("am", -1, 1),
				new Among("iam", 20, 1),
				new Among("ariam", 21, 1),
				new Among("eriam", 21, 1),
				new Among("iriam", 21, 1),
				new Among("aram", 20, 1),
				new Among("eram", 20, 1),
				new Among("iram", 20, 1),
				new Among("avam", 20, 1),
				new Among("em", -1, 1),
				new Among("arem", 29, 1),
				new Among("erem", 29, 1),
				new Among("irem", 29, 1),
				new Among("assem", 29, 1),
				new Among("essem", 29, 1),
				new Among("issem", 29, 1),
				new Among("ado", -1, 1),
				new Among("ido", -1, 1),
				new Among("ando", -1, 1),
				new Among("endo", -1, 1),
				new Among("indo", -1, 1),
				new Among("ara~o", -1, 1),
				new Among("era~o", -1, 1),
				new Among("ira~o", -1, 1),
				new Among("ar", -1, 1),
				new Among("er", -1, 1),
				new Among("ir", -1, 1),
				new Among("as", -1, 1),
				new Among("adas", 47, 1),
				new Among("idas", 47, 1),
				new Among("ias", 47, 1),
				new Among("arias", 50, 1),
				new Among("erias", 50, 1),
				new Among("irias", 50, 1),
				new Among("aras", 47, 1),
				new Among("eras", 47, 1),
				new Among("iras", 47, 1),
				new Among("avas", 47, 1),
				new Among("es", -1, 1),
				new Among("ardes", 58, 1),
				new Among("erdes", 58, 1),
				new Among("irdes", 58, 1),
				new Among("ares", 58, 1),
				new Among("eres", 58, 1),
				new Among("ires", 58, 1),
				new Among("asses", 58, 1),
				new Among("esses", 58, 1),
				new Among("isses", 58, 1),
				new Among("astes", 58, 1),
				new Among("estes", 58, 1),
				new Among("istes", 58, 1),
				new Among("is", -1, 1),
				new Among("ais", 71, 1),
				new Among("eis", 71, 1),
				new Among("areis", 73, 1),
				new Among("ereis", 73, 1),
				new Among("ireis", 73, 1),
				new Among("áreis", 73, 1),
				new Among("éreis", 73, 1),
				new Among("íreis", 73, 1),
				new Among("ásseis", 73, 1),
				new Among("ésseis", 73, 1),
				new Among("ísseis", 73, 1),
				new Among("áveis", 73, 1),
				new Among("íeis", 73, 1),
				new Among("aríeis", 84, 1),
				new Among("eríeis", 84, 1),
				new Among("iríeis", 84, 1),
				new Among("ados", -1, 1),
				new Among("idos", -1, 1),
				new Among("amos", -1, 1),
				new Among("áramos", 90, 1),
				new Among("éramos", 90, 1),
				new Among("íramos", 90, 1),
				new Among("ávamos", 90, 1),
				new Among("íamos", 90, 1),
				new Among("aríamos", 95, 1),
				new Among("eríamos", 95, 1),
				new Among("iríamos", 95, 1),
				new Among("emos", -1, 1),
				new Among("aremos", 99, 1),
				new Among("eremos", 99, 1),
				new Among("iremos", 99, 1),
				new Among("ássemos", 99, 1),
				new Among("êssemos", 99, 1),
				new Among("íssemos", 99, 1),
				new Among("imos", -1, 1),
				new Among("armos", -1, 1),
				new Among("ermos", -1, 1),
				new Among("irmos", -1, 1),
				new Among("ámos", -1, 1),
				new Among("arás", -1, 1),
				new Among("erás", -1, 1),
				new Among("irás", -1, 1),
				new Among("eu", -1, 1),
				new Among("iu", -1, 1),
				new Among("ou", -1, 1),
				new Among("ará", -1, 1),
				new Among("erá", -1, 1),
				new Among("irá", -1, 1)
			};
			a_7 = new Among[7]
			{
				new Among("a", -1, 1),
				new Among("i", -1, 1),
				new Among("o", -1, 1),
				new Among("os", -1, 1),
				new Among("á", -1, 1),
				new Among("í", -1, 1),
				new Among("ó", -1, 1)
			};
			a_8 = new Among[4]
			{
				new Among("e", -1, 1),
				new Among("ç", -1, 2),
				new Among("é", -1, 1),
				new Among("ê", -1, 1)
			};
		}

		private bool r_prelude()
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
					slice_from("a~");
					continue;
				case 2:
					slice_from("o~");
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

		private bool r_mark_regions()
		{
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int cursor2 = base.cursor;
			if (in_grouping(g_v, 97, 250, repeat: false) == 0)
			{
				int cursor3 = base.cursor;
				if (out_grouping(g_v, 97, 250, repeat: false) == 0)
				{
					int num = out_grouping(g_v, 97, 250, repeat: true);
					if (num >= 0)
					{
						base.cursor += num;
						goto IL_017b;
					}
				}
				base.cursor = cursor3;
				if (in_grouping(g_v, 97, 250, repeat: false) == 0)
				{
					int num2 = in_grouping(g_v, 97, 250, repeat: true);
					if (num2 >= 0)
					{
						base.cursor += num2;
						goto IL_017b;
					}
				}
			}
			base.cursor = cursor2;
			if (out_grouping(g_v, 97, 250, repeat: false) == 0)
			{
				int cursor4 = base.cursor;
				if (out_grouping(g_v, 97, 250, repeat: false) == 0)
				{
					int num3 = out_grouping(g_v, 97, 250, repeat: true);
					if (num3 >= 0)
					{
						base.cursor += num3;
						goto IL_017b;
					}
				}
				base.cursor = cursor4;
				if (in_grouping(g_v, 97, 250, repeat: false) == 0 && base.cursor < limit)
				{
					base.cursor++;
					goto IL_017b;
				}
			}
			goto IL_0187;
			IL_0187:
			base.cursor = cursor;
			int cursor5 = base.cursor;
			int num4 = out_grouping(g_v, 97, 250, repeat: true);
			if (num4 >= 0)
			{
				base.cursor += num4;
				int num5 = in_grouping(g_v, 97, 250, repeat: true);
				if (num5 >= 0)
				{
					base.cursor += num5;
					I_p1 = base.cursor;
					int num6 = out_grouping(g_v, 97, 250, repeat: true);
					if (num6 >= 0)
					{
						base.cursor += num6;
						int num7 = in_grouping(g_v, 97, 250, repeat: true);
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
					slice_from("ã");
					continue;
				case 2:
					slice_from("õ");
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

		private bool r_standard_suffix()
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
				if (!r_R2())
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
				slice_from("log");
				break;
			case 3:
				if (!r_R2())
				{
					return false;
				}
				slice_from("u");
				break;
			case 4:
				if (!r_R2())
				{
					return false;
				}
				slice_from("ente");
				break;
			case 5:
			{
				if (!r_R1())
				{
					return false;
				}
				slice_del();
				int num5 = limit - cursor;
				ket = cursor;
				num = find_among_b(a_2);
				if (num == 0)
				{
					cursor = limit - num5;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num5;
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
			case 6:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num4 = limit - cursor;
				ket = cursor;
				if (find_among_b(a_3) == 0)
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
			case 7:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num2 = limit - cursor;
				ket = cursor;
				if (find_among_b(a_4) == 0)
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
			case 8:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num3 = limit - cursor;
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
			case 9:
				if (!r_RV())
				{
					return false;
				}
				if (!eq_s_b("e"))
				{
					return false;
				}
				slice_from("ir");
				break;
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
			if (find_among_b(a_6) == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			slice_del();
			base.limit_backward = limit_backward;
			return true;
		}

		private bool r_residual_suffix()
		{
			ket = cursor;
			if (find_among_b(a_7) == 0)
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

		private bool r_residual_form()
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
				if (!r_RV())
				{
					return false;
				}
				slice_del();
				ket = cursor;
				int num2 = limit - cursor;
				if (eq_s_b("u"))
				{
					bra = cursor;
					int num3 = limit - cursor;
					if (eq_s_b("g"))
					{
						cursor = limit - num3;
						goto IL_00fc;
					}
				}
				cursor = limit - num2;
				if (!eq_s_b("i"))
				{
					return false;
				}
				bra = cursor;
				int num4 = limit - cursor;
				if (!eq_s_b("c"))
				{
					return false;
				}
				cursor = limit - num4;
				goto IL_00fc;
			}
			case 2:
				{
					slice_from("c");
					break;
				}
				IL_00fc:
				if (!r_RV())
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
			r_prelude();
			base.cursor = cursor;
			r_mark_regions();
			limit_backward = base.cursor;
			base.cursor = limit;
			int num = limit - base.cursor;
			int num2 = limit - base.cursor;
			int num3 = limit - base.cursor;
			int num4 = limit - base.cursor;
			if (!r_standard_suffix())
			{
				base.cursor = limit - num4;
				if (!r_verb_suffix())
				{
					base.cursor = limit - num2;
					r_residual_suffix();
					goto IL_0130;
				}
			}
			base.cursor = limit - num3;
			int num5 = limit - base.cursor;
			ket = base.cursor;
			if (eq_s_b("i"))
			{
				bra = base.cursor;
				int num6 = limit - base.cursor;
				if (eq_s_b("c"))
				{
					base.cursor = limit - num6;
					if (r_RV())
					{
						slice_del();
					}
				}
			}
			base.cursor = limit - num5;
			goto IL_0130;
			IL_0130:
			base.cursor = limit - num;
			int num7 = limit - base.cursor;
			r_residual_form();
			base.cursor = limit - num7;
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
					StemmerFactory.Register("pt", () => new Stemmer_pt(), 1287610548);
				}
				_registered = true;
			}
		}
	}
}
