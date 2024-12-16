using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IComment : IMetaDataContainer, ICloneable
	{
		string Text
		{
			get;
			set;
		}

		string Author
		{
			get;
			set;
		}

		string Version
		{
			get;
			set;
		}

		DateTime Date
		{
			get;
			set;
		}

		bool DateSpecified
		{
			get;
			set;
		}

		Severity Severity
		{
			get;
			set;
		}
	}
}
