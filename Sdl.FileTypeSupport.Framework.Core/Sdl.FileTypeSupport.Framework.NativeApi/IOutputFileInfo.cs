using Sdl.Core.Globalization;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IOutputFileInfo
	{
		string Filename
		{
			get;
			set;
		}

		string FileDialogWildcardExpression
		{
			get;
			set;
		}

		string FileTypeName
		{
			get;
			set;
		}

		Codepage Encoding
		{
			get;
			set;
		}

		ContentRestriction ContentRestriction
		{
			get;
		}
	}
}
