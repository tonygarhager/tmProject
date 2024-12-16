using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ISubSegmentReference : ICloneable
	{
		ISubSegmentProperties Properties
		{
			get;
			set;
		}

		ParagraphUnitId ParagraphUnitId
		{
			get;
			set;
		}
	}
}
