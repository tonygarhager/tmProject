using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ISubSegmentProperties : ICloneable
	{
		int Length
		{
			get;
			set;
		}

		int StartOffset
		{
			get;
			set;
		}

		IContextProperties Contexts
		{
			get;
			set;
		}
	}
}
