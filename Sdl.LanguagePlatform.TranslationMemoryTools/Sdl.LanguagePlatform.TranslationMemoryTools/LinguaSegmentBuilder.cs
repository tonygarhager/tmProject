using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class LinguaSegmentBuilder : IMarkupDataVisitor
	{
		private readonly bool _excludeTagsInLockedContentText;

		private int _nextAnchor;

		private readonly bool _acceptTrackChanges;

		private readonly bool _includeTrackChanges;

		private readonly bool _includeComments;

		private readonly Func<IRevisionMarker, string> _revisionMarkerTagIdFunction;

		public Segment Result
		{
			get;
			set;
		}

		public List<KeyValuePair<Tag, IAbstractMarkupData>> TagAssociations
		{
			get;
		}

		public List<KeyValuePair<Text, IAbstractMarkupData>> TextAssociations
		{
			get;
		}

		public bool IgnoreTags
		{
			get;
			set;
		}

		public bool HasTrackChanges
		{
			get;
			private set;
		}

		public LinguaSegmentBuilder(Segment result, LinguaTuBuilderSettings flags)
		{
			Result = result;
			IgnoreTags = flags.StripTags;
			_excludeTagsInLockedContentText = flags.ExcludeTagsInLockedContentText;
			_acceptTrackChanges = flags.AcceptTrackChanges;
			_includeTrackChanges = flags.IncludeTrackChanges;
			_includeComments = flags.IncludeComments;
			_revisionMarkerTagIdFunction = flags.RevisionMarkerTagIdFunction;
			_nextAnchor = 0;
			TagAssociations = new List<KeyValuePair<Tag, IAbstractMarkupData>>();
			TextAssociations = new List<KeyValuePair<Text, IAbstractMarkupData>>();
		}

		public LinguaSegmentBuilder(Segment result, bool ignoreTags, bool excludeTagsInLockedContentText)
			: this(result, new LinguaTuBuilderSettings
			{
				StripTags = ignoreTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = true
			})
		{
		}

		public LinguaSegmentBuilder(Segment result, bool ignoreTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, bool includeTrackChanges = false)
			: this(result, new LinguaTuBuilderSettings
			{
				StripTags = ignoreTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = acceptTrackChanges,
				IncludeTrackChanges = includeTrackChanges
			})
		{
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
			_nextAnchor++;
			int nextAnchor = _nextAnchor;
			string tagId = $"c{commentMarker.UniqueId}";
			if (_includeComments)
			{
				Tag tag = new Tag(TagType.Start, tagId, nextAnchor, 0, null, canHide: false);
				RecordTag(tag, commentMarker);
			}
			VisitChildNodes(commentMarker);
			if (_includeComments)
			{
				Tag tag2 = new Tag(TagType.End, tagId, nextAnchor, 0, null, canHide: false);
				RecordTag(tag2, commentMarker);
			}
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			if (!IgnoreTags)
			{
				_nextAnchor++;
				Tag tag = new Tag(TagType.LockedContent, _nextAnchor.ToString(), _nextAnchor)
				{
					TextEquivalent = (_excludeTagsInLockedContentText ? TextCollector.CollectText(lockedContent) : lockedContent.Content.ToString())
				};
				RecordTag(tag, null);
			}
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildNodes(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
			if (!IgnoreTags)
			{
				_nextAnchor++;
				Tag tag = new Tag(TagType.Standalone, placeholderTag.TagProperties.TagId.Id, _nextAnchor);
				if (placeholderTag.Properties.HasTextEquivalent)
				{
					tag.Type = TagType.TextPlaceholder;
					tag.TextEquivalent = placeholderTag.Properties.TextEquivalent;
				}
				RecordTag(tag, placeholderTag);
			}
		}

		private void RecordTag(Tag tag, IAbstractMarkupData bcmTag)
		{
			Result.Add(tag);
			TagAssociations.Add(new KeyValuePair<Tag, IAbstractMarkupData>(tag, bcmTag));
		}

		public void VisitSegment(ISegment segment)
		{
			throw new LanguagePlatformException(ErrorCode.UnexpectedDocumentContent);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			_nextAnchor++;
			int nextAnchor = _nextAnchor;
			string id = tagPair.StartTagProperties.TagId.Id;
			if (!IgnoreTags)
			{
				Tag tag = new Tag(TagType.Start, id, nextAnchor, 0, null, tagPair.StartTagProperties.CanHide);
				RecordTag(tag, tagPair);
			}
			VisitChildNodes(tagPair);
			if (!IgnoreTags)
			{
				Tag tag2 = new Tag(TagType.End, id, nextAnchor, 0, null, tagPair.EndTagProperties.CanHide);
				RecordTag(tag2, tagPair);
			}
		}

		public void VisitText(IText bcmText)
		{
			Result.Add(bcmText.Properties.Text);
			Text key = (Text)Result.Elements[Result.Elements.Count - 1];
			TextAssociations.Add(new KeyValuePair<Text, IAbstractMarkupData>(key, bcmText));
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			HasTrackChanges = true;
			if (_includeTrackChanges)
			{
				_nextAnchor++;
				int nextAnchor = _nextAnchor;
				string tagId = revisionMarker.Properties.Author + '|' + revisionMarker.Properties.RevisionType + '|' + (revisionMarker.Properties.Date.HasValue ? revisionMarker.Properties.Date.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
				if (_revisionMarkerTagIdFunction != null)
				{
					string text = _revisionMarkerTagIdFunction(revisionMarker);
					if (!string.IsNullOrEmpty(text))
					{
						tagId = text;
					}
				}
				Tag tag = new Tag(TagType.Start, tagId, nextAnchor);
				RecordTag(tag, revisionMarker);
				VisitChildNodes(revisionMarker);
				Tag tag2 = new Tag(TagType.End, tagId, nextAnchor);
				RecordTag(tag2, revisionMarker);
				return;
			}
			if (_acceptTrackChanges)
			{
				if (revisionMarker.Properties.RevisionType == RevisionType.Insert || revisionMarker.Properties.RevisionType == RevisionType.FeedbackAdded)
				{
					VisitChildNodes(revisionMarker);
				}
			}
			else if (revisionMarker.Properties.RevisionType == RevisionType.Delete || revisionMarker.Properties.RevisionType == RevisionType.FeedbackDeleted)
			{
				VisitChildNodes(revisionMarker);
			}
			if (revisionMarker.Properties.RevisionType == RevisionType.FeedbackComment || revisionMarker.Properties.RevisionType == RevisionType.Unchanged)
			{
				VisitChildNodes(revisionMarker);
			}
		}
	}
}
