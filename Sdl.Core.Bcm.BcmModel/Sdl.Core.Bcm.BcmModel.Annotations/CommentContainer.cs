using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Annotations
{
	[DataContract]
	public class CommentContainer : AnnotationContainer, ISkeletonItemReference<CommentDefinition>
	{
		[DataMember(Name = "commentDefinitionId")]
		public int CommentDefinitionId
		{
			get;
			set;
		}

		public CommentDefinition Definition => ParentParagraphUnit.ParentFile.Skeleton.CommentDefinitions[SkeletonCollectionKey.From(CommentDefinitionId)];

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "comment";
			}
			set
			{
			}
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitCommentContainer(this);
		}

		public new CommentContainer Clone()
		{
			return base.Clone() as CommentContainer;
		}

		public new CommentContainer UniqueClone()
		{
			return base.UniqueClone() as CommentContainer;
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
			CommentContainer commentContainer = other as CommentContainer;
			if (commentContainer == null)
			{
				return false;
			}
			if (base.Equals(commentContainer))
			{
				return CommentDefinitionId.Equals(commentContainer.CommentDefinitionId);
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
			return (base.GetHashCode() * 397) ^ CommentDefinitionId;
		}
	}
}
