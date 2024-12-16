using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ILockedContainer : IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, ICloneable
	{
		ILockedContent LockedContent
		{
			get;
			set;
		}
	}
}
