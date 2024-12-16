using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class TagPair : MarkupDataContainer, ISkeletonItemReference<TagPairDefinition>
	{
		[DataMember(Name = "tagPairDefinitionId")]
		public int TagPairDefinitionId
		{
			get;
			set;
		}

		public TagPairDefinition Definition
		{
			get
			{
				if (base.ParentFragment == null)
				{
					File parentFile = ParentParagraphUnit.ParentFile;
					if (parentFile == null)
					{
						return null;
					}
					return parentFile.Skeleton.TagPairDefinitions[SkeletonCollectionKey.From(TagPairDefinitionId)];
				}
				return base.ParentFragment.Skeleton.TagPairDefinitions[SkeletonCollectionKey.From(TagPairDefinitionId)];
			}
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "tagPair";
			}
			set
			{
			}
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitTagPair(this);
		}

		public void CopyPropertiesTo(TagPair tagPair)
		{
			CopyPropertiesTo((MarkupDataContainer)tagPair);
			tagPair.TagPairDefinitionId = TagPairDefinitionId;
		}

		public new TagPair Clone()
		{
			return base.Clone() as TagPair;
		}

		public new TagPair UniqueClone()
		{
			return base.UniqueClone() as TagPair;
		}

		public override void ClearTagPairs()
		{
			base.ClearTagPairs();
			List<MarkupData> list = base.Children.ToList();
			MarkupDataContainer parent = base.Parent;
			int num = base.IndexInParent;
			RemoveFromParent();
			foreach (MarkupData item in list)
			{
				item.RemoveFromParent();
				parent.InsertAt(num, item);
				num++;
			}
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
			TagPair tagPair = other as TagPair;
			if (tagPair == null)
			{
				return false;
			}
			if (base.Equals(tagPair))
			{
				return TagPairDefinitionId == tagPair.TagPairDefinitionId;
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
			return (base.GetHashCode() * 397) ^ TagPairDefinitionId;
		}
	}
}
