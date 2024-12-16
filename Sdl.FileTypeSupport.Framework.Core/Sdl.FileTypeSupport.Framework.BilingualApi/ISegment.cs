using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ISegment : IAbstractMarker, IAbstractMarkupData, ICloneable, ISupportsUniqueId, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable
	{
		ISegmentPairProperties Properties
		{
			get;
			set;
		}

		IParagraphUnit ParentParagraphUnit
		{
			get;
		}
	}
}
