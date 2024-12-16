using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public abstract class AbstractNativeExtractionContentProcessor : AbstractNativeFileTypeComponent, INativeExtractionContentProcessor, INativeExtractionContentHandler, IAbstractNativeContentHandler
	{
		private INativeExtractionContentHandler _Output;

		public virtual INativeExtractionContentHandler Output
		{
			get
			{
				return _Output;
			}
			set
			{
				_Output = value;
			}
		}

		public virtual void StructureTag(IStructureTagProperties tagInfo)
		{
			if (Output != null)
			{
				Output.StructureTag(tagInfo);
			}
		}

		public virtual void InlineStartTag(IStartTagProperties tagInfo)
		{
			if (Output != null)
			{
				Output.InlineStartTag(tagInfo);
			}
		}

		public virtual void InlineEndTag(IEndTagProperties tagInfo)
		{
			if (Output != null)
			{
				Output.InlineEndTag(tagInfo);
			}
		}

		public virtual void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			if (Output != null)
			{
				Output.InlinePlaceholderTag(tagInfo);
			}
		}

		public virtual void Text(ITextProperties textInfo)
		{
			if (Output != null)
			{
				Output.Text(textInfo);
			}
		}

		public virtual void CustomInfo(ICustomInfoProperties info)
		{
			if (Output != null)
			{
				Output.CustomInfo(info);
			}
		}

		public virtual void LocationMark(LocationMarkerId markerId)
		{
			if (Output != null)
			{
				Output.LocationMark(markerId);
			}
		}

		public virtual void ChangeContext(IContextProperties contexts)
		{
			if (Output != null)
			{
				Output.ChangeContext(contexts);
			}
		}

		public virtual void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			if (Output != null)
			{
				Output.LockedContentStart(lockedContentInfo);
			}
		}

		public virtual void LockedContentEnd()
		{
			if (Output != null)
			{
				Output.LockedContentEnd();
			}
		}

		public virtual void RevisionStart(IRevisionProperties revisionInfo)
		{
			if (Output != null)
			{
				Output.RevisionStart(revisionInfo);
			}
		}

		public virtual void RevisionEnd()
		{
			if (Output != null)
			{
				Output.RevisionEnd();
			}
		}

		public virtual void CommentStart(ICommentProperties commentInfo)
		{
			if (Output != null)
			{
				Output.CommentStart(commentInfo);
			}
		}

		public virtual void CommentEnd()
		{
			if (Output != null)
			{
				Output.CommentEnd();
			}
		}

		public virtual void ParagraphComments(ICommentProperties commentInfo)
		{
			if (Output != null)
			{
				Output.ParagraphComments(commentInfo);
			}
		}
	}
}
