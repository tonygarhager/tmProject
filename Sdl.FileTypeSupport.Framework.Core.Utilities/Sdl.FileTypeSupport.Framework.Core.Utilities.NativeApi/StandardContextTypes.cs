using Sdl.FileTypeSupport.Framework.Core.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi
{
	public static class StandardContextTypes
	{
		public class ContextData : ICloneable
		{
			public string Name
			{
				get;
				set;
			}

			public string Code
			{
				get;
				set;
			}

			public string Description
			{
				get;
				set;
			}

			public Color Color
			{
				get;
				set;
			}

			public ContextData(string name, string code, string description, Color color)
			{
				Name = name;
				Code = code;
				Description = description;
				Color = color;
			}

			public ContextData(ContextData other)
			{
				Name = other.Name;
				Code = other.Code;
				Description = other.Description;
				Color = other.Color;
			}

			public object Clone()
			{
				return new ContextData(this);
			}
		}

		public const string Table = "sdl:table";

		public const string TableRow = "sdl:table-row";

		public const string Paragraph = "sdl:paragraph";

		public const string Footnote = "sdl:footnote";

		public const string KeywordList = "sdl:keywords";

		public const string MasterPage = "sdl:masterpage";

		public const string Sidebar = "sdl:sidebar";

		public const string Script = "sdl:script";

		public const string Callout = "sdl:callout";

		public const string Field = "sdl:field";

		public const string ParagraphUnitReference = "sdl:transunit-ref";

		public const string Comment = "sdl:comment";

		public const string BookmarkName = "sdl:bookmark-name";

		public const string DisplayContent = "sdl:display-content";

		public const string EndNote = "sdl:endnote";

		public const string Group = "sdl:group";

		public const string Hyperlink = "sdl:hyperlink";

		public const string Slide = "sdl:slide";

		public const string TranslatableContent = "sdl:translatable-content";

		public const string TranslatableFieldOption = "sdl:translatable-fieldoption";

		public const string WordArt = "sdl:wordart";

		public const string ID = "sdl:id";

		public const string WorkSheet = "sdl:worksheet";

		public const string WorkSheetName = "sdl:worksheet-name";

		public const string ChartSheet = "sdl:chartsheet";

		public const string Title = "sdl:title";

		public const string Cell = "sdl:cell";

		public const string StringTable = "sdl:string-table";

		public const string Menus = "sdl:menus";

		public const string Menu = "sdl:menu";

		public const string Items = "sdl:items";

		public const string Item = "sdl:item";

		public const string Dialogs = "sdl:dialogs";

		public const string Dialog = "sdl:dialog";

		public const string Caption = "sdl:caption";

		public const string VersionInfo = "sdl:version-info";

		public const string Copyright = "sdl:copyright";

		public const string Trademark = "sdl:trademark";

		public const string ProductVersion = "sdl:product-version";

		public const string ProductName = "sdl:product-name";

		public const string Button = "sdl:button";

		public const string RadioButton = "sdl:radio-button";

		public const string CheckBox = "sdl:checkbox";

		public const string ComboBox = "sdl:combobox";

		public const string GroupBox = "sdl:groupbox";

		public const string ListBox = "sdl:listbox";

		public const string Shortcut = "sdl:shortcut";

		public const string MasterSpread = "sdl:master-spread";

		public const string CrossReferenceContent = "sdl:x-ref-content";

		public const string Spread = "sdl:spread";

		public const string ContentFlow = "sdl:content-flow";

		public const string NormalSpread = "sdl:normal-spread";

		public const string ExternalLink = "sdl:external-link";

		public const string HyperlinkDestination = "sdl:hyperlink-destination";

		public const string Topic = "sdl:topic";

		public const string Note = "sdl:note";

		public const string Address = "sdl:address";

		public const string DocumentLink = "sdl:documentLink";

		public const string IndexTopics = "sdl:index-topics";

		public const string CData = "sdl:cdata";

		public const string Element = "sdl:element";

		public const string BookmarkReference = "sdl:bookmark-reference";

		public const string CreateIndex = "sdl:create-index";

		public const string TextBox = "sdl:textbox";

		public const string TableOfContentsEntry = "sdl:table-of-contents-entry";

		public const string CreateTableOfContents = "sdl:create-table-of-contents";

		public const string CreateTableOfAuthorities = "sdl:create-table-of-authorities";

		public const string MarkTableOfAuthoritiesEntry = "sdl:mark-table-of-authorities-entry";

		public const string UnknownKeyword = "sdl:unknown-keyword";

		public const string PossibleBookmark = "sdl:possible-bookmark";

		public const string FootnoteReference = "sdl:footnote-reference";

		public const string SetCommand = "sdl:set-command";

		public const string IfCommand = "sdl:if-command";

		public const string StyleReference = "sdl:style-reference";

		public const string DocumentReference = "sdl:document-reference";

		public const string SequenceMark = "sdl:sequence-mark";

		public const string QuoteInfoVariable = "sdl:quote-info-variable";

		public const string QuoteTitleVariable = "sdl:quote-title-variable";

		public const string QuoteSubjectVariable = "sdl:quote-subject-variable";

		public const string QuoteAuthorVariable = "sdl:quote-author-variable";

		public const string QuoteKeywordsVariable = "sdl:quote-keywords-variable";

		public const string QuoteCommentsVariable = "sdl:quote-comments-variable";

		public const string QuoteLastRevisedByVariable = "sdl:quote-last-revised-by-variable";

		public const string QuoteCreationDateVariable = "sdl:quote-creation-date-variable";

		public const string QuoteRevisionDateVariable = "sdl:quote-revision-date-variable";

		public const string QuotePrintDateVariable = "sdl:quote-print-date-variable";

		public const string QuoteRevisionNumberVariable = "sdl:quote-revision-number-variable";

		public const string QuoteEditTimeVariable = "sdl:quote-edit-time-variable";

		public const string QuoteNumberOfPagesVariable = "sdl:quote-number-of-pages-variable";

		public const string QuoteNumberOfWordsVariable = "sdl:quote-number-of-words-variable";

		public const string QuoteNumberOfCharactersVariable = "sdl:quote-number-of-characters-variable";

		public const string QuoteFileNameVariable = "sdl:quote-file-name-variable";

		public const string QuoteDocumentTemplateNameVariable = "sdl:quote-document-template-name-variable";

		public const string QuoteCurrentDateVariable = "sdl:quote-current-date-variable";

		public const string QuoteCurrentTimeVariable = "sdl:quote-current-time-variable";

		public const string QuoteCurrentPageVariable = "sdl:quote-current-page-variable";

		public const string EvaluateExpression = "sdl:evaluate-expression";

		public const string InsertLiteralText = "sdl:insert-literal-text";

		public const string IncludeCommand = "sdl:include-command";

		public const string PageReference = "sdl:page-reference";

		public const string AskCommand = "sdl:ask-command";

		public const string FillInCommandToDisplayPrompt = "sdl:fill-in-command-to-display-prompt";

		public const string DataCommand = "sdl:data-command";

		public const string NextCommand = "sdl:next-command";

		public const string NextIfCommand = "sdl:nextif-command";

		public const string SkipIf = "sdl:skipif";

		public const string InsertsNumberOfCurrentPrintMergeRecord = "sdl:inserts-number-of-current-print-merge-record";

		public const string DDEReference = "sdl:dde-reference";

		public const string DDEAutomaticReference = "sdl:dde-automatic-reference";

		public const string InsertsGlossaryEntry = "sdl:inserts-glossary-entry";

		public const string SendsCharactersToPrinterWithoutTranslation = "sdl:sends-characters-to-printer-without-translation";

		public const string FormulaDefinition = "sdl:formula-definition";

		public const string GotoButton = "sdl:goto-button";

		public const string MacroButton = "sdl:macro-button";

		public const string InsertAutoNumberingFieldInOutlineFormat = "sdl:insert-auto-numbering-field-in-outline-format";

		public const string InsertAutoNumberingFieldInLegalFormat = "sdl:insert-auto-numbering-field-in-legal-format";

		public const string InsertAutoNumberingFieldInArabicNumberlFormat = "sdl:insert-auto-numbering-field-in-arabic-number-format";

		public const string ReadsATiffFile = "sdl:reads-a-tiff-file";

		public const string Link = "sdl:link";

		public const string Symbol = "sdl:symbol";

		public const string EmbeddedObject = "sdl:embedded-object";

		public const string MergeFields = "sdl:merge-fields";

		public const string UserName = "sdl:user-name";

		public const string UserInitial = "sdl:user-initial";

		public const string UserAddress = "sdl:user-address";

		public const string BarCode = "sdl:bar-code";

		public const string DocumentVariable = "sdl:document-variable";

		public const string Section = "sdl:section";

		public const string SectionPages = "sdl:section-pages";

		public const string IncludePicture = "sdl:include-picture";

		public const string IncludeText = "sdl:include-text";

		public const string FileSize = "sdl:file-size";

		public const string FormTextBox = "sdl:form-text-box";

		public const string FormCheckBox = "sdl:form-check-box";

		public const string NoteReference = "sdl:note-reference";

		public const string MergeRecordSequenceNumber = "sdl:merge-record-sequence-number";

		public const string Macro = "sdl:macro";

		public const string Private = "sdl:private";

		public const string InsertDatabase = "sdl:insert-database";

		public const string AutoText = "sdl:autotext";

		public const string AutoShape = "sdl:autoshape";

		public const string ChartShape = "sdl:chartshape";

		public const string CompareTwoValues = "sdl:compare-two-values";

		public const string PlugInModulePrivate = "sdl:plug-in-module-private";

		public const string Subscriber = "sdl:subscriber";

		public const string FormListBox = "sdl:form-list-box";

		public const string Advance = "sdl:advance";

		public const string DocumentProperty = "sdl:document-property";

		public const string InvertedExclamationMarks = "sdl:inverted-exclamation-marks";

		public const string OCX = "sdl:ocx";

		public const string AutoTextList = "sdl:autotextlist";

		public const string ListElement = "sdl:list-element";

		public const string HTMLControl = "sdl:html-control";

		public const string BIDIOutline = "sdl:bidi-outline";

		public const string AddressBlock = "sdl:address-block";

		public const string GreetingsLine = "sdl:greetings-line";

		public const string PseudoInlineShape = "sdl:pseudo-inline-shape";

		public const string BlockQuote = "sdl:block-quote";

		public const string Body = "sdl:body";

		public const string Cite = "sdl:cite";

		public const string Code = "sdl:code";

		public const string Form = "sdl:form";

		public const string Frame = "sdl:frame";

		public const string Input = "sdl:input";

		public const string Meta = "sdl:meta";

		public const string NoFrames = "sdl:no-frames";

		public const string NoScript = "sdl:no-script";

		public const string OrderedList = "sdl:ordered-list";

		public const string Samp = "sdl:samp";

		public const string Select = "sdl:select";

		public const string TextArea = "sdl:text-area";

		public const string UnorderedList = "sdl:unordered-list";

		public const string META_DATA_LEVEL = "level";

		public const string META_DATA_GENERIC = "generic";

		public const string SID = "sdl:sid";

		private static IDictionary<string, ContextData> _StandardContextData;

		public static IDictionary<string, ContextData> StandardContextData => _StandardContextData;

		static StandardContextTypes()
		{
			CreateStandardContextData();
		}

		private static void CreateStandardContextData()
		{
			_StandardContextData = new Dictionary<string, ContextData>();
			Color empty = Color.Empty;
			_StandardContextData.Add("sdl:address", new ContextData(Resources.StandardContextTypes_Address_Name, Resources.StandardContextTypes_Address_Code, Resources.StandardContextTypes_Address_Description, empty));
			_StandardContextData.Add("sdl:block-quote", new ContextData(Resources.StandardContextTypes_BlockQuote_Name, Resources.StandardContextTypes_BlockQuote_Code, Resources.StandardContextTypes_BlockQuote_Description, empty));
			_StandardContextData.Add("sdl:body", new ContextData(Resources.StandardContextTypes_Body_Name, Resources.StandardContextTypes_Body_Code, Resources.StandardContextTypes_Body_Description, empty));
			_StandardContextData.Add("sdl:bookmark-name", new ContextData(Resources.StandardContextTypes_BookmarkName_Name, Resources.StandardContextTypes_BookmarkName_Code, Resources.StandardContextTypes_BookmarkName_Description, empty));
			_StandardContextData.Add("sdl:bookmark-reference", new ContextData(Resources.StandardContextTypes_BookmarkReference_Name, Resources.StandardContextTypes_BookmarkReference_Code, Resources.StandardContextTypes_BookmarkReference_Description, empty));
			_StandardContextData.Add("sdl:button", new ContextData(Resources.StandardContextTypes_Button_Name, Resources.StandardContextTypes_Button_Code, Resources.StandardContextTypes_Button_Description, empty));
			_StandardContextData.Add("sdl:callout", new ContextData(Resources.StandardContextTypes_Callout_Name, Resources.StandardContextTypes_Callout_Code, Resources.StandardContextTypes_Callout_Description, empty));
			_StandardContextData.Add("sdl:caption", new ContextData(Resources.StandardContextTypes_Caption_Name, Resources.StandardContextTypes_Caption_Code, Resources.StandardContextTypes_Caption_Description, empty));
			_StandardContextData.Add("sdl:cdata", new ContextData(Resources.StandardContextTypes_CData_Name, Resources.StandardContextTypes_CData_Code, Resources.StandardContextTypes_CData_Description, empty));
			_StandardContextData.Add("sdl:cell", new ContextData(Resources.StandardContextTypes_Cell_Name, Resources.StandardContextTypes_Cell_Code, Resources.StandardContextTypes_Cell_Description, empty));
			_StandardContextData.Add("sdl:chartsheet", new ContextData(Resources.StandardContextTypes_ChartSheet_Name, Resources.StandardContextTypes_ChartSheet_Code, Resources.StandardContextTypes_ChartSheet_Description, empty));
			_StandardContextData.Add("sdl:checkbox", new ContextData(Resources.StandardContextTypes_CheckBox_Name, Resources.StandardContextTypes_CheckBox_Code, Resources.StandardContextTypes_CheckBox_Description, empty));
			_StandardContextData.Add("sdl:cite", new ContextData(Resources.StandardContextTypes_Cite_Name, Resources.StandardContextTypes_Cite_Code, Resources.StandardContextTypes_Cite_Description, empty));
			_StandardContextData.Add("sdl:code", new ContextData(Resources.StandardContextTypes_Code_Name, Resources.StandardContextTypes_Code_Code, Resources.StandardContextTypes_Code_Description, empty));
			_StandardContextData.Add("sdl:combobox", new ContextData(Resources.StandardContextTypes_ComboBox_Name, Resources.StandardContextTypes_ComboBox_Code, Resources.StandardContextTypes_ComboBox_Description, empty));
			_StandardContextData.Add("sdl:comment", new ContextData(Resources.StandardContextTypes_Comment_Name, Resources.StandardContextTypes_Comment_Code, Resources.StandardContextTypes_Comment_Description, empty));
			_StandardContextData.Add("sdl:content-flow", new ContextData(Resources.StandardContextTypes_ContentFlow_Name, Resources.StandardContextTypes_ContentFlow_Code, Resources.StandardContextTypes_ContentFlow_Description, empty));
			_StandardContextData.Add("sdl:copyright", new ContextData(Resources.StandardContextTypes_Copyright_Name, Resources.StandardContextTypes_Copyright_Code, Resources.StandardContextTypes_Copyright_Description, empty));
			_StandardContextData.Add("sdl:create-index", new ContextData(Resources.StandardContextTypes_CreateIndex_Name, Resources.StandardContextTypes_CreateIndex_Code, Resources.StandardContextTypes_CreateIndex_Description, empty));
			_StandardContextData.Add("sdl:create-table-of-authorities", new ContextData(Resources.StandardContextTypes_CreateTableOfAuthorities_Name, Resources.StandardContextTypes_CreateTableOfAuthorities_Code, Resources.StandardContextTypes_CreateTableOfAuthroities_Description, empty));
			_StandardContextData.Add("sdl:create-table-of-contents", new ContextData(Resources.StandardContextTypes_CreateTableOfContents_Name, Resources.StandardContextTypes_CreateTableOfContents_Code, Resources.StandardContextTypes_CreateTableOfContents_Description, empty));
			_StandardContextData.Add("sdl:dialog", new ContextData(Resources.StandardContextTypes_Dialog_Name, Resources.StandardContextTypes_Dialog_Code, Resources.StandardContextTypes_Dialog_Description, empty));
			_StandardContextData.Add("sdl:dialogs", new ContextData(Resources.StandardContextTypes_Dialogs_Name, Resources.StandardContextTypes_Dialogs_Code, Resources.StandardContextTypes_Dialogs_Description, empty));
			_StandardContextData.Add("sdl:display-content", new ContextData(Resources.StandardContextTypes_DisplayContent_Name, Resources.StandardContextTypes_DisplayContent_Code, Resources.StandardContextTypes_DisplayContent_Description, empty));
			_StandardContextData.Add("sdl:documentLink", new ContextData(Resources.StandardContextTypes_DocumentLink_Name, Resources.StandardContextTypes_DocumentLink_Code, Resources.StandardContextTypes_DocumentLink_Description, empty));
			_StandardContextData.Add("sdl:element", new ContextData(Resources.StandardContextTypes_Element_Name, Resources.StandardContextTypes_Element_Code, Resources.StandardContextTypes_Element_Description, empty));
			_StandardContextData.Add("sdl:endnote", new ContextData(Resources.StandardContextTypes_EndNote_Name, Resources.StandardContextTypes_EndNote_Code, Resources.StandardContextTypes_EndNote_Description, empty));
			_StandardContextData.Add("sdl:external-link", new ContextData(Resources.StandardContextTypes_ExternalLink_Name, Resources.StandardContextTypes_ExternalLink_Code, Resources.StandardContextTypes_ExternalLink_Description, empty));
			_StandardContextData.Add("sdl:field", new ContextData(Resources.StandardContextTypes_Field_Name, Resources.StandardContextTypes_Field_Code, Resources.StandardContextTypes_Field_Description, empty));
			_StandardContextData.Add("sdl:footnote", new ContextData(Resources.StandardContextTypes_Footnote_Name, Resources.StandardContextTypes_Footnote_Code, Resources.StandardContextTypes_Footnote_Description, empty));
			_StandardContextData.Add("sdl:form", new ContextData(Resources.StandardContextTypes_Form_Name, Resources.StandardContextTypes_Form_Code, "", empty));
			_StandardContextData.Add("sdl:frame", new ContextData(Resources.StandardContextTypes_Frame_Name, Resources.StandardContextTypes_Frame_Code, "", empty));
			_StandardContextData.Add("sdl:group", new ContextData(Resources.StandardContextTypes_Group_Name, Resources.StandardContextTypes_Group_Code, Resources.StandardContextTypes_Group_Description, empty));
			_StandardContextData.Add("sdl:groupbox", new ContextData(Resources.StandardContextTypes_GroupBox_Name, Resources.StandardContextTypes_GroupBox_Code, Resources.StandardContextTypes_GroupBox_Description, empty));
			_StandardContextData.Add("x-tm-heading", new ContextData(Resources.StandardContextTypes_Heading_Name, Resources.StandardContextTypes_Heading_Code, Resources.StandardContextTypes_Heading_Description, Color.CadetBlue));
			_StandardContextData.Add("sdl:hyperlink", new ContextData(Resources.StandardContextTypes_Hyperlink_Name, Resources.StandardContextTypes_Hyperlink_Code, Resources.StandardContextTypes_Hyperlink_Description, empty));
			_StandardContextData.Add("sdl:hyperlink-destination", new ContextData(Resources.StandardContextTypes_HyperlinkDestination_Name, Resources.StandardContextTypes_HyperlinkDestination_Code, Resources.StandardContextTypes_HyperlinkDestination_Description, empty));
			_StandardContextData.Add("sdl:id", new ContextData(Resources.StandardContextTypes_ID_Name, Resources.StandardContextTypes_ID_Code, Resources.StandardContextTypes_ID_Description, empty));
			_StandardContextData.Add("x-tm-index-entry", new ContextData(Resources.StandardContextTypes_IndexEntry_Name, Resources.StandardContextTypes_IndexEntry_Code, Resources.StandardContextTypes_IndexEntry_Description, Color.PeachPuff));
			_StandardContextData.Add("sdl:index-topics", new ContextData(Resources.StandardContextTypes_IndexTopics_Name, Resources.StandardContextTypes_IndexTopics_Code, Resources.StandardContextTypes_IndexTopics_Description, empty));
			_StandardContextData.Add("sdl:input", new ContextData(Resources.StandardContextTypes_Input_Name, Resources.StandardContextTypes_Input_Code, Resources.StandardContextTypes_Input_Description, empty));
			_StandardContextData.Add("sdl:item", new ContextData(Resources.StandardContextTypes_Item_Name, Resources.StandardContextTypes_Item_Code, Resources.StandardContextTypes_Item_Description, empty));
			_StandardContextData.Add("sdl:items", new ContextData(Resources.StandardContextTypes_Items_Name, Resources.StandardContextTypes_Items_Code, Resources.StandardContextTypes_Items_Description, empty));
			_StandardContextData.Add("sdl:keywords", new ContextData(Resources.StandardContextTypes_KeywordList_Name, Resources.StandardContextTypes_KeywordList_Code, Resources.StandardContextTypes_KeywordList_Description, empty));
			_StandardContextData.Add("x-tm-label", new ContextData(Resources.StandardContextTypes_Label_Name, Resources.StandardContextTypes_Label_Code, Resources.StandardContextTypes_Label_Description, Color.Khaki));
			_StandardContextData.Add("sdl:listbox", new ContextData(Resources.StandardContextTypes_ListBox_Name, Resources.StandardContextTypes_ListBox_Code, Resources.StandardContextTypes_ListBox_Description, empty));
			_StandardContextData.Add("x-tm-listitem", new ContextData(Resources.StandardContextTypes_ListItem_Name, Resources.StandardContextTypes_ListItem_Code, Resources.StandardContextTypes_ListItem_Description, Color.Turquoise));
			_StandardContextData.Add("sdl:mark-table-of-authorities-entry", new ContextData(Resources.StandardContextTypes_MarkTableOfAuthorities_Name, Resources.StandardContextTypes_MarkTableOfAuthorities_Code, Resources.StandardContextTypes_MarkTableOfAuthorities_Description, empty));
			_StandardContextData.Add("sdl:masterpage", new ContextData(Resources.StandardContextTypes_MasterPage_Name, Resources.StandardContextTypes_MasterPage_Code, Resources.StandardContextTypes_MasterPage_Description, empty));
			_StandardContextData.Add("sdl:master-spread", new ContextData(Resources.StandardContextTypes_MasterSpread_Name, Resources.StandardContextTypes_MasterSpread_Code, Resources.StandardContextTypes_MasterSpread_Description, empty));
			_StandardContextData.Add("sdl:x-ref-content", new ContextData(Resources.StandardContextTypes_CrossReferenceContent_Name, Resources.StandardContextTypes_CrossReferenceContent_Code, Resources.StandardContextTypes_CrossReferenceContent_Description, empty));
			_StandardContextData.Add("sdl:menu", new ContextData(Resources.StandardContextTypes_Menu_Name, Resources.StandardContextTypes_Menu_Code, Resources.StandardContextTypes_Menu_Description, empty));
			_StandardContextData.Add("sdl:menus", new ContextData(Resources.StandardContextTypes_Menus_Name, Resources.StandardContextTypes_Menus_Code, Resources.StandardContextTypes_Menus_Description, empty));
			_StandardContextData.Add("sdl:meta", new ContextData(Resources.StandardContextTypes_Meta_Name, Resources.StandardContextTypes_Meta_Code, Resources.StandardContextTypes_Meta_Description, empty));
			_StandardContextData.Add("sdl:no-frames", new ContextData(Resources.StandardContextTypes_NoFrames_Name, Resources.StandardContextTypes_NoFrames_Code, Resources.StandardContextTypes_NoFrames_Description, empty));
			_StandardContextData.Add("sdl:normal-spread", new ContextData(Resources.StandardContextTypes_NormalSpread_Name, Resources.StandardContextTypes_NormalSpread_Code, Resources.StandardContextTypes_NormalSpread_Description, empty));
			_StandardContextData.Add("sdl:no-script", new ContextData(Resources.StandardContextTypes_NoScript_Name, Resources.StandardContextTypes_NoScript_Code, Resources.StandardContextTypes_NoScript_Description, empty));
			_StandardContextData.Add("sdl:note", new ContextData(Resources.StandardContextTypes_Note_Name, Resources.StandardContextTypes_Note_Code, Resources.StandardContextTypes_Note_Description, empty));
			_StandardContextData.Add("sdl:ordered-list", new ContextData(Resources.StandardContextTypes_OrderedList_Name, Resources.StandardContextTypes_OrderedList_Code, "", empty));
			_StandardContextData.Add("x-tm-header-footer", new ContextData(Resources.StandardContextTypes_PageHeader_Name, Resources.StandardContextTypes_PageHeaderFooter_Code, Resources.StandardContextTypes_PageHeader_Description, Color.LightSteelBlue));
			_StandardContextData.Add("sdl:paragraph", new ContextData(Resources.StandardContextTypes_Paragraph_Name, Resources.StandardContextTypes_Paragraph_Code, Resources.StandardContextTypes_Paragraph_Description, empty));
			_StandardContextData.Add("sdl:transunit-ref", new ContextData(Resources.StandardContextTypes_ParagraphUnitReference_Name, Resources.StandardContextTypes_ParagraphUnitReference_Code, Resources.StandardContextTypes_ParagraphUnitReference_Description, empty));
			_StandardContextData.Add("sdl:product-name", new ContextData(Resources.StandardContextTypes_ProductName_Name, Resources.StandardContextTypes_ProductName_Code, Resources.StandardContextTypes_ProductName_Description, empty));
			_StandardContextData.Add("sdl:product-version", new ContextData(Resources.StandardContextTypes_ProductVersion_Name, Resources.StandardContextTypes_ProductVersion_Code, Resources.StandardContextTypes_ProductVersion_Description, empty));
			_StandardContextData.Add("sdl:radio-button", new ContextData(Resources.StandardContextTypes_RadioButton_Name, Resources.StandardContextTypes_RadioButton_Code, Resources.StandardContextTypes_RadioButton_Description, empty));
			_StandardContextData.Add("sdl:samp", new ContextData(Resources.StandardContextTypes_Samp_Name, Resources.StandardContextTypes_Samp_Code, Resources.StandardContextTypes_Samp_Description, empty));
			_StandardContextData.Add("sdl:script", new ContextData(Resources.StandardContextTypes_Script_Name, Resources.StandardContextTypes_Script_Code, Resources.StandardContextTypes_Script_Description, empty));
			_StandardContextData.Add("sdl:select", new ContextData(Resources.StandardContextTypes_Select_Name, Resources.StandardContextTypes_Select_Code, "", empty));
			_StandardContextData.Add("sdl:shortcut", new ContextData(Resources.StandardContextTypes_Shortcut_Name, Resources.StandardContextTypes_Shortcut_Code, Resources.StandardContextTypes_Shortcut_Description, empty));
			_StandardContextData.Add("sdl:sidebar", new ContextData(Resources.StandardContextTypes_Sidebar_Name, Resources.StandardContextTypes_Sidebar_Code, Resources.StandardContextTypes_Sidebar_Description, empty));
			_StandardContextData.Add("sdl:slide", new ContextData(Resources.StandardContextTypes_Slide_Name, Resources.StandardContextTypes_Slide_Code, Resources.StandardContextTypes_Slide_Description, empty));
			_StandardContextData.Add("sdl:spread", new ContextData(Resources.StandardContextTypes_Spread_Name, Resources.StandardContextTypes_Spread_Code, Resources.StandardContextTypes_Spread_Description, empty));
			_StandardContextData.Add("sdl:string-table", new ContextData(Resources.StandardContextTypes_StringTable_Name, Resources.StandardContextTypes_StringTable_Code, Resources.StandardContextTypes_StringTable_Description, empty));
			_StandardContextData.Add("sdl:table", new ContextData(Resources.StandardContextTypes_Table_Name, Resources.StandardContextTypes_Table_Code, Resources.StandardContextTypes_Table_Description, empty));
			_StandardContextData.Add("x-tm-table-cell", new ContextData(Resources.StandardContextTypes_TableCell_Name, Resources.StandardContextTypes_TableCell_Code, Resources.StandardContextTypes_TableCell_Description, Color.Thistle));
			_StandardContextData.Add("x-tm-table-heading", new ContextData(Resources.StandardContextTypes_TableHeading_Name, Resources.StandardContextTypes_TableHeading_Code, Resources.StandardContextTypes_TableHeading_Description, Color.PowderBlue));
			_StandardContextData.Add("sdl:table-of-contents-entry", new ContextData(Resources.StandardContextTypes_TableOfContentsEntry_Name, Resources.StandardContextTypes_TableOfContentsEntry_Code, Resources.StandardContextTypes_TableOfContentsEntry_Description, empty));
			_StandardContextData.Add("sdl:table-row", new ContextData(Resources.StandardContextTypes_TableRow_Name, Resources.StandardContextTypes_TableRow_Code, Resources.StandardContextTypes_TableRow_Description, empty));
			_StandardContextData.Add("x-tm-tag", new ContextData(Resources.StandardContextTypes_TagContent_Name, Resources.StandardContextTypes_TagContent_Code, Resources.StandardContextTypes_TagContent_Description, Color.Tan));
			_StandardContextData.Add("sdl:text-area", new ContextData(Resources.StandardContextTypes_TextArea_Name, Resources.StandardContextTypes_TextArea_Code, "", empty));
			_StandardContextData.Add("sdl:textbox", new ContextData(Resources.StandardContextTypes_TextBox_Name, Resources.StandardContextTypes_TextBox_Code, Resources.StandardContextTypes_TextBox_Description, empty));
			_StandardContextData.Add("sdl:title", new ContextData(Resources.StandardContextTypes_Title_Name, Resources.StandardContextTypes_Title_Code, Resources.StandardContextTypes_Title_Description, empty));
			_StandardContextData.Add("sdl:topic", new ContextData(Resources.StandardContextTypes_Topic_Name, Resources.StandardContextTypes_Topic_Code, Resources.StandardContextTypes_Topic_Description, empty));
			_StandardContextData.Add("sdl:trademark", new ContextData(Resources.StandardContextTypes_Trademark_Name, Resources.StandardContextTypes_Trademark_Code, Resources.StandardContextTypes_Trademark_Description, empty));
			_StandardContextData.Add("sdl:translatable-content", new ContextData(Resources.StandardContextTypes_TranslatableContent_Name, Resources.StandardContextTypes_TranslatableContent_Code, Resources.StandardContextTypes_TranslatableContent_Description, empty));
			_StandardContextData.Add("sdl:translatable-fieldoption", new ContextData(Resources.StandardContextTypes_FillInCommandToDisplayOption_Name, Resources.StandardContextTypes_FillInCommandToDisplayOption_Code, Resources.StandardContextTypes_FillInCommandToDisplayOption_Description, empty));
			_StandardContextData.Add("sdl:version-info", new ContextData(Resources.StandardContextTypes_VersionInfo_Name, Resources.StandardContextTypes_VersionInfo_Code, Resources.StandardContextTypes_VersionInfo_Description, empty));
			_StandardContextData.Add("sdl:wordart", new ContextData(Resources.StandardContextTypes_WordArt_Name, Resources.StandardContextTypes_WordArt_Code, Resources.StandardContextTypes_WordArt_Description, empty));
			_StandardContextData.Add("sdl:worksheet", new ContextData(Resources.StandardContextTypes_WorkSheet_Name, Resources.StandardContextTypes_WorkSheet_Code, Resources.StandardContextTypes_WorkSheet_Description, empty));
			_StandardContextData.Add("sdl:worksheet-name", new ContextData(Resources.StandardContextTypes_WorksheetName_Name, Resources.StandardContextTypes_WorksheetName_Code, Resources.StandardContextTypes_WorkSheetName_Description, empty));
			_StandardContextData.Add("sdl:unknown-keyword", new ContextData(Resources.StandardContextTypes_UnknownKeyword_Name, Resources.StandardContextTypes_UnknownKeyword_Code, Resources.StandardContextTypes_UnknownKeyword_Description, empty));
			_StandardContextData.Add("sdl:unordered-list", new ContextData(Resources.StandardContextTypes_UnorderedList_Name, Resources.StandardContextTypes_UnorderedList_Code, "", empty));
			_StandardContextData.Add("sdl:possible-bookmark", new ContextData(Resources.StandardContextTypes_PossibleBookmark_Name, Resources.StandardContextTypes_PossibleBookmark_Code, Resources.StandardContextTypes_PossibleBookmark_Description, empty));
			_StandardContextData.Add("sdl:footnote-reference", new ContextData(Resources.StandardContextTypes_FootnoteReference_Name, Resources.StandardContextTypes_FootnoteReference_Code, "", empty));
			_StandardContextData.Add("sdl:set-command", new ContextData(Resources.StandardContextTypes_SetCommand_Name, Resources.StandardContextTypes_SetCommand_Code, Resources.StandardContextTypes_SetCommand_Description, empty));
			_StandardContextData.Add("sdl:if-command", new ContextData(Resources.StandardContextTypes_IfCommand_Name, Resources.StandardContextTypes_IfCommand_Code, Resources.StandardContextTypes_IfCommand_Description, empty));
			_StandardContextData.Add("sdl:style-reference", new ContextData(Resources.StandardContextTypes_StyleReference_Name, Resources.StandardContextTypes_StyleReference_Code, Resources.StandardContextTypes_StyleReference_Description, empty));
			_StandardContextData.Add("sdl:document-reference", new ContextData(Resources.StandardContextTypes_DocumentReference_Name, Resources.StandardContextTypes_DocumentReference_Code, Resources.StandardContextTypes_DocumentReference_Description, empty));
			_StandardContextData.Add("sdl:sequence-mark", new ContextData(Resources.StandardContextTypes_SequenceMark_Name, Resources.StandardContextTypes_SequenceMark_Code, Resources.StandardContextTypes_SequenceMark_Description, empty));
			_StandardContextData.Add("sdl:quote-info-variable", new ContextData(Resources.StandardContextTypes_QuoteInfoVariable_Name, Resources.StandardContextTypes_QuoteInfoVariable_Code, Resources.StandardContextTypes_QuoteInfoVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-title-variable", new ContextData(Resources.StandardContextTypes_QuoteTitleVariable_Name, Resources.StandardContextTypes_QuoteTitleVariable_Code, Resources.StandardContextTypes_QuoteTitleVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-subject-variable", new ContextData(Resources.StandardContextTypes_QuoteSubjectVariable_Name, Resources.StandardContextTypes_QuoteSubjectVariable_Code, Resources.StandardContextTypes_QuoteSubjectVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-author-variable", new ContextData(Resources.StandardContextTypes_QuoteAuthorVariable_Name, Resources.StandardContextTypes_QuoteAuthorVariable_Code, Resources.StandardContextTypes_QuoteAuthorVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-keywords-variable", new ContextData(Resources.StandardContextTypes_QuoteKeywordsVariable_Name, Resources.StandardContextTypes_QuoteKeywordsVariable_Code, Resources.StandardContextTypes_QuoteKeywordsVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-comments-variable", new ContextData(Resources.StandardContextTypes_QuoteCommentsVariable_Name, Resources.StandardContextTypes_QuoteCommentsVariable_Code, Resources.StandardContextTypes_QuoteCommentsVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-last-revised-by-variable", new ContextData(Resources.StandardContextTypes_QuoteLastRevisedByVariable_Name, Resources.StandardContextTypes_QuoteLastRevisedByVariable_Code, Resources.StandardContextTypes_QuoteLastRevisedByVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-creation-date-variable", new ContextData(Resources.StandardContextTypes_QuoteCreationDateVariable_Name, Resources.StandardContextTypes_QuoteCreationDateVariable_Code, Resources.StandardContextTypes_QuoteCreationDateVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-revision-date-variable", new ContextData(Resources.StandardContextTypes_QuoteRevisionDateVariable_Name, Resources.StandardContextTypes_QuoteRevisionDateVariable_Code, Resources.StandardContextTypes_QuoteRevisionDateVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-print-date-variable", new ContextData(Resources.StandardContextTypes_QuotePrintDateVariable_Name, Resources.StandardContextTypes_QuotePrintDateVariable_Code, "", empty));
			_StandardContextData.Add("sdl:quote-revision-number-variable", new ContextData(Resources.StandardContextTypes_QuoteRevisionNumberVariable_Name, Resources.StandardContextTypes_QuoteRevisionNumberVariable_Code, Resources.StandardContextTypes_QuoteRevisionNumberVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-edit-time-variable", new ContextData(Resources.StandardContextTypes_QuoteEditTimeVariable_Name, Resources.StandardContextTypes_QuoteEditTimeVariable_Code, Resources.StandardContextTypes_QuoteEditTimeVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-number-of-pages-variable", new ContextData(Resources.StandardContextTypes_QuoteNumberOfPagesVariable_Name, Resources.StandardContextTypes_QuoteNumberOfPagesVariable_Code, "", empty));
			_StandardContextData.Add("sdl:quote-number-of-words-variable", new ContextData(Resources.StandardContextTypes_QuoteNumberOfWordsVariable_Name, Resources.StandardContextTypes_QuoteNumberOfWordsVariable_Code, Resources.StandardContextTypes_QuoteNumberOfWordsVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-number-of-characters-variable", new ContextData(Resources.StandardContextTypes_QuoteNumberOfCharactersVariable_Name, Resources.StandardContextTypes_QuoteNumberOfCharactersVariable_Code, "", empty));
			_StandardContextData.Add("sdl:quote-file-name-variable", new ContextData(Resources.StandardContextTypes_QuoteFileNameVariable_Name, Resources.StandardContextTypes_QuoteFileNameVariable_Code, Resources.StandardContextTypes_QuoteFileNameVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-document-template-name-variable", new ContextData(Resources.StandardContextTypes_QuoteDocumentTemplateNameVariable_Name, Resources.StandardContextTypes_QuoteDocumentTemplateNameVariable_Code, Resources.StandardContextTypes_QuoteDocumentTemplateNameVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-current-date-variable", new ContextData(Resources.StandardContextTypes_QuoteCurrentDateVariable_Name, Resources.StandardContextTypes_QuoteCurrentDateVariable_Code, Resources.StandardContextTypes_QuoteCurrentDateVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-current-time-variable", new ContextData(Resources.StandardContextTypes_QuoteCurrentTimeVariable_Name, Resources.StandardContextTypes_QuoteCurrentTimeVariable_Code, Resources.StandardContextTypes_QuoteCurrentTimeVariable_Description, empty));
			_StandardContextData.Add("sdl:quote-current-page-variable", new ContextData(Resources.StandardContextTypes_QuoteCurrentPageVariable_Name, Resources.StandardContextTypes_QuoteCurrentPageVariable_Code, Resources.StandardContextTypes_QuoteCurrentPageVariable_Description, empty));
			_StandardContextData.Add("sdl:evaluate-expression", new ContextData(Resources.StandardContextTypes_EvaluateExpression_Name, Resources.StandardContextTypes_EvaluateExpression_Code, Resources.StandardContextTypes_EvaluateExpression_Description, empty));
			_StandardContextData.Add("sdl:insert-literal-text", new ContextData(Resources.StandardContextTypes_InsertLiteralText_Name, Resources.StandardContextTypes_InsertLiteralText_Code, Resources.StandardContextTypes_InsertLiteralText_Description, empty));
			_StandardContextData.Add("sdl:include-command", new ContextData(Resources.StandardContextTypes_IncludeCommand_Name, Resources.StandardContextTypes_IncludeCommand_Code, Resources.StandardContextTypes_IncludeCommand_Description, empty));
			_StandardContextData.Add("sdl:page-reference", new ContextData(Resources.StandardContextTypes_PageReference_Name, Resources.StandardContextTypes_PageReference_Code, Resources.StandardContextTypes_PageReference_Description, empty));
			_StandardContextData.Add("sdl:ask-command", new ContextData(Resources.StandardContextTypes_AskCommand_Name, Resources.StandardContextTypes_AskCommand_Code, Resources.StandardContextTypes_AskCommand_Description, empty));
			_StandardContextData.Add("sdl:fill-in-command-to-display-prompt", new ContextData(Resources.StandardContextTypes_FillInCommandToDisplayPrompt_Name, Resources.StandardContextTypes_FillInCommandToDisplayPrompt_Code, Resources.StandardContextTypes_FillInCommandToDisplayPrompt_Description, empty));
			_StandardContextData.Add("sdl:data-command", new ContextData(Resources.StandardContextTypes_DataCommand_Name, Resources.StandardContextTypes_DataCommand_Code, Resources.StandardContextTypes_DataCommand_Description, empty));
			_StandardContextData.Add("sdl:next-command", new ContextData(Resources.StandardContextTypes_NextCommand_Name, Resources.StandardContextTypes_NextCommand_Code, Resources.StandardContextTypes_NextCommand_Description, empty));
			_StandardContextData.Add("sdl:nextif-command", new ContextData(Resources.StandardContextTypes_NextIfCommand_Name, Resources.StandardContextTypes_NextIfCommand_Code, Resources.StandardContextTypes_NextIfCommand_Description, empty));
			_StandardContextData.Add("sdl:skipif", new ContextData(Resources.StandardContextTypes_SkipIfCommand_Name, Resources.StandardContextTypes_SkipIfCommand_Code, Resources.StandardContextTypes_SkipIfCommand_Description, empty));
			_StandardContextData.Add("sdl:inserts-number-of-current-print-merge-record", new ContextData(Resources.StandardContextTypes_InsertsNumberOfCurrentPrintMergeRecord_Name, Resources.StandardContextTypes_InsertsNumberOfCurrentPrintMergeRecord_Code, "", empty));
			_StandardContextData.Add("sdl:dde-reference", new ContextData(Resources.StandardContextTypes_DDEReference_Name, Resources.StandardContextTypes_DDEReference_Code, Resources.StandardContextTypes_DDEReference_Description, empty));
			_StandardContextData.Add("sdl:dde-automatic-reference", new ContextData(Resources.StandardContextTypes_DDEAutomaticReference_Name, Resources.StandardContextTypes_DDEAutomaticReference_Code, Resources.StandardContextTypes_DDEAutomaticReference_Description, empty));
			_StandardContextData.Add("sdl:inserts-glossary-entry", new ContextData(Resources.StandardContextTypes_InsertsGlossaryEntry_Name, Resources.StandardContextTypes_InsertsGlossaryEntry_Code, Resources.StandardContextTypes_InsertsGlossaryEntry_Description, empty));
			_StandardContextData.Add("sdl:sends-characters-to-printer-without-translation", new ContextData(Resources.StandardContextTypes_SendsCharactersToPrinterWithoutTranslation_Name, Resources.StandardContextTypes_SendsCharactersToPrinterWithoutTranslation_Code, "", empty));
			_StandardContextData.Add("sdl:formula-definition", new ContextData(Resources.StandardContextTypes_FormulaDefinition_Name, Resources.StandardContextTypes_FormulaDefinition_Code, Resources.StandardContextTypes_FormulaDefinition_Description, empty));
			_StandardContextData.Add("sdl:goto-button", new ContextData(Resources.StandardContextTypes_GoToButton_Name, Resources.StandardContextTypes_GoToButton_Code, Resources.StandardContextTypes_GoToButton_Description, empty));
			_StandardContextData.Add("sdl:macro-button", new ContextData(Resources.StandardContextTypes_MacroButton_Name, Resources.StandardContextTypes_MacroButton_Code, Resources.StandardContextTypes_MacroButton_Description, empty));
			_StandardContextData.Add("sdl:insert-auto-numbering-field-in-outline-format", new ContextData(Resources.StandardContextTypes_InsertAutoNumberingFieldInOutlineFormat_Name, Resources.StandardContextTypes_InsertAutoNumberingFieldInOutlineFormat_Code, "", empty));
			_StandardContextData.Add("sdl:insert-auto-numbering-field-in-legal-format", new ContextData(Resources.StandardContextTypes_InsertAutoNumberingFieldInLegalFormat_Name, Resources.StandardContextTypes_InsertAutoNumberingFieldInLegalFormat_Code, "", empty));
			_StandardContextData.Add("sdl:insert-auto-numbering-field-in-arabic-number-format", new ContextData(Resources.StandardContextTypes_InsertAutoNumberingFieldInArabicNumberFormat_Name, Resources.StandardContextTypes_InsertAutoNumberingFieldInArabicNumberFormat_Code, "", empty));
			_StandardContextData.Add("sdl:reads-a-tiff-file", new ContextData(Resources.StandardContextTypes_ReadsATIFFFile_Name, Resources.StandardContextTypes_ReadsATIFFFile_Code, "", empty));
			_StandardContextData.Add("sdl:link", new ContextData(Resources.StandardContextTypes_Link_Name, Resources.StandardContextTypes_Link_Code, Resources.StandardContextTypes_Link_Description, empty));
			_StandardContextData.Add("sdl:symbol", new ContextData(Resources.StandardContextTypes_Symbol_Name, Resources.StandardContextTypes_Symbol_Code, Resources.StandardContextTypes_Symbol_Description, empty));
			_StandardContextData.Add("sdl:embedded-object", new ContextData(Resources.StandardContextTypes_EmbeddedObject_Name, Resources.StandardContextTypes_EmbeddedObject_Code, Resources.StandardContextTypes_EmbeddedObject_Description, empty));
			_StandardContextData.Add("sdl:merge-fields", new ContextData(Resources.StandardContextTypes_MergeFields_Name, Resources.StandardContextTypes_MergeFields_Code, "", empty));
			_StandardContextData.Add("sdl:user-name", new ContextData(Resources.StandardContextTypes_UserName_Name, Resources.StandardContextTypes_UserName_Code, "", empty));
			_StandardContextData.Add("sdl:user-initial", new ContextData(Resources.StandardContextTypes_UserInitial_Name, Resources.StandardContextTypes_UserInitial_Code, "", empty));
			_StandardContextData.Add("sdl:user-address", new ContextData(Resources.StandardContextTypes_UserAddress_Name, Resources.StandardContextTypes_UserAddress_Code, "", empty));
			_StandardContextData.Add("sdl:bar-code", new ContextData(Resources.StandardContextTypes_BarCode_Name, Resources.StandardContextTypes_BarCode_Code, "", empty));
			_StandardContextData.Add("sdl:document-variable", new ContextData(Resources.StandardContextTypes_DocumentVariable_Name, Resources.StandardContextTypes_DocumentVariable_Code, "", empty));
			_StandardContextData.Add("sdl:section", new ContextData(Resources.StandardContextTypes_Section_Name, Resources.StandardContextTypes_Section_Code, "", empty));
			_StandardContextData.Add("sdl:section-pages", new ContextData(Resources.StandardContextTypes_SectionPages_Name, Resources.StandardContextTypes_SectionPages_Code, "", empty));
			_StandardContextData.Add("sdl:include-picture", new ContextData(Resources.StandardContextTypes_IncludePicture_Name, Resources.StandardContextTypes_IncludePicture_Code, "", empty));
			_StandardContextData.Add("sdl:include-text", new ContextData(Resources.StandardContextTypes_IncludeText_Name, Resources.StandardContextTypes_IncludeText_Code, "", empty));
			_StandardContextData.Add("sdl:file-size", new ContextData(Resources.StandardContextTypes_FileSize_Name, Resources.StandardContextTypes_FileSize_Code, "", empty));
			_StandardContextData.Add("sdl:form-text-box", new ContextData(Resources.StandardContextTypes_FormTextBox_Name, Resources.StandardContextTypes_FormTextBox_Code, "", empty));
			_StandardContextData.Add("sdl:form-check-box", new ContextData(Resources.StandardContextTypes_FormCheckBox_Name, Resources.StandardContextTypes_FormCheckBox_Code, "", empty));
			_StandardContextData.Add("sdl:note-reference", new ContextData(Resources.StandardContextTypes_NoteReference_Name, Resources.StandardContextTypes_NoteReference_Code, Resources.StandardContextTypes_NoteReference_Description, empty));
			_StandardContextData.Add("sdl:merge-record-sequence-number", new ContextData(Resources.StandardContextTypes_MergeRecordSequenceNumber_Name, Resources.StandardContextTypes_MergeRecordSequenceNumber_Code, "", empty));
			_StandardContextData.Add("sdl:macro", new ContextData(Resources.StandardContextTypes_Macro_Name, Resources.StandardContextTypes_Macro_Code, "", empty));
			_StandardContextData.Add("sdl:private", new ContextData(Resources.StandardContextTypes_Private_Name, Resources.StandardContextTypes_Private_Code, "", empty));
			_StandardContextData.Add("sdl:insert-database", new ContextData(Resources.StandardContextTypes_InsertDatabase_Name, Resources.StandardContextTypes_InsertDatabase_Code, "", empty));
			_StandardContextData.Add("sdl:autotext", new ContextData(Resources.StandardContextTypes_AutoText_Name, Resources.StandardContextTypes_AutoText_Code, "", empty));
			_StandardContextData.Add("sdl:autoshape", new ContextData(Resources.StandardContextTypes_AutoShape_Name, Resources.StandardContextTypes_AutoShape_Code, Resources.StandardContextTypes_AutoShape_Description, empty));
			_StandardContextData.Add("sdl:chartshape", new ContextData(Resources.StandardContextTypes_ChartShape_Name, Resources.StandardContextTypes_ChartShape_Code, Resources.StandardContextTypes_ChartShape_Description, empty));
			_StandardContextData.Add("sdl:compare-two-values", new ContextData(Resources.StandardContextTypes_CompareTwoValues_Name, Resources.StandardContextTypes_CompareTwoValues_Code, "", empty));
			_StandardContextData.Add("sdl:plug-in-module-private", new ContextData(Resources.StandardContextTypes_PlugInModulePrivate_Name, Resources.StandardContextTypes_PlugInModulePrivate_Code, "", empty));
			_StandardContextData.Add("sdl:subscriber", new ContextData(Resources.StandardContextTypes_Subscriber_Name, Resources.StandardContextTypes_Subscriber_Code, "", empty));
			_StandardContextData.Add("sdl:form-list-box", new ContextData(Resources.StandardContextTypes_FormListBox_Name, Resources.StandardContextTypes_FormListBox_Code, "", empty));
			_StandardContextData.Add("sdl:advance", new ContextData(Resources.StandardContextTypes_Advance_Name, Resources.StandardContextTypes_Advance_Code, "", empty));
			_StandardContextData.Add("sdl:document-property", new ContextData(Resources.StandardContextTypes_DocumentProperty_Name, Resources.StandardContextTypes_DocumentProperty_Code, "", empty));
			_StandardContextData.Add("sdl:inverted-exclamation-marks", new ContextData(Resources.StandardContextTypes_InvertedExclamationMarks_Name, Resources.StandardContextTypes_InvertedExclamationMarks_Code, Resources.StandardContextTypes_InvertedExclamationMarks_Description, empty));
			_StandardContextData.Add("sdl:ocx", new ContextData(Resources.StandardContextTypes_OCX_Name, Resources.StandardContextTypes_OCX_Code, "", empty));
			_StandardContextData.Add("sdl:autotextlist", new ContextData(Resources.StandardContextTypes_AutoTextList_Name, Resources.StandardContextTypes_AutoTextList_Code, "", empty));
			_StandardContextData.Add("sdl:list-element", new ContextData(Resources.StandardContextTypes_ListElement_Name, Resources.StandardContextTypes_ListElement_Code, "", empty));
			_StandardContextData.Add("sdl:html-control", new ContextData(Resources.StandardContextTypes_HTMLControl_Name, Resources.StandardContextTypes_HTMLControl_Code, "", empty));
			_StandardContextData.Add("sdl:bidi-outline", new ContextData(Resources.StandardContextTypes_BIDIOutline_Name, Resources.StandardContextTypes_BIDIOutline_Code, "", empty));
			_StandardContextData.Add("sdl:address-block", new ContextData(Resources.StandardContextTypes_AddressBlock_Name, Resources.StandardContextTypes_AddressBlock_Code, "", empty));
			_StandardContextData.Add("sdl:greetings-line", new ContextData(Resources.StandardContextTypes_GreetingsLine_Name, Resources.StandardContextTypes_GreetingsLine_Code, "", empty));
			_StandardContextData.Add("sdl:pseudo-inline-shape", new ContextData(Resources.StandardContextTypes_PseudoInlineShape_Name, Resources.StandardContextTypes_PseudoInlineShape_Code, "", empty));
			_StandardContextData.Add("x-tm-length-info", new ContextData(Resources.StandardContextTypes_Length_Name, Resources.StandardContextTypes_Length_Code, Resources.StandardContextTypes_Length_Description, Color.Tomato));
		}
	}
}
