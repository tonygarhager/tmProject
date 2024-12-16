using Sdl.Core.Bcm.BcmModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.Extensions
{
	public static class FragmentExtensions
	{
		private const string FragmentSegmentPairContentMessage = "Source or target content has to be segment or paragraph!";

		internal static ParagraphUnit CreateParagraphUnit(this Fragment fragment)
		{
			Paragraph paragraph = fragment.SourceContent as Paragraph;
			Paragraph paragraph2 = fragment.TargetContent as Paragraph;
			ParagraphUnit paragraphUnit;
			if (paragraph == null && paragraph2 == null)
			{
				Segment segment = fragment.SourceContent as Segment;
				Segment segment2 = fragment.TargetContent as Segment;
				if (segment == null && segment2 == null)
				{
					throw new InvalidOperationException("Source or target content has to be segment or paragraph!");
				}
				paragraphUnit = new ParagraphUnit
				{
					Source = new Paragraph()
				};
				if (segment != null)
				{
					paragraphUnit.Source.Add(segment);
				}
				paragraphUnit.Target = new Paragraph();
				if (segment2 != null)
				{
					paragraphUnit.Target.Add(segment2);
				}
			}
			else
			{
				paragraphUnit = new ParagraphUnit
				{
					Source = paragraph,
					Target = paragraph2
				};
			}
			paragraphUnit.ContextList = fragment.ContextList;
			paragraphUnit.CommentDefinitionIds = fragment.CommentDefinitionIds;
			paragraphUnit.StructureContextId = fragment.StructureContextId;
			if (fragment.Metadata != null)
			{
				paragraphUnit.AddMetadataFrom(fragment.Metadata);
			}
			return paragraphUnit;
		}

		internal static void ExtractFromDocument(this Fragment fragment, Document document, bool single)
		{
			File file = document.Files.First();
			ParagraphUnit paragraphUnit = file.ParagraphUnits.First();
			fragment.Skeleton = file.Skeleton;
			fragment.SourceLanguageCode = document.SourceLanguageCode;
			fragment.TargetLanguageCode = document.TargetLanguageCode;
			if (single)
			{
				fragment.SourceContent = (paragraphUnit.Source.Children.First() as Segment);
				fragment.TargetContent = (paragraphUnit.Target.Children.First() as Segment);
			}
			else
			{
				fragment.SourceContent = paragraphUnit.Source;
				fragment.TargetContent = paragraphUnit.Target;
			}
			fragment.CommentDefinitionIds = paragraphUnit.CommentDefinitionIds;
			if (paragraphUnit.Metadata != null)
			{
				fragment.AddMetadataFrom(paragraphUnit.Metadata);
			}
		}

		internal static File GetFile(this Fragment fragment)
		{
			if (fragment.Skeleton == null)
			{
				return new File();
			}
			if (fragment.Skeleton.ParentFile != null)
			{
				return fragment.Skeleton.ParentFile;
			}
			return new File
			{
				Skeleton = fragment.Skeleton
			};
		}

		public static IEnumerable<SegmentPair> GetSegmentPairs(this Fragment fragment, out File file)
		{
			IEnumerable<SegmentPair> segmentPairs = fragment.GetSegmentPairs();
			file = fragment.GetFile();
			return segmentPairs;
		}

		public static IEnumerable<SegmentPair> GetSegmentPairs(this Fragment fragment)
		{
			Paragraph paragraph = fragment.SourceContent as Paragraph;
			Paragraph paragraph2 = fragment.TargetContent as Paragraph;
			if (paragraph == null && paragraph2 == null)
			{
				Segment segment = fragment.SourceContent as Segment;
				Segment segment2 = fragment.TargetContent as Segment;
				if (segment == null && segment2 == null)
				{
					throw new InvalidOperationException("Source or target content has to be segment or paragraph!");
				}
				return new List<SegmentPair>
				{
					new SegmentPair(segment, segment2)
				};
			}
			ParagraphUnit paragraphUnit = new ParagraphUnit
			{
				Source = paragraph,
				Target = paragraph2
			};
			return paragraphUnit.SegmentPairs;
		}
	}
}
