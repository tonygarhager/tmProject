using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryTools;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.TM.ImportExport
{
	internal sealed class BilingualContentImporter : IBilingualContentHandler
	{
		private IDocumentProperties _documentProperties;

		private readonly Importer _importer;

		private readonly LanguagePair _tmLanguageDirection;

		private LanguagePair _documentLanguageDirection;

		private bool _swapLanguageDirection;

		private bool _isItdFile;

		public ImportStatistics ImportStatistics
		{
			get;
			private set;
		}

		public BilingualContentImporter(Importer parent, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			_importer = parent;
			_tmLanguageDirection = new LanguagePair(sourceLanguage, targetLanguage);
			ImportStatistics = new ImportStatistics();
		}

		public void Complete()
		{
			_importer.Flush(ImportStatistics);
		}

		public void FileComplete()
		{
			_importer.Flush(ImportStatistics);
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;
			CultureInfo cultureInfo = _documentProperties.SourceLanguage.CultureInfo;
			CultureInfo cultureInfo2 = _documentProperties.TargetLanguage.CultureInfo;
			_documentLanguageDirection = new LanguagePair(cultureInfo, cultureInfo2);
			_swapLanguageDirection = _importer.CheckLanguageDirections(cultureInfo, cultureInfo2, out bool languageDirectionCompatible, allowReverse: true);
			if (!languageDirectionCompatible)
			{
				throw new LanguageMismatchException(cultureInfo, cultureInfo2, _tmLanguageDirection.SourceCulture, _tmLanguageDirection.TargetCulture);
			}
		}

		public static ConfirmationLevel MapItdConfirmationLevel(ConfirmationLevel ffwConfirmationLevel, ITranslationOrigin translationOrigin)
		{
			if (ffwConfirmationLevel == ConfirmationLevel.Draft)
			{
				if (translationOrigin.MatchPercent < 100)
				{
					return ConfirmationLevel.Draft;
				}
				return ConfirmationLevel.Translated;
			}
			return ffwConfirmationLevel;
		}

		public void ProcessParagraphUnit(IParagraphUnit transUnit)
		{
			if (transUnit != null && !transUnit.IsStructure && transUnit.Properties.LockType == LockTypeFlags.Unlocked)
			{
				foreach (ISegmentPair segmentPair in transUnit.SegmentPairs)
				{
					if (segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Unspecified || !segmentPair.Target.AllSubItems.Any())
					{
						ImportStatistics.DiscardedTranslationUnits++;
						ImportStatistics.TotalRead++;
					}
					else
					{
						ConfirmationLevel value = _isItdFile ? MapItdConfirmationLevel(segmentPair.Properties.ConfirmationLevel, segmentPair.Properties.TranslationOrigin) : segmentPair.Properties.ConfirmationLevel;
						ConfirmationLevel[] confirmationLevels = _importer.ImportSettings.ConfirmationLevels;
						if (confirmationLevels != null && Array.IndexOf(confirmationLevels, value) == -1)
						{
							ImportStatistics.DiscardedTranslationUnits++;
							ImportStatistics.TotalRead++;
						}
						else if (segmentPair.Source != null && segmentPair.Target != null)
						{
							TranslationUnit translationUnit;
							if (_swapLanguageDirection)
							{
								translationUnit = TUConverter.BuildLinguaTranslationUnit(_documentLanguageDirection, segmentPair, transUnit.Properties, _importer.ImportSettings.PlainText, excludeTagsInLockedContentText: false);
								Segment sourceSegment = translationUnit.SourceSegment;
								translationUnit.SourceSegment = translationUnit.TargetSegment;
								translationUnit.TargetSegment = sourceSegment;
							}
							else
							{
								translationUnit = TUConverter.BuildLinguaTranslationUnit(_tmLanguageDirection, segmentPair, transUnit.Properties, _importer.ImportSettings.PlainText, excludeTagsInLockedContentText: false);
							}
							if (translationUnit != null)
							{
								translationUnit.CheckAndComputeTagAssociations();
								_importer.ImportTranslationUnit(translationUnit, ImportStatistics);
							}
						}
					}
				}
			}
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			if (fileInfo?.FileConversionProperties != null)
			{
				_isItdFile = (string.Compare(Path.GetExtension(fileInfo.FileConversionProperties.OriginalFilePath), ".itd", StringComparison.OrdinalIgnoreCase) == 0);
			}
		}
	}
}
