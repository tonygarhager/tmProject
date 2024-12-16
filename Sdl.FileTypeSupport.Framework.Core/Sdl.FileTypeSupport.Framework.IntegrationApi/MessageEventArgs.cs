using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class MessageEventArgs : EventArgs
	{
		private string _filePath;

		private string _origin;

		private ErrorLevel _level;

		private string _message;

		private IMessageLocation _fromLocation;

		private IMessageLocation _uptoLocation;

		private ExtendedMessageEventData _extendedData;

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
			}
		}

		public string Origin
		{
			get
			{
				return _origin;
			}
			set
			{
				_origin = value;
			}
		}

		public ErrorLevel Level
		{
			get
			{
				return _level;
			}
			set
			{
				_level = value;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		public IMessageLocation FromLocation
		{
			get
			{
				return _fromLocation;
			}
			set
			{
				_fromLocation = value;
			}
		}

		public IMessageLocation UptoLocation
		{
			get
			{
				return _uptoLocation;
			}
			set
			{
				_uptoLocation = value;
			}
		}

		public ExtendedMessageEventData ExtendedData
		{
			get
			{
				return _extendedData;
			}
			set
			{
				_extendedData = value;
			}
		}

		public MessageEventArgs()
		{
		}

		public MessageEventArgs(string filePath, string origin, ErrorLevel level, string message, IMessageLocation fromLocation, IMessageLocation uptoLocation)
			: this(filePath, origin, level, message, fromLocation, uptoLocation, null)
		{
		}

		public MessageEventArgs(string filePath, string origin, ErrorLevel level, string message, IMessageLocation fromLocation, IMessageLocation uptoLocation, ExtendedMessageEventData extendedData)
		{
			_filePath = filePath;
			_origin = origin;
			_level = level;
			_message = message;
			_fromLocation = fromLocation;
			_uptoLocation = uptoLocation;
			_extendedData = extendedData;
		}

		public override string ToString()
		{
			return $"{Level.ToString()}: {Message}";
		}
	}
}
