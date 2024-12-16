using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_ar : Stemmer
	{
		private bool B_is_defined;

		private bool B_is_verb;

		private bool B_is_noun;

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

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_ar()
		{
			a_0 = new Among[144]
			{
				new Among("\u0640", -1, 1),
				new Among("\u064b", -1, 1),
				new Among("\u064c", -1, 1),
				new Among("\u064d", -1, 1),
				new Among("\u064e", -1, 1),
				new Among("\u064f", -1, 1),
				new Among("\u0650", -1, 1),
				new Among("\u0651", -1, 1),
				new Among("\u0652", -1, 1),
				new Among("٠", -1, 2),
				new Among("١", -1, 3),
				new Among("٢", -1, 4),
				new Among("٣", -1, 5),
				new Among("٤", -1, 6),
				new Among("٥", -1, 7),
				new Among("٦", -1, 8),
				new Among("٧", -1, 9),
				new Among("٨", -1, 10),
				new Among("٩", -1, 11),
				new Among("ﺀ", -1, 12),
				new Among("ﺁ", -1, 16),
				new Among("ﺂ", -1, 16),
				new Among("ﺃ", -1, 13),
				new Among("ﺄ", -1, 13),
				new Among("ﺅ", -1, 17),
				new Among("ﺆ", -1, 17),
				new Among("ﺇ", -1, 14),
				new Among("ﺈ", -1, 14),
				new Among("ﺉ", -1, 15),
				new Among("ﺊ", -1, 15),
				new Among("ﺋ", -1, 15),
				new Among("ﺌ", -1, 15),
				new Among("ﺍ", -1, 18),
				new Among("ﺎ", -1, 18),
				new Among("ﺏ", -1, 19),
				new Among("ﺐ", -1, 19),
				new Among("ﺑ", -1, 19),
				new Among("ﺒ", -1, 19),
				new Among("ﺓ", -1, 20),
				new Among("ﺔ", -1, 20),
				new Among("ﺕ", -1, 21),
				new Among("ﺖ", -1, 21),
				new Among("ﺗ", -1, 21),
				new Among("ﺘ", -1, 21),
				new Among("ﺙ", -1, 22),
				new Among("ﺚ", -1, 22),
				new Among("ﺛ", -1, 22),
				new Among("ﺜ", -1, 22),
				new Among("ﺝ", -1, 23),
				new Among("ﺞ", -1, 23),
				new Among("ﺟ", -1, 23),
				new Among("ﺠ", -1, 23),
				new Among("ﺡ", -1, 24),
				new Among("ﺢ", -1, 24),
				new Among("ﺣ", -1, 24),
				new Among("ﺤ", -1, 24),
				new Among("ﺥ", -1, 25),
				new Among("ﺦ", -1, 25),
				new Among("ﺧ", -1, 25),
				new Among("ﺨ", -1, 25),
				new Among("ﺩ", -1, 26),
				new Among("ﺪ", -1, 26),
				new Among("ﺫ", -1, 27),
				new Among("ﺬ", -1, 27),
				new Among("ﺭ", -1, 28),
				new Among("ﺮ", -1, 28),
				new Among("ﺯ", -1, 29),
				new Among("ﺰ", -1, 29),
				new Among("ﺱ", -1, 30),
				new Among("ﺲ", -1, 30),
				new Among("ﺳ", -1, 30),
				new Among("ﺴ", -1, 30),
				new Among("ﺵ", -1, 31),
				new Among("ﺶ", -1, 31),
				new Among("ﺷ", -1, 31),
				new Among("ﺸ", -1, 31),
				new Among("ﺹ", -1, 32),
				new Among("ﺺ", -1, 32),
				new Among("ﺻ", -1, 32),
				new Among("ﺼ", -1, 32),
				new Among("ﺽ", -1, 33),
				new Among("ﺾ", -1, 33),
				new Among("ﺿ", -1, 33),
				new Among("ﻀ", -1, 33),
				new Among("ﻁ", -1, 34),
				new Among("ﻂ", -1, 34),
				new Among("ﻃ", -1, 34),
				new Among("ﻄ", -1, 34),
				new Among("ﻅ", -1, 35),
				new Among("ﻆ", -1, 35),
				new Among("ﻇ", -1, 35),
				new Among("ﻈ", -1, 35),
				new Among("ﻉ", -1, 36),
				new Among("ﻊ", -1, 36),
				new Among("ﻋ", -1, 36),
				new Among("ﻌ", -1, 36),
				new Among("ﻍ", -1, 37),
				new Among("ﻎ", -1, 37),
				new Among("ﻏ", -1, 37),
				new Among("ﻐ", -1, 37),
				new Among("ﻑ", -1, 38),
				new Among("ﻒ", -1, 38),
				new Among("ﻓ", -1, 38),
				new Among("ﻔ", -1, 38),
				new Among("ﻕ", -1, 39),
				new Among("ﻖ", -1, 39),
				new Among("ﻗ", -1, 39),
				new Among("ﻘ", -1, 39),
				new Among("ﻙ", -1, 40),
				new Among("ﻚ", -1, 40),
				new Among("ﻛ", -1, 40),
				new Among("ﻜ", -1, 40),
				new Among("ﻝ", -1, 41),
				new Among("ﻞ", -1, 41),
				new Among("ﻟ", -1, 41),
				new Among("ﻠ", -1, 41),
				new Among("ﻡ", -1, 42),
				new Among("ﻢ", -1, 42),
				new Among("ﻣ", -1, 42),
				new Among("ﻤ", -1, 42),
				new Among("ﻥ", -1, 43),
				new Among("ﻦ", -1, 43),
				new Among("ﻧ", -1, 43),
				new Among("ﻨ", -1, 43),
				new Among("ﻩ", -1, 44),
				new Among("ﻪ", -1, 44),
				new Among("ﻫ", -1, 44),
				new Among("ﻬ", -1, 44),
				new Among("ﻭ", -1, 45),
				new Among("ﻮ", -1, 45),
				new Among("ﻯ", -1, 46),
				new Among("ﻰ", -1, 46),
				new Among("ﻱ", -1, 47),
				new Among("ﻲ", -1, 47),
				new Among("ﻳ", -1, 47),
				new Among("ﻴ", -1, 47),
				new Among("ﻵ", -1, 51),
				new Among("ﻶ", -1, 51),
				new Among("ﻷ", -1, 49),
				new Among("ﻸ", -1, 49),
				new Among("ﻹ", -1, 50),
				new Among("ﻺ", -1, 50),
				new Among("ﻻ", -1, 48),
				new Among("ﻼ", -1, 48)
			};
			a_1 = new Among[5]
			{
				new Among("آ", -1, 1),
				new Among("أ", -1, 1),
				new Among("ؤ", -1, 1),
				new Among("إ", -1, 1),
				new Among("ئ", -1, 1)
			};
			a_2 = new Among[5]
			{
				new Among("آ", -1, 1),
				new Among("أ", -1, 1),
				new Among("ؤ", -1, 2),
				new Among("إ", -1, 1),
				new Among("ئ", -1, 3)
			};
			a_3 = new Among[4]
			{
				new Among("ال", -1, 2),
				new Among("بال", -1, 1),
				new Among("كال", -1, 1),
				new Among("لل", -1, 2)
			};
			a_4 = new Among[5]
			{
				new Among("أآ", -1, 2),
				new Among("أأ", -1, 1),
				new Among("أؤ", -1, 1),
				new Among("أإ", -1, 4),
				new Among("أا", -1, 3)
			};
			a_5 = new Among[2]
			{
				new Among("ف", -1, 1),
				new Among("و", -1, 1)
			};
			a_6 = new Among[4]
			{
				new Among("ال", -1, 2),
				new Among("بال", -1, 1),
				new Among("كال", -1, 1),
				new Among("لل", -1, 2)
			};
			a_7 = new Among[3]
			{
				new Among("ب", -1, 1),
				new Among("بب", 0, 2),
				new Among("كك", -1, 3)
			};
			a_8 = new Among[4]
			{
				new Among("سأ", -1, 4),
				new Among("ست", -1, 2),
				new Among("سن", -1, 3),
				new Among("سي", -1, 1)
			};
			a_9 = new Among[3]
			{
				new Among("تست", -1, 1),
				new Among("نست", -1, 1),
				new Among("يست", -1, 1)
			};
			a_10 = new Among[10]
			{
				new Among("كما", -1, 3),
				new Among("هما", -1, 3),
				new Among("نا", -1, 2),
				new Among("ها", -1, 2),
				new Among("ك", -1, 1),
				new Among("كم", -1, 2),
				new Among("هم", -1, 2),
				new Among("هن", -1, 2),
				new Among("ه", -1, 1),
				new Among("ي", -1, 1)
			};
			a_11 = new Among[1]
			{
				new Among("ن", -1, 1)
			};
			a_12 = new Among[3]
			{
				new Among("ا", -1, 1),
				new Among("و", -1, 1),
				new Among("ي", -1, 1)
			};
			a_13 = new Among[1]
			{
				new Among("ات", -1, 1)
			};
			a_14 = new Among[1]
			{
				new Among("ت", -1, 1)
			};
			a_15 = new Among[1]
			{
				new Among("ة", -1, 1)
			};
			a_16 = new Among[1]
			{
				new Among("ي", -1, 1)
			};
			a_17 = new Among[12]
			{
				new Among("كما", -1, 3),
				new Among("هما", -1, 3),
				new Among("نا", -1, 2),
				new Among("ها", -1, 2),
				new Among("ك", -1, 1),
				new Among("كم", -1, 2),
				new Among("هم", -1, 2),
				new Among("كن", -1, 2),
				new Among("هن", -1, 2),
				new Among("ه", -1, 1),
				new Among("كمو", -1, 3),
				new Among("ني", -1, 2)
			};
			a_18 = new Among[11]
			{
				new Among("ا", -1, 1),
				new Among("تا", 0, 2),
				new Among("تما", 0, 4),
				new Among("نا", 0, 2),
				new Among("ت", -1, 1),
				new Among("ن", -1, 1),
				new Among("ان", 5, 3),
				new Among("تن", 5, 2),
				new Among("ون", 5, 3),
				new Among("ين", 5, 3),
				new Among("ي", -1, 1)
			};
			a_19 = new Among[2]
			{
				new Among("وا", -1, 1),
				new Among("تم", -1, 1)
			};
			a_20 = new Among[2]
			{
				new Among("و", -1, 1),
				new Among("تمو", 0, 2)
			};
			a_21 = new Among[1]
			{
				new Among("ى", -1, 1)
			};
		}

		private bool r_Normalize_pre()
		{
			int cursor = base.cursor;
			int cursor2;
			while (true)
			{
				cursor2 = base.cursor;
				int cursor3 = base.cursor;
				bra = base.cursor;
				int num = find_among(a_0);
				if (num != 0)
				{
					ket = base.cursor;
					switch (num)
					{
					case 1:
						slice_del();
						break;
					case 2:
						slice_from("0");
						break;
					case 3:
						slice_from("1");
						break;
					case 4:
						slice_from("2");
						break;
					case 5:
						slice_from("3");
						break;
					case 6:
						slice_from("4");
						break;
					case 7:
						slice_from("5");
						break;
					case 8:
						slice_from("6");
						break;
					case 9:
						slice_from("7");
						break;
					case 10:
						slice_from("8");
						break;
					case 11:
						slice_from("9");
						break;
					case 12:
						slice_from("ء");
						break;
					case 13:
						slice_from("أ");
						break;
					case 14:
						slice_from("إ");
						break;
					case 15:
						slice_from("ئ");
						break;
					case 16:
						slice_from("آ");
						break;
					case 17:
						slice_from("ؤ");
						break;
					case 18:
						slice_from("ا");
						break;
					case 19:
						slice_from("ب");
						break;
					case 20:
						slice_from("ة");
						break;
					case 21:
						slice_from("ت");
						break;
					case 22:
						slice_from("ث");
						break;
					case 23:
						slice_from("ج");
						break;
					case 24:
						slice_from("ح");
						break;
					case 25:
						slice_from("خ");
						break;
					case 26:
						slice_from("د");
						break;
					case 27:
						slice_from("ذ");
						break;
					case 28:
						slice_from("ر");
						break;
					case 29:
						slice_from("ز");
						break;
					case 30:
						slice_from("س");
						break;
					case 31:
						slice_from("ش");
						break;
					case 32:
						slice_from("ص");
						break;
					case 33:
						slice_from("ض");
						break;
					case 34:
						slice_from("ط");
						break;
					case 35:
						slice_from("ظ");
						break;
					case 36:
						slice_from("ع");
						break;
					case 37:
						slice_from("غ");
						break;
					case 38:
						slice_from("ف");
						break;
					case 39:
						slice_from("ق");
						break;
					case 40:
						slice_from("ك");
						break;
					case 41:
						slice_from("ل");
						break;
					case 42:
						slice_from("م");
						break;
					case 43:
						slice_from("ن");
						break;
					case 44:
						slice_from("ه");
						break;
					case 45:
						slice_from("و");
						break;
					case 46:
						slice_from("ى");
						break;
					case 47:
						slice_from("ي");
						break;
					case 48:
						slice_from("لا");
						break;
					case 49:
						slice_from("لأ");
						break;
					case 50:
						slice_from("لإ");
						break;
					case 51:
						slice_from("لآ");
						break;
					}
				}
				else
				{
					base.cursor = cursor3;
					if (base.cursor >= limit)
					{
						break;
					}
					base.cursor++;
				}
			}
			base.cursor = cursor2;
			base.cursor = cursor;
			return true;
		}

		private bool r_Normalize_post()
		{
			int cursor = base.cursor;
			limit_backward = base.cursor;
			base.cursor = limit;
			ket = base.cursor;
			if (find_among_b(a_1) != 0)
			{
				bra = base.cursor;
				slice_from("ء");
				base.cursor = limit_backward;
			}
			base.cursor = cursor;
			int cursor2 = base.cursor;
			int cursor3;
			while (true)
			{
				cursor3 = base.cursor;
				int cursor4 = base.cursor;
				bra = base.cursor;
				int num = find_among(a_2);
				if (num != 0)
				{
					ket = base.cursor;
					switch (num)
					{
					case 1:
						slice_from("ا");
						break;
					case 2:
						slice_from("و");
						break;
					case 3:
						slice_from("ي");
						break;
					}
				}
				else
				{
					base.cursor = cursor4;
					if (base.cursor >= limit)
					{
						break;
					}
					base.cursor++;
				}
			}
			base.cursor = cursor3;
			base.cursor = cursor2;
			return true;
		}

		private bool r_Checks1()
		{
			bra = cursor;
			int num = find_among(a_3);
			if (num == 0)
			{
				return false;
			}
			ket = cursor;
			switch (num)
			{
			case 1:
				if (current.Length <= 4)
				{
					return false;
				}
				B_is_noun = true;
				B_is_verb = false;
				B_is_defined = true;
				break;
			case 2:
				if (current.Length <= 3)
				{
					return false;
				}
				B_is_noun = true;
				B_is_verb = false;
				B_is_defined = true;
				break;
			}
			return true;
		}

		private bool r_Prefix_Step1()
		{
			bra = cursor;
			int num = find_among(a_4);
			if (num == 0)
			{
				return false;
			}
			ket = cursor;
			switch (num)
			{
			case 1:
				if (current.Length <= 3)
				{
					return false;
				}
				slice_from("أ");
				break;
			case 2:
				if (current.Length <= 3)
				{
					return false;
				}
				slice_from("آ");
				break;
			case 3:
				if (current.Length <= 3)
				{
					return false;
				}
				slice_from("ا");
				break;
			case 4:
				if (current.Length <= 3)
				{
					return false;
				}
				slice_from("إ");
				break;
			}
			return true;
		}

		private bool r_Prefix_Step2()
		{
			int cursor = base.cursor;
			if (eq_s("فا"))
			{
				return false;
			}
			base.cursor = cursor;
			int cursor2 = base.cursor;
			if (eq_s("وا"))
			{
				return false;
			}
			base.cursor = cursor2;
			bra = base.cursor;
			if (find_among(a_5) == 0)
			{
				return false;
			}
			ket = base.cursor;
			if (current.Length <= 3)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Prefix_Step3a_Noun()
		{
			bra = cursor;
			int num = find_among(a_6);
			if (num == 0)
			{
				return false;
			}
			ket = cursor;
			switch (num)
			{
			case 1:
				if (current.Length <= 5)
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (current.Length <= 4)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_Prefix_Step3b_Noun()
		{
			int cursor = base.cursor;
			if (eq_s("با"))
			{
				return false;
			}
			base.cursor = cursor;
			bra = base.cursor;
			int num = find_among(a_7);
			if (num == 0)
			{
				return false;
			}
			ket = base.cursor;
			switch (num)
			{
			case 1:
				if (current.Length <= 3)
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (current.Length <= 3)
				{
					return false;
				}
				slice_from("ب");
				break;
			case 3:
				if (current.Length <= 3)
				{
					return false;
				}
				slice_from("ك");
				break;
			}
			return true;
		}

		private bool r_Prefix_Step3_Verb()
		{
			bra = cursor;
			int num = find_among(a_8);
			if (num == 0)
			{
				return false;
			}
			ket = cursor;
			switch (num)
			{
			case 1:
				if (current.Length <= 4)
				{
					return false;
				}
				slice_from("ي");
				break;
			case 2:
				if (current.Length <= 4)
				{
					return false;
				}
				slice_from("ت");
				break;
			case 3:
				if (current.Length <= 4)
				{
					return false;
				}
				slice_from("ن");
				break;
			case 4:
				if (current.Length <= 4)
				{
					return false;
				}
				slice_from("أ");
				break;
			}
			return true;
		}

		private bool r_Prefix_Step4_Verb()
		{
			bra = cursor;
			if (find_among(a_9) == 0)
			{
				return false;
			}
			ket = cursor;
			if (current.Length <= 4)
			{
				return false;
			}
			B_is_verb = true;
			B_is_noun = false;
			slice_from("است");
			return true;
		}

		private bool r_Suffix_Noun_Step1a()
		{
			ket = cursor;
			int num = find_among_b(a_10);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				if (current.Length < 4)
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (current.Length < 5)
				{
					return false;
				}
				slice_del();
				break;
			case 3:
				if (current.Length < 6)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_Suffix_Noun_Step1b()
		{
			ket = cursor;
			if (find_among_b(a_11) == 0)
			{
				return false;
			}
			bra = cursor;
			if (current.Length <= 5)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Suffix_Noun_Step2a()
		{
			ket = cursor;
			if (find_among_b(a_12) == 0)
			{
				return false;
			}
			bra = cursor;
			if (current.Length <= 4)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Suffix_Noun_Step2b()
		{
			ket = cursor;
			if (find_among_b(a_13) == 0)
			{
				return false;
			}
			bra = cursor;
			if (current.Length < 5)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Suffix_Noun_Step2c1()
		{
			ket = cursor;
			if (find_among_b(a_14) == 0)
			{
				return false;
			}
			bra = cursor;
			if (current.Length < 4)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Suffix_Noun_Step2c2()
		{
			ket = cursor;
			if (find_among_b(a_15) == 0)
			{
				return false;
			}
			bra = cursor;
			if (current.Length < 4)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Suffix_Noun_Step3()
		{
			ket = cursor;
			if (find_among_b(a_16) == 0)
			{
				return false;
			}
			bra = cursor;
			if (current.Length < 3)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Suffix_Verb_Step1()
		{
			ket = cursor;
			int num = find_among_b(a_17);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				if (current.Length < 4)
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (current.Length < 5)
				{
					return false;
				}
				slice_del();
				break;
			case 3:
				if (current.Length < 6)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_Suffix_Verb_Step2a()
		{
			ket = cursor;
			int num = find_among_b(a_18);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				if (current.Length < 4)
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (current.Length < 5)
				{
					return false;
				}
				slice_del();
				break;
			case 3:
				if (current.Length <= 5)
				{
					return false;
				}
				slice_del();
				break;
			case 4:
				if (current.Length < 6)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_Suffix_Verb_Step2b()
		{
			ket = cursor;
			if (find_among_b(a_19) == 0)
			{
				return false;
			}
			bra = cursor;
			if (current.Length < 5)
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_Suffix_Verb_Step2c()
		{
			ket = cursor;
			int num = find_among_b(a_20);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
				if (current.Length < 4)
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (current.Length < 6)
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_Suffix_All_alef_maqsura()
		{
			ket = cursor;
			if (find_among_b(a_21) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_from("ي");
			return true;
		}

		protected override bool stem()
		{
			B_is_noun = true;
			B_is_verb = true;
			B_is_defined = false;
			int cursor = base.cursor;
			r_Checks1();
			base.cursor = cursor;
			r_Normalize_pre();
			limit_backward = base.cursor;
			base.cursor = limit;
			int num = limit - base.cursor;
			int num2 = limit - base.cursor;
			if (!B_is_verb)
			{
				goto IL_014a;
			}
			int num3 = limit - base.cursor;
			int num4 = 1;
			int num5;
			while (true)
			{
				num5 = limit - base.cursor;
				if (!r_Suffix_Verb_Step1())
				{
					break;
				}
				num4--;
			}
			base.cursor = limit - num5;
			if (num4 > 0)
			{
				goto IL_0118;
			}
			int num6 = limit - base.cursor;
			if (!r_Suffix_Verb_Step2a())
			{
				base.cursor = limit - num6;
				if (!r_Suffix_Verb_Step2c())
				{
					base.cursor = limit - num6;
					if (base.cursor <= limit_backward)
					{
						goto IL_0118;
					}
					base.cursor--;
				}
			}
			goto IL_02ea;
			IL_02cd:
			if (!r_Suffix_Noun_Step3())
			{
				goto IL_02d5;
			}
			goto IL_02ea;
			IL_0118:
			base.cursor = limit - num3;
			if (!r_Suffix_Verb_Step2b())
			{
				base.cursor = limit - num3;
				if (!r_Suffix_Verb_Step2a())
				{
					goto IL_014a;
				}
			}
			goto IL_02ea;
			IL_02ea:
			base.cursor = limit - num;
			base.cursor = limit_backward;
			int cursor2 = base.cursor;
			int cursor3 = base.cursor;
			if (!r_Prefix_Step1())
			{
				base.cursor = cursor3;
			}
			int cursor4 = base.cursor;
			if (!r_Prefix_Step2())
			{
				base.cursor = cursor4;
			}
			int cursor5 = base.cursor;
			if (!r_Prefix_Step3a_Noun())
			{
				base.cursor = cursor5;
				if (!B_is_noun || !r_Prefix_Step3b_Noun())
				{
					base.cursor = cursor5;
					if (B_is_verb)
					{
						int cursor6 = base.cursor;
						if (!r_Prefix_Step3_Verb())
						{
							base.cursor = cursor6;
						}
						r_Prefix_Step4_Verb();
					}
				}
			}
			base.cursor = cursor2;
			r_Normalize_post();
			return true;
			IL_02d5:
			base.cursor = limit - num2;
			r_Suffix_All_alef_maqsura();
			goto IL_02ea;
			IL_014a:
			base.cursor = limit - num2;
			int num7;
			int num8;
			if (B_is_noun)
			{
				num7 = limit - base.cursor;
				num8 = limit - base.cursor;
				if (!r_Suffix_Noun_Step2c2())
				{
					base.cursor = limit - num8;
					if (B_is_defined || !r_Suffix_Noun_Step1a())
					{
						goto IL_022c;
					}
					int num9 = limit - base.cursor;
					if (!r_Suffix_Noun_Step2a())
					{
						base.cursor = limit - num9;
						if (!r_Suffix_Noun_Step2b())
						{
							base.cursor = limit - num9;
							if (!r_Suffix_Noun_Step2c1())
							{
								base.cursor = limit - num9;
								if (base.cursor <= limit_backward)
								{
									goto IL_022c;
								}
								base.cursor--;
							}
						}
					}
				}
				goto IL_02cd;
			}
			goto IL_02d5;
			IL_022c:
			base.cursor = limit - num8;
			if (!r_Suffix_Noun_Step1b())
			{
				goto IL_0288;
			}
			int num10 = limit - base.cursor;
			if (!r_Suffix_Noun_Step2a())
			{
				base.cursor = limit - num10;
				if (!r_Suffix_Noun_Step2b())
				{
					base.cursor = limit - num10;
					if (!r_Suffix_Noun_Step2c1())
					{
						goto IL_0288;
					}
				}
			}
			goto IL_02cd;
			IL_0288:
			base.cursor = limit - num8;
			if (B_is_defined || !r_Suffix_Noun_Step2a())
			{
				base.cursor = limit - num8;
				if (!r_Suffix_Noun_Step2b())
				{
					base.cursor = limit - num7;
				}
			}
			goto IL_02cd;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("ar", () => new Stemmer_ar(), -450746484);
				}
				_registered = true;
			}
		}
	}
}
