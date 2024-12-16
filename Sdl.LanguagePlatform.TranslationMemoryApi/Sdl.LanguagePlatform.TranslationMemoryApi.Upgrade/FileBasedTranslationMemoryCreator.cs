using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class FileBasedTranslationMemoryCreator : AbstractTranslationMemoryCreator, IFileBasedTranslationMemoryCreator, ITranslationMemoryCreator
	{
		private string _translationMemoryDirectory;

		public override string DisplayName => TranslationMemoryDirectory;

		public string TranslationMemoryDirectory
		{
			get
			{
				return _translationMemoryDirectory;
			}
			set
			{
				_translationMemoryDirectory = value;
			}
		}

		protected override ITranslationMemory CreateEmptyTranslationMemory(string name, IEnumerable<LanguagePair> languagePairs, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, FGASupport fgaSupport, TextContextMatchType textContextMatchType, bool usesIdContextMatch, bool usesLegacyHashes)
		{
			if (!IsValid(out string errorMessage))
			{
				throw new InvalidOperationException(errorMessage);
			}
			LanguagePair languagePair = languagePairs.First();
			string tmFilePath = Path.Combine(_translationMemoryDirectory, name + ".sdltm");
			return new FileBasedTranslationMemory(tmFilePath, "", languagePair.SourceCulture, languagePair.TargetCulture, indexes, recognizers, tokenizerFlags, wordCountFlags, fgaSupport != FGASupport.Off, textContextMatchType, usesIdContextMatch, usesLegacyHashes);
		}

		public override bool IsValid(out string errorMessage)
		{
			if (TranslationMemoryDirectory.Length == 0)
			{
				errorMessage = null;
				return true;
			}
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(TranslationMemoryDirectory);
				if (!directoryInfo.Exists)
				{
					errorMessage = StringResources.FileBasedTranslationMemoryCreator_ErrorMessage_LocationNotFound;
					return false;
				}
			}
			catch (Exception ex)
			{
				errorMessage = string.Format(StringResources.FileBasedTranslationMemoryCreator_ErrorMessage_LocationIsNotValid, ex.Message);
				return false;
			}
			errorMessage = null;
			return true;
		}

		public override bool IsValidName(string translationMemoryName, out string errorMessage)
		{
			if (!base.IsValidName(translationMemoryName, out errorMessage))
			{
				return false;
			}
			errorMessage = null;
			return true;
		}
	}
}
