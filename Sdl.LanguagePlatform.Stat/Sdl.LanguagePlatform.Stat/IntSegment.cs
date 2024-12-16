using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class IntSegment
	{
		public int this[int index]
		{
			get
			{
				return Elements[index];
			}
			set
			{
				Elements[index] = value;
			}
		}

		public int Count => Elements.Count;

		public List<int> Elements
		{
			get;
			private set;
		}

		public IntSegment()
			: this(null)
		{
		}

		public IntSegment(List<int> elements)
		{
			Elements = new List<int>();
			if (elements != null)
			{
				Elements.AddRange(elements);
			}
		}

		public bool Verify(int maxVoc)
		{
			foreach (int element in Elements)
			{
				if (element >= maxVoc)
				{
					return false;
				}
			}
			return true;
		}

		public int IndexOf(IntSegment other)
		{
			return IndexOf(other.Elements, 0);
		}

		public int IndexOf(IntSegment other, int startAt)
		{
			return IndexOf(other.Elements, startAt);
		}

		public int IndexOf(IList<int> elements)
		{
			return IndexOf(elements, 0);
		}

		public int IndexOf(IList<int> elements, int startAt)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			if (startAt + elements.Count > Elements.Count)
			{
				return -1;
			}
			int num = Elements.Count - elements.Count;
			if (elements.Count == 0)
			{
				return startAt;
			}
			int num2;
			while (true)
			{
				num2 = Elements.IndexOf(elements[0], startAt);
				if (num2 < 0 || num2 > num)
				{
					return -1;
				}
				bool flag = true;
				for (int i = 0; i < elements.Count; i++)
				{
					if (!flag)
					{
						break;
					}
					if (elements[i] != Elements[num2 + i])
					{
						flag = false;
					}
				}
				if (flag)
				{
					break;
				}
				startAt++;
			}
			return num2;
		}

		public void Dump(VocabularyFile v, TextWriter stream)
		{
			Dump(0, Elements.Count - 1, v, stream);
		}

		public void Dump(int from, int into, VocabularyFile v, TextWriter stream)
		{
			int num = (into >= Elements.Count) ? Elements.Count : (into + 1);
			for (int i = from; i < num; i++)
			{
				if (i > from)
				{
					stream.Write(" ");
				}
				stream.Write(v.Lookup(Elements[i]));
			}
		}

		public void Uniq()
		{
			if (Elements.Count <= 1)
			{
				return;
			}
			Elements.Sort();
			List<int> list = new List<int>();
			int num = 0;
			for (int i = 0; i < Elements.Count; i++)
			{
				if (i == 0 || Elements[i] != num)
				{
					num = Elements[i];
					list.Add(num);
				}
			}
			Elements = list;
		}
	}
}
