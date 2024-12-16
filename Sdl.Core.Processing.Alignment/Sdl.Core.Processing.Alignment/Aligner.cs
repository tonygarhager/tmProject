using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Core.Algorithms;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment
{
	internal class Aligner
	{
		private readonly IAlignmentAlgorithm _algorithm;

		private readonly List<AlignmentData> _alignments;

		private int _leftSegmentCounter;

		private int _rightSegmentCounter;

		public bool CancelProcessing
		{
			get
			{
				return _algorithm.CancelProcessing;
			}
			set
			{
				_algorithm.CancelProcessing = value;
			}
		}

		public List<AlignmentData> Alignments => _alignments;

		public IAlignmentAlgorithm Algorithm => _algorithm;

		protected int LeftIndexOffset
		{
			get;
			set;
		}

		protected int RightIndexOffset
		{
			get;
			set;
		}

		public event EventHandler<ProgressEventArgs> OnCurrentParagraphUnitProgress;

		public Aligner(IAlignmentAlgorithm algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			_algorithm = algorithm;
			_alignments = new List<AlignmentData>();
			LeftIndexOffset = 0;
			RightIndexOffset = 0;
			_leftSegmentCounter = 0;
			_rightSegmentCounter = 0;
		}

		public virtual void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			IList<AlignmentElement> sourceElements = GetSourceElements(paragraphUnit, null, null);
			IList<AlignmentElement> targetElements = GetTargetElements(paragraphUnit, null, null);
			if (this.OnCurrentParagraphUnitProgress != null)
			{
				_algorithm.OnProgress += this.OnCurrentParagraphUnitProgress;
			}
			try
			{
				_alignments.AddRange(_algorithm.Align(sourceElements, targetElements));
			}
			finally
			{
				if (this.OnCurrentParagraphUnitProgress != null)
				{
					_algorithm.OnProgress -= this.OnCurrentParagraphUnitProgress;
				}
			}
		}

		protected void InvokeProgressHandler(ProgressEventArgs args)
		{
			if (this.OnCurrentParagraphUnitProgress != null)
			{
				this.OnCurrentParagraphUnitProgress(this, args);
			}
		}

		protected IList<AlignmentElement> GetSourceElements(IParagraphUnit paragraphUnit, IEnumerable<SegmentId> segmentsToPick, IEnumerable<SegmentId> segmentsToIgnore, bool isRetrofit = false)
		{
			return GetSegments(paragraphUnit.Source, segmentsToPick, segmentsToIgnore, _algorithm.SourceCulture, ref _leftSegmentCounter, isRetrofit);
		}

		protected IList<AlignmentElement> GetTargetElements(IParagraphUnit paragraphUnit, IEnumerable<SegmentId> segmentsToPick, IEnumerable<SegmentId> segmentsToIgnore, bool isRetrofit = false)
		{
			return GetSegments(paragraphUnit.Target, segmentsToPick, segmentsToIgnore, _algorithm.TargetCulture, ref _rightSegmentCounter, isRetrofit);
		}

		protected static IList<AlignmentElement> GetSegments(IParagraph paragraph, IEnumerable<SegmentId> segmentsToPick, IEnumerable<SegmentId> segmentsToIgnore, CultureInfo culture, ref int segmentCounter, bool isRetrofit)
		{
			List<ISegment> list = new List<ISegment>();
			foreach (IAbstractMarkupData item in paragraph)
			{
				if (!(item is StructureTag))
				{
					SegmentCollectionVisitor segmentCollectionVisitor = new SegmentCollectionVisitor();
					item.AcceptVisitor(segmentCollectionVisitor);
					list.AddRange(segmentCollectionVisitor.Segments);
				}
			}
			return GetSegmentsAsAlignmentElements(list, segmentsToPick, segmentsToIgnore, culture, ref segmentCounter, isRetrofit);
		}

		private static IList<AlignmentElement> GetSegmentsAsAlignmentElements(IEnumerable<ISegment> segments, IEnumerable<SegmentId> segmentsToPick, IEnumerable<SegmentId> segmentsToIgnore, CultureInfo culture, ref int segmentCounter, bool isRetrofit)
		{
			List<AlignmentElement> list = new List<AlignmentElement>();
			foreach (ISegment segment2 in segments)
			{
				if (!IsPaddingSegment(segment2) && HasSegmentText(segment2) && (segmentsToPick == null || segmentsToPick.Contains(segment2.Properties.Id)) && (segmentsToIgnore == null || !segmentsToIgnore.Contains(segment2.Properties.Id)))
				{
					string segmentAsString = GetSegmentAsString(segment2);
					Sdl.LanguagePlatform.Core.Segment segment = LanguagePlatformSegmentGenerator.Convert(segment2, culture);
					AlignmentSegment alignmentSegment = segment2 as AlignmentSegment;
					string documentStructurePath = (alignmentSegment != null) ? alignmentSegment.ContextPath : string.Empty;
					AlignmentElement alignmentElement = new AlignmentElement(segment2.ParentParagraphUnit.Properties.ParagraphUnitId, segment2.Properties.Id, segmentAsString, segment, documentStructurePath, segmentCounter++);
					list.Add(alignmentElement);
					AddTextBetweenSegments(alignmentElement, segment2, isRetrofit);
				}
			}
			return list;
		}

		private static void AddTextBetweenSegments(AlignmentElement element, ISegment segment, bool isRetrofit)
		{
			if (isRetrofit)
			{
				int indexInParent = segment.IndexInParent;
				if (indexInParent > 0 && segment.Parent[indexInParent - 1] is IText)
				{
					element.TextBetweenSegment = ((IText)segment.Parent[indexInParent - 1]).Properties.Text;
				}
			}
		}

		protected static bool IsPaddingSegment(ISegment segment)
		{
			foreach (IAbstractMarkupData item in segment)
			{
				IOtherMarker otherMarker = item as IOtherMarker;
				if (otherMarker != null && otherMarker.MarkerType == "AlignmentReadOnlySegment")
				{
					return true;
				}
			}
			return false;
		}

		private static bool HasSegmentText(ISegment segment)
		{
			return GetSegmentAsString(segment).Any((char c) => !char.IsWhiteSpace(c));
		}

		private static string GetSegmentAsString(ISegment segment)
		{
			TextCollectionVisitor textCollectionVisitor = new TextCollectionVisitor(tagContentAsText: false);
			segment.AcceptVisitor(textCollectionVisitor);
			return textCollectionVisitor.CollectedText;
		}
	}
}
