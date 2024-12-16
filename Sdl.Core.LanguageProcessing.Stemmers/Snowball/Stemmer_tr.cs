using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_tr : Stemmer
	{
		private bool B_continue_stemming_noun_suffixes;

		private static string g_vowel = "aeıioöuü";

		private static string g_U = "ıiuü";

		private static string g_vowel1 = "aıou";

		private static string g_vowel2 = "eiöü";

		private static string g_vowel3 = "aı";

		private static string g_vowel4 = "ei";

		private static string g_vowel5 = "ou";

		private static string g_vowel6 = "öü";

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

		private readonly Among[] a_11;

		private readonly Among[] a_12;

		private readonly Among[] a_13;

		private readonly Among[] a_14;

		private readonly Among[] a_15;

		private readonly Among[] a_16;

		private readonly Among[] a_17;

		private readonly Among[] a_18;

		private readonly Among[] a_19;

		private readonly Among[] a_20;

		private readonly Among[] a_21;

		private readonly Among[] a_22;

		private readonly Among[] a_23;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_tr()
		{
			a_0 = new Among[10]
			{
				new Among("m", -1, -1),
				new Among("n", -1, -1),
				new Among("miz", -1, -1),
				new Among("niz", -1, -1),
				new Among("muz", -1, -1),
				new Among("nuz", -1, -1),
				new Among("müz", -1, -1),
				new Among("nüz", -1, -1),
				new Among("mız", -1, -1),
				new Among("nız", -1, -1)
			};
			a_1 = new Among[2]
			{
				new Among("leri", -1, -1),
				new Among("ları", -1, -1)
			};
			a_2 = new Among[4]
			{
				new Among("ni", -1, -1),
				new Among("nu", -1, -1),
				new Among("nü", -1, -1),
				new Among("nı", -1, -1)
			};
			a_3 = new Among[4]
			{
				new Among("in", -1, -1),
				new Among("un", -1, -1),
				new Among("ün", -1, -1),
				new Among("ın", -1, -1)
			};
			a_4 = new Among[2]
			{
				new Among("a", -1, -1),
				new Among("e", -1, -1)
			};
			a_5 = new Among[2]
			{
				new Among("na", -1, -1),
				new Among("ne", -1, -1)
			};
			a_6 = new Among[4]
			{
				new Among("da", -1, -1),
				new Among("ta", -1, -1),
				new Among("de", -1, -1),
				new Among("te", -1, -1)
			};
			a_7 = new Among[2]
			{
				new Among("nda", -1, -1),
				new Among("nde", -1, -1)
			};
			a_8 = new Among[4]
			{
				new Among("dan", -1, -1),
				new Among("tan", -1, -1),
				new Among("den", -1, -1),
				new Among("ten", -1, -1)
			};
			a_9 = new Among[2]
			{
				new Among("ndan", -1, -1),
				new Among("nden", -1, -1)
			};
			a_10 = new Among[2]
			{
				new Among("la", -1, -1),
				new Among("le", -1, -1)
			};
			a_11 = new Among[2]
			{
				new Among("ca", -1, -1),
				new Among("ce", -1, -1)
			};
			a_12 = new Among[4]
			{
				new Among("im", -1, -1),
				new Among("um", -1, -1),
				new Among("üm", -1, -1),
				new Among("ım", -1, -1)
			};
			a_13 = new Among[4]
			{
				new Among("sin", -1, -1),
				new Among("sun", -1, -1),
				new Among("sün", -1, -1),
				new Among("sın", -1, -1)
			};
			a_14 = new Among[4]
			{
				new Among("iz", -1, -1),
				new Among("uz", -1, -1),
				new Among("üz", -1, -1),
				new Among("ız", -1, -1)
			};
			a_15 = new Among[4]
			{
				new Among("siniz", -1, -1),
				new Among("sunuz", -1, -1),
				new Among("sünüz", -1, -1),
				new Among("sınız", -1, -1)
			};
			a_16 = new Among[2]
			{
				new Among("lar", -1, -1),
				new Among("ler", -1, -1)
			};
			a_17 = new Among[4]
			{
				new Among("niz", -1, -1),
				new Among("nuz", -1, -1),
				new Among("nüz", -1, -1),
				new Among("nız", -1, -1)
			};
			a_18 = new Among[8]
			{
				new Among("dir", -1, -1),
				new Among("tir", -1, -1),
				new Among("dur", -1, -1),
				new Among("tur", -1, -1),
				new Among("dür", -1, -1),
				new Among("tür", -1, -1),
				new Among("dır", -1, -1),
				new Among("tır", -1, -1)
			};
			a_19 = new Among[2]
			{
				new Among("casına", -1, -1),
				new Among("cesine", -1, -1)
			};
			a_20 = new Among[32]
			{
				new Among("di", -1, -1),
				new Among("ti", -1, -1),
				new Among("dik", -1, -1),
				new Among("tik", -1, -1),
				new Among("duk", -1, -1),
				new Among("tuk", -1, -1),
				new Among("dük", -1, -1),
				new Among("tük", -1, -1),
				new Among("dık", -1, -1),
				new Among("tık", -1, -1),
				new Among("dim", -1, -1),
				new Among("tim", -1, -1),
				new Among("dum", -1, -1),
				new Among("tum", -1, -1),
				new Among("düm", -1, -1),
				new Among("tüm", -1, -1),
				new Among("dım", -1, -1),
				new Among("tım", -1, -1),
				new Among("din", -1, -1),
				new Among("tin", -1, -1),
				new Among("dun", -1, -1),
				new Among("tun", -1, -1),
				new Among("dün", -1, -1),
				new Among("tün", -1, -1),
				new Among("dın", -1, -1),
				new Among("tın", -1, -1),
				new Among("du", -1, -1),
				new Among("tu", -1, -1),
				new Among("dü", -1, -1),
				new Among("tü", -1, -1),
				new Among("dı", -1, -1),
				new Among("tı", -1, -1)
			};
			a_21 = new Among[8]
			{
				new Among("sa", -1, -1),
				new Among("se", -1, -1),
				new Among("sak", -1, -1),
				new Among("sek", -1, -1),
				new Among("sam", -1, -1),
				new Among("sem", -1, -1),
				new Among("san", -1, -1),
				new Among("sen", -1, -1)
			};
			a_22 = new Among[4]
			{
				new Among("miş", -1, -1),
				new Among("muş", -1, -1),
				new Among("müş", -1, -1),
				new Among("mış", -1, -1)
			};
			a_23 = new Among[4]
			{
				new Among("b", -1, 1),
				new Among("c", -1, 2),
				new Among("d", -1, 3),
				new Among("ğ", -1, 4)
			};
		}

		private bool r_check_vowel_harmony()
		{
			int num = limit - cursor;
			if (out_grouping_b(g_vowel, 97, 305, repeat: true) < 0)
			{
				return false;
			}
			int num2 = limit - cursor;
			if (!eq_s_b("a") || out_grouping_b(g_vowel1, 97, 305, repeat: true) < 0)
			{
				cursor = limit - num2;
				if (!eq_s_b("e") || out_grouping_b(g_vowel2, 101, 252, repeat: true) < 0)
				{
					cursor = limit - num2;
					if (!eq_s_b("ı") || out_grouping_b(g_vowel3, 97, 305, repeat: true) < 0)
					{
						cursor = limit - num2;
						if (!eq_s_b("i") || out_grouping_b(g_vowel4, 101, 105, repeat: true) < 0)
						{
							cursor = limit - num2;
							if (!eq_s_b("o") || out_grouping_b(g_vowel5, 111, 117, repeat: true) < 0)
							{
								cursor = limit - num2;
								if (!eq_s_b("ö") || out_grouping_b(g_vowel6, 246, 252, repeat: true) < 0)
								{
									cursor = limit - num2;
									if (!eq_s_b("u") || out_grouping_b(g_vowel5, 111, 117, repeat: true) < 0)
									{
										cursor = limit - num2;
										if (!eq_s_b("ü"))
										{
											return false;
										}
										if (out_grouping_b(g_vowel6, 246, 252, repeat: true) < 0)
										{
											return false;
										}
									}
								}
							}
						}
					}
				}
			}
			cursor = limit - num;
			return true;
		}

		private bool r_mark_suffix_with_optional_n_consonant()
		{
			int num = limit - cursor;
			if (eq_s_b("n"))
			{
				int num2 = limit - cursor;
				if (in_grouping_b(g_vowel, 97, 305, repeat: false) == 0)
				{
					cursor = limit - num2;
					goto IL_00f9;
				}
			}
			cursor = limit - num;
			int num3 = limit - cursor;
			int num4 = limit - cursor;
			if (eq_s_b("n"))
			{
				cursor = limit - num4;
				return false;
			}
			cursor = limit - num3;
			int num5 = limit - cursor;
			if (cursor <= limit_backward)
			{
				return false;
			}
			cursor--;
			if (in_grouping_b(g_vowel, 97, 305, repeat: false) != 0)
			{
				return false;
			}
			cursor = limit - num5;
			goto IL_00f9;
			IL_00f9:
			return true;
		}

		private bool r_mark_suffix_with_optional_s_consonant()
		{
			int num = limit - cursor;
			if (eq_s_b("s"))
			{
				int num2 = limit - cursor;
				if (in_grouping_b(g_vowel, 97, 305, repeat: false) == 0)
				{
					cursor = limit - num2;
					goto IL_00f9;
				}
			}
			cursor = limit - num;
			int num3 = limit - cursor;
			int num4 = limit - cursor;
			if (eq_s_b("s"))
			{
				cursor = limit - num4;
				return false;
			}
			cursor = limit - num3;
			int num5 = limit - cursor;
			if (cursor <= limit_backward)
			{
				return false;
			}
			cursor--;
			if (in_grouping_b(g_vowel, 97, 305, repeat: false) != 0)
			{
				return false;
			}
			cursor = limit - num5;
			goto IL_00f9;
			IL_00f9:
			return true;
		}

		private bool r_mark_suffix_with_optional_y_consonant()
		{
			int num = limit - cursor;
			if (eq_s_b("y"))
			{
				int num2 = limit - cursor;
				if (in_grouping_b(g_vowel, 97, 305, repeat: false) == 0)
				{
					cursor = limit - num2;
					goto IL_00f9;
				}
			}
			cursor = limit - num;
			int num3 = limit - cursor;
			int num4 = limit - cursor;
			if (eq_s_b("y"))
			{
				cursor = limit - num4;
				return false;
			}
			cursor = limit - num3;
			int num5 = limit - cursor;
			if (cursor <= limit_backward)
			{
				return false;
			}
			cursor--;
			if (in_grouping_b(g_vowel, 97, 305, repeat: false) != 0)
			{
				return false;
			}
			cursor = limit - num5;
			goto IL_00f9;
			IL_00f9:
			return true;
		}

		private bool r_mark_suffix_with_optional_U_vowel()
		{
			int num = limit - cursor;
			if (in_grouping_b(g_U, 105, 305, repeat: false) == 0)
			{
				int num2 = limit - cursor;
				if (out_grouping_b(g_vowel, 97, 305, repeat: false) == 0)
				{
					cursor = limit - num2;
					goto IL_0109;
				}
			}
			cursor = limit - num;
			int num3 = limit - cursor;
			int num4 = limit - cursor;
			if (in_grouping_b(g_U, 105, 305, repeat: false) == 0)
			{
				cursor = limit - num4;
				return false;
			}
			cursor = limit - num3;
			int num5 = limit - cursor;
			if (cursor <= limit_backward)
			{
				return false;
			}
			cursor--;
			if (out_grouping_b(g_vowel, 97, 305, repeat: false) != 0)
			{
				return false;
			}
			cursor = limit - num5;
			goto IL_0109;
			IL_0109:
			return true;
		}

		private bool r_mark_possessives()
		{
			if (find_among_b(a_0) == 0)
			{
				return false;
			}
			return r_mark_suffix_with_optional_U_vowel();
		}

		private bool r_mark_sU()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (in_grouping_b(g_U, 105, 305, repeat: false) != 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_s_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_lArI()
		{
			if (find_among_b(a_1) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_yU()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (in_grouping_b(g_U, 105, 305, repeat: false) != 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_nU()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_2) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_nUn()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_3) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_n_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_yA()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_4) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_nA()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_5) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_DA()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_6) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_ndA()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_7) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_DAn()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_8) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_ndAn()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_9) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_ylA()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_10) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_ki()
		{
			if (!eq_s_b("ki"))
			{
				return false;
			}
			return true;
		}

		private bool r_mark_ncA()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_11) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_n_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_yUm()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_12) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_sUn()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_13) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_yUz()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_14) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_sUnUz()
		{
			if (find_among_b(a_15) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_lAr()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_16) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_nUz()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_17) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_DUr()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_18) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_cAsInA()
		{
			if (find_among_b(a_19) == 0)
			{
				return false;
			}
			return true;
		}

		private bool r_mark_yDU()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_20) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_ysA()
		{
			if (find_among_b(a_21) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_ymUs_()
		{
			if (!r_check_vowel_harmony())
			{
				return false;
			}
			if (find_among_b(a_22) == 0)
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_mark_yken()
		{
			if (!eq_s_b("ken"))
			{
				return false;
			}
			if (!r_mark_suffix_with_optional_y_consonant())
			{
				return false;
			}
			return true;
		}

		private bool r_stem_nominal_verb_suffixes()
		{
			ket = cursor;
			B_continue_stemming_noun_suffixes = true;
			int num = limit - cursor;
			int num2 = limit - cursor;
			if (!r_mark_ymUs_())
			{
				cursor = limit - num2;
				if (!r_mark_yDU())
				{
					cursor = limit - num2;
					if (!r_mark_ysA())
					{
						cursor = limit - num2;
						if (!r_mark_yken())
						{
							cursor = limit - num;
							if (r_mark_cAsInA())
							{
								int num3 = limit - cursor;
								if (!r_mark_sUnUz())
								{
									cursor = limit - num3;
									if (!r_mark_lAr())
									{
										cursor = limit - num3;
										if (!r_mark_yUm())
										{
											cursor = limit - num3;
											if (!r_mark_sUn())
											{
												cursor = limit - num3;
												if (!r_mark_yUz())
												{
													cursor = limit - num3;
												}
											}
										}
									}
								}
								if (r_mark_ymUs_())
								{
									goto IL_03be;
								}
							}
							cursor = limit - num;
							if (r_mark_lAr())
							{
								bra = cursor;
								slice_del();
								int num4 = limit - cursor;
								ket = cursor;
								int num5 = limit - cursor;
								if (!r_mark_DUr())
								{
									cursor = limit - num5;
									if (!r_mark_yDU())
									{
										cursor = limit - num5;
										if (!r_mark_ysA())
										{
											cursor = limit - num5;
											if (!r_mark_ymUs_())
											{
												cursor = limit - num4;
											}
										}
									}
								}
								B_continue_stemming_noun_suffixes = false;
							}
							else
							{
								cursor = limit - num;
								if (!r_mark_nUz())
								{
									goto IL_022a;
								}
								int num6 = limit - cursor;
								if (!r_mark_yDU())
								{
									cursor = limit - num6;
									if (!r_mark_ysA())
									{
										goto IL_022a;
									}
								}
							}
						}
					}
				}
			}
			goto IL_03be;
			IL_022a:
			cursor = limit - num;
			int num7 = limit - cursor;
			if (!r_mark_sUnUz())
			{
				cursor = limit - num7;
				if (!r_mark_yUz())
				{
					cursor = limit - num7;
					if (!r_mark_sUn())
					{
						cursor = limit - num7;
						if (!r_mark_yUm())
						{
							cursor = limit - num;
							if (!r_mark_DUr())
							{
								return false;
							}
							bra = cursor;
							slice_del();
							int num8 = limit - cursor;
							ket = cursor;
							int num9 = limit - cursor;
							if (!r_mark_sUnUz())
							{
								cursor = limit - num9;
								if (!r_mark_lAr())
								{
									cursor = limit - num9;
									if (!r_mark_yUm())
									{
										cursor = limit - num9;
										if (!r_mark_sUn())
										{
											cursor = limit - num9;
											if (!r_mark_yUz())
											{
												cursor = limit - num9;
											}
										}
									}
								}
							}
							if (!r_mark_ymUs_())
							{
								cursor = limit - num8;
							}
							goto IL_03be;
						}
					}
				}
			}
			bra = cursor;
			slice_del();
			int num10 = limit - cursor;
			ket = cursor;
			if (!r_mark_ymUs_())
			{
				cursor = limit - num10;
			}
			goto IL_03be;
			IL_03be:
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_stem_suffix_chain_before_ki()
		{
			ket = cursor;
			if (!r_mark_ki())
			{
				return false;
			}
			int num = limit - cursor;
			if (r_mark_DA())
			{
				bra = cursor;
				slice_del();
				int num2 = limit - cursor;
				ket = cursor;
				int num3 = limit - cursor;
				if (r_mark_lAr())
				{
					bra = cursor;
					slice_del();
					int num4 = limit - cursor;
					if (!r_stem_suffix_chain_before_ki())
					{
						cursor = limit - num4;
					}
				}
				else
				{
					cursor = limit - num3;
					if (!r_mark_possessives())
					{
						cursor = limit - num2;
					}
					else
					{
						bra = cursor;
						slice_del();
						int num5 = limit - cursor;
						ket = cursor;
						if (!r_mark_lAr())
						{
							cursor = limit - num5;
						}
						else
						{
							bra = cursor;
							slice_del();
							if (!r_stem_suffix_chain_before_ki())
							{
								cursor = limit - num5;
							}
						}
					}
				}
			}
			else
			{
				cursor = limit - num;
				if (r_mark_nUn())
				{
					bra = cursor;
					slice_del();
					int num6 = limit - cursor;
					ket = cursor;
					int num7 = limit - cursor;
					if (r_mark_lArI())
					{
						bra = cursor;
						slice_del();
					}
					else
					{
						cursor = limit - num7;
						ket = cursor;
						int num8 = limit - cursor;
						if (!r_mark_possessives())
						{
							cursor = limit - num8;
							if (!r_mark_sU())
							{
								cursor = limit - num7;
								if (!r_stem_suffix_chain_before_ki())
								{
									cursor = limit - num6;
								}
								goto IL_039e;
							}
						}
						bra = cursor;
						slice_del();
						int num9 = limit - cursor;
						ket = cursor;
						if (!r_mark_lAr())
						{
							cursor = limit - num9;
						}
						else
						{
							bra = cursor;
							slice_del();
							if (!r_stem_suffix_chain_before_ki())
							{
								cursor = limit - num9;
							}
						}
					}
				}
				else
				{
					cursor = limit - num;
					if (!r_mark_ndA())
					{
						return false;
					}
					int num10 = limit - cursor;
					if (r_mark_lArI())
					{
						bra = cursor;
						slice_del();
					}
					else
					{
						cursor = limit - num10;
						if (r_mark_sU())
						{
							bra = cursor;
							slice_del();
							int num11 = limit - cursor;
							ket = cursor;
							if (!r_mark_lAr())
							{
								cursor = limit - num11;
							}
							else
							{
								bra = cursor;
								slice_del();
								if (!r_stem_suffix_chain_before_ki())
								{
									cursor = limit - num11;
								}
							}
						}
						else
						{
							cursor = limit - num10;
							if (!r_stem_suffix_chain_before_ki())
							{
								return false;
							}
						}
					}
				}
			}
			goto IL_039e;
			IL_039e:
			return true;
		}

		private bool r_stem_noun_suffixes()
		{
			int num = limit - cursor;
			ket = cursor;
			if (r_mark_lAr())
			{
				bra = cursor;
				slice_del();
				int num2 = limit - cursor;
				if (!r_stem_suffix_chain_before_ki())
				{
					cursor = limit - num2;
				}
			}
			else
			{
				cursor = limit - num;
				ket = cursor;
				if (r_mark_ncA())
				{
					bra = cursor;
					slice_del();
					int num3 = limit - cursor;
					int num4 = limit - cursor;
					ket = cursor;
					if (r_mark_lArI())
					{
						bra = cursor;
						slice_del();
					}
					else
					{
						cursor = limit - num4;
						ket = cursor;
						int num5 = limit - cursor;
						if (!r_mark_possessives())
						{
							cursor = limit - num5;
							if (!r_mark_sU())
							{
								cursor = limit - num4;
								ket = cursor;
								if (!r_mark_lAr())
								{
									cursor = limit - num3;
								}
								else
								{
									bra = cursor;
									slice_del();
									if (!r_stem_suffix_chain_before_ki())
									{
										cursor = limit - num3;
									}
								}
								goto IL_0983;
							}
						}
						bra = cursor;
						slice_del();
						int num6 = limit - cursor;
						ket = cursor;
						if (!r_mark_lAr())
						{
							cursor = limit - num6;
						}
						else
						{
							bra = cursor;
							slice_del();
							if (!r_stem_suffix_chain_before_ki())
							{
								cursor = limit - num6;
							}
						}
					}
				}
				else
				{
					cursor = limit - num;
					ket = cursor;
					int num7 = limit - cursor;
					if (!r_mark_ndA())
					{
						cursor = limit - num7;
						if (!r_mark_nA())
						{
							goto IL_0329;
						}
					}
					int num8 = limit - cursor;
					if (r_mark_lArI())
					{
						bra = cursor;
						slice_del();
					}
					else
					{
						cursor = limit - num8;
						if (r_mark_sU())
						{
							bra = cursor;
							slice_del();
							int num9 = limit - cursor;
							ket = cursor;
							if (!r_mark_lAr())
							{
								cursor = limit - num9;
							}
							else
							{
								bra = cursor;
								slice_del();
								if (!r_stem_suffix_chain_before_ki())
								{
									cursor = limit - num9;
								}
							}
						}
						else
						{
							cursor = limit - num8;
							if (!r_stem_suffix_chain_before_ki())
							{
								goto IL_0329;
							}
						}
					}
				}
			}
			goto IL_0983;
			IL_041f:
			cursor = limit - num;
			ket = cursor;
			if (r_mark_DAn())
			{
				bra = cursor;
				slice_del();
				int num10 = limit - cursor;
				ket = cursor;
				int num11 = limit - cursor;
				if (r_mark_possessives())
				{
					bra = cursor;
					slice_del();
					int num12 = limit - cursor;
					ket = cursor;
					if (!r_mark_lAr())
					{
						cursor = limit - num12;
					}
					else
					{
						bra = cursor;
						slice_del();
						if (!r_stem_suffix_chain_before_ki())
						{
							cursor = limit - num12;
						}
					}
				}
				else
				{
					cursor = limit - num11;
					if (r_mark_lAr())
					{
						bra = cursor;
						slice_del();
						int num13 = limit - cursor;
						if (!r_stem_suffix_chain_before_ki())
						{
							cursor = limit - num13;
						}
					}
					else
					{
						cursor = limit - num11;
						if (!r_stem_suffix_chain_before_ki())
						{
							cursor = limit - num10;
						}
					}
				}
			}
			else
			{
				cursor = limit - num;
				ket = cursor;
				int num14 = limit - cursor;
				if (!r_mark_nUn())
				{
					cursor = limit - num14;
					if (!r_mark_ylA())
					{
						cursor = limit - num;
						ket = cursor;
						if (r_mark_lArI())
						{
							bra = cursor;
							slice_del();
						}
						else
						{
							cursor = limit - num;
							if (!r_stem_suffix_chain_before_ki())
							{
								cursor = limit - num;
								ket = cursor;
								int num15 = limit - cursor;
								if (!r_mark_DA())
								{
									cursor = limit - num15;
									if (!r_mark_yU())
									{
										cursor = limit - num15;
										if (!r_mark_yA())
										{
											cursor = limit - num;
											ket = cursor;
											int num16 = limit - cursor;
											if (!r_mark_possessives())
											{
												cursor = limit - num16;
												if (!r_mark_sU())
												{
													return false;
												}
											}
											bra = cursor;
											slice_del();
											int num17 = limit - cursor;
											ket = cursor;
											if (!r_mark_lAr())
											{
												cursor = limit - num17;
											}
											else
											{
												bra = cursor;
												slice_del();
												if (!r_stem_suffix_chain_before_ki())
												{
													cursor = limit - num17;
												}
											}
											goto IL_0983;
										}
									}
								}
								bra = cursor;
								slice_del();
								int num18 = limit - cursor;
								ket = cursor;
								int num19 = limit - cursor;
								if (r_mark_possessives())
								{
									bra = cursor;
									slice_del();
									int num20 = limit - cursor;
									ket = cursor;
									if (!r_mark_lAr())
									{
										cursor = limit - num20;
									}
								}
								else
								{
									cursor = limit - num19;
									if (!r_mark_lAr())
									{
										cursor = limit - num18;
										goto IL_0983;
									}
								}
								bra = cursor;
								slice_del();
								ket = cursor;
								if (!r_stem_suffix_chain_before_ki())
								{
									cursor = limit - num18;
								}
							}
						}
						goto IL_0983;
					}
				}
				bra = cursor;
				slice_del();
				int num21 = limit - cursor;
				int num22 = limit - cursor;
				ket = cursor;
				if (r_mark_lAr())
				{
					bra = cursor;
					slice_del();
					if (r_stem_suffix_chain_before_ki())
					{
						goto IL_0983;
					}
				}
				cursor = limit - num22;
				ket = cursor;
				int num23 = limit - cursor;
				if (!r_mark_possessives())
				{
					cursor = limit - num23;
					if (!r_mark_sU())
					{
						cursor = limit - num22;
						if (!r_stem_suffix_chain_before_ki())
						{
							cursor = limit - num21;
						}
						goto IL_0983;
					}
				}
				bra = cursor;
				slice_del();
				int num24 = limit - cursor;
				ket = cursor;
				if (!r_mark_lAr())
				{
					cursor = limit - num24;
				}
				else
				{
					bra = cursor;
					slice_del();
					if (!r_stem_suffix_chain_before_ki())
					{
						cursor = limit - num24;
					}
				}
			}
			goto IL_0983;
			IL_0329:
			cursor = limit - num;
			ket = cursor;
			int num25 = limit - cursor;
			if (!r_mark_ndAn())
			{
				cursor = limit - num25;
				if (!r_mark_nU())
				{
					goto IL_041f;
				}
			}
			int num26 = limit - cursor;
			if (r_mark_sU())
			{
				bra = cursor;
				slice_del();
				int num27 = limit - cursor;
				ket = cursor;
				if (!r_mark_lAr())
				{
					cursor = limit - num27;
				}
				else
				{
					bra = cursor;
					slice_del();
					if (!r_stem_suffix_chain_before_ki())
					{
						cursor = limit - num27;
					}
				}
			}
			else
			{
				cursor = limit - num26;
				if (!r_mark_lArI())
				{
					goto IL_041f;
				}
			}
			goto IL_0983;
			IL_0983:
			return true;
		}

		private bool r_post_process_last_consonants()
		{
			ket = cursor;
			int num = find_among_b(a_23);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				slice_from("p");
				break;
			case 2:
				slice_from("ç");
				break;
			case 3:
				slice_from("t");
				break;
			case 4:
				slice_from("k");
				break;
			}
			return true;
		}

		private bool r_append_U_to_stems_ending_with_d_or_g()
		{
			int num = limit - base.cursor;
			int num2 = limit - base.cursor;
			if (!eq_s_b("d"))
			{
				base.cursor = limit - num2;
				if (!eq_s_b("g"))
				{
					return false;
				}
			}
			base.cursor = limit - num;
			int num3 = limit - base.cursor;
			int num4 = limit - base.cursor;
			if (out_grouping_b(g_vowel, 97, 305, repeat: true) < 0)
			{
				goto IL_00f8;
			}
			int num5 = limit - base.cursor;
			if (!eq_s_b("a"))
			{
				base.cursor = limit - num5;
				if (!eq_s_b("ı"))
				{
					goto IL_00f8;
				}
			}
			base.cursor = limit - num4;
			int cursor = base.cursor;
			insert(base.cursor, base.cursor, "ı");
			base.cursor = cursor;
			goto IL_02e9;
			IL_02e9:
			return true;
			IL_00f8:
			base.cursor = limit - num3;
			int num6 = limit - base.cursor;
			if (out_grouping_b(g_vowel, 97, 305, repeat: true) < 0)
			{
				goto IL_019e;
			}
			int num7 = limit - base.cursor;
			if (!eq_s_b("e"))
			{
				base.cursor = limit - num7;
				if (!eq_s_b("i"))
				{
					goto IL_019e;
				}
			}
			base.cursor = limit - num6;
			int cursor2 = base.cursor;
			insert(base.cursor, base.cursor, "i");
			base.cursor = cursor2;
			goto IL_02e9;
			IL_0244:
			base.cursor = limit - num3;
			int num8 = limit - base.cursor;
			if (out_grouping_b(g_vowel, 97, 305, repeat: true) < 0)
			{
				return false;
			}
			int num9 = limit - base.cursor;
			if (!eq_s_b("ö"))
			{
				base.cursor = limit - num9;
				if (!eq_s_b("ü"))
				{
					return false;
				}
			}
			base.cursor = limit - num8;
			int cursor3 = base.cursor;
			insert(base.cursor, base.cursor, "ü");
			base.cursor = cursor3;
			goto IL_02e9;
			IL_019e:
			base.cursor = limit - num3;
			int num10 = limit - base.cursor;
			if (out_grouping_b(g_vowel, 97, 305, repeat: true) < 0)
			{
				goto IL_0244;
			}
			int num11 = limit - base.cursor;
			if (!eq_s_b("o"))
			{
				base.cursor = limit - num11;
				if (!eq_s_b("u"))
				{
					goto IL_0244;
				}
			}
			base.cursor = limit - num10;
			int cursor4 = base.cursor;
			insert(base.cursor, base.cursor, "u");
			base.cursor = cursor4;
			goto IL_02e9;
		}

		private bool r_is_reserved_word()
		{
			if (!eq_s_b("ad"))
			{
				return false;
			}
			int num = limit - cursor;
			if (!eq_s_b("soy"))
			{
				cursor = limit - num;
			}
			if (cursor > limit_backward)
			{
				return false;
			}
			return true;
		}

		private bool r_more_than_one_syllable_word()
		{
			int cursor = base.cursor;
			int num = 2;
			int cursor2;
			while (true)
			{
				cursor2 = base.cursor;
				int num2 = out_grouping(g_vowel, 97, 305, repeat: true);
				if (num2 < 0)
				{
					break;
				}
				base.cursor += num2;
				num--;
			}
			base.cursor = cursor2;
			if (num > 0)
			{
				return false;
			}
			base.cursor = cursor;
			return true;
		}

		private bool r_postlude()
		{
			limit_backward = cursor;
			cursor = limit;
			int num = limit - cursor;
			if (r_is_reserved_word())
			{
				return false;
			}
			cursor = limit - num;
			int num2 = limit - cursor;
			r_append_U_to_stems_ending_with_d_or_g();
			cursor = limit - num2;
			int num3 = limit - cursor;
			r_post_process_last_consonants();
			cursor = limit - num3;
			cursor = limit_backward;
			return true;
		}

		protected override bool stem()
		{
			if (!r_more_than_one_syllable_word())
			{
				return false;
			}
			limit_backward = cursor;
			cursor = limit;
			int num = limit - cursor;
			r_stem_nominal_verb_suffixes();
			cursor = limit - num;
			if (!B_continue_stemming_noun_suffixes)
			{
				return false;
			}
			int num2 = limit - cursor;
			r_stem_noun_suffixes();
			cursor = limit - num2;
			cursor = limit_backward;
			if (!r_postlude())
			{
				return false;
			}
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("tr", () => new Stemmer_tr(), -467532233);
				}
				_registered = true;
			}
		}
	}
}
