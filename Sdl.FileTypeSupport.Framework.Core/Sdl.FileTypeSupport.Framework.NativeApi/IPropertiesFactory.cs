using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IPropertiesFactory
	{
		IFormattingItemFactory FormattingItemFactory
		{
			get;
			set;
		}

		ITextProperties CreateTextProperties(string text);

		IPlaceholderTagProperties CreatePlaceholderTagProperties(string tagContent);

		IStructureTagProperties CreateStructureTagProperties(string tagContent);

		ICustomInfoProperties CreateCustomInfoProperties();

		IStartTagProperties CreateStartTagProperties(string tagContent);

		IEndTagProperties CreateEndTagProperties(string tagContent);

		ISubSegmentProperties CreateSubSegmentProperties(int offset, int length);

		IContextProperties CreateContextProperties();

		IContextInfo CreateContextInfo(string contextType);

		ILockedContentProperties CreateLockedContentProperties(LockTypeFlags lockType);

		IStructureInfo CreateStructureInfo(IContextInfo contextInfo, bool mustUseDisplayName, IStructureInfo parentStructure);

		IStructureInfo CreateStructureInfo();

		IDependencyFileProperties CreateDependencyFileProperties(string currentFilePath);

		ICommentProperties CreateCommentProperties();

		IComment CreateComment(string text, string author, Severity severity);

		IRevisionProperties CreateRevisionProperties(RevisionType type);

		IRevisionProperties CreateFeedbackProperties(RevisionType type);
	}
}
