using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework.Implementation.Administration
{
	public static class AdministrationNames
	{
		public const string SchemaNamespaceUri = "http://www.w3.org/2001/XMLSchema-instance";

		public const string SchemaPrefix = "xsi";

		public const string SpringNamespaceUri = "http://www.springframework.net";

		public const string SpringPrefix = null;

		public const string SpringSchemaUri = "http://www.springframework.net/xsd/spring-objects.xsd";

		public const string SpringXpathPrefix = "spring";

		public const string ParserName = "Parser";

		public const string BilingualParserName = "BilingualParser";

		public const string NativeProcessorsName = "Processors";

		public const string BilingualProcessorsName = "BilingualProcessors";

		public const string FileTypeDefinitionIdName = "FileTypeDefinitionId";

		public const string OBSOLETE_FilterDefinitionIdName = "FilterDefinitionId";

		public const string FileTypeNameName = "FileTypeName";

		public const string FileTypeDocumentNameName = "FileTypeDocumentName";

		public const string FileTypeDocumentsNameName = "FileTypeDocumentsName";

		public const string FileDialogWildcardExpressionName = "FileDialogWildcardExpression";

		public const string ExpressionName = "Expression";

		public const string DefaultFileExtensionName = "DefaultFileExtension";

		public const string EnabledName = "Enabled";

		public const string HiddenName = "Hidden";

		public const string IsBilingualDocumentFileTypeName = "IsBilingualDocumentFileType";

		public const string DescriptionName = "Description";

		public const string FilterDefinitionGraphicAssemblyName = "FilterDefinitionGraphicAssembly";

		public const string FilterDefinitionGraphicResourceIdName = "FilterDefinitionGraphicResourceId";

		public const string NativeGeneratorName = "NativeGenerator";

		public const string WriterName = "Writer";

		public const string FileTypeInformationName = "FileTypeInformation";

		public const string FileSnifferName = "FileSniffer";

		public const string FilterFramework1FilterDefinitionFileName = "FilterFramework1FilterDefinitionFile";

		public const string ExtractorName = "Extractor";

		public const string NativeExtractorName = "NativeExtractor";

		public const string GeneratorName = "Generator";

		public const string AdditionalGeneratorsName = "AdditionalGeneratorsInfo";

		public const string VerifierName = "VerifierCollection";

		public const string BilingualVerifiersName = "BilingualVerifiers";

		public const string BilingualFileVerifiersName = "BilingualFileVerifiers";

		public const string BilingualInteractiveVerifiersName = "BilingualInteractiveVerifiers";

		public const string NativeVerifiersName = "NativeVerifiers";

		public const string NativeFileVerifiersName = "NativeFileVerifiers";

		public const string StandardQuickTagsPropertyName = "StandardQuickTags";

		[Obsolete]
		public const string CustomUIAdminName = "CustomUIAdmin";

		public const string FilterDefinitionAdministratorUiName = "FilterDefinitionAdministratorUI";

		public const string SettingsPageReferencesPropertyName = "SettingsPageReferences";

		public const string SettingsPageCategoryReferencesPropertyName = "SettingsPageCategoryReferences";

		public const string BilingualWriterName = "BilingualWriter";

		public const string DefaultBilingualFileTypePropertyName = "DefaultBilingualFileType";

		public const string CommandIdName = "CommandId";

		public const string CommandNameName = "CommandName";

		public const string ImageName = "Image";

		public const string ImageResourceName = "ImageResource";

		public const string Icon = "Icon";

		public const string CategoryIcon = "CategoryIcon";

		public const string StartTagPropertiesName = "StartTagProperties";

		public const string EndTagPropertiesName = "EndTagProperties";

		public const string PlaceholderTagPropertiesName = "Properties";

		public const string DisplayTextName = "DisplayText";

		public const string TagContentName = "TagContent";

		public const string TextPropertiesName = "Properties";

		public const string TextName = "Text";

		public const string OtherMarkerTypeName = "MarkerType";

		public const string OtherMarkerIdName = "Id";

		public const string LocationMarkerIdName = "MarkerId";

		public const string FormattingName = "Formatting";

		public const string StringValueName = "StringValue";

		public const string CommentsName = "Comments";

		public const string CommentsXmlName = "Xml";

		public const string LockedContentPropertiesName = "Properties";

		public const string DisplayOnToolBarName = "DisplayOnToolBar";

		public const string CanHideName = "CanHide";

		public const string WordStopName = "IsWordStop";

		public const string SoftBreakName = "IsSoftBreak";

		public const string TextEquivalentName = "TextEquivalent";

		public const string FileTweakersName = "FileTweakers";

		public const string DependencyFilenameName = "DependencyFilename";

		public const string DependencyFileTypeName = "DependencyFileType";

		public const string DependencyFileName = "DependencyFile";

		public const string NativeExtractorInitMethodName = "ReconnectComponents";

		public const string NativeGeneratorInitMethodName = "ReconnectComponents";

		public const string DefaultFactoryMethodName = "Create";

		public const string QuickTagFactoryMethodName = "Create";

		public const string FormattingFactoryMethodName = "Create";

		public const string TagPairFactoryMethodName = "Create";

		public static string FileTypeDefinitionIdClass = typeof(FileTypeDefinitionId).FullName;

		public static string FileTypeDefinitionIdAssembly = GetSimpleAssemblyName(typeof(FileTypeDefinitionId));

		public static string FileTypeInformationClass = typeof(FileTypeInformation).FullName;

		public static string FileTypeInformationAssembly = GetSimpleAssemblyName(typeof(FileTypeInformation));

		public static string LocalizableStringClass = typeof(LocalizableString).FullName;

		public static string LocalizableStringAssembly = GetSimpleAssemblyName(typeof(LocalizableString));

		public static string ExtractorClass = typeof(FileExtractor).FullName;

		public static string ExtractorAssembly = GetSimpleAssemblyName(typeof(FileExtractor));

		public static string NativeExtractorClass = typeof(NativeExtractor).FullName;

		public static string NativeExtractorAssembly = GetSimpleAssemblyName(typeof(NativeExtractor));

		public static string GeneratorClass = typeof(FileGenerator).FullName;

		public static string GeneratorAssembly = GetSimpleAssemblyName(typeof(FileGenerator));

		public static string NativeGeneratorClass = typeof(NativeGenerator).FullName;

		public static string NativeGeneratorAssembly = GetSimpleAssemblyName(typeof(NativeGenerator));

		public static string QuickTagsClass = typeof(QuickTags).FullName;

		public static string QuickTagsAssembly = GetSimpleAssemblyName(typeof(QuickTags));

		public static string verifierCollectionClass = typeof(VerifierCollection).FullName;

		public static string VerifierAssembly = GetSimpleAssemblyName(typeof(VerifierCollection));

		public static string QuickTagAssembly = GetSimpleAssemblyName(typeof(QuickTag));

		public static string CommentMarkerAssembly = GetSimpleAssemblyName(typeof(CommentMarker));

		public static string OtherMarkerAssembly = GetSimpleAssemblyName(typeof(OtherMarker));

		public static string FormattingAssembly = GetSimpleAssemblyName(typeof(FormattingGroup));

		public static string CommentsAssembly = GetSimpleAssemblyName(typeof(CommentProperties));

		public static string LockedContentPropertiesAssembly = GetSimpleAssemblyName(typeof(LockedContentProperties));

		public static string ConfigurableFilterDefinitionSettingsPageReferenceClass = "Sdl.FileTypeSupport.Framework.AdministrationUI.ConfigurableFilterDefinitionSettingsPageReference";

		public static string ConfigurableFilterDefinitionSettingsPageReferenceAssembly = "Sdl.FileTypeSupport.Framework.AdministrationUI";

		public static string ConfigurableSettingsPageCategoryReferenceClass = "Sdl.FileTypeSupport.Framework.AdministrationUI.ConfigurableSettingsPageCategoryReference";

		public static string ConfigurableSettingsPageCategoryReferenceAssembly = "Sdl.FileTypeSupport.Framework.AdministrationUI";

		public static string LegacyFilterDefinitionSettingsPageClass = "Sdl.FileTypeSupport.Framework.AdministrationUIHelpers.LegacyFilterDefinitionSettingsPage";

		public static string LegacyFilterDefinitionSettingsPageAssembly = "Sdl.FileTypeSupport.Framework.AdministrationUI";

		public static string FilterDefinitionAdministratorUIClass = "Sdl.FileTypeSupport.Framework.AdministrationUI.FilterDefinitionAdministratorUI";

		public static string FilterDefinitionAdministratorUIAssembly = "Sdl.FileTypeSupport.Framework.AdministrationUI";

		public static string QuickTagClass
		{
			get
			{
				QuickTag quickTag = new QuickTag();
				return quickTag.GetType().FullName;
			}
		}

		public static string CommentMarkerClass
		{
			get
			{
				ICommentMarker commentMarker = new CommentMarker();
				return commentMarker.GetType().FullName;
			}
		}

		public static string OtherMarkerClass
		{
			get
			{
				IOtherMarker otherMarker = new OtherMarker();
				return otherMarker.GetType().FullName;
			}
		}

		public static string SegmentClass
		{
			get
			{
				ISegment segment = new Segment();
				return segment.GetType().FullName;
			}
		}

		public static string TagPairClass
		{
			get
			{
				ITagPair tagPair = new TagPair();
				return tagPair.GetType().FullName;
			}
		}

		public static string LockedContentClass
		{
			get
			{
				ILockedContent lockedContent = new LockedContent();
				return lockedContent.GetType().FullName;
			}
		}

		public static string PlaceholderTagClass
		{
			get
			{
				IPlaceholderTag placeholderTag = new PlaceholderTag();
				return placeholderTag.GetType().FullName;
			}
		}

		public static string StructureTagClass
		{
			get
			{
				IStructureTag structureTag = new StructureTag();
				return structureTag.GetType().FullName;
			}
		}

		public static string TextClass
		{
			get
			{
				IText text = new Text();
				return text.GetType().FullName;
			}
		}

		public static string LocationMarkerClass
		{
			get
			{
				ILocationMarker locationMarker = new LocationMarker();
				return locationMarker.GetType().FullName;
			}
		}

		public static string StartTagPropertiesClass
		{
			get
			{
				IStartTagProperties startTagProperties = new StartTagProperties();
				return startTagProperties.GetType().FullName;
			}
		}

		public static string EndTagPropertiesClass
		{
			get
			{
				IEndTagProperties endTagProperties = new EndTagProperties();
				return endTagProperties.GetType().FullName;
			}
		}

		public static string PlaceholderTagPropertiesClass
		{
			get
			{
				IPlaceholderTagProperties placeholderTagProperties = new PlaceholderTagProperties();
				return placeholderTagProperties.GetType().FullName;
			}
		}

		public static string TextPropertiesClass
		{
			get
			{
				ITextProperties textProperties = new TextProperties();
				return textProperties.GetType().FullName;
			}
		}

		public static string LocationMarkerIdClass
		{
			get
			{
				LocationMarkerId locationMarkerId = new LocationMarkerId();
				return locationMarkerId.GetType().FullName;
			}
		}

		public static string FormattingClass => typeof(FormattingGroup).FullName;

		public static string CommentsClass
		{
			get
			{
				CommentProperties commentProperties = new CommentProperties();
				return commentProperties.GetType().FullName;
			}
		}

		public static string LockedContentPropertiesClass
		{
			get
			{
				ILockedContentProperties lockedContentProperties = new LockedContentProperties();
				return lockedContentProperties.GetType().FullName;
			}
		}

		public static string SerializableIconClass => typeof(IconDescriptor).AssemblyQualifiedName;

		public static string GetSimpleAssemblyName(Type type)
		{
			return new AssemblyName(type.Assembly.FullName).Name;
		}

		public static string GetClassName(Type type)
		{
			return type.FullName;
		}

		public static string GetSpringObjectType(Type type)
		{
			return GetClassName(type) + ", " + GetSimpleAssemblyName(type);
		}
	}
}
