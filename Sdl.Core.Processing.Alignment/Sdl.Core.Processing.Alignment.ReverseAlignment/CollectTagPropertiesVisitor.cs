using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	public class CollectTagPropertiesVisitor : IMarkupDataVisitor
	{
		private Dictionary<string, IAbstractBasicTagProperties> _startTagProperties = new Dictionary<string, IAbstractBasicTagProperties>();

		private Dictionary<string, IAbstractBasicTagProperties> _endTagProperties = new Dictionary<string, IAbstractBasicTagProperties>();

		public Dictionary<string, IAbstractBasicTagProperties> StartTagProperties => _startTagProperties;

		public Dictionary<string, IAbstractBasicTagProperties> EndTagProperties => _endTagProperties;

		public void VisitTagPair(ITagPair tagPair)
		{
			if (!_startTagProperties.ContainsKey(tagPair.StartTagProperties.TagId.Id))
			{
				_startTagProperties.Add(tagPair.StartTagProperties.TagId.Id, tagPair.StartTagProperties);
				_endTagProperties.Add(tagPair.StartTagProperties.TagId.Id, tagPair.EndTagProperties);
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (!_startTagProperties.ContainsKey(tag.Properties.TagId.Id))
			{
				_startTagProperties.Add(tag.Properties.TagId.Id, tag.Properties);
			}
		}

		public void VisitText(IText text)
		{
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChildren(lockedContent.Content);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
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
