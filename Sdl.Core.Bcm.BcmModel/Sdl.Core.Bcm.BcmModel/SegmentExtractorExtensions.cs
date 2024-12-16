using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel
{
	internal static class SegmentExtractorExtensions
	{
		public static void ExtractSegmentSkeleton(this Segment segment, FileSkeleton oldSkeleton, FileSkeleton newSkeleton)
		{
			segment.ExtractMarkupDataContainerSkeleton(oldSkeleton, newSkeleton);
		}

		public static void ExtractMarkupDataContainerSkeleton(this MarkupDataContainer markupDataContainer, FileSkeleton oldSkeleton, FileSkeleton newSkeleton)
		{
			foreach (TagPair tagPair in markupDataContainer.AllSubItems.OfType<TagPair>())
			{
				TagPairDefinition tagPairDefinition = oldSkeleton.TagPairDefinitions.SingleOrDefault((TagPairDefinition tp) => tp.Id == tagPair.TagPairDefinitionId);
				if (tagPairDefinition != null)
				{
					TagPairDefinition cloneDef = tagPairDefinition.Clone();
					FormattingGroup formattingGroup = oldSkeleton.FormattingGroups.SingleOrDefault((FormattingGroup fg) => fg.Id == cloneDef.FormattingGroupId);
					if (formattingGroup != null)
					{
						FormattingGroup elem = formattingGroup.Clone();
						FormattingGroup orAddWithExistingId = newSkeleton.FormattingGroups.GetOrAddWithExistingId(elem);
						cloneDef.FormattingGroupId = orAddWithExistingId.Id;
					}
					TagPairDefinition orAddWithExistingId2 = newSkeleton.TagPairDefinitions.GetOrAddWithExistingId(cloneDef);
					tagPair.TagPairDefinitionId = orAddWithExistingId2.Id;
				}
			}
			foreach (PlaceholderTag placeholderTag in markupDataContainer.AllSubItems.OfType<PlaceholderTag>())
			{
				PlaceholderTagDefinition placeholderTagDefinition = oldSkeleton.PlaceholderTagDefinitions.SingleOrDefault((PlaceholderTagDefinition pt) => pt.Id == placeholderTag.PlaceholderTagDefinitionId);
				if (placeholderTagDefinition != null)
				{
					PlaceholderTagDefinition elem2 = placeholderTagDefinition.Clone();
					PlaceholderTagDefinition orAddWithExistingId3 = newSkeleton.PlaceholderTagDefinitions.GetOrAddWithExistingId(elem2);
					placeholderTag.PlaceholderTagDefinitionId = orAddWithExistingId3.Id;
				}
			}
			foreach (StructureTag structureTag in markupDataContainer.AllSubItems.OfType<StructureTag>())
			{
				StructureTagDefinition structureTagDefinition = oldSkeleton.StructureTagDefinitions.SingleOrDefault((StructureTagDefinition st) => st.Id == structureTag.StructureTagDefinitionId);
				if (structureTagDefinition != null)
				{
					StructureTagDefinition elem3 = structureTagDefinition.Clone();
					StructureTagDefinition orAddWithExistingId4 = newSkeleton.StructureTagDefinitions.GetOrAddWithExistingId(elem3);
					structureTag.StructureTagDefinitionId = orAddWithExistingId4.Id;
				}
			}
			foreach (CommentContainer comment in markupDataContainer.AllSubItems.OfType<CommentContainer>())
			{
				CommentDefinition commentDefinition = oldSkeleton.CommentDefinitions.SingleOrDefault((CommentDefinition cd) => cd.Id == comment.CommentDefinitionId);
				if (commentDefinition != null)
				{
					CommentDefinition elem4 = commentDefinition.Clone();
					CommentDefinition orAddWithExistingId5 = newSkeleton.CommentDefinitions.GetOrAddWithExistingId(elem4);
					comment.CommentDefinitionId = orAddWithExistingId5.Id;
				}
			}
		}
	}
}
