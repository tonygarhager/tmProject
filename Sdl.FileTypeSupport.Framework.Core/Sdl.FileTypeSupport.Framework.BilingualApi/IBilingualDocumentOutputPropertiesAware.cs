using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualDocumentOutputPropertiesAware
	{
		void GetProposedFileInfo(IDocumentProperties documentInfo, IOutputFileInfo proposedFileInfo);

		void SetOutputProperties(IBilingualDocumentOutputProperties outputProperties);
	}
}
