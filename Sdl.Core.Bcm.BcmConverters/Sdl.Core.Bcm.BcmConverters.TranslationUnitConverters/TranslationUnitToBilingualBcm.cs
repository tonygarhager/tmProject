using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.Extensions;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi;
using Sdl.Core.Bcm.BcmModel;
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
	internal class TranslationUnitToBilingualBcm : ITranslationUnitToBcm
	{
		private readonly TransUnitConversionSettings _conversionSettings;

		private readonly DocumentItemFactory _documentItemFactory;

		private IFileProperties _fileProperties;

		private IDocumentProperties _documentProperties;

		private TranslationUnit _currentTu;

		public event EventHandler<TuConversionEventArgs> TranslationUnitConverted;

		public TranslationUnitToBilingualBcm(TransUnitConversionSettings conversionSettings)
		{
			_conversionSettings = conversionSettings;
			_documentItemFactory = new DocumentItemFactory();
		}

		public Document ConvertToDocument(TranslationUnit[] inputTransUnits)
		{
			BcmExtractor bcmExtractor = PrepareExtractor(inputTransUnits);
			Array.ForEach(inputTransUnits, delegate(TranslationUnit tu)
			{
				_currentTu = tu;
				IParagraphUnit paragraphUnit = CreateParagraphUnit(tu);
				bcmExtractor.ProcessParagraphUnit(paragraphUnit);
			});
			bcmExtractor.FileComplete();
			bcmExtractor.Complete();
			return bcmExtractor.OutputDocument;
		}

		public Fragment ConvertToFragment(TranslationUnit[] inputTransUnits)
		{
			BcmExtractor bcmExtractor = PrepareExtractor(inputTransUnits);
			IParagraphUnit paragraphUnit = CreateParagraphUnitMultipleTus(inputTransUnits);
			bcmExtractor.ProcessParagraphUnit(paragraphUnit);
			bcmExtractor.FileComplete();
			bcmExtractor.Complete();
			Fragment fragment = new Fragment();
			fragment.ExtractFromDocument(bcmExtractor.OutputDocument, inputTransUnits.Length == 1);
			return fragment;
		}

		private BcmExtractor PrepareExtractor(TranslationUnit[] inputTransUnits)
		{
			StoreFileProperties(inputTransUnits);
			StoreDocumentProperties(inputTransUnits);
			BcmExtractor bcmExtractor = new BcmExtractor
			{
				BcmExtractionSettings = new BcmExtractionSettings
				{
					ProcessComments = _conversionSettings.ProcessComments
				}
			};
			bcmExtractor.ParagraphUnitProcessed += delegate(object sender, ParagraphUnitEventArgs args)
			{
				OnTranslationUnitConverted(new TuConversionEventArgs(_currentTu, args.ParagraphUnit));
			};
			bcmExtractor.Initialize(_documentProperties);
			bcmExtractor.SetFileProperties(_fileProperties);
			return bcmExtractor;
		}

		protected virtual void OnTranslationUnitConverted(TuConversionEventArgs e)
		{
			this.TranslationUnitConverted?.Invoke(this, e);
		}

		private void StoreDocumentProperties(IEnumerable<TranslationUnit> inputTransUnits)
		{
			TranslationUnit translationUnit = inputTransUnits.FirstOrDefault((TranslationUnit x) => x.DocumentProperties != null);
			IDocumentProperties documentProperties;
			if (translationUnit != null)
			{
				documentProperties = translationUnit.DocumentProperties;
			}
			else
			{
				IDocumentProperties documentProperties2 = new DocumentProperties();
				documentProperties = documentProperties2;
			}
			_documentProperties = documentProperties;
		}

		private void StoreFileProperties(IEnumerable<TranslationUnit> inputTransUnits)
		{
			TranslationUnit translationUnit = inputTransUnits.FirstOrDefault((TranslationUnit x) => x.FileProperties != null);
			if (translationUnit != null)
			{
				_fileProperties = translationUnit.FileProperties;
			}
			else
			{
				_fileProperties = new FileProperties
				{
					FileConversionProperties = new PersistentFileConversionProperties
					{
						InputFilePath = Guid.NewGuid().ToString()
					}
				};
			}
		}

		private IParagraphUnit CreateParagraphUnit(TranslationUnit tu)
		{
			IParagraphUnit paragraphUnit = _documentItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
			SetPunitProperties(tu, paragraphUnit);
			ISegmentPair segmentPair = CreateSegmentPair(tu);
			paragraphUnit.Source = new Sdl.FileTypeSupport.Framework.Bilingual.Paragraph
			{
				segmentPair.Source
			};
			paragraphUnit.Target = ((segmentPair.Target != null) ? new Sdl.FileTypeSupport.Framework.Bilingual.Paragraph
			{
				segmentPair.Target
			} : new Sdl.FileTypeSupport.Framework.Bilingual.Paragraph());
			return paragraphUnit;
		}

		private IParagraphUnit CreateParagraphUnitMultipleTus(TranslationUnit[] tus)
		{
			IParagraphUnit paragraphUnit = _documentItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
			SetPunitProperties(tus, paragraphUnit);
			paragraphUnit.Source = new Sdl.FileTypeSupport.Framework.Bilingual.Paragraph();
			paragraphUnit.Target = new Sdl.FileTypeSupport.Framework.Bilingual.Paragraph();
			foreach (TranslationUnit tu in tus)
			{
				ISegmentPair segmentPair = CreateSegmentPair(tu);
				paragraphUnit.Source.Add(segmentPair.Source);
				if (segmentPair.Target != null)
				{
					paragraphUnit.Target.Add(segmentPair.Target);
				}
			}
			return paragraphUnit;
		}

		private ISegmentPair CreateSegmentPair(TranslationUnit tu)
		{
			ISegment segment = tu.DocumentSegmentPair.Source.Clone() as ISegment;
			SetMetaData(segment, tu);
			segment.Parent = null;
			ISegment segment2 = null;
			if (tu.DocumentSegmentPair.Target != null)
			{
				segment2 = (tu.DocumentSegmentPair.Target.Clone() as ISegment);
				SetMetaData(segment2, tu);
				segment2.Parent = null;
			}
			return new Sdl.FileTypeSupport.Framework.Bilingual.SegmentPair(segment, segment2);
		}

		private void SetMetaData(ISegment segment, TranslationUnit tu)
		{
			if (segment.Properties.TranslationOrigin == null)
			{
				segment.Properties.TranslationOrigin = new Sdl.FileTypeSupport.Framework.Native.TranslationOrigin();
			}
			segment.Properties.TranslationOrigin.SetMetaData("tuid", Convert.ToString(tu.ResourceId.Id));
			segment.Properties.TranslationOrigin.SetMetaData("tuguid", Convert.ToString(tu.ResourceId.Guid));
		}

		private static void SetPunitProperties(TranslationUnit tu, IParagraphUnit punit)
		{
			IParagraphUnitProperties paragraphUnitProperties = tu.DocumentSegmentPair.Source.ParentParagraphUnit?.Properties;
			if (paragraphUnitProperties != null)
			{
				punit.Properties = paragraphUnitProperties;
			}
		}

		private static void SetPunitProperties(TranslationUnit[] tus, IParagraphUnit punit)
		{
			ISegmentPair segmentPair = tus.Select((TranslationUnit tu) => tu.DocumentSegmentPair).FirstOrDefault((ISegmentPair sp) => sp.Source.ParentParagraphUnit?.Properties != null);
			if (segmentPair != null)
			{
				punit.Properties = segmentPair.Source.ParentParagraphUnit.Properties;
			}
		}
	}
}
