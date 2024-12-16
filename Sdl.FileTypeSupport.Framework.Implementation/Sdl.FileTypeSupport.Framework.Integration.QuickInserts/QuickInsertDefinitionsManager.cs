using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration.QuickInserts
{
	public class QuickInsertDefinitionsManager : IQuickInsertDefinitionsManager
	{
		private enum ResourceTypes
		{
			QuickInsertName,
			QuickInsertDescription
		}

		private static class QuickInsertResources
		{
			public static string GetEnumerationString(ResourceTypes type, Enum enumeration)
			{
				string name = type.ToString() + "_" + enumeration;
				string @string = StringResources.ResourceManager.GetString(name);
				if (!string.IsNullOrEmpty(@string))
				{
					return @string;
				}
				return string.Empty;
			}
		}

		private static readonly Lazy<DocumentItemFactory> lazy = new Lazy<DocumentItemFactory>(() => new DocumentItemFactory());

		private static readonly Lazy<FormattingItemFactory> lazyFIF = new Lazy<FormattingItemFactory>(() => new FormattingItemFactory());

		private static Dictionary<QuickInsertIds, IAbstractMarkupDataContainer> _markupData = new Dictionary<QuickInsertIds, IAbstractMarkupDataContainer>
		{
			{
				QuickInsertIds.qBold,
				new MarkupDataContainer
				{
					CreateTagPair("<bold>", "</bold>", QuickInsertIds.qBold, canHide: true, new FormattingGroup
					{
						{
							"Bold",
							InstanceFIF.CreateFormattingItem("Bold", "true")
						}
					})
				}
			},
			{
				QuickInsertIds.qItalic,
				new MarkupDataContainer
				{
					CreateTagPair("<italic>", "</italic>", QuickInsertIds.qItalic, canHide: true, new FormattingGroup
					{
						{
							"Italic",
							InstanceFIF.CreateFormattingItem("Italic", "true")
						}
					})
				}
			},
			{
				QuickInsertIds.qUnderline,
				new MarkupDataContainer
				{
					CreateTagPair("<underline>", "</underline>", QuickInsertIds.qUnderline, canHide: true, new FormattingGroup
					{
						{
							"Underline",
							InstanceFIF.CreateFormattingItem("Underline", "true")
						}
					})
				}
			},
			{
				QuickInsertIds.qNonBreakingHyphen,
				new MarkupDataContainer
				{
					CreatePlaceholder("<non-breaking-hyphen/>", QuickInsertIds.qNonBreakingHyphen)
				}
			},
			{
				QuickInsertIds.qOptionalHyphen,
				new MarkupDataContainer
				{
					CreatePlaceholder("<optional-hyphen/>", QuickInsertIds.qOptionalHyphen)
				}
			},
			{
				QuickInsertIds.qSmallCaps,
				new MarkupDataContainer
				{
					CreateTagPair("<small-caps>", "</small-caps>", QuickInsertIds.qSmallCaps, canHide: false, new FormattingGroup
					{
						{
							"smallcaps",
							InstanceFIF.CreateFormattingItem("smallcaps", "true")
						}
					})
				}
			},
			{
				QuickInsertIds.qSubscript,
				new MarkupDataContainer
				{
					CreateTagPair("<subscript>", "</subscript>", QuickInsertIds.qSubscript, canHide: true, new FormattingGroup
					{
						{
							"TextPosition",
							new TextPosition(TextPosition.SuperSub.Subscript)
						}
					})
				}
			},
			{
				QuickInsertIds.qSuperscript,
				new MarkupDataContainer
				{
					CreateTagPair("<superscript>", "</superscript>", QuickInsertIds.qSuperscript, canHide: true, new FormattingGroup
					{
						{
							"TextPosition",
							new TextPosition(TextPosition.SuperSub.Superscript)
						}
					})
				}
			},
			{
				QuickInsertIds.qNoBold,
				new MarkupDataContainer
				{
					CreateTagPair("<no-bold>", "</no-bold>", QuickInsertIds.qNoBold, canHide: true, new FormattingGroup
					{
						{
							"Bold",
							InstanceFIF.CreateFormattingItem("Bold", "false")
						}
					})
				}
			},
			{
				QuickInsertIds.qNoItalic,
				new MarkupDataContainer
				{
					CreateTagPair("<no-italic>", "</no-italic>", QuickInsertIds.qNoItalic, canHide: true, new FormattingGroup
					{
						{
							"Italic",
							InstanceFIF.CreateFormattingItem("Italic", "false")
						}
					})
				}
			},
			{
				QuickInsertIds.qNoUnderline,
				new MarkupDataContainer
				{
					CreateTagPair("<no-underline>", "</no-underline>", QuickInsertIds.qNoUnderline, canHide: true, new FormattingGroup
					{
						{
							"Underline",
							InstanceFIF.CreateFormattingItem("Underline", "false")
						}
					})
				}
			},
			{
				QuickInsertIds.qNoSubscript,
				new MarkupDataContainer
				{
					CreateTagPair("<no-subscript>", "</no-subscript>", QuickInsertIds.qNoSubscript, canHide: true, new FormattingGroup
					{
						{
							"TextPosition",
							new TextPosition(TextPosition.SuperSub.Normal)
						}
					})
				}
			},
			{
				QuickInsertIds.qNoSuperscript,
				new MarkupDataContainer
				{
					CreateTagPair("<no-superscript>", "</no-superscript>", QuickInsertIds.qNoSuperscript, canHide: true, new FormattingGroup
					{
						{
							"TextPosition",
							new TextPosition(TextPosition.SuperSub.Normal)
						}
					})
				}
			},
			{
				QuickInsertIds.qStrikeThrough,
				new MarkupDataContainer
				{
					CreateTagPair("<strike-through>", "</strike-through>", QuickInsertIds.qStrikeThrough, canHide: true, new FormattingGroup
					{
						{
							"Strikethrough",
							InstanceFIF.CreateFormattingItem("Strikethrough", "true")
						}
					})
				}
			},
			{
				QuickInsertIds.qNoStrikeThrough,
				new MarkupDataContainer
				{
					CreateTagPair("<no-strike-through>", "</no-strike-through>", QuickInsertIds.qNoStrikeThrough, canHide: true, new FormattingGroup
					{
						{
							"Strikethrough",
							InstanceFIF.CreateFormattingItem("Strikethrough", "false")
						}
					})
				}
			},
			{
				QuickInsertIds.qNoSmallCaps,
				new MarkupDataContainer
				{
					CreateTagPair("<no-small-caps>", "</no-small-caps>", QuickInsertIds.qNoSmallCaps, canHide: false, new FormattingGroup
					{
						{
							"smallcaps",
							InstanceFIF.CreateFormattingItem("smallcaps", "false")
						}
					})
				}
			},
			{
				QuickInsertIds.qDoubleUnderline,
				new MarkupDataContainer
				{
					CreateTagPair("<double-underline>", "</double-underline>", QuickInsertIds.qDoubleUnderline, canHide: true, new FormattingGroup
					{
						{
							"Underline",
							InstanceFIF.CreateFormattingItem("Underline", "true")
						}
					})
				}
			},
			{
				QuickInsertIds.qNoDoubleUnderline,
				new MarkupDataContainer
				{
					CreateTagPair("<no-double-underline>", "</no-double-underline>", QuickInsertIds.qNoDoubleUnderline, canHide: true, new FormattingGroup
					{
						{
							"Underline",
							InstanceFIF.CreateFormattingItem("Underline", "false")
						}
					})
				}
			},
			{
				QuickInsertIds.qSortOrder,
				new MarkupDataContainer
				{
					CreateTagPair("<sort-order>", "</sort-order>", QuickInsertIds.qSortOrder, canHide: false, null)
				}
			}
		};

		private static Dictionary<QuickInsertIds, QuickInsert> _definitions = new Dictionary<QuickInsertIds, QuickInsert>
		{
			{
				QuickInsertIds.qBold,
				new QuickInsert(QuickInsertIds.qBold, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qBold), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qBold), _markupData[QuickInsertIds.qBold], (_markupData[QuickInsertIds.qBold][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qItalic,
				new QuickInsert(QuickInsertIds.qItalic, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qItalic), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qItalic), _markupData[QuickInsertIds.qItalic], (_markupData[QuickInsertIds.qItalic][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qUnderline,
				new QuickInsert(QuickInsertIds.qUnderline, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qUnderline), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qUnderline), _markupData[QuickInsertIds.qUnderline], (_markupData[QuickInsertIds.qUnderline][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qNonBreakingHyphen,
				new QuickInsert(QuickInsertIds.qNonBreakingHyphen, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNonBreakingHyphen), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNonBreakingHyphen), _markupData[QuickInsertIds.qNonBreakingHyphen], null, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qOptionalHyphen,
				new QuickInsert(QuickInsertIds.qOptionalHyphen, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qOptionalHyphen), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qOptionalHyphen), _markupData[QuickInsertIds.qOptionalHyphen], null, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qSubscript,
				new QuickInsert(QuickInsertIds.qSubscript, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSubscript), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSubscript), _markupData[QuickInsertIds.qSubscript], (_markupData[QuickInsertIds.qSubscript][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qSuperscript,
				new QuickInsert(QuickInsertIds.qSuperscript, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSuperscript), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSuperscript), _markupData[QuickInsertIds.qSuperscript], (_markupData[QuickInsertIds.qSuperscript][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qSmallCaps,
				new QuickInsert(QuickInsertIds.qSmallCaps, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSmallCaps), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSmallCaps), _markupData[QuickInsertIds.qSmallCaps], (_markupData[QuickInsertIds.qSmallCaps][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qStrikeThrough,
				new QuickInsert(QuickInsertIds.qStrikeThrough, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qStrikeThrough), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qStrikeThrough), _markupData[QuickInsertIds.qStrikeThrough], (_markupData[QuickInsertIds.qStrikeThrough][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qDoubleUnderline,
				new QuickInsert(QuickInsertIds.qDoubleUnderline, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qDoubleUnderline), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qDoubleUnderline), _markupData[QuickInsertIds.qDoubleUnderline], (_markupData[QuickInsertIds.qDoubleUnderline][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qSortOrder,
				new QuickInsert(QuickInsertIds.qSortOrder, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSortOrder), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSortOrder), _markupData[QuickInsertIds.qSortOrder], (_markupData[QuickInsertIds.qSortOrder][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: true)
			},
			{
				QuickInsertIds.qNoDoubleUnderline,
				new QuickInsert(QuickInsertIds.qNoDoubleUnderline, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoDoubleUnderline), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoDoubleUnderline), _markupData[QuickInsertIds.qNoDoubleUnderline], (_markupData[QuickInsertIds.qNoDoubleUnderline][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			},
			{
				QuickInsertIds.qNoSmallCaps,
				new QuickInsert(QuickInsertIds.qNoSmallCaps, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoSmallCaps), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoSmallCaps), _markupData[QuickInsertIds.qNoSmallCaps], (_markupData[QuickInsertIds.qNoSmallCaps][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			},
			{
				QuickInsertIds.qNoStrikeThrough,
				new QuickInsert(QuickInsertIds.qNoStrikeThrough, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoStrikeThrough), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoStrikeThrough), _markupData[QuickInsertIds.qNoStrikeThrough], (_markupData[QuickInsertIds.qNoStrikeThrough][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			},
			{
				QuickInsertIds.qNoBold,
				new QuickInsert(QuickInsertIds.qNoBold, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoBold), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoBold), _markupData[QuickInsertIds.qNoBold], (_markupData[QuickInsertIds.qNoBold][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			},
			{
				QuickInsertIds.qNoItalic,
				new QuickInsert(QuickInsertIds.qNoItalic, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoItalic), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoItalic), _markupData[QuickInsertIds.qNoItalic], (_markupData[QuickInsertIds.qNoItalic][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			},
			{
				QuickInsertIds.qNoUnderline,
				new QuickInsert(QuickInsertIds.qNoUnderline, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoUnderline), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoUnderline), _markupData[QuickInsertIds.qNoUnderline], (_markupData[QuickInsertIds.qNoUnderline][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			},
			{
				QuickInsertIds.qNoSubscript,
				new QuickInsert(QuickInsertIds.qNoSubscript, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoSubscript), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoSubscript), _markupData[QuickInsertIds.qNoSubscript], (_markupData[QuickInsertIds.qNoSubscript][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			},
			{
				QuickInsertIds.qNoSuperscript,
				new QuickInsert(QuickInsertIds.qNoSuperscript, QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoSuperscript), QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoSuperscript), _markupData[QuickInsertIds.qNoSuperscript], (_markupData[QuickInsertIds.qNoSuperscript][0] as ITagPair)?.StartTagProperties.Formatting, displayOnToolbar: false)
			}
		};

		private static DocumentItemFactory Instance => lazy.Value;

		private static FormattingItemFactory InstanceFIF => lazyFIF.Value;

		private static IAbstractMarkupData CreatePlaceholder(string tagContent, QuickInsertIds tagId)
		{
			IPlaceholderTag placeholderTag = Instance.CreatePlaceholderTag(Instance.PropertiesFactory.CreatePlaceholderTagProperties(tagContent));
			placeholderTag.TagProperties.TagId = new TagId(BuildQuickTagId(tagId));
			SetDisplayText(placeholderTag.TagProperties, tagId);
			return placeholderTag;
		}

		private static void SetDisplayText(IAbstractBasicTagProperties tagProperties, QuickInsertIds tagId)
		{
			tagProperties.DisplayText = QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, tagId);
		}

		private static IAbstractMarkupData CreateTagPair(string startTagContent, string endTagContent, QuickInsertIds tagId, bool canHide, IFormattingGroup formattingGroup)
		{
			IStartTagProperties startTagProperties = Instance.PropertiesFactory.CreateStartTagProperties(startTagContent);
			SetDisplayText(startTagProperties, tagId);
			startTagProperties.TagId = new TagId(BuildQuickTagId(tagId));
			startTagProperties.CanHide = canHide;
			if (formattingGroup != null)
			{
				startTagProperties.Formatting = formattingGroup;
			}
			IEndTagProperties endTagProperties = Instance.PropertiesFactory.CreateEndTagProperties(endTagContent);
			SetDisplayText(endTagProperties, tagId);
			endTagProperties.CanHide = canHide;
			return Instance.CreateTagPair(startTagProperties, endTagProperties);
		}

		public IQuickInsert BuildQuickInsert(QuickInsertIds id)
		{
			return _definitions[id];
		}

		public IQuickInsert BuildClonedQuickInsert(QuickInsertIds id)
		{
			return _definitions[id].Clone() as IQuickInsert;
		}

		public bool IsQuickInsert(IAbstractMarkupData item)
		{
			if (!(item is ITagPair) && !(item is IPlaceholderTag))
			{
				return false;
			}
			IAbstractTag abstractTag = item as IAbstractTag;
			QuickInsertIds quickTag;
			return TryParseQuickInsertId(abstractTag.TagProperties.TagId.Id, out quickTag);
		}

		public bool TryParseQuickInsertId(string tagId, out QuickInsertIds quickTag)
		{
			foreach (object value in Enum.GetValues(typeof(QuickInsertIds)))
			{
				if (BuildQuickTagId((QuickInsertIds)value) == tagId)
				{
					quickTag = (QuickInsertIds)value;
					return true;
				}
			}
			quickTag = QuickInsertIds.None;
			return false;
		}

		private static string BuildQuickTagId(QuickInsertIds id)
		{
			return id.ToString();
		}

		public List<QuickInsertIds> ParseQuickInsertIds(string quickInsertIds)
		{
			List<QuickInsertIds> list = new List<QuickInsertIds>();
			if (string.IsNullOrEmpty(quickInsertIds))
			{
				return null;
			}
			string[] array = quickInsertIds.Split(';');
			string[] array2 = array;
			foreach (string tagId in array2)
			{
				if (TryParseQuickInsertId(tagId, out QuickInsertIds quickTag))
				{
					list.Add(quickTag);
				}
			}
			return list;
		}
	}
}
