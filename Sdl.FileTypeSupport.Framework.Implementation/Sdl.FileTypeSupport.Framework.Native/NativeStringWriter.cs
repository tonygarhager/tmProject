using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class NativeStringWriter : INativeGenerationContentHandler, IAbstractNativeContentHandler
	{
		private StringBuilder _Output = new StringBuilder();

		public string Output => _Output.ToString();

		public void StructureTag(IStructureTagProperties tagInfo)
		{
			if (tagInfo == null)
			{
				throw new ArgumentNullException("startTagInfo");
			}
			_Output.Append(tagInfo.TagContent);
		}

		public void InlineStartTag(IStartTagProperties tagInfo)
		{
			if (tagInfo == null)
			{
				throw new ArgumentNullException("startTagInfo");
			}
			_Output.Append(tagInfo.TagContent);
		}

		public void InlineEndTag(IEndTagProperties tagInfo)
		{
			if (tagInfo == null)
			{
				throw new ArgumentNullException("startTagInfo");
			}
			_Output.Append(tagInfo.TagContent);
		}

		public void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			if (tagInfo == null)
			{
				throw new ArgumentNullException("startTagInfo");
			}
			_Output.Append(tagInfo.TagContent);
		}

		public void Text(ITextProperties textInfo)
		{
			if (textInfo == null)
			{
				throw new ArgumentNullException("textInfo");
			}
			_Output.Append(textInfo.Text);
		}

		public void CustomInfo(ICustomInfoProperties info)
		{
		}

		public void ChangeContext(IContextProperties contexts)
		{
		}

		public void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
		}

		public void LockedContentEnd()
		{
		}

		public void LocationMark(LocationMarkerId markerId)
		{
		}

		public void RevisionStart(IRevisionProperties revisionInfo)
		{
		}

		public void RevisionEnd()
		{
		}

		public void CommentStart(ICommentProperties commentInfo)
		{
		}

		public void CommentEnd()
		{
		}

		public void ParagraphComments(ICommentProperties commentInfo)
		{
		}

		public void ParagraphUnitStart(IParagraphUnitProperties properties)
		{
		}

		public void ParagraphUnitEnd()
		{
		}

		public void SegmentStart(ISegmentPairProperties properties)
		{
		}

		public void SegmentEnd()
		{
		}
	}
}
