using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SortSpecification
	{
		[DataMember]
		public List<SortCriterium> Criteria
		{
			get;
			private set;
		}

		public int Count => Criteria.Count;

		public SortCriterium this[int index]
		{
			get
			{
				return Criteria[index];
			}
			set
			{
				Criteria[index] = value;
			}
		}

		public SortSpecification()
		{
			Criteria = new List<SortCriterium>();
		}

		public SortSpecification(string sortSpecification)
		{
			Criteria = new List<SortCriterium>();
			if (string.IsNullOrEmpty(sortSpecification))
			{
				return;
			}
			string[] array = sortSpecification.Split(new char[1]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			while (true)
			{
				if (num >= array.Length)
				{
					return;
				}
				string text = array[num];
				int num2 = text.IndexOf('/');
				if (num2 > 0)
				{
					string text2 = text.Substring(num2 + 1).ToLower(CultureInfo.InvariantCulture);
					string text3 = text.Substring(0, num2).ToLower(CultureInfo.InvariantCulture);
					if (text2.Length <= 0 || text3.Length <= 0 || (!text2.StartsWith("d", StringComparison.OrdinalIgnoreCase) && !text2.StartsWith("a", StringComparison.OrdinalIgnoreCase)))
					{
						break;
					}
					Criteria.Add(new SortCriterium(text3, text2.StartsWith("d", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending));
				}
				num++;
			}
			throw new LanguagePlatformException(ErrorCode.TMInvalidSortSpecification);
		}

		public void Add(SortCriterium sc)
		{
			Criteria.Add(sc);
		}
	}
}
