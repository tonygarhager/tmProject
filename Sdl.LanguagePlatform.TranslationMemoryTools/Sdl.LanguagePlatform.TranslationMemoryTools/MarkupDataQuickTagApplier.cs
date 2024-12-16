using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataQuickTagApplier : IMarkupDataVisitor
	{
		private class TagSetter : IMarkupDataVisitor
		{
			private readonly string _tagId;

			public TagSetter(string tagId)
			{
				_tagId = tagId;
			}

			public void VisitContainer(IAbstractMarkupDataContainer container)
			{
				if (container != null)
				{
					foreach (IAbstractMarkupData allSubItem in container.AllSubItems)
					{
						allSubItem.AcceptVisitor(this);
					}
				}
			}

			public void VisitCommentMarker(ICommentMarker commentMarker)
			{
			}

			public void VisitLocationMarker(ILocationMarker location)
			{
			}

			public void VisitLockedContent(ILockedContent lockedContent)
			{
			}

			public void VisitOtherMarker(IOtherMarker marker)
			{
			}

			public void VisitPlaceholderTag(IPlaceholderTag tag)
			{
				tag.TagProperties.TagId = new TagId(_tagId);
			}

			public void VisitRevisionMarker(IRevisionMarker revisionMarker)
			{
			}

			public void VisitSegment(ISegment segment)
			{
			}

			public void VisitTagPair(ITagPair tagPair)
			{
				tagPair.StartTagProperties.TagId = new TagId(_tagId);
			}

			public void VisitText(IText text)
			{
			}
		}

		private readonly IQuickTags _quickTags;

		private readonly List<IAbstractMarkupData> _matchedList;

		public MarkupDataQuickTagApplier(IQuickTags quickTags)
		{
			_quickTags = quickTags;
			_matchedList = new List<IAbstractMarkupData>();
		}

		public void ApplyQuickTags(List<IAbstractMarkupData> markupDataList)
		{
			_matchedList.Clear();
			foreach (IAbstractMarkupData markupData in markupDataList)
			{
				markupData.AcceptVisitor(this);
			}
			foreach (IAbstractMarkupData matched in _matchedList)
			{
				markupDataList.Remove(matched);
			}
		}

		private IQuickTag GetQuickTag(IAbstractTagProperties tagProperties, bool isTagPair)
		{
			if (_quickTags == null)
			{
				return null;
			}
			return _quickTags.Where((IQuickTag quickTag) => quickTag.CommandId == tagProperties.TagId.Id).FirstOrDefault((IQuickTag quickTag) => (isTagPair && quickTag.MarkupDataContent.ToList().FirstOrDefault() is TagPair) || (!isTagPair && quickTag.MarkupDataContent.ToList().FirstOrDefault() is PlaceholderTag));
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			IQuickTag quickTag = GetQuickTag(tag.TagProperties, isTagPair: false);
			if (quickTag != null)
			{
				IAbstractMarkupDataContainer abstractMarkupDataContainer = quickTag.MarkupDataContent.Clone() as IAbstractMarkupDataContainer;
				new TagSetter(quickTag.CommandId).VisitContainer(abstractMarkupDataContainer);
				abstractMarkupDataContainer.MoveAllItemsTo(tag.Parent, tag.IndexInParent + 1);
				tag.RemoveFromParent();
				_matchedList.Add(tag);
			}
		}

		public void VisitSegment(ISegment segment)
		{
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			IQuickTag quickTag = GetQuickTag(tagPair.TagProperties, isTagPair: true);
			if (quickTag != null)
			{
				IAbstractMarkupDataContainer abstractMarkupDataContainer = quickTag.MarkupDataContent.Clone() as IAbstractMarkupDataContainer;
				new TagSetter(quickTag.CommandId).VisitContainer(abstractMarkupDataContainer);
				IAbstractMarkupData abstractMarkupData = null;
				foreach (IAbstractMarkupData allSubItem in abstractMarkupDataContainer.AllSubItems)
				{
					ILocationMarker locationMarker = allSubItem as ILocationMarker;
					if (locationMarker != null && !(locationMarker.MarkerId.Id != "Content_Goes_Here"))
					{
						abstractMarkupData = allSubItem;
						break;
					}
				}
				if (abstractMarkupData != null)
				{
					tagPair.MoveAllItemsTo(abstractMarkupData.Parent, abstractMarkupData.IndexInParent + 1);
					abstractMarkupData.RemoveFromParent();
					abstractMarkupDataContainer.MoveAllItemsTo(tagPair.Parent, tagPair.IndexInParent + 1);
					tagPair.RemoveFromParent();
					_matchedList.Add(tagPair);
				}
			}
		}

		public void VisitText(IText text)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
		}
	}
}
