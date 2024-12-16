using Sdl.Core.LanguageProcessing.Stemmers;
using System.CodeDom.Compiler;

namespace Snowball
{
	[GeneratedCode("Snowball", "1.0.0")]
	public class Stemmer_ne : Stemmer
	{
		private readonly Among[] a_0;

		private readonly Among[] a_1;

		private readonly Among[] a_2;

		private readonly Among[] a_3;

		private static bool _registered;

		private static readonly object _locker = new object();

		public Stemmer_ne()
		{
			a_0 = new Among[17]
			{
				new Among("ल\u093eइ", -1, 1),
				new Among("ल\u093eई", -1, 1),
				new Among("स\u0901ग", -1, 1),
				new Among("स\u0902ग", -1, 1),
				new Among("म\u093eर\u094dफत", -1, 1),
				new Among("रत", -1, 1),
				new Among("क\u093e", -1, 2),
				new Among("म\u093e", -1, 1),
				new Among("द\u094dव\u093eर\u093e", -1, 1),
				new Among("क\u093f", -1, 2),
				new Among("पछ\u093f", -1, 1),
				new Among("क\u0940", -1, 2),
				new Among("ल\u0947", -1, 1),
				new Among("क\u0948", -1, 2),
				new Among("स\u0901ग\u0948", -1, 1),
				new Among("म\u0948", -1, 1),
				new Among("क\u094b", -1, 2)
			};
			a_1 = new Among[3]
			{
				new Among("\u0901", -1, -1),
				new Among("\u0902", -1, -1),
				new Among("\u0948", -1, -1)
			};
			a_2 = new Among[3]
			{
				new Among("\u0901", -1, 1),
				new Among("\u0902", -1, 1),
				new Among("\u0948", -1, 2)
			};
			a_3 = new Among[91]
			{
				new Among("थ\u093fए", -1, 1),
				new Among("छ", -1, 1),
				new Among("इछ", 1, 1),
				new Among("एछ", 1, 1),
				new Among("\u093fछ", 1, 1),
				new Among("\u0947छ", 1, 1),
				new Among("न\u0947छ", 5, 1),
				new Among("ह\u0941न\u0947छ", 6, 1),
				new Among("इन\u094dछ", 1, 1),
				new Among("\u093fन\u094dछ", 1, 1),
				new Among("ह\u0941न\u094dछ", 1, 1),
				new Among("एक\u093e", -1, 1),
				new Among("इएक\u093e", 11, 1),
				new Among("\u093fएक\u093e", 11, 1),
				new Among("\u0947क\u093e", -1, 1),
				new Among("न\u0947क\u093e", 14, 1),
				new Among("द\u093e", -1, 1),
				new Among("इद\u093e", 16, 1),
				new Among("\u093fद\u093e", 16, 1),
				new Among("द\u0947ख\u093f", -1, 1),
				new Among("म\u093eथ\u093f", -1, 1),
				new Among("एक\u0940", -1, 1),
				new Among("इएक\u0940", 21, 1),
				new Among("\u093fएक\u0940", 21, 1),
				new Among("\u0947क\u0940", -1, 1),
				new Among("द\u0947ख\u0940", -1, 1),
				new Among("थ\u0940", -1, 1),
				new Among("द\u0940", -1, 1),
				new Among("छ\u0941", -1, 1),
				new Among("एछ\u0941", 28, 1),
				new Among("\u0947छ\u0941", 28, 1),
				new Among("न\u0947छ\u0941", 30, 1),
				new Among("न\u0941", -1, 1),
				new Among("हर\u0941", -1, 1),
				new Among("हर\u0942", -1, 1),
				new Among("छ\u0947", -1, 1),
				new Among("थ\u0947", -1, 1),
				new Among("न\u0947", -1, 1),
				new Among("एक\u0948", -1, 1),
				new Among("\u0947क\u0948", -1, 1),
				new Among("न\u0947क\u0948", 39, 1),
				new Among("द\u0948", -1, 1),
				new Among("इद\u0948", 41, 1),
				new Among("\u093fद\u0948", 41, 1),
				new Among("एक\u094b", -1, 1),
				new Among("इएक\u094b", 44, 1),
				new Among("\u093fएक\u094b", 44, 1),
				new Among("\u0947क\u094b", -1, 1),
				new Among("न\u0947क\u094b", 47, 1),
				new Among("द\u094b", -1, 1),
				new Among("इद\u094b", 49, 1),
				new Among("\u093fद\u094b", 49, 1),
				new Among("य\u094b", -1, 1),
				new Among("इय\u094b", 52, 1),
				new Among("भय\u094b", 52, 1),
				new Among("\u093fय\u094b", 52, 1),
				new Among("थ\u093fय\u094b", 55, 1),
				new Among("द\u093fय\u094b", 55, 1),
				new Among("थ\u094dय\u094b", 52, 1),
				new Among("छ\u094c", -1, 1),
				new Among("इछ\u094c", 59, 1),
				new Among("एछ\u094c", 59, 1),
				new Among("\u093fछ\u094c", 59, 1),
				new Among("\u0947छ\u094c", 59, 1),
				new Among("न\u0947छ\u094c", 63, 1),
				new Among("य\u094c", -1, 1),
				new Among("थ\u093fय\u094c", 65, 1),
				new Among("छ\u094dय\u094c", 65, 1),
				new Among("थ\u094dय\u094c", 65, 1),
				new Among("छन\u094d", -1, 1),
				new Among("इछन\u094d", 69, 1),
				new Among("एछन\u094d", 69, 1),
				new Among("\u093fछन\u094d", 69, 1),
				new Among("\u0947छन\u094d", 69, 1),
				new Among("न\u0947छन\u094d", 73, 1),
				new Among("ल\u093eन\u094d", -1, 1),
				new Among("छ\u093fन\u094d", -1, 1),
				new Among("थ\u093fन\u094d", -1, 1),
				new Among("पर\u094d", -1, 1),
				new Among("इस\u094d", -1, 1),
				new Among("थ\u093fइस\u094d", 79, 1),
				new Among("छस\u094d", -1, 1),
				new Among("इछस\u094d", 81, 1),
				new Among("एछस\u094d", 81, 1),
				new Among("\u093fछस\u094d", 81, 1),
				new Among("\u0947छस\u094d", 81, 1),
				new Among("न\u0947छस\u094d", 85, 1),
				new Among("\u093fस\u094d", -1, 1),
				new Among("थ\u093fस\u094d", 87, 1),
				new Among("छ\u0947स\u094d", -1, 1),
				new Among("ह\u094bस\u094d", -1, 1)
			};
		}

		private bool r_remove_category_1()
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
				slice_del();
				break;
			case 2:
			{
				int num2 = limit - cursor;
				int num3 = limit - cursor;
				if (!eq_s_b("ए"))
				{
					cursor = limit - num3;
					if (!eq_s_b("\u0947"))
					{
						cursor = limit - num2;
						slice_del();
					}
				}
				break;
			}
			}
			return true;
		}

		private bool r_check_category_2()
		{
			ket = cursor;
			if (find_among_b(a_1) == 0)
			{
				return false;
			}
			bra = cursor;
			return true;
		}

		private bool r_remove_category_2()
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
			{
				int num2 = limit - cursor;
				if (!eq_s_b("य\u094c"))
				{
					cursor = limit - num2;
					if (!eq_s_b("छ\u094c"))
					{
						cursor = limit - num2;
						if (!eq_s_b("न\u094c"))
						{
							cursor = limit - num2;
							if (!eq_s_b("थ\u0947"))
							{
								return false;
							}
						}
					}
				}
				slice_del();
				break;
			}
			case 2:
				if (!eq_s_b("त\u094dर"))
				{
					return false;
				}
				slice_del();
				break;
			}
			return true;
		}

		private bool r_remove_category_3()
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

		protected override bool stem()
		{
			limit_backward = cursor;
			cursor = limit;
			int num = limit - cursor;
			r_remove_category_1();
			cursor = limit - num;
			int num2 = limit - cursor;
			int num3;
			do
			{
				num3 = limit - cursor;
				int num4 = limit - cursor;
				int num5 = limit - cursor;
				if (r_check_category_2())
				{
					cursor = limit - num5;
					r_remove_category_2();
				}
				cursor = limit - num4;
			}
			while (r_remove_category_3());
			cursor = limit - num3;
			cursor = limit - num2;
			cursor = limit_backward;
			return true;
		}

		private static void Register()
		{
			lock (_locker)
			{
				if (!_registered)
				{
					StemmerFactory.Register("ne", () => new Stemmer_ne(), 379243777);
				}
				_registered = true;
			}
		}
	}
}
