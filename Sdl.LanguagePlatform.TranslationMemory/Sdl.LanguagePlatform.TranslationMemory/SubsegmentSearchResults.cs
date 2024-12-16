using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SubsegmentSearchResults : IEnumerable<SubsegmentSearchResult>, IEnumerable
	{
		private readonly Dictionary<string, SubsegmentSearchResult> _dedupDictionary = new Dictionary<string, SubsegmentSearchResult>();

		private readonly List<SubsegmentSearchResult> _results = new List<SubsegmentSearchResult>();

		public int Count => _results.Count;

		public short SourceFeatureStartIndex
		{
			get;
		}

		public short SourceFeatureCount
		{
			get;
		}

		public short SignificantFeatureCount
		{
			get;
		}

		public SubsegmentSearchResults(short sourceFeatureStartIndex, short sourceFeatureCount, short significantFeatureCount)
		{
			SourceFeatureStartIndex = sourceFeatureStartIndex;
			SourceFeatureCount = sourceFeatureCount;
			SignificantFeatureCount = significantFeatureCount;
		}

		public void Remove(SubsegmentSearchResult result)
		{
			string dedupString = GetDedupString(result);
			if (!_dedupDictionary.ContainsKey(dedupString))
			{
				throw new Exception("The result is not in the collection");
			}
			_results.Remove(result);
			_dedupDictionary.Remove(dedupString);
		}

		public int RemoveAll(Func<SubsegmentSearchResult, bool> predicate)
		{
			List<SubsegmentSearchResult> list = _results.Where(predicate).ToList();
			list.ForEach(Remove);
			return list.Count;
		}

		public void Add(SubsegmentSearchResult result)
		{
			if (result == null)
			{
				return;
			}
			string dedupString = GetDedupString(result);
			if (_dedupDictionary.TryGetValue(dedupString, out SubsegmentSearchResult value))
			{
				if (result.ScoringResult.Match <= value.ScoringResult.Match)
				{
					value.Repetitions++;
					return;
				}
				int index = _results.IndexOf(value);
				_results.RemoveAt(index);
				_dedupDictionary.Remove(dedupString);
				result.Repetitions += value.Repetitions;
			}
			_dedupDictionary.Add(dedupString, result);
			_results.Add(result);
		}

		private static string GetDedupString(SubsegmentSearchResult result)
		{
			return result.MatchType.ToString() + "-" + result.TranslationFeatureString;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _results.GetEnumerator();
		}

		IEnumerator<SubsegmentSearchResult> IEnumerable<SubsegmentSearchResult>.GetEnumerator()
		{
			return _results.GetEnumerator();
		}
	}
}
