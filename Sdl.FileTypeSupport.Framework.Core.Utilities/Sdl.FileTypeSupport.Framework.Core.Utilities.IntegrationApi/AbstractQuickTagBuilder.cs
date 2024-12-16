#define TRACE
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Properties;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	public abstract class AbstractQuickTagBuilder
	{
		private const string ImplementationAssemblyName = "Sdl.FileTypeSupport.Framework.Implementation";

		private const string QuickTagClassName = "Sdl.FileTypeSupport.Framework.Integration.QuickTag";

		private const string QuickTagsClassName = "Sdl.FileTypeSupport.Framework.Integration.QuickTags";

		private IDocumentItemFactory _itemFactory;

		public IPropertiesFactory PropertiesFactory
		{
			get
			{
				if (_itemFactory == null)
				{
					return null;
				}
				return _itemFactory.PropertiesFactory;
			}
			set
			{
				if (_itemFactory != null)
				{
					_itemFactory.PropertiesFactory = value;
				}
			}
		}

		public IDocumentItemFactory ItemFactory
		{
			get
			{
				return _itemFactory;
			}
			set
			{
				_itemFactory = value;
			}
		}

		public AbstractQuickTagBuilder()
		{
			_itemFactory = DefaultDocumentItemFactory.CreateInstance();
			if (_itemFactory.PropertiesFactory == null)
			{
				_itemFactory.PropertiesFactory = DefaultPropertiesFactory.CreateInstance();
			}
		}

		public IQuickTag CreateDefaultTagPair(QuickTagDefaultId id, string startTagContent, string endTagContent, string displayText)
		{
			IDefaultQuickTagInfo defaultQuickTagInfo = QuickTagDefaults.GetDefaultQuickTagInfo(id);
			return CreateTagPair(defaultQuickTagInfo.CommandID, startTagContent, endTagContent, displayText, defaultQuickTagInfo.CommandName, defaultQuickTagInfo.Description, defaultQuickTagInfo.ImageResource, defaultQuickTagInfo.ImagePath, displayOnToolbar: true, defaultQuickTagInfo.Formatting, HasVisibleFormatting(defaultQuickTagInfo.Formatting), isSoftBreak: true, defaultQuickTagInfo.Formatting.Count == 0);
		}

		private bool HasVisibleFormatting(IFormattingGroup formatting)
		{
			return formatting.Any((KeyValuePair<string, IFormattingItem> pair) => !(pair.Value is UnknownFormatting));
		}

		public IQuickTag CreateTagPair(string commandId, string startTagContent, string endTagContent, string displayText, string commandName, string description, string iconResource, string iconPath, bool displayOnToolbar, IFormattingGroup formatting, bool canHide, bool isSoftBreak, bool isWordStop)
		{
			IQuickTag quickTag = CreateQuickTag(commandId, commandName, description, displayOnToolbar, iconResource, iconPath);
			IStartTagProperties startTagProperties = PropertiesFactory.CreateStartTagProperties(startTagContent);
			if (formatting != null)
			{
				startTagProperties.Formatting = formatting;
			}
			startTagProperties.CanHide = canHide;
			startTagProperties.DisplayText = displayText;
			startTagProperties.IsSoftBreak = isSoftBreak;
			startTagProperties.IsWordStop = isWordStop;
			IEndTagProperties endTagProperties = PropertiesFactory.CreateEndTagProperties(endTagContent);
			endTagProperties.CanHide = startTagProperties.CanHide;
			endTagProperties.DisplayText = startTagProperties.DisplayText;
			endTagProperties.IsSoftBreak = startTagProperties.IsSoftBreak;
			endTagProperties.IsWordStop = startTagProperties.IsWordStop;
			ILocationMarker locationMarker = _itemFactory.CreateLocationMarker();
			locationMarker.MarkerId = new LocationMarkerId("Content_Goes_Here");
			ITagPair tagPair = _itemFactory.CreateTagPair(startTagProperties, endTagProperties);
			tagPair.Add(locationMarker);
			quickTag.MarkupDataContent.Add(tagPair);
			return quickTag;
		}

		public IQuickTag CreateDefaultPlaceholder(QuickTagDefaultId id, string tagContent, string displayText, bool isWordStop, bool isSoftBreak)
		{
			IDefaultQuickTagInfo defaultQuickTagInfo = QuickTagDefaults.GetDefaultQuickTagInfo(id);
			return CreatePlaceholder(defaultQuickTagInfo.CommandID, tagContent, displayText, defaultQuickTagInfo.CommandName, defaultQuickTagInfo.Description, displayOnToolbar: true, defaultQuickTagInfo.ImageResource, defaultQuickTagInfo.ImagePath, isSoftBreak, isWordStop, displayText);
		}

		public IQuickTag CreatePlaceholder(string commandId, string tagContent, string displayText, string commandName, string description, bool displayOnToolbar, string iconResource, string iconPath, bool isSoftBreak, bool isWordStop, string textEquivalent)
		{
			IQuickTag quickTag = CreateQuickTag(commandId, commandName, description, displayOnToolbar, iconResource, iconPath);
			IPlaceholderTagProperties placeholderTagProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
			placeholderTagProperties.DisplayText = displayText;
			placeholderTagProperties.CanHide = false;
			placeholderTagProperties.IsSoftBreak = isSoftBreak;
			placeholderTagProperties.IsWordStop = isWordStop;
			placeholderTagProperties.TextEquivalent = textEquivalent;
			IPlaceholderTag item = _itemFactory.CreatePlaceholderTag(placeholderTagProperties);
			quickTag.MarkupDataContent.Add(item);
			return quickTag;
		}

		public IQuickTag CreateTextPair(string commandId, string commandName, string description, string startContent, string endContent, string displayText, string iconResource, string iconPath)
		{
			IQuickTag quickTag = CreateQuickTag(commandId, commandName, description, displayOnToolbar: true, iconResource, iconPath);
			ITextProperties textInfo = PropertiesFactory.CreateTextProperties(startContent);
			ITextProperties textInfo2 = PropertiesFactory.CreateTextProperties(endContent);
			ILocationMarker locationMarker = ItemFactory.CreateLocationMarker();
			locationMarker.MarkerId = new LocationMarkerId("Content_Goes_Here");
			quickTag.MarkupDataContent.Add(ItemFactory.CreateText(textInfo));
			quickTag.MarkupDataContent.Add(locationMarker);
			quickTag.MarkupDataContent.Add(ItemFactory.CreateText(textInfo2));
			return quickTag;
		}

		public IQuickTag CreateTextPlaceholder(string commandId, string textContent, string displayText, string commandName, string description)
		{
			IQuickTag quickTag = CreateQuickTag(commandId, commandName, description, displayOnToolbar: false, null, null);
			ITextProperties textInfo = PropertiesFactory.CreateTextProperties(textContent);
			quickTag.MarkupDataContent.Add(ItemFactory.CreateText(textInfo));
			return quickTag;
		}

		protected IList<IQuickTag> CreateDefaultBidiQuickTags()
		{
			IList<IQuickTag> list = new List<IQuickTag>();
			list.Add(CreateTextPlaceholder("LRM", '\u200e'.ToString(), "LRM", Resources.QuickTag_LRM_Name, Resources.QuickTag_LRM_Description));
			list.Add(CreateTextPlaceholder("RLM", '\u200f'.ToString(), "RLM", Resources.QuickTag_RLM_Name, Resources.QuickTag_RLM_Description));
			list.Add(CreateTextPlaceholder("LRE", '\u202a'.ToString(), "LRE", Resources.QuickTag_LRE_Name, Resources.QuickTag_LRE_Description));
			list.Add(CreateTextPlaceholder("RLE", '\u202b'.ToString(), "RLE", Resources.QuickTag_RLE_Name, Resources.QuickTag_RLE_Description));
			list.Add(CreateTextPlaceholder("LRO", '\u202d'.ToString(), "LRO", Resources.QuickTag_LRO_Name, Resources.QuickTag_LRO_Description));
			list.Add(CreateTextPlaceholder("RLO", '\u202e'.ToString(), "RLO", Resources.QuickTag_RLO_Name, Resources.QuickTag_RLO_Description));
			list.Add(CreateTextPlaceholder("PDF", '\u202c'.ToString(), "PDF", Resources.QuickTag_PDF_Name, Resources.QuickTag_PDF_Description));
			return list;
		}

		protected IQuickTag CreateQuickTag(string commandId, string commandName, string description, bool displayOnToolbar, string iconResource, string iconPath)
		{
			IQuickTag quickTag = CreateQuickTagInstance();
			quickTag.CommandId = commandId;
			quickTag.CommandName = new LocalizableString(commandName);
			quickTag.Description = new LocalizableString(description);
			quickTag.DisplayOnToolBar = displayOnToolbar;
			if (iconResource != null && iconPath != null)
			{
				quickTag.Icon = new IconDescriptor("assembly://" + iconResource + "/" + iconPath);
			}
			return quickTag;
		}

		public IQuickTags CreateQuickTags()
		{
			try
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Sdl.FileTypeSupport.Framework.Implementation";
				return Assembly.Load(assemblyName).CreateInstance("Sdl.FileTypeSupport.Framework.Integration.QuickTags") as IQuickTags;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Failed to create quick tags implementation.\r\n\r\n{ex.ToString()}");
				throw;
			}
		}

		protected static IQuickTag CreateQuickTagInstance()
		{
			try
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Sdl.FileTypeSupport.Framework.Implementation";
				return Assembly.Load(assemblyName).CreateInstance("Sdl.FileTypeSupport.Framework.Integration.QuickTag") as IQuickTag;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Failed to create quick tag implementation.\r\n\r\n{ex.ToString()}");
				throw;
			}
		}

		public IFormattingGroup CreateFormatting(params IFormattingItem[] formattingItems)
		{
			IFormattingGroup formattingGroup = PropertiesFactory.FormattingItemFactory.CreateFormatting();
			foreach (IFormattingItem formatting in formattingItems)
			{
				formattingGroup.Add(formatting);
			}
			return formattingGroup;
		}
	}
}
