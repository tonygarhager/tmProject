using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public class SharedObjectPublishedEventArgs : EventArgs
	{
		private string _id;

		private object _sharedObject;

		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public object SharedObject
		{
			get
			{
				return _sharedObject;
			}
			set
			{
				_sharedObject = value;
			}
		}

		public SharedObjectPublishedEventArgs(string id, object sharedObject)
		{
			_id = id;
			_sharedObject = sharedObject;
		}
	}
}
