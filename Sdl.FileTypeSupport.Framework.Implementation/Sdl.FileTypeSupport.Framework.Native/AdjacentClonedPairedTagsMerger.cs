using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.Buffer;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class AdjacentClonedPairedTagsMerger : INativeGenerationContentProcessor, INativeGenerationContentHandler, IAbstractNativeContentHandler, INativeContentCycleAware
	{
		private Stack<MergeableObject> _JustClosedMergeableObjects = new Stack<MergeableObject>();

		private Stack<MergeableObject> _OpenMergeableObjects = new Stack<MergeableObject>();

		private NativeBuffer _OutputBuffer = new NativeBuffer();

		private bool _inStructureParagraph;

		private ParagraphUnitId _currentParagraphId;

		private bool _autoCloneFlagSupported;

		public INativeGenerationContentHandler Output
		{
			get
			{
				return _OutputBuffer.GenerationOutput;
			}
			set
			{
				_OutputBuffer.GenerationOutput = value;
			}
		}

		public void StopHolding()
		{
			foreach (AbstractContentItem bufferedCall in _OutputBuffer.BufferedCalls)
			{
				InlineStartTagContentItem inlineStartTagContentItem = bufferedCall as InlineStartTagContentItem;
				if (inlineStartTagContentItem != null)
				{
					IStartTagProperties properties = inlineStartTagContentItem.Properties;
					if (properties.MetaDataContainsKey("SDL:AutoCloned"))
					{
						properties = (IStartTagProperties)properties.Clone();
						properties.RemoveMetaData("SDL:AutoCloned");
						inlineStartTagContentItem.Properties = properties;
					}
				}
			}
			_OutputBuffer.Release();
			_JustClosedMergeableObjects.Clear();
			_OutputBuffer.Hold();
		}

		public void SetFileProperties(IFileProperties properties)
		{
			_autoCloneFlagSupported = properties.FileConversionProperties.MetaDataContainsKey("SDL:AutoClonedFlagSupported");
		}

		public void StartOfInput()
		{
			StopHolding();
		}

		public void EndOfInput()
		{
			StopHolding();
		}

		public void StructureTag(IStructureTagProperties tagInfo)
		{
			if (tagInfo != null && tagInfo.TagContent != null && tagInfo.TagContent.Contains("#SDL-SUBCONTENT-MARKER#"))
			{
				StopHolding();
			}
			else
			{
				_OutputBuffer.StructureTag(tagInfo);
			}
		}

		public void InlineStartTag(IStartTagProperties tagInfo)
		{
			if (_JustClosedMergeableObjects.Count > 0)
			{
				MergeableObject mergeableObject = _JustClosedMergeableObjects.Peek();
				IStartTagProperties startTagProperties = mergeableObject.StartTagProperties;
				if (startTagProperties != null)
				{
					bool flag = tagInfo == startTagProperties || tagInfo.TagId.Equals(startTagProperties.TagId);
					if (_autoCloneFlagSupported && mergeableObject.ParagraphUnitId != _currentParagraphId && !startTagProperties.MetaDataContainsKey("SDL:AutoCloned"))
					{
						flag = false;
					}
					if (flag)
					{
						bool flag2 = false;
						int num;
						for (num = _OutputBuffer.BufferedCalls.Count - 1; num >= 0; num--)
						{
							if (_OutputBuffer.BufferedCalls[num] is CommentStartContentItem || _OutputBuffer.BufferedCalls[num] is CommentEndContentItem)
							{
								flag2 = true;
								break;
							}
							if (_OutputBuffer.BufferedCalls[num] is InlineEndTagContentItem)
							{
								break;
							}
						}
						if (!flag2)
						{
							_OutputBuffer.BufferedCalls.RemoveAt(num);
							_OpenMergeableObjects.Push(_JustClosedMergeableObjects.Pop());
							return;
						}
					}
				}
			}
			StopHolding();
			_OpenMergeableObjects.Push(new MergeableObject(tagInfo, _currentParagraphId));
			_OutputBuffer.InlineStartTag(tagInfo);
		}

		public void InlineEndTag(IEndTagProperties tagInfo)
		{
			_JustClosedMergeableObjects.Push(_OpenMergeableObjects.Pop());
			_OutputBuffer.Hold();
			_OutputBuffer.InlineEndTag(tagInfo);
		}

		public void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			StopHolding();
			_OutputBuffer.InlinePlaceholderTag(tagInfo);
		}

		public void Text(ITextProperties textInfo)
		{
			if (_inStructureParagraph)
			{
				_OutputBuffer.Text(textInfo);
				return;
			}
			StopHolding();
			_OutputBuffer.Text(textInfo);
		}

		public void ChangeContext(IContextProperties newContexts)
		{
			_OutputBuffer.ChangeContext(newContexts);
		}

		public void CustomInfo(ICustomInfoProperties info)
		{
			_OutputBuffer.CustomInfo(info);
		}

		public void LocationMark(LocationMarkerId markerId)
		{
			_OutputBuffer.LocationMark(markerId);
		}

		public void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			_OutputBuffer.LockedContentStart(lockedContentInfo);
		}

		public void LockedContentEnd()
		{
			_OutputBuffer.LockedContentEnd();
		}

		public void RevisionStart(IRevisionProperties revisionInfo)
		{
			_OutputBuffer.RevisionStart(revisionInfo);
		}

		public void RevisionEnd()
		{
			_OutputBuffer.RevisionEnd();
		}

		public void CommentStart(ICommentProperties commentInfo)
		{
			_OutputBuffer.CommentStart(commentInfo);
		}

		public void CommentEnd()
		{
			_OutputBuffer.CommentEnd();
		}

		public void ParagraphComments(ICommentProperties commentInfo)
		{
			_OutputBuffer.ParagraphComments(commentInfo);
		}

		public void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
			_currentParagraphId = properties.ParagraphUnitId;
			_inStructureParagraph = (properties.LockType == LockTypeFlags.Structure);
			_OutputBuffer.ParagraphUnitStart(properties);
		}

		public void ParagraphUnitEnd()
		{
			_currentParagraphId = default(ParagraphUnitId);
			_OutputBuffer.ParagraphUnitEnd();
		}

		public void SegmentStart(ISegmentPairProperties properties)
		{
			_OutputBuffer.SegmentStart(properties);
		}

		public void SegmentEnd()
		{
			_OutputBuffer.SegmentEnd();
		}
	}
}
