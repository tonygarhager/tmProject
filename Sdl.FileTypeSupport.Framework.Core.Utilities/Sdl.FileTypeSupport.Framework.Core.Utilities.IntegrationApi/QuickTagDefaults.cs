using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Properties;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	public static class QuickTagDefaults
	{
		public const string ContentPlaceholderId = "Content_Goes_Here";

		public const string DefaultImageResource = "Sdl.FileTypeSupport.Filters.Resources";

		public const string DefaultImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagDefault.ico";

		private const string QuickTag_BoldImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagBold.ico";

		private const string QuickTag_ItalicImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagItalic.ico";

		private const string QuickTag_UnderlineImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagUnderline.ico";

		private const string QuickTag_SubscriptImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagSubscript.ico";

		private const string QuickTag_SuperscriptImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagSuperscript.ico";

		private const string QuickTag_SmallCapsImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagSmallCaps.ico";

		private const string QuickTag_LeftToRightImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagLeftToRight.ico";

		private const string QuickTag_RightToLeftImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagRightToLeft.ico";

		private const string QuickTag_EmDashImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagEMDash.ico";

		private const string QuickTag_EnDashImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagENDash.ico";

		private const string QuickTag_NonBreakHyphenImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagNBHyphen.ico";

		private const string QuickTag_OptionalHyphenImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagOptionalHyphen.ico";

		private const string QuickTag_NonBreakSpaceImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagNBSpace.ico";

		private const string QuickTag_EuroImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagEuro.ico";

		private const string QuickTag_CopyrightImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagCopyright.ico";

		private const string QuickTag_RegisteredImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagRegistered.ico";

		private const string QuickTag_TrademarkImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagTrademark.ico";

		private const string QuickTag_SoftbreakImageLocation = "Sdl.FileTypeSupport.Filters.Resources.quickTagSoftBreak.ico";

		private static List<DefaultQuickTagInfo> _DefaultQuickTagList = new List<DefaultQuickTagInfo>();

		private static IFormattingItemFactory _factory = new FormattingItemFactory();

		private static DefaultQuickTagInfo[] _DefaultQuickTags = new DefaultQuickTagInfo[18]
		{
			new DefaultQuickTagInfo(Resources.QuickTag_Bold_Name, QuickTagDefaultId.Bold.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagBold.ico", Resources.QuickTag_Bold_Description, new Bold()),
			new DefaultQuickTagInfo(Resources.QuickTag_Italic_Name, QuickTagDefaultId.Italic.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagItalic.ico", Resources.QuickTag_Italic_Description, new Italic()),
			new DefaultQuickTagInfo(Resources.QuickTag_Underline_Name, QuickTagDefaultId.Underline.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagUnderline.ico", Resources.QuickTag_Underline_Description, new Underline()),
			new DefaultQuickTagInfo(Resources.QuickTag_Subscript_Name, QuickTagDefaultId.Subscript.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagSubscript.ico", Resources.QuickTag_Subscript_Description, new TextPosition(TextPosition.SuperSub.Subscript)),
			new DefaultQuickTagInfo(Resources.QuickTag_Superscript_Name, QuickTagDefaultId.Superscript.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagSuperscript.ico", Resources.QuickTag_Superscript_Description, new TextPosition(TextPosition.SuperSub.Superscript)),
			new DefaultQuickTagInfo(Resources.QuickTag_SmallCaps_Name, QuickTagDefaultId.SmallCaps.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagSmallCaps.ico", Resources.QuickTag_SmallCaps_Description, _factory.CreateFormattingItem("smallcaps", "on")),
			new DefaultQuickTagInfo(Resources.QuickTag_SoftBreak_Name, QuickTagDefaultId.SoftBreak.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagSoftBreak.ico", Resources.QuickTag_SoftBreak_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_LeftToRight_Name, QuickTagDefaultId.LeftToRight.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagLeftToRight.ico", Resources.QuickTag_LeftToRight_Description, new TextDirection(Direction.LeftToRight)),
			new DefaultQuickTagInfo(Resources.QuickTag_RightToLeft_Name, QuickTagDefaultId.RightToLeft.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagRightToLeft.ico", Resources.QuickTag_RightToLeft_Description, new TextDirection(Direction.RightToLeft)),
			new DefaultQuickTagInfo(Resources.QuickTag_EmDash_Name, QuickTagDefaultId.EmDash.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagEMDash.ico", Resources.QuickTag_EmDash_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_EnDash_Name, QuickTagDefaultId.EnDash.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagENDash.ico", Resources.QuickTag_EnDash_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_NonBreakHyphen_Name, QuickTagDefaultId.NonBreakingHyphen.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagNBHyphen.ico", Resources.QuickTag_NonBreakHyphen_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_OptionalHyphen_Name, QuickTagDefaultId.OptionalHyphen.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagOptionalHyphen.ico", Resources.QuickTag_OptionalHyphen_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_NonBreakSpace_Name, QuickTagDefaultId.NonBreakingSpace.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagNBSpace.ico", Resources.QuickTag_NonBreakSpace_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_Euro_Name, QuickTagDefaultId.Euro.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagEuro.ico", Resources.QuickTag_Euro_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_Copyright_Name, QuickTagDefaultId.Copyright.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagCopyright.ico", Resources.QuickTag_Copyright_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_Registered_Name, QuickTagDefaultId.Registered.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagRegistered.ico", Resources.QuickTag_Registered_Description, null),
			new DefaultQuickTagInfo(Resources.QuickTag_Trademark_Name, QuickTagDefaultId.Trademark.ToString(), "Sdl.FileTypeSupport.Filters.Resources", "Sdl.FileTypeSupport.Filters.Resources.quickTagTrademark.ico", Resources.QuickTag_Trademark_Description, null)
		};

		public static string[] QuickTagCommandNames = new string[18]
		{
			Resources.QuickTag_Bold_Name,
			Resources.QuickTag_Italic_Name,
			Resources.QuickTag_Underline_Name,
			Resources.QuickTag_Subscript_Name,
			Resources.QuickTag_Superscript_Name,
			Resources.QuickTag_SmallCaps_Name,
			Resources.QuickTag_SoftBreak_Name,
			Resources.QuickTag_LeftToRight_Name,
			Resources.QuickTag_RightToLeft_Name,
			Resources.QuickTag_EmDash_Name,
			Resources.QuickTag_EnDash_Name,
			Resources.QuickTag_NonBreakHyphen_Name,
			Resources.QuickTag_OptionalHyphen_Name,
			Resources.QuickTag_NonBreakSpace_Name,
			Resources.QuickTag_Euro_Name,
			Resources.QuickTag_Copyright_Name,
			Resources.QuickTag_Registered_Name,
			Resources.QuickTag_Trademark_Name
		};

		public static List<DefaultQuickTagInfo> DefaultQuickTagList
		{
			get
			{
				lock (typeof(QuickTagDefaults))
				{
					if (_DefaultQuickTagList.Count == 0)
					{
						_DefaultQuickTagList.AddRange(DefaultQuickTags);
					}
				}
				return _DefaultQuickTagList;
			}
		}

		public static DefaultQuickTagInfo[] DefaultQuickTags => _DefaultQuickTags;

		public static IDefaultQuickTagInfo GetDefaultQuickTagInfo(QuickTagDefaultId id)
		{
			int num = (int)id;
			if (num < 0 || num >= _DefaultQuickTags.Length)
			{
				throw new ArgumentOutOfRangeException($"Invalid ID: {id.ToString()} = {num}. Must be a value between 0 ({_DefaultQuickTags[0].CommandID}) and {_DefaultQuickTags.Length - 1} ({_DefaultQuickTags[_DefaultQuickTags.Length - 1].CommandID}).");
			}
			return _DefaultQuickTags[(int)id];
		}
	}
}
