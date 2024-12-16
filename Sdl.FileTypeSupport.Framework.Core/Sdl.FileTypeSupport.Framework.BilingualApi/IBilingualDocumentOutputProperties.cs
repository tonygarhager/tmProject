using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualDocumentOutputProperties
	{
		string OutputFilePath
		{
			get;
			set;
		}

		IList<IDependencyFileProperties> LinkedDependencyFiles
		{
			get;
		}
	}
}
