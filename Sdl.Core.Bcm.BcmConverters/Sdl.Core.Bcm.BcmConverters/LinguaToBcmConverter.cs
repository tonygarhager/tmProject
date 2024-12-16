using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.Extensions;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Alignment;
using Sdl.Core.Bcm.BcmModel.Operations;
using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class LinguaToBcmConverter
	{
		private readonly Document _document;

		private readonly File _currentFile;

		private ParagraphUnit _currentPunit;

		private int _currentParagraphUnitIndex;

		private int _currentSegmentNumber;

		public event EventHandler<TuConversionEventArgs> TranslationUnitConverted;

		public LinguaToBcmConverter()
		{
			_document = new Document();
			_currentFile = new File();
			_document.Files.Add(_currentFile);
		}

		public Document ConvertToDocument(TranslationUnit[] inputTransUnits)
		{
			return ConvertToDocument(inputTransUnits, includeTokens: false, includeAlignmentData: false, includeUserNameSystemFields: false);
		}

		public Document ConvertToDocument(TranslationUnit[] inputTransUnits, bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields)
		{
			SetBcmDocumentProperties(inputTransUnits);
			ConvertTransUnits(inputTransUnits, includeTokens, includeAlignmentData, includeUserNameSystemFields);
			return _document;
		}

		public Fragment ConvertToFragment(TranslationUnit[] inputTransUnits, bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields)
		{
			SetBcmDocumentProperties(inputTransUnits);
			ConvertTransUnitsInSingleParagraphUnit(inputTransUnits, includeTokens, includeAlignmentData, includeUserNameSystemFields);
			Fragment fragment = new Fragment();
			fragment.ExtractFromDocument(_document, inputTransUnits.Length == 1);
			return fragment;
		}

		protected virtual void OnTranslationUnitConverted(TuConversionEventArgs e)
		{
			this.TranslationUnitConverted?.Invoke(this, e);
		}

		private void SetBcmDocumentProperties(TranslationUnit[] inputTransUnits)
		{
			_document.Name = Guid.NewGuid().ToString();
			TranslationUnit translationUnit = inputTransUnits.FirstOrDefault((TranslationUnit x) => x.SourceSegment.Culture != null);
			TranslationUnit translationUnit2 = inputTransUnits.FirstOrDefault((TranslationUnit x) => x.TargetSegment?.Culture != null);
			if (translationUnit != null)
			{
				Language language = new Language(translationUnit.SourceSegment.Culture);
				_document.SourceLanguageCode = language.IsoAbbreviation;
				_document.SourceLanguageName = language.DisplayName;
			}
			if (translationUnit2 != null)
			{
				Language language2 = new Language(translationUnit2.TargetSegment.Culture);
				_document.TargetLanguageCode = language2.IsoAbbreviation;
				_document.TargetLanguageName = language2.DisplayName;
			}
		}

		private void ConvertTransUnits(IEnumerable<TranslationUnit> inputTransUnits, bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields)
		{
			foreach (TranslationUnit inputTransUnit in inputTransUnits)
			{
				_currentPunit = new ParagraphUnit
				{
					Index = ++_currentParagraphUnitIndex,
					Source = new Paragraph(),
					Target = new Paragraph()
				};
				ConvertTransUnit(includeTokens, includeAlignmentData, includeUserNameSystemFields, inputTransUnit);
			}
		}

		private void ConvertTransUnitsInSingleParagraphUnit(IEnumerable<TranslationUnit> inputTransUnits, bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields)
		{
			_currentPunit = new ParagraphUnit
			{
				Index = ++_currentParagraphUnitIndex,
				Source = new Paragraph(),
				Target = new Paragraph()
			};
			foreach (TranslationUnit inputTransUnit in inputTransUnits)
			{
				ConvertTransUnit(includeTokens, includeAlignmentData, includeUserNameSystemFields, inputTransUnit);
			}
		}

		private void ConvertTransUnit(bool includeTokens, bool includeAlignmentData, bool includeUserNameSystemFields, TranslationUnit tu)
		{
			Sdl.Core.Bcm.BcmModel.Segment segment = new Sdl.Core.Bcm.BcmModel.Segment
			{
				ConfirmationLevel = MarkupDataConverter.Convert(tu.ConfirmationLevel),
				SegmentNumber = Convert.ToString(++_currentSegmentNumber)
			};
			Sdl.Core.Bcm.BcmModel.Segment segment2 = segment.Clone();
			AlignmentData alignmentData = null;
			if (includeAlignmentData && tu.AlignmentData != null)
			{
				alignmentData = CreateAlignmentData(tu);
			}
			UpdateBcmSegment(segment, tu.SourceSegment.Elements, tu.SourceSegment.Tokens, includeTokens, includeAlignment: false, null);
			if (tu.TargetSegment != null)
			{
				UpdateBcmSegment(segment2, tu.TargetSegment.Elements, tu.TargetSegment.Tokens, includeTokens, includeAlignmentData, alignmentData);
				_currentPunit.Target.Add(segment2);
			}
			segment2.CopyMetadataFromLinguaTu(tu, includeUserNameSystemFields);
			_currentPunit.Source.Add(segment);
			_currentFile.ParagraphUnits.Add(_currentPunit);
			OnTranslationUnitConverted(new TuConversionEventArgs(tu, _currentPunit));
		}

		private void UpdateBcmSegment(Sdl.Core.Bcm.BcmModel.Segment bcmSegment, List<SegmentElement> linguaElements, List<Token> linguaTokens, bool includeTokens, bool includeAlignment, AlignmentData alignmentData)
		{
			SegmentElementVisitor visitor = new SegmentElementVisitor(bcmSegment, _currentFile.Skeleton);
			foreach (SegmentElement linguaElement in linguaElements)
			{
				linguaElement?.AcceptSegmentElementVisitor(visitor);
			}
			bcmSegment.NormalizeTextItems();
			if (linguaTokens != null && includeTokens)
			{
				SegmentTokenVisitor visitor2 = new SegmentTokenVisitor(bcmSegment);
				foreach (Token linguaToken in linguaTokens)
				{
					linguaToken?.AcceptSegmentElementVisitor(visitor2);
				}
			}
			if (alignmentData != null && includeAlignment)
			{
				bcmSegment.AlignmentData = alignmentData;
			}
		}

		private AlignmentData CreateAlignmentData(TranslationUnit tu)
		{
			AlignmentData alignmentData = new AlignmentData
			{
				ContentInsertDate = tu.InsertDate.GetValueOrDefault(),
				ModelDate = tu.AlignModelDate.GetValueOrDefault(),
				TuId = tu.ResourceId.Id,
				SpanPairSet = new List<SpanPairNode>(),
				IncompatibleSpanPairSet = new List<SpanPairNode>()
			};
			LiftAlignedSpanPairSet alignmentData2 = tu.AlignmentData;
			if (!alignmentData2.IsEmpty)
			{
				List<LiftAlignedSpanPair> allAlignedSpanPairs = alignmentData2.GetAllAlignedSpanPairs(includeIncompatible: false);
				foreach (LiftAlignedSpanPair item3 in allAlignedSpanPairs)
				{
					SpanPairNode item = new SpanPairNode
					{
						SourceSpan = new Sdl.Core.Bcm.BcmModel.Alignment.LiftSpan
						{
							StartIndex = item3.SourceSpan.StartIndex,
							Length = item3.SourceSpan.Length
						},
						TargetSpan = new Sdl.Core.Bcm.BcmModel.Alignment.LiftSpan
						{
							StartIndex = item3.TargetSpan.StartIndex,
							Length = item3.TargetSpan.Length
						},
						Confidence = item3.Confidence,
						Provenance = item3.Provenance
					};
					alignmentData.SpanPairSet.Add(item);
				}
				List<LiftAlignedSpanPair> incompatibleAlignedSpanPairs = alignmentData2.GetIncompatibleAlignedSpanPairs();
				{
					foreach (LiftAlignedSpanPair item4 in incompatibleAlignedSpanPairs)
					{
						SpanPairNode item2 = new SpanPairNode
						{
							SourceSpan = new Sdl.Core.Bcm.BcmModel.Alignment.LiftSpan
							{
								StartIndex = item4.SourceSpan.StartIndex,
								Length = item4.SourceSpan.Length
							},
							TargetSpan = new Sdl.Core.Bcm.BcmModel.Alignment.LiftSpan
							{
								StartIndex = item4.TargetSpan.StartIndex,
								Length = item4.TargetSpan.Length
							},
							Confidence = item4.Confidence,
							Provenance = item4.Provenance
						};
						alignmentData.IncompatibleSpanPairSet.Add(item2);
					}
					return alignmentData;
				}
			}
			return alignmentData;
		}
	}
}
