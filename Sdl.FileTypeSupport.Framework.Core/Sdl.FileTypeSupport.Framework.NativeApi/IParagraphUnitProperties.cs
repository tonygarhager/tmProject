using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IParagraphUnitProperties : ICloneable
	{
		ParagraphUnitId ParagraphUnitId
		{
			get;
			set;
		}

		IContextProperties Contexts
		{
			get;
			set;
		}

		LockTypeFlags LockType
		{
			get;
			set;
		}

		ICommentProperties Comments
		{
			get;
			set;
		}

		SourceCount SourceCount
		{
			get;
			set;
		}
	}
}
