using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class StringResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("Sdl.LanguagePlatform.TranslationMemoryApi.StringResources", typeof(StringResources).Assembly);
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Upgraded Language Resources.
		/// </summary>
		public static string AbstractTradosLegacyTranslationMemory_UpgradedLanguageResources => ResourceManager.GetString("AbstractTradosLegacyTranslationMemory_UpgradedLanguageResources", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory copyright has not been specified..
		/// </summary>
		public static string AbstractTranslationMemoryCreator_ErrorMessage_CopyrightNotSpecified => ResourceManager.GetString("AbstractTranslationMemoryCreator_ErrorMessage_CopyrightNotSpecified", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory copyright is too long..
		/// </summary>
		public static string AbstractTranslationMemoryCreator_ErrorMessage_CopyrightTooLong => ResourceManager.GetString("AbstractTranslationMemoryCreator_ErrorMessage_CopyrightTooLong", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory description has not been specified..
		/// </summary>
		public static string AbstractTranslationMemoryCreator_ErrorMessage_DescriptionNotSpecified => ResourceManager.GetString("AbstractTranslationMemoryCreator_ErrorMessage_DescriptionNotSpecified", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory description is too long..
		/// </summary>
		public static string AbstractTranslationMemoryCreator_ErrorMessage_DescriptionTooLong => ResourceManager.GetString("AbstractTranslationMemoryCreator_ErrorMessage_DescriptionTooLong", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory name has not been specified..
		/// </summary>
		public static string AbstractTranslationMemoryCreator_ErrorMessage_NameNotSpecified => ResourceManager.GetString("AbstractTranslationMemoryCreator_ErrorMessage_NameNotSpecified", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory name is too long..
		/// </summary>
		public static string AbstractTranslationMemoryCreator_ErrorMessage_NameTooLong => ResourceManager.GetString("AbstractTranslationMemoryCreator_ErrorMessage_NameTooLong", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Container.
		/// </summary>
		public static string Container => ResourceManager.GetString("Container", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Containers.
		/// </summary>
		public static string Containers => ResourceManager.GetString("Containers", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Copyright.
		/// </summary>
		public static string Copyright => ResourceManager.GetString("Copyright", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Creation Date.
		/// </summary>
		public static string CreationDate => ResourceManager.GetString("CreationDate", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Creation User Name.
		/// </summary>
		public static string CreationUserName => ResourceManager.GetString("CreationUserName", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Database Name.
		/// </summary>
		public static string DatabaseName => ResourceManager.GetString("DatabaseName", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Database Server.
		/// </summary>
		public static string DatabaseServer => ResourceManager.GetString("DatabaseServer", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The database server entity should have an Id..
		/// </summary>
		public static string DatabaseServer_EntityHasNoId => ResourceManager.GetString("DatabaseServer_EntityHasNoId", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Description.
		/// </summary>
		public static string Description => ResourceManager.GetString("Description", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to This type of translation memory cannot be exported: {0}.
		/// </summary>
		public static string EMSG_CannotExport => ResourceManager.GetString("EMSG_CannotExport", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to You cannot export this translation memory. This is already a TMX-based transalation memory..
		/// </summary>
		public static string EMSG_CannotExportTMX => ResourceManager.GetString("EMSG_CannotExportTMX", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Error while writing output file.
		/// </summary>
		public static string EMSG_ErrorWritingOutputFile => ResourceManager.GetString("EMSG_ErrorWritingOutputFile", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Field name conflict (reserved name).
		/// </summary>
		public static string EMSG_FieldNameConflict => ResourceManager.GetString("EMSG_FieldNameConflict", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to You cannot set the TMX file path. This is already a TMX-based transalation memory..
		/// </summary>
		public static string EMSG_InputIsTMX => ResourceManager.GetString("EMSG_InputIsTMX", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Invalid locale: {0}.
		/// </summary>
		public static string EMSG_InvalidLocale => ResourceManager.GetString("EMSG_InvalidLocale", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The connection url is invalid: {0}.
		/// </summary>
		public static string EMSG_InvalidUrl => ResourceManager.GetString("EMSG_InvalidUrl", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Multiple date field values are not supported.
		/// </summary>
		public static string EMSG_MultipleValueDateFieldsUnsupported => ResourceManager.GetString("EMSG_MultipleValueDateFieldsUnsupported", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to There are no translation memories to process..
		/// </summary>
		public static string EMSG_NoTranslationMemoriesToProcess => ResourceManager.GetString("EMSG_NoTranslationMemoriesToProcess", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to SDLX temp file creation failed..
		/// </summary>
		public static string EMSG_SdlxTempFileCreationFailed => ResourceManager.GetString("EMSG_SdlxTempFileCreationFailed", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Failed to convert SRX segmentation rules..
		/// </summary>
		public static string EMSG_SRXConversionFailed => ResourceManager.GetString("EMSG_SRXConversionFailed", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to You must specify a TMX file path before starting the export..
		/// </summary>
		public static string EMSG_TMXPathUnspecified => ResourceManager.GetString("EMSG_TMXPathUnspecified", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Error during parsing XML return value.
		/// </summary>
		public static string EMSG_XmlParseError => ResourceManager.GetString("EMSG_XmlParseError", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to User {0} does not have required permissions {1}..
		/// </summary>
		public static string ErrorAssertPermission => ResourceManager.GetString("ErrorAssertPermission", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Error, must be called with a valid SDL Thread principal..
		/// </summary>
		public static string ErrorInvalidSdlThreadPrincipal => ResourceManager.GetString("ErrorInvalidSdlThreadPrincipal", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Expiration Date.
		/// </summary>
		public static string ExpirationDate => ResourceManager.GetString("ExpirationDate", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Is Picklist.
		/// </summary>
		public static string FD_IsPicklist => ResourceManager.GetString("FD_IsPicklist", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Value Type.
		/// </summary>
		public static string FD_ValueType => ResourceManager.GetString("FD_ValueType", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Field Definitions.
		/// </summary>
		public static string FieldDefinitions => ResourceManager.GetString("FieldDefinitions", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Fields Template.
		/// </summary>
		public static string FieldsTemplate => ResourceManager.GetString("FieldsTemplate", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The specified output translation memory location is not valid.
		///             ({0}).
		/// </summary>
		public static string FileBasedTranslationMemoryCreator_ErrorMessage_LocationIsNotValid => ResourceManager.GetString("FileBasedTranslationMemoryCreator_ErrorMessage_LocationIsNotValid", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The specified output translation memory location could not be found..
		/// </summary>
		public static string FileBasedTranslationMemoryCreator_ErrorMessage_LocationNotFound => ResourceManager.GetString("FileBasedTranslationMemoryCreator_ErrorMessage_LocationNotFound", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Creates a file based translation memory object..
		/// </summary>
		public static string FileBasedTranslationMemoryProvider_Description => ResourceManager.GetString("FileBasedTranslationMemoryProvider_Description", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to File based translation memory provider.
		/// </summary>
		public static string FileBasedTranslationMemoryProvider_Name => ResourceManager.GetString("FileBasedTranslationMemoryProvider_Name", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Fuzzy Indexes.
		/// </summary>
		public static string FuzzyIndexes => ResourceManager.GetString("FuzzyIndexes", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Cannot determine the source language of the import file. .
		/// </summary>
		public static string ImportWizard_CannotValidateSourceLang => ResourceManager.GetString("ImportWizard_CannotValidateSourceLang", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Cannot determine the target language of the import file. There may be no target content?.
		/// </summary>
		public static string ImportWizard_CannotValidateTargetLang => ResourceManager.GetString("ImportWizard_CannotValidateTargetLang", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The language pair of the import file [{0}-&gt;{1}] does not match the language pair of the Translation Memory [{2}-&gt;{3}].
		/// </summary>
		public static string Importwizard_NoLanguagePairMatch => ResourceManager.GetString("Importwizard_NoLanguagePairMatch", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Language Directions.
		/// </summary>
		public static string LanguageDirections => ResourceManager.GetString("LanguageDirections", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Language Resource Bundles.
		/// </summary>
		public static string LanguageResourceBundles => ResourceManager.GetString("LanguageResourceBundles", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Language Resource Template.
		/// </summary>
		public static string LanguageResourceTemplate => ResourceManager.GetString("LanguageResourceTemplate", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Linked Resource Group Paths.
		/// </summary>
		public static string LinkedResourceGroupPaths => ResourceManager.GetString("LinkedResourceGroupPaths", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Data.
		/// </summary>
		public static string LRD_Data => ResourceManager.GetString("LRD_Data", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Type.
		/// </summary>
		public static string LRD_Type => ResourceManager.GetString("LRD_Type", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to SDL Automated Translation Server.
		/// </summary>
		public static string MachineTranslation_ServerConnection => ResourceManager.GetString("MachineTranslation_ServerConnection", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The export was not found. Either this export has been deleted or you do not have sufficient privileges to access this export..
		/// </summary>
		public static string MessageExportNotFound => ResourceManager.GetString("MessageExportNotFound", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The import was not found. Either this import has been deleted or you do not have sufficient privileges to access this import..
		/// </summary>
		public static string MessageImportNotFound => ResourceManager.GetString("MessageImportNotFound", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Name.
		/// </summary>
		public static string Name => ResourceManager.GetString("Name", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Enter any unique and friendly name you want to use for the server..
		/// </summary>
		public static string NameToolTip => ResourceManager.GetString("NameToolTip", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Not unlocked with sufficient permissions..
		/// </summary>
		public static string NotUnlockedWithSufficientPermissions => ResourceManager.GetString("NotUnlockedWithSufficientPermissions", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Field.
		/// </summary>
		public static string OutputTranslationMemory_DefaultFieldName => ResourceManager.GetString("OutputTranslationMemory_DefaultFieldName", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory field conflicts have not been resolved..
		/// </summary>
		public static string OutputTranslationMemory_ErrorMessage_FieldConflicts => ResourceManager.GetString("OutputTranslationMemory_ErrorMessage_FieldConflicts", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The input translation memories have incompatible source or target languages..
		/// </summary>
		public static string OutputTranslationMemory_ErrorMessage_IncompatibleLanguages => ResourceManager.GetString("OutputTranslationMemory_ErrorMessage_IncompatibleLanguages", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The input translation memories have not been specified..
		/// </summary>
		public static string OutputTranslationMemory_ErrorMessage_InputLanguagePairsNotSpecified => ResourceManager.GetString("OutputTranslationMemory_ErrorMessage_InputLanguagePairsNotSpecified", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory location has not been specified..
		/// </summary>
		public static string OutputTranslationMemory_ErrorMessage_LocationNotSpecified => ResourceManager.GetString("OutputTranslationMemory_ErrorMessage_LocationNotSpecified", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The output translation memory name has not been specified..
		/// </summary>
		public static string OutputTranslationMemory_ErrorMessage_NameNotSpecified => ResourceManager.GetString("OutputTranslationMemory_ErrorMessage_NameNotSpecified", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The TMX file for translation memory '{0}' is not available..
		/// </summary>
		public static string OutputTranslationMemory_ErrorMessage_TMXNotAvailable => ResourceManager.GetString("OutputTranslationMemory_ErrorMessage_TMXNotAvailable", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Completed.
		/// </summary>
		public static string OutputTranslationMemory_ProgressMessage_Completed => ResourceManager.GetString("OutputTranslationMemory_ProgressMessage_Completed", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Creating translation memory '{0}'.
		/// </summary>
		public static string OutputTranslationMemory_ProgressMessage_CreatingTranslationMemory => ResourceManager.GetString("OutputTranslationMemory_ProgressMessage_CreatingTranslationMemory", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Importing {0} (read: {1}, imported: {2}, errors: {3}.
		/// </summary>
		public static string OutputTranslationMemory_ProgressMessage_Importing => ResourceManager.GetString("OutputTranslationMemory_ProgressMessage_Importing", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Importing {0}: (read: {1}, imported: {2}, errors: {3}, total: {4}.
		/// </summary>
		public static string OutputTranslationMemory_ProgressMessage_ImportingWithTotal => ResourceManager.GetString("OutputTranslationMemory_ProgressMessage_ImportingWithTotal", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Parent Resource Group Path.
		/// </summary>
		public static string ParentResourceGroupPath => ResourceManager.GetString("ParentResourceGroupPath", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Password.
		/// </summary>
		public static string Password => ResourceManager.GetString("Password", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Picklist Items.
		/// </summary>
		public static string PicklistItems => ResourceManager.GetString("PicklistItems", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Recognizers.
		/// </summary>
		public static string Recognizers => ResourceManager.GetString("Recognizers", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Server Name.
		/// </summary>
		public static string ServerName => ResourceManager.GetString("ServerName", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Enter an existing server name (usually a URL or instance name)..
		/// </summary>
		public static string ServerNameToolTip => ResourceManager.GetString("ServerNameToolTip", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Server Type.
		/// </summary>
		public static string ServerType => ResourceManager.GetString("ServerType", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Source Language.
		/// </summary>
		public static string SourceLanguage => ResourceManager.GetString("SourceLanguage", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Supported Language Directions.
		/// </summary>
		public static string SupportedLanguageDirections => ResourceManager.GetString("SupportedLanguageDirections", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Target Language.
		/// </summary>
		public static string TargetLanguage => ResourceManager.GetString("TargetLanguage", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Test.
		/// </summary>
		public static string Test => ResourceManager.GetString("Test", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to This server does not support TM Server functionality: {0}..
		/// </summary>
		public static string TmServiceNotAvailable => ResourceManager.GetString("TmServiceNotAvailable", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Translation Memories.
		/// </summary>
		public static string TranslationMemories => ResourceManager.GetString("TranslationMemories", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The container entity should have an Id..
		/// </summary>
		public static string TranslationMemoryContainer_EntryHasNoID => ResourceManager.GetString("TranslationMemoryContainer_EntryHasNoID", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Translation Provider.
		/// </summary>
		public static string TranslationProvider => ResourceManager.GetString("TranslationProvider", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Translation Provider Server.
		/// </summary>
		public static string TranslationProviderServer => ResourceManager.GetString("TranslationProviderServer", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Items.
		/// </summary>
		public static string TS_Items => ResourceManager.GetString("TS_Items", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The specified URI is not a valid server-based translation memory URI..
		/// </summary>
		public static string UnvalidServerBasedTMUri => ResourceManager.GetString("UnvalidServerBasedTMUri", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to User Name.
		/// </summary>
		public static string UserName => ResourceManager.GetString("UserName", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Warning, database server {0} not found within scope of organization {1}..
		/// </summary>
		public static string WarningDatabaseServerNotFound => ResourceManager.GetString("WarningDatabaseServerNotFound", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Warning, OrganizationId not found for {0}, {1}..
		/// </summary>
		public static string WarningOrgIdNotFoundForObject => ResourceManager.GetString("WarningOrgIdNotFoundForObject", resourceCulture);

		internal StringResources()
		{
		}
	}
}
