using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_fr : Stemmer
	{
		private int I_p2;

		private int I_p1;

		private int I_pV;

		private static string g_v = "aeiouyâàëéêèïîôûù";

		private static string g_keep_with_s = "aiouès";

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

		public Stemmer_fr()
		{
			a_0 = new Among[3]
			{
				new Among("col", -1, -1),
				new Among("par", -1, -1),
				new Among("tap", -1, -1)
			};
			a_1 = new Among[7]
			{
				new Among("", -1, 7),
				new Among("H", 0, 6),
				new Among("He", 1, 4),
				new Among("Hi", 1, 5),
				new Among("I", 0, 1),
				new Among("U", 0, 2),
				new Among("Y", 0, 3)
			};
			a_2 = new Among[6]
			{
				new Among("iqU", -1, 3),
				new Among("abl", -1, 3),
				new Among("Ièr", -1, 4),
				new Among("ièr", -1, 4),
				new Among("eus", -1, 2),
				new Among("iv", -1, 1)
			};
			a_3 = new Among[3]
			{
				new Among("ic", -1, 2),
				new Among("abil", -1, 1),
				new Among("iv", -1, 3)
			};
			a_4 = new Among[43]
			{
				new Among("iqUe", -1, 1),
				new Among("atrice", -1, 2),
				new Among("ance", -1, 1),
				new Among("ence", -1, 5),
				new Among("logie", -1, 3),
				new Among("able", -1, 1),
				new Among("isme", -1, 1),
				new Among("euse", -1, 11),
				new Among("iste", -1, 1),
				new Among("ive", -1, 8),
				new Among("if", -1, 8),
				new Among("usion", -1, 4),
				new Among("ation", -1, 2),
				new Among("ution", -1, 4),
				new Among("ateur", -1, 2),
				new Among("iqUes", -1, 1),
				new Among("atrices", -1, 2),
				new Among("ances", -1, 1),
				new Among("ences", -1, 5),
				new Among("logies", -1, 3),
				new Among("ables", -1, 1),
				new Among("ismes", -1, 1),
				new Among("euses", -1, 11),
				new Among("istes", -1, 1),
				new Among("ives", -1, 8),
				new Among("ifs", -1, 8),
				new Among("usions", -1, 4),
				new Among("ations", -1, 2),
				new Among("utions", -1, 4),
				new Among("ateurs", -1, 2),
				new Among("ments", -1, 15),
				new Among("ements", 30, 6),
				new Among("issements", 31, 12),
				new Among("ités", -1, 7),
				new Among("ment", -1, 15),
				new Among("ement", 34, 6),
				new Among("issement", 35, 12),
				new Among("amment", 34, 13),
				new Among("emment", 34, 14),
				new Among("aux", -1, 10),
				new Among("eaux", 39, 9),
				new Among("eux", -1, 1),
				new Among("ité", -1, 7)
			};
			a_5 = new Among[35]
			{
				new Among("ira", -1, 1),
				new Among("ie", -1, 1),
				new Among("isse", -1, 1),
				new Among("issante", -1, 1),
				new Among("i", -1, 1),
				new Among("irai", 4, 1),
				new Among("ir", -1, 1),
				new Among("iras", -1, 1),
				new Among("ies", -1, 1),
				new Among("îmes", -1, 1),
				new Among("isses", -1, 1),
				new Among("issantes", -1, 1),
				new Among("îtes", -1, 1),
				new Among("is", -1, 1),
				new Among("irais", 13, 1),
				new Among("issais", 13, 1),
				new Among("irions", -1, 1),
				new Among("issions", -1, 1),
				new Among("irons", -1, 1),
				new Among("issons", -1, 1),
				new Among("issants", -1, 1),
				new Among("it", -1, 1),
				new Among("irait", 21, 1),
				new Among("issait", 21, 1),
				new Among("issant", -1, 1),
				new Among("iraIent", -1, 1),
				new Among("issaIent", -1, 1),
				new Among("irent", -1, 1),
				new Among("issent", -1, 1),
				new Among("iront", -1, 1),
				new Among("ît", -1, 1),
				new Among("iriez", -1, 1),
				new Among("issiez", -1, 1),
				new Among("irez", -1, 1),
				new Among("issez", -1, 1)
			};
			a_6 = new Among[38]
			{
				new Among("a", -1, 3),
				new Among("era", 0, 2),
				new Among("asse", -1, 3),
				new Among("ante", -1, 3),
				new Among("ée", -1, 2),
				new Among("ai", -1, 3),
				new Among("erai", 5, 2),
				new Among("er", -1, 2),
				new Among("as", -1, 3),
				new Among("eras", 8, 2),
				new Among("âmes", -1, 3),
				new Among("asses", -1, 3),
				new Among("antes", -1, 3),
				new Among("âtes", -1, 3),
				new Among("ées", -1, 2),
				new Among("ais", -1, 3),
				new Among("erais", 15, 2),
				new Among("ions", -1, 1),
				new Among("erions", 17, 2),
				new Among("assions", 17, 3),
				new Among("erons", -1, 2),
				new Among("ants", -1, 3),
				new Among("és", -1, 2),
				new Among("ait", -1, 3),
				new Among("erait", 23, 2),
				new Among("ant", -1, 3),
				new Among("aIent", -1, 3),
				new Among("eraIent", 26, 2),
				new Among("èrent", -1, 2),
				new Among("assent", -1, 3),
				new Among("eront", -1, 2),
				new Among("ât", -1, 3),
				new Among("ez", -1, 2),
				new Among("iez", 32, 2),
				new Among("eriez", 33, 2),
				new Among("assiez", 33, 3),
				new Among("erez", 32, 2),
				new Among("é", -1, 2)
			};
			a_7 = new Among[6]
			{
				new Among("e", -1, 3),
				new Among("Ière", 0, 2),
				new Among("ière", 0, 2),
				new Among("ion", -1, 1),
				new Among("Ier", -1, 2),
				new Among("ier", -1, 2)
			};
			a_8 = new Among[5]
			{
				new Among("ell", -1, -1),
				new Among("eill", -1, -1),
				new Among("enn", -1, -1),
				new Among("onn", -1, -1),
				new Among("ett", -1, -1)
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
					int cursor3 = base.cursor;
					if (in_grouping(g_v, 97, 251, repeat: false) == 0)
					{
						bra = base.cursor;
						int cursor4 = base.cursor;
						if (eq_s("u"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 251, repeat: false) == 0)
							{
								slice_from("U");
								break;
							}
						}
						base.cursor = cursor4;
						if (eq_s("i"))
						{
							ket = base.cursor;
							if (in_grouping(g_v, 97, 251, repeat: false) == 0)
							{
								slice_from("I");
								break;
							}
						}
						base.cursor = cursor4;
						if (eq_s("y"))
						{
							ket = base.cursor;
							slice_from("Y");
							break;
						}
					}
					base.cursor = cursor3;
					bra = base.cursor;
					if (eq_s("ë"))
					{
						ket = base.cursor;
						slice_from("He");
						break;
					}
					base.cursor = cursor3;
					bra = base.cursor;
					if (eq_s("ï"))
					{
						ket = base.cursor;
						slice_from("Hi");
						break;
					}
					base.cursor = cursor3;
					bra = base.cursor;
					if (eq_s("y"))
					{
						ket = base.cursor;
						if (in_grouping(g_v, 97, 251, repeat: false) == 0)
						{
							slice_from("Y");
							break;
						}
					}
					base.cursor = cursor3;
					if (eq_s("q"))
					{
						bra = base.cursor;
						if (eq_s("u"))
						{
							ket = base.cursor;
							slice_from("U");
							break;
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
			if (in_grouping(g_v, 97, 251, repeat: false) == 0 && in_grouping(g_v, 97, 251, repeat: false) == 0 && base.cursor < limit)
			{
				base.cursor++;
			}
			else
			{
				base.cursor = cursor2;
				if (find_among(a_0) == 0)
				{
					base.cursor = cursor2;
					if (base.cursor < limit)
					{
						base.cursor++;
						int num = out_grouping(g_v, 97, 251, repeat: true);
						if (num >= 0)
						{
							base.cursor += num;
							goto IL_00d8;
						}
					}
					goto IL_00e4;
				}
			}
			goto IL_00d8;
			IL_00d8:
			I_pV = base.cursor;
			goto IL_00e4;
			IL_00e4:
			base.cursor = cursor;
			int cursor3 = base.cursor;
			int num2 = out_grouping(g_v, 97, 251, repeat: true);
			if (num2 >= 0)
			{
				base.cursor += num2;
				int num3 = in_grouping(g_v, 97, 251, repeat: true);
				if (num3 >= 0)
				{
					base.cursor += num3;
					I_p1 = base.cursor;
					int num4 = out_grouping(g_v, 97, 251, repeat: true);
					if (num4 >= 0)
					{
						base.cursor += num4;
						int num5 = in_grouping(g_v, 97, 251, repeat: true);
						if (num5 >= 0)
						{
							base.cursor += num5;
							I_p2 = base.cursor;
						}
					}
				}
			}
			base.cursor = cursor3;
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
					slice_from("i");
					continue;
				case 2:
					slice_from("u");
					continue;
				case 3:
					slice_from("y");
					continue;
				case 4:
					slice_from("ë");
					continue;
				case 5:
					slice_from("ï");
					continue;
				case 6:
					slice_del();
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
			int num = find_among_b(a_4);
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
				int num9 = limit - cursor;
				ket = cursor;
				if (!eq_s_b("ic"))
				{
					cursor = limit - num9;
					break;
				}
				bra = cursor;
				int num10 = limit - cursor;
				if (r_R2())
				{
					slice_del();
					break;
				}
				cursor = limit - num10;
				slice_from("iqU");
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
				slice_from("ent");
				break;
			case 6:
			{
				if (!r_RV())
				{
					return false;
				}
				slice_del();
				int num4 = limit - cursor;
				ket = cursor;
				num = find_among_b(a_2);
				if (num == 0)
				{
					cursor = limit - num4;
					break;
				}
				bra = cursor;
				switch (num)
				{
				case 1:
					if (!r_R2())
					{
						cursor = limit - num4;
						break;
					}
					slice_del();
					ket = cursor;
					if (!eq_s_b("at"))
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
				case 2:
				{
					int num5 = limit - cursor;
					if (r_R2())
					{
						slice_del();
						break;
					}
					cursor = limit - num5;
					if (!r_R1())
					{
						cursor = limit - num4;
					}
					else
					{
						slice_from("eux");
					}
					break;
				}
				case 3:
					if (!r_R2())
					{
						cursor = limit - num4;
					}
					else
					{
						slice_del();
					}
					break;
				case 4:
					if (!r_RV())
					{
						cursor = limit - num4;
					}
					else
					{
						slice_from("i");
					}
					break;
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
				int num6 = limit - cursor;
				ket = cursor;
				num = find_among_b(a_3);
				if (num == 0)
				{
					cursor = limit - num6;
					break;
				}
				bra = cursor;
				switch (num)
				{
				case 1:
				{
					int num8 = limit - cursor;
					if (r_R2())
					{
						slice_del();
						break;
					}
					cursor = limit - num8;
					slice_from("abl");
					break;
				}
				case 2:
				{
					int num7 = limit - cursor;
					if (r_R2())
					{
						slice_del();
						break;
					}
					cursor = limit - num7;
					slice_from("iqU");
					break;
				}
				case 3:
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
				break;
			}
			case 8:
			{
				if (!r_R2())
				{
					return false;
				}
				slice_del();
				int num11 = limit - cursor;
				ket = cursor;
				if (!eq_s_b("at"))
				{
					cursor = limit - num11;
					break;
				}
				bra = cursor;
				if (!r_R2())
				{
					cursor = limit - num11;
					break;
				}
				slice_del();
				ket = cursor;
				if (!eq_s_b("ic"))
				{
					cursor = limit - num11;
					break;
				}
				bra = cursor;
				int num12 = limit - cursor;
				if (r_R2())
				{
					slice_del();
					break;
				}
				cursor = limit - num12;
				slice_from("iqU");
				break;
			}
			case 9:
				slice_from("eau");
				break;
			case 10:
				if (!r_R1())
				{
					return false;
				}
				slice_from("al");
				break;
			case 11:
			{
				int num3 = limit - cursor;
				if (r_R2())
				{
					slice_del();
					break;
				}
				cursor = limit - num3;
				if (!r_R1())
				{
					return false;
				}
				slice_from("eux");
				break;
			}
			case 12:
				if (!r_R1())
				{
					return false;
				}
				if (out_grouping_b(g_v, 97, 251, repeat: false) != 0)
				{
					return false;
				}
				slice_del();
				break;
			case 13:
				if (!r_RV())
				{
					return false;
				}
				slice_from("ant");
				return false;
			case 14:
				if (!r_RV())
				{
					return false;
				}
				slice_from("ent");
				return false;
			case 15:
			{
				int num2 = limit - cursor;
				if (in_grouping_b(g_v, 97, 251, repeat: false) != 0)
				{
					return false;
				}
				if (!r_RV())
				{
					return false;
				}
				cursor = limit - num2;
				slice_del();
				return false;
			}
			}
			return true;
		}

		private bool r_i_verb_suffix()
		{
			if (cursor < I_pV)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_pV;
			ket = cursor;
			if (find_among_b(a_5) == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			int num = limit - cursor;
			if (eq_s_b("H"))
			{
				base.limit_backward = limit_backward;
				return false;
			}
			cursor = limit - num;
			if (out_grouping_b(g_v, 97, 251, repeat: false) != 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			slice_del();
			base.limit_backward = limit_backward;
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
			int num = find_among_b(a_6);
			if (num == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				if (!r_R2())
				{
					base.limit_backward = limit_backward;
					return false;
				}
				slice_del();
				break;
			case 2:
				slice_del();
				break;
			case 3:
			{
				slice_del();
				int num2 = limit - cursor;
				ket = cursor;
				if (!eq_s_b("e"))
				{
					cursor = limit - num2;
					break;
				}
				bra = cursor;
				slice_del();
				break;
			}
			}
			base.limit_backward = limit_backward;
			return true;
		}

		private bool r_residual_suffix()
		{
			int num = limit - cursor;
			ket = cursor;
			if (!eq_s_b("s"))
			{
				cursor = limit - num;
			}
			else
			{
				bra = cursor;
				int num2 = limit - cursor;
				int num3 = limit - cursor;
				if (!eq_s_b("Hi"))
				{
					cursor = limit - num3;
					if (out_grouping_b(g_keep_with_s, 97, 232, repeat: false) != 0)
					{
						cursor = limit - num;
						goto IL_00b5;
					}
				}
				cursor = limit - num2;
				slice_del();
			}
			goto IL_00b5;
			IL_00b5:
			if (cursor < I_pV)
			{
				return false;
			}
			int limit_backward = base.limit_backward;
			base.limit_backward = I_pV;
			ket = cursor;
			int num4 = find_among_b(a_7);
			if (num4 == 0)
			{
				base.limit_backward = limit_backward;
				return false;
			}
			bra = cursor;
			switch (num4)
			{
			case 1:
			{
				if (!r_R2())
				{
					base.limit_backward = limit_backward;
					return false;
				}
				int num5 = limit - cursor;
				if (!eq_s_b("s"))
				{
					cursor = limit - num5;
					if (!eq_s_b("t"))
					{
						base.limit_backward = limit_backward;
						return false;
					}
				}
				slice_del();
				break;
			}
			case 2:
				slice_from("i");
				break;
			case 3:
				slice_del();
				break;
			}
			base.limit_backward = limit_backward;
			return true;
		}

		private bool r_un_double()
		{
			int num = limit - cursor;
			if (find_among_b(a_8) == 0)
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

		private bool r_un_accent()
		{
			int num = 1;
			while (out_grouping_b(g_v, 97, 251, repeat: false) == 0)
			{
				num--;
			}
			if (num > 0)
			{
				return false;
			}
			ket = cursor;
			int num2 = limit - cursor;
			if (!eq_s_b("é"))
			{
				cursor = limit - num2;
				if (!eq_s_b("è"))
				{
					return false;
				}
			}
			bra = cursor;
			slice_from("e");
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
				if (!r_i_verb_suffix())
				{
					base.cursor = limit - num4;
					if (!r_verb_suffix())
					{
						base.cursor = limit - num2;
						r_residual_suffix();
						goto IL_015f;
					}
				}
			}
			base.cursor = limit - num3;
			int num5 = limit - base.cursor;
			ket = base.cursor;
			int num6 = limit - base.cursor;
			if (eq_s_b("Y"))
			{
				bra = base.cursor;
				slice_from("i");
			}
			else
			{
				base.cursor = limit - num6;
				if (!eq_s_b("ç"))
				{
					base.cursor = limit - num5;
				}
				else
				{
					bra = base.cursor;
					slice_from("c");
				}
			}
			goto IL_015f;
			IL_015f:
			base.cursor = limit - num;
			int num7 = limit - base.cursor;
			r_un_double();
			base.cursor = limit - num7;
			int num8 = limit - base.cursor;
			r_un_accent();
			base.cursor = limit - num8;
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
					StemmerFactory.Register("fr", () => new Stemmer_fr(), -1679619262);
				}
				_registered = true;
			}
		}
	}
}
