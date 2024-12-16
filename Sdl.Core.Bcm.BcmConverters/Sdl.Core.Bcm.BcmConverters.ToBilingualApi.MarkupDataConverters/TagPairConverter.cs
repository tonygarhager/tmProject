using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class TagPairConverter : MarkupDataConverter
	{
		private readonly IPropertiesFactory _propertiesFactory;

		private readonly IDocumentItemFactory _documentItemFactory;

		private readonly FileSkeleton _fileSkeleton;

		public TagPairConverter(IPropertiesFactory propertiesFactory, IDocumentItemFactory documentItemFactory, FileSkeleton fileSkeleton)
		{
			_propertiesFactory = propertiesFactory;
			_documentItemFactory = documentItemFactory;
			_fileSkeleton = fileSkeleton;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			TagPair tagPair = source as TagPair;
			if (tagPair == null)
			{
				return null;
			}
			TagPairDefinition tagPairDefinition = GetTagPairDefinition(tagPair.TagPairDefinitionId);
			if (tagPairDefinition == null)
			{
				return null;
			}
			if (!QuickInsertHelper.TryCreateQuickInsertMarkupFromId(tagPairDefinition.QuickInsertId, out ITagPair quickInsertMarkup))
			{
				IStartTagProperties startTagInfo = SetStartTagProperties(tagPairDefinition, tagPair);
				IEndTagProperties endTagInfo = SetEndTagProperties(tagPairDefinition, tagPair);
				quickInsertMarkup = _documentItemFactory.CreateTagPair(startTagInfo, endTagInfo);
			}
			quickInsertMarkup.ConvertAndAddChildren(tagPair.Children, base.ConverterFactory);
			quickInsertMarkup = (SubcontentConverter.AddSubsegmentReferences(quickInsertMarkup, tagPairDefinition.SubContent, base.ConverterFactory.ContextTable, _propertiesFactory) as ITagPair);
			tagPairDefinition.SubContent?.ForEach(delegate(LocalizableSubContent x)
			{
				_fileSkeleton.SubContentPUs.Add(x.ParagraphUnitId);
			});
			return quickInsertMarkup;
		}

		private IStartTagProperties SetStartTagProperties(TagPairDefinition tagPairDef, TagPair tagPair)
		{
			IStartTagProperties startTagProperties = _propertiesFactory.CreateStartTagProperties(tagPairDef.StartTagContent);
			startTagProperties.CanHide = tagPairDef.CanHide;
			startTagProperties.DisplayText = tagPairDef.StartTagDisplayText;
			IFormattingGroup formattingGroup = ConvertFormattingGroup(tagPairDef.FormattingGroupId);
			if (formattingGroup != null)
			{
				startTagProperties.Formatting = formattingGroup;
			}
			string metadata = tagPair.GetMetadata("frameworkOriginalTagId");
			tagPair.RemoveMetadata("frameworkOriginalTagId");
			startTagProperties.SegmentationHint = tagPairDef.SegmentationHint.Convert();
			RestoreSegmentationHint(startTagProperties, tagPair);
			startTagProperties.CopyMetadataFrom(tagPair.Metadata.Where((KeyValuePair<string, string> x) => !x.Key.StartsWith("__end_")));
			SubcontentConverter.AddLocalizableSubContent(tagPairDef.SubContent, startTagProperties, base.ConverterFactory.ContextTable, _propertiesFactory);
			startTagProperties.TagId = new TagId(metadata);
			return startTagProperties;
		}

		private IEndTagProperties SetEndTagProperties(TagPairDefinition tagPairDef, TagPair tagPair)
		{
			IEndTagProperties endTagProperties = _propertiesFactory.CreateEndTagProperties(tagPairDef.EndTagContent);
			endTagProperties.CanHide = tagPairDef.CanHide;
			endTagProperties.DisplayText = tagPairDef.EndTagDisplayText;
			endTagProperties.CopyMetadataFrom(tagPair.Metadata.Where((KeyValuePair<string, string> x) => x.Key.StartsWith("__end_")), isEndTagMetadata: true);
			return endTagProperties;
		}

		private IFormattingGroup ConvertFormattingGroup(int formattingGroupId)
		{
			FormattingGroupConverter formattingGroupConverter = new FormattingGroupConverter(new FormattingItemFactory(), _fileSkeleton);
			return formattingGroupConverter.Convert(formattingGroupId);
		}

		private TagPairDefinition GetTagPairDefinition(int id)
		{
			return _fileSkeleton.TagPairDefinitions.GetById(id);
		}

		private static void RestoreSegmentationHint(IStartTagProperties props, TagPair tagPair)
		{
			string metadata = tagPair.GetMetadata("_originalSegmentationHint");
			if (metadata != null)
			{
				Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint segmentationHint2 = props.SegmentationHint = (Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint)Enum.Parse(typeof(Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint), metadata);
				tagPair.RemoveMetadata("_originalSegmentationHint");
			}
		}
	}
}
