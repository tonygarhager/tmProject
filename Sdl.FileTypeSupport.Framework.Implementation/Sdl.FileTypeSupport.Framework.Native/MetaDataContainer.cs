using System;

namespace Sdl.FileTypeSupport.Framework.Native
{
	internal class MetaDataContainer : AbstractMetaDataContainer, ICloneable
	{
		public MetaDataContainer()
		{
		}

		protected MetaDataContainer(MetaDataContainer other)
			: base(other)
		{
		}

		public object Clone()
		{
			return new MetaDataContainer(this);
		}
	}
}
