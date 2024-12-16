using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class Sorter<T> : IComparer<T>
	{
		private readonly IFieldValueComparer<T> _comparer;

		private readonly SortSpecification _sortSpecification;

		private readonly SortDisambiguator _sortDisambiguator;

		public Sorter(IFieldValueComparer<T> comparer, SortSpecification sortSpecification)
			: this(comparer, sortSpecification, (SortDisambiguator)null)
		{
		}

		public Sorter(IFieldValueComparer<T> comparer, SortSpecification sortSpecification, SortDisambiguator sortDisambiguator)
		{
			if (comparer == null || sortSpecification == null)
			{
				throw new ArgumentNullException();
			}
			if (sortSpecification.Count == 0)
			{
				throw new ArgumentException("Sort specification doesn't contain any sort criteria");
			}
			_comparer = comparer;
			_sortSpecification = sortSpecification;
			_sortDisambiguator = sortDisambiguator;
		}

		public int Compare(T x, T y)
		{
			int num = 0;
			for (int i = 0; i < _sortSpecification.Count; i++)
			{
				if (num != 0)
				{
					break;
				}
				num = _comparer.Compare(x, y, _sortSpecification[i].FieldName);
				if (_sortSpecification[i].Direction == SortDirection.Descending)
				{
					num = -num;
				}
			}
			if (_sortDisambiguator != null)
			{
				num = _sortDisambiguator.Disambiguate(num, x as SearchResult, y as SearchResult);
			}
			return num;
		}
	}
}
