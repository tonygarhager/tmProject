using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ITagPair : IAbstractTag, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable
	{
		IStartTagProperties StartTagProperties
		{
			get;
			set;
		}

		IRevisionProperties StartTagRevisionProperties
		{
			get;
			set;
		}

		bool IsStartTagGhost
		{
			get;
			set;
		}

		IEndTagProperties EndTagProperties
		{
			get;
			set;
		}

		IRevisionProperties EndTagRevisionProperties
		{
			get;
			set;
		}

		bool IsEndTagGhost
		{
			get;
			set;
		}
	}
}
