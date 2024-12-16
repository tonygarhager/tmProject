using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public abstract class AbstractTranslationMemoryCreator : ITranslationMemoryCreator
	{
		public abstract string DisplayName
		{
			get;
		}

		public virtual int MaximumNameLength => TranslationMemorySetup.MaximumTranslationMemoryNameLength;

		public virtual int MaximumCopyrightLength => TranslationMemorySetup.MaximumCopyrightFieldLength;

		public virtual int MaximumDescriptionLength => TranslationMemorySetup.MaximumDescriptionFieldLength;

		public ITranslationMemory CreateEmptyTranslationMemory(ITranslationMemorySetupOptions setup)
		{
			ITranslationMemory translationMemory = CreateEmptyTranslationMemory(setup.Name, setup.LanguageDirections, setup.FuzzyIndexes, setup.Recognizers, setup.TokenizerFlags, setup.WordCountFlags, setup.FGASupport, setup.TextContextMatchType, setup.UsesIdContextMatch, setup.UsesLegacyHashes);
			translationMemory.Copyright = setup.Copyright;
			translationMemory.Description = setup.Description;
			if (setup.ExpirationDate.HasValue)
			{
				translationMemory.ExpirationDate = setup.ExpirationDate;
			}
			if (setup.Fields.Count > 0)
			{
				translationMemory.FieldDefinitions.AddRange(setup.Fields);
			}
			foreach (CultureInfo key in setup.LanguageResources.Keys)
			{
				LegacyTranslationMemoryLanguageResources legacyTranslationMemoryLanguageResources = new LegacyTranslationMemoryLanguageResources(setup, setup.LanguageResources[key]);
				translationMemory.LanguageResourceBundles.Add(legacyTranslationMemoryLanguageResources.AsResourceBundle(key));
			}
			translationMemory.Save();
			return translationMemory;
		}

		protected abstract ITranslationMemory CreateEmptyTranslationMemory(string name, IEnumerable<LanguagePair> languageDirections, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, FGASupport fgaSupport, TextContextMatchType textContextMatchType, bool UsesIdContextMatch, bool usesLegacyHashes);

		public abstract bool IsValid(out string errorMessage);

		public virtual bool IsValidName(string translationMemoryName, out string errorMessage)
		{
			if (string.IsNullOrEmpty(translationMemoryName))
			{
				errorMessage = StringResources.AbstractTranslationMemoryCreator_ErrorMessage_NameNotSpecified;
				return false;
			}
			if (translationMemoryName.Length > MaximumNameLength)
			{
				errorMessage = StringResources.AbstractTranslationMemoryCreator_ErrorMessage_NameTooLong;
				return false;
			}
			errorMessage = null;
			return true;
		}

		public virtual bool IsValidCopyright(string translationMemoryCopyright, out string errorMessage)
		{
			if (translationMemoryCopyright != null && translationMemoryCopyright.Length > MaximumCopyrightLength)
			{
				errorMessage = StringResources.AbstractTranslationMemoryCreator_ErrorMessage_CopyrightTooLong;
				return false;
			}
			errorMessage = null;
			return true;
		}

		public virtual bool IsValidDescription(string translationMemoryDescription, out string errorMessage)
		{
			if (translationMemoryDescription != null && translationMemoryDescription.Length > MaximumDescriptionLength)
			{
				errorMessage = StringResources.AbstractTranslationMemoryCreator_ErrorMessage_DescriptionTooLong;
				return false;
			}
			errorMessage = null;
			return true;
		}
	}
}
