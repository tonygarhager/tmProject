using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IRevisionMarker : IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, IAbstractMarker, IAbstractMarkupData, ICloneable
	{
		IRevisionProperties Properties
		{
			get;
			set;
		}
	}
}
