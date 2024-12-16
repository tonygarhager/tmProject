using Sdl.Core.Bcm.BcmConverters.FromBilingualApi;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.Integration.QuickInserts;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using System;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	internal static class QuickInsertHelper
	{
		private static readonly IQuickInsertDefinitionsManager _quickInsertDefinitionsManager = new QuickInsertDefinitionsManager();

		internal static void AddQuickTagDefinitionToSkeleton(string quickInsertId, FileSkeleton fileSkeleton)
		{
			IQuickInsert quickInsert = CreateQuickInsertFromId(quickInsertId);
			IAbstractMarkupData abstractMarkupData = quickInsert.MarkupData[0];
			if (TryCreateTag(abstractMarkupData, out ITagPair tag))
			{
				CreateSkeletonTagPairDefinition(tag, quickInsert.Formatting, fileSkeleton);
				return;
			}
			if (TryCreateTag(abstractMarkupData, out IPlaceholderTag tag2))
			{
				CreateSkeletonPlaceholderTagDefinition(quickInsert.Id, tag2, fileSkeleton);
				return;
			}
			throw new NotSupportedException($"Not supported {abstractMarkupData.GetType()} type found for quick insert!");
		}

		internal static bool TryCreateSkeletonTagPairDefinition(string quickInsertId, FileSkeleton fileSkeleton, out TagPairDefinition tagPairDefinition)
		{
			tagPairDefinition = null;
			if (!TryCreateQuickInsertFromId(quickInsertId, out IQuickInsert quickInsert))
			{
				return false;
			}
			IAbstractMarkupData markupData = quickInsert.MarkupData[0];
			if (!TryCreateTag(markupData, out ITagPair tag))
			{
				return false;
			}
			tagPairDefinition = CreateSkeletonTagPairDefinition(tag, quickInsert.Formatting, fileSkeleton);
			return true;
		}

		internal static bool TryCreateSkeletonPlaceholderTagDefinition(string quickInsertId, FileSkeleton fileSkeleton, out PlaceholderTagDefinition placeholderTagDefinition)
		{
			placeholderTagDefinition = null;
			if (!TryCreateQuickInsertFromId(quickInsertId, out IQuickInsert quickInsert))
			{
				return false;
			}
			IAbstractMarkupData markupData = quickInsert.MarkupData[0];
			if (!TryCreateTag(markupData, out IPlaceholderTag tag))
			{
				return false;
			}
			placeholderTagDefinition = CreateSkeletonPlaceholderTagDefinition(quickInsert.Id, tag, fileSkeleton);
			return true;
		}

		internal static bool TryCreateQuickInsertMarkupFromId<T>(string quickInsertId, out T quickInsertMarkup) where T : class, IAbstractMarkupData
		{
			quickInsertMarkup = null;
			if (!TryCreateQuickInsertFromId(quickInsertId, out IQuickInsert quickInsert))
			{
				return false;
			}
			quickInsertMarkup = (quickInsert.MarkupData[0] as T);
			if (quickInsertMarkup == null)
			{
				return false;
			}
			quickInsertMarkup.Parent?.Remove(quickInsertMarkup);
			return true;
		}

		private static TagPairDefinition CreateSkeletonTagPairDefinition(ITagPair tagPair, IFormattingGroup formattingGroup, FileSkeleton fileSkeleton)
		{
			TagPairDefinition tagPairDefinition = TagDefinitionBuilder.BuildTagPair(0, tagPair);
			tagPairDefinition.QuickInsertId = tagPair.StartTagProperties.TagId.Id;
			if (formattingGroup != null)
			{
				FormattingGroup formattingGroup2 = FormattingGroupHelper.CreateFormatting(formattingGroup);
				tagPairDefinition.FormattingGroupId = FormattingGroupHelper.AddFormatting(fileSkeleton, formattingGroup2);
			}
			return fileSkeleton.TagPairDefinitions.GetOrAdd(tagPairDefinition);
		}

		private static PlaceholderTagDefinition CreateSkeletonPlaceholderTagDefinition(QuickInsertIds quickInsertId, IPlaceholderTag placeholderTag, FileSkeleton fileSkeleton)
		{
			PlaceholderTagDefinition placeholderTagDefinition = TagDefinitionBuilder.BuildPlaceholder(0, placeholderTag);
			placeholderTagDefinition.QuickInsertId = quickInsertId.ToString();
			return fileSkeleton.PlaceholderTagDefinitions.GetOrAdd(placeholderTagDefinition);
		}

		private static IQuickInsert CreateQuickInsertFromId(string quickInsertId)
		{
			if (!_quickInsertDefinitionsManager.TryParseQuickInsertId(quickInsertId, out QuickInsertIds quickInsertId2))
			{
				throw new InvalidOperationException("invalid quick insert id " + quickInsertId + "!");
			}
			return CreateValidatedMarkupDataQuickInsert(quickInsertId2);
		}

		private static bool TryCreateQuickInsertFromId(string quickInsertId, out IQuickInsert quickInsert)
		{
			quickInsert = null;
			if (string.IsNullOrEmpty(quickInsertId))
			{
				return false;
			}
			if (!_quickInsertDefinitionsManager.TryParseQuickInsertId(quickInsertId, out QuickInsertIds quickInsertId2))
			{
				return false;
			}
			quickInsert = CreateValidatedMarkupDataQuickInsert(quickInsertId2);
			return true;
		}

		private static IQuickInsert CreateValidatedMarkupDataQuickInsert(QuickInsertIds typedQuickInsertId)
		{
			IQuickInsert quickInsert = _quickInsertDefinitionsManager.BuildClonedQuickInsert(typedQuickInsertId);
			if (quickInsert.MarkupData.Count != 1)
			{
				throw new NotSupportedException($"Count {quickInsert.MarkupData.Count} for quick insert markup children is not supported!");
			}
			return quickInsert;
		}

		private static bool TryCreateTag<T>(IAbstractMarkupData markupData, out T tag) where T : class
		{
			tag = (markupData as T);
			return tag != null;
		}
	}
}
