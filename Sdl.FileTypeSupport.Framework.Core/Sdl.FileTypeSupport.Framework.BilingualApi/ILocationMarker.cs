using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ILocationMarker : IAbstractMarker, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		LocationMarkerId MarkerId
		{
			get;
			set;
		}
	}
}
