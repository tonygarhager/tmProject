using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_ru : Stemmer
	{
		private int I_p2;

		private int I_pV;

		private static string g_v = "аеиоуыэюя";

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

		public Stemmer_ru()
		{
			a_0 = new Among[9]
			{
				new Among("в", -1, 1),
				new Among("ив", 0, 2),
				new Among("ыв", 0, 2),
				new Among("вши", -1, 1),
				new Among("ивши", 3, 2),
				new Among("ывши", 3, 2),
				new Among("вшись", -1, 1),
				new Among("ившись", 6, 2),
				new Among("ывшись", 6, 2)
			};
			a_1 = new Among[26]
			{
				new Among("ее", -1, 1),
				new Among("ие", -1, 1),
				new Among("ое", -1, 1),
				new Among("ые", -1, 1),
				new Among("ими", -1, 1),
				new Among("ыми", -1, 1),
				new Among("ей", -1, 1),
				new Among("ий", -1, 1),
				new Among("ой", -1, 1),
				new Among("ый", -1, 1),
				new Among("ем", -1, 1),
				new Among("им", -1, 1),
				new Among("ом", -1, 1),
				new Among("ым", -1, 1),
				new Among("его", -1, 1),
				new Among("ого", -1, 1),
				new Among("ему", -1, 1),
				new Among("ому", -1, 1),
				new Among("их", -1, 1),
				new Among("ых", -1, 1),
				new Among("ею", -1, 1),
				new Among("ою", -1, 1),
				new Among("ую", -1, 1),
				new Among("юю", -1, 1),
				new Among("ая", -1, 1),
				new Among("яя", -1, 1)
			};
			a_2 = new Among[8]
			{
				new Among("ем", -1, 1),
				new Among("нн", -1, 1),
				new Among("вш", -1, 1),
				new Among("ивш", 2, 2),
				new Among("ывш", 2, 2),
				new Among("щ", -1, 1),
				new Among("ющ", 5, 1),
				new Among("ующ", 6, 2)
			};
			a_3 = new Among[2]
			{
				new Among("сь", -1, 1),
				new Among("ся", -1, 1)
			};
			a_4 = new Among[46]
			{
				new Among("ла", -1, 1),
				new Among("ила", 0, 2),
				new Among("ыла", 0, 2),
				new Among("на", -1, 1),
				new Among("ена", 3, 2),
				new Among("ете", -1, 1),
				new Among("ите", -1, 2),
				new Among("йте", -1, 1),
				new Among("ейте", 7, 2),
				new Among("уйте", 7, 2),
				new Among("ли", -1, 1),
				new Among("или", 10, 2),
				new Among("ыли", 10, 2),
				new Among("й", -1, 1),
				new Among("ей", 13, 2),
				new Among("уй", 13, 2),
				new Among("л", -1, 1),
				new Among("ил", 16, 2),
				new Among("ыл", 16, 2),
				new Among("ем", -1, 1),
				new Among("им", -1, 2),
				new Among("ым", -1, 2),
				new Among("н", -1, 1),
				new Among("ен", 22, 2),
				new Among("ло", -1, 1),
				new Among("ило", 24, 2),
				new Among("ыло", 24, 2),
				new Among("но", -1, 1),
				new Among("ено", 27, 2),
				new Among("нно", 27, 1),
				new Among("ет", -1, 1),
				new Among("ует", 30, 2),
				new Among("ит", -1, 2),
				new Among("ыт", -1, 2),
				new Among("ют", -1, 1),
				new Among("уют", 34, 2),
				new Among("ят", -1, 2),
				new Among("ны", -1, 1),
				new Among("ены", 37, 2),
				new Among("ть", -1, 1),
				new Among("ить", 39, 2),
				new Among("ыть", 39, 2),
				new Among("ешь", -1, 1),
				new Among("ишь", -1, 2),
				new Among("ю", -1, 2),
				new Among("ую", 44, 2)
			};
			a_5 = new Among[36]
			{
				new Among("а", -1, 1),
				new Among("ев", -1, 1),
				new Among("ов", -1, 1),
				new Among("е", -1, 1),
				new Among("ие", 3, 1),
				new Among("ье", 3, 1),
				new Among("и", -1, 1),
				new Among("еи", 6, 1),
				new Among("ии", 6, 1),
				new Among("ами", 6, 1),
				new Among("ями", 6, 1),
				new Among("иями", 10, 1),
				new Among("й", -1, 1),
				new Among("ей", 12, 1),
				new Among("ией", 13, 1),
				new Among("ий", 12, 1),
				new Among("ой", 12, 1),
				new Among("ам", -1, 1),
				new Among("ем", -1, 1),
				new Among("ием", 18, 1),
				new Among("ом", -1, 1),
				new Among("ям", -1, 1),
				new Among("иям", 21, 1),
				new Among("о", -1, 1),
				new Among("у", -1, 1),
				new Among("ах", -1, 1),
				new Among("ях", -1, 1),
				new Among("иях", 26, 1),
				new Among("ы", -1, 1),
				new Among("ь", -1, 1),
				new Among("ю", -1, 1),
				new Among("ию", 30, 1),
				new Among("ью", 30, 1),
				new Among("я", -1, 1),
				new Among("ия", 33, 1),
				new Among("ья", 33, 1)
			};
			a_6 = new Among[2]
			{
				new Among("ост", -1, 1),
				new Among("ость", -1, 1)
			};
			a_7 = new Among[4]
			{
				new Among("ейше", -1, 1),
				new Among("н", -1, 2),
				new Among("ейш", -1, 1),
				new Among("ь", -1, 3)
			};
		}

		private bool r_mark_regions()
		{
			I_pV = limit;
			I_p2 = limit;
			int cursor = base.cursor;
			int num = out_grouping(g_v, 1072, 1103, repeat: true);
			if (num >= 0)
			{
				base.cursor += num;
				I_pV = base.cursor;
				int num2 = in_grouping(g_v, 1072, 1103, repeat: true);
				if (num2 >= 0)
				{
					base.cursor += num2;
					int num3 = out_grouping(g_v, 1072, 1103, repeat: true);
					if (num3 >= 0)
					{
						base.cursor += num3;
						int num4 = in_grouping(g_v, 1072, 1103, repeat: true);
						if (num4 >= 0)
						{
							base.cursor += num4;
							I_p2 = base.cursor;
						}
					}
				}
			}
			base.cursor = cursor;
			return true;
		}

		private bool r_R2()
		{
			return I_p2 <= cursor;
		}

		private bool r_perfective_gerund()
		{
			ket = cursor;
			int num = find_among_b(a_0);
			if (num == 0)
			{
				return false;
			}
			bra = cursor;
			switch (num)
			{
			case 1:
			{
				int num2 = limit - cursor;
				if (!eq_s_b("а"))
				{
					cursor = limit - num2;
					if (!eq_s_b("я"))
					{
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
			return true;
		}

		private bool r_adjective()
		{
			ket = cursor;
			if (find_among_b(a_1) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_adjectival()
		{
			if (!r_adjective())
			{
				return false;
			}
			int num = limit - cursor;
			ket = cursor;
			int num2 = find_among_b(a_2);
			if (num2 == 0)
			{
				cursor = limit - num;
			}
			else
			{
				bra = cursor;
				switch (num2)
				{
				case 1:
				{
					int num3 = limit - cursor;
					if (!eq_s_b("а"))
					{
						cursor = limit - num3;
						if (!eq_s_b("я"))
						{
							cursor = limit - num;
							break;
						}
					}
					slice_del();
					break;
				}
				case 2:
					slice_del();
					break;
				}
			}
			return true;
		}

		private bool r_reflexive()
		{
			ket = cursor;
			if (find_among_b(a_3) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_verb()
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
			{
				int num2 = limit - cursor;
				if (!eq_s_b("а"))
				{
					cursor = limit - num2;
					if (!eq_s_b("я"))
					{
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
			return true;
		}

		private bool r_noun()
		{
			ket = cursor;
			if (find_among_b(a_5) == 0)
			{
				return false;
			}
			bra = cursor;
			slice_del();
			return true;
		}

		private bool r_derivational()
		{
			ket = cursor;
			if (find_among_b(a_6) == 0)
			{
				return false;
			}
			bra = cursor;
			if (!r_R2())
			{
				return false;
			}
			slice_del();
			return true;
		}

		private bool r_tidy_up()
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
				slice_del();
				ket = cursor;
				if (!eq_s_b("н"))
				{
					return false;
				}
				bra = cursor;
				if (!eq_s_b("н"))
				{
					return false;
				}
				slice_del();
				break;
			case 2:
				if (!eq_s_b("н"))
				{
					return false;
				}
				slice_del();
				break;
			case 3:
				slice_del();
				break;
			}
			return true;
		}

		protected override bool stem()
		{
			int cursor = base.cursor;
			while (true)
			{
				int cursor2 = base.cursor;
				int cursor3;
				while (true)
				{
					cursor3 = base.cursor;
					bra = base.cursor;
					if (eq_s("ё"))
					{
						break;
					}
					base.cursor = cursor3;
					if (base.cursor < limit)
					{
						base.cursor++;
						continue;
					}
					base.cursor = cursor2;
					base.cursor = cursor;
					r_mark_regions();
					base.limit_backward = base.cursor;
					base.cursor = limit;
					if (base.cursor < I_pV)
					{
						return false;
					}
					int limit_backward = base.limit_backward;
					base.limit_backward = I_pV;
					int num = limit - base.cursor;
					int num2 = limit - base.cursor;
					if (!r_perfective_gerund())
					{
						base.cursor = limit - num2;
						int num3 = limit - base.cursor;
						if (!r_reflexive())
						{
							base.cursor = limit - num3;
						}
						int num4 = limit - base.cursor;
						if (!r_adjectival())
						{
							base.cursor = limit - num4;
							if (!r_verb())
							{
								base.cursor = limit - num4;
								r_noun();
							}
						}
					}
					base.cursor = limit - num;
					int num5 = limit - base.cursor;
					ket = base.cursor;
					if (!eq_s_b("и"))
					{
						base.cursor = limit - num5;
					}
					else
					{
						bra = base.cursor;
						slice_del();
					}
					int num6 = limit - base.cursor;
					r_derivational();
					base.cursor = limit - num6;
					int num7 = limit - base.cursor;
					r_tidy_up();
					base.cursor = limit - num7;
					base.limit_backward = limit_backward;
					base.cursor = base.limit_backward;
					return true;
				}
				ket = base.cursor;
				base.cursor = cursor3;
				slice_from("е");
			}
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("ru", () => new Stemmer_ru(), -1201657710);
				}
				_registered = true;
			}
		}
	}
}
