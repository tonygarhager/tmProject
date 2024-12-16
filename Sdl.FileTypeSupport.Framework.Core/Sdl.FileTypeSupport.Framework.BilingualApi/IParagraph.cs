using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IParagraph : IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, ICloneable
	{
		bool IsSource
		{
			get;
		}

		bool IsTarget
		{
			get;
		}

		IParagraphUnit Parent
		{
			get;
			set;
		}

		Direction TextDirection
		{
			get;
			set;
		}
	}
}
