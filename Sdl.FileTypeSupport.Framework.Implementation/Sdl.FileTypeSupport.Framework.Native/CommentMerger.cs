using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class CommentMerger : INativeGenerationContentProcessor, INativeGenerationContentHandler, IAbstractNativeContentHandler, INativeContentCycleAware
	{
		private class CommentItem
		{
			public ICommentProperties CommentInfo
			{
				get;
				private set;
			}

			public bool IsStart
			{
				get;
				private set;
			}

			public bool IsEnd => !IsStart;

			public int OutputBufferIndex
			{
				get;
				private set;
			}

			public CommentItem(ICommentProperties commentInfo, bool isStart, int outputBufferIndex)
			{
				CommentInfo = commentInfo;
				IsStart = isStart;
				OutputBufferIndex = outputBufferIndex;
			}
		}

		private readonly NativeBuffer _outputBuffer = new NativeBuffer();

		private readonly Stack<ICommentProperties> _commentStack = new Stack<ICommentProperties>();

		private readonly List<CommentItem> _commentBuffer = new List<CommentItem>();

		public INativeGenerationContentHandler Output
		{
			get
			{
				return _outputBuffer.GenerationOutput;
			}
			set
			{
				_outputBuffer.GenerationOutput = value;
			}
		}

		public void StructureTag(IStructureTagProperties tagInfo)
		{
			_outputBuffer.StructureTag(tagInfo);
		}

		public void InlineStartTag(IStartTagProperties tagInfo)
		{
			_outputBuffer.InlineStartTag(tagInfo);
		}

		public void InlineEndTag(IEndTagProperties tagInfo)
		{
			_outputBuffer.InlineEndTag(tagInfo);
		}

		public void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			_outputBuffer.InlinePlaceholderTag(tagInfo);
		}

		public void Text(ITextProperties textInfo)
		{
			_outputBuffer.Text(textInfo);
		}

		public void ChangeContext(IContextProperties newContexts)
		{
			_outputBuffer.ChangeContext(newContexts);
		}

		public void CustomInfo(ICustomInfoProperties info)
		{
			_outputBuffer.CustomInfo(info);
		}

		public void LocationMark(LocationMarkerId markerId)
		{
			_outputBuffer.LocationMark(markerId);
		}

		public void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			_outputBuffer.LockedContentStart(lockedContentInfo);
		}

		public void LockedContentEnd()
		{
			_outputBuffer.LockedContentEnd();
		}

		public void RevisionStart(IRevisionProperties revisionInfo)
		{
			_outputBuffer.RevisionStart(revisionInfo);
		}

		public void RevisionEnd()
		{
			_outputBuffer.RevisionEnd();
		}

		public void CommentStart(ICommentProperties commentInfo)
		{
			_outputBuffer.CommentStart(commentInfo);
			_commentStack.Push(commentInfo);
			int outputBufferIndex = _outputBuffer.BufferedCalls.Count - 1;
			_commentBuffer.Add(new CommentItem(commentInfo, isStart: true, outputBufferIndex));
		}

		public void CommentEnd()
		{
			_outputBuffer.CommentEnd();
			ICommentProperties commentInfo = _commentStack.Pop();
			int outputBufferIndex = _outputBuffer.BufferedCalls.Count - 1;
			_commentBuffer.Add(new CommentItem(commentInfo, isStart: false, outputBufferIndex));
		}

		public void ParagraphComments(ICommentProperties commentInfo)
		{
			_outputBuffer.ParagraphComments(commentInfo);
		}

		public void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
			_outputBuffer.ParagraphUnitStart(properties);
		}

		public void ParagraphUnitEnd()
		{
			_outputBuffer.ParagraphUnitEnd();
			MergeComments();
			_outputBuffer.Release();
			_outputBuffer.Hold();
		}

		public void SegmentStart(ISegmentPairProperties properties)
		{
			_outputBuffer.SegmentStart(properties);
		}

		public void SegmentEnd()
		{
			_outputBuffer.SegmentEnd();
		}

		public void SetFileProperties(IFileProperties properties)
		{
		}

		public void StartOfInput()
		{
			_commentStack.Clear();
			_outputBuffer.Hold();
		}

		public void EndOfInput()
		{
			MergeComments();
			_outputBuffer.Release();
		}

		private void MergeComments()
		{
			DeleteOutputBufferItems(GetOutputBufferIndexesToDeleteToMergeComments());
			_commentBuffer.Clear();
		}

		private IEnumerable<int> GetOutputBufferIndexesToDeleteToMergeComments()
		{
			List<int> list = new List<int>();
			foreach (ICommentProperties mergeableComment in GetMergeableComments())
			{
				list.AddRange(GetOutputBufferIndexesToDeleteToMergeComments(mergeableComment));
			}
			return list;
		}

		private IEnumerable<ICommentProperties> GetMergeableComments()
		{
			return (from item in _commentBuffer
				select item.CommentInfo into item
				where item.Count < 2
				select item).Distinct();
		}

		private IEnumerable<int> GetOutputBufferIndexesToDeleteToMergeComments(ICommentProperties commentInfo)
		{
			IList<int> list = new List<int>();
			CommentItem commentItem = null;
			foreach (CommentItem commentItem2 in GetCommentItems(commentInfo))
			{
				if (commentItem != null && commentItem.IsEnd && commentItem2.IsStart)
				{
					list.Add(commentItem.OutputBufferIndex);
					list.Add(commentItem2.OutputBufferIndex);
				}
				commentItem = commentItem2;
			}
			return list;
		}

		private IEnumerable<CommentItem> GetCommentItems(ICommentProperties commentInfo)
		{
			return _commentBuffer.Where((CommentItem item) => item.CommentInfo.Equals(commentInfo));
		}

		private void DeleteOutputBufferItems(IEnumerable<int> outputBufferIndexesToDelete)
		{
			if (outputBufferIndexesToDelete.Count() == 0)
			{
				return;
			}
			List<int> list = new List<int>();
			foreach (int item in outputBufferIndexesToDelete.OrderByDescending((int i) => i))
			{
				list.Add(item);
			}
			Stack<bool> stack = new Stack<bool>();
			bool flag = false;
			for (int j = 0; j <= list.Count - 2; j += 2)
			{
				for (int k = list[j + 1]; k < list[j]; k++)
				{
					if (_outputBuffer.BufferedCalls[k] is InlineEndTagContentItem)
					{
						if (stack.Count > 0 && stack.Peek())
						{
							stack.Pop();
						}
						else
						{
							stack.Push(item: false);
						}
					}
					if (_outputBuffer.BufferedCalls[k] is InlineStartTagContentItem)
					{
						stack.Push(item: true);
					}
				}
				if (stack.Count > 0)
				{
					flag = true;
					stack.Clear();
					break;
				}
			}
			for (int l = 1; l < list.Count - 2; l += 2)
			{
				for (int m = list[l + 1]; m < list[l]; m++)
				{
					if (_outputBuffer.BufferedCalls[m] is InlineEndTagContentItem)
					{
						stack.Push(item: false);
					}
				}
				if (stack.Count > 0 && (stack.Count % 2 > 0 || !stack.Peek()))
				{
					flag = true;
					stack.Clear();
					break;
				}
			}
			if (!flag)
			{
				foreach (int item2 in outputBufferIndexesToDelete.OrderByDescending((int i) => i))
				{
					_outputBuffer.BufferedCalls.RemoveAt(item2);
				}
			}
		}
	}
}
