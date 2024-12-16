using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeFileParser : INativeFileTypeComponent, IParser, IDisposable
	{
		INativeExtractionContentHandler Output
		{
			get;
			set;
		}
	}
}
