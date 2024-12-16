using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualParser : IParser, IDisposable, IBilingualFileTypeComponent
	{
		IDocumentProperties DocumentProperties
		{
			get;
			set;
		}

		IBilingualContentHandler Output
		{
			get;
			set;
		}
	}
}
