using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.Extensions;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Operations;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters
{
	internal class BcmToBilingual
	{
		public event EventHandler<BcmTuConversionEventArgs> TranslationUnitCreated;

		public TranslationUnit[] ConvertToTranslationUnits(Document inputDocument)
		{
			IDocumentProperties documentProperties = CreateDocumentProperties(inputDocument);
			List<TranslationUnit> list = new List<TranslationUnit>();
			foreach (File file in inputDocument.Files)
			{
				BcmToBilingualConverter converter = new BcmToBilingualConverter(file, null);
				IFileProperties fileProperties = CreateFileProperties(file);
				foreach (Sdl.Core.Bcm.BcmModel.ParagraphUnit paragraphUnit in file.ParagraphUnits)
				{
					ConvertParagraphUnit(paragraphUnit, converter, documentProperties, fileProperties, list);
				}
			}
			return list.ToArray();
		}

		public TranslationUnit[] ConvertToTranslationUnits(Fragment fragment)
		{
			File file = fragment.GetFile();
			BcmToBilingualConverter converter = new BcmToBilingualConverter(file, null);
			IDocumentProperties documentProperties = CreateDocumentProperties(fragment.SourceLanguageCode, fragment.TargetLanguageCode);
			IFileProperties fileProperties = CreateFileProperties(file);
			Sdl.Core.Bcm.BcmModel.ParagraphUnit bcmPunit = fragment.CreateParagraphUnit();
			List<TranslationUnit> list = new List<TranslationUnit>();
			ConvertParagraphUnit(bcmPunit, converter, documentProperties, fileProperties, list);
			return list.ToArray();
		}

		private void ConvertParagraphUnit(Sdl.Core.Bcm.BcmModel.ParagraphUnit bcmPunit, BcmToBilingualConverter converter, IDocumentProperties documentProperties, IFileProperties fileProperties, List<TranslationUnit> result)
		{
			bcmPunit.Source.NormalizeTextItems();
			bcmPunit.Target?.NormalizeTextItems();
			IParagraphUnit paragraphUnit = converter.Convert(bcmPunit);
			TranslationUnit[] array = CreateTranslationUnit(paragraphUnit, documentProperties, fileProperties).ToArray();
			result.AddRange(array);
			int num = 0;
			foreach (Sdl.Core.Bcm.BcmModel.SegmentPair segmentPair in bcmPunit.SegmentPairs)
			{
				if (num >= array.Length)
				{
					break;
				}
				OnTranslationUnitCreated(new BcmTuConversionEventArgs(segmentPair, array[num]));
				num++;
			}
		}

		protected virtual void OnTranslationUnitCreated(BcmTuConversionEventArgs e)
		{
			this.TranslationUnitCreated?.Invoke(this, e);
		}

		private static IFileProperties CreateFileProperties(File file)
		{
			FileProperties fileProperties = new FileProperties
			{
				FileConversionProperties = new PersistentFileConversionProperties
				{
					FileId = new FileId(file.Id),
					OriginalEncoding = new Codepage(file.OriginalEncoding),
					OriginalFilePath = file.OriginalFileName,
					FileTypeDefinitionId = new FileTypeDefinitionId(file.FileTypeDefinitionId)
				}
			};
			DependencyFileConverter dependencyFileConverter = new DependencyFileConverter();
			foreach (DependencyFile dependencyFile in file.DependencyFiles)
			{
				fileProperties.FileConversionProperties.DependencyFiles.Add(dependencyFileConverter.Convert(dependencyFile));
			}
			return fileProperties;
		}

		private static IDocumentProperties CreateDocumentProperties(Document document)
		{
			return CreateDocumentProperties(document.SourceLanguageCode, document.TargetLanguageCode);
		}

		private static IDocumentProperties CreateDocumentProperties(string sourceLanguageCode, string targetLanguageCode)
		{
			return new DocumentProperties
			{
				SourceLanguage = new Language(sourceLanguageCode),
				TargetLanguage = new Language(targetLanguageCode)
			};
		}

		private IEnumerable<TranslationUnit> CreateTranslationUnit(IParagraphUnit paragraphUnit, IDocumentProperties documentProperties, IFileProperties fileProperties)
		{
			return paragraphUnit.SegmentPairs.Select(delegate(ISegmentPair segmentPair)
			{
				Guid guid = Guid.Empty;
				int id = 0;
				if (segmentPair.Source.Properties.TranslationOrigin != null && segmentPair.Source.Properties.TranslationOrigin.HasMetaData)
				{
					id = int.Parse(segmentPair.Source.Properties.TranslationOrigin.GetMetaData("tuid") ?? "0");
					guid = new Guid(segmentPair.Source.Properties.TranslationOrigin.GetMetaData("tuguid") ?? Guid.Empty.ToString());
				}
				return new TranslationUnit
				{
					ResourceId = new PersistentObjectToken(id, guid),
					DocumentSegmentPair = new Sdl.FileTypeSupport.Framework.Bilingual.SegmentPair(segmentPair.Source, segmentPair.Target),
					DocumentProperties = documentProperties,
					FileProperties = fileProperties
				};
			});
		}
	}
}
