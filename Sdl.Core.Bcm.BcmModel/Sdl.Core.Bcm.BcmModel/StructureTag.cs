using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class StructureTag : MarkupData, ISkeletonItemReference<StructureTagDefinition>
	{
		[DataMember(Name = "structureTagDefinitionId")]
		public int StructureTagDefinitionId
		{
			get;
			set;
		}

		public StructureTagDefinition Definition
		{
			get
			{
				if (base.ParentFragment == null)
				{
					return ParentParagraphUnit.ParentFile.Skeleton.StructureTagDefinitions[SkeletonCollectionKey.From(StructureTagDefinitionId)];
				}
				return base.ParentFragment.Skeleton.StructureTagDefinitions[SkeletonCollectionKey.From(StructureTagDefinitionId)];
			}
		}

		[JsonProperty("type")]
		protected override string Type
		{
			get
			{
				return "structureTag";
			}
			set
			{
			}
		}

		public override void AcceptVisitor(BcmVisitor visitor)
		{
			visitor.VisitStructure(this);
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
			StructureTag structureTag = other as StructureTag;
			if (structureTag == null)
			{
				return false;
			}
			if (base.Equals(structureTag))
			{
				return StructureTagDefinitionId == structureTag.StructureTagDefinitionId;
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
			return (base.GetHashCode() * 397) ^ StructureTagDefinitionId;
		}

		public new StructureTag Clone()
		{
			return base.Clone() as StructureTag;
		}
	}
}
