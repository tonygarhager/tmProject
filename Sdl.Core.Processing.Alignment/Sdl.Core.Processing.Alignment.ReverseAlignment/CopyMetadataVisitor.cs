using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	public class CopyMetadataVisitor : IMarkupDataVisitor
	{
		private Dictionary<string, IAbstractBasicTagProperties> _startTagProperties = new Dictionary<string, IAbstractBasicTagProperties>();

		private Dictionary<string, IAbstractBasicTagProperties> _endTagProperties = new Dictionary<string, IAbstractBasicTagProperties>();

		public Dictionary<string, IAbstractBasicTagProperties> StartTagProperties
		{
			set
			{
				_startTagProperties = value;
			}
		}

		public Dictionary<string, IAbstractBasicTagProperties> EndTagProperties
		{
			set
			{
				_endTagProperties = value;
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (_startTagProperties.ContainsKey(tagPair.StartTagProperties.TagId.Id))
			{
				string id = tagPair.StartTagProperties.TagId.Id;
				if (tagPair.StartTagProperties != _startTagProperties[id])
				{
					tagPair.StartTagProperties = (IStartTagProperties)_startTagProperties[id].Clone();
					tagPair.EndTagProperties = (IEndTagProperties)_endTagProperties[id].Clone();
				}
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			tag.Properties.ClearMetaData();
			if (_startTagProperties.ContainsKey(tag.Properties.TagId.Id))
			{
				string id = tag.Properties.TagId.Id;
				if (tag.TagProperties != _startTagProperties[id])
				{
					tag.Properties = (IPlaceholderTagProperties)_startTagProperties[id];
				}
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
