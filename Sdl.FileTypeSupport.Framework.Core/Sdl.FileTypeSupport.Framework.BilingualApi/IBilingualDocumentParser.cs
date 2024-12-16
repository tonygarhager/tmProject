using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualDocumentParser : IBilingualParser, IParser, IDisposable, IBilingualFileTypeComponent
	{
		DependencyFileLocator DependencyFileLocator
		{
			get;
			set;
		}

		Predicate<IPersistentFileConversionProperties> FileRestriction
		{
			get;
			set;
		}
	}
}
