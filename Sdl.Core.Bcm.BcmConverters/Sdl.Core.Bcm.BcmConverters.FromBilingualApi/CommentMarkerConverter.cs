using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi
{
	internal class CommentMarkerConverter
	{
		private readonly MarkupDataContainerVisitorData _data;

		private readonly List<MarkupData> _children;

		public CommentMarkerConverter(MarkupDataContainerVisitorData data, List<MarkupData> children)
		{
			_data = data;
			_children = children;
		}

		public void Convert(ICommentMarker commentMarker)
		{
			CommentContainer currentContainer;
			CommentContainer rootComment = ConvertCommentMarker(commentMarker, out currentContainer);
			AddChildren(commentMarker, rootComment, currentContainer);
		}

		private CommentContainer ConvertCommentMarker(ICommentMarker commentMarker, out CommentContainer currentContainer)
		{
			CommentContainer commentContainer = null;
			CommentContainer commentContainer2 = null;
			currentContainer = null;
			foreach (IComment comment in commentMarker.Comments.Comments)
			{
				currentContainer = new CommentContainer
				{
					Id = Guid.NewGuid().ToString()
				};
				CommentDefinition elem = MarkupDataConverter.Convert(comment);
				int id = _data.File.Skeleton.CommentDefinitions.GetOrAdd(elem).Id;
				currentContainer.CommentDefinitionId = id;
				if (commentContainer == null)
				{
					commentContainer = currentContainer;
				}
				if (commentContainer2 == null)
				{
					commentContainer2 = currentContainer;
				}
				else
				{
					commentContainer2.Add(currentContainer);
					commentContainer2 = currentContainer;
				}
			}
			return commentContainer;
		}

		private void AddChildren(ICommentMarker commentMarker, CommentContainer rootComment, CommentContainer leafCommentContainer)
		{
			if (_data.IsInSegment)
			{
				List<MarkupData> range = HandleCommentInsideSegment(commentMarker, rootComment, leafCommentContainer);
				leafCommentContainer.AddRange(range);
				return;
			}
			List<MarkupData> collection = HandleCommentOutsideSegment(commentMarker, rootComment);
			while (_data.BufferedComments.Count > 0)
			{
				_children.Add(_data.BufferedComments.Dequeue());
			}
			_data.BufferedComments.Clear();
			_children.AddRange(collection);
		}

		private List<MarkupData> HandleCommentInsideSegment(ICommentMarker commentMarker, CommentContainer rootComment, CommentContainer leafCommentContainer)
		{
			_children.Add(rootComment);
			if (leafCommentContainer == null)
			{
				return new List<MarkupData>();
			}
			MarkupDataContainerVisitor markupDataContainerVisitor = new MarkupDataContainerVisitor(_data);
			return markupDataContainerVisitor.Visit(commentMarker);
		}

		private List<MarkupData> HandleCommentOutsideSegment(ICommentMarker commentMarker, CommentContainer rootComment)
		{
			_data.BufferedComments.Enqueue(rootComment);
			MarkupDataContainerVisitor markupDataContainerVisitor = new MarkupDataContainerVisitor(_data);
			return markupDataContainerVisitor.Visit(commentMarker);
		}
	}
}
