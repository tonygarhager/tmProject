using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[DataContract]
	public class SegmentationContext : ICloneable
	{
		[DataMember]
		public LocalizedString Description
		{
			get;
			set;
		}

		[XmlAttribute(AttributeName = "requires", DataType = "string")]
		[DataMember]
		public string Requires
		{
			get;
			set;
		}

		[XmlAttribute("type")]
		[DataMember]
		public ContextType ContextType
		{
			get;
			set;
		}

		[DataMember]
		public string TriggerChars
		{
			get;
			set;
		}

		[DataMember]
		public Context PrecedingContext
		{
			get;
			set;
		}

		[DataMember]
		public Context FollowingContext
		{
			get;
			set;
		}

		[XmlAttribute("enabled")]
		[DataMember]
		public bool IsEnabled
		{
			get;
			set;
		}

		public SegmentationContext()
		{
			IsEnabled = true;
			ContextType = ContextType.Unknown;
		}

		public SegmentationContext(SegmentationContext other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Description = new LocalizedString(other.Description);
			TriggerChars = other.TriggerChars;
			Requires = other.Requires;
			if (other.PrecedingContext != null)
			{
				PrecedingContext = new Context(other.PrecedingContext);
			}
			if (other.FollowingContext != null)
			{
				FollowingContext = new Context(other.FollowingContext);
			}
			IsEnabled = other.IsEnabled;
			ContextType = other.ContextType;
		}

		public SegmentationContext(string description, string triggerChars, Context precedingContext, Context followingContext)
		{
			TriggerChars = triggerChars;
			PrecedingContext = precedingContext;
			FollowingContext = followingContext;
			Description = new LocalizedString();
			Description.SetText(CultureInfo.InvariantCulture, description);
			IsEnabled = true;
		}

		public bool MatchesAt(string input, int position, bool assumeEof, bool followedByWordBreak)
		{
			if (!string.IsNullOrEmpty(TriggerChars) && position > 0 && TriggerChars.IndexOf(input[position - 1]) < 0)
			{
				return false;
			}
			if (PrecedingContext != null && !PrecedingContext.MatchesUpto(input, position))
			{
				return false;
			}
			if (FollowingContext != null)
			{
				return FollowingContext.MatchesAt(input, position, assumeEof, followedByWordBreak);
			}
			return true;
		}

		public List<int> FindAllMatches(string input, int startOffset, bool assumeEof, bool followedByWordBreak)
		{
			List<int> list = new List<int>();
			if (!string.IsNullOrEmpty(TriggerChars))
			{
				for (int i = startOffset; i < input.Length; i++)
				{
					if (TriggerChars.IndexOf(input[i]) >= 0 && MatchesAt(input, i + 1, assumeEof, followedByWordBreak))
					{
						list.Add(i + 1);
					}
				}
			}
			else
			{
				if (PrecedingContext != null)
				{
					List<Match> list2 = PrecedingContext.FindAllMatches(input, startOffset);
					{
						foreach (Match item in list2)
						{
							if (FollowingContext == null || FollowingContext.MatchesAt(input, item.Index + item.Length, assumeEof, followedByWordBreak))
							{
								list.Add(item.Index + item.Length);
							}
						}
						return list;
					}
				}
				if (FollowingContext != null)
				{
					List<Match> list3 = FollowingContext.FindAllMatches(input, startOffset);
					{
						foreach (Match item2 in list3)
						{
							if (item2.Index > 0)
							{
								list.Add(item2.Index - 1);
							}
						}
						return list;
					}
				}
				for (int j = startOffset; j < input.Length; j++)
				{
					list.Add(j);
				}
			}
			return list;
		}

		public object Clone()
		{
			return new SegmentationContext(this);
		}

		public override string ToString()
		{
			if (Description != null)
			{
				return Description.Text;
			}
			return "(null)";
		}
	}
}
