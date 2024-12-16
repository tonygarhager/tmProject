using Sdl.Core.Globalization;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeOutputFileProperties
	{
		ContentRestriction ContentRestriction
		{
			get;
			set;
		}

		string OutputFilePath
		{
			get;
			set;
		}

		Codepage Encoding
		{
			get;
			set;
		}
	}
}
