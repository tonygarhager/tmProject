using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Collections;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class Term : ExtensionDataContainer, ICloneable<Term>, IEquatable<Term>, ITermAttributeContainer
	{
		[DataMember(Name = "id")]
		public string Id
		{
			get;
			set;
		}

		[DataMember(Name = "text")]
		public string Text
		{
			get;
			set;
		}

		[DataMember(Name = "score")]
		public double Score
		{
			get;
			set;
		}

		[DataMember(Name = "termOrigin")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public TermOrigin TermOrigin
		{
			get;
			set;
		}

		[DataMember(Name = "termTranslations")]
		public List<TermTranslation> TermTranslations
		{
			get;
			set;
		}

		[DataMember(Name = "termAttributes")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DictionaryEx<string, object> TermAttributes
		{
			get;
			set;
		}

		public Term()
		{
			TermTranslations = new List<TermTranslation>();
			TermAttributes = new DictionaryEx<string, object>();
		}

		public Term Clone()
		{
			Term result = (Term)MemberwiseClone();
			if (TermTranslations != null)
			{
				result.TermTranslations = new List<TermTranslation>();
				TermTranslations.ForEach(delegate(TermTranslation x)
				{
					result.TermTranslations.Add(x.Clone());
				});
			}
			if (TermAttributes != null)
			{
				result.TermAttributes = new DictionaryEx<string, object>(TermAttributes);
			}
			return result;
		}

		public bool Equals(Term other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (string.Equals(Text, other.Text) && string.Equals(Id, other.Id) && Score.Equals(other.Score) && ((TermTranslations == null && other.TermTranslations == null) || (TermTranslations != null && TermTranslations.IsSequenceEqual(other.TermTranslations))))
			{
				if (TermAttributes != null || other.TermAttributes != null)
				{
					if (TermAttributes != null)
					{
						return TermAttributes.Equals(other.TermAttributes);
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((Term)obj);
		}

		public override int GetHashCode()
		{
			int num = (Text != null) ? Text.GetHashCode() : 0;
			num = ((num * 397) ^ ((Id != null) ? Id.GetHashCode() : 0));
			num = ((num * 397) ^ Score.GetHashCode());
			num = ((num * 397) ^ ((TermTranslations != null && TermTranslations.Count > 0) ? TermTranslations.GetHashCode() : 0));
			return (num * 397) ^ ((TermAttributes != null && TermAttributes.Count > 0) ? TermAttributes.GetHashCode() : 0);
		}

		public bool ShouldSerializeTermTranslations()
		{
			if (TermTranslations != null)
			{
				return TermTranslations.Count > 0;
			}
			return false;
		}

		public bool ShouldSerializeTermAttributes()
		{
			if (TermAttributes != null)
			{
				return TermAttributes.Count > 0;
			}
			return false;
		}
	}
}
