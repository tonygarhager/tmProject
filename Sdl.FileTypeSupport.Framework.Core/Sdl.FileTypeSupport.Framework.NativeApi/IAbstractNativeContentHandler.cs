using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IAbstractNativeContentHandler
	{
		void StructureTag(IStructureTagProperties tagInfo);

		void InlineStartTag(IStartTagProperties tagInfo);

		void InlineEndTag(IEndTagProperties tagInfo);

		void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo);

		void Text(ITextProperties textInfo);

		void ChangeContext(IContextProperties newContexts);

		void CustomInfo(ICustomInfoProperties info);

		void LocationMark(LocationMarkerId markerId);

		void LockedContentStart(ILockedContentProperties lockedContentInfo);

		void LockedContentEnd();

		void RevisionStart(IRevisionProperties revisionInfo);

		void RevisionEnd();

		void CommentStart(ICommentProperties commentInfo);

		void CommentEnd();

		void ParagraphComments(ICommentProperties commentInfo);
	}
}
