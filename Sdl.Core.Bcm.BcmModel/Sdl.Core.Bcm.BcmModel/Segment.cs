using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Alignment;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class Segment : MarkupDataContainer
	{
		[DataMember(Name = "confirmationLevel", EmitDefaultValue = false)]
		[DefaultValue(ConfirmationLevel.NotTranslated)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
		public ConfirmationLevel ConfirmationLevel
		{
			get;
			set;
		}

		[DataMember(Name = "isLocked", EmitDefaultValue = false)]
		[DefaultValue(false)]
		public bool IsLocked
		{
			get;
			set;
		}

		[DataMember(Name = "wordCount", EmitDefaultValue = false)]
		public int WordCount
		{
			get;
			set;
		}

		[DataMember(Name = "characterCount", EmitDefaultValue = false)]
		public int CharacterCount
		{
			get;
			set;
		}

		[DataMember(Name = "segmentNumber")]
		public string SegmentNumber
		{
			get;
			set;
		}

		[DataMember(Name = "translationOrigin", EmitDefaultValue = false)]
		public TranslationOrigin TranslationOrigin
		{
			get;
			set;
		}

		[DataMember(Name = "tokens")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<Token> Tokens
		{
			get;
			set;
		}

		[DataMember(Name = "alignmentData")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public AlignmentData AlignmentData
		{
			get;
			set;
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "segment";
			}
			set
			{
			}
		}

		public Segment SiblingSegment
		{
			get
			{
				if (base.ParentParagraph == null || ParentParagraphUnit == null)
				{
					return null;
				}
				bool flag = object.Equals(base.ParentParagraph, ParentParagraphUnit.Source);
				SegmentPair segmentPair = ParentParagraphUnit.SegmentPairs[SegmentNumber];
				if (!flag)
				{
					return segmentPair.Source;
				}
				return segmentPair.Target;
			}
		}

		public Segment(string segmentNumber)
		{
			SegmentNumber = segmentNumber;
			base.Id = Guid.NewGuid().ToString();
		}

		public Segment()
		{
			ConfirmationLevel = ConfirmationLevel.NotTranslated;
			IsLocked = false;
			base.Id = Guid.NewGuid().ToString();
		}

		public Segment(string segmentNumber, ConfirmationLevel confirmationLevel)
		{
			SegmentNumber = segmentNumber;
			ConfirmationLevel = confirmationLevel;
			base.Id = Guid.NewGuid().ToString();
		}

		public void CopyPropertiesTo(Segment target)
		{
			CopyPropertiesTo((MarkupDataContainer)target);
			target.SegmentNumber = SegmentNumber;
			target.ConfirmationLevel = ConfirmationLevel;
			target.IsLocked = IsLocked;
			target.WordCount = WordCount;
			target.CharacterCount = CharacterCount;
			target.TranslationOrigin = TranslationOrigin?.Clone();
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitSegment(this);
		}

		public new Segment Clone()
		{
			Segment result = base.Clone() as Segment;
			CloneTranslationOrigin(result);
			return result;
		}

		public new Segment UniqueClone()
		{
			Segment result = base.UniqueClone() as Segment;
			CloneTranslationOrigin(result);
			return result;
		}

		internal override MarkupDataContainer CloneWithoutChildren()
		{
			Segment result = base.CloneWithoutChildren() as Segment;
			CloneTranslationOrigin(result);
			return result;
		}

		public override bool Equals(MarkupData other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			Segment segment = other as Segment;
			if (segment == null)
			{
				return false;
			}
			if (base.Equals(other) && ConfirmationLevel == segment.ConfirmationLevel && IsLocked.Equals(segment.IsLocked) && WordCount == segment.WordCount && CharacterCount == segment.CharacterCount && string.Equals(SegmentNumber, segment.SegmentNumber))
			{
				return object.Equals(TranslationOrigin, segment.TranslationOrigin);
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
			return Equals((MarkupData)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ (int)ConfirmationLevel);
			hashCode = ((hashCode * 397) ^ IsLocked.GetHashCode());
			hashCode = ((hashCode * 397) ^ WordCount);
			hashCode = ((hashCode * 397) ^ CharacterCount);
			hashCode = ((hashCode * 397) ^ ((SegmentNumber != null) ? SegmentNumber.GetHashCode() : 0));
			return (hashCode * 397) ^ ((TranslationOrigin != null) ? TranslationOrigin.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<segment \"{0}\">", SegmentNumber);
			foreach (MarkupData child in base.Children)
			{
				stringBuilder.Append(child);
			}
			stringBuilder.Append("</segment>");
			return stringBuilder.ToString();
		}

		private void CloneTranslationOrigin(Segment result)
		{
			if (TranslationOrigin != null)
			{
				result.TranslationOrigin = TranslationOrigin.Clone();
			}
		}
	}
}
