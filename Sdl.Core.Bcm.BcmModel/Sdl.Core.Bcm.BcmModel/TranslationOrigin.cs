using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class TranslationOrigin : MetadataContainer, ICloneable<TranslationOrigin>, IEquatable<TranslationOrigin>
	{
		[DataMember(Name = "originType")]
		public string OriginType
		{
			get;
			set;
		}

		[DataMember(Name = "originSystem")]
		public string OriginSystem
		{
			get;
			set;
		}

		[DataMember(Name = "matchPercent")]
		public int MatchPercent
		{
			get;
			set;
		}

		[DataMember(Name = "isStructureContextMatch", EmitDefaultValue = false)]
		public bool IsStructureContextMatch
		{
			get;
			set;
		}

		[DataMember(Name = "isSidContextMatch", EmitDefaultValue = false)]
		public bool IsSidContextMatch
		{
			get;
			set;
		}

		[DataMember(Name = "textContextMatchLevel")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public TextContextMatchLevel TextContextMatchLevel
		{
			get;
			set;
		}

		[DataMember(Name = "originalTranslationHash", EmitDefaultValue = false)]
		public string OriginalTranslationHash
		{
			get;
			set;
		}

		[DataMember(Name = "originBeforeAdaptation", EmitDefaultValue = false)]
		public TranslationOrigin OriginBeforeAdaptation
		{
			get;
			set;
		}

		public TranslationOrigin()
		{
		}

		public TranslationOrigin(string originType, string originSystem, int matchPercent, bool isStructureContextMatch, bool isSidContextMatch, TextContextMatchLevel textContextMatchLevel, string originalTranslationHash, TranslationOrigin originBeforeAdaptation)
		{
			OriginType = originType;
			OriginSystem = originSystem;
			MatchPercent = matchPercent;
			IsStructureContextMatch = isStructureContextMatch;
			IsSidContextMatch = isSidContextMatch;
			TextContextMatchLevel = textContextMatchLevel;
			OriginalTranslationHash = originalTranslationHash;
			OriginBeforeAdaptation = originBeforeAdaptation;
		}

		public TranslationOrigin Clone()
		{
			TranslationOrigin translationOrigin = (TranslationOrigin)MemberwiseClone();
			translationOrigin.ReplaceMetadataWith(base.Metadata);
			if (OriginBeforeAdaptation != null)
			{
				translationOrigin.OriginBeforeAdaptation = OriginBeforeAdaptation.Clone();
			}
			return translationOrigin;
		}

		public bool Equals(TranslationOrigin other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((MetadataContainer)other) && string.Equals(OriginType, other.OriginType) && string.Equals(OriginSystem, other.OriginSystem) && MatchPercent == other.MatchPercent && IsStructureContextMatch.Equals(other.IsStructureContextMatch) && IsSidContextMatch.Equals(other.IsSidContextMatch) && TextContextMatchLevel == other.TextContextMatchLevel && string.Equals(OriginalTranslationHash, other.OriginalTranslationHash))
			{
				return object.Equals(OriginBeforeAdaptation, other.OriginBeforeAdaptation);
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
			return Equals((TranslationOrigin)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((OriginType != null) ? OriginType.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((OriginSystem != null) ? OriginSystem.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ MatchPercent);
			hashCode = ((hashCode * 397) ^ IsStructureContextMatch.GetHashCode());
			hashCode = ((hashCode * 397) ^ IsSidContextMatch.GetHashCode());
			hashCode = ((hashCode * 397) ^ (int)TextContextMatchLevel);
			hashCode = ((hashCode * 397) ^ ((OriginalTranslationHash != null) ? OriginalTranslationHash.GetHashCode() : 0));
			return (hashCode * 397) ^ ((OriginBeforeAdaptation != null) ? OriginBeforeAdaptation.GetHashCode() : 0);
		}
	}
}
