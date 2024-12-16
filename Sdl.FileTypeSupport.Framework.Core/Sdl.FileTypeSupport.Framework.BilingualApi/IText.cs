using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IText : IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		ITextProperties Properties
		{
			get;
			set;
		}

		IText Split(int fromIndex);
	}
}
