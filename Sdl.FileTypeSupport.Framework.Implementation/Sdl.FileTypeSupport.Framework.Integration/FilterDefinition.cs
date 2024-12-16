using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Implementation.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class FilterDefinition : IFileTypeDefinition
	{
		private IFileTypeComponentBuilder _componentBuilder;

		private IFileTypeInformation _fileTypeInfo;

		private INativeFileSniffer _fileSniffer;

		private bool _isSnifferInitialized;

		public const string FILE_SNIFFER_NAME = "FileSniffer";

		public const string FILTERFRAMEWORK1_FILTER_DEFINITION_FILENAME_NAME = "FilterFramework1FilterDefinitionFile";

		public const string EXTRACTOR_NAME = "Extractor";

		public const string SUBCONTENT_EXTRACTOR_NAME = "SubContentExtractor";

		public const string GENERATOR_NAME = "Generator";

		public const string SUBCONTENT_GENERATOR_NAME = "SubContentGenerator";

		public const string ADDITIONAL_GENERATORS_INFO_NAME = "AdditionalGeneratorsInfo";

		public const string FILETYPE_INFORMATION_NAME = "FileTypeInformation";

		public const string FILETYPE_IDENTITY_NAME = "FileTypeIdentity";

		public const string QUICKTAGS_FACTORY_NAME = "QuickTagsFactory";

		public const string BILINGUAL_WRITER_NAME = "BilingualWriter";

		public const string PREVIEW_SETS_FACTORY_NAME = "PreviewSetsFactory";

		public const string VERIFIER_COLLECTION_NAME = "VerifierCollection";

		public const string INTERACTIVE_PREVIEW_PROCESSOR_NAME = "InteractivePreviewProcessor";

		public const string INTERACTIVE_PREVIEW_CONTROL_FACTORY = "InteractivePreviewComponentFactory";

		public const string GENERATOR_ID_PREFIX = "Generator_";

		public const string PREVIEW_CONTROL_ID_PREFIX = "PreviewControl_";

		public const string PREVIEW_APPLICATION_ID_PREFIX = "PreviewApplication_";

		public virtual IFileTypeInformation FileTypeInformation
		{
			get
			{
				GetFileTypeInformation();
				return _fileTypeInfo;
			}
		}

		public virtual IFileTypeComponentBuilder ComponentBuilder
		{
			get
			{
				return _componentBuilder;
			}
			set
			{
				_componentBuilder = value;
			}
		}

		public FileTypeDefinitionCustomizationLevel CustomizationLevel
		{
			get;
			set;
		}

		public FilterDefinition()
		{
			CustomizationLevel = FileTypeDefinitionCustomizationLevel.Standard;
		}

		public FilterDefinition(IFileTypeComponentBuilder componentBuilder)
		{
			CustomizationLevel = FileTypeDefinitionCustomizationLevel.Standard;
			_componentBuilder = componentBuilder;
		}

		private void GetFileTypeInformation()
		{
			if (_fileTypeInfo == null)
			{
				CheckComponentBuilder();
				_fileTypeInfo = _componentBuilder.BuildFileTypeInformation("FileTypeInformation");
			}
		}

		public virtual IQuickTagsFactory BuildQuickTagsFactory()
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildQuickTagsFactory("QuickTagsFactory");
		}

		public IQuickTags GetQuickInserts(ISettingsBundle settingsBundle, IFileProperties fileProperties)
		{
			IQuickTagsFactory quickTagsFactory = BuildQuickTagsFactory();
			IQuickTags quickTags = null;
			if (quickTagsFactory != null)
			{
				quickTags = quickTagsFactory.GetQuickTags(fileProperties);
			}
			IQuickTags quickTags2 = new QuickTags();
			if (quickTags != null)
			{
				foreach (IQuickTag item in quickTags)
				{
					quickTags2.Add(item);
				}
			}
			IList<IQuickTag> list = QuickInsertsInflator.InflateQuickInserts(settingsBundle, FileTypeInformation.FileTypeDefinitionId.Id);
			foreach (IQuickTag qt in list)
			{
				IEnumerable<IQuickTag> source = quickTags2.Where((IQuickTag tag) => tag.CommandId == qt.CommandId);
				IQuickTag quickTag = null;
				do
				{
					quickTag = source.FirstOrDefault();
					if (quickTag != null)
					{
						quickTag.CommandId += 1.ToString();
					}
				}
				while (quickTag != null);
				quickTags2.Add(qt);
			}
			return quickTags2;
		}

		public virtual IPreviewSetsFactory BuildPreviewSetsFactory()
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildPreviewSetsFactory("PreviewSetsFactory");
		}

		protected void CheckComponentBuilder()
		{
			if (_componentBuilder == null)
			{
				throw new FileTypeSupportException(StringResources.FilterDefinition_NoComponentBuilderError);
			}
		}

		public virtual IFileExtractor BuildExtractor()
		{
			CheckComponentBuilder();
			IFileExtractor fileExtractor = _componentBuilder.BuildFileExtractor("Extractor");
			fileExtractor.FileTypeDefinition = this;
			return fileExtractor;
		}

		public virtual ISubContentExtractor BuildSubContentExtractor()
		{
			CheckComponentBuilder();
			ISubContentComponentBuilder subContentComponentBuilder = _componentBuilder as ISubContentComponentBuilder;
			if (subContentComponentBuilder != null)
			{
				ISubContentExtractor subContentExtractor = subContentComponentBuilder.BuildSubContentExtractor("SubContentExtractor");
				subContentExtractor.FileTypeDefinition = this;
				return subContentExtractor;
			}
			return null;
		}

		public virtual ISubContentGenerator BuildSubContentGenerator()
		{
			CheckComponentBuilder();
			ISubContentComponentBuilder subContentComponentBuilder = _componentBuilder as ISubContentComponentBuilder;
			if (subContentComponentBuilder != null)
			{
				ISubContentGenerator subContentGenerator = subContentComponentBuilder.BuildSubContentGenerator("SubContentGenerator");
				subContentGenerator.FileTypeDefinition = this;
				return subContentGenerator;
			}
			return null;
		}

		public virtual IFileGenerator BuildNativeGenerator()
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildFileGenerator("Generator");
		}

		public virtual IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo()
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildAdditionalGeneratorsInfo("AdditionalGeneratorsInfo");
		}

		public virtual IAbstractGenerator BuildGenerator(GeneratorId generatorId)
		{
			CheckComponentBuilder();
			if (generatorId == GeneratorId.Default)
			{
				IAbstractGenerator abstractGenerator = BuildNativeGenerator();
				if (abstractGenerator == null)
				{
					abstractGenerator = BuildBilingualDocumentGenerator();
				}
				return abstractGenerator;
			}
			return _componentBuilder.BuildAbstractGenerator("Generator_" + generatorId.Id);
		}

		public virtual IAbstractPreviewControl BuildPreviewControl(PreviewControlId previewControlId)
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildPreviewControl("PreviewControl_" + previewControlId.Id);
		}

		public virtual IAbstractPreviewApplication BuildPreviewApplication(PreviewApplicationId previewApplicationId)
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildPreviewApplication("PreviewApplication_" + previewApplicationId.Id);
		}

		public virtual bool IsSupportedFilename(string nativeFilePath)
		{
			GetFileTypeInformation();
			if (_fileTypeInfo.Expression == null)
			{
				return false;
			}
			string fileExtension = Path.GetExtension(nativeFilePath);
			IEnumerable<string> allExtensionsFromWildcard = GetAllExtensionsFromWildcard();
			return allExtensionsFromWildcard.Any((string extension) => string.Equals(extension, fileExtension, StringComparison.InvariantCultureIgnoreCase));
		}

		private IEnumerable<string> GetAllExtensionsFromWildcard()
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(_fileTypeInfo.FileDialogWildcardExpression))
			{
				return list;
			}
			string[] array = _fileTypeInfo.FileDialogWildcardExpression.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				if (!string.IsNullOrEmpty(text2))
				{
					try
					{
						text2 = (text2.Contains('*') ? text2.Substring(text2.LastIndexOf('*') + 1) : text2.Substring(text2.LastIndexOf('.')));
					}
					catch (Exception)
					{
					}
					if (!string.IsNullOrEmpty(text2) && !list.Contains(text2))
					{
						list.Add(text2);
					}
				}
			}
			return list;
		}

		public virtual SniffInfo SniffFile(string nativeFilePath, Language language, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter, ISettingsBundle settingsBundle)
		{
			if (!_isSnifferInitialized)
			{
				lock (this)
				{
					if (!_isSnifferInitialized)
					{
						CheckComponentBuilder();
						_fileSniffer = _componentBuilder.BuildFileSniffer("FileSniffer");
						_isSnifferInitialized = true;
					}
				}
			}
			if (_fileSniffer != null)
			{
				try
				{
					ISettingsGroup settingsGroup = null;
					if (settingsBundle != null && settingsBundle.ContainsSettingsGroup(FileTypeInformation.FileTypeDefinitionId.Id))
					{
						settingsGroup = settingsBundle.GetSettingsGroup(FileTypeInformation.FileTypeDefinitionId.Id);
					}
					return _fileSniffer.Sniff(nativeFilePath, language, suggestedCodepage, messageReporter, settingsGroup);
				}
				catch (Exception ex)
				{
					if (messageReporter != null)
					{
						string message = string.Format(StringResources.FilterDefinition_FileSnifferFailedError, (_fileTypeInfo != null) ? _fileTypeInfo.FileTypeDefinitionId.ToString() : StringResources.FilterDefinition_UnknownDefinitionName, ex.Message);
						messageReporter.ReportMessage(_fileSniffer, StringResources.FileSniffingProcess, ErrorLevel.Warning, message, null);
					}
				}
			}
			SniffInfo sniffInfo = new SniffInfo();
			sniffInfo.IsSupported = true;
			if (suggestedCodepage != null)
			{
				sniffInfo.DetectedEncoding = new Pair<Codepage, DetectionLevel>(suggestedCodepage, DetectionLevel.Guess);
			}
			if (language != null)
			{
				sniffInfo.DetectedSourceLanguage = new Pair<Language, DetectionLevel>(language, DetectionLevel.Guess);
			}
			return sniffInfo;
		}

		public IBilingualDocumentGenerator BuildBilingualDocumentGenerator()
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildBilingualGenerator("BilingualWriter");
		}

		public IVerifierCollection BuildVerifierCollection()
		{
			CheckComponentBuilder();
			return _componentBuilder.BuildVerifierCollection("VerifierCollection");
		}

		public List<QuickInsertIds> BuildQuickInsertIdsList()
		{
			CheckComponentBuilder();
			IQuickInsertsBuilder quickInsertsBuilder = _componentBuilder as IQuickInsertsBuilder;
			if (quickInsertsBuilder != null)
			{
				return quickInsertsBuilder.BuildQuickInsertIdList();
			}
			return new List<QuickInsertIds>();
		}
	}
}
