using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ILockedContent : IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		ILockedContainer Content
		{
			get;
			set;
		}

		ILockedContentProperties Properties
		{
			get;
			set;
		}
	}
}
