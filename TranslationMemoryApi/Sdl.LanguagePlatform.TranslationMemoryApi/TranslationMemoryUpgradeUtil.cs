using Sdl.Core.FineGrainedAlignment;
using Sdl.Core.LanguageProcessing;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Utility class for upgrading file-based translation memories and 
	/// checking if they need maintenance
	/// </summary>
	public class TranslationMemoryUpgradeUtil
	{
		private static bool UpgradeToStrictHashingDisabled => FileBasedTranslationMemory.FileBasedTranslationMemoryDescriptor.UseLegacyHashingByDefault;

		/// <summary>
		/// Determines if TM requires model rebuild
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool TranslationMemoryRequiresModelRebuild(ITranslationProvider translationMemory)
		{
			FileBasedTranslationMemory fileBasedTranslationMemory = translationMemory as FileBasedTranslationMemory;
			if (fileBasedTranslationMemory == null)
			{
				return false;
			}
			if (fileBasedTranslationMemory.FGASupport == FGASupport.Legacy || fileBasedTranslationMemory.FGASupport == FGASupport.Off)
			{
				return false;
			}
			AlignerDefinition alignerDefinition = fileBasedTranslationMemory.AlignerDefinition;
			if (alignerDefinition == null)
			{
				return false;
			}
			if (!alignerDefinition.IsModelFree)
			{
				TranslationModelDetails modelDetails = fileBasedTranslationMemory.ModelDetails;
				if (modelDetails == null)
				{
					return false;
				}
				if (!modelDetails.ModelDate.HasValue)
				{
					return false;
				}
			}
			return fileBasedTranslationMemory.ShouldBuildModel;
		}

		/// <summary>
		/// Determine if TM has content that should be aligned (FGA)
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool TranslationMemoryRequiresAlignment(ITranslationProvider translationMemory)
		{
			FileBasedTranslationMemory fileBasedTranslationMemory = translationMemory as FileBasedTranslationMemory;
			if (fileBasedTranslationMemory == null)
			{
				return false;
			}
			if (fileBasedTranslationMemory.FGASupport == FGASupport.Legacy || fileBasedTranslationMemory.FGASupport == FGASupport.Off)
			{
				return false;
			}
			AlignerDefinition alignerDefinition = fileBasedTranslationMemory.AlignerDefinition;
			if (alignerDefinition == null)
			{
				return false;
			}
			if (!alignerDefinition.IsModelFree)
			{
				TranslationModelDetails modelDetails = fileBasedTranslationMemory.ModelDetails;
				if (modelDetails == null)
				{
					return false;
				}
				if (!modelDetails.ModelDate.HasValue)
				{
					return false;
				}
			}
			return fileBasedTranslationMemory.ShouldAlign;
		}

		/// <summary>
		/// Determine if TM is indicating it has content that needs reindexing
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool TranslationMemoryRequiresReindex(ITranslationProvider translationMemory)
		{
			FileBasedTranslationMemory fileBasedTranslationMemory = translationMemory as FileBasedTranslationMemory;
			if (fileBasedTranslationMemory == null)
			{
				return false;
			}
			if (!fileBasedTranslationMemory.CanReportReindexRequired)
			{
				return false;
			}
			bool? reindexRequired = fileBasedTranslationMemory.ReindexRequired;
			if (reindexRequired.HasValue && reindexRequired.Value)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determine if the TM requires upgrade
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool TranslationMemoryRequiresUpgrade(ITranslationProvider translationMemory)
		{
			if (translationMemory is ServerBasedTranslationMemory)
			{
				return false;
			}
			IReindexableTranslationMemory reindexableTranslationMemory = translationMemory as IReindexableTranslationMemory;
			bool flag = false;
			if (reindexableTranslationMemory != null)
			{
				flag = !reindexableTranslationMemory.CanReportReindexRequired;
			}
			if (!UpgradeToStrictHashingDisabled)
			{
				IAdvancedContextTranslationMemory advancedContextTranslationMemory = translationMemory as IAdvancedContextTranslationMemory;
				if (advancedContextTranslationMemory != null)
				{
					flag |= advancedContextTranslationMemory.UsesLegacyHashes;
				}
			}
			return flag;
		}

		/// <summary>
		/// Determine if the TM will require a reindex after upgrade
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool TranslationMemoryWillRequireReindexAfterUpgrade(ITranslationMemory translationMemory)
		{
			bool flag = false;
			IReindexableTranslationMemory reindexableTranslationMemory = translationMemory as IReindexableTranslationMemory;
			flag = (reindexableTranslationMemory == null || !reindexableTranslationMemory.ReindexRequired.HasValue || reindexableTranslationMemory.ReindexRequired.Value);
			if (!UpgradeToStrictHashingDisabled)
			{
				IAdvancedContextTranslationMemory advancedContextTranslationMemory = translationMemory as IAdvancedContextTranslationMemory;
				if (advancedContextTranslationMemory != null)
				{
					flag |= advancedContextTranslationMemory.UsesLegacyHashes;
				}
			}
			return flag;
		}

		/// <summary>
		/// Determine if the TM requires an upgrade with an Asian source language
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool TranslationMemoryWithAsianLanguageRequiresUpgrade(ITranslationMemory translationMemory)
		{
			IReindexableTranslationMemory reindexableTranslationMemory = translationMemory as IReindexableTranslationMemory;
			if (reindexableTranslationMemory != null)
			{
				if (!reindexableTranslationMemory.CanReportReindexRequired && translationMemory.SupportedLanguageDirections.Count == 1)
				{
					if (!UseCharacterCounts(translationMemory.SupportedLanguageDirections[0].SourceCulture))
					{
						return UseCharacterCounts(translationMemory.SupportedLanguageDirections[0].TargetCulture);
					}
					return true;
				}
				return false;
			}
			return false;
		}

		/// <summary>
		/// Modifies the translation memory settings so as to prompt in-place upgrade when saved
		/// </summary>
		/// <param name="translationMemory"></param>
		public static void UpgradeTranslationMemory(ITranslationMemory translationMemory)
		{
			IReindexableTranslationMemory reindexableTranslationMemory = translationMemory as IReindexableTranslationMemory;
			if (reindexableTranslationMemory != null)
			{
				reindexableTranslationMemory.CanReportReindexRequired = true;
			}
			if (!UpgradeToStrictHashingDisabled)
			{
				IAdvancedContextTranslationMemory advancedContextTranslationMemory = translationMemory as IAdvancedContextTranslationMemory;
				if (advancedContextTranslationMemory != null)
				{
					advancedContextTranslationMemory.UsesLegacyHashes = false;
				}
			}
		}

		/// <summary>
		/// Determine if the TM requires upgrade or FGA support is recommended
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool TranslationMemoryRequiresUpgradeOrFGASupportRecommended(ITranslationProvider translationMemory)
		{
			if (!TranslationMemoryRequiresUpgrade(translationMemory))
			{
				return FineGrainedAlignmentSupportRecommended(translationMemory as ITranslationMemory);
			}
			return true;
		}

		/// <summary>
		/// Determine if file grained alingment (FGA) support is recommended
		/// </summary>
		/// <param name="translationMemory"></param>
		/// <returns></returns>
		public static bool FineGrainedAlignmentSupportRecommended(ITranslationMemory translationMemory)
		{
			IFileBasedTranslationMemory fileBasedTranslationMemory = translationMemory as IFileBasedTranslationMemory;
			if (fileBasedTranslationMemory != null)
			{
				int num = 1000;
				bool flag = false;
				try
				{
					flag = (fileBasedTranslationMemory.GetTranslationUnitCount() >= num);
				}
				catch
				{
				}
				if (flag && fileBasedTranslationMemory.FGASupport != FGASupport.NonAutomatic)
				{
					return fileBasedTranslationMemory.FGASupport != FGASupport.Automatic;
				}
				return false;
			}
			return false;
		}

		/// <summary>
		/// Diferentiate between Asian/non Asian source culture
		/// </summary>
		/// <param name="cultureInfo"></param>
		/// <returns></returns>
		public static bool UseCharacterCounts(CultureInfo cultureInfo)
		{
			if (!CultureInfoExtensions.UseBlankAsWordSeparator(cultureInfo))
			{
				return AdvancedTokenization.TokenizesToWords(cultureInfo);
			}
			return false;
		}
	}
}
