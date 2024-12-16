using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[DataContract]
	public class SegmentationRule : ICloneable
	{
		private SegmentationContext _matchingContext;

		[XmlAttribute("enabled")]
		[DataMember]
		public bool IsEnabled
		{
			get;
			set;
		}

		[XmlAttribute("minchars")]
		[DataMember]
		public int MinimumChars
		{
			get;
			set;
		}

		[XmlAttribute("minwords")]
		[DataMember]
		public int MinimumWords
		{
			get;
			set;
		}

		[XmlAttribute("type")]
		[DataMember]
		public RuleType Type
		{
			get;
			set;
		}

		[XmlAttribute("origin")]
		[DataMember]
		public RuleOrigin Origin
		{
			get;
			set;
		}

		[DataMember]
		public LocalizedString Description
		{
			get;
			set;
		}

		[DataMember]
		public SegmentationContext MatchingContext
		{
			get
			{
				return _matchingContext;
			}
			set
			{
				_matchingContext = (value ?? throw new ArgumentNullException("MatchingContext"));
			}
		}

		[DataMember]
		public List<SegmentationContext> Exceptions
		{
			get;
			set;
		}

		public SegmentationRule()
		{
			Exceptions = new List<SegmentationContext>();
			Description = new LocalizedString();
			IsEnabled = true;
			Type = RuleType.Unknown;
			Origin = RuleOrigin.Unknown;
		}

		public SegmentationRule(SegmentationRule other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Description = new LocalizedString(other.Description);
			if (other._matchingContext != null)
			{
				_matchingContext = new SegmentationContext(other._matchingContext);
			}
			if (other.Exceptions != null)
			{
				Exceptions = new List<SegmentationContext>();
				foreach (SegmentationContext exception in other.Exceptions)
				{
					Exceptions.Add(new SegmentationContext(exception));
				}
			}
			IsEnabled = other.IsEnabled;
			Type = other.Type;
			Origin = other.Origin;
		}

		public SegmentationRule(string description, SegmentationContext matchingContext)
			: this(description, matchingContext, null)
		{
		}

		public SegmentationRule(string description, SegmentationContext matchingContext, List<SegmentationContext> exceptions)
		{
			if (matchingContext == null)
			{
				throw new ArgumentNullException("matchingContext");
			}
			Description = new LocalizedString();
			Description.SetText(CultureInfo.InvariantCulture, description);
			_matchingContext = matchingContext;
			Exceptions = (exceptions ?? new List<SegmentationContext>());
			IsEnabled = true;
			Type = RuleType.Unknown;
			Origin = RuleOrigin.Unknown;
		}

		public override string ToString()
		{
			if (Description != null)
			{
				return Description.Text;
			}
			return "(null)";
		}

		public void AddException(SegmentationContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (Exceptions == null)
			{
				Exceptions = new List<SegmentationContext>();
			}
			Exceptions.Add(context);
		}

		public bool MatchesAt(string input, int position, bool assumeEof, bool followedByWordBreak)
		{
			bool flag = _matchingContext.MatchesAt(input, position, assumeEof, followedByWordBreak);
			if (!flag || Exceptions == null)
			{
				return flag;
			}
			return !Exceptions.Any((SegmentationContext sc) => sc.IsEnabled && sc.MatchesAt(input, position, assumeEof, followedByWordBreak));
		}

		public int FindFirstMatch(string input, int startOffset, bool assumeEof, bool followedByWordBreak)
		{
			List<int> list = _matchingContext.FindAllMatches(input, startOffset, assumeEof, followedByWordBreak);
			if (list == null || list.Count == 0)
			{
				return -1;
			}
			if (Exceptions == null || Exceptions.Count == 0)
			{
				return list[0];
			}
			foreach (int candidate in list)
			{
				if (!Exceptions.Any((SegmentationContext exception) => exception.IsEnabled && exception.MatchesAt(input, candidate, assumeEof, followedByWordBreak)))
				{
					return candidate;
				}
			}
			return -1;
		}

		public object Clone()
		{
			return new SegmentationRule(this);
		}
	}
}
