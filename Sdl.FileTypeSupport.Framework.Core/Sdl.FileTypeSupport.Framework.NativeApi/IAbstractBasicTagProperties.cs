using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IAbstractBasicTagProperties : IMetaDataContainer, ICloneable
	{
		string DisplayText
		{
			get;
			set;
		}

		string TagContent
		{
			get;
			set;
		}
	}
}
