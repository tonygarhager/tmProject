using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[DataContract]
	public class Context : ICloneable
	{
		private string _pattern;

		private Regex _regex;

		private readonly Dictionary<string, MatchCollection> _matches = new Dictionary<string, MatchCollection>();

		[XmlAttribute(AttributeName = "CaseInsensitiveMatching")]
		[DataMember]
		public bool CaseInsensitiveMatching
		{
			get;
			set;
		}

		[XmlAttribute(AttributeName = "MatchesAtInputBoundary")]
		[DataMember]
		public bool MatchesAtInputBoundary
		{
			get;
			set;
		}

		[DataMember]
		public string Pattern
		{
			get
			{
				return _pattern;
			}
			set
			{
				_pattern = value;
				_regex = new Regex(_pattern, CaseInsensitiveMatching ? RegexOptions.IgnoreCase : RegexOptions.None);
			}
		}

		public Context()
		{
		}

		public Context(string pattern)
		{
			Init(pattern, caseInsensitive: false, matchesInputBoundary: false);
		}

		public Context(Context other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Init(other._pattern, other.CaseInsensitiveMatching, other.MatchesAtInputBoundary);
		}

		public Context(string pattern, bool caseInsensitive, bool matchesInputBoundary)
		{
			Init(pattern, caseInsensitive, matchesInputBoundary);
		}

		private void Init(string pattern, bool caseInsensitive, bool matchesInputBoundary)
		{
			CaseInsensitiveMatching = caseInsensitive;
			MatchesAtInputBoundary = matchesInputBoundary;
			_pattern = pattern;
			_regex = new Regex(_pattern, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
		}

		public bool MatchesAt(string s, int position, bool assumeEof, bool followedByWordBreak)
		{
			if (position >= s.Length)
			{
				if (MatchesAtInputBoundary)
				{
					return assumeEof | followedByWordBreak;
				}
				return false;
			}
			System.Text.RegularExpressions.Match match = _regex.Match(s, position);
			if (match.Success)
			{
				return match.Index == position;
			}
			return false;
		}

		public bool MatchesUpto(string s, int position)
		{
			int startat = Math.Max(0, position - 50);
			string key = $"{s}{_regex}";
			MatchCollection matchCollection;
			if (!_matches.TryGetValue(key, out MatchCollection value))
			{
				matchCollection = _regex.Matches(s, startat);
				_matches.Add(key, matchCollection);
			}
			else
			{
				matchCollection = value;
			}
			foreach (System.Text.RegularExpressions.Match item in matchCollection)
			{
				if (item.Success && item.Index + item.Length == position)
				{
					return true;
				}
			}
			return false;
		}

		public List<Sdl.LanguagePlatform.Core.Tokenization.Match> FindAllMatches(string s, int startOffset)
		{
			MatchCollection matchCollection = _regex.Matches(s, startOffset);
			List<Sdl.LanguagePlatform.Core.Tokenization.Match> list = new List<Sdl.LanguagePlatform.Core.Tokenization.Match>();
			foreach (System.Text.RegularExpressions.Match item in matchCollection)
			{
				if (item.Success)
				{
					list.Add(new Sdl.LanguagePlatform.Core.Tokenization.Match(item.Index, item.Length));
				}
			}
			return list;
		}

		public override string ToString()
		{
			return _pattern ?? string.Empty;
		}

		public object Clone()
		{
			return new Context(this);
		}
	}
}
