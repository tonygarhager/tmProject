using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IFileProperties : ICloneable
	{
		IPersistentFileConversionProperties FileConversionProperties
		{
			get;
			set;
		}

		ICommentProperties Comments
		{
			get;
			set;
		}

		bool IsStartOfFileSection();

		bool IsEndOfFileSection();
	}
}
