using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class PlaceholderTagConverter : MarkupDataConverter
	{
		private readonly IPropertiesFactory _propertiesFactory;

		private readonly IDocumentItemFactory _documentItemFactory;

		private readonly FileSkeleton _fileSkeleton;

		public PlaceholderTagConverter(IPropertiesFactory propertiesFactory, IDocumentItemFactory documentItemFactory, FileSkeleton fileSkeleton)
		{
			_propertiesFactory = propertiesFactory;
			_documentItemFactory = documentItemFactory;
			_fileSkeleton = fileSkeleton;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			PlaceholderTag placeholderTag = source as PlaceholderTag;
			if (placeholderTag == null)
			{
				return null;
			}
			PlaceholderTagDefinition placeholderTagDefinition = GetPlaceholderTagDefinition(placeholderTag.PlaceholderTagDefinitionId);
			if (placeholderTagDefinition == null)
			{
				return null;
			}
			if (!QuickInsertHelper.TryCreateQuickInsertMarkupFromId(placeholderTagDefinition.QuickInsertId, out IPlaceholderTag quickInsertMarkup))
			{
				IPlaceholderTagProperties placeholderTagProperties = SetTagProperties(placeholderTagDefinition);
				RestoreOriginalProperties(source, placeholderTagProperties, placeholderTag);
				placeholderTagProperties.CopyMetadataFrom(placeholderTag.Metadata);
				quickInsertMarkup = _documentItemFactory.CreatePlaceholderTag(placeholderTagProperties);
			}
			quickInsertMarkup = (SubcontentConverter.AddSubsegmentReferences(quickInsertMarkup, placeholderTagDefinition.SubContent, base.ConverterFactory.ContextTable, _propertiesFactory) as IPlaceholderTag);
			placeholderTagDefinition.SubContent?.ForEach(delegate(LocalizableSubContent x)
			{
				_fileSkeleton.SubContentPUs.Add(x.ParagraphUnitId);
			});
			return quickInsertMarkup;
		}

		private void RestoreOriginalProperties(MarkupData source, IPlaceholderTagProperties props, PlaceholderTag placeholder)
		{
			RestoreFrameworkOriginalTagId(props, source);
			RestoreSegmentationHint(props, placeholder);
		}

		private static void RestoreFrameworkOriginalTagId(IAbstractTagProperties props, MetadataContainer sourceMetadata)
		{
			string metadata = sourceMetadata.GetMetadata("frameworkOriginalTagId");
			sourceMetadata.RemoveMetadata("frameworkOriginalTagId");
			props.TagId = new TagId(metadata);
		}

		private IPlaceholderTagProperties SetTagProperties(PlaceholderTagDefinition tagDef)
		{
			IPlaceholderTagProperties placeholderTagProperties = _propertiesFactory.CreatePlaceholderTagProperties(tagDef.TagContent);
			placeholderTagProperties.DisplayText = tagDef.DisplayText;
			placeholderTagProperties.TextEquivalent = tagDef.TextEquivalent;
			placeholderTagProperties.SegmentationHint = tagDef.SegmentationHint.Convert();
			SubcontentConverter.AddLocalizableSubContent(tagDef.SubContent, placeholderTagProperties, base.ConverterFactory.ContextTable, _propertiesFactory);
			return placeholderTagProperties;
		}

		private static void RestoreSegmentationHint(IPlaceholderTagProperties props, PlaceholderTag placeholder)
		{
			string metadata = placeholder.GetMetadata("_originalSegmentationHint");
			if (metadata != null)
			{
				Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint segmentationHint2 = props.SegmentationHint = (Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint)Enum.Parse(typeof(Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint), metadata);
				placeholder.RemoveMetadata("_originalSegmentationHint");
			}
		}

		private PlaceholderTagDefinition GetPlaceholderTagDefinition(int id)
		{
			return _fileSkeleton.PlaceholderTagDefinitions.GetById(id);
		}
	}
}
