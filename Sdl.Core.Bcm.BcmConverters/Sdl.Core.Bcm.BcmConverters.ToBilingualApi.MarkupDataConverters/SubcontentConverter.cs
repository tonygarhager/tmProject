using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal static class SubcontentConverter
	{
		internal static void AddLocalizableSubContent(IEnumerable<LocalizableSubContent> subContents, IAbstractTagProperties props, ContextTable contextTable, IPropertiesFactory propertiesFactory)
		{
			if (subContents != null)
			{
				foreach (LocalizableSubContent subContent in subContents)
				{
					SubSegmentProperties subSegmentProperties = new SubSegmentProperties
					{
						Length = subContent.Length,
						StartOffset = subContent.SourceTagContentOffset
					};
					SetContextsToTagProperties(contextTable, propertiesFactory, subContent, subSegmentProperties);
					props.AddSubSegment(subSegmentProperties);
				}
			}
		}

		internal static IAbstractTag AddSubsegmentReferences(IAbstractTag tag, IEnumerable<LocalizableSubContent> subContents, ContextTable contextTable, IPropertiesFactory propertiesFactory)
		{
			if (subContents == null)
			{
				return tag;
			}
			foreach (LocalizableSubContent subContent in subContents)
			{
				SubSegmentReference subSegmentReference = new SubSegmentReference
				{
					ParagraphUnitId = new ParagraphUnitId(subContent.ParagraphUnitId),
					Properties = new SubSegmentProperties
					{
						StartOffset = subContent.SourceTagContentOffset,
						Length = subContent.Length
					}
				};
				SetContextsToSubsegmentReference(contextTable, propertiesFactory, subContent, subSegmentReference);
				tag.AddSubSegmentReference(subSegmentReference);
			}
			return tag;
		}

		private static void SetContextsToTagProperties(ContextTable contextTable, IPropertiesFactory propertiesFactory, LocalizableSubContent localizableSubContent, SubSegmentProperties item)
		{
			if (contextTable != null)
			{
				ContextPropertiesItem lastCtxInfoItem = new ContextPropertiesItem();
				bool changeContext;
				Tuple<ContextPropertiesItem, IContextProperties> contextProperties = contextTable.GetContextProperties(localizableSubContent.ParagraphUnitId, propertiesFactory, ref lastCtxInfoItem, out changeContext);
				if (((contextProperties != null) ? contextProperties.Item2 : null) != null)
				{
					item.Contexts = (contextProperties.Item2.Clone() as IContextProperties);
				}
			}
		}

		private static void SetContextsToSubsegmentReference(ContextTable contextTable, IPropertiesFactory propertiesFactory, LocalizableSubContent localizableSubContent, SubSegmentReference item)
		{
			if (contextTable != null)
			{
				ContextPropertiesItem lastCtxInfoItem = new ContextPropertiesItem();
				bool changeContext;
				Tuple<ContextPropertiesItem, IContextProperties> contextProperties = contextTable.GetContextProperties(localizableSubContent.ParagraphUnitId, propertiesFactory, ref lastCtxInfoItem, out changeContext);
				if (((contextProperties != null) ? contextProperties.Item2 : null) != null)
				{
					item.Properties.Contexts = (contextProperties.Item2.Clone() as IContextProperties);
				}
			}
		}
	}
}
