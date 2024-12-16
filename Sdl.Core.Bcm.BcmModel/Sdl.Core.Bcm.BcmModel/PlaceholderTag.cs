using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class PlaceholderTag : MarkupData, ISkeletonItemReference<PlaceholderTagDefinition>
	{
		[DataMember(Name = "placeholderTagDefinitionId")]
		public int PlaceholderTagDefinitionId
		{
			get;
			set;
		}

		public PlaceholderTagDefinition Definition
		{
			get
			{
				if (base.ParentFragment == null)
				{
					return ParentParagraphUnit.ParentFile.Skeleton.PlaceholderTagDefinitions[SkeletonCollectionKey.From(PlaceholderTagDefinitionId)];
				}
				return base.ParentFragment.Skeleton.PlaceholderTagDefinitions[SkeletonCollectionKey.From(PlaceholderTagDefinitionId)];
			}
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "placeholderTag";
			}
			set
			{
			}
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitPlaceholderTag(this);
		}

		public void CopyPropertiesTo(PlaceholderTag tag)
		{
			CopyPropertiesTo((MarkupData)tag);
			tag.PlaceholderTagDefinitionId = PlaceholderTagDefinitionId;
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
			PlaceholderTag placeholderTag = other as PlaceholderTag;
			if (placeholderTag == null)
			{
				return false;
			}
			if (base.Equals(placeholderTag))
			{
				return PlaceholderTagDefinitionId == placeholderTag.PlaceholderTagDefinitionId;
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
			return (base.GetHashCode() * 397) ^ PlaceholderTagDefinitionId;
		}

		public new PlaceholderTag Clone()
		{
			return base.Clone() as PlaceholderTag;
		}
	}
}
