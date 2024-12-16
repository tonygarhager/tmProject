using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ICustomInfoProperties : ICloneable
	{
		string NamespaceUri
		{
			get;
			set;
		}

		string ValueXml
		{
			get;
			set;
		}
	}
}
