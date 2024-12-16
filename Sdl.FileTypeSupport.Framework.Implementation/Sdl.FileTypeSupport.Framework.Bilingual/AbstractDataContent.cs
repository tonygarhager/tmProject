using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public abstract class AbstractDataContent : AbstractMarkupData, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId
	{
		protected AbstractDataContent()
		{
		}

		protected AbstractDataContent(AbstractDataContent other)
			: base(other)
		{
		}
	}
}
