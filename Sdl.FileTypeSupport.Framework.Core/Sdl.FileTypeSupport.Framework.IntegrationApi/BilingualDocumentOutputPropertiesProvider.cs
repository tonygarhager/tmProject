using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public delegate void BilingualDocumentOutputPropertiesProvider(IBilingualDocumentOutputProperties outputProperties, IDocumentProperties documentInfo, IOutputFileInfo proposedFileInfo);
}
