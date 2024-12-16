using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class NativeOutputFileProperties : INativeOutputFileProperties
	{
		private Codepage _Encoding;

		private ContentRestriction _ContentRestriction = ContentRestriction.Target;

		private string _OutputFilePath;

		public Codepage Encoding
		{
			get
			{
				if (_Encoding == null)
				{
					_Encoding = new Codepage();
				}
				return _Encoding;
			}
			set
			{
				_Encoding = value;
			}
		}

		public ContentRestriction ContentRestriction
		{
			get
			{
				return _ContentRestriction;
			}
			set
			{
				_ContentRestriction = value;
			}
		}

		public string OutputFilePath
		{
			get
			{
				return _OutputFilePath;
			}
			set
			{
				_OutputFilePath = value;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			NativeOutputFileProperties nativeOutputFileProperties = (NativeOutputFileProperties)obj;
			if (_Encoding == null != (nativeOutputFileProperties._Encoding == null))
			{
				return false;
			}
			if (_Encoding != null && !_Encoding.Equals(nativeOutputFileProperties._Encoding))
			{
				return false;
			}
			if (nativeOutputFileProperties._ContentRestriction != _ContentRestriction)
			{
				return false;
			}
			if (nativeOutputFileProperties._OutputFilePath != _OutputFilePath)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_Encoding != null) ? _Encoding.GetHashCode() : 0) ^ _ContentRestriction.GetHashCode() ^ ((_OutputFilePath != null) ? _OutputFilePath.GetHashCode() : 0);
		}
	}
}
