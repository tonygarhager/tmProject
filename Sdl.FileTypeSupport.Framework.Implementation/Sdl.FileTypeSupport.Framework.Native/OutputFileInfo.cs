using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class OutputFileInfo : IOutputFileInfo
	{
		private string _filename;

		private string _wildcard;

		private string _fileTypeName;

		private Codepage _encoding;

		private ContentRestriction _contentRestriction;

		public string Filename
		{
			get
			{
				return _filename;
			}
			set
			{
				_filename = value;
			}
		}

		public string FileDialogWildcardExpression
		{
			get
			{
				return _wildcard;
			}
			set
			{
				_wildcard = value;
			}
		}

		public string FileTypeName
		{
			get
			{
				return _fileTypeName;
			}
			set
			{
				_fileTypeName = value;
			}
		}

		public Codepage Encoding
		{
			get
			{
				return _encoding;
			}
			set
			{
				_encoding = value;
			}
		}

		public ContentRestriction ContentRestriction
		{
			get
			{
				return _contentRestriction;
			}
			set
			{
				_contentRestriction = value;
			}
		}

		public OutputFileInfo(ContentRestriction contentRestriction)
		{
			_contentRestriction = contentRestriction;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			OutputFileInfo outputFileInfo = (OutputFileInfo)obj;
			if (_filename != outputFileInfo._filename)
			{
				return false;
			}
			if (_wildcard != outputFileInfo._wildcard)
			{
				return false;
			}
			if (_fileTypeName != outputFileInfo._fileTypeName)
			{
				return false;
			}
			if (_encoding == null != (outputFileInfo._encoding == null))
			{
				return false;
			}
			if (_encoding != null && !_encoding.Equals(outputFileInfo._encoding))
			{
				return false;
			}
			if (_contentRestriction != outputFileInfo._contentRestriction)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_filename != null) ? _filename.GetHashCode() : 0) ^ ((_wildcard != null) ? _wildcard.GetHashCode() : 0) ^ ((_fileTypeName != null) ? _fileTypeName.GetHashCode() : 0) ^ ((_encoding != null) ? _encoding.GetHashCode() : 0) ^ _contentRestriction.GetHashCode();
		}
	}
}
