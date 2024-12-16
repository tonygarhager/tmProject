using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sdl.Core.LanguageProcessing
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class ErrorMessages
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("Sdl.Core.LanguageProcessing.ErrorMessages", typeof(ErrorMessages).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
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

		internal static string EMSG_ConfigurationAbstractType => ResourceManager.GetString("EMSG_ConfigurationAbstractType", resourceCulture);

		internal static string EMSG_ConfigurationCannotInstantiateOrCastType => ResourceManager.GetString("EMSG_ConfigurationCannotInstantiateOrCastType", resourceCulture);

		internal static string EMSG_ConfigurationCannotResolveType => ResourceManager.GetString("EMSG_ConfigurationCannotResolveType", resourceCulture);

		internal static string EMSG_ConfigurationInvalidType => ResourceManager.GetString("EMSG_ConfigurationInvalidType", resourceCulture);

		internal static string EMSG_InvalidLanguageResourceFile => ResourceManager.GetString("EMSG_InvalidLanguageResourceFile", resourceCulture);

		internal static string EMSG_LanguageResourceFileNotFound => ResourceManager.GetString("EMSG_LanguageResourceFileNotFound", resourceCulture);

		internal static string EMSG_NoRegionSpecificLanguageFound => ResourceManager.GetString("EMSG_NoRegionSpecificLanguageFound", resourceCulture);

		internal static string EMSG_ResourceNotAvailable => ResourceManager.GetString("EMSG_ResourceNotAvailable", resourceCulture);

		internal static string EMSG_SegmentationEmptyRuleSet => ResourceManager.GetString("EMSG_SegmentationEmptyRuleSet", resourceCulture);

		internal static string EMSG_SegmentationIllegalContinuation => ResourceManager.GetString("EMSG_SegmentationIllegalContinuation", resourceCulture);

		internal static string EMSG_SegmentationIllegalKeywordInRule => ResourceManager.GetString("EMSG_SegmentationIllegalKeywordInRule", resourceCulture);

		internal static string EMSG_SegmentationInvalidRule => ResourceManager.GetString("EMSG_SegmentationInvalidRule", resourceCulture);

		internal static string EMSG_SegmentationInvalidVariableName => ResourceManager.GetString("EMSG_SegmentationInvalidVariableName", resourceCulture);

		internal static string EMSG_SegmentationNoRulesForLanguage => ResourceManager.GetString("EMSG_SegmentationNoRulesForLanguage", resourceCulture);

		internal static string EMSG_SegmentationRuleDeserializationError => ResourceManager.GetString("EMSG_SegmentationRuleDeserializationError", resourceCulture);

		internal static string EMSG_SegmentationRuleLoadError => ResourceManager.GetString("EMSG_SegmentationRuleLoadError", resourceCulture);

		internal static string EMSG_SegmentationTrailingJunk => ResourceManager.GetString("EMSG_SegmentationTrailingJunk", resourceCulture);

		internal static string EMSG_SegmentationUnknownRuleType => ResourceManager.GetString("EMSG_SegmentationUnknownRuleType", resourceCulture);

		internal static string EMSG_SegmentationUnknownVariable => ResourceManager.GetString("EMSG_SegmentationUnknownVariable", resourceCulture);

		internal static string EMSG_SegmentNotTokenized => ResourceManager.GetString("EMSG_SegmentNotTokenized", resourceCulture);

		internal static string EMSG_StemmerErrorInStemmingRule => ResourceManager.GetString("EMSG_StemmerErrorInStemmingRule", resourceCulture);

		internal static string EMSG_TokenizerInvalidCharacterSet => ResourceManager.GetString("EMSG_TokenizerInvalidCharacterSet", resourceCulture);

		internal static string EMSG_TokenizerInvalidNumericFormat => ResourceManager.GetString("EMSG_TokenizerInvalidNumericFormat", resourceCulture);

		internal ErrorMessages()
		{
		}
	}
}
