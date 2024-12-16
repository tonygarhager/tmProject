using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IPlaceholderTag : IAbstractTag, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		IPlaceholderTagProperties Properties
		{
			get;
			set;
		}

		IRevisionProperties RevisionProperties
		{
			get;
			set;
		}
	}
}
