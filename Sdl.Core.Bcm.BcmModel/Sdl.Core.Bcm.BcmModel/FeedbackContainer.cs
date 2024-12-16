using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class FeedbackContainer : MarkupDataContainer
	{
		[DataMember(Name = "author")]
		public string Author
		{
			get;
			set;
		}

		[DataMember(Name = "timestamp")]
		public DateTime? Timestamp
		{
			get;
			set;
		}

		[DataMember(Name = "category")]
		public string Category
		{
			get;
			set;
		}

		[DataMember(Name = "documentCategory")]
		public string DocumentCategory
		{
			get;
			set;
		}

		[DataMember(Name = "severity")]
		public string Severity
		{
			get;
			set;
		}

		[Obsolete("SHOULD be removed in next MAJOR release")]
		[DataMember(Name = "commentDefinitionId")]
		public int CommentDefinitionId
		{
			get;
			set;
		}

		[DataMember(Name = "feedbackType")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public FeedbackType FeedbackType
		{
			get;
			set;
		}

		[DataMember(Name = "replacementId")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ReplacementId
		{
			get;
			set;
		}

		[DataMember(Name = "comment")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Comment
		{
			get;
			set;
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "feedback";
			}
			set
			{
			}
		}

		public FeedbackContainer()
		{
		}

		public FeedbackContainer(FeedbackType feedbackType)
		{
			FeedbackType = feedbackType;
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitFeedbackContainer(this);
		}

		public new FeedbackContainer Clone()
		{
			return base.Clone() as FeedbackContainer;
		}

		public new FeedbackContainer UniqueClone()
		{
			return base.UniqueClone() as FeedbackContainer;
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
			FeedbackContainer feedbackContainer = other as FeedbackContainer;
			if (feedbackContainer == null)
			{
				return false;
			}
			if (base.Equals(feedbackContainer) && string.Equals(Author, feedbackContainer.Author) && Timestamp.Equals(feedbackContainer.Timestamp) && string.Equals(Category, feedbackContainer.Category) && string.Equals(DocumentCategory, feedbackContainer.DocumentCategory) && string.Equals(Severity, feedbackContainer.Severity) && CommentDefinitionId == feedbackContainer.CommentDefinitionId && FeedbackType == feedbackContainer.FeedbackType && string.Equals(ReplacementId, feedbackContainer.ReplacementId))
			{
				return string.Equals(Comment, feedbackContainer.Comment);
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
			hashCode = ((hashCode * 397) ^ ((Author != null) ? Author.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ Timestamp.GetHashCode());
			hashCode = ((hashCode * 397) ^ ((Category != null) ? Category.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((DocumentCategory != null) ? DocumentCategory.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Severity != null) ? Severity.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ CommentDefinitionId);
			hashCode = ((hashCode * 397) ^ (int)FeedbackType);
			hashCode = ((hashCode * 397) ^ ((ReplacementId != null) ? ReplacementId.GetHashCode() : 0));
			return (hashCode * 397) ^ ((Comment != null) ? Comment.GetHashCode() : 0);
		}
	}
}
