using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SearchResults : IEnumerable<SearchResult>, IEnumerable
	{
		public delegate int SearchResultComparer(SearchResult a, SearchResult b);

		private List<SearchResult> _Results;

		private SortSpecification _SortOrder;

		public static readonly string DefaultSortOrder = "Sco/D ChD/D UsC/D";

		public static readonly string DefaultSortOrderConcordance = "Sco/D ChD/D UsC/D";

		[DataMember]
		public List<SearchResult> Results
		{
			get
			{
				return _Results;
			}
			set
			{
				_Results = (value ?? throw new ArgumentNullException());
			}
		}

		[DataMember]
		public SortSpecification SortOrder
		{
			get
			{
				return _SortOrder;
			}
			set
			{
				_SortOrder = (value ?? GetDefaultSortOrder());
			}
		}

		[DataMember]
		public bool MultipleTranslations
		{
			get;
			set;
		}

		[DataMember]
		public WordCounts SourceWordCounts
		{
			get;
			set;
		}

		[DataMember]
		public List<Placeable> DocumentPlaceables
		{
			get;
			set;
		}

		[DataMember]
		public Segment SourceSegment
		{
			get;
			set;
		}

		public int Count => _Results.Count;

		[DataMember]
		public long SourceHash
		{
			get;
			set;
		}

		public SearchResult this[int index]
		{
			get
			{
				return _Results[index];
			}
			set
			{
				_Results[index] = (value ?? throw new ArgumentNullException());
			}
		}

		protected void CopyFrom(SearchResults r)
		{
			_Results = r._Results;
			_SortOrder = r._SortOrder;
			SourceWordCounts = r.SourceWordCounts;
			SourceHash = r.SourceHash;
			SourceSegment = r.SourceSegment;
			DocumentPlaceables = r.DocumentPlaceables;
			MultipleTranslations = r.MultipleTranslations;
		}

		public SearchResults()
			: this(null)
		{
		}

		public SearchResults(SortSpecification defaultSortOrder)
		{
			_Results = new List<SearchResult>();
			_SortOrder = (defaultSortOrder ?? GetDefaultSortOrder());
		}

		private static SortSpecification GetDefaultSortOrder()
		{
			return new SortSpecification(DefaultSortOrder);
		}

		public void Sort(SortSpecification sortOrder)
		{
			Sort(sortOrder, null);
		}

		public void Sort(SortSpecification sortOrder, SearchResultComparer disambiguator)
		{
			_SortOrder = (sortOrder ?? GetDefaultSortOrder());
			if (_Results != null && _Results.Count >= 2 && _SortOrder.Count != 0)
			{
				SearchResultFieldValueComparer comparer = new SearchResultFieldValueComparer();
				Sorter<SearchResult> comparer2 = (disambiguator != null) ? new Sorter<SearchResult>(comparer, _SortOrder, new SortDisambiguator(_SortOrder, (SearchResult a, SearchResult b) => disambiguator(a, b))) : new Sorter<SearchResult>(comparer, _SortOrder);
				_Results.Sort(comparer2);
			}
		}

		public void Sort(string sortOrder)
		{
			if (string.IsNullOrEmpty(sortOrder))
			{
				Sort();
			}
			Sort(new SortSpecification(sortOrder));
		}

		public void Sort()
		{
			Sort(_SortOrder);
		}

		public void Merge(SearchResults other, bool removeDuplicates)
		{
			if (other?._Results == null || other._Results.Count <= 0)
			{
				return;
			}
			if (removeDuplicates && _Results.Count > 0)
			{
				bool flag = false;
				Dictionary<ulong, int> dictionary = new Dictionary<ulong, int>();
				for (int i = 0; i < _Results.Count; i++)
				{
					ulong searchResultHash = GetSearchResultHash(_Results[i]);
					if (searchResultHash != 0L && !dictionary.ContainsKey(searchResultHash))
					{
						dictionary.Add(searchResultHash, i);
					}
				}
				foreach (SearchResult result in other._Results)
				{
					ulong searchResultHash2 = GetSearchResultHash(result);
					if (!dictionary.ContainsKey(searchResultHash2) || result.MemoryTranslationUnit?.SourceSegment == null || result.MemoryTranslationUnit.TargetSegment == null)
					{
						_Results.Add(result);
					}
					else
					{
						int index = dictionary[searchResultHash2];
						SearchResult searchResult = _Results[index];
						if (searchResult != null && AreEqual(searchResult, result))
						{
							int num = result.ScoringResult.Match.CompareTo(searchResult.ScoringResult.Match);
							if (num == 0)
							{
								num = ((result.ScoringResult.TextContextMatch != 0 && searchResult.ScoringResult.TextContextMatch == TextContextMatch.NoMatch) ? 1 : searchResult.CascadeEntryIndex.CompareTo(result.CascadeEntryIndex));
							}
							if (num > 0)
							{
								_Results.Add(result);
								_Results[index] = null;
							}
							flag = true;
						}
						else
						{
							_Results.Add(result);
						}
					}
				}
				if (flag)
				{
					RemoveAll(_Results, (SearchResult x) => x == null);
					CheckForMultipleTranslations(null);
				}
			}
			else
			{
				_Results.AddRange(other._Results);
			}
		}

		private static int RemoveAll(IList<SearchResult> elements, Func<SearchResult, bool> d)
		{
			int num = 0;
			int num2 = 0;
			while (num < elements.Count)
			{
				if (d(elements[num]))
				{
					elements.RemoveAt(num);
					num2++;
				}
				else
				{
					num++;
				}
			}
			return num2;
		}

		private static ulong GetSearchResultHash(SearchResult sr)
		{
			if (sr.MemoryTranslationUnit?.SourceSegment == null || sr.MemoryTranslationUnit.TargetSegment == null)
			{
				return 0uL;
			}
			long num = (long)sr.MemoryTranslationUnit.SourceSegment.GetHashCode() << 32;
			ulong num2 = (ulong)(sr.MemoryTranslationUnit.TargetSegment.GetHashCode() & uint.MaxValue);
			return (ulong)(num | (long)num2);
		}

		public void Cap(int maxCapacity)
		{
			if (_Results != null && _Results.Count > maxCapacity)
			{
				_Results.RemoveRange(maxCapacity, _Results.Count - maxCapacity);
			}
		}

		private static Dictionary<string, List<SearchResult>> SearchResultsGroupedBySource(IEnumerable<SearchResult> searchResults, SearchSettings settings)
		{
			Dictionary<string, List<SearchResult>> dictionary = new Dictionary<string, List<SearchResult>>();
			foreach (SearchResult searchResult in searchResults)
			{
				if (searchResult.TranslationProposal != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < searchResult.TranslationProposal.SourceSegment.Elements.Count; i++)
					{
						SegmentElement obj = searchResult.TranslationProposal.SourceSegment.Elements[i] ?? searchResult.MemoryTranslationUnit.SourceSegment.Elements[i];
						Text text = obj as Text;
						Tag tag = obj as Tag;
						if (text != null)
						{
							stringBuilder.Append(WebUtility.HtmlEncode(text.Value));
						}
						else if (tag != null)
						{
							stringBuilder.Append("<");
							stringBuilder.Append(tag.Type);
							stringBuilder.Append(tag.TagID);
							if (tag.TextEquivalent != null)
							{
								stringBuilder.Append("-" + tag.TextEquivalent);
							}
							stringBuilder.Append(">");
						}
					}
					string text2 = stringBuilder.ToString();
					if (settings?.Filters != null && settings.Filters.Count > 0 && searchResult.TranslationProposal.FieldValues != null)
					{
						text2 = text2 + "#" + (from a in searchResult.TranslationProposal.FieldValues
							orderby a.ValueType, a.Name ?? ""
							where !string.IsNullOrEmpty(a.Name)
							select a).Aggregate(string.Empty, (string current, FieldValue fieldValue) => current + "[" + fieldValue.ValueType.ToString() + "," + fieldValue.Name + "]");
					}
					if (dictionary.ContainsKey(text2))
					{
						dictionary[text2].Add(searchResult);
					}
					else
					{
						dictionary.Add(text2, new List<SearchResult>
						{
							searchResult
						});
					}
				}
			}
			return dictionary;
		}

		public void CheckForMultipleTranslations(SearchSettings settings)
		{
			if (_Results != null && (settings == null || !settings.IsConcordanceSearch))
			{
				foreach (KeyValuePair<string, List<SearchResult>> item in SearchResultsGroupedBySource(_Results, settings))
				{
					CheckForMultipleTranslations(settings, item.Value);
				}
			}
		}

		private string ToNormalizedString(Segment s)
		{
			if (s.Tokens == null)
			{
				return s.ToString();
			}
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Token token in s.Tokens)
				{
					if (token != null)
					{
						switch (token.Type)
						{
						default:
							stringBuilder.Append(token.ToString());
							break;
						case TokenType.Number:
						{
							NumberToken numberToken = token as NumberToken;
							if (numberToken != null)
							{
								stringBuilder.Append(numberToken.Value.ToString());
							}
							break;
						}
						case TokenType.Measurement:
						{
							MeasureToken measureToken = token as MeasureToken;
							if (measureToken != null)
							{
								stringBuilder.Append(measureToken.Value.ToString());
								if (measureToken.UnitSeparator != 0)
								{
									stringBuilder.Append(measureToken.UnitSeparator);
								}
								stringBuilder.Append(measureToken.UnitString);
							}
							break;
						}
						case TokenType.Date:
						case TokenType.Time:
						{
							DateTimeToken dateTimeToken = token as DateTimeToken;
							if (dateTimeToken != null)
							{
								stringBuilder.Append(dateTimeToken.Value.ToString(dateTimeToken.IsDateToken ? "d" : "t"));
								stringBuilder.Append(dateTimeToken.DateTimePatternType);
							}
							break;
						}
						}
					}
				}
				return stringBuilder.ToString();
			}
			catch (Exception)
			{
				return s.ToString();
			}
		}

		public void CheckForMultipleTranslations(SearchSettings settings, List<SearchResult> searchResults)
		{
			if (searchResults == null || (settings != null && settings.IsConcordanceSearch))
			{
				return;
			}
			MultipleTranslations = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			bool flag = false;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			foreach (SearchResult searchResult in searchResults)
			{
				if (searchResult?.ScoringResult != null)
				{
					if (searchResult.ScoringResult.IsExactMatch)
					{
						if (dictionary.ContainsKey(searchResult.ScoringResult.TextReplacements))
						{
							dictionary[searchResult.ScoringResult.TextReplacements]++;
						}
						else
						{
							dictionary.Add(searchResult.ScoringResult.TextReplacements, 1);
						}
						if (dictionary2.ContainsKey(searchResult.ScoringResult.PlaceableFormatChanges))
						{
							dictionary2[searchResult.ScoringResult.PlaceableFormatChanges]++;
						}
						else
						{
							dictionary2.Add(searchResult.ScoringResult.PlaceableFormatChanges, 1);
						}
						num++;
						if (searchResult.ScoringResult.IsStructureContextMatch)
						{
							num5++;
						}
						if (!searchResult.ScoringResult.TagMismatch && !searchResult.ScoringResult.MemoryTagsDeleted)
						{
							num6++;
						}
						if (searchResult.ScoringResult.FindPenalty(PenaltyType.ProviderPenalty) == null)
						{
							num7++;
						}
						if (searchResult.ScoringResult.AppliedPenalties == null || searchResult.ScoringResult.AppliedPenalties.All((AppliedPenalty e) => e.Type != PenaltyType.FilterPenalty))
						{
							num8++;
						}
						if (!flag && searchResult.ScoringResult.FindPenalty(PenaltyType.MultipleTranslations) != null)
						{
							flag = true;
						}
					}
					switch (searchResult.ScoringResult.TextContextMatch)
					{
					case TextContextMatch.SourceMatch:
						num2++;
						break;
					case TextContextMatch.SourceTargetMatch:
						num3++;
						if (searchResult.ScoringResult.IsStructureContextMatch)
						{
							num4++;
						}
						break;
					}
				}
			}
			if (num <= 1)
			{
				return;
			}
			MultipleTranslations = true;
			bool flag2 = num3 != 1 && (num3 != 0 || num2 != 1) && (num3 <= 1 || num4 != 1) && ((num3 != 0 && num3 != num) || (num2 != 0 && num2 != num) || (num5 != 0 && num5 != num) || (num6 != 1 && num7 != 1 && num8 != 1));
			if (flag2 && dictionary.Count > 0 && dictionary[dictionary.Keys.Min()] == 1)
			{
				flag2 = false;
			}
			if (flag2 && dictionary2.Count > 0 && dictionary2[dictionary2.Keys.Min()] == 1)
			{
				flag2 = false;
			}
			if (flag2)
			{
				List<SearchResult> list = (num4 > 0) ? searchResults.FindAll((SearchResult x) => x.ScoringResult.TextContextMatch == TextContextMatch.SourceTargetMatch && x.ScoringResult.IsStructureContextMatch) : ((num3 > 0) ? searchResults.FindAll((SearchResult x) => x.ScoringResult.TextContextMatch == TextContextMatch.SourceTargetMatch) : ((num2 <= 0) ? searchResults : searchResults.FindAll((SearchResult x) => x.ScoringResult.TextContextMatch == TextContextMatch.SourceMatch)));
				bool flag3 = false;
				if (list.Count > 0)
				{
					string strA = ToNormalizedString(list[0].TranslationProposal.TargetSegment);
					for (int i = 1; i < list.Count; i++)
					{
						string strB = ToNormalizedString(list[i].TranslationProposal.TargetSegment);
						if (string.CompareOrdinal(strA, strB) != 0)
						{
							flag3 = true;
							break;
						}
					}
				}
				flag2 = flag3;
			}
			if (flag2 && settings != null)
			{
				Penalty pt;
				if ((pt = settings.FindPenalty(PenaltyType.MultipleTranslations)) != null)
				{
					foreach (SearchResult searchResult2 in searchResults)
					{
						if (CanApplyMultipleTranslationsPenalty(searchResult2))
						{
							searchResult2.ScoringResult.ApplyPenalty(pt);
						}
					}
				}
			}
			else if (!flag2 && flag)
			{
				foreach (SearchResult searchResult3 in searchResults)
				{
					if (searchResult3?.ScoringResult != null && searchResult3.ScoringResult.IsExactMatch)
					{
						searchResult3.ScoringResult.RemovePenalty(PenaltyType.MultipleTranslations);
					}
				}
			}
		}

		public virtual void Add(SearchResult result)
		{
			_Results.Add(result);
		}

		public void RemoveDuplicates()
		{
			for (int i = 0; i < _Results.Count; i++)
			{
				for (int num = _Results.Count - 1; num > i; num--)
				{
					if (AreEqual(_Results[i], _Results[num]))
					{
						_Results.RemoveAt(num);
					}
				}
			}
		}

		public int RemoveAll(Func<SearchResult, bool> predicate)
		{
			return RemoveAll(_Results, predicate);
		}

		public void RemoveDuplicates(Func<SearchResult, SearchResult, SearchResult> GetSearchResultToRemove)
		{
			BitArray bitArray = new BitArray(_Results.Count);
			for (int i = 0; i < _Results.Count; i++)
			{
				for (int num = _Results.Count - 1; num > i; num--)
				{
					if (AreEqual(_Results[i], _Results[num]))
					{
						SearchResult searchResult = GetSearchResultToRemove(_Results[i], _Results[num]);
						if (searchResult == _Results[i])
						{
							bitArray[i] = true;
						}
						else if (searchResult == _Results[num])
						{
							bitArray[num] = true;
						}
					}
				}
			}
			for (int num2 = bitArray.Length - 1; num2 >= 0; num2--)
			{
				if (bitArray[num2])
				{
					_Results.RemoveAt(num2);
				}
			}
		}

		public static bool AreEqual(SearchResult result1, SearchResult result2)
		{
			if (result1 == null || result2 == null)
			{
				throw new ArgumentNullException();
			}
			TranslationUnit translationUnit = result1.MemoryTranslationUnit ?? result1.TranslationProposal;
			TranslationUnit translationUnit2 = result2.MemoryTranslationUnit ?? result2.TranslationProposal;
			if (translationUnit == null || translationUnit2 == null)
			{
				throw new ArgumentException("No memory translation unit or translation proposal set");
			}
			if (translationUnit.SourceSegment == null || translationUnit2.SourceSegment == null || translationUnit.TargetSegment == null || translationUnit2.TargetSegment == null)
			{
				throw new ArgumentException("At least one segment is null");
			}
			if (translationUnit.SourceSegment.Equals(translationUnit2.SourceSegment))
			{
				return translationUnit.TargetSegment.Equals(translationUnit2.TargetSegment);
			}
			return false;
		}

		public void Clear()
		{
			_Results.Clear();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Results.GetEnumerator();
		}

		IEnumerator<SearchResult> IEnumerable<SearchResult>.GetEnumerator()
		{
			return _Results.GetEnumerator();
		}

		private static bool CanApplyMultipleTranslationsPenalty(SearchResult searchResult)
		{
			if (searchResult?.ScoringResult == null || !searchResult.ScoringResult.IsExactMatch)
			{
				return false;
			}
			if (searchResult.ScoringResult.AppliedPenalties == null)
			{
				return true;
			}
			foreach (AppliedPenalty appliedPenalty in searchResult.ScoringResult.AppliedPenalties)
			{
				if (appliedPenalty.Type == PenaltyType.TagMismatch || appliedPenalty.Type == PenaltyType.MemoryTagsDeleted || appliedPenalty.Type == PenaltyType.AutoLocalization || appliedPenalty.Type == PenaltyType.FilterPenalty || appliedPenalty.Type == PenaltyType.ProviderPenalty || appliedPenalty.Type == PenaltyType.TextReplacement)
				{
					return false;
				}
			}
			return true;
		}
	}
}
