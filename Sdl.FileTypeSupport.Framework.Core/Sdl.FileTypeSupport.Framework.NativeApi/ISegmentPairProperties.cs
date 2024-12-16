using Sdl.Core.Globalization;
using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ISegmentPairProperties : ICloneable
	{
		SegmentId Id
		{
			get;
			set;
		}

		ITranslationOrigin TranslationOrigin
		{
			get;
			set;
		}

		ConfirmationLevel ConfirmationLevel
		{
			get;
			set;
		}

		bool IsLocked
		{
			get;
			set;
		}
	}
}
