using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi
{
	internal static class TagDefinitionBuilder
	{
		internal static PlaceholderTagDefinition BuildPlaceholder(int id, IPlaceholderTag tag)
		{
			PlaceholderTagDefinition placeholderTagDefinition = new PlaceholderTagDefinition
			{
				Id = id,
				DisplayText = tag.TagProperties.DisplayText,
				SegmentationHint = MarkupDataConverter.Convert(tag.Properties.SegmentationHint),
				SubContent = MarkupDataConverter.Convert(tag.SubSegments),
				TagContent = tag.Properties.TagContent,
				TextEquivalent = tag.Properties.TextEquivalent
			};
			placeholderTagDefinition.CopyMetadataFrom(tag.Properties);
			return placeholderTagDefinition;
		}

		internal static StructureTagDefinition BuildStructure(int id, IStructureTag tag)
		{
			return new StructureTagDefinition
			{
				Id = id,
				DisplayText = tag.Properties.DisplayText,
				TagContent = tag.Properties.TagContent,
				SubContent = MarkupDataConverter.Convert(tag.SubSegments)
			};
		}

		internal static TagPairDefinition BuildTagPair(int id, ITagPair tag)
		{
			return new TagPairDefinition
			{
				Id = id,
				CanHide = tag.StartTagProperties.CanHide,
				EndTagContent = tag.EndTagProperties.TagContent,
				EndTagDisplayText = tag.EndTagProperties.DisplayText,
				SubContent = MarkupDataConverter.Convert(tag.SubSegments),
				SegmentationHint = MarkupDataConverter.Convert(tag.StartTagProperties.SegmentationHint),
				StartTagContent = tag.StartTagProperties.TagContent,
				StartTagDisplayText = tag.StartTagProperties.DisplayText
			};
		}
	}
}
