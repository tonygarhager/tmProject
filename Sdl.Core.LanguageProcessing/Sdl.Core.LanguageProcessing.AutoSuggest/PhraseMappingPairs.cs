using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Sdl.Core.LanguageProcessing.AutoSuggest
{
	public class PhraseMappingPairs : Collection<PhraseMappingPair>
	{
		private const int DefaultMaximumSourceCount = 3;

		private const int MaximumSourceCount = 3;

		private readonly Dictionary<Segment, int> _sourceCount = new Dictionary<Segment, int>();

		public PhraseMappingPairs()
		{
		}

		public PhraseMappingPairs(IList<PhraseMappingPair> phraseMappingPairs)
			: base(phraseMappingPairs)
		{
		}

		public void Add(Segment source, Segment target, int frequency)
		{
			Add(new PhraseMappingPair(source, target, frequency));
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			_sourceCount.Clear();
		}

		protected override void InsertItem(int index, PhraseMappingPair item)
		{
			Segment source = item.Source;
			int sourceCount = GetSourceCount(source);
			if (sourceCount < 3)
			{
				base.InsertItem(index, item);
				IncrementSourceCount(source);
			}
		}

		protected override void RemoveItem(int index)
		{
			PhraseMappingPair phraseMappingPair = base[index];
			Segment source = phraseMappingPair.Source;
			int sourceCount = GetSourceCount(source);
			if (sourceCount > 0)
			{
				DecrementSourceCount(source);
			}
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, PhraseMappingPair item)
		{
			throw new NotSupportedException();
		}

		private int GetSourceCount(Segment source)
		{
			EnsureSourceCountExists(source);
			return _sourceCount[source];
		}

		private void IncrementSourceCount(Segment source)
		{
			EnsureSourceCountExists(source);
			_sourceCount[source]++;
		}

		private void DecrementSourceCount(Segment source)
		{
			EnsureSourceCountExists(source);
			_sourceCount[source]--;
		}

		private void EnsureSourceCountExists(Segment source)
		{
			if (!_sourceCount.TryGetValue(source, out int _))
			{
				_sourceCount[source] = 0;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (IEnumerator<PhraseMappingPair> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PhraseMappingPair current = enumerator.Current;
					stringBuilder.Append(current.Source);
					stringBuilder.Append("->");
					stringBuilder.Append(current.Target);
					stringBuilder.Append("(");
					stringBuilder.Append(current.Frequency);
					stringBuilder.Append(")");
					stringBuilder.Append(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
