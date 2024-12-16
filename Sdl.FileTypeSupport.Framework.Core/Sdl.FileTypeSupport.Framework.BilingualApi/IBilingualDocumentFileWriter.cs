using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualDocumentFileWriter : IBilingualDocumentWriter, IBilingualWriter, IBilingualContentHandler, IDisposable, IBilingualDocumentOutputPropertiesAware
	{
	}
}
