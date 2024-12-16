using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class LanguagePlatformSegmentGenerator : IMarkupDataVisitor
	{
		private Segment _Result;

		private int _NextAnchor;

		private bool _HasTrackChanges;

		private List<KeyValuePair<Tag, IAbstractTag>> _tagAssociations;

		public bool HasTrackChanges => _HasTrackChanges;

		public static Segment Convert(ISegment bcmSegment, CultureInfo culture)
		{
			Segment result = new Segment(culture);
			LanguagePlatformSegmentGenerator languagePlatformSegmentGenerator = new LanguagePlatformSegmentGenerator(result);
			languagePlatformSegmentGenerator.VisitChildNodes(bcmSegment);
			return result;
		}

		private LanguagePlatformSegmentGenerator(Segment result)
		{
			_Result = result;
			_NextAnchor = 0;
			_tagAssociations = new List<KeyValuePair<Tag, IAbstractTag>>();
		}

		public void VisitChildNodes(IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildNodes(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChildNodes(lockedContent.Content);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildNodes(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
			_NextAnchor++;
			Tag tag = new AlignmentTag(TagType.Standalone, placeholderTag.TagProperties.TagId.Id, _NextAnchor, placeholderTag);
			if (placeholderTag.Properties.HasTextEquivalent)
			{
				tag.Type = TagType.TextPlaceholder;
				tag.TextEquivalent = placeholderTag.Properties.TextEquivalent;
			}
			RecordTag(tag, placeholderTag);
		}

		private void RecordTag(Tag tag, IAbstractTag bcmTag)
		{
			_Result.Add(tag);
			_tagAssociations.Add(new KeyValuePair<Tag, IAbstractTag>(tag, bcmTag));
		}

		public void VisitSegment(ISegment segment)
		{
			throw new InvalidOperationException("Nested segments - unexpected");
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			_NextAnchor++;
			int nextAnchor = _NextAnchor;
			string id = tagPair.StartTagProperties.TagId.Id;
			Tag tag = new AlignmentTag(TagType.Start, id, nextAnchor, tagPair);
			RecordTag(tag, tagPair);
			VisitChildNodes(tagPair);
			Tag tag2 = new AlignmentTag(TagType.End, id, nextAnchor, tagPair);
			RecordTag(tag2, tagPair);
		}

		public void VisitText(IText text)
		{
			_Result.Add(text.Properties.Text);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			_HasTrackChanges = true;
			if (revisionMarker.Properties.RevisionType == RevisionType.Insert)
			{
				VisitChildNodes(revisionMarker);
			}
			if (revisionMarker.Properties.RevisionType == RevisionType.Unchanged)
			{
				VisitChildNodes(revisionMarker);
			}
		}
	}
}
