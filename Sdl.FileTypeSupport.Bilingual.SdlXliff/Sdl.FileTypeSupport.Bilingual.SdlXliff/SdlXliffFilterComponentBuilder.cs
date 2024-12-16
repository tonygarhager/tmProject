using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Bilingual.SdlXliff.Preview;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Core.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.FileTypeSupport.Framework.PreviewControls;
using System;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	[FileTypeComponentBuilder(Id = "SdlXliff_FilterComponentBuilderExtension_Id", Name = "SdlXliff_FilterComponentBuilderExtension_Name", Description = "SdlXliff_FilterComponentBuilderExtension_Description")]
	public class SdlXliffFilterComponentBuilder : IFileTypeComponentBuilder, IFileTypeDefinitionAware, IDefaultFileTypeSettingsProvider, IFileTypeSettingsConverterComponentBuilder
	{
		public IFileTypeManager FileTypeManager
		{
			get;
			set;
		}

		public virtual IFileTypeDefinition FileTypeDefinition
		{
			get;
			set;
		}

		public virtual IAbstractGenerator BuildAbstractGenerator(string name)
		{
			if (name == "Generator_XliffHtmlPreview")
			{
				return FileTypeManager.BuildBilingualGenerator(new XliffPreviewXmlWriter());
			}
			return null;
		}

		public virtual IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
		{
			return null;
		}

		public virtual IBilingualDocumentGenerator BuildBilingualGenerator(string name)
		{
			return FileTypeManager.BuildBilingualGenerator(new XliffFileWriter
			{
				MaxEmbeddableFileSize = 20971520L
			});
		}

		public virtual IFileExtractor BuildFileExtractor(string name)
		{
			return FileTypeManager.BuildFileExtractor(new XliffFileReader
			{
				ValidateXliff = true
			}, this);
		}

		public virtual IFileGenerator BuildFileGenerator(string name)
		{
			return null;
		}

		public virtual INativeFileSniffer BuildFileSniffer(string name)
		{
			return new SdlXliffFileSniffer
			{
				ReportUnsupportedOlderVersions = true,
				DisableVersionCheck = false
			};
		}

		public virtual IFileTypeInformation BuildFileTypeInformation(string name)
		{
			IFileTypeInformation fileTypeInformation = FileTypeManager.BuildFileTypeInformation();
			fileTypeInformation.FileTypeDefinitionId = new FileTypeDefinitionId("SDL XLIFF 1.0 v 1.0.0.0");
			fileTypeInformation.IsBilingualDocumentFileType = true;
			fileTypeInformation.FileTypeName = new LocalizableString(StringResources.FileTypeName);
			fileTypeInformation.FileTypeDocumentName = new LocalizableString(StringResources.FileTypeNameSingular);
			fileTypeInformation.FileTypeDocumentsName = new LocalizableString(StringResources.FileTypeNamePlural);
			fileTypeInformation.Description = new LocalizableString(StringResources.FileTypeDescription);
			fileTypeInformation.FileTypeFrameworkVersion = new Version("2.3.0.0");
			fileTypeInformation.FileDialogWildcardExpression = "*.sdlxliff";
			fileTypeInformation.DefaultFileExtension = "sdlxliff";
			fileTypeInformation.Icon = new IconDescriptor("assembly://Sdl.FileTypeSupport.Bilingual.SdlXliff/Sdl.FileTypeSupport.Bilingual.SdlXliff.SDLXLIFF.ico");
			fileTypeInformation.SilverlightSettingsPageIds = new string[1]
			{
				"SdlXliff_GeneralSettings"
			};
			fileTypeInformation.WinFormSettingsPageIds = new string[1]
			{
				"SdlXliff_GeneralSettings"
			};
			return fileTypeInformation;
		}

		public virtual IAbstractPreviewApplication BuildPreviewApplication(string name)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			if (!(name == "PreviewApplication_ExternalBrowser"))
			{
				return null;
			}
			return (IAbstractPreviewApplication)new GenericExteralPreviewApplication();
		}

		public virtual IAbstractPreviewControl BuildPreviewControl(string name)
		{
			return null;
		}

		public virtual IPreviewSetsFactory BuildPreviewSetsFactory(string name)
		{
			IPreviewSetsFactory previewSetsFactory = FileTypeManager.BuildPreviewSetsFactory();
			IPreviewSet previewSet = previewSetsFactory.CreatePreviewSet();
			previewSet.Id = new PreviewSetId("ExternalXliffHtmlPreview");
			previewSet.Name = new LocalizableString(StringResources.ExternalHtmlPreview_Name);
			previewSet.Description = new LocalizableString(StringResources.ExternalHtmlPreview_Description);
			IApplicationPreviewType applicationPreviewType = previewSetsFactory.CreatePreviewType<IApplicationPreviewType>() as IApplicationPreviewType;
			if (applicationPreviewType != null)
			{
				applicationPreviewType.TargetGeneratorId = new GeneratorId("XliffHtmlPreview");
				applicationPreviewType.SingleFilePreviewApplicationId = new PreviewApplicationId("ExternalBrowser");
			}
			previewSet.SideBySide = applicationPreviewType;
			previewSetsFactory.GetPreviewSets(null).Add(previewSet);
			return previewSetsFactory;
		}

		public virtual IQuickTagsFactory BuildQuickTagsFactory(string name)
		{
			return FileTypeManager.BuildQuickTagsFactory();
		}

		public virtual IVerifierCollection BuildVerifierCollection(string name)
		{
			return FileTypeManager.BuildVerifierCollection();
		}

		public void PopulateDefaultSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			new SdlXliffGeneralSettings().SaveDefaultsToSettingsBundle(settingsBundle, fileTypeConfigurationId);
		}

		public IFileTypeSettingsConverter BuildFileTypeSettingsConverter(string name)
		{
			return new GenericFileTypeSettingsConverter(SettingsFormatConverter.ConvertSettings<SdlXliffGeneralSettings>);
		}
	}
}
