using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	internal class RemoveSegmentVisitor : IMarkupDataVisitor
	{
		private readonly IDocumentItemFactory ItemFactory = new DocumentItemFactory();

		private readonly IParagraph _paragraph = new Paragraph();

		private IAbstractMarkupDataContainer _currentContainer;

		private readonly Stack<IAbstractMarkupDataContainer> _stack = new Stack<IAbstractMarkupDataContainer>();

		private bool isCollecting;

		private IList<ISegment> _segmentsToMerge;

		public IParagraph NewParagraph => _paragraph;

		public void VisitParagraph(IParagraph paragraph, IList<ISegment> segmentsToMerge)
		{
			_segmentsToMerge = segmentsToMerge;
			_currentContainer = _paragraph;
			VisitChildren(paragraph);
		}

		private void PushContainer(IAbstractMarkupDataContainer newContainer)
		{
			_stack.Push(_currentContainer);
			_currentContainer = newContainer;
		}

		private void PopContainer()
		{
			_currentContainer = _stack.Pop();
		}

		private bool isSegmentInsideContainer(IAbstractMarkupDataContainer container, IAbstractMarkupData segment)
		{
			return container.AllSubItems.Contains(segment);
		}

		private void AddToCurrentContainer(IAbstractMarkupData data, bool forceAdd = false)
		{
			if (isCollecting | forceAdd)
			{
				_currentContainer.Add(data);
			}
		}

		private void VisitContainer(IAbstractMarkupDataContainer newContainer, IAbstractMarkupDataContainer visitContainer)
		{
			PushContainer(newContainer);
			VisitChildren(visitContainer);
			PopContainer();
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			ICommentMarker commentMarker2 = ItemFactory.CreateCommentMarker((ICommentProperties)commentMarker.Comments.Clone());
			AddToCurrentContainer(commentMarker2);
			VisitContainer(commentMarker2, commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			AddToCurrentContainer((ILocationMarker)location.Clone());
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			ILockedContent lockedContent2 = ItemFactory.CreateLockedContent((ILockedContentProperties)lockedContent.Properties.Clone());
			AddToCurrentContainer(lockedContent2);
			VisitContainer(lockedContent2.Content, lockedContent.Content);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			IOtherMarker otherMarker = ItemFactory.CreateOtherMarker();
			AddToCurrentContainer(otherMarker);
			VisitContainer(otherMarker, marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			IPlaceholderTag data = ItemFactory.CreatePlaceholderTag((IPlaceholderTagProperties)tag.Properties.Clone());
			AddToCurrentContainer(data);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			IRevisionMarker revisionMarker2 = ItemFactory.CreateRevision((IRevisionProperties)revisionMarker.Properties.Clone());
			AddToCurrentContainer(revisionMarker2);
			VisitContainer(revisionMarker2, revisionMarker);
		}

		public void VisitSegment(ISegment segment)
		{
			if (segment.Properties.Id == _segmentsToMerge[0].Properties.Id)
			{
				isCollecting = true;
			}
			if (segment.Properties.Id == _segmentsToMerge[_segmentsToMerge.Count - 1].Properties.Id)
			{
				VisitChildren(segment);
				isCollecting = false;
			}
			else if (isCollecting)
			{
				VisitChildren(segment);
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			ITagPair tagPair2 = ItemFactory.CreateTagPair((IStartTagProperties)tagPair.StartTagProperties.Clone(), (IEndTagProperties)tagPair.EndTagProperties.Clone());
			bool forceAdd = tagPair.AllSubItems.Contains(_segmentsToMerge[0]);
			AddToCurrentContainer(tagPair2, forceAdd);
			VisitContainer(tagPair2, tagPair);
		}

		public void VisitText(IText text)
		{
			IText data = ItemFactory.CreateText((ITextProperties)text.Properties.Clone());
			AddToCurrentContainer(data);
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
