using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Implementation.Integration
{
	public class QuickInsertsInflator
	{
		public static IList<IQuickTag> InflateQuickInserts(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			IList<IQuickTag> list = new List<IQuickTag>();
			QuickInsertsSettings quickInsertsSettings = new QuickInsertsSettings();
			quickInsertsSettings.PopulateFromSettingsBundle(settingsBundle, fileTypeConfigurationId);
			foreach (QuickInsertSettings quickInsert in quickInsertsSettings.QuickInserts)
			{
				IQuickTag item = MapQuickInsertSettingsToQuickInsert(quickInsert);
				list.Add(item);
			}
			return list;
		}

		protected static IQuickTag MapQuickInsertSettingsToQuickInsert(QuickInsertSettings settings)
		{
			IQuickTag quickTag = new QuickTag();
			quickTag.CommandId = settings.CommandId;
			quickTag.CommandName = new LocalizableString(settings.CommandName);
			quickTag.Description = new LocalizableString(settings.Description);
			quickTag.DisplayOnToolBar = settings.DisplayOnToolbar;
			GetMarkupDataFromSettings(quickTag.MarkupDataContent, settings.Markup);
			return quickTag;
		}

		protected static void GetMarkupDataFromSettings(IAbstractMarkupDataContainer container, BaseMarkupDataType markup)
		{
			IDocumentItemFactory documentItemFactory = new DocumentItemFactory();
			IPropertiesFactory propertiesFactory = new PropertiesFactory();
			if (markup is TextMarkup)
			{
				TextMarkup textMarkup = markup as TextMarkup;
				IText item = documentItemFactory.CreateText(propertiesFactory.CreateTextProperties(textMarkup.Text));
				container.Add(item);
			}
			else if (markup is TextPairMarkup)
			{
				TextPairMarkup textPairMarkup = markup as TextPairMarkup;
				IText item2 = documentItemFactory.CreateText(propertiesFactory.CreateTextProperties(textPairMarkup.StartText));
				IText item3 = documentItemFactory.CreateText(propertiesFactory.CreateTextProperties(textPairMarkup.EndText));
				ILocationMarker locationMarker = documentItemFactory.CreateLocationMarker();
				locationMarker.MarkerId = new LocationMarkerId("Content_Goes_Here");
				container.Add(item2);
				container.Add(locationMarker);
				container.Add(item3);
			}
			else if (markup is PlaceholderTagMarkup)
			{
				PlaceholderTagMarkup placeholderTagMarkup = markup as PlaceholderTagMarkup;
				IPlaceholderTagProperties placeholderTagProperties = propertiesFactory.CreatePlaceholderTagProperties("<" + placeholderTagMarkup.Text + " />");
				placeholderTagProperties.TextEquivalent = placeholderTagMarkup.TextEquivalent;
				placeholderTagProperties.DisplayText = placeholderTagMarkup.Text;
				IPlaceholderTag item4 = documentItemFactory.CreatePlaceholderTag(placeholderTagProperties);
				container.Add(item4);
			}
			else if (markup is TagPairMarkup)
			{
				TagPairMarkup tagPairMarkup = markup as TagPairMarkup;
				IStartTagProperties startTagProperties = propertiesFactory.CreateStartTagProperties("<" + tagPairMarkup.StartTagText + ">");
				if (tagPairMarkup.Formatting != null && tagPairMarkup.Formatting.FormattingItems.Count > 0)
				{
					startTagProperties.Formatting = FormattingInflator.InflateFormatting(tagPairMarkup.Formatting);
				}
				startTagProperties.CanHide = tagPairMarkup.CanHide;
				startTagProperties.DisplayText = tagPairMarkup.StartTagText;
				IEndTagProperties endTagProperties = propertiesFactory.CreateEndTagProperties("</" + tagPairMarkup.EndTagText + ">");
				endTagProperties.CanHide = startTagProperties.CanHide;
				endTagProperties.DisplayText = startTagProperties.DisplayText;
				ILocationMarker locationMarker2 = documentItemFactory.CreateLocationMarker();
				locationMarker2.MarkerId = new LocationMarkerId("Content_Goes_Here");
				ITagPair tagPair = documentItemFactory.CreateTagPair(startTagProperties, endTagProperties);
				tagPair.Add(locationMarker2);
				container.Add(tagPair);
			}
		}
	}
}
