using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_id : Stemmer
	{
		private int I_prefix;

		private int I_measure;

		private static string g_vowel = "aeiou";

		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private readonly Among[] a_4;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_id()
		{
			a_0 = new Among[3]
			{
				new Among("kah", -1, 1),
				new Among("lah", -1, 1),
				new Among("pun", -1, 1)
			};
			a_1 = new Among[3]
			{
				new Among("nya", -1, 1),
				new Among("ku", -1, 1),
				new Among("mu", -1, 1)
			};
			a_2 = new Among[3]
			{
				new Among("i", -1, 1, r_SUFFIX_I_OK),
				new Among("an", -1, 1, r_SUFFIX_AN_OK),
				new Among("kan", 1, 1, r_SUFFIX_KAN_OK)
			};
			a_3 = new Among[12]
			{
				new Among("di", -1, 1),
				new Among("ke", -1, 2),
				new Among("me", -1, 1),
				new Among("mem", 2, 5),
				new Among("men", 2, 1),
				new Among("meng", 4, 1),
				new Among("meny", 4, 3, r_VOWEL),
				new Among("pem", -1, 6),
				new Among("pen", -1, 2),
				new Among("peng", 8, 2),
				new Among("peny", 8, 4, r_VOWEL),
				new Among("ter", -1, 1)
			};
			a_4 = new Among[6]
			{
				new Among("be", -1, 3, r_KER),
				new Among("belajar", 0, 4),
				new Among("ber", 0, 3),
				new Among("pe", -1, 1),
				new Among("pelajar", 3, 2),
				new Among("per", 3, 1)
			};
		}

		private bool r_remove_particle()
		{
			ket = cursor;
			if (find_among_b(a_0) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			I_measure--;
			return true;
		}

		private bool r_remove_possessive_pronoun()
		{
			ket = cursor;
			if (find_among_b(a_1) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			I_measure--;
			return true;
		}

		private bool r_SUFFIX_KAN_OK()
		{
			if (I_prefix != 2)
			{
				return I_prefix != 3;
			}
			return false;
		}

		private bool r_SUFFIX_AN_OK()
		{
			return I_prefix != 1;
		}

		private bool r_SUFFIX_I_OK()
		{
			if (I_prefix > 2)
			{
				return false;
			}
			int num = limit - cursor;
			if (eq_s_b("s"))
			{
				return false;
			}
			cursor = limit - num;
			return true;
		}

		private bool r_remove_suffix()
		{
			ket = cursor;
			if (find_among_b(a_2) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			I_measure--;
			return true;
		}

		private bool r_VOWEL()
		{
			return in_grouping(g_vowel, 97, 117, repeat: false) == 0;
		}

		private bool r_KER()
		{
			if (out_grouping(g_vowel, 97, 117, repeat: false) != 0)
			{
				return false;
			}
			return eq_s("er");
		}

		private bool r_remove_first_order_prefix()
		{
			bra = base.cursor;
			int num = find_among(a_3);
			if (num == 0)
			{
				return false;
			}
			ket = base.cursor;
			switch (num)
			{
			case 1:
				slice_del();
				I_prefix = 1;
				I_measure--;
				break;
			case 2:
				slice_del();
				I_prefix = 3;
				I_measure--;
				break;
			case 3:
				I_prefix = 1;
				slice_from("s");
				I_measure--;
				break;
			case 4:
				I_prefix = 3;
				slice_from("s");
				I_measure--;
				break;
			case 5:
			{
				I_prefix = 1;
				I_measure--;
				int cursor3 = base.cursor;
				int cursor4 = base.cursor;
				if (in_grouping(g_vowel, 97, 117, repeat: false) == 0)
				{
					base.cursor = cursor4;
					slice_from("p");
				}
				else
				{
					base.cursor = cursor3;
					slice_del();
				}
				break;
			}
			case 6:
			{
				I_prefix = 3;
				I_measure--;
				int cursor = base.cursor;
				int cursor2 = base.cursor;
				if (in_grouping(g_vowel, 97, 117, repeat: false) == 0)
				{
					base.cursor = cursor2;
					slice_from("p");
				}
				else
				{
					base.cursor = cursor;
					slice_del();
				}
				break;
			}
			}
			return true;
		}

		private bool r_remove_second_order_prefix()
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
				slice_del();
				I_prefix = 2;
				I_measure--;
				break;
			case 2:
				slice_from("ajar");
				I_measure--;
				break;
			case 3:
				slice_del();
				I_prefix = 4;
				I_measure--;
				break;
			case 4:
				slice_from("ajar");
				I_prefix = 4;
				I_measure--;
				break;
			}
			return true;
		}

		protected override bool stem()
		{
			I_measure = 0;
			int cursor = base.cursor;
			int cursor2;
			while (true)
			{
				cursor2 = base.cursor;
				int num = out_grouping(g_vowel, 97, 117, repeat: true);
				if (num < 0)
				{
					break;
				}
				base.cursor += num;
				I_measure++;
			}
			base.cursor = cursor2;
			base.cursor = cursor;
			if (I_measure <= 2)
			{
				return false;
			}
			I_prefix = 0;
			limit_backward = base.cursor;
			base.cursor = limit;
			int num2 = limit - base.cursor;
			r_remove_particle();
			base.cursor = limit - num2;
			if (I_measure <= 2)
			{
				return false;
			}
			int num3 = limit - base.cursor;
			r_remove_possessive_pronoun();
			base.cursor = limit - num3;
			base.cursor = limit_backward;
			if (I_measure <= 2)
			{
				return false;
			}
			int cursor3 = base.cursor;
			int cursor4 = base.cursor;
			if (r_remove_first_order_prefix())
			{
				int cursor5 = base.cursor;
				int cursor6 = base.cursor;
				if (I_measure > 2)
				{
					limit_backward = base.cursor;
					base.cursor = limit;
					if (r_remove_suffix())
					{
						base.cursor = limit_backward;
						base.cursor = cursor6;
						if (I_measure > 2)
						{
							r_remove_second_order_prefix();
						}
					}
				}
				base.cursor = cursor5;
				base.cursor = cursor4;
			}
			else
			{
				base.cursor = cursor3;
				int cursor7 = base.cursor;
				r_remove_second_order_prefix();
				base.cursor = cursor7;
				int cursor8 = base.cursor;
				if (I_measure > 2)
				{
					limit_backward = base.cursor;
					base.cursor = limit;
					if (r_remove_suffix())
					{
						base.cursor = limit_backward;
					}
				}
				base.cursor = cursor8;
			}
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("id", () => new Stemmer_id(), 1890649462);
				}
				_registered = true;
			}
		}
	}
}
