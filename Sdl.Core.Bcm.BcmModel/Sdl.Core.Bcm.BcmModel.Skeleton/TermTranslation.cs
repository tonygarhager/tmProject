using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Collections;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class TermTranslation : ExtensionDataContainer, ICloneable<TermTranslation>, IEquatable<TermTranslation>, ITermAttributeContainer
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

		[DataMember(Name = "termAttributes")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DictionaryEx<string, object> TermAttributes
		{
			get;
			set;
		}

		public TermTranslation()
		{
			TermAttributes = new DictionaryEx<string, object>();
		}

		public TermTranslation Clone()
		{
			TermTranslation termTranslation = (TermTranslation)MemberwiseClone();
			if (TermAttributes != null)
			{
				termTranslation.TermAttributes = new DictionaryEx<string, object>(TermAttributes);
			}
			return termTranslation;
		}

		public bool Equals(TermTranslation other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (string.Equals(Text, other.Text) && string.Equals(Id, other.Id))
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
			return Equals((TermTranslation)obj);
		}

		public override int GetHashCode()
		{
			int num = (Text != null) ? Text.GetHashCode() : 0;
			num = ((num * 397) ^ ((Id != null) ? Id.GetHashCode() : 0));
			return (num * 397) ^ ((TermAttributes != null) ? TermAttributes.GetHashCode() : 0);
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
